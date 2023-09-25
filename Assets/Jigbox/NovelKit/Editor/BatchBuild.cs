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
using System;

namespace Jigbox.NovelKit
{
    public class BatchBuild
    {
#region inner classes, enum, and structs

        public static class CommandLineArgs
        {
            /// <summary>パラメータ</summary>
            static string[] args;

            /// <summary>パラメータ</summary>
            static string[] Args
            {
                get
                {
                    if (args == null)
                    {
                        args = Environment.GetCommandLineArgs();
                    }
                    return args;
                }
            }

            /// <summary>
            /// パラメータから値を取得します。
            /// </summary>
            /// <param name="parameterName">パラメータ名</param>
            /// <returns></returns>
            public static string GetValue(string parameterName)
            {
                foreach (string arg in Args)
                {
                    if (arg.ToLower().StartsWith(parameterName.ToLower()))
                    {
                        return arg.Substring(parameterName.Length);
                    }
                }

                return null;
            }
        }

#endregion

#region constants

        /// <summary>ビルド成功時の戻り値</summary>
        static readonly int RESULT_OK = 0;

        /// <summary>ビルド失敗時の戻り値</summary>
        static readonly int RESULT_NG = 1;

        /// <summary>シナリオスクリプトの保存用ディレクトリ</summary>
        static readonly string ScenarioDirectoryParam = "-scenarioDirectory=";

        /// <summary>実行用ファイルの出力先ディレクトリ</summary>
        static readonly string ExportDirectoryParam = "-exportDirectory=";

#endregion

#region public methods

        /// <summary>
        /// 定数定義ファイルをビルドします。
        /// </summary>
        /// <returns></returns>
        static int BuildConstants()
        {
            string scenarioDirectory = CommandLineArgs.GetValue(ScenarioDirectoryParam);
            string exportDirectory = CommandLineArgs.GetValue(ExportDirectoryParam);

            AdvExecutableFileProvider provider = AdvExecutableFileProvider.Instance;
            provider.SetDirectory(scenarioDirectory, exportDirectory);
            AdvExecutableFileProvider.GetCustomParser();

            bool result = provider.Build(AdvScriptFileSearcher.GetConstants(scenarioDirectory));

            if (result)
            {
                AdvLog.Print("全ての定数ファイルを正常に出力しました。");
                // Unityの仕組み上、一度AssetDatabaseを更新しないと正しくファイルを読めない
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return RESULT_OK;
            }
            else
            {
                AdvLog.Error("定数ファイルの出力に失敗しました。");
                EditorApplication.Exit(RESULT_NG);
                return RESULT_NG;
            }
        }

        /// <summary>
        /// マクロ定義ファイルをビルドします。
        /// </summary>
        /// <returns></returns>
        static int BuildMacro()
        {
            string scenarioDirectory = CommandLineArgs.GetValue(ScenarioDirectoryParam);
            string exportDirectory = CommandLineArgs.GetValue(ExportDirectoryParam);

            AdvExecutableFileProvider provider = AdvExecutableFileProvider.Instance;
            provider.SetDirectory(scenarioDirectory, exportDirectory);
            AdvExecutableFileProvider.GetCustomParser();

            bool result = provider.Build(AdvScriptFileSearcher.GetMacro(scenarioDirectory));

            if (result)
            {
                AdvLog.Print("全てのマクロファイルを正常に出力しました。");
                // Unityの仕組み上、一度AssetDatabaseを更新しないと正しくファイルを読めない
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return RESULT_OK;
            }
            else
            {
                AdvLog.Error("マクロファイルの出力に失敗しました。");
                EditorApplication.Exit(RESULT_NG);
                return RESULT_NG;
            }
        }

        /// <summary>
        /// スクリプトファイルをビルドします。
        /// </summary>
        /// <returns></returns>
        static int BuildScripts()
        {
            string scenarioDirectory = CommandLineArgs.GetValue(ScenarioDirectoryParam);
            string exportDirectory = CommandLineArgs.GetValue(ExportDirectoryParam);

            AdvExecutableFileProvider provider = AdvExecutableFileProvider.Instance;
            provider.SetDirectory(scenarioDirectory, exportDirectory);
            AdvExecutableFileProvider.GetCustomParser();

            bool result = provider.Build(AdvScriptFileSearcher.GetScripts(scenarioDirectory));

            if (result)
            {
                AdvLog.Print("全てのスクリプトファイルを正常に出力しました。");
                // Unityの仕組み上、一度AssetDatabaseを更新しないと正しくファイルを読めない
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return RESULT_OK;
            }
            else
            {
                AdvLog.Error("スクリプトファイルの出力に失敗しました。");
                EditorApplication.Exit(RESULT_NG);
                return RESULT_NG;
            }
        }

#endregion
    }
}
