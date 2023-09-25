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
using System.Linq;

namespace Jigbox.AtlasPacker
{
    public class PackingSpriteList
    {
#region constants

        /// <summary>フラグのレイアウト上の横幅</summary>
        protected static readonly float FlagLayoutWidth = 45.0f;

        /// <summary>テクスチャ名のレイアウト上の横幅</summary>
        protected static readonly float NameLayoutWidth = 180.0f;

        /// <summary>テクスチャのプレビューのレイアウト上の横幅</summary>
        protected static readonly float TextureLayoutWidth = 50.0f;

        /// <summary>テクスチャの状態のレイアウト上の横幅</summary>
        protected static readonly float StatusLayoutWidth = 55.0f;

        /// <summary>テクスチャのプレビューサイズ</summary>
        protected static readonly float TexturePreviewSize = 32.0f;

        /// <summary>テクスチャが何も変化しない場合の文言</summary>
        protected static readonly string StatusNone = "-";

        /// <summary>テクスチャが追加される場合の文言</summary>
        protected static readonly string StatusAdd = "Add";

        /// <summary>テクスチャが破棄される場合の文言</summary>
        protected static readonly string StatusRemove = "Remove";

        /// <summary>テクスチャが更新される場合の文言</summary>
        protected static readonly string StatusUpdate = "Update";

#endregion

#region properties

        /// <summary>Atlasnに含まれているスプライト情報</summary>
        protected List<AtlasSpriteData> sprites = new List<AtlasSpriteData>();

        /// <summary>Atlasnに含まれているスプライト情報</summary>
        public IEnumerable<AtlasSpriteData> Sprites { get { return sprites.Where(sprite => sprite.IsPack); } }

        /// <summary>現在選択中にある追加予定のスプライト情報</summary>
        protected List<AtlasSpriteData> selectedSprites = new List<AtlasSpriteData>();

        /// <summary>選択、編集状態が保存された追加予定のスプライト情報</summary>
        protected List<AtlasSpriteData> editingSprites = new List<AtlasSpriteData>();

        /// <summary>追加予定のスプライト情報</summary>
        public IEnumerable<AtlasSpriteData> NewSprites
        {
            get
            {
                List<AtlasSpriteData> temp = new List<AtlasSpriteData>();
                temp.AddRange(selectedSprites.Where(sprite => sprite.IsPack));
                temp.AddRange(editingSprites.Where(sprite => sprite.IsPack));
                return temp;
            }
        }

        /// <summary>前回の選択状態</summary>
        protected Object[] lastSelection = null;

        /// <summary>スプライト情報の表示のスクロール量</summary>
        protected Vector2 scrollPosition = Vector2.zero;

        /// <summary>スプライト情報が編集されているかどうか</summary>
        public bool HasEdited
        {
            get
            {
                if (sprites.Count == 0 && selectedSprites.Count == 0 && editingSprites.Count == 0)
                {
                    return false;
                }

                if (selectedSprites.Count > 0 || editingSprites.Count > 0)
                {
                    return true;
                }

                for (int i = 0; i < sprites.Count; ++i)
                {
                    if (!sprites[i].IsPack
                        || sprites[i].IsUpdated
                        || sprites[i].IsExpand != sprites[i].IsExpandDefault)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>追加予定のスプライトがあるかどうか</summary>
        public bool HasNewSprites
        {
            get
            {
                if (selectedSprites.Count > 0)
                {
                    return true;
                }

                for (int i = 0; i < sprites.Count; ++i)
                {
                    if (sprites[i].IsUpdated && !sprites[i].IsHoldUpdateData)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

#endregion

#region public methods

        /// <summary>
        /// スプライト情報のリストを初期化します。
        /// </summary>
        /// <param name="sprites">Atlasに含まれているスプライト情報</param>
        public void InitList(List<AtlasSpriteData> sprites)
        {
            this.sprites = sprites;
            selectedSprites.Clear();
            editingSprites.Clear();
        }

        /// <summary>
        /// スプライト情報のリストをクリアします。
        /// </summary>
        public void ClearList()
        {
            sprites.Clear();
            selectedSprites.Clear();
            editingSprites.Clear();
            lastSelection = null;
        }

        /// <summary>
        /// 現在選択中にある追加予定のスプライト情報を編集情報として保存します。
        /// </summary>
        public void SaveEditingTextures()
        {
            editingSprites.AddRange(selectedSprites);
            selectedSprites.Clear();
            
            for (int i = 0; i < sprites.Count; ++i)
            {
                if (sprites[i].IsUpdated)
                {
                    sprites[i].IsHoldUpdateData = true;
                }
            }
        }

        /// <summary>
        /// 選択状態にあるテクスチャの状態を更新します。
        /// </summary>
        /// <returns>選択状態が変化していれば<c>true</c>を返します。</returns>
        public bool UpdateSelectTextures()
        {
            Object[] objects = Selection.objects;
            if (lastSelection == objects)
            {
                return false;
            }

            lastSelection = objects;

            List<AtlasSpriteData> selectSprites = new List<AtlasSpriteData>();

            for (int i = 0; i < sprites.Count; ++i)
            {
                if (!sprites[i].IsHoldUpdateData)
                {
                    sprites[i].UpdateData = null;
                }
            }

            for (int i = 0; i < objects.Length; ++i)
            {
                Texture2D texture = objects[i] as Texture2D;

                if (texture == null)
                {
                    continue;
                }

                // 選択状態が保存されたテクスチャに存在する場合は無視
                AtlasSpriteData spriteData = editingSprites.Where(data => data.Name == texture.name).FirstOrDefault();
                if (spriteData != null)
                {
                    continue;
                }

                // アトラス自体に含まれている
                spriteData = sprites.Where(data => data.Name == texture.name).FirstOrDefault();
                if (spriteData != null)
                {
                    // 選択状態が保存されている場合は無視
                    if (!spriteData.IsHoldUpdateData)
                    {
                        spriteData.UpdateData = new AtlasSpriteData(texture);
                    }
                    continue;
                }

                // 既に選択済みのテクスチャに存在する
                spriteData = selectedSprites.Where(data => data.Name == texture.name).FirstOrDefault();
                if (spriteData != null)
                {
                    selectSprites.Add(spriteData);
                    continue;
                }

                TextureImporter importer = AtlasBuilder.GetTextureImporter(texture);
                if (importer.spriteImportMode != SpriteImportMode.Single
                    && importer.spriteImportMode != SpriteImportMode.None)
                {
                    continue;
                }

                selectSprites.Add(new AtlasSpriteData(texture));
            }

            selectedSprites = selectSprites;
            return true;
        }

        /// <summary>
        /// スプライト情報のリストのヘッダーを表示します。
        /// </summary>
        public void DrawHeader()
        {
            if (sprites.Count == 0 && selectedSprites.Count == 0 && editingSprites.Count == 0)
            {
                EditorGUILayout.HelpBox("画像が選択されていません。", MessageType.Info);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                // パッキングするかどうか
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("Pack", GUILayout.Width(FlagLayoutWidth));
                }
                EditorGUILayout.EndHorizontal();

                // 色を拡張するかどうか
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("Expand", GUILayout.Width(FlagLayoutWidth));
                }
                EditorGUILayout.EndHorizontal();

                // テクスチャのプレビュー
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("Texture", GUILayout.Width(TextureLayoutWidth));
                }
                EditorGUILayout.EndHorizontal();

                // テクスチャ名
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("Name", GUILayout.Width(NameLayoutWidth));
                }
                EditorGUILayout.EndHorizontal();

                // 状態
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("Status", GUILayout.Width(StatusLayoutWidth));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// スプライト情報のリストを表示します。
        /// </summary>
        public void DrawSpriteList()
        {
            if (sprites.Count == 0 && selectedSprites.Count == 0 && editingSprites.Count == 0)
            {
                return;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            {
                DrawList(selectedSprites);
                DrawList(editingSprites);
                DrawList(sprites);
            }
            GUILayout.EndScrollView();
        }

#endregion

#region protected methods

        /// <summary>
        /// スプライト象法のリストを表示します。
        /// </summary>
        /// <param name="sprites">表示するスプライト情報のリスト</param>
        protected virtual void DrawList(List<AtlasSpriteData> sprites)
        {
            for (int i = 0; i < sprites.Count; ++i)
            {
                AtlasSpriteData data = sprites[i];
                SetBackColor(data);

                EditorGUILayout.BeginHorizontal();
                {
                    // パッキングするかどうか
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        data.IsPack = EditorGUILayout.Toggle("", data.IsPack, GUILayout.Width(FlagLayoutWidth), GUILayout.Height(TexturePreviewSize));
                    }
                    EditorGUILayout.EndHorizontal();

                    // 色を拡張するかどうか
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        data.IsExpand = EditorGUILayout.Toggle("", data.IsExpand, GUILayout.Width(FlagLayoutWidth), GUILayout.Height(TexturePreviewSize));
                    }
                    EditorGUILayout.EndHorizontal();

                    // テクスチャのプレビュー
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        DrawTexture(data);
                    }
                    EditorGUILayout.EndHorizontal();

                    // テクスチャ名
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        EditorGUILayout.LabelField(data.Name, GUILayout.Width(NameLayoutWidth), GUILayout.Height(TexturePreviewSize));
                    }
                    EditorGUILayout.EndHorizontal();

                    // 状態
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        EditorGUILayout.LabelField(GetStatus(data), GUILayout.Width(StatusLayoutWidth), GUILayout.Height(TexturePreviewSize));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();
            }

            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// テクスチャのプレビューを表示します。
        /// </summary>
        /// <param name="spriteData">スプライト情報</param>
        protected void DrawTexture(AtlasSpriteData spriteData)
        {
            // 幅に2足しているのはレイアウト上の誤差の補正
            Rect rect = GUILayoutUtility.GetRect(TextureLayoutWidth + 2, TexturePreviewSize, GUILayout.ExpandWidth(false));
            Vector2 center = rect.center;
            rect.xMin = center.x - (TexturePreviewSize / 2.0f);
            rect.xMax = center.x + (TexturePreviewSize / 2.0f);
            GUI.Box(rect, "");

            AtlasSpriteData viewData = !spriteData.IsUpdated ? spriteData : spriteData.UpdateData;

            // アス比に合わせて表示を補正
            float aspect = viewData.TextureRect.width / viewData.TextureRect.height;
            if (aspect != 1.0f)
            {
                // 横長
                if (aspect > 1.0f)
                {
                    float padding = rect.height * (1.0f - 1.0f / aspect) * 0.5f;
                    rect.yMin += padding;
                    rect.yMax -= padding;
                }
                // 縦長
                else
                {
                    float padding = rect.width * (1.0f - aspect) * 0.5f;
                    rect.xMin += padding;
                    rect.xMax -= padding;
                }
            }

            if (viewData.Texture != null)
            {
                GUI.DrawTextureWithTexCoords(rect, viewData.Texture, viewData.Texcoord);
            }
        }

        /// <summary>
        /// 要素の背景色を設定します。
        /// </summary>
        /// <param name="spriteData">スプライト情報</param>
        protected void SetBackColor(AtlasSpriteData spriteData)
        {
            if (!spriteData.IsPack)
            {
                GUI.backgroundColor = Color.red;
                return;
            }
            if (spriteData.IsNew)
            {
                GUI.backgroundColor = Color.green;
                return;
            }
            if (spriteData.IsUpdated)
            {
                GUI.backgroundColor = Color.cyan;
                return;
            }

            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// 要素の状態を取得します。
        /// </summary>
        /// <param name="spriteData">スプライト情報</param>
        /// <returns></returns>
        protected string GetStatus(AtlasSpriteData spriteData)
        {
            if (spriteData.IsNew)
            {
                return spriteData.IsPack ? StatusAdd : StatusNone;
            }
            if (!spriteData.IsPack)
            {
                return StatusRemove;
            }
            if (spriteData.IsUpdated)
            {
                return StatusUpdate;
            }

            return StatusNone;
        }

#endregion
    }
}
