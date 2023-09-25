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

namespace ArabicUtils.Tokenizer
{
    public sealed class Token
    {
        /// <summary>文字列を読み込むインデックス</summary>
        public readonly int startIndex;

        /// <summary>文字列の長さ</summary>
        public readonly int length;

        /// <summary>トークンの種類</summary>
        public readonly TokenType type;

        /// <summary>右から左に処理するかどうか</summary>
        public bool isRightToLeft;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="startIndex">文字列を読み込むインデックス</param>
        /// <param name="length">文字列の長さ</param>
        /// <param name="type">トークンの種類</param>
        public Token(int startIndex, int length, TokenType type)
        {
            this.startIndex = startIndex;
            this.length = length;
            this.type = type;
        }
    }
}
