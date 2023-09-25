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
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    using CommandType = AdvCommandBase.CommandType;

    public static class AdvMacroParser
    {
#region inner classes, enum, and structs

        /// <summary>
        /// マクロ情報
        /// </summary>
        public class MacroInfo
        {
            /// <summary>マクロ</summary>
            public AdvScriptMacro Macro { get; protected set; }

            /// <summary>コマンドの文字列情報</summary>
            protected List<string> commandStrings;

            /// <summary>コマンドの文字列情報</summary>
            public List<string> CommandStrings { get { return new List<string>(commandStrings); } }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="macro">マクロ</param>
            /// <param name="commandStrings">コマンドの文字列情報</param>
            public MacroInfo(AdvScriptMacro macro, List<string> commandStrings)
            {
                Macro = macro;
                this.commandStrings = new List<string>(commandStrings);
            }
        }

#endregion

#region public methods

        /// <summary>
        /// ストリーム情報からマクロを生成します。
        /// </summary>
        /// <param name="stream">マクロファイルのストリーム情報</param>
        /// <param name="resource">対象ファイルのパス</param>
        /// <param name="constantValueManager">定数管理クラス</param>
        /// <returns></returns>
        public static Dictionary<string, MacroInfo> CreateMacro(string[] stream, string resource, AdvConstantValueManager constantValueManager)
        {
            Dictionary<string, MacroInfo> macroInfo = new Dictionary<string, MacroInfo>();
            List<string> commandStrings = new List<string>();

            AdvScriptMacro current = null;
            Dictionary<string, string> currentMacroArgs = new Dictionary<string, string>();
            int macroIndex = 0;

            for (int i = 0; i < stream.Length; ++i)
            {
                if (string.IsNullOrEmpty(stream[i]))
                {
                    continue;
                }
                AdvCommandBase command = new AdvCommandBase(GetString(stream[i], currentMacroArgs), constantValueManager);

                if (command.Type == CommandType.Macro)
                {
                    AdvScriptMacro macro = new AdvScriptMacro(stream[i]);
                    if (string.IsNullOrEmpty(macro.ErrorMessage))
                    {
                        if (current == null)
                        {
                            current = macro;
                            currentMacroArgs = macro.Args;
                            macroIndex = i;
                            continue;
                        }
                        else
                        {
                            if (commandStrings.Count > 0)
                            {
                                if (macroInfo.ContainsKey(current.Name))
                                {
                                    macroInfo[current.Name] = new MacroInfo(current, commandStrings);
                                }
                                else
                                {
                                    macroInfo.Add(current.Name, new MacroInfo(current, commandStrings));
                                }
                                commandStrings.Clear();
                            }
#if UNITY_EDITOR
                            else
                            {
                                current.ErrorMessage = "コマンドの定義が存在しないマクロが定義されました。";
                                LogErrorMacro(macroIndex + 1, current, resource);
                            }
#endif
                            current = macro;
                            currentMacroArgs = macro.Args;
                            macroIndex = i;
                            continue;
                        }
                    }
                }

                if (current == null)
                {
#if UNITY_EDITOR
                    command.ErrorMessage = "マクロ未定義状態でコマンドは追加できません";
                    LogErrorMacro(i + 1, command, resource);
#endif
                    continue;
                }

                if (command.Type == CommandType.ErrorCommand)
                {
#if UNITY_EDITOR
                    LogErrorMacro(i + 1, command, resource);
#endif
                    continue;
                }
                else
                {
                    commandStrings.Add(stream[i]);
                }
            }

            if (commandStrings.Count > 0)
            {
                macroInfo.Add(current.Name, new MacroInfo(current, commandStrings));
            }
#if UNITY_EDITOR
            else
            {
                current.ErrorMessage = "コマンドの定義が存在しないマクロが定義されました。";
                LogErrorMacro(macroIndex + 1, current, resource);
            }
#endif

            return macroInfo;
        }

        /// <summary>
        /// 文字列内の引数を置換した状態の文字列を取得します。
        /// </summary>
        /// <param name="str">元の文字列</param>
        /// <param name="args">引数情報</param>
        /// <returns></returns>
        public static string GetString(string str, Dictionary<string, string> args)
        {
            string[] param = AdvScriptParams.Split(str);

            for (int i = 0; i < param.Length; ++i)
            {
                if (args.ContainsKey(param[i]))
                {
                    param[i] = args[param[i]];
                }
            }

            return AdvScriptParams.Join(param);
        }

        /// <summary>
        /// マクロ中のコマンドのバリデーションを行います。
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <returns></returns>
        public static AdvCommandBase ValidateMcroCommand(AdvCommandBase command)
        {
            switch (command.Type)
            {
                case CommandType.SceneName:
                    return AdvCommandParser.CreateErrorCommand(command, "マクロ中に、シーン名は設定できません。");
                case CommandType.FileLoad:
                    return AdvCommandParser.CreateErrorCommand(command, "マクロ中に、マクロ、定数ファイルの読み込みはできません");
                case CommandType.Variable:
                    return AdvCommandParser.CreateErrorCommand(command, "マクロ中に、変数の設定はできません。");
                case CommandType.ConstantValue:
                    return AdvCommandParser.CreateErrorCommand(command, "マクロ中に、定数の定義はできません。");
            }
            return command;
        }

        /// <summary>
        /// エラー内容を出力します。
        /// </summary>
        /// <param name="lineIndex">対象の行番号</param>
        /// <param name="scriptParam">エラーとなっているマクロ</param>
        /// <param name="resource">対象スクリプトのパス</param>
        public static void LogErrorMacro(int lineIndex, AdvScriptParams scriptParam, string resource)
        {
#if UNITY_EDITOR || NOVELKIT_DEBUG
            string text = string.Empty;
            foreach (string param in scriptParam.BaseParam)
            {
                text += param + " ";
            }

            AdvLog.Error("マクロの定義に失敗しました！"
                + "\n対象ファイル : " + resource
                + "\n" + lineIndex + " : " + text
                + "\nメッセージ : " + scriptParam.ErrorMessage);
#endif
        }

#endregion
    }
}
