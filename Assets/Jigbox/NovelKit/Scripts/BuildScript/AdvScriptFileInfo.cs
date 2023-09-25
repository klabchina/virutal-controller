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
using System.IO;

namespace Jigbox.NovelKit
{
    public class AdvScriptFileInfo
    {
#region properties

        /// <summary>ファイル名</summary>
        public string Name { get; protected set; }

        /// <summary>拡張子</summary>
        public string Extension { get; protected set; }
        
        /// <summary>サブディレクトリの相対パス</summary>
        public string SubDirectory { get; protected set; }

        /// <summary>ディレクトリ、拡張しを含めたファイル名</summary>
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(SubDirectory))
                {
                    return Path.Combine(SubDirectory, Name + Extension);
                }
                else
                {
                    return Name + Extension;
                }
            }
        }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <param name="extension">拡張子</param>
        /// <param name="subDirectory">サブディレクトリの相対パス</param>
        public AdvScriptFileInfo(string name, string extension,string subDirectory)
        {
            Name = name;
            Extension = extension;
            SubDirectory = subDirectory;
        }

        /// <summary>
        /// ファイルの絶対パスを取得します。
        /// </summary>
        /// <param name="rootDirectory">ルートとなるディレクトリのパス</param>
        /// <returns></returns>
        public string GetFilePath(string rootDirectory)
        {
            string directoryPath = string.IsNullOrEmpty(SubDirectory)
            ? rootDirectory
            : Path.Combine(rootDirectory, SubDirectory);
            return Path.Combine(directoryPath, Name + Extension);
        }

        /// <summary>
        /// 出力先のパスを取得します。
        /// </summary>
        /// <param name="exportDirectory">出力先のディレクトリのパス</param>
        /// <param name="extension">拡張子</param>
        /// <returns></returns>
        public string GetExportPath(string exportDirectory, string extension)
        {
            string dstDirectoryPath = string.IsNullOrEmpty(SubDirectory)
            ? exportDirectory
            : Path.Combine(exportDirectory, SubDirectory);
            return Path.Combine(dstDirectoryPath, Name + extension);
        }

        /// <summary>
        /// 出力先のディレクトリのパスを取得します。
        /// </summary>
        /// <param name="exportDirectory">出力先のディレクトリのパス</param>
        /// <returns></returns>
        public string GetExprotDirectory(string exportDirectory)
        {
            return string.IsNullOrEmpty(SubDirectory)
            ? exportDirectory
            : Path.Combine(exportDirectory, SubDirectory);
        }

#endregion
    }
}
#endif
