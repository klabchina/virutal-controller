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
    /// <summary>
    /// トークンの種類
    /// </summary>
    public enum TokenType
    {
        /// <summary>アラビア語</summary>
        Arabic,
        /// <summary>アラビア文字の数字</summary>
        ArabicNumber,
        /// <summary>アラビア語以外の文字</summary>
        Letter,
        /// <summary>数字</summary>
        Number,
        /// <summary>空白</summary>
        WhiteSpace,
        /// <summary>制御文字</summary>
        Control,
        /// <summary>左括弧(開き括弧)</summary>
        LeftBracket,
        /// <summary>右括弧(閉じ括弧)</summary>
        RightBracket,
        /// <summary>その他</summary>
        Other
    }
}
