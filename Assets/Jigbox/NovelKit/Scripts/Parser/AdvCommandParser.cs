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

    public static class AdvCommandParser
    {
#region properties

        /// <summary>カスタムコマンドのパーサー</summary>
        public static IAdvCustomCommandParser CustomParser { get; set; }

#endregion

#region public methods

        /// <summary>
        /// テキストからコマンドを生成します。
        /// </summary>
        /// <param name="text">コマンドの元となるテキスト</param>
        /// <param name="constantValueManager">定数管理クラス</param>
        /// <returns></returns>
        public static AdvCommandBase CreateCommand(string text, AdvConstantValueManager constantValueManager)
        {
            AdvCommandBase commandBase = new AdvCommandBase(text, constantValueManager);
            // 定数展開に失敗していた場合はエラーとなっているためそのままコマンドを返す
            if (commandBase.Type == CommandType.ErrorCommand)
            {
                return commandBase;
            }

            AdvCommandBase command = ParseDefault(commandBase);

            if (commandBase.Type == CommandType.Macro)
            {
                if (commandBase.MaybeExistPackableParamter())
                {
                    command = new AdvCommandPackableMacro(text);
                }
            }

            // カスタムコマンド用のパーサーが設定されていた場合、既存コマンドを上書きできる
            if (CustomParser != null)
            {
                // 以下の種類のコマンドは上書き不可
                switch (commandBase.Type)
                {
                    case CommandType.None:
                    case CommandType.Comment:
                    case CommandType.Annotation:
                    case CommandType.SceneName:
                    case CommandType.Macro:
                    case CommandType.ConstantValue:
                    case CommandType.FileLoad:
                    case CommandType.Variable:
                    case CommandType.VariableParam:
                        return command;
                }

                command = CustomParser.Parse(command);
            }

            return command;
        }

        /// <summary>
        /// スクリプトのストリーム情報からコマンドを生成します。
        /// </summary>
        /// <param name="engine">シナリオ制御統合コンポーネント</param>
        /// <param name="stream">スクリプトのストリーム情報</param>
        /// <param name="resource">対象スクリプトのパス</param>
        /// <returns></returns>
        public static Dictionary<string, List<AdvCommandBase>> CreateCommands(AdvMainEngine engine, string[] stream, string resource)
        {
            Dictionary<string, List<AdvCommandBase>> commands = new Dictionary<string, List<AdvCommandBase>>();

            string currentScene = string.Empty;
            bool isDelay = false;
            bool isIf = false;
            bool isElse = false;
            List<AdvCommandBase> commandList = new List<AdvCommandBase>();

            int beginIndex = 0;
            if (AdvResourceUtil.IsHeader(stream[0]))
            {
                // ヘッダーとヘッダー以下に含まれるリソース情報分を飛ばす
                beginIndex = AdvResourceUtil.GetResourcesTotalCount(stream[0]) + 1;
            }

            AdvConstantValueManager constantValueManager = engine.ConstantValueManager;
            AdvScriptVariableManager variableParamManager = engine.VariableParamManager;

            // 文字列情報を展開してコマンドを生成
            for (int i = beginIndex; i < stream.Length; ++i)
            {
                if (string.IsNullOrEmpty(stream[i]))
                {
                    continue;
                }

                AdvCommandBase command = CreateCommand(stream[i], constantValueManager);

                if (command == null)
                {
                    continue;
                }
                if (command.Type == CommandType.ErrorCommand)
                {
#if UNITY_EDITOR || NOVELKIT_EDITOR
                    // エディタ実行時以外はコマンドを記録しない(する必要が無い)
                    LogErrorCommand(i + 1, command, resource);
                    commands[currentScene].Add(command);
#endif
                    continue;
                }

                command = ValidateDelayCommand(command, ref isDelay);
                command = ValidateIfCommand(command, ref isIf, ref isElse);

                switch (command.Type)
                {
                    case CommandType.SceneName:
                        // 先頭文字は識別子なので読み飛ばす
                        string sceneName = command.Param[0].Substring(1);
                        if (!commands.ContainsKey(sceneName))
                        {
                            currentScene = sceneName;
                            commands.Add(sceneName, new List<AdvCommandBase>());
#if UNITY_EDITOR || NOVELKIT_EDITOR
                            // エディタ実行時のみ記録
                            commandList.Add(command);
#endif
                        }
                        // シーン名の重複定義は不正
                        else
                        {
                            command = CreateErrorCommand(command, "すでに同一のシーン名が定義されています。");
#if UNITY_EDITOR || NOVELKIT_EDITOR
                            // エディタ実行時のみ記録
                            commandList.Add(command);
#endif
                        }
                        break;
                    case CommandType.FileLoad:
                        // エラーしている場合、ここを通らないのでnullチェックはしない
                        AdvCommandFileLoad fileLoadCommand = command as AdvCommandFileLoad;

                        switch (fileLoadCommand.FileType)
                        {
                            case AdvCommandFileLoad.LoadFileType.Macro:
                                engine.LoadMacro(fileLoadCommand.FilePath);
                                break;
                            case AdvCommandFileLoad.LoadFileType.ConstantValue:
                                engine.LoadConstantValues(fileLoadCommand.FilePath);
                                break;
                        }
                        commandList.Add(command);
                        break;
                    case CommandType.Macro:
                        string macroName = command.Param[0].Substring(1);
                        AdvMacroParser.MacroInfo macroInfo = engine.MacroManager.Get(macroName);
                        if (macroInfo == null)
                        {
                            command = CreateErrorCommand(command, "定義されていないマクロ(" + macroName + ")が指定されました。");
                            commandList.Add(command);
                            break;
                        }
                        if (macroInfo.Macro.Args.Count < command.Param.Length - 1)
                        {
                            command = CreateErrorCommand(command, "マクロに設定されている以上の引数が指定されました。");
                            commandList.Add(command);
                            break;
                        }
                        commandList.AddRange(AdvMacroManager.CreateCommand(macroInfo, command, constantValueManager));
                        break;
                    case CommandType.ConstantValue:
                        command = CreateErrorCommand(command, "スクリプト内での定数の定義はできません。");
#if UNITY_EDITOR || NOVELKIT_EDITOR
                        // エディタ実行時のみ記録
                        commandList.Add(command);
#endif
                        break;
                    default:
                        commandList.Add(command);
                        break;
                }

                // マクロで複数コマンドが同時に処理される場合があるため、ループで処理
                for (int j = 0; j < commandList.Count; ++j)
                {
                    // ラベル未設定状態ではコマンドは読み込み不可
                    if (string.IsNullOrEmpty(currentScene))
                    {
                        commandList[j] = CreateErrorCommand(commandList[j], "ラベルが未設定の状態でコマンドが読み込まれました。");
                    }
                    else
                    {
                        // 選択肢の場合は連続判定を行う
                        if (commandList[j].Type == CommandType.Select)
                        {
                            int commandCount = commands[currentScene].Count;
                            while (commandCount > 0)
                            {
                                --commandCount;
                                AdvCommandBase lastCommand = commands[currentScene][commandCount];
                                switch (lastCommand.Type)
                                {
                                    case CommandType.If:
                                    case CommandType.Else:
                                    case CommandType.EndIf:
                                        continue;
                                    case CommandType.Select:
                                        AdvCommandSelect lastSelectCommand = lastCommand as AdvCommandSelect;
                                        lastSelectCommand.EnableContinual();
                                        commandCount = 0;
                                        break;
                                }
                            }
                        }
                    }

                    if (commandList[j].Type == CommandType.ErrorCommand)
                    {
#if UNITY_EDITOR || NOVELKIT_EDITOR
                        LogErrorCommand(i + 1, commandList[j], resource);
#else
                        // エディタ実行時以外はコマンドを記録しない(する必要が無い)
                        continue;
#endif
                    }

                    commands[currentScene].Add(commandList[j]);
                }
                commandList.Clear();
            }

#if UNITY_EDITOR
            if (isDelay)
            {
                AdvCommandBase errorCommand = new AdvCommandBase(string.Empty);
                LogErrorCommand(stream.Length, errorCommand, resource);
                commands[currentScene].Add(CreateErrorCommand(errorCommand, "遅延実行状態のまま、スクリプトを終了することはできません。"));
            }
            if (isIf)
            {
                AdvCommandBase errorCommand = new AdvCommandBase(string.Empty);
                LogErrorCommand(stream.Length, errorCommand, resource);
                commands[currentScene].Add(CreateErrorCommand(errorCommand, "ifコマンドを終了しないまま、スクリプトを終了することはできません。"));
            }
#endif

            return commands;
        }

        /// <summary>
        /// 遅延実行状態でのコマンドのバリデーションを行います。
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <param name="isDelay">遅延状態かどうか</param>
        /// <returns></returns>
        public static AdvCommandBase ValidateDelayCommand(AdvCommandBase command, ref bool isDelay)
        {
            string errorMessage = string.Empty;
            // 遅延処理のバリデーション
            if (isDelay)
            {
                switch (command.Type)
                {
                    case CommandType.SceneName:
                        errorMessage = "遅延実行中に、シーン名は設定できません。";
                        break;
                    case CommandType.FileLoad:
                        errorMessage = "遅延実行中に、マクロ、定数ファイルの読み込みはできません。";
                        break;
                    case CommandType.Text:
                        errorMessage = "遅延実行中に、テキストコマンドは使用できません。";
                        break;
                    case CommandType.Wait:
                        errorMessage = "遅延実行中に、待機コマンドは使用できません。";
                        break;
                    case CommandType.Delay:
                        errorMessage = "遅延実行中に、更にdelayコマンドを使用することはできません。";
                        break;
                    case CommandType.Select:
                        errorMessage = "遅延実行中に、selectコマンドは使用できません。";
                        break;
                    case CommandType.GoTo:
                        errorMessage = "遅延実行中に、gotoコマンドは使用できません。";
                        break;
                    case CommandType.Fade:
                        errorMessage = "遅延実行中に、fadeコマンドは使用できません。";
                        break;
                    case CommandType.EndDelay:
                        isDelay = false;
                        break;
                }
            }
            else
            {
                if (command.Type == CommandType.Delay)
                {
                    isDelay = true;
                }
                else if (command.Type == CommandType.EndDelay)
                {
                    errorMessage = "遅延実行中でないため、enddelayコマンドは使用できません。";
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                command = CreateErrorCommand(command, errorMessage);
            }

            return command;
        }

        /// <summary>
        /// ifコマンド状態でのコマンドのバリデーションを行います。
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <param name="isIf">if文状態かどうか</param>
        /// <param name="isElse">else状態かどうか</param>
        /// <returns></returns>
        public static AdvCommandBase ValidateIfCommand(AdvCommandBase command, ref bool isIf, ref bool isElse)
        {
            string errorMessage = string.Empty;
            if (isIf)
            {
                switch (command.Type)
                {
                    case CommandType.SceneName:
                        errorMessage = "ifコマンド状態で、シーン名は設定できません。";
                        break;
                    case CommandType.If:
                        errorMessage = "ifコマンド状態で、別なifコマンドは使用できません。";
                        break;
                    case CommandType.Else:
                        if (isElse)
                        {
                            errorMessage = "else状態で、別なelseコマンドは使用できません。";
                        }
                        else
                        {
                            isElse = true;
                        }
                        break;
                    case CommandType.EndIf:
                        isIf = false;
                        isElse = false;
                        break;
                }
            }
            else
            {
                switch (command.Type)
                {
                    case CommandType.If:
                        isIf = true;
                        break;
                    case CommandType.Else:
                        errorMessage = "ifコマンドを設定しない状態で、elseコマンドは使用できません。";
                        break;
                    case CommandType.EndIf:
                        errorMessage = "ifコマンドを設定しない状態で、endifコマンドは使用できません。";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                command = CreateErrorCommand(command, errorMessage);
            }

            return command;
        }

        /// <summary>
        /// エラー用コマンドを生成します。
        /// </summary>
        /// <param name="command">元のコマンド</param>
        /// <param name="message">エラーメッセージ</param>
        /// <returns></returns>
        public static AdvCommandBase CreateErrorCommand(AdvCommandBase command, string message)
        {
            AdvCommandBase errorCommand = new AdvCommandBase(command, CommandType.ErrorCommand);
            errorCommand.ErrorMessage = message;
            return errorCommand;
        }

        /// <summary>
        /// エラー内容を出力します。
        /// </summary>
        /// <param name="lineIndex">対象コマンドの行番号</param>
        /// <param name="command">エラーとなっているコマンド</param>
        /// <param name="resource">対象スクリプトのパス</param>
        public static void LogErrorCommand(int lineIndex, AdvCommandBase command, string resource)
        {
#if UNITY_EDITOR || NOVELKIT_DEBUG
            System.Type commandType = command.GetType();
            string format = "コマンドの生成に失敗しました！\n対象ファイル : {0}\n{1} : {2}";
            if (commandType != typeof(AdvCommandBase))
            {
                format += "\nCommand : {3}";
            }
            format += "\n<color=red>メッセージ : {4}</color>\n";

            string message = string.Format(
                format,
                resource,
                lineIndex,
                AdvScriptParams.Join(command.BaseParam),
                commandType,
                command.ErrorMessage);

            AdvLog.Error(message, GetSourceScript(resource));
#endif
        }

#endregion

#region private methods

        /// <summary>
        /// 基礎定義コマンドとしてパースを行います。
        /// </summary>
        /// <param name="commandBase">コマンド</param>
        /// <returns></returns>
        static AdvCommandBase ParseDefault(AdvCommandBase commandBase)
        {
            switch (commandBase.Type)
            {
                case CommandType.None:
                case CommandType.Comment:
                    return null;

                case CommandType.SceneName:
                case CommandType.Macro:
                case CommandType.ConstantValue:
                case CommandType.Annotation:
                case CommandType.ErrorCommand:
                    return commandBase;

                case CommandType.FileLoad:
                    return new AdvCommandFileLoad(commandBase);
                    
                case CommandType.Variable:
                    return new AdvCommandVariable(commandBase);

                case CommandType.VariableParam:
                    return new AdvCommandVariableParam(commandBase);
            }
            
            AdvCommandBase command;

            // オブジェクト系コマンドのパースを実行
            command = AdvObjectCommandParser.Parse(commandBase);
            if (command != null)
            {
                return command;
            }

            // オブジェクト系以外のコマンドのパースを実行
            command = AdvDefaultCommandParser.Parse(commandBase);
            return command;
        }

#if UNITY_EDITOR || NOVELKIT_DEBUG

        /// <summary>
        /// 元となるスクリプトファイルの参照を返します。
        /// </summary>
        /// <param name="resource">スクリプトのパス</param>
        /// <returns>Unityプロジェクト以下に存在しれいれば参照を、存在していなければ<c>null</c>を返します。</returns>
        static Object GetSourceScript(string resource)
        {
            int assetPathBeginIndex = resource.IndexOf("Assets/");
            if (assetPathBeginIndex < 0)
            {
                return null;
            }
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(resource.Substring(assetPathBeginIndex));
#else
            return null;
#endif
        }

#endif

#endregion
    }
}
