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

using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using Jigbox.EditorUtils;

namespace Jigbox.NovelKit
{
    public class AdvScriptImportWindow : EditorWindow
    {
#region inner classes, enum, structs

        /// <summary>
        /// ビルド設定
        /// </summary>
        protected class BuildSettings
        {
            /// <summary>シナリオスクリプトの保存ディレクトリ</summary>
            public string ScenarioDirectory { get; protected set; }

            /// <summary>実行用ファイルの出力先ディレクトリ</summary>
            public string ExportDirectory { get; protected set; }

            /// <summary>マクロ、定数を編集した際に自動的にスクリプトをリビルドするかどうか</summary>
            public bool AutoRebuildScripts { get; protected set; }

            /// <summary>Custom Parserを使用するかどうか</summary>
            public bool UseCustomParser { get; protected set; }

            /// <summary>実行用ファイルをバイナリデータとして出力するかどうか</summary>
            public bool IsBuildBinary { get; protected set; }

            /// <summary>実行用ファイルを圧縮するかどうか</summary>
            public bool UseCompression { get; protected set; }

            /// <summary>実行用ファイルの改竄防止を行うかどうか</summary>
            public bool IsBlockTamper { get; protected set; }

            /// <summary>実行用ファイルに改竄防止用のデータを含める際に利用するキーワード</summary>
            public string AuthSecretWord { get; protected set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public BuildSettings()
            {
                bool isSharedSetting = GetSharedSetting();
                ScenarioDirectory = GetDirectory(ScenarioDirectoryKey, isSharedSetting);
                if (!string.IsNullOrEmpty(ScenarioDirectory))
                {
                    if (!Directory.Exists(ScenarioDirectory))
                    {
                        ScenarioDirectory = string.Empty;
                    }
                }
                ExportDirectory = GetDirectory(ExecutableExportDirectoryKey, isSharedSetting);
                if (!string.IsNullOrEmpty(ExportDirectory))
                {
                    if (!Directory.Exists(ExportDirectory))
                    {
                        ExportDirectory = string.Empty;
                    }
                }

                AutoRebuildScripts = GetAutoRebuild(isSharedSetting);
                UseCustomParser = GetUseCustomParser(isSharedSetting);
                IsBuildBinary = GetBuildBinary(isSharedSetting);
                UseCompression = GetCompression(isSharedSetting);
                IsBlockTamper = GetBlockTamper(isSharedSetting);
                AuthSecretWord = GetAuthSecretWord(isSharedSetting);
            }
        }

#endregion

#region constants

        /// <summary>シナリオスクリプトの保存ディレクトリの保存用キー</summary>
        public static readonly string ScenarioDirectoryKey = "SCENARIO_SCRIPT_DIRECTORY";

        /// <summary>実行用ファイルの出力先ディレクトリの保存用キー</summary>
        public static readonly string ExecutableExportDirectoryKey = "EXECUTABLE_EXPORT_DIRECTORY";

        /// <summary>プロジェクト間で設定を共有するかどうかの保存用キー</summary>
        public static readonly string SharedSettingKey = "SHARED_SETTING";

        /// <summary>CustomParserを使用するかどうかの保存用キー</summary>
        static readonly string UseCustomParserKey = "USE_ADV_CUSTOM_PARSER";

        /// <summary>マクロ、定数を編集した際に自動的にスクリプトをリビルドするかどうかの保存用キー</summary>
        static readonly string AutoRebuildScriptsKey = "AUTO_REBUILD_SCRIPTS";

        /// <summary>実行用ファイルをバイナリデータとして出力するかどうかの保存用キー</summary>
        static readonly string ExecutableDataBuildBinaryKey = "EXECUTABLE_DATA_BUILD_BINARY";

        /// <summary>実行用ファイルを圧縮するかどうかの保存用キー</summary>
        static readonly string ExecutableDataCompressionKey = "EXECUTABLE_DATA_COMPRESSION";

        /// <summary>実行用ファイルの改竄防止を行うどうかの保存用キー</summary>
        static readonly string ExecutableDataBlockTamperKey = "EXECUTABLE_DATA_BLOCK_TAMPER";

        /// <summary>実行用ファイルに改竄防止用のデータを含める際に利用するキーワードの保存用キー</summary>
        static readonly string ExecutableDataAuthSecretWordKey = "EXECUTABLE_DATA_AUTH_SECRET_KEY";

        /// <summary>ディレクトリ設定を表示ているかどうかの保存キー</summary>
        static readonly string ShowDirectorySettingKey = "SHOW_DIRECTORY_SETTING";

#endregion

#region properties

        /// <summary>シナリオスクリプトの保存ディレクトリ</summary>
        protected string scenarioDirectory = string.Empty;

        /// <summary>実行用ファイルの出力先ディレクトリ</summary>
        protected string exportDirectory = string.Empty;

        /// <summary>マクロ、定数を編集した際に自動的にスクリプトをリビルドするかどうか</summary>
        protected bool autoRebuildScripts = true;

        /// <summary>プロジェクト間で設定を共有するかどうか</summary>
        protected bool isSharedSetting = false;

        /// <summary>Custom Parserを使用するかどうか</summary>
        protected bool useCustomParser = false;

        /// <summary>Custom Parserのクラス名</summary>
        protected string customParserType = string.Empty;

        /// <summary>実行用ファイルをバイナリデータとして出力するかどうか</summary>
        protected bool isBuildBinary = false;

        /// <summary>実行用ファイルを圧縮するかどうか</summary>
        protected bool useCompression = false;

        /// <summary>実行用ファイルの改竄防止を行うかどうか</summary>
        protected bool isBlockTamper = false;

        /// <summary>実行用ファイルに改竄防止用のデータを含める際に利用するキーワード</summary>
        protected string authSecretWord = string.Empty;

        /// <summary>ディレクトリ設定を表示しているか</summary>
        protected bool isShowDirectorySetting = true;

#endregion

#region public methods

        /// <summary>
        /// ディレクトリのパスを取得します。
        /// </summary>
        /// <param name="key">保存キー</param>
        /// <param name="isSharedSetting">設定を共有するかどうか</param>
        /// <returns></returns>
        public static string GetDirectory(string key, bool isSharedSetting)
        {
            if (!isSharedSetting)
            {
                key += "_" + PlayerSettings.productName;
            }
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetString(key);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// プロジェクト間で設定を共有するかどうかを取得します。
        /// </summary>
        /// <returns></returns>
        public static bool GetSharedSetting()
        {
            if (EditorPrefs.HasKey(SharedSettingKey))
            {
                return EditorPrefs.GetBool(SharedSettingKey);
            }
            else
            {
                // 共有設定が保存されていない場合、共有領域に情報が保存されているかどうかで判断する
                return EditorPrefs.HasKey(ScenarioDirectoryKey) || EditorPrefs.HasKey(ExecutableExportDirectoryKey);
            }
        }

        /// <summary>
        /// マクロ、定数を編集した際に自動的にスクリプトをリビルドするかどうかを取得します。
        /// </summary>
        /// <param name="isSharedSetting">設定を共有するかどうか</param>
        /// <returns></returns>
        public static bool GetAutoRebuild(bool isSharedSetting)
        {
            return GetBoolSetting(AutoRebuildScriptsKey, true, isSharedSetting);
        }

        /// <summary>
        /// CustomParserを使用するかどうかを取得します。
        /// </summary>
        /// <param name="isSharedSetting">設定を共有するかどうか</param>
        /// <returns></returns>
        public static bool GetUseCustomParser(bool isSharedSetting)
        {
            return GetBoolSetting(UseCustomParserKey, false, isSharedSetting);
        }

        /// <summary>
        /// 実行用ファイルをバイナリデータとして出力するかどうかを取得します。
        /// </summary>
        /// <param name="isSharedSetting">設定を共有するかどうか</param>
        /// <returns></returns>
        public static bool GetBuildBinary(bool isSharedSetting)
        {
            return GetBoolSetting(ExecutableDataBuildBinaryKey, false, isSharedSetting);
        }

        /// <summary>
        /// 実行用ファイルを圧縮するかどうかを取得します。
        /// </summary>
        /// <param name="isSharedSetting">設定を共有するかどうか</param>
        /// <returns></returns>
        public static bool GetCompression(bool isSharedSetting)
        {
            return GetBoolSetting(ExecutableDataCompressionKey, false, isSharedSetting);
        }

        /// <summary>
        /// 実行用ファイルに改竄防止用のデータを含めるかどうかを取得します。
        /// </summary>
        /// <param name="isSharedSetting">設定を共有するかどうか</param>
        /// <returns></returns>
        public static bool GetBlockTamper(bool isSharedSetting)
        {
            return GetBoolSetting(ExecutableDataBlockTamperKey, false, isSharedSetting);
        }

        /// <summary>
        ///  実行用ファイルに改竄防止用のデータを含める際に利用するキーワードを取得します。
        /// </summary>
        /// <param name="isSharedSetting">設定を共有するかどうか</param>
        /// <returns></returns>
        public static string GetAuthSecretWord(bool isSharedSetting)
        {
            string key = ExecutableDataAuthSecretWordKey;
            if (!isSharedSetting)
            {
                key += "_" + PlayerSettings.productName;
            }
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetString(key);
            }
            else
            {
                return string.Empty;
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// string型で設定を保存します。
        /// </summary>
        /// <param name="key">保存キー</param>
        /// <param name="str">保存する文字列</param>
        /// <param name="isSharedSetting">設定を共有するかどうか</param>
        protected static void SaveStringSetting(string key, string str, bool isSharedSetting)
        {
            if (!isSharedSetting)
            {
                key += "_" + PlayerSettings.productName;
            }
            EditorPrefs.SetString(key, str);
        }

        /// <summary>
        /// bool値で保存されている設定を取得します。
        /// </summary>
        /// <param name="key">保存キー</param>
        /// <param name="defaultValue">キーが存在しなかった場合のデフォルト値</param>
        /// <param name="isSharedSetting">設定を共有するかどうか</param>
        /// <returns>取得した値を返します。</returns>
        protected static bool GetBoolSetting(string key, bool defaultValue, bool isSharedSetting)
        {
            if (!isSharedSetting)
            {
                key += "_" + PlayerSettings.productName;
            }
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetBool(key);
            }
            else
            {
                EditorPrefs.SetBool(key, defaultValue);
                return defaultValue;
            }
        }

        /// <summary>
        /// bool値で設定を保存します。
        /// </summary>
        /// <param name="key">保存キー</param>
        /// <param name="value">保存する値</param>
        /// <param name="isSharedSetting">設定を共有するかどうか</param>
        protected static void SaveBoolSetting(string key, bool value, bool isSharedSetting)
        {
            if (!isSharedSetting)
            {
                key += "_" + PlayerSettings.productName;
            }
            EditorPrefs.SetBool(key, value);
        }

        /// <summary>
        /// 編集する設定のタブを表示します。
        /// </summary>
        protected virtual void DrawEditTab()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUI.backgroundColor = isShowDirectorySetting ? Color.white : Color.gray;

                if (GUILayout.Toggle(false, "ディレクトリ設定","ButtonLeft"))
                {
                    isShowDirectorySetting = true;
                    EditorPrefs.SetBool(ShowDirectorySettingKey, isShowDirectorySetting);
                }

                GUI.backgroundColor = !isShowDirectorySetting ? Color.white : Color.gray;

                if (GUILayout.Toggle(false, "出力データ設定","ButtonRight"))
                {
                    isShowDirectorySetting = false;
                    EditorPrefs.SetBool(ShowDirectorySettingKey, isShowDirectorySetting);
                }

                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// ディレクトリの編集を表示します。
        /// </summary>
        protected virtual void DrawEditDirectory()
        {
            bool openScenarioFolderPanel;
            bool openExportFolderPanel;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                GUILayout.Label("シナリオスクリプトの保存ディレクトリ");
                EditorGUILayout.BeginHorizontal();
                {
                    scenarioDirectory = EditorGUILayout.TextField(scenarioDirectory);
                    openScenarioFolderPanel = GUILayout.Button("...", GUILayout.Width(35.0f));
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("実行用ファイルの出力先ディレクトリ");
                EditorGUILayout.BeginHorizontal();
                {
                    exportDirectory = EditorGUILayout.TextField(exportDirectory);
                    openExportFolderPanel = GUILayout.Button("...", GUILayout.Width(35.0f));
                }
                EditorGUILayout.EndHorizontal();

                GUI.changed = false;
                autoRebuildScripts = EditorGUILayout.ToggleLeft("マクロ、定数編集時にスクリプトをリビルドする", autoRebuildScripts);
                if (GUI.changed)
                {
                    GUI.changed = false;
                    SaveBoolSetting(AutoRebuildScriptsKey, autoRebuildScripts, isSharedSetting);
                }
            }
            EditorGUILayout.EndVertical();

            if (openScenarioFolderPanel)
            {
                string directory = EditorUtility.SaveFolderPanel("スクリプトの保存ディレクトリの選択", scenarioDirectory, "");
                if (!string.IsNullOrEmpty(directory) && directory != scenarioDirectory)
                {
                    scenarioDirectory = directory;
                    SaveStringSetting(ScenarioDirectoryKey, scenarioDirectory, isSharedSetting);
                }
            }

            if (openExportFolderPanel)
            {
                string directory = EditorUtility.SaveFolderPanel("実行用ファイルの出力先ディレクトリの選択", exportDirectory, "");
                if (!string.IsNullOrEmpty(directory) && directory != exportDirectory)
                {
                    exportDirectory = directory;
                    SaveStringSetting(ExecutableExportDirectoryKey, exportDirectory, isSharedSetting);
                }
            }
        }

        /// <summary>
        /// 出力データの編集を表示します。
        /// </summary>
        protected virtual void DrawEditDataSetting()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                GUI.changed = false;
                isBuildBinary = EditorGUILayout.ToggleLeft("バイナリデータで出力する", isBuildBinary);
                if (GUI.changed)
                {
                    GUI.changed = false;
                    SaveBoolSetting(ExecutableDataBuildBinaryKey, isBuildBinary, isSharedSetting);
                    SetExporter();
                }

                EditorGUI.BeginDisabledGroup(!isBuildBinary);
                {
                    GUI.changed = false;
                    useCompression = EditorGUILayout.ToggleLeft("データを圧縮する", useCompression);
                    if (GUI.changed)
                    {
                        GUI.changed = false;
                        SaveBoolSetting(ExecutableDataCompressionKey, useCompression, isSharedSetting);
                        SetCompressionSetting();
                    }

                    float labelWidth = EditorGUIUtility.labelWidth;

                    GUI.changed = false;
                    isBlockTamper = EditorGUILayout.ToggleLeft("データの改竄防止を行う", isBlockTamper);
                    EditorUtilsTools.SetLabelWidth(160.0f);
                    authSecretWord = EditorGUILayout.TextField("改竄防止に利用するキーワード", authSecretWord);
                    if (GUI.changed)
                    {
                        GUI.changed = false;
                        SaveBoolSetting(ExecutableDataBlockTamperKey, isBlockTamper, isSharedSetting);
                        SaveStringSetting(ExecutableDataAuthSecretWordKey, authSecretWord, isSharedSetting);
                        SetAuthSetting();
                    }
                    EditorUtilsTools.SetLabelWidth(labelWidth);

                    if (isBlockTamper)
                    {
                        EditorGUILayout.HelpBox("改竄防止処理には時間がかかります。この設定は、実際にユーザーに展開するデータを出力する際にのみ使用することを推奨します。", MessageType.Info);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Custom Parserの編集を表示します。
        /// </summary>
        protected virtual void DrawEditCustomParser()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                GUI.changed = false;
                useCustomParser = EditorGUILayout.ToggleLeft("Custom Parserを使用する", useCustomParser);
                if (GUI.changed)
                {
                    GUI.changed = false;
                    SaveBoolSetting(UseCustomParserKey, useCustomParser, isSharedSetting);

                    if (useCustomParser && string.IsNullOrEmpty(customParserType))
                    {
                        customParserType = AdvExecutableFileProvider.GetCustomParser();
                    }
                }

                if (useCustomParser)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Custom Parser");
                        EditorGUILayout.TextField(customParserType);
                        if (GUILayout.Button("検出", GUILayout.Width(35.0f)))
                        {
                            customParserType = AdvExecutableFileProvider.GetCustomParser();
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (string.IsNullOrEmpty(customParserType))
                    {
                        EditorGUILayout.HelpBox("Custom Parserとして使用可能なクラスが存在しません。", MessageType.Error);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// スクリプトの出力用モジュールを設定します。
        /// </summary>
        protected virtual void SetExporter()
        {
            AdvExecutableFileProvider.Instance.Exporter = CreateExporter(new AdvScriptLoader(), new AdvResourceLoader());

            SetCompressionSetting();
            SetAuthSetting();
        }

        /// <summary>
        /// スクリプトの出力モジュールを作成します。
        /// </summary>
        /// <param name="scriptLoader">スクリプトファイルのLoader</param>
        /// <param name="loader">リソースのLoader</param>
        protected virtual AdvScriptDataExporter CreateExporter(IAdvScriptLoader scriptLoader, IAdvResourceLoader loader)
        {
            AdvScriptDataExporter exporter = isBuildBinary ? new AdvScriptDataBinaryExporter() : new AdvScriptDataExporter();
            scriptLoader.Loader = loader;
            exporter.ScriptLoader = scriptLoader;
            return exporter;
        }

        /// <summary>
        /// 圧縮するかどうかの設定を反映します。
        /// </summary>
        protected virtual void SetCompressionSetting()
        {
            if (isBuildBinary)
            {
                AdvScriptDataBinaryExporter exporter = AdvExecutableFileProvider.Instance.Exporter as AdvScriptDataBinaryExporter;
                exporter.Encoder.IsCompression = useCompression;
            }
        }

        /// <summary>
        /// 改竄防止のための認証設定を反映します。
        /// </summary>
        protected virtual void SetAuthSetting()
        {
            if (isBuildBinary)
            {
                AdvScriptDataBinaryExporter exporter = AdvExecutableFileProvider.Instance.Exporter as AdvScriptDataBinaryExporter;
                AdvScriptDataEncoder encoder = exporter.Encoder;
                encoder.IsBlockTamper = isBlockTamper;
                encoder.SecretWord = authSecretWord;
            }
        }

        /// <summary>
        /// 設定を取得します。
        /// </summary>
        protected virtual void GetSettings()
        {
            scenarioDirectory = GetDirectory(ScenarioDirectoryKey, isSharedSetting);
            if (!string.IsNullOrEmpty(scenarioDirectory))
            {
                if (!Directory.Exists(scenarioDirectory))
                {
                    scenarioDirectory = string.Empty;
                }
            }
            exportDirectory = GetDirectory(ExecutableExportDirectoryKey, isSharedSetting);
            if (!string.IsNullOrEmpty(exportDirectory))
            {
                if (!Directory.Exists(exportDirectory))
                {
                    exportDirectory = string.Empty;
                }
            }

            autoRebuildScripts = GetAutoRebuild(isSharedSetting);
            useCustomParser = GetUseCustomParser(isSharedSetting);
            isBuildBinary = GetBuildBinary(isSharedSetting);
            useCompression = GetCompression(isSharedSetting);
            isBlockTamper = GetBlockTamper(isSharedSetting);
            authSecretWord = GetAuthSecretWord(isSharedSetting);

            if (useCustomParser)
            {
                customParserType = AdvExecutableFileProvider.GetCustomParser();
            }

            SetExporter();
        }

        /// <summary>
        /// ビルドに必要な設定を行います。
        /// </summary>
        /// <param name="modifyExporter">スクリプトの出力モジュールの変更処理</param>
        /// <returns>ビルド設定を返します。</returns>
        protected static BuildSettings BuildSetup(Func<BuildSettings, AdvScriptDataExporter, AdvScriptDataExporter> modifyExporter = null)
        {
            BuildSettings settings = new BuildSettings();
            AdvExecutableFileProvider.Instance.SetDirectory(settings.ScenarioDirectory, settings.ExportDirectory);

            AdvScriptDataExporter exporter = settings.IsBuildBinary ? new AdvScriptDataBinaryExporter() : new AdvScriptDataExporter();
            IAdvScriptLoader scriptLoader = new AdvScriptLoader();
            scriptLoader.Loader = new AdvResourceLoader();
            exporter.ScriptLoader = scriptLoader;
            if (modifyExporter != null)
            {
                exporter = modifyExporter(settings, exporter);
            }

            if (settings.IsBuildBinary)
            {
                AdvScriptDataBinaryExporter binaryExporter = exporter as AdvScriptDataBinaryExporter;
                binaryExporter.Encoder.IsCompression = settings.UseCompression;
                AdvScriptDataEncoder encoder = binaryExporter.Encoder;
                encoder.IsBlockTamper = settings.IsBlockTamper;
                encoder.SecretWord = settings.AuthSecretWord;
            }

            AdvExecutableFileProvider.Instance.Exporter = exporter;

            if (settings.UseCustomParser)
            {
                AdvExecutableFileProvider.GetCustomParser();
            }

            return settings;
        }

#endregion

#region private methods

        /// <summary>
        /// シナリオスクリプトのインポート用ウィンドウを開きます。
        /// </summary>
        [MenuItem("Window/Jigbox/NovelKit/Import Scenario")]
        static void OpenWindow()
        {
#pragma warning disable 219
            AdvScriptImportWindow window = GetWindow(typeof(AdvScriptImportWindow)) as AdvScriptImportWindow;
#pragma warning restore 219
        }

        [MenuItem("Window/Jigbox/NovelKit/Rebuild")]
        static void Rebuild()
        {
            BuildSettings settings = BuildSetup();
            AdvExecutableFileProvider.Instance.RebuildAll(settings.AutoRebuildScripts);
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            isSharedSetting = GetSharedSetting();
            isShowDirectorySetting = EditorPrefs.GetBool(ShowDirectorySettingKey, true);
            GetSettings();
        }

        protected virtual void BuildProgress(float progress, AdvScriptFileInfo info)
        {
            EditorUtility.DisplayProgressBar("", "Building " + info.Name, progress );
            if (progress >= 1.0f)
            {
                EditorUtility.ClearProgressBar();
            }
        }

        protected virtual void OnGUI()
        {
            // 共有設定を使用するかどうかの設定
            GUI.changed = false;
            isSharedSetting = EditorGUILayout.ToggleLeft("プロジェクト間で設定を共有する", isSharedSetting);
            if (GUI.changed)
            {
                GUI.changed = false;
                EditorPrefs.SetBool(SharedSettingKey, isSharedSetting);
                GetSettings();
            }

            EditorGUILayout.Space();

            // 編集する設定の選択
            DrawEditTab();

            GUILayout.Space(-2.0f);

            if (isShowDirectorySetting)
            {
                // ディレクトリ設定
                DrawEditDirectory();
            }
            else
            {
                // 出力データ設定
                DrawEditDataSetting();
            }

            EditorGUILayout.Space();

            // Custom Parserの設定
            DrawEditCustomParser();

            EditorGUILayout.Space();

            // 出力
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(scenarioDirectory) || string.IsNullOrEmpty(exportDirectory));
            {
                if (GUILayout.Button("出力"))
                {
                    AdvExecutableFileProvider.Instance.SetDirectory(scenarioDirectory, exportDirectory);
                    AdvExecutableFileProvider.Instance.BuildProgressHandler = BuildProgress;
                    AdvExecutableFileProvider.Instance.BuildAll();
                }

                if (GUILayout.Button("リビルド"))
                {
                    AdvExecutableFileProvider.Instance.SetDirectory(scenarioDirectory, exportDirectory);
                    AdvExecutableFileProvider.Instance.BuildProgressHandler = BuildProgress;
                    AdvExecutableFileProvider.Instance.RebuildAll(autoRebuildScripts);
                }
            }
            EditorGUI.EndDisabledGroup();
        }

#endregion
    }
}

