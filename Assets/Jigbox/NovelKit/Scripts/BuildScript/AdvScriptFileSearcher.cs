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

#if UNITY_EDITOR || NOVELKIT_EDITOR
using System.Collections.Generic;
using System.IO;

namespace Jigbox.NovelKit
{
    public static class AdvScriptFileSearcher
    {
#region inner classes, enum, and structs

        /// <summary>
        /// スクリプトファイルの種類
        /// </summary>
        public enum ScriptFileType
        {
            /// <summary>スクリプトファイル</summary>
            Script,
            /// <summary>マクロ定義ファイル</summary>
            Macro,
            /// <summary>定数定義ファイル</summary>
            Constant,
        }

#endregion

#region public methods

        /// <summary>
        /// 全てのシナリオスクリプトを取得します。
        /// </summary>
        /// <param name="rootDirectoryPath">ルートとなるディレクトリのパス</param>
        /// <returns></returns>
        public static Dictionary<ScriptFileType, List<AdvScriptFileInfo>> GetAllScenarioScripts(string rootDirectoryPath)
        {
            Dictionary<ScriptFileType, List<AdvScriptFileInfo>> scriptFiles = new Dictionary<ScriptFileType, List<AdvScriptFileInfo>>();

            List<FileInfo> files = GetFiles(new DirectoryInfo(rootDirectoryPath));

            scriptFiles.Add(ScriptFileType.Script,
                GetScenarioFiles(files, rootDirectoryPath, AdvScriptDataExporter.ScenarioScriptFileExtension));
            scriptFiles.Add(ScriptFileType.Macro,
                GetScenarioFiles(files, rootDirectoryPath, AdvScriptDataExporter.MacroFileExtension));
            scriptFiles.Add(ScriptFileType.Constant,
                GetScenarioFiles(files, rootDirectoryPath, AdvScriptDataExporter.ConstantValueFileExtension));
            return scriptFiles;
        }

        /// <summary>
        /// 全てのスクリプトファイルを取得します。
        /// </summary>
        /// <param name="rootDirectoryPath">ルートとなるディレクトリのパス</param>
        /// <returns></returns>
        public static List<AdvScriptFileInfo> GetScripts(string rootDirectoryPath)
        {
            List<FileInfo> files = GetFiles(new DirectoryInfo(rootDirectoryPath));
            return GetScenarioFiles(files, rootDirectoryPath, AdvScriptDataExporter.ScenarioScriptFileExtension);
        }

        /// <summary>
        /// 全てのマクロ定義ファイルを取得します。
        /// </summary>
        /// <param name="rootDirectoryPath">ルートとなるディレクトリのパス</param>
        /// <returns></returns>
        public static List<AdvScriptFileInfo> GetMacro(string rootDirectoryPath)
        {
            List<FileInfo> files = GetFiles(new DirectoryInfo(rootDirectoryPath));
            return GetScenarioFiles(files, rootDirectoryPath, AdvScriptDataExporter.MacroFileExtension);
        }

        /// <summary>
        /// 全ての定数定義ファイルを取得します。
        /// </summary>
        /// <param name="rootDirectoryPath">ルートとなるディレクトリのパス</param>
        /// <returns></returns>
        public static List<AdvScriptFileInfo> GetConstants(string rootDirectoryPath)
        {
            List<FileInfo> files = GetFiles(new DirectoryInfo(rootDirectoryPath));
            return GetScenarioFiles(files, rootDirectoryPath, AdvScriptDataExporter.ConstantValueFileExtension);
        }

#endregion

#region private methods

        /// <summary>
        /// シナリオ保存ディレクトリ以下に存在するシナリオスクリプトを再帰的に取得します。
        /// </summary>
        /// <param name="files">ファイル情報</param>
        /// <param name="rootDirectoryPath">ルートディレクトリのパス</param>
        /// <param name="extension">拡張子</param>
        /// <returns></returns>
        static List<AdvScriptFileInfo> GetScenarioFiles(List<FileInfo> files, string rootDirectoryPath, string extension)
        {
            List<AdvScriptFileInfo> scriptFiles = new List<AdvScriptFileInfo>();
            
            foreach (FileInfo file in files)
            {
                if (file.Extension != extension)
                {
                    continue;
                }

                scriptFiles.Add(new AdvScriptFileInfo(
                    Path.GetFileNameWithoutExtension(file.Name),
                    file.Extension,
                    file.DirectoryName.Substring(rootDirectoryPath.Length + 1)));
            }
            
            return scriptFiles;
        }

        /// <summary>
        /// シナリオ保存ディレクトリ以下に存在するシナリオスクリプトを再帰的に取得します。
        /// </summary>
        /// <param name="directoryInfo">ディレクトリ情報</param>
        /// <returns></returns>
        static List<FileInfo> GetFiles(DirectoryInfo directoryInfo)
        {
            List<FileInfo> files = new List<FileInfo>();

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                files.Add(file);
            }

            foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
            {
                List<FileInfo> subDirectoryFiles = GetFiles(subDirectory);
                if (subDirectoryFiles.Count > 0)
                {
                    files.AddRange(subDirectoryFiles);
                }
            }

            return files;
        }

#endregion
    }
}
#endif
