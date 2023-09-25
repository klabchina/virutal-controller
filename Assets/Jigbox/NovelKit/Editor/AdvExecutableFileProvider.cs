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

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Jigbox.EditorUtils;
using Object = UnityEngine.Object;

namespace Jigbox.NovelKit
{
    using ExportResult = AdvScriptDataExporter.ExportResult;
    using ScriptFileType = AdvScriptFileSearcher.ScriptFileType;

    public sealed class AdvExecutableFileProvider
    {
#region properties

        /// <summary>インスタンス</summary>
        static AdvExecutableFileProvider instance;

        /// <summary>インスタンス</summary>
        public static AdvExecutableFileProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdvExecutableFileProvider();
                }
                return instance;
            }
        }

        /// <summary>スクリプトから実行用データを出力するためのコンバータ</summary>
        public AdvScriptDataExporter Exporter { get; set; }

        /// <summary>ビルドの進捗状況をハンドリングします。floatにはprogress値が、AdvScriptFileInfoにはビルドにかけるスクリプトの情報が入ります</summary>
        public Action<float, AdvScriptFileInfo> BuildProgressHandler { get; set; }

        /// <summary>シナリオスクリプトの保存ディレクトリ</summary>
        string scenarioDirectory;

        /// <summary>実行用ファイルの出力先ディレクトリ</summary>
        string exportDirectory;

#endregion

#region public methods

        /// <summary>
        /// ディレクトリを設定します。
        /// </summary>
        /// <param name="scenarioDirectory">シナリオスクリプトの保存ディレクトリ</param>
        /// <param name="exportDirectory">実行用ファイルの出力先ディレクトリparam>
        public void SetDirectory(string scenarioDirectory, string exportDirectory)
        {
            this.scenarioDirectory = scenarioDirectory;
            this.exportDirectory = exportDirectory;
        }

        /// <summary>
        /// シナリオの保存ディレクトリ以下のファイルを全てビルドして、実行用ファイルを出力します。
        /// </summary>
        public void BuildAll()
        {
            if (string.IsNullOrEmpty(scenarioDirectory) || string.IsNullOrEmpty(exportDirectory))
            {
                AdvLog.Error("ディレクトリ情報が設定されていません。");
                return;
            }

            AdvScriptBuildInfo.Instance.Clear();
            
            Dictionary<ScriptFileType, List<AdvScriptFileInfo>> scriptFiles = AdvScriptFileSearcher.GetAllScenarioScripts(scenarioDirectory);
            
            EditorSerialProcessor processor = new EditorSerialProcessor(false,
                (_) =>
                {
                    bool result = Build(scriptFiles[ScriptFileType.Constant], ScriptFileType.Constant);

                    if (result)
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        AdvScriptBuildInfo.Instance.Save();
                        AdvLog.Error("定数ファイルの出力に失敗したため、マクロ、スクリプトファイルを出力できませんでした。");
                    }
                    return result;
                },
                (flag) =>
                {
                    if (!(bool) flag)
                    {
                        return false;
                    }

                    bool result = Build(scriptFiles[ScriptFileType.Macro], ScriptFileType.Macro);

                    if (result)
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        AdvScriptBuildInfo.Instance.Save();
                        AdvLog.Error("マクロファイルの出力に失敗したため、スクリプトファイルを出力できませんでした。");
                    }
                    return result;
                },
                (flag) =>
                {
                    if (!(bool) flag)
                    {
                        return false;
                    }

                    bool result = Build(scriptFiles[ScriptFileType.Script], ScriptFileType.Script);

                    if (result)
                    {
                        AdvLog.Print("全てのファイルを正常に出力しました。");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        AdvLog.Error("ファイルの出力に失敗しました。");
                    }

                    AdvScriptBuildInfo.Instance.Save();

                    return result;
                });

            processor.UpdateExceptionCallback = (e) =>
            {
                Debug.LogErrorFormat("Build Failed. {0} : {1}", e.GetType(), e.Message);
                processor.Clear();
            };
        }

        /// <summary>
        /// シナリオの保存ディレクトリ以下のファイルのうち、更新があったものを出力します。
        /// </summary>
        /// <summary>マクロ、定数を編集した際に自動的にスクリプトをリビルドするかどうか</summary>
        public void RebuildAll(bool autoRebuildScripts)
        {
            if (string.IsNullOrEmpty(scenarioDirectory) || string.IsNullOrEmpty(exportDirectory))
            {
                AdvLog.Error("ディレクトリ情報が設定されていません。");
                return;
            }

            AdvScriptBuildInfo.Instance.Load();

            Dictionary<ScriptFileType, List<AdvScriptFileInfo>> scriptFiles = AdvScriptFileSearcher.GetAllScenarioScripts(scenarioDirectory);

            EditorSerialProcessor processor = new EditorSerialProcessor(false,
                (_) =>
                {
                    List<AdvScriptFileInfo> rebuildFiles = GetRebuildFiles(ScriptFileType.Constant, scriptFiles[ScriptFileType.Constant]);
                    bool result = Build(rebuildFiles, ScriptFileType.Constant);

                    if (result)
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        AdvScriptBuildInfo.Instance.Save();
                        AdvLog.Error("定数ファイルの出力に失敗したため、マクロ、スクリプトファイルを出力できませんでした。");
                    }
                    return new bool[] { result, rebuildFiles.Count > 0 };
                },
                (arg) =>
                {
                    bool[] flags = (bool[]) arg;
                    if (!flags[0])
                    {
                        return new bool[] { false, false };
                    }

                    List<AdvScriptFileInfo> rebuildFiles = null;
                    if (autoRebuildScripts && flags[1])
                    {
                        rebuildFiles = scriptFiles[ScriptFileType.Macro];
                    }
                    else
                    {
                        rebuildFiles = GetRebuildFiles(ScriptFileType.Macro, scriptFiles[ScriptFileType.Macro]);
                    }

                    bool result = Build(rebuildFiles, ScriptFileType.Macro);

                    if (result)
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        AdvScriptBuildInfo.Instance.Save();
                        AdvLog.Error("マクロファイルの出力に失敗したため、スクリプトファイルを出力できませんでした。");
                    }
                    return new bool[] { result, rebuildFiles.Count > 0 };
                },
                (arg) =>
                {
                    bool[] flags = (bool[]) arg;
                    if (!flags[0])
                    {
                        return new bool[] { false, false };
                    }

                    List<AdvScriptFileInfo> rebuildFiles = null;
                    if (autoRebuildScripts && flags[1])
                    {
                        rebuildFiles = scriptFiles[ScriptFileType.Script];
                    }
                    else
                    {
                        rebuildFiles = GetRebuildFiles(ScriptFileType.Script, scriptFiles[ScriptFileType.Script]);
                    }

                    bool result = Build(rebuildFiles, ScriptFileType.Script);

                    if (result)
                    {
                        AdvLog.Print("全てのファイルを正常に出力しました。");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        AdvLog.Error("ファイルの出力に失敗しました。");
                    }

                    AdvScriptBuildInfo.Instance.Save();

                    return new bool[] { result, rebuildFiles.Count > 0 };
                });

            processor.UpdateExceptionCallback = (e) =>
            {
                Debug.LogErrorFormat("Build Failed. {0} : {1}", e.GetType(), e.Message);
                processor.Clear();
            };
        }

        /// <summary>
        /// シナリオスクリプトをビルドして実行用のファイルを出力します。
        /// </summary>
        /// <param name="script">スクリプトファイル情報</param>
        /// <returns>ビルドに成功すれば<c>true</c>、失敗すれば<c>false</c>を返します。</returns>
        public bool Build(AdvScriptFileInfo script)
        {
            if (string.IsNullOrEmpty(scenarioDirectory) || string.IsNullOrEmpty(exportDirectory))
            {
                AdvLog.Error("ディレクトリ情報が設定されていません。");
                return false;
            }

            return BuildA(script);
        }

        /// <summary>
        /// シナリオスクリプトをビルドして実行用のファイルを出力します。
        /// 第二引数を指定された場合はビルドしたスクリプトのタイムスタンプ情報を更新します。
        /// </summary>
        /// <param name="scripts">スクリプトファイル情報</param>
        /// <param name="type">更新するScriptFileType</param>
        /// <returns></returns>
        public bool Build(IEnumerable<AdvScriptFileInfo> scripts, ScriptFileType? type = null)
        {
            if (string.IsNullOrEmpty(scenarioDirectory) || string.IsNullOrEmpty(exportDirectory))
            {
                AdvLog.Error("ディレクトリ情報が設定されていません。");
                return false;
            }

            bool result = true;

            float buildCount = 0f;
            float scriptCount = scripts.Count();
            foreach (AdvScriptFileInfo script in scripts)
            {
                buildCount++;
                if (BuildProgressHandler != null)
                {
                    BuildProgressHandler(buildCount / scriptCount, script);
                }
                result &= BuildA(script);
            }

            if (type != null)
            {
                UpdateBuildInfo(type.Value, scripts);
            }

            return result;
        }

        /// <summary>
        /// Custom Parserを取得します。
        /// </summary>
        /// <returns>Custom Parserが存在していれば型を、存在しなければ空文字列を返します。</returns>
        public static string GetCustomParser()
        {
            IAdvCustomCommandParser customParser = AdvEditorCustomParserSearcher.GetCustomParser();
            AdvCommandParser.CustomParser = customParser;
            return customParser != null ? customParser.GetType().ToString() : string.Empty;
        }

#endregion

#region private methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        AdvExecutableFileProvider()
        {
            bool isSharedSetting = AdvScriptImportWindow.GetSharedSetting();
            bool isBuildBinary = AdvScriptImportWindow.GetBuildBinary(isSharedSetting);

            Exporter = isBuildBinary ? new AdvScriptDataBinaryExporter() : new AdvScriptDataExporter();
            IAdvResourceLoader loader = new AdvResourceLoader();
            IAdvScriptLoader scriptLoader = new AdvScriptLoader();
            scriptLoader.Loader = loader;
            Exporter.ScriptLoader = scriptLoader;

            if (isBuildBinary)
            {
                AdvScriptDataBinaryExporter binaryExporter = Exporter as AdvScriptDataBinaryExporter;
                binaryExporter.Encoder.IsCompression = AdvScriptImportWindow.GetCompression(isSharedSetting);
                binaryExporter.Encoder.IsBlockTamper = AdvScriptImportWindow.GetBlockTamper(isSharedSetting);
                binaryExporter.Encoder.SecretWord = AdvScriptImportWindow.GetAuthSecretWord(isSharedSetting);
            }
        }

        /// <summary>
        /// スクリプトファイルをビルドして実行用のファイルを出力します。
        /// </summary>
        /// <param name="script">スクリプトファイル情報</param>
        /// <returns>ビルドに成功すれば<c>true</c>、失敗すれば<c>false</c>を返します。</returns>
        bool BuildA(AdvScriptFileInfo script)
        {
            string fileType = string.Empty;
            switch (script.Extension)
            {
                case AdvScriptDataExporter.ScenarioScriptFileExtension:
                    fileType = "スクリプトファイル";
                    break;

                case AdvScriptDataExporter.MacroFileExtension:
                    fileType = "マクロ定義ファイル";
                    break;

                case AdvScriptDataExporter.ConstantValueFileExtension:
                    fileType = "定数定義ファイル";
                    break;
            }

            ExportResult result = Exporter.Export(script, scenarioDirectory, exportDirectory);
            if (result.Succeeded)
            {
                if (result.HasCreatedNew)
                {
                    AdvLog.Print(string.Format("<color=green>{0}を作成しました。\nPath : {1}</color>",
                        fileType,
                        result.DstFilePath));
                }
                else
                {
                    Object asset = AdvScriptDataExporter.SetDirty(result.DstAssetPath);
                    AdvLog.Print(string.Format("<color=green>{0}を上書きしました。\nPath : {1}</color>",
                        fileType,
                        result.DstFilePath),
                        asset);
                }
            }
            else
            {
                Object asset = AdvScriptDataExporter.GetSourceAsset(script, scenarioDirectory);
                AdvLog.Error(string.Format("<color=red>{0}の出力に失敗しました。\nPath : {1}</color>",
                    fileType,
                    result.SrcFilePath),
                    asset);
            }
            return result.Succeeded;
        }

        /// <summary>
        /// リビルドするファイルを取得します。
        /// </summary>
        /// <param name="type">スクリプトファイルの種類</param>
        /// <param name="fileInfo">シナリオ用ファイルの一覧</param>
        /// <returns>リビルドするファイルの一覧を返します。</returns>
        List<AdvScriptFileInfo> GetRebuildFiles(ScriptFileType type, List<AdvScriptFileInfo> fileInfo)
        {
            List<AdvScriptFileInfo> targets = new List<AdvScriptFileInfo>();
            AdvScriptBuildInfo buildInfo = AdvScriptBuildInfo.Instance;

            foreach (AdvScriptFileInfo info in fileInfo)
            {
                if (buildInfo.IsNeedBuild(type, info.FullName, info.GetFilePath(scenarioDirectory)))
                {
                    targets.Add(info);
                }
            }

            return targets;
        }

        /// <summary>
        /// ビルド情報を更新します。
        /// </summary>
        /// <param name="type">スクリプトファイルの種類</param>
        /// <param name="fileInfo">シナリオ用ファイルの一覧</param>
        void UpdateBuildInfo(ScriptFileType type, IEnumerable<AdvScriptFileInfo> fileInfo)
        {
            AdvScriptBuildInfo buildInfo = AdvScriptBuildInfo.Instance;

            foreach (AdvScriptFileInfo info in fileInfo)
            {
                buildInfo.UpdateBuildInfo(type, info.FullName, info.GetFilePath(scenarioDirectory));
            }
        }

#endregion
    }
}
