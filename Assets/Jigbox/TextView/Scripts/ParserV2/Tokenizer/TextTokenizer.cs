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

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Jigbox.TextView.ParserV2
{
    public abstract class TextTokenizer
    {
#region inner classes, enum, and structs

        public sealed class Enumerator : IEnumerator<Token>, IEnumerable
        {
            /// <summary>トークン</summary>
            List<Token> tokens;

            /// <summary>現在参照している要素のインデックス</summary>
            int currentIndex;

            /// <summary>現在参照している要素</summary>
            Token current;

            /// <summary>現在参照している要素</summary>
            public Token Current { get { return current; } }

            /// <summary>現在参照している要素</summary>
            object IEnumerator.Current { get { return current; } }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="tokens">トークン</param>
            public Enumerator(List<Token> tokens)
            {
                this.tokens = tokens;
                currentIndex = -1;
                current = null;
            }

            /// <summary>
            /// 参照先を次の要素に進めます。
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                ++currentIndex;
                if (currentIndex >= tokens.Count)
                {
                    return false;
                }

                current = tokens[currentIndex];
                return true;
            }

            /// <summary>
            /// 参照を初期位置に戻します。
            /// </summary>
            public void Reset()
            {
                currentIndex = -1;
            }

            /// <summary>
            /// アンマネージドリソースを解放します。
            /// </summary>
            void System.IDisposable.Dispose()
            {
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }
        }

#endregion

#region constant

        /// <summary>ハイフン</summary>
        public const char CharHyphen = '-';

        /// <summary>アンダースコア</summary>
        public const char CharUnderScore = '_';

        /// <summary>山括弧(左)</summary>
        public const char CharOpenTag = '<';

        /// <summary>山括弧(右)</summary>
        public const char CharCloseTag = '>';

        /// <summary>スラッシュ</summary>
        public const char CharSlash = '/';

        /// <summary>等号</summary>
        public const char CharEqual = '=';

        /// <summary>シングルクォーテーションマーク</summary>
        public const char CharSingleQuote = '\'';

        /// <summary>ダブルクォーテーションマーク</summary>
        public const char CharDoubleQuote = '"';

        /// <summary>アンパサンド</summary>
        public const char CharAmpersand = '&';

        /// <summary>セミコロン</summary>
        public const char CharSemicolon = ';';

        /// <summary>バックスラッシュ</summary>
        public const char CharBackSlash = '\\';

#endregion

#region properties

        /// <summary>トークナイズ中の処理情報</summary>
        protected static TokenizeInfo tokenizeInfo = new TokenizeInfo();

        /// <summary>トークナイズが終了したかどうか</summary>
        public static bool IsEnd { get { return tokenizeInfo.IsEnd; } }

        /// <summary>パースに失敗したかどうか</summary>
        public static bool IsFailed { get { return tokenizeInfo.IsFailed; } }

        /// <summary>エラー時のメッセージ</summary>
        public static string Message { get { return tokenizeInfo.Message; } }

        /// <summary>トークン</summary>
        protected static List<Token> tokens = new List<Token>();

        /// <summary>Enumerator</summary>
        protected static Enumerator enumerator;

        /// <summary>トークナイズの状態</summary>
        public static TokenizeMode Mode { get; protected set; }

        /// <summary>文字列の抽出開始位置のインデックス</summary>
        protected static int fetchStartIndex = 0;

        /// <summary>文字列の抽出開始位置のインデックス</summary>
        public static int FetchStartIndex { get { return fetchStartIndex; } }

        /// <summary>抽出する文字列の長さ</summary>
        protected static int fetchLength = 0;

        /// <summary>StringBuilder</summary>
        protected static StringBuilder stringBuilder = new StringBuilder();

        /// <summary>要素計算用のタグ用トークンのスタック</summary>
        static Stack<BeginTagToken> cachedTagStack = new Stack<BeginTagToken>();

#endregion

#region public methods

        /// <summary>
        /// トークナイザの共通部分に残っているデータをクリアして、再処理可能な状態にします。
        /// </summary>
        /// <param name="text">トークナイズする文字列</param>
        public static void Refresh(string text)
        {
            tokenizeInfo.Init(text);
            tokens.Clear();
            Mode = TokenizeMode.Text;
            stringBuilder.Length = 0;
            if (enumerator == null)
            {
                enumerator = new Enumerator(tokens);
            }
        }

        /// <summary>
        /// 元となる文字列から情報を解析して、要素ごとにトークン化します。
        /// </summary>
        public void Tokenize()
        {
            Init();

            while (!IsEnd && !Read())
            {
            }

            if (IsEnd && !IsFailed)
            {
                if (!CheckErrorFinalTokenizer()
                    && this is PlainTextTokenizer)
                {
                    PlainTextTokenizer tokenizer = this as PlainTextTokenizer;
                    tokenizer.CreateToken();
                }
                CountUpRelativeNodes();
            }
        }

        /// <summary>
        /// トークンを取得します。
        /// </summary>
        /// <returns></returns>
        public static Enumerator GetTokens()
        {
            enumerator.Reset();
            return enumerator;
        }

        /// <summary>
        /// ノード化する際に親ノードが存在しないトップレベルのノードの数を取得します。
        /// </summary>
        /// <returns></returns>
        public static int GetTopNodesCount()
        {
            int count = 0;
            int opendTagCount = 0;

            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Text:
                        if (opendTagCount == 0)
                        {
                            ++count;
                        }
                        break;
                    case TokenType.Tag:
                        if (opendTagCount == 0)
                        {
                            ++count;
                        }
                        ++opendTagCount;
                        break;
                    case TokenType.EndTag:
                        --opendTagCount;
                        if (opendTagCount < 0)
                        {
                            // 不正な構文で入力されている
                            return count;
                        }
                        break;
                    default:
                        break;
                }
            }

            return count;
        }

#endregion

#region protected methods

        /// <summary>
        /// ノード化する際に付加される属性や子要素の数をカウントしてトークンに設定します。
        /// </summary>
        protected static void CountUpRelativeNodes()
        {
            cachedTagStack.Clear();

            for (int i = 0; i < tokens.Count; ++i)
            {
                switch (tokens[i].Type)
                {
                    case TokenType.Text:
                        if (cachedTagStack.Count > 0)
                        {
                            ++cachedTagStack.Peek().ChildrenCount;
                        }
                        break;
                    case TokenType.Tag:
                        if (cachedTagStack.Count > 0)
                        {
                            ++cachedTagStack.Peek().ChildrenCount;
                        }
                        cachedTagStack.Push(tokens[i] as BeginTagToken);
                        break;
                    case TokenType.EndTag:
                        if (cachedTagStack.Count > 0)
                        {
                            cachedTagStack.Pop();
                        }
                        else
                        {
                            // 不正な構文で入力されている
                            return;
                        }
                        break;
                    case TokenType.Attribute:
                        ++cachedTagStack.Peek().AttributeCount;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 初期化します。
        /// </summary>
        protected virtual void Init()
        {
            fetchStartIndex = tokenizeInfo.Index;
            fetchLength = 0;
        }

        /// <summary>
        /// 文字列からトークン化する文字列までを読み出します。
        /// </summary>
        /// <returns>読み出しが完了して、トークン化したら<c>true</c>を返します。</returns>
        protected abstract bool Read();

        /// <summary>
        /// 渡された文字が通常の文字、数字、'-'、'='のいずれかであるかを返します。
        /// </summary>
        /// <param name="character">文字</param>
        /// <returns></returns>
        protected bool IsLetterOrDigitOrTrail(char character)
        {
            return char.IsLetterOrDigit(character)
                || character == CharHyphen
                || character == CharUnderScore;
        }

        /// <summary>
        /// 最後に利用しているトークナイザを確認してエラーがあるかを返します。
        /// </summary>
        /// <returns>エラーがあれば、<c>true</c>を返します。</returns>
        protected bool CheckErrorFinalTokenizer()
        {
            switch (Mode)
            {
                case TokenizeMode.Tag:
                    tokenizeInfo.SetError(
                        "> が入力されていないため、タグが正しく閉じられませんでした。",
                        FetchStartIndex,
                        CharOpenTag);
                    return true;
                case TokenizeMode.CharacterReference:
                    tokenizeInfo.SetError(
                        "; が入力されていないため、文字参照が正しく行われませんでした。",
                        FetchStartIndex,
                        CharAmpersand);
                    return true;
                case TokenizeMode.EndTag:
                    tokenizeInfo.SetError(
                        "> が入力されていないため、閉じタグが正しく閉じられませんでした。",
                        FetchStartIndex,
                        CharOpenTag);
                    return true;
                case TokenizeMode.Attribute:
                    tokenizeInfo.SetError(
                        "属性名以降の入力がないため、タグが正しく閉じられませんでした。",
                        FetchStartIndex);
                    return true;
                case TokenizeMode.AttributeValue:
                    tokenizeInfo.SetError(
                        "属性の値の指定が閉じられていないため、値を正しく設定できませんでした。",
                        FetchStartIndex);
                    return true;
                default:
                    return false;
            }
        }

#endregion
    }
}
