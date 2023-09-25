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
    public sealed class LeftBracketTokenizer : ArabicTextTokenizer
    {
#region properties

        /// <summary>トークンの種類</summary>
        protected override TokenType Type { get { return TokenType.LeftBracket; } }

#endregion

#region protected methods

        /// <summary>
        /// トークンとして有効な文字かどうかを返します。
        /// </summary>
        /// <param name="character"></param>
        /// <returns>トークンとして有効な文字の場合、<c>true</c>を返します。</returns>
        protected override bool IsValid(char character)
        {
            // 括弧のトークンは1文字ずつにする
            return BracketTable.IsLeftBracket(character) && length < 1;
        }

#endregion
    }
}
