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

namespace Jigbox.TextView
{
    /// <summary>
    /// MainRenderingFontElementの約物半角用拡張
    /// </summary>
    public static class PunctuationExtensions
    {
        /// <summary>
        /// 対象の約物一覧
        /// </summary>
        static readonly HashSet<char> LeftPunctuations = new HashSet<char>()
        {
            '「',
            '『',
            '（',
            '｟',
            '〔',
            '〘',
            '［',
            '【',
            '〖',
            '〈',
            '《',
            '｛',
            '〝',
        };

        /// <summary>
        /// 約物半角対象のMainRenderingFontElementか
        /// </summary>
        public static bool IsHalfElement(this MainRenderingFontElement fontElement, MainRenderingElementPosition position)
        {
            // 行頭約物半角
            return fontElement.Option.BeginningOfLineHalf && LeftPunctuations.Contains(fontElement.Glyph.Character) && position.IsLineHead();
        }
    }
}
