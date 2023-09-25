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

namespace Jigbox.AtlasPacker
{
    public class AtlasPackerWindow : EditorWindow
    {
#region properties

        /// <summary>Atlasの設定</summary>
        protected AtlasPackerSettings settings;

        /// <summary>Atlas生成モジュール</summary>
        protected AtlasBuilder builder;

        /// <summary>Atlas化するスプライトのリスト</summary>
        protected PackingSpriteList spriteList;

        /// <summary>何らかの編集が行われているかどうか</summary>
        public bool HasEdited { get { return settings.HasEdited || spriteList.HasEdited; } }

#endregion

#region public methods

        /// <summary>
        /// 確認ダイアログを表示します。
        /// </summary>
        /// <param name="message">ダイアログに表示するメッセージ</param>
        /// <param name="allowEvent">OKを押した際のコールバック</param>
        public virtual void ShowConfirm(string message, Action allowEvent)
        {
            AtlasPackerWindow window = GetWindow(typeof(AtlasPackerWindow)) as AtlasPackerWindow;
            AtlasEditorConfirmWindow.OpenWindow(message, window.position, allowEvent);
        }

        /// <summary>
        /// 編集するAtlasを設定します。
        /// </summary>
        /// <param name="atlas">Atlas</param>
        public virtual void SetAtlas(Texture2D atlas)
        {
            Selection.activeObject = atlas;
            spriteList.ClearList();
            spriteList.InitList(builder.LoadSprites(atlas));
        }

        /// <summary>
        /// 編集中のAtlasをクリアします。
        /// </summary>
        public virtual void ClearAtlas()
        {
            Selection.activeObject = null;
            builder.ClearAtlas();
            spriteList.ClearList();
        }

#endregion

#region protected methods

        /// <summary>
        /// ボタンを表示します。
        /// </summary>
        protected virtual void DrawFooterButton()
        {
            if (!settings.HasAtlas && !spriteList.HasEdited)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                EditorGUI.BeginDisabledGroup(!spriteList.HasNewSprites);
                {
                    if (GUILayout.Button("選択状態を保持", GUILayout.Width(100.0f)))
                    {
                        spriteList.SaveEditingTextures();
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!HasEdited || string.IsNullOrEmpty(settings.AtlasName));
                {
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Atlasを生成"))
                    {
                        builder.SaveSprites(
                            spriteList.Sprites,
                            spriteList.NewSprites,
                            atlas =>
                            {
                                Selection.activeObject = atlas;
                                settings.SaveCommonSettings();
                                settings.SetTemporarySetting(atlas);
                                settings.ApplyTemporarySetting();
                            },
                            isClear =>
                            {
                                if (isClear)
                                {
                                    Selection.activeObject = null;
                                    settings.SetTemporarySetting("NewAtlas");
                                    settings.ApplyTemporarySetting();
                                }
                            });
                    }
                    GUI.backgroundColor = Color.white;
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// ドラッグアンドドロップ操作を受け付けます。
        /// </summary>
        protected virtual void AcceptDragAndDrop()
        {
            Event current = Event.current;

            switch (current.type)
            {
                // ドロップされた
                case EventType.DragPerform:
                    if (DragAndDrop.objectReferences.Length != 1)
                    {
                        return;
                    }
                    Texture2D texture = DragAndDrop.objectReferences[0] as Texture2D;
                    if (texture != null)
                    {
                        TextureImporter textureImporter = AtlasBuilder.GetTextureImporter(texture);
                        if (textureImporter.textureType != TextureImporterType.Sprite)
                        {
                            Debug.LogWarning("Spriteではないテクスチャは、Atlasとして認識できません。");
                            return;
                        }

                        // Atlasとして有効なオブジェクトをドラッグアンドドロップした場合のみ
                        if (textureImporter.spriteImportMode == SpriteImportMode.Multiple)
                        {
                            settings.SetTemporarySetting(texture);

                            if (HasEdited)
                            {
                                ShowConfirm("編集中の情報は破棄されますが、よろしいですか？",
                                    () =>
                                    {
                                        settings.ApplyTemporarySetting();
                                    });
                            }
                            else
                            {
                                settings.ApplyTemporarySetting();
                            }
                        }
                        else
                        {
                            Debug.LogWarning("MultipleSpriteではないテクスチャは、Atlasとして認識できません。");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("テクスチャ以外のオブジェクトは、Atlasとして認識できません。");
                    }
                    break;
                // ウィンドウ上でのドラッグ中
                case EventType.DragUpdated:
                    if (DragAndDrop.objectReferences.Length == 1)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    }
                    // 複数個のドラッグは受け付けないので、表示を切り替える
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                    break;
            }
        }

#endregion

#region private methods

        /// <summary>
        /// Atlasの編集ウィンドウを開きます。
        /// </summary>
        [MenuItem("Window/Jigbox/Atlas Packer")]
        static AtlasPackerWindow Open()
        {
            AtlasPackerWindow window = GetWindow(typeof(AtlasPackerWindow)) as AtlasPackerWindow;
            window.titleContent.text = "Atlas Packer";
            window.minSize = new Vector2(455.0f, 330.0f);
            return window;
        }

        /// <summary>
        /// 選択されているAtlasから編集ウィンドウを開きます。
        /// </summary>
        [MenuItem("Assets/Jigbox/Edit Atlas")]
        static void OpenWithAtlas()
        {
            AtlasPackerWindow window = Open();

            // 別メソッドでチェックが通っていないとこのメソッドは呼び出せないので、
            // ここでのチェックは冗長になるので省く
            Texture2D texture = Selection.objects[0] as Texture2D;
            window.settings.SetTemporarySetting(texture);

            if (window.HasEdited)
            {
                window.ShowConfirm("編集中の情報は破棄されますが、よろしいですか？",
                    () =>
                    {
                        window.settings.ApplyTemporarySetting();
                    });
            }
            else
            {
                window.settings.ApplyTemporarySetting();
            }
        }

        /// <summary>
        /// 選択されている対象がAtlasかどうかを確認します。
        /// </summary>
        [MenuItem("Assets/Jigbox/Edit Atlas", true)]
        static bool ValidateOpenWithAtlas()
        {
            if (Selection.objects.Length != 1)
            {
                return false;
            }

            Texture2D texture = Selection.objects[0] as Texture2D;
            if (texture == null)
            {
                return false;
            }
            
            TextureImporter textureImporter = AtlasBuilder.GetTextureImporter(texture);
            if (textureImporter.textureType != TextureImporterType.Sprite
                || textureImporter.spriteImportMode != SpriteImportMode.Multiple)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 選択されているディレクトリに新規Atlasを生成する設定で編集ウィンドウを開きます。
        /// </summary>
        [MenuItem("Assets/Jigbox/Create Atlas")]
        static void OpenWithCreateAtlas()
        {
            string directoryPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

            AtlasPackerWindow window = Open();
            window.settings.SetTemporarySetting("NewAtlas", directoryPath);
            if (window.HasEdited)
            {
                window.ShowConfirm("編集中の情報は破棄されますが、よろしいですか？",
                    () =>
                    {
                        window.settings.ApplyTemporarySetting();
                    });
            }
            else
            {
                window.settings.ApplyTemporarySetting();
            }
        }

        /// <summary>
        /// 選択されている対象がディレクトリかどうかを確認します。
        /// </summary>
        [MenuItem("Assets/Jigbox/Create Atlas", true)]
        static bool VaidateOpenWithCreateAtlas()
        {
            if (Selection.assetGUIDs.Length > 1 || Selection.assetGUIDs.Length == 0)
            {
                return false;
            }

            // プロジェクトビューをTwoTabで表示しているとディレクトリ選択時、Selection.activeObjectがnullになり、
            // OneTabで表示している時は、ディレクトリの存在確認でディレクトリかどうかを判断する
            if (Selection.activeObject != null
                && !AssetDatabase.IsValidFolder(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0])))
            {
                return false;
            }

            return true;
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            settings = new AtlasPackerSettings(this);
            builder = new AtlasBuilder(settings);
            spriteList = new PackingSpriteList();
        }

        protected virtual void OnGUI()
        {
            settings.DrawEditFields();

            // 選択状態が変化していれば、リストの表示を更新するために再描画を行う
            if (spriteList.UpdateSelectTextures())
            {
                Repaint();
            }
            spriteList.DrawHeader();
            spriteList.DrawSpriteList();

            DrawFooterButton();

            AcceptDragAndDrop();
        }

#endregion
    }
}
