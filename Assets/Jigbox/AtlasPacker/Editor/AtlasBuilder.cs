/**
 * Jigbox
 * Copyright(c) 2016 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 *
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications 
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Jigbox.EditorUtils;

namespace Jigbox.AtlasPacker
{
    public class AtlasBuilder
    {
#region properties

        /// <summary>Atlasの設定</summary>
        protected AtlasPackerSettings settings;

        /// <summary>Atlasとして扱うTextureの参照</summary>
        protected Texture2D atlas;

        /// <summary>Atlasに紐づくパッキング用の情報</summary>
        protected AtlasCustomInfo customInfo;

        /// <summary>Atlasに既にパッキングされているスプライトの情報</summary>
        protected IEnumerable<AtlasSpriteData> sprites;

        /// <summary>新しく追加するスプライトの情報</summary>
        protected IEnumerable<AtlasSpriteData> newSprites;

        /// <summary>Atlasに既にパッキングされているスプライトの数</summary>
        protected int spriteCount = 0;

        /// <summary>新しく追加するスプライトの数</summary>
        protected int newSpriteCount = 0;

        /// <summary>Atlasの色の圧縮形式</summary>
        protected TextureImporterCompression compression = TextureImporterCompression.Uncompressed;

        /// <summary>Atlasの色情報を読み出せるかどうかのキャッシュ</summary>
        protected bool isReadableAtlas = false;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">Atlasの設定</param>
        public AtlasBuilder(AtlasPackerSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Atlasを新しく生成します。
        /// </summary>
        /// <param name="assetPath">テクスチャアセットとして保存するパス</param>
        public void CreateNewAtlas(string assetPath)
        {
            atlas = new Texture2D(8192, 8192, TextureFormat.ARGB32, false);
            AssetDatabase.CreateAsset(atlas, "Assets/" + assetPath);
        }

        /// <summary>
        /// Atlas情報をクリアします。
        /// </summary>
        public void ClearAtlas() 
        {
            atlas = null;
            customInfo = null;
            sprites = null;
            newSprites = null;
            spriteCount = 0;
            newSpriteCount = 0;
        }

        /// <summary>
        /// Atlasからスプライト情報を読み込みます。
        /// </summary>
        /// <param name="atlas"></param>
        /// <returns></returns>
        public List<AtlasSpriteData> LoadSprites(Texture2D atlas)
        {
            this.atlas = atlas;

            UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(atlas));
            TextureImporter textureImporter = GetTextureImporter(atlas);
            SpriteMetaData[] spritesheet = textureImporter.spritesheet;

            customInfo = AtlasCustomInfo.Load(atlas);
            if (customInfo != null)
            {
                settings.SetPaddingFromAtlas(customInfo.Padding);
            }

            List<AtlasSpriteData> sprites = new List<AtlasSpriteData>();

            for (int i = 1; i < objects.Length; ++i)
            {
                Sprite sprite = objects[i] as Sprite;
                SpriteMetaData metaData = new SpriteMetaData();
                for (int j = 0; j < spritesheet.Length; ++j)
                {
                    if (spritesheet[j].name == sprite.name)
                    {
                        metaData = spritesheet[j];
                        break;
                    }
                }
                bool isExpand = customInfo != null ? customInfo.IsExpanded(sprite.name) : false;

                AtlasSpriteData data = new AtlasSpriteData(sprite, metaData, isExpand);
                sprites.Add(data);
            }

            return sprites;
        }

        /// <summary>
        /// Atlasを保存します。
        /// </summary>
        /// <param name="sprites">Atlasに既にパッキングされているスプライトの情報</param>
        /// <param name="newSprites">新しく追加するスプライトの情報</param>
        /// <param name="onComplete">Atlasの保存が完了した際のコールバック</param>
        /// <param name="onFailed">Atlasが保存できない場合のコールバック</param>
        public void SaveSprites(
            IEnumerable<AtlasSpriteData> sprites,
            IEnumerable<AtlasSpriteData> newSprites,
            Action<Texture2D> onComplete,
            Action<bool> onFailed)
        {
            this.sprites = sprites;
            this.newSprites = newSprites;

            spriteCount = GetPackingTextureCount(sprites);
            newSpriteCount = GetPackingTextureCount(newSprites);

            if (ValidatePacking(onFailed))
            {
                return;
            }

            // AtlasPackerを表示しならがスプライト情報を編集した場合、
            // キャッシュされている情報が古くなってしまうので、
            // パッキング前に最新の情報に更新する
            if (atlas != null)
            {
                TextureImporter textureImporter = GetTextureImporter(atlas);
                SpriteMetaData[] spritesheet = textureImporter.spritesheet;
                
                foreach (AtlasSpriteData sprite in sprites)
                {
                    for (int i = 0; i < spritesheet.Length; ++i)
                    {
                        if (spritesheet[i].name == sprite.Name)
                        {
                            sprite.UpdateAtlas(atlas, spritesheet[i]);
                            break;
                        }
                    }
                }
            }

#pragma warning disable 219
            EditorSerialProcessor processor = new EditorSerialProcessor(
                null,
                // パッキングするテクスチャ、Atlasの設定を必要なフォーマットに変換
                PreparePacking,
                // パッキング
                PackSprites,
                // Atlasの元の設定を復元
                UndoAtlasSettings,
                // 完了
                _ =>
                {
                    sprites = null;
                    newSprites = null;
                    spriteCount = 0;
                    newSpriteCount = 0;

                    if (onComplete != null)
                    {
                        onComplete(atlas);
                    }
                    return null;
                });
#pragma warning restore 219
        }

        /// <summary>
        /// Textureのインポート情報を取得します。
        /// </summary>
        /// <param name="texture">テクスチャの参照</param>
        /// <returns></returns>
        public static TextureImporter GetTextureImporter(Texture2D texture)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            return assetImporter as TextureImporter;
        }

        /// <summary>
        /// テクスチャアセットを読み込み直します。
        /// </summary>
        /// <param name="texture">テクスチャの参照</param>
        /// <returns></returns>
        public static Texture2D ReloadTexture2D(Texture2D texture)
        {
            return AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(texture), typeof(Texture2D)) as Texture2D;
        }

#endregion

#region protected methods

        /// <summary>
        /// パッキングするデータやAtlasの状態が正しいかどうかを確認します。
        /// </summary>
        /// <param name="onFailed">Atlasが保存できない場合のコールバック</param>
        /// <returns>データが全て正しければ<c>true</c>を返します。</returns>
        protected bool ValidatePacking(Action<bool> onFailed)
        {
            // パッキングするスプライトが一枚もない
            if (spriteCount + newSpriteCount == 0)
            {
                Debug.LogError("Can't packing! Target sprites are not exist!");
                if (onFailed != null)
                {
                    onFailed(false);
                }
                return true;
            }
            // 既存のAtlasを編集する場合
            if (spriteCount > 0)
            {
                // Atlasを途中で削除した場合
                if (atlas == null)
                {
                    Debug.LogError("Can't packing! Target atlas is deleted!");
                    if (onFailed != null)
                    {
                        onFailed(true);
                    }
                    return true;
                }
                // Atlasの名前を途中で変更した場合
                if (atlas.name != settings.AtlasName)
                {
                    Debug.LogError("Can't packing! Target atlas's name is changed!");
                    if (onFailed != null)
                    {
                        onFailed(true);
                    }
                    return true;
                }
                // Atlasのディレクトリを変更した場合
                if (!AssetDatabase.GetAssetPath(atlas).Contains(settings.ExportDirectory + "/" + settings.AtlasName))
                {
                    Debug.LogError("Can't packing! Target atlas is moved another directory!");
                    if (onFailed != null)
                    {
                        onFailed(true);
                    }
                    return true;
                }
            }
            // 新規で作成する場合
            else
            {
                if (Directory.Exists(Application.dataPath + "/" + settings.ExportDirectory))
                {
                    // 既に同名のアセットが存在する場合
                    if (AssetDatabase.FindAssets(
                        settings.AtlasName,
                        new string[] { "Assets/" + settings.ExportDirectory }).Length != 0)
                    {
                        Debug.LogError("Can't packing! Already exist same name asset!");
                        if (onFailed != null)
                        {
                            onFailed(false);
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// パッキング前の準備を行います。
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected object PreparePacking(object argument)
        {
            // 更新するスプライトのフォーマットを変更
            foreach (AtlasSpriteData sprite in sprites)
            {
                if (!sprite.IsUpdated)
                {
                    continue;
                }
                ConvertTextureToSprite(sprite.UpdateData.Texture);
            }

            // 追加するスプライトのフォーマットを変更
            foreach (AtlasSpriteData sprite in newSprites)
            {
                ConvertTextureToSprite(sprite.Texture);
            }

            // Atlasのフォーマットを変更
            if (atlas != null)
            {
                TextureImporter textureImporter = GetTextureImporter(atlas);
                bool isDirty = false;

                compression = textureImporter.textureCompression;
                if (textureImporter.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                    isDirty = true;
                }

                isReadableAtlas = textureImporter.isReadable;
                if (!textureImporter.isReadable)
                {
                    textureImporter.isReadable = true;
                    isDirty = true;
                }

                if (isDirty)
                {
                    EditorUtility.SetDirty(atlas);
                    EditorUtility.SetDirty(textureImporter);
                    textureImporter.SaveAndReimport();
                }
            }

            return null;
        }

        /// <summary>
        /// スプライトをパッキングします。
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected object PackSprites(object argument)
        {
            bool isNewAtlas = atlas == null;

            Texture2D[] textures = GenerateTextureArray();

            Texture2D newAtlas = new Texture2D(8192, 8192, TextureFormat.ARGB32, false);
            Rect[] texcoords = newAtlas.PackTextures(textures, settings.Padding);
            byte[] bytes = newAtlas.EncodeToPNG();
            int atlasWidth = newAtlas.width;
            int atlasHeight = newAtlas.height;

            string atlasPath = settings.ExportDirectory + "/" + settings.AtlasName;
            
            string directoryPath = Application.dataPath + "/" + settings.ExportDirectory;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            File.WriteAllBytes(Application.dataPath + "/" + atlasPath + ".png", bytes);

            // 新規Atlasの場合は、出力したデータをインポートする
            if (isNewAtlas)
            {
                string assetPath = "Assets/" + atlasPath + ".png";
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
                atlas = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
            }

            if (customInfo == null)
            {
                customInfo = AtlasCustomInfo.Create(atlas.name, "Assets/" + settings.ExportDirectory);
            }

            // Atlasとしての基本設定
            TextureImporter textureImporter = GetTextureImporter(atlas);
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            if (isNewAtlas)
            {
                textureImporter.npotScale = TextureImporterNPOTScale.None;
                textureImporter.alphaIsTransparency = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
            }

            // 新しいスプライトの情報を設定する
            textureImporter.spritesheet = GetSpriteSheet(texcoords, atlasWidth, atlasHeight);

            EditorUtility.SetDirty(atlas);
            EditorUtility.SetDirty(textureImporter);
            textureImporter.SaveAndReimport();

            UpdateCustomInfo();
            EditorUtility.SetDirty(customInfo);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(customInfo));

            return null;
        }

        /// <summary>
        /// パッキング時に変更したAtlasの設定を元に戻します。
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected object UndoAtlasSettings(object argument)
        {
            TextureImporter textureImporter = GetTextureImporter(atlas);
            bool isDirty = false;

            if (textureImporter.isReadable != isReadableAtlas)
            {
                textureImporter.isReadable = isReadableAtlas;
                isDirty = true;
            }

            // フルカラーで出力しない設定の場合は、フォーマットを元に戻す
            if (!settings.ForceExportFullcolor)
            {
                if (textureImporter.textureCompression != compression)
                {
                    textureImporter.textureCompression = compression;
                    isDirty = true;
                }
            }

            if (isDirty)
            {
                EditorUtility.SetDirty(textureImporter);
                textureImporter.SaveAndReimport();
            }
            return null;
        }

        /// <summary>
        /// パッキングするスプライトからテクスチャ配列を生成します。
        /// </summary>
        /// <returns></returns>
        protected Texture2D[] GenerateTextureArray()
        {
            Texture2D[] textures = new Texture2D[spriteCount + newSpriteCount];

            int index = 0;
            foreach (AtlasSpriteData sprite in sprites)
            {
                textures[index] = sprite.GetPackingTexture();
                ++index;
            }

            foreach (AtlasSpriteData sprite in newSprites)
            {
                textures[index] = sprite.GetPackingTexture();
                ++index;
            }

            return textures;
        }

        /// <summary>
        /// Atlasに保存するスプライト情報を取得します。
        /// </summary>
        /// <param name="texcoords">Atlasにパッキングしたスプライトの配置情報</param>
        /// <param name="atlasWidth">Atlasの横幅</param>
        /// <param name="atlasHeight">Atlasの縦幅</param>
        /// <returns></returns>
        protected SpriteMetaData[] GetSpriteSheet(Rect[] texcoords, int atlasWidth, int atlasHeight)
        {
            SpriteMetaData[] spritesheet = new SpriteMetaData[spriteCount + newSpriteCount];
            int index = 0;

            // 元々パッキングされている or 更新分
            foreach (AtlasSpriteData spriteData in sprites)
            {
                SpriteMetaData metaData;
                if (!spriteData.IsUpdated || spriteData.CompareSizeUpdateData())
                {
                    metaData = spriteData.MetaData;
                }
                else
                {
                    metaData = spriteData.UpdateData.MetaData;
                    metaData.name = spriteData.Name;
                }

                Rect texcoord = texcoords[index];
                metaData.rect = GetRectFromAtlas(texcoord, atlasWidth, atlasHeight, spriteData.IsExpand);

                spritesheet[index] = metaData;
                ++index;
            }
            // 新規分
            foreach (AtlasSpriteData spriteData in newSprites)
            {
                SpriteMetaData metaData = spriteData.MetaData;
                metaData.name = spriteData.Name;

                Rect texcoord = texcoords[index];
                metaData.rect = GetRectFromAtlas(texcoord, atlasWidth, atlasHeight, spriteData.IsExpand);

                spritesheet[index] = metaData;
                ++index;
            }

            return spritesheet;
        }

        /// <summary>
        /// Atlasに紐づくパッキング情報を更新します。
        /// </summary>
        protected void UpdateCustomInfo()
        {
            List<string> expandedSprites = new List<string>();
            expandedSprites.AddRange(sprites.Where(sprite => sprite.IsExpand).Select(sprite => sprite.Name));
            expandedSprites.AddRange(newSprites.Where(sprite => sprite.IsExpand).Select(sprite => sprite.Name));

            customInfo.UpdateExpandedSprites(expandedSprites);
            customInfo.Padding = settings.Padding;
        }

        /// <summary>
        /// パッキングするスプライトの数を取得します。
        /// </summary>
        /// <param name="sprites">スプライト情報</param>
        /// <returns></returns>
        protected static int GetPackingTextureCount(IEnumerable<AtlasSpriteData> sprites)
        {
            int count = 0;
#pragma warning disable 219
            foreach (AtlasSpriteData sprite in sprites)
            {
                ++count;
            }
#pragma warning restore 219
            return count;
        }

        /// <summary>
        /// テクスチとして保持されているテクスチャアセットをスプライトの形式に変換します。
        /// </summary>
        /// <param name="texture">変換するテクスチャアセット</param>
        /// <returns></returns>
        protected static bool ConvertTextureToSprite(Texture2D texture)
        {
            bool isDirty = false;

            TextureImporter textureImporter = GetTextureImporter(texture);

            // Sprite状態
            if (textureImporter.textureType != TextureImporterType.Sprite
                || textureImporter.spriteImportMode == SpriteImportMode.None)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                isDirty = true;
            }

            // フルカラー
            if (textureImporter.textureCompression != TextureImporterCompression.Uncompressed)
            {
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                isDirty = true;
            }

            // アルファを有効
            if (textureImporter.alphaSource == TextureImporterAlphaSource.None)
            {
                textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
            }

            // NPOT許容
            if (textureImporter.npotScale != TextureImporterNPOTScale.None)
            {
                textureImporter.npotScale = TextureImporterNPOTScale.None;
                isDirty = true;
            }

            // ピクセル読み出し可
            if (!textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                isDirty = true;
            }

            // Alpha Is Transparency有効
            if (!textureImporter.alphaIsTransparency)
            {
                textureImporter.alphaIsTransparency = true;
                isDirty = true;
            }

            // ミップマップ無効
            if (textureImporter.mipmapEnabled)
            {
                textureImporter.mipmapEnabled = false;
                isDirty = true;
            }

            if (isDirty)
            {
                EditorUtility.SetDirty(texture);
                EditorUtility.SetDirty(textureImporter);
                textureImporter.SaveAndReimport();
            }

            return isDirty;
        }

        /// <summary>
        /// Atlas内におけるスプライトの位置を取得します。
        /// </summary>
        /// <param name="texcoord">テクスチャ座標</param>
        /// <param name="atlasWidth">Atlasの横幅</param>
        /// <param name="atlasHeight">Atlasの縦幅</param>
        /// <param name="isExpand">色を拡張しているかどうか</param>
        /// <returns></returns>
        protected static Rect GetRectFromAtlas(Rect texcoord, int atlasWidth, int atlasHeight, bool isExpand)
        {
            Rect textureRect = new Rect(
                Mathf.RoundToInt(texcoord.x * atlasWidth),
                Mathf.RoundToInt(texcoord.y * atlasHeight),
                Mathf.RoundToInt(texcoord.width * atlasWidth),
                Mathf.RoundToInt(texcoord.height * atlasHeight));

            // 色を拡張している場合、本来のテクスチャに外周1pixel分追加した状態でパッキングするので
            // その分のズレが発生するので、本来のテクスチャ領域と同じになるように補正する
            if (isExpand)
            {
                textureRect.x += 1;
                textureRect.y += 1;
                textureRect.width -= 2;
                textureRect.height -= 2;
            }

            return textureRect;
        }

#endregion
    }
}
