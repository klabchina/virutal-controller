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
using System.Collections.Generic;

namespace Jigbox.AtlasPacker
{
    public class AtlasSpriteData
    {
#region properties

        /// <summary>テクスチャ名</summary>
        public string Name { get; protected set; }

        /// <summary>テクスチャアセットの参照</summary>
        public Texture2D Texture { get; protected set; }

        /// <summary>MultipleSpriteとして記録する際のメタ情報</summary>
        public SpriteMetaData MetaData { get; protected set; }

        /// <summary>テクスチャ内での画像の座標</summary>
        public Rect Texcoord { get; protected set; }

        /// <summary>テクスチャ内での画像の座標(ピクセル)</summary>
        public Rect TextureRect { get; protected set; }

        /// <summary>色を拡張するかどうか</summary>
        public bool IsExpand { get; set; }

        /// <summary>色を拡張するかどうかの元の状態</summary>
        public bool IsExpandDefault { get; protected set; }

        /// <summary>テクスチャをAtlasにパッキングするかどうか</summary>
        public bool IsPack { get; set; }

        /// <summary>新規追加のテクスチャかどうか</summary>
        public bool IsNew { get; set; }

        /// <summary>更新するスプライト情報</summary>
        public AtlasSpriteData UpdateData { get; set; }

        /// <summary>更新するテクスチャかどうか</summary>
        public bool IsUpdated { get { return UpdateData != null; } }

        /// <summary>更新するスプライト情報を保持するかどうか</summary>
        public bool IsHoldUpdateData { get; set; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sprite">スプライトの参照</param>
        /// <param name="metaData">スプライトのメタ情報</param>
        /// <param name="isExpandDefault">色を拡張するかどうか</param>
        public AtlasSpriteData(Sprite sprite, SpriteMetaData metaData, bool isExpandDefault)
        {
            Name = sprite.name;
            Texture = sprite.texture;
            MetaData = metaData;
            float width = Texture.width;
            float height = Texture.height;
            TextureRect = metaData.rect;
            Texcoord = new Rect(
                TextureRect.xMin / width,
                TextureRect.yMin / height,
                TextureRect.width / width,
                TextureRect.height / height);
            IsPack = true;
            IsExpand = isExpandDefault;
            IsExpandDefault = isExpandDefault;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="texture">テクスチャの参照</param>
        public AtlasSpriteData(Texture2D texture)
        {
            Name = texture.name;
            Texture = texture;
            TextureRect = new Rect(0.0f, 0.0f, texture.width, texture.height);
            Texcoord = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            IsPack = true;
            IsNew = true;
            IsExpandDefault = false;
        }

        /// <summary>
        /// パッキングするテクスチャを取得します。
        /// </summary>
        /// <returns></returns>
        public Texture2D GetPackingTexture()
        {
            Texture2D texture;
            if (IsNew)
            {
                texture = !IsExpand ? Texture : GetExpandTexture(Texture);
            }
            else if (IsUpdated)
            {
                texture = !IsExpand ? UpdateData.Texture : GetExpandTexture(UpdateData.Texture);
            }
            else
            {
                texture = GetTextureFromAtlas();
            }

            return texture;
        }

        /// <summary>
        /// Atlasの情報を最新のものに更新します。
        /// </summary>
        /// <param name="texture">Atlasとして扱うテクスチャの参照</param>
        /// <param name="metaData">MultipleSpriteとして記録する際のメタ情報</param>
        public void UpdateAtlas(Texture2D texture, SpriteMetaData metaData)
        {
            Texture = texture;
            MetaData = metaData;
            float width = Texture.width;
            float height = Texture.height;
            TextureRect = metaData.rect;
            Texcoord = new Rect(
                TextureRect.xMin / width,
                TextureRect.yMin / height,
                TextureRect.width / width,
                TextureRect.height / height);
        }

        /// <summary>
        /// テクスチャを読み込み直します。
        /// </summary>
        public void ReloadTexture()
        {
            if (!IsUpdated)
            {
                Texture = AtlasBuilder.ReloadTexture2D(Texture);
            }
            else
            {
                UpdateData.Texture = AtlasBuilder.ReloadTexture2D(UpdateData.Texture);
            }
        }

        /// <summary>
        /// 更新するスプライト情報と比較して、サイズが同じかどうかを返します。
        /// </summary>
        /// <returns></returns>
        public bool CompareSizeUpdateData()
        {
            return TextureRect.width == UpdateData.TextureRect.width
                && TextureRect.height == UpdateData.TextureRect.height;
        }

#endregion

#region protected methods

        /// <summary>
        /// Atlasからスプライトとしてのテクスチャを取得します。
        /// </summary>
        /// <returns></returns>
        protected Texture2D GetTextureFromAtlas()
        {
            int x = Mathf.RoundToInt(TextureRect.x);
            int y = Mathf.RoundToInt(TextureRect.y);
            int width = Mathf.RoundToInt(TextureRect.width);
            int height = Mathf.RoundToInt(TextureRect.height);

            Color[] pixels = Texture.GetPixels(x, y, width, height);

            Texture2D texture;

            if (!IsExpand)
            {
                texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                texture.name = Name;
                texture.SetPixels(pixels);
            }
            else
            {
                texture = GetExpandTexture(pixels, Name, width, height);
            }

            return texture;
        }

        /// <summary>
        /// 色を拡張した状態のテクスチャを取得します。
        /// </summary>
        /// <param name="texture">テクスチャの参照</param>
        /// <returns></returns>
        protected static Texture2D GetExpandTexture(Texture2D texture)
        {
            return GetExpandTexture(texture.GetPixels(), texture.name, texture.width, texture.height);
        }

        /// <summary>
        /// 色を拡張した状態のテクスチャを取得します。
        /// </summary>
        /// <param name="pixels">テクスチャの色情報</param>
        /// <param name="name">テクスチャ名</param>
        /// <param name="width">テクスチャの横幅</param>
        /// <param name="height">テクスチャの縦幅</param>
        /// <returns></returns>
        protected static Texture2D GetExpandTexture(Color[] pixels, string name, int width, int height)
        {
            // 色情報を拡張する場合、外周1pixel分大きいサイズのテクスチャに色情報を複写する
            // この時、外周分の色情報は、元となるテクスチャの端のピクセルから取得した色で埋める

            int expandWidth = width + 2;
            int expandHeight = height + 2;

            Color[] expand = new Color[expandWidth * expandHeight];

            for (int i = 0; i < expandHeight; ++i)
            {
                int heightIndex = Mathf.Clamp(i - 1, 0, height - 1);
                for (int j = 0; j < expandWidth; ++j)
                {
                    int widthIndex = Mathf.Clamp(j - 1, 0, width - 1);
                    expand[i * expandWidth + j] = pixels[heightIndex * width + widthIndex];
                }
            }

            Texture2D texture = new Texture2D(expandWidth, expandHeight, TextureFormat.ARGB32, false);
            texture.name = name;
            texture.SetPixels(expand);

            return texture;
        }

#endregion
    }
}
