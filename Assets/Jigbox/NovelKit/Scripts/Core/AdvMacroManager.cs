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

using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    using MacroInfo = AdvMacroParser.MacroInfo;

    public sealed class AdvMacroManager
    {
#region properties

        /// <summary>マクロ</summary>
        Dictionary<string, MacroInfo> macro = new Dictionary<string, MacroInfo>();

#endregion

#region public methods

        /// <summary>
        /// マクロを記録します。
        /// </summary>
        /// <param name="macroInfo">マクロ情報</param>
        public void Add(MacroInfo macroInfo)
        {
            if (macro.ContainsKey(macroInfo.Macro.Name))
            {
                macro[macroInfo.Macro.Name] = macroInfo;
            }
            else
            {
                macro.Add(macroInfo.Macro.Name, macroInfo);
            }
        }

        /// <summary>
        /// マクロを記録します。
        /// </summary>
        /// <param name="name">マクロ名</param>
        /// <param name="macroInfo">マクロ情報</param>
        public void Add(string name, MacroInfo macroInfo)
        {
            if (macro.ContainsKey(name))
            {
                macro[name] = macroInfo;
            }
            else
            {
                macro.Add(name, macroInfo);
            }
        }

        /// <summary>
        /// マクロを記録します。
        /// </summary>
        /// <param name="macroInfo">マクロ情報</param>
        public void Add(Dictionary<string, MacroInfo> macroInfo)
        {
            foreach (KeyValuePair<string, MacroInfo> info in macroInfo)
            {
                Add(info.Key, info.Value);
            }
        }

        /// <summary>
        /// 記録されているマクロを破棄します。
        /// </summary>
        /// <param name="name">マクロ名</param>
        public void Remove(string name)
        {
            if (macro.ContainsKey(name))
            {
                macro.Remove(name);
            }
        }

        /// <summary>
        /// 記録されているマクロを破棄します。
        /// </summary>
        /// <param name="names">マクロ名</param>
        public void Remove(List<string> names)
        {
            foreach (string name in names)
            {
                Remove(name);
            }
        }

        /// <summary>
        /// マクロを取得します。
        /// </summary>
        /// <param name="name">マクロ名</param>
        /// <returns>マクロが存在しなかった場合、nullが返ります。</returns>
        public MacroInfo Get(string name)
        {
            if (macro.ContainsKey(name))
            {
                return macro[name];
            }
            return null;
        }

        /// <summary>
        /// マクロをクリアします。
        /// </summary>
        public void Clear()
        {
            macro.Clear();
        }

        /// <summary>
        /// マクロからコマンドを生成します。
        /// </summary>
        /// <param name="info">マクロ情報</param>
        /// <param name="macroCommand">マクロを呼び出しているコマンド</param>
        /// <param name="constantValueManager">定数管理クラス</param>
        /// <returns></returns>
        public static List<AdvCommandBase> CreateCommand(MacroInfo info, AdvCommandBase macroCommand, AdvConstantValueManager constantValueManager)
        {
            List<AdvCommandBase> commands = new List<AdvCommandBase>();

            Dictionary<string, string> tempArgs = info.Macro.Args;
            Dictionary<string, string> args = new Dictionary<string, string>();
            int index = 1;

            // デフォルト情報をコマンドの引数で上書き
            foreach (KeyValuePair<string, string> arg in tempArgs)
            {
                if (index < macroCommand.Param.Length)
                {
                    args.Add(arg.Key, macroCommand.Param[index]);
                }
                else
                {
                    args.Add(arg.Key, arg.Value);
                }
                ++index;
            }

            List<string> commandStrings = info.CommandStrings;

            // 引数を文字列に反映
            foreach (KeyValuePair<string, string> arg in args)
            {
                for (int i = 0; i < commandStrings.Count; ++i)
                {
                    if (commandStrings[i].Contains(arg.Key))
                    {
                        commandStrings[i] = commandStrings[i].Replace(arg.Key, arg.Value);
                    }
                }
            }

            foreach (string str in commandStrings)
            {
                AdvCommandBase command = AdvCommandParser.CreateCommand(str, constantValueManager);
                if (command == null)
                {
                    continue;
                }

                commands.Add(command);
            }

            return commands;
        }

#endregion
    }
}
