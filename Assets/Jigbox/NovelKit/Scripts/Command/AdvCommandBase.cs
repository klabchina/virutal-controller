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
    public class AdvCommandBase : AdvScriptParams
    {
#region inner classes, enum, and structs

        /// <summary>コマンドの種類</summary>
        public enum CommandType
        {
            /// <summary>分類なし</summary>
            None,
            /// <summary>シナリオのシーン名</summary>
            SceneName,
            /// <summary>マクロ、定数ファイル</summary>
            FileLoad,
            /// <summary>変数、式</summary>
            Variable,
            /// <summary>マクロ</summary>
            Macro,
            /// <summary>定数</summary>
            ConstantValue,
            /// <summary>コマンドの引数として使用可能な変数</summary>
            VariableParam,
            /// <summary>コマンド</summary>
            Command,
            /// <summary>オブジェクト操作コマンド</summary>
            ObjectCommand,
            /// <summary>テキストコマンド</summary>
            Text,
            /// <summary>待機状態コマンド</summary>
            Wait,
            /// <summary>遅延実行コマンド</summary>
            Delay,
            /// <summary>遅延実行終了コマンド</summary>
            EndDelay,
            /// <summary>選択肢コマンド</summary>
            Select,
            /// <summary>ラベル、ファイル移動コマンド</summary>
            GoTo,
            /// <summary>フェードコマンド</summary>
            Fade,
            /// <summary>リソースの事前読み込み</summary>
            Preload,
            /// <summary>条件分岐コマンド:条件一致</summary>
            If,
            /// <summary>条件分岐コマンド:条件不一致</summary>
            Else,
            /// <summary>条件分岐コマンド:終了</summary>
            EndIf,
            /// <summary>コメント</summary>
            Comment,
            /// <summary>注釈コメント</summary>
            Annotation,
            /// <summary>その他のコマンド</summary>
            Other,
            /// <summary>デバッグ用コマンド</summary>
            Debug,
            /// <summary>不正コマンド</summary>
            ErrorCommand,
            /// <summary>カスタムコマンド</summary>
            Custom,
        }

        /// <summary>
        /// 符号付きの整数パラメータ情報
        /// </summary>
        protected class ValueInfoInt : ValueInfo<int>
        {
            /// <summary>
            /// 文字列から値をパースします。
            /// </summary>
            /// <param name="str">元となる文字列</param>
            /// <param name="value">パースに成功した場合に値を格納するデータ</param>
            /// <returns>パースに成功した場合、trueを返します。</returns>
            protected override bool TryParse(string str, out int value)
            {
                return int.TryParse(str, out value);
            }
        }

        /// <summary>
        /// 符号付きの実数パラメータ情報
        /// </summary>
        protected class ValueInfoFloat : ValueInfo<float>
        {
            /// <summary>
            /// 文字列から値をパースします。
            /// </summary>
            /// <param name="str">元となる文字列</param>
            /// <param name="value">パースに成功した場合に値を格納するデータ</param>
            /// <returns>パースに成功した場合、trueを返します。</returns>
            protected override bool TryParse(string str, out float value)
            {
                return float.TryParse(str, out value);
            }
        }

        /// <summary>
        /// パラメータの値情報
        /// </summary>
        protected abstract class ValueInfo<T>
        {
            /// <summary>オフセットとして扱うかどうか</summary>
            public bool IsOffset { get; protected set; }

            /// <summary>値</summary>
            public T Value { get; protected set; }

            /// <summary>
            /// 文字列から値をパースします。
            /// </summary>
            /// <param name="str">元となる文字列</param>
            /// <returns>パースに成功した場合、trueを返します。</returns>
            public bool Parse(string str)
            {
                // 1文字目が'/'の場合、値をオフセットとして扱う
                IsOffset = str[0] == '/';

                T value;
                bool result = TryParse(IsOffset ? str.Substring(1) : str, out value);
                if (result)
                {
                    Value = value;
                }
                return result;
            }

            /// <summary>
            /// 文字列から値をパースします。
            /// </summary>
            /// <param name="str">元となる文字列</param>
            /// <param name="value">パースに成功した場合に値を格納するデータ</param>
            /// <returns>パースに成功した場合、trueを返します。</returns>
            protected abstract bool TryParse(string str, out T value);
        }

#endregion

#region constants

        /// <summary>シナリオのシーン名のプレフィックス</summary>
        public const char SceneNamePrefix = '#';

        /// <summary>定数、マクロファイルのプレフィックス</summary>
        public const char FileLoadPrefix = '&';

        /// <summary>変数、式のプレフィックス</summary>
        public const char VariablePrefix = '$';

        /// <summary>マクロのプレフィックス</summary>
        public const char MacroPrefix = '%';

        /// <summary>定数のプレフィックス</summary>
        public const char ConstantValuePrefix = '@';

        /// <summary>コマンドの引数として使用可能な引数のプレフィックス</summary>
        public const char VariableParamPrefix = '_';

        /// <summary>コメントのプレフィックス</summary>
        public const char CommentPrefix = '/';
        
        /// <summary>コマンドを連続処理する際に付けるパラメータ</summary>
        protected static readonly string ContinualCommandParameter = "+";

        /// <summary>同種のリソースを複数扱うコマンドでリソースを分割する区切り文字</summary>
        public static readonly char MultipleResourceDelimiter = '|';

#endregion

#region properties
        
        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected virtual int MinParameterCount { get { return 1; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected virtual int MaxParameterCount { get { return 1; } }
        
        /// <summary>コマンドの種類</summary>
        public CommandType Type { get; protected set; }

#endregion

#region public methods

        /// <summary>
        /// <para>コンストラクタ</para>
        /// <para>※定数は展開されません</para>
        /// </summary>
        /// <param name="str">コマンドとして解析する文字列</param>
        public AdvCommandBase(string str) : base(str)
        {
            CheckType();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="str">コマンドとして解析する文字列</param>
        /// <param name="constantValueManager">定数管理クラス</param>
        public AdvCommandBase(string str, AdvConstantValueManager constantValueManager) : base(str)
        {
            CheckType();

            // 定数定義では相互参照になって破綻するため、定数は展開できない
            // コメントやアノテーションの場合も特に処理しない
            if (Type == CommandType.ConstantValue||
                Type == CommandType.Comment ||
                Type == CommandType.Annotation)
            {
                return;
            }

            bool isDeploied = false;
            List<string> deploiedParam = new List<string>();
            foreach (string param in Param)
            {
                if (param[0] == ConstantValuePrefix)
                {
                    string valueName = param.Substring(1);
                    string[] constantValues = constantValueManager.Get(valueName);
                    if (constantValues == null)
                    {
                        Type = CommandType.ErrorCommand;
                        ErrorMessage = "定義されていない定数(" + valueName + ")が指定されました。";
                        return;
                    }
                    else
                    {
                        isDeploied = true;
                        foreach (string constantValue in constantValues)
                        {
                            deploiedParam.Add(constantValue);
                        }
                    }
                }
                else
                {
                    deploiedParam.Add(param);
                }
            }

            if (isDeploied)
            {
                Param = deploiedParam.ToArray();
            }
        }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandBase(AdvCommandBase command) : base(command.BaseParam)
        {
            // コマンドの上書きする場合、エラー扱いのままだと処理できなくなるので、
            // ErrorCommandの場合は一旦Custom扱いとして処理を進める
            Type = command.Type == CommandType.ErrorCommand ? CommandType.Custom : command.Type;
            IsValidParameterCount();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        /// <param name="type">コマンドの種類</param>
        public AdvCommandBase(AdvCommandBase command, CommandType type) : base(command.BaseParam)
        {
            Type = type;
            IsValidParameterCount();
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public virtual bool Execute(AdvMainEngine engine)
        {
            return true;
        }

        /// <summary>
        /// パッキングできるパラメータが存在する可能性があるかどうかを返します。
        /// </summary>
        /// <returns></returns>
        public bool MaybeExistPackableParamter()
        {
            for (int i = 0; i < Param.Length; ++i)
            {
                if (Param[i][0] == '|')
                {
                    return true;
                }
            }

            return false;
        }

#endregion

#region protected methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">コマンドのデータを分割したパラメータ</param>
        protected AdvCommandBase(string[] param) : base(param)
        {
        }

        /// <summary>
        /// パラメータの数が十分かどうかを確認します。
        /// </summary>
        /// <returns>パラメータが適切である場合、trueを返します。</returns>
        protected virtual bool IsValidParameterCount()
        {
            bool isShort = Param.Length < MinParameterCount;
            bool isExcess = Param.Length > MaxParameterCount;
            if (isShort)
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "パラメータが不足しています。";
            }
            if (isExcess)
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "パラメータが過多です。";
            }
            return Type != CommandType.ErrorCommand;
        }

        /// <summary>
        /// スケールされた時間を取得します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <param name="time">時間</param>
        /// <returns>スキップ状態の場合、スケールされた時間を返します。</returns>
        protected float GetScaledTime(AdvMainEngine engine, float time)
        {
            return engine.StatusManager.Mode.IsSkip ? time * engine.Settings.EngineSetting.SkipTimeScale : time;
        }

#endregion

#region private methods

        /// <summary>
        /// コマンドの種類を確認します。
        /// </summary>
        void CheckType()
        {
            if (Param.Length == 0)
            {
                Type = CommandType.None;
                return;
            }

            // 先頭文字から判断可能なコマンドとしての種類を設定
            switch (Param[0][0])
            {
                case SceneNamePrefix:
                    Type = CommandType.SceneName;
                    break;
                case FileLoadPrefix:
                    Type = CommandType.FileLoad;
                    break;
                case VariablePrefix:
                    Type = CommandType.Variable;
                    break;
                case MacroPrefix:
                    Type = CommandType.Macro;
                    break;
                case ConstantValuePrefix:
                    Type = CommandType.ConstantValue;
                    break;
                case VariableParamPrefix:
                    Type = CommandType.VariableParam;
                    break;
                case CommentPrefix:
                    // 先頭が"//"の場合はコメント
                    Type = Param[0][1] == CommentPrefix ? CommandType.Comment : CommandType.ErrorCommand;
#if UNITY_EDITOR || NOVELKIT_EDITOR
                    if (Type == CommandType.Comment && AdvAnnotationProcessor.IsAnnotation(this))
                    {
                        Type = CommandType.Annotation;
                    }
#endif
                    break;
                default:
                    Type = CommandType.Other;
                    break;
            }

            // 一部コマンド以外は第1パラメータが1文字というケースは存在しない
            if (Type != CommandType.Other && Param[0].Length <= 1)
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "第1パラメータに1文字以下の設定はできません。";
                return;
            }
        }

#endregion
    }
}
