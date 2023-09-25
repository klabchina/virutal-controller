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
using System.IO;
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jigbox.NovelKit
{
    using CommandType = AdvCommandBase.CommandType;

    public class AdvScriptDataExporter
    {
#region inner classes, enum, and structs

        public class ExportResult
        {
            /// <summary>成功しているかどうか</summary>
            public bool Succeeded { get; protected set; }

            /// <summary>新しく作成したかどうか</summary>
            public bool HasCreatedNew { get; protected set; }

            /// <summary>元となるスクリプトファイルのパス</summary>
            public string SrcFilePath { get; protected set; }

            /// <summary>出力される実行ファイルのパス</summary>
            public string DstFilePath { get; protected set; }

            /// <summary>出力されるアセットのパス</summary>
            public string DstAssetPath { get; protected set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="succeeded">成功しているかどうか</param>
            /// <param name="hasCreatedNew">新しく作成したかどうか</param>
            /// <param name="srcFilePath">元となるスクリプトファイルのパス</param>
            /// <param name="dstFilePath">出力される実行ファイルのパス</param>
            /// <param name="dstAssetPath">出力されるアセットのパス</param>
            public ExportResult(bool succeeded, bool hasCreatedNew, string srcFilePath, string dstFilePath, string dstAssetPath)
            {
                Succeeded = succeeded;
                HasCreatedNew = hasCreatedNew;
                SrcFilePath = srcFilePath;
                DstFilePath = dstFilePath;
                DstAssetPath = dstAssetPath;
            }
        }

        protected class AnnotationInfo
        {
            /// <summary>記述されている行数</summary>
            public int LineIndex { get; protected set; }

            /// <summary>対象となるコマンド</summary>
            public AdvCommandBase Command { get; protected set; }

            /// <summary>対象ファイルのパス</summary>
            public string TargetPath { get; protected set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="lineIndex">記述されている行数</param>
            /// <param name="command">対象となるコマンド</param>
            /// <param name="targetPath">対象ファイルのパス</param>
            public AnnotationInfo(int lineIndex, AdvCommandBase command, string targetPath)
            {
                LineIndex = lineIndex;
                Command = command;
                TargetPath = targetPath;
            }
        }
        
#endregion

#region constants

        /// <summary>シナリオスクリプトの拡張子</summary>
        public const string ScenarioScriptFileExtension = ".kns";

        /// <summary>マクロファイルの拡張子</summary>
        public const string MacroFileExtension = ".knm";

        /// <summary>定数ファイルの拡張子</summary>
        public const string ConstantValueFileExtension = ".knc";

        /// <summary>実行用ファイルの拡張子</summary>
        protected static readonly string ExecutableFileExtension = ".bytes";

#endregion

#region properties

        /// <summary>シナリオスクリプトの読み込み用インタフェース</summary>
        public IAdvScriptLoader ScriptLoader { get; set; }

        /// <summary>アノテーション扱いのコマンド</summary>
        protected List<AnnotationInfo> annotations = new List<AnnotationInfo>();

#endregion

#region public methods

        /// <summary>
        /// スクリプトファイルから実行用ファイルを出力します。
        /// </summary>
        /// <param name="script">スクリプトファイル情報</param>
        /// <param name="scenarioDirectory">スクリプトファイルの保存ディレクトリ</param>
        /// <param name="exportDirectory">実行用ファイルの出力先ディレクトリ</param>
        /// <returns>出力結果</returns>
        public virtual ExportResult Export(AdvScriptFileInfo script, string scenarioDirectory, string exportDirectory)
        {
            bool isSucceeded = false;
            bool hasCreatedNew = true;
            string srcFilePath = script.GetFilePath(scenarioDirectory);
            string dstFilePath = script.GetExportPath(exportDirectory, ExecutableFileExtension);
            string dstAssetPath = null;

            annotations.Clear();
            IAdvAnnotationProcessor annotationProcessor;
            if (AdvCommandParser.CustomParser is IAdvAnnotationProcessor)
            {
                annotationProcessor = AdvCommandParser.CustomParser as IAdvAnnotationProcessor;
            }
            else
            {
                annotationProcessor = new AdvAnnotationProcessor();
            }
            AdvAnnotationProcessor.UpdateAnotation(annotationProcessor.Annotations);

            if (!File.Exists(srcFilePath))
            {
                return new ExportResult(isSucceeded, hasCreatedNew, srcFilePath, dstFilePath, dstAssetPath);
            }

            try
            {
                bool hasError = false;
                List<string> stream = ReadStream(srcFilePath, script.Extension, ref hasError);

                if (hasError)
                {
                    return new ExportResult(isSucceeded, hasCreatedNew, srcFilePath, dstFilePath, dstAssetPath);
                }

                CreateDstFileAndDirectory(script.GetExprotDirectory(exportDirectory), dstFilePath);
                WriteStream(dstFilePath, stream);

                int assetPathBeginIndex = dstFilePath.IndexOf("Assets/");
                if (assetPathBeginIndex >= 0)
                {
                    dstAssetPath = dstFilePath.Substring(assetPathBeginIndex);
                    hasCreatedNew = !IsExistAsset(script.Name, exportDirectory.Substring(assetPathBeginIndex));
                }

                isSucceeded = true;

                OutputAnnotation(annotationProcessor);
            }
            catch (System.Exception e)
            {
                AdvLog.Error(e.ToString());
            }

            return new ExportResult(isSucceeded, hasCreatedNew, srcFilePath, dstFilePath, dstAssetPath);
        }

        /// <summary>
        /// 対象アセットにDirtyを設定します。
        /// </summary>
        /// <param name="assetPath">アセットのパス</param>
        /// <returns>対象アセットがUnityプロジェクト以下に存在しれいれば参照を、存在していなければ<c>null</c>を返します。</returns>
        public static Object SetDirty(string assetPath)
        {
#if !UNITY_EDITOR
            return null;
#else
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            TextAsset dstFile = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            if (dstFile != null)
            {
                AssetDatabase.ImportAsset(assetPath);
                EditorUtility.SetDirty(dstFile);
            }
            else
            {
                AdvLog.Error("AdvScriptDataConverter.DirtyToScript : Asset not exist!\nPath : " + assetPath);
            }
            return dstFile;
#endif
        }

        /// <summary>
        /// 元となるスクリプトファイルがUnityプロジェクト以下に存在していれば、アセットの参照を返します。
        /// </summary>
        /// <param name="script">スクリプトファイル情報</param>
        /// <param name="scenarioDirectory"></param>
        /// <returns>Unityプロジェクト以下に存在していれば参照を、存在していなければ<c>null</c>を返します。</returns>
        public static Object GetSourceAsset(AdvScriptFileInfo script, string scenarioDirectory)
        {
#if !UNITY_EDITOR
            return null;
#else
            string srcFilePath = script.GetFilePath(scenarioDirectory);
            int assetPathBeginIndex = srcFilePath.IndexOf("Assets/");
            if (assetPathBeginIndex < 0)
            {
                return null;
            }
            return AssetDatabase.LoadAssetAtPath<Object>(srcFilePath.Substring(assetPathBeginIndex));
#endif
        }

#endregion

#region protected methods

        /// <summary>
        /// 対象アセットが存在しているかどうかを返します。
        /// </summary>
        /// <param name="assetName">アセットのパス</param>
        /// <param name="directory">アセットの存在するディレクトリのパス</param>
        /// <returns></returns>
        protected static bool IsExistAsset(string assetName, string directory)
        {
#if !UNITY_EDITOR
            return false;
#else
            // ディレクトリがUnityにとっての隠しフォルダの場合もあるのでIsValidFolderで確認する
            if (string.IsNullOrEmpty(assetName) || !AssetDatabase.IsValidFolder(directory))
            {
                return false;
            }
            return AssetDatabase.FindAssets(assetName + " t:TextAsset", new string[] { directory }).Length > 0;
#endif
        }

        /// <summary>
        /// スクリプトファイルを読み込んで、バリデーションを行ったストリーム情報を取得します。
        /// </summary>
        /// <param name="srcFilePath">スクリプトファイルのパス</param>
        /// <param name="extension">拡張子</param>
        /// <param name="hasError">エラーしているかどうか</param>
        /// <returns>読み込んだストリーム情報</returns>
        protected virtual List<string> ReadStream(string srcFilePath, string extension, ref bool hasError)
        {
            using (StreamReader reader = new StreamReader(srcFilePath, Encoding.UTF8))
            {
                switch (extension)
                {
                    case ScenarioScriptFileExtension:
                        return ValidateScenarioScript(reader, srcFilePath, ref hasError);

                    case MacroFileExtension:
                        return ValidateMacro(reader, srcFilePath, ref hasError);

                    case ConstantValueFileExtension:
                        return ValidateConstantValues(reader, srcFilePath, ref hasError);
                }
            }

            return null;
        }

        /// <summary>
        /// 出力するファイル、ディレクトリの存在確認、生成を行います。
        /// </summary>
        /// <param name="dstDirectoryPath">実行用ファイルの出力先ディレクトリ</param>
        /// <param name="dstFilePath">実行用ファイルのパス</param>
        protected virtual void CreateDstFileAndDirectory(string dstDirectoryPath, string dstFilePath)
        {
            if (!Directory.Exists(dstDirectoryPath))
            {
                Directory.CreateDirectory(dstDirectoryPath);
            }
            if (!File.Exists(dstFilePath))
            {
                FileStream file = File.Create(dstFilePath);
                file.Close();
            }
        }

        /// <summary>
        /// 実行用ファイルに実行用のストリーム情報を書き込みます。
        /// </summary>
        /// <param name="dstFilePath">実行用ファイルのパス</param>
        /// <param name="stream">ストリーム情報</param>
        protected virtual void WriteStream(string dstFilePath, List<string> stream)
        {
            using (StreamWriter writer = new StreamWriter(dstFilePath, false, new UTF8Encoding(false)))
            {
                writer.NewLine = "\n";
                foreach (string line in stream)
                {
                    writer.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// アノテーションを出力します。
        /// </summary>
        /// <param name="annotationProcessor">アノテーションの処理用クラス</param>
        protected virtual void OutputAnnotation(IAdvAnnotationProcessor annotationProcessor)
        {
            foreach (AnnotationInfo annotation in annotations)
            {
                string message = annotationProcessor.GetMessage(annotation.LineIndex, annotation.Command, annotation.TargetPath);
                AdvLog.Warning(message, GetTargetScript(annotation.TargetPath));
            }
        }

        /// <summary>
        /// スクリプトファイルに対してバリデーションを行い、エクスポートする文字列を返します。
        /// </summary>
        /// <param name="reader">ストリーム情報</param>
        /// <param name="srcFilePath">スクリプトファイルのパス</param>
        /// <param name="hasError">エラーしているかどうか</param>
        /// <returns></returns>
        protected virtual List<string> ValidateScenarioScript(StreamReader reader, string srcFilePath, ref bool hasError)
        {
            List<string> stream = new List<string>();
            bool isDelay = false;
            bool isIf = false;
            bool isElse = false;

            string line = string.Empty;
            int index = 0;
            List<string> sceneNames = new List<string>();
            List<AdvCommandBase> commandList = new List<AdvCommandBase>();
            AdvMacroManager macroManager = new AdvMacroManager();
            AdvConstantValueManager constantValueManager = new AdvConstantValueManager();
            AdvScriptVariableManager variableManager = new AdvScriptVariableManager();

            List<string> graphicResources = new List<string>();
            List<string> soundResources = new List<string>();
            List<string> scriptResources = new List<string>();
            List<string> definitionResources = new List<string>();

            // パーサー本体を使うこともできるが、スクリプトと実行用ファイルでは
            // 保持する情報が違うため、同様のパーサーで処理しようとすると
            // 処理が非常に複雑化するため、別で記述している
            while ((line = reader.ReadLine()) != null)
            {
                ++index;
                AdvCommandBase command = AdvCommandParser.CreateCommand(line, constantValueManager);
                if (command == null)
                {
                    continue;
                }
                if (command.Type == CommandType.Annotation)
                {
                    annotations.Add(new AnnotationInfo(index, command, srcFilePath));
                    continue;
                }

                stream.Add(line);

                if (command.Type == CommandType.ErrorCommand)
                {
                    AdvCommandParser.LogErrorCommand(index, command, srcFilePath);
                    hasError = true;
                    continue;
                }

                command = AdvCommandParser.ValidateDelayCommand(command, ref isDelay);
                command = AdvCommandParser.ValidateIfCommand(command, ref isIf, ref isElse);

                switch (command.Type)
                {
                    case CommandType.SceneName:
                        // 先頭文字は識別子なので読み飛ばす
                        string sceneName = command.Param[0].Substring(1);
                        if (sceneNames.Contains(sceneName))
                        {
                            command = AdvCommandParser.CreateErrorCommand(command, "すでに同一のシーン名が定義されています。");
                        }
                        else
                        {
                            sceneNames.Add(sceneName);
                        }
                        commandList.Add(command);
                        break;
                    case CommandType.FileLoad:
                        AdvCommandFileLoad fileLoadCommand = command as AdvCommandFileLoad;

                        switch (fileLoadCommand.FileType)
                        {
                            case AdvCommandFileLoad.LoadFileType.Macro:
                                Dictionary<string, AdvMacroParser.MacroInfo> macro = null;
                                macro = LoadMacro(fileLoadCommand.FilePath, constantValueManager);
                                if (macro == null)
                                {
                                    command = AdvCommandParser.CreateErrorCommand(command, "マクロファイルとして読み込めないファイルが指定されました。");
                                }
                                else
                                {
                                    macroManager.Add(macro);
                                }
                                break;
                            case AdvCommandFileLoad.LoadFileType.ConstantValue:
                                Dictionary<string, string[]> values = null;
                                values = LoadConstantValues(fileLoadCommand.FilePath);
                                if (values == null)
                                {
                                    command = AdvCommandParser.CreateErrorCommand(command, "定数ファイルとして読み込めないファイルが指定されました。");
                                }
                                else
                                {
                                    constantValueManager.Add(values);
                                }
                                break;
                        }
                        break;
                    case CommandType.Variable:
                        {
                            // 引数に設定可能な変数は、マクロ内などでも使用できる性質などがあるため、
                            // 完全なバリデーションが出来ないので、ビルド時の定義チェックはしない

                            AdvCommandVariable variableCommand = command as AdvCommandVariable;
                            string name = variableCommand.Name;
                            string formula = variableCommand.Formula;

                            string errorName = AdvCommandVariable.ConvertVariables(ref formula, AdvCommandBase.VariablePrefix, variableManager);
                            if (!string.IsNullOrEmpty(errorName))
                            {
                                command = AdvCommandParser.CreateErrorCommand(command, "定義されていない変数(" + errorName + ")が指定されました。");
                            }
                            else
                            {
                                int value = AdvFormulaUtil.GetValue(formula);
                                if (value != AdvFormulaUtil.Failed)
                                {
                                    variableManager.Set(name, value);
                                }
                                else
                                {
                                    command = AdvCommandParser.CreateErrorCommand(command, "式を正しく処理できませんでした。");
                                }
                            }
                        }
                        break;
                    case CommandType.Macro:
                        string macroName = command.Param[0].Substring(1);
                        AdvMacroParser.MacroInfo macroInfo = macroManager.Get(macroName);
                        if (macroInfo == null)
                        {
                            command = AdvCommandParser.CreateErrorCommand(command, "定義されていないマクロ(" + macroName + ")が指定されました。");
                            break;
                        }
                        if (macroInfo.Macro.Args.Count < command.Param.Length - 1)
                        {
                            command = AdvCommandParser.CreateErrorCommand(command, "マクロに設定されている以上の引数が指定されました。");
                            break;
                        }
                        commandList.AddRange(AdvMacroManager.CreateCommand(macroInfo, command, constantValueManager));
                        break;
                    case CommandType.ConstantValue:
                        command = AdvCommandParser.CreateErrorCommand(command, "スクリプト内での定数の定義はできません。");
                        break;
                }

                if (command.Type != CommandType.Macro)
                {
                    commandList.Add(command);
                }

                // マクロで複数コマンドが同時に処理される場合があるため、ループで処理
                for (int i = 0; i < commandList.Count; ++i)
                {
                    if (commandList[i].Type == CommandType.ErrorCommand)
                    {
                        AdvCommandParser.LogErrorCommand(index, commandList[i], srcFilePath);
                        hasError = true;
                    }
                    else
                    {
                        // リソース用インタフェースを持つ場合、リソースのリストに追加
                        if (commandList[i] is IAdvCommandResource)
                        {
                            bool isMultiple = commandList[i] is IAdvCommandMultipleResource;

                            if (commandList[i] is IAdvCommandGraphicResource)
                            {
                                IAdvCommandGraphicResource commandResource = commandList[i] as IAdvCommandGraphicResource;
                                GetCommandResources(commandResource.GraphicResource, graphicResources, isMultiple);
                            }
                            if (commandList[i] is IAdvCommandSoundResource)
                            {
                                IAdvCommandSoundResource commandResource = commandList[i] as IAdvCommandSoundResource;
                                GetCommandResources(commandResource.SoundResource, soundResources, isMultiple);
                            }
                            if (commandList[i] is IAdvCommandScriptResource)
                            {
                                IAdvCommandScriptResource commandResource = commandList[i] as IAdvCommandScriptResource;
                                GetCommandResources(commandResource.ScriptResource, scriptResources, isMultiple);
                            }
                            if (commandList[i] is IAdvCommandDefinitionResource)
                            {
                                IAdvCommandDefinitionResource commandResource = commandList[i] as IAdvCommandDefinitionResource;
                                GetCommandResources(commandResource.DefinitionResource, definitionResources, isMultiple);
                            }
                        }
                    }
                }
                commandList.Clear();
            }

            if (isDelay)
            {
                AdvCommandBase errorCommand = new AdvCommandBase(string.Empty);
                errorCommand = AdvCommandParser.CreateErrorCommand(errorCommand, "遅延実行状態のまま、スクリプトを終了することはできません。");
                AdvCommandParser.LogErrorCommand(stream.Count, errorCommand, srcFilePath);
                hasError = true;
            }
            if (isIf)
            {
                AdvCommandBase errorCommand = new AdvCommandBase(string.Empty);
                errorCommand = AdvCommandParser.CreateErrorCommand(errorCommand, "if文を終了しないまま、スクリプトを終了することはできません。");
                AdvCommandParser.LogErrorCommand(stream.Count, errorCommand, srcFilePath);
                hasError = true;
            }

            if (!hasError)
            {
                string resourceHeader = AdvResourceUtil.GetResourceHeader(
                    graphicResources.Count,
                    soundResources.Count,
                    scriptResources.Count,
                    definitionResources.Count);
                stream.Insert(0, resourceHeader);

                int offset = 1;
                if (graphicResources.Count > 0)
                {
                    stream.InsertRange(offset, graphicResources);
                    offset += graphicResources.Count;
                }
                if (soundResources.Count > 0)
                {
                    stream.InsertRange(offset, soundResources);
                    offset += soundResources.Count;
                }
                if (scriptResources.Count > 0)
                {
                    stream.InsertRange(offset, scriptResources);
                    offset += scriptResources.Count;
                }
                if (definitionResources.Count > 0)
                {
                    stream.InsertRange(offset, definitionResources);
                    offset += definitionResources.Count;
                }
            }

            return stream;
        }

        /// <summary>
        /// マクロファイルに対してバリデーションを行い、エクスポートする文字列を返します。
        /// </summary>
        /// <param name="reader">ストリーム情報</param>
        /// <param name="srcFilePath">スクリプトファイルのパス</param>
        /// <param name="hasError">エラーしているかどうか</param>
        /// <returns></returns>
        protected virtual List<string> ValidateMacro(StreamReader reader, string srcFilePath, ref bool hasError)
        {
            List<string> stream = new List<string>();

            string line = string.Empty;
            int index = 0;
            AdvConstantValueManager constantValueManager = new AdvConstantValueManager();
            AdvScriptMacro current = null;
            Dictionary<string, string> currentMacroArgs = new Dictionary<string, string>();
            int macroIndex = 0;
            int commandCount = 0;

            // パーサー本体を使うこともできるが、スクリプトと実行用ファイルでは
            // 保持する情報が違うため、同様のパーサーで処理しようとすると
            // 処理が非常に複雑化するため、別で記述している
            while ((line = reader.ReadLine()) != null)
            {
                ++index;
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                string str = AdvMacroParser.GetString(line, currentMacroArgs);
                AdvCommandBase command = AdvCommandParser.CreateCommand(str, constantValueManager);
                if (command == null)
                {
                    continue;
                }
                if (command.Type == CommandType.Annotation)
                {
                    annotations.Add(new AnnotationInfo(index, command, srcFilePath));
                    continue;
                }

                switch (command.Type)
                {
                    case CommandType.FileLoad:
                        // マクロが定義される前の定数ファイルの読み込みは実行
                        if (current != null)
                        {
                            ++commandCount;
                            break;
                        }
                        AdvCommandFileLoad fileLoadCommand = command as AdvCommandFileLoad;
                        Dictionary<string, string[]> values = null;

                        switch (fileLoadCommand.FileType)
                        {
                            case AdvCommandFileLoad.LoadFileType.Macro:
                                command = AdvCommandParser.CreateErrorCommand(command, "マクロファイル内でマクロファイルを読み込むことはできません。");
                                break;
                            case AdvCommandFileLoad.LoadFileType.ConstantValue:
                                values = LoadConstantValues(fileLoadCommand.FilePath);
                                if (values == null)
                                {
                                    command = AdvCommandParser.CreateErrorCommand(command, "マクロ、定数ファイルとして読み込めないファイルが指定されました。");
                                }
                                else
                                {
                                    constantValueManager.Add(values);
                                }
                                break;
                        }
                        continue;
                    case CommandType.Macro:
                        if (current != null && commandCount == 0)
                        {
                            command.ErrorMessage = "コマンドの定義が存在しないマクロが定義されました。";
                            AdvMacroParser.LogErrorMacro(macroIndex, command, srcFilePath);
                            hasError = true;
                            break;
                        }

                        AdvScriptMacro macro = new AdvScriptMacro(line);
                        if (!string.IsNullOrEmpty(macro.ErrorMessage))
                        {
                            AdvMacroParser.LogErrorMacro(index, macro, srcFilePath);
                            hasError = true;
                        }
                        else
                        {
                            current = macro;
                            currentMacroArgs = macro.Args;
                            macroIndex = index;
                            commandCount = 0;
                        }
                        break;
                    default:
                        ++commandCount;
                        break;
                }

                command = AdvMacroParser.ValidateMcroCommand(command);

                if (command.Type == CommandType.ErrorCommand)
                {
                    AdvMacroParser.LogErrorMacro(index, command, srcFilePath);
                    hasError = true;
                }

                stream.Add(line);
            }

            return stream;
        }

        /// <summary>
        /// コマンドから使用するリソースの情報を取得します。
        /// </summary>
        /// <param name="resource">リソース</param>
        /// <param name="list">リソースを記録するためのリスト</param>
        /// <param name="isMultiple">複数のリソースを含むかどうか</param>
        protected virtual void GetCommandResources(string resource, List<string> list, bool isMultiple)
        {
            if (string.IsNullOrEmpty(resource))
            {
                return;
            }

            if (isMultiple)
            {
                foreach (string res in resource.Split(AdvCommandBase.MultipleResourceDelimiter))
                {
                    if (!string.IsNullOrEmpty(res) && !list.Contains(res))
                    {
                        list.Add(res);
                    }
                }
            }
            else
            {
                if (!list.Contains(resource))
                {
                    list.Add(resource);
                }
            }
        }

        /// <summary>
        /// 定数ファイルに対してバリデーションを行い、エクスポートする文字列を返します。
        /// </summary>
        /// <param name="reader">ストリーム情報</param>
        /// <param name="srcFilePath">スクリプトファイルのパス</param>
        /// <param name="hasError">エラーしているかどうか</param>
        /// <returns></returns>
        protected virtual List<string> ValidateConstantValues(StreamReader reader, string srcFilePath, ref bool hasError)
        {
            List<string> stream = new List<string>();

            string line = string.Empty;
            int index = 0;

            // パーサー本体を使うこともできるが、スクリプトと実行用ファイルでは
            // 保持する情報が違うため、同様のパーサーで処理しようとすると
            // 処理が非常に複雑化するため、別で記述している
            while ((line = reader.ReadLine()) != null)
            {
                ++index;
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                AdvCommandBase command = new AdvCommandBase(line);
                if (command.Type == CommandType.Comment)
                {
                    continue;
                }
                if (command.Type == CommandType.Annotation)
                {
                    annotations.Add(new AnnotationInfo(index, command, srcFilePath));
                    continue;
                }

                stream.Add(line);

                AdvScriptConstantValue value = new AdvScriptConstantValue(line);
                if (!string.IsNullOrEmpty(value.ErrorMessage))
                {
                    AdvConstantValueParser.LogErrorConstantValue(index, value, srcFilePath);
                    hasError = true;
                }
            }

            return stream;
        }

        /// <summary>
        /// マクロファイルを読み込みます。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        /// <param name="constantValueManager">定数管理クラス</param>
        /// <returns></returns>
        protected virtual Dictionary<string, AdvMacroParser.MacroInfo> LoadMacro(string resource, AdvConstantValueManager constantValueManager)
        {
            string[] data = ScriptLoader.Load(resource);
            if (data.Length == 0)
            {
                return null;
            }

            Dictionary<string, AdvMacroParser.MacroInfo> macro = AdvMacroParser.CreateMacro(data, resource, constantValueManager);
            return macro;
        }

        /// <summary>
        /// 定数ファイルを読み込みます。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        /// <returns></returns>
        protected virtual Dictionary<string, string[]> LoadConstantValues(string resource)
        {
            string[] data = ScriptLoader.Load(resource);
            if (data.Length == 0)
            {
                return null;
            }

            Dictionary<string, string[]> values = AdvConstantValueParser.CreateConstantValues(data, resource);
            return values;
        }

        /// <summary>
        /// スクリプトファイルの参照を返します。
        /// </summary>
        /// <param name="resource">スクリプトのパス</param>
        /// <returns>Unityプロジェクト以下に存在しれいれば参照を、存在していなければ<c>null</c>を返します。</returns>
        protected static Object GetTargetScript(string resource)
        {
            int assetPathBeginIndex = resource.IndexOf("Assets/");
            if (assetPathBeginIndex < 0)
            {
                return null;
            }
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<Object>(resource.Substring(assetPathBeginIndex));
#else
            return null;
#endif
        }
        
#endregion
    }
}
#endif
