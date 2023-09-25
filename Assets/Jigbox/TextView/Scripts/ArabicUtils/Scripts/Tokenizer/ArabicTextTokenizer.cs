/**
 * Additional Language Utility Library
 * Copyright(c) 2018 KLab, Inc. All Rights Reserved.
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

namespace ArabicUtils.Tokenizer
{
    public abstract class ArabicTextTokenizer
    {
#region inner classes, enum, and structs

        protected struct TokenizeInfo
        {
            /// <summary>テキスト</summary>
            string text;

            /// <summary>テキストの長さ</summary>
            int length;

            /// <summary>現在参照している文字のインデックス</summary>
            public int Index { get; private set; }

            /// <summary>現在参照している文字</summary>
            public char Character { get; private set; }

            /// <summary>文字列の終端かどうか</summary>
            public bool IsEnd { get; private set; }

            /// <summary>
            /// 初期化します。
            /// </summary>
            /// <param name="text">テキスト</param>
            public void Init(string text)
            {
                this.text = text;
                this.length = text.Length;
                Index = 0;
                Character = text[0];
                IsEnd = false;
            }

            /// <summary>
            /// 参照位置を次の文字に進めます。
            /// </summary>
            public void Next()
            {
                ++Index;
                if (Index >= length)
                {
                    Character = ArabicLetter.InvalidUnicodeCharacter;
                    IsEnd = true;
                }
                else
                {
                    Character = text[Index];
                }
            }
        }

#endregion

#region properties

#region static

        /// <summary>トークナイズ状態の情報</summary>
        static TokenizeInfo tokenizeInfo = new TokenizeInfo();

        /// <summary>トークン</summary>
        static List<Token> tokens = new List<Token>();

        /// <summary>トークン</summary>
        public static List<Token> Tokens { get { return tokens; } }

        /// <summary>トークナイザ</summary>
        protected static ArabicTextTokenizer[] tokenizers = null;

#endregion

        /// <summary>文字列を読み込むインデックス</summary>
        protected int startIndex = 0;

        /// <summary>文字列の長さ</summary>
        protected int length;

        /// <summary>トークンの種類</summary>
        protected abstract TokenType Type { get; }

#endregion

#region public methods

        /// <summary>
        /// テキストをトークナイズします。
        /// </summary>
        /// <param name="text">テキスト</param>
        public static void Tokenize(string text)
        {
            tokenizeInfo.Init(text);
            tokens.Clear();

            while (!tokenizeInfo.IsEnd)
            {
                for (int i = 0; i < tokenizers.Length; ++i)
                {
                    if (tokenizers[i].Read())
                    {
                        break;
                    }
                }
            }

            ArabicTextDirectionModifier.Modify(text, tokens);
        }

#endregion

#region protected methods

        /// <summary>
        /// テキストを読み込みます。
        /// </summary>
        /// <returns>トークンの生成に成功した場合、<c>true</c>を返します。</returns>
        protected bool Read()
        {
            startIndex = tokenizeInfo.Index;
            length = 0;

            while (!tokenizeInfo.IsEnd && IsValid(tokenizeInfo.Character))
            {
                ++length;
                tokenizeInfo.Next();
            }

            if (length > 0)
            {
                tokens.Add(new Token(startIndex, length, Type));
                return true;
            }

            return false;
        }

        /// <summary>
        /// トークンとして有効な文字かどうかを返します。
        /// </summary>
        /// <param name="character"></param>
        /// <returns>トークンとして有効な文字の場合、<c>true</c>を返します。</returns>
        protected abstract bool IsValid(char character);

#endregion

#region private methods

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static ArabicTextTokenizer()
        {
            tokenizers = new ArabicTextTokenizer[]
            {
                new ArabicTokenizer(),
                new ArabicNumberTokenizer(),
                new LetterTokenizer(),
                new NumberTokenizer(),
                new WhiteSpaceTokenizer(),
                new ControlCharacterTokenizer(),
                new LeftBracketTokenizer(),
                new RightBracketTokenizer(),
                new OtherTextTokenizer(),
            };
        }

#endregion
    }
}
