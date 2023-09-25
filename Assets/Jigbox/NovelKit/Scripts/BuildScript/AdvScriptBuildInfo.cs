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
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jigbox.NovelKit
{
    using ScriptFileType = AdvScriptFileSearcher.ScriptFileType;

    /// <summary>
    /// スクリプトのビルド情報
    /// </summary>
    public class AdvScriptBuildInfo
    {
        /// <summary>ビルド情報の区切り文字(Split用)</summary>
        protected static readonly char DelimiterCharacter = '|';

        /// <summary>ビルド情報の区切り文字(Join用)</summary>
        protected static readonly string Delimiter = "|";

        /// <summary>定数ファイルのビルド情報を表す識別文字</summary>
        protected const string FileTypeConstant = "c";

        /// <summary>マクロファイルのビルド情報を表す識別文字</summary>
        protected const string FileTypeMacro = "m";

        /// <summary>スクリプトファイルのビルド情報を表す識別文字</summary>
        protected const string FileTypeScript = "s";

        /// <summary>インスタンス</summary>
        static AdvScriptBuildInfo instance;

        /// <summary>インスタンス</summary>
        public static AdvScriptBuildInfo Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdvScriptBuildInfo();
                }

                return instance;
            }
        }

        /// <summary>データの保存ファイルのパス</summary>
        protected virtual string DataPath { get { return Application.persistentDataPath + "/novelkit_build_info.dat"; } }

        /// <summary>ビルド情報が有効かどうか</summary>
        bool isEnable = true;

        /// <summary>
        /// <para>ビルド情報が有効かどうか</para>
        /// <para>バッチビルドなどでビルド情報の管理が不要な場合は、<c>false</c>を指定してください。</para>
        /// </summary>
        public bool IsEnable { get { return isEnable; } set { isEnable = value; } }

        /// <summary>定数ファイルの比較用のテーブル</summary>
        Dictionary<string, string> constantTable = new Dictionary<string, string>();

        /// <summary>マクロファイルの比較用のテーブル</summary>
        Dictionary<string, string> macroTable = new Dictionary<string, string>();

        /// <summary>スクリプトファイルの比較用のテーブル</summary>
        Dictionary<string, string> scriptTable = new Dictionary<string, string>();

        /// <summary>
        /// ビルド情報を保存します。
        /// </summary>
        public virtual void Save()
        {
            if (!isEnable)
            {
                return;
            }

            string path = DataPath;
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, string> element in constantTable)
                {
                    string[] str = new string[] { FileTypeConstant, element.Key, element.Value };
                    writer.WriteLine(string.Join(Delimiter, str));
                }

                foreach (KeyValuePair<string, string> element in macroTable)
                {
                    string[] str = new string[] { FileTypeMacro, element.Key, element.Value };
                    writer.WriteLine(string.Join(Delimiter, str));
                }

                foreach (KeyValuePair<string, string> element in scriptTable)
                {
                    string[] str = new string[] { FileTypeScript, element.Key, element.Value };
                    writer.WriteLine(string.Join(Delimiter, str));
                }
            }
        }

        /// <summary>
        /// ビルド情報を読み込みます。
        /// </summary>
        public virtual void Load()
        {
            if (!isEnable)
            {
                return;
            }

            string path = DataPath;
            if (!File.Exists(path))
            {
                return;
            }

            Clear();

            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] data = line.Split(DelimiterCharacter);

                    Dictionary<string, string> table = null;
                    switch (data[0])
                    {
                        case FileTypeConstant:
                            table = constantTable;
                            break;
                        case FileTypeMacro:
                            table = macroTable;
                            break;
                        case FileTypeScript:
                            table = scriptTable;
                            break;
                        default:
                            continue;
                    }

                    table.Add(data[1], data[2]);
                }
            }
        }

        /// <summary>
        /// スクリプトのビルドが必要かどうかを返します。
        /// </summary>
        /// <param name="type">スクリプトファイルの種類</param>
        /// <param name="path">スクリプトファイルのパス</param>
        /// <param name="targetFilePath">ビルド対象のファイルのフルパス</param>
        /// <returns>更新が必要な場合、<c>true</c>を返し、不要な場合、<c>false</c>を返します。</returns>
        public virtual bool IsNeedBuild(ScriptFileType type, string path, string targetFilePath)
        {
            if (!isEnable)
            {
                return true;
            }

            Dictionary<string, string> table = null;
            switch (type)
            {
                case ScriptFileType.Constant:
                    table = constantTable;
                    break;
                case ScriptFileType.Macro:
                    table = macroTable;
                    break;
                case ScriptFileType.Script:
                    table = scriptTable;
                    break;
                default:
                    return false;
            }

            if (table.ContainsKey(path))
            {
                return table[path] != File.GetLastWriteTime(targetFilePath).ToString();
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// ビルド情報を更新します。
        /// </summary>
        /// <param name="type">スクリプトファイルの種類</param>
        /// <param name="path">スクリプトファイルのパス</param>
        /// <param name="targetFilePath">ビルド対象のファイルのフルパス</param>
        public virtual void UpdateBuildInfo(ScriptFileType type, string path, string targetFilePath)
        {
            if (!isEnable)
            {
                return;
            }

            Dictionary<string, string> table = null;
            switch (type)
            {
                case ScriptFileType.Constant:
                    table = constantTable;
                    break;
                case ScriptFileType.Macro:
                    table = macroTable;
                    break;
                case ScriptFileType.Script:
                    table = scriptTable;
                    break;
                default:
                    return;
            }

            string timestamp = File.GetLastWriteTime(targetFilePath).ToString();

            if (table.ContainsKey(path))
            {
                table[path] = timestamp;
            }
            else
            {
                table.Add(path, timestamp);
            }
        }

        /// <summary>
        /// ビルド情報をクリアします。
        /// </summary>
        public virtual void Clear()
        {
            constantTable.Clear();
            macroTable.Clear();
            scriptTable.Clear();
        }
    }
}
#endif
