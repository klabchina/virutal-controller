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
using System.Collections.Generic;
using Jigbox.EditorUtils;

namespace Jigbox.NovelKit
{
    using ScriptFileType = AdvScriptFileSearcher.ScriptFileType;

    public class AdvScriptImporter : AssetPostprocessor
    {
#region private methods

        /// <summary>
        /// 再度インポートし直すファイルを取得します。
        /// </summary>
        /// <param name="assets">アセットの一覧</param>
        /// <param name="fileInfo">シナリオ用ファイルの一覧</param>
        /// <returns></returns>
        static List<AdvScriptFileInfo> GetImportFiles(List<string> assets, List<AdvScriptFileInfo> fileInfo)
        {
            List<AdvScriptFileInfo> importFiles = new List<AdvScriptFileInfo>();

            foreach (string asset in assets)
            {
                foreach (AdvScriptFileInfo info in fileInfo)
                {
#if UNITY_EDITOR_WIN
                    string path = info.FullName.Replace('\\', '/');
#else
                    string path = info.FullName;
#endif
                    if (asset.Contains(path))
                    {
                        importFiles.Add(info);
                        break;
                    }
                }
            }

            return importFiles;
        }

#endregion

#region override unity methods

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Length == 0)
            {
                return;
            }

            bool isSharedSetting = AdvScriptImportWindow.GetSharedSetting();

            string scenarioDirectory = AdvScriptImportWindow.GetDirectory(AdvScriptImportWindow.ScenarioDirectoryKey, isSharedSetting);
            if (string.IsNullOrEmpty(scenarioDirectory))
            {
                return;
            }
            string exportDirectory = AdvScriptImportWindow.GetDirectory(AdvScriptImportWindow.ExecutableExportDirectoryKey, isSharedSetting);
            if (string.IsNullOrEmpty(exportDirectory))
            {
                return;
            }
            string relativeScenarioDirectory = scenarioDirectory;
            if (!relativeScenarioDirectory.Contains("Assets"))
            {
                // Unity上でAssetとして認識されない場所にシナリオがある場合は、自動インポート不可能
                return;
            }
            else
            {
                // Assets/以下のシナリオのスクリプト用ディレクトリのパス
                relativeScenarioDirectory = relativeScenarioDirectory.Substring(relativeScenarioDirectory.IndexOf("Assets"));
            }

            // シナリオスクリプトと同様の拡張子のAssetのみ抽出
            List<string> scriptAssets = new List<string>();
            List<string> macroAssets = new List<string>();
            List<string> constantValueAssets = new List<string>();
            foreach (string asset in importedAssets)
            {
                if (!asset.Contains(relativeScenarioDirectory))
                {
                    continue;
                }

                if (asset.Contains(AdvScriptDataExporter.ScenarioScriptFileExtension))
                {
                    scriptAssets.Add(asset);
                }
                if (asset.Contains(AdvScriptDataExporter.MacroFileExtension))
                {
                    macroAssets.Add(asset);
                }
                if (asset.Contains(AdvScriptDataExporter.ConstantValueFileExtension))
                {
                    constantValueAssets.Add(asset);
                }
            }
            if (scriptAssets.Count == 0 && macroAssets.Count == 0 && constantValueAssets.Count == 0)
            {
                return;
            }

            bool autoRebuildScripts = AdvScriptImportWindow.GetAutoRebuild(isSharedSetting);
            bool useCustomParser = AdvScriptImportWindow.GetUseCustomParser(isSharedSetting);
            if (useCustomParser && AdvCommandParser.CustomParser == null)
            {
                IAdvCustomCommandParser customParser = AdvEditorCustomParserSearcher.GetCustomParser();
                AdvCommandParser.CustomParser = customParser;
            }

            Dictionary<ScriptFileType, List<AdvScriptFileInfo>> scriptFiles = AdvScriptFileSearcher.GetAllScenarioScripts(scenarioDirectory);

            AdvExecutableFileProvider.Instance.SetDirectory(scenarioDirectory, exportDirectory);
            // ビルド用ウィンドウで設定されてしまうためprogressハンドラーを初期化しておく
            AdvExecutableFileProvider.Instance.BuildProgressHandler = null;

            AdvScriptBuildInfo.Instance.Load();

            EditorSerialProcessor processor = new EditorSerialProcessor(false,
                (_) =>
                {
                    bool isRefresh = false;
                    List<AdvScriptFileInfo> importFiles = GetImportFiles(constantValueAssets, scriptFiles[ScriptFileType.Constant]);
                    AdvExecutableFileProvider.Instance.Build(importFiles, ScriptFileType.Constant);
                    isRefresh |= importFiles.Count > 0;
                    if (isRefresh)
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    return isRefresh;
                },
                (flag) =>
                {
                    bool isRefresh = (bool) flag;
                    List<AdvScriptFileInfo> macro = scriptFiles[ScriptFileType.Macro];
                    List<AdvScriptFileInfo> importFiles = autoRebuildScripts && isRefresh ? macro : GetImportFiles(macroAssets, macro);
                    AdvExecutableFileProvider.Instance.Build(importFiles, ScriptFileType.Macro);
                    isRefresh |= importFiles.Count > 0;
                    if (importFiles.Count > 0)
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    return isRefresh;
                },
                (flag) =>
                {
                    bool isRefresh = (bool) flag;
                    List<AdvScriptFileInfo> scripts = scriptFiles[ScriptFileType.Script];
                    List<AdvScriptFileInfo> importFiles = autoRebuildScripts && isRefresh ? scripts : GetImportFiles(scriptAssets, scripts);
                    AdvExecutableFileProvider.Instance.Build(importFiles, ScriptFileType.Script);
                    isRefresh |= importFiles.Count > 0;
                    if (importFiles.Count > 0)
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    AdvScriptBuildInfo.Instance.Save();
                    return isRefresh;
                });

            processor.UpdateExceptionCallback = (e) =>
            {
                UnityEngine.Debug.LogErrorFormat("Build Failed. {0} : {1}", e.GetType(), e.Message);
                processor.Clear();
            };
        }

#endregion
    }
}
