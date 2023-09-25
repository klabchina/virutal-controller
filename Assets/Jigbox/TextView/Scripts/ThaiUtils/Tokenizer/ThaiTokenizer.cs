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

namespace ThaiUtils.Tokenizer
{
    /// <summary>
    /// レイアウトが必要かどうかを判別するトークナイザ
    /// </summary>
    public static class ThaiTokenizer
    {
#region inner classes, enum, and structs

        struct TokenizeInfo
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
                length = text.Length;
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
                    Character = (char) 0;
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

        /// <summary>トークナイズ状態の情報</summary>
        static TokenizeInfo tokenizeInfo = new TokenizeInfo();

        /// <summary>トークン</summary>
        static List<Token> tokens = new List<Token>();

        /// <summary>トークン</summary>
        public static List<Token> Tokens { get { return tokens; } }

#endregion

#region public methods

        /// <summary>
        /// テキストをトークナイズします。
        /// </summary>
        /// <param name="text">テキスト</param>
        public static void Tokenize(string text)
        {
            tokens.Clear();

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            tokenizeInfo.Init(text);

            int startIndex = 0;
            int length = 1;
            TokenType currentType = GetTokenTypeByCurrentCharacter();
            tokenizeInfo.Next();

            while (!tokenizeInfo.IsEnd)
            {
                TokenType type = GetTokenTypeByCurrentCharacter();
                if (currentType != type)
                {
                    tokens.Add(new Token(startIndex, length, currentType));
                    startIndex += length;
                    length = 0;
                    currentType = type;
                }

                tokenizeInfo.Next();
                ++length;
            }

            tokens.Add(new Token(startIndex, length, currentType));
        }

#endregion

#region private methods

        /// <summary>
        /// 現在参照されている文字からトークンの種類を判別して取得します。
        /// </summary>
        /// <returns>トークンの種類を返します。</returns>
        static TokenType GetTokenTypeByCurrentCharacter()
        {
            return ThaiTable.IsLayoutNeeded(tokenizeInfo.Character) ? TokenType.NeedLayout : TokenType.Plain;
        }

#endregion
    }
}
