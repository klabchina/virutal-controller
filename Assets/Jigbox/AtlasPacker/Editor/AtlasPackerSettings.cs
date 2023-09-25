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
using System.IO;
using Jigbox.EditorUtils;

namespace Jigbox.AtlasPacker
{
    public class AtlasPackerSettings
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 共通設定
        /// </summary>
        protected class CommonSettings
        {
            /// <summary>最後にAtlasを出力したディレクトリの保存用キー</summary>
            protected static readonly string LastExprotDirectoryKey = "ATLAS_PACKER_LAST_EXPORT_DIRECTORY_KEY";

            /// <summary>画像のPaddingの設定の保存用キー</summary>
            protected static readonly string PaddingKey = "ATLAS_PACKER_PADDING_KEY";

            /// <summary>Atlasをフルカラーで出力するかどうかの保存用キー</summary>
            protected static readonly string ForceExportFullcolorKey = "ATLAS_PACKER_FORCE_EXPORT_FULLCOLOR_KEY";

            /// <summary>最後にAtlasを出力したディレクトリ</summary>
            public string LastExportDirectory { get; set; }

            /// <summary>画像のPaddingの設定</summary>
            public int Padding { get; set; }

            /// <summary>Atlasをフルカラーで出力するかどうか</summary>
            public bool ForceExportFullcolor { get; set; }

            /// <summary>
            /// 設定を読み込みます。
            /// </summary>
            public virtual void LoadSettings()
            {
                LastExportDirectory = EditorPrefs.GetString(LastExprotDirectoryKey, string.Empty);
                Padding = EditorPrefs.GetInt(PaddingKey, 1);
                ForceExportFullcolor = EditorPrefs.GetBool(ForceExportFullcolorKey, false);
            }

            /// <summary>
            /// 設定を保存します。
            /// </summary>
            public virtual void SaveSettings()
            {
                EditorPrefs.SetString(LastExprotDirectoryKey, LastExportDirectory);
                EditorPrefs.SetInt(PaddingKey, Padding);
                EditorPrefs.SetBool(ForceExportFullcolorKey, ForceExportFullcolor);
            }
        }

        /// <summary>
        /// Atlas別の設定
        /// </summary>
        protected class AtlasExportSettings
        {
            /// <summary>Atlas名</summary>
            public string AtlasName { get; set; }

            /// <summary>出力先ディレクトリ</summary>
            public string ExportDirectory { get; set; }

            /// <summary>Atlasとして扱うテクスチャの参照</summary>
            public Texture2D Atlas { get; set; }

            /// <summary>画像のPaddingの設定</summary>
            public int Padding { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public AtlasExportSettings()
            {
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="settings">コピー元</param>
            public AtlasExportSettings(AtlasExportSettings settings)
            {
                AtlasName = settings.AtlasName;
                ExportDirectory = settings.ExportDirectory;
                Atlas = settings.Atlas;
                Padding = settings.Padding;
            }

            /// <summary>
            /// 設定を初期化します。
            /// </summary>
            /// <param name="commonSettings">共通設定</param>
            public virtual void Init(CommonSettings commonSettings)
            {
                AtlasName = "NewAtlas";
                ExportDirectory = commonSettings.LastExportDirectory;
                Atlas = null;
                Padding = commonSettings.Padding;
            }
        }

#endregion

#region constants

        /// <summary>ラベルの幅</summary>
        protected static readonly float LabelWidth = 115.0f;

        /// <summary>Atlasのプレビューサイズ</summary>
        protected static readonly float AtlasPreviewSize = 96.0f;

        /// <summary>キャンセルボタンのサイズ</summary>
        protected static readonly float CancelButtonSize = 20.0f;

        /// <summary>キャンセルボタンの色</summary>
        protected static readonly Color CancelButtonColor = new Color(0.8f, 0.4f, 0.4f);

#endregion

#region properties

        /// <summary>Atlasの編集用ウィンドウ</summary>
        protected AtlasPackerWindow window;

        /// <summary>共通設定</summary>
        protected CommonSettings commonSettings = new CommonSettings();

        /// <summary>Atlas別の設定</summary>
        protected AtlasExportSettings atlasSetting = new AtlasExportSettings();

        /// <summary>設定を適用するまでの一時保存状態の設定</summary>
        protected AtlasExportSettings temporarySetting = null;

        /// <summary>Atlasの参照を持っているかどうか</summary>
        public bool HasAtlas { get { return atlasSetting.Atlas != null; } }

        /// <summary>Atlas名</summary>
        public string AtlasName { get { return atlasSetting.AtlasName; } }

        /// <summary>出力先ディレクトリ</summary>
        public string ExportDirectory { get { return atlasSetting.ExportDirectory.TrimEnd('/'); } }

        /// <summary>画像のPaddingの設定</summary>
        public int Padding { get { return atlasSetting.Padding; } }

        /// <summary>Atlasをフルカラーで出力するかどうか</summary>
        public bool ForceExportFullcolor { get { return commonSettings.ForceExportFullcolor; } }

        protected bool lastForceExportFullcolor = false;

        /// <summary>何らかの編集が行われているかどうか</summary>
        public bool HasEdited 
        {
            get
            {
                return commonSettings.Padding != atlasSetting.Padding
                    || commonSettings.ForceExportFullcolor != lastForceExportFullcolor;
            }
        }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="window">Atlasの編集用ウィンドウ</param>
        public AtlasPackerSettings(AtlasPackerWindow window)
        {
            this.window = window;
            commonSettings.LoadSettings();
            atlasSetting.Init(commonSettings);
            lastForceExportFullcolor = commonSettings.ForceExportFullcolor;
        }

        /// <summary>
        /// 設定の編集用フィールドを表示します。
        /// </summary>
        public virtual void DrawEditFields()
        {
            EditorGUILayout.BeginHorizontal();
            {
                DrawEditSettings();
                PreviewAtlas();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 選択されたAtlasの情報を一時設定します。
        /// </summary>
        /// <param name="atlasName">Atlas名</param>
        /// <param name="exportDirectory">Atlasの出力先ディレクトリ</param>
        public virtual void SetTemporarySetting(string atlasName, string exportDirectory = null)
        {
            temporarySetting = new AtlasExportSettings();
            temporarySetting.AtlasName = atlasName;
            temporarySetting.ExportDirectory = !string.IsNullOrEmpty(exportDirectory) ?
                exportDirectory.Substring(7) : commonSettings.LastExportDirectory;
            temporarySetting.Padding = commonSettings.Padding;
        }

        /// <summary>
        /// 選択されたAtlasの情報を一時設定します。
        /// </summary>
        /// <param name="texture">Atlasとして扱うテクスチャの参照</param>
        public virtual void SetTemporarySetting(Texture2D texture)
        {
            temporarySetting = new AtlasExportSettings();
            temporarySetting.Atlas = texture;
            temporarySetting.AtlasName = texture.name;
            // Assets/以下のパスのみを格納
            string assetPath = AssetDatabase.GetAssetPath(texture);
            int index = assetPath.LastIndexOf("/");
            temporarySetting.ExportDirectory = assetPath.Substring(7, index - 7);
            temporarySetting.Padding = commonSettings.Padding;
        }

        /// <summary>
        /// 設定に一時設定状態の情報を反映させます。
        /// </summary>
        public virtual void ApplyTemporarySetting()
        {
            if (temporarySetting != null)
            {
                atlasSetting = temporarySetting;
                temporarySetting = null;

                if (atlasSetting.Atlas != null)
                {
                    window.SetAtlas(atlasSetting.Atlas);
                }
                else
                {
                    window.ClearAtlas();
                }
            }
        }

        /// <summary>
        /// Atlasの付加情報から読み出したPaddingを設定します。
        /// </summary>
        /// <param name="padding">画像のPaddingの設定</param>
        public virtual void SetPaddingFromAtlas(int padding)
        {
            commonSettings.Padding = padding;
            atlasSetting.Padding = padding;
        }

        /// <summary>
        /// 共通設定を保存します。
        /// </summary>
        public virtual void SaveCommonSettings()
        {
            commonSettings.LastExportDirectory = atlasSetting.ExportDirectory;
            commonSettings.Padding = atlasSetting.Padding;
            commonSettings.SaveSettings();
            lastForceExportFullcolor = commonSettings.ForceExportFullcolor;
        }

#endregion

#region protected methods

        /// <summary>
        /// 設定の編集用フィールドを表示します。
        /// </summary>
        protected virtual void DrawEditSettings()
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorUtilsTools.SetLabelWidth(LabelWidth);

            EditorGUILayout.BeginVertical();
            {
                EditorGUI.BeginDisabledGroup(atlasSetting.Atlas != null);
                {
                    atlasSetting.AtlasName = EditorGUILayout.TextField("Atlas Name", atlasSetting.AtlasName);

                    EditorGUILayout.BeginHorizontal();
                    {
                        atlasSetting.ExportDirectory = EditorGUILayout.TextField("Directory (Assets/)", atlasSetting.ExportDirectory);
                        if (GUILayout.Button("...", GUILayout.Width(35.0f)))
                        {
                            string path = Application.dataPath + "/" + atlasSetting.ExportDirectory;
                            if (!Directory.Exists(path))
                            {
                                path = Application.dataPath;
                            }

                            string directory = EditorUtility.SaveFolderPanel("Atlasの保存ディレクトリの選択", path, "");
                            if (string.IsNullOrEmpty(directory))
                            {
                                return;
                            }

                            if (directory.IndexOf(Application.dataPath) < 0)
                            {
                                Debug.LogWarning("プロジェクト内のディレクトリを選択して下さい。");
                                return;
                            }

                            int assetPathLength = Application.dataPath.Length;
                            // Assets/以下のパスのみを格納
                            atlasSetting.ExportDirectory = directory.Length > assetPathLength ? directory.Substring(assetPathLength + 1) : string.Empty;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.EndDisabledGroup();

                atlasSetting.Padding = Mathf.Max(0, EditorGUILayout.IntField("Padding (pixel)", atlasSetting.Padding));
                commonSettings.ForceExportFullcolor = EditorGUILayout.Toggle("Export Truecolor", commonSettings.ForceExportFullcolor);
            }
            EditorGUILayout.EndVertical();

            EditorUtilsTools.SetLabelWidth(labelWidth);
        }

        /// <summary>
        /// 編集するAtlasをプレビューします。
        /// </summary>
        protected virtual void PreviewAtlas()
        {
            if (!HasAtlas)
            {
                return;
            }

            Rect rect = GUILayoutUtility.GetRect(AtlasPreviewSize, AtlasPreviewSize, GUILayout.ExpandWidth(false));

            GUI.Box(rect, "");

            // アス比に合わせて表示を補正
            Rect textureRect = rect;
            float aspect = (float) atlasSetting.Atlas.width / (float) atlasSetting.Atlas.height;
            if (aspect != 1.0f)
            {
                // 横長
                if (aspect > 1.0f)
                {
                    float padding = rect.height * (1.0f - 1.0f / aspect) * 0.5f;
                    textureRect.yMin += padding;
                    textureRect.yMax -= padding;
                }
                // 縦長
                else
                {
                    float padding = rect.width * (1.0f - aspect) * 0.5f;
                    textureRect.xMin += padding;
                    textureRect.xMax -= padding;
                }
            }

            GUI.DrawTextureWithTexCoords(textureRect, atlasSetting.Atlas, new Rect(0.0f, 0.0f, 1.0f, 1.0f));

            Rect cancelButtonRect = new Rect(
                rect.x + (AtlasPreviewSize - CancelButtonSize),
                rect.y,
                CancelButtonSize,
                CancelButtonSize);

            GUI.backgroundColor = CancelButtonColor;
            if (GUI.Button(cancelButtonRect, "x"))
            {
                if (window.HasEdited)
                {
                    window.ShowConfirm("編集中の情報は破棄されますが、よろしいですか？",
                        () =>
                        {
                            atlasSetting.Init(commonSettings);
                            window.ClearAtlas();
                        });
                }
                else
                {
                    atlasSetting.Init(commonSettings);
                    window.ClearAtlas();
                }
            }
            GUI.backgroundColor = Color.white;
        }

#endregion
    }
}
