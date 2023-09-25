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

namespace Jigbox.Components
{
    /// <summary>
    /// グリフデータを行で管理するクラス
    /// </summary>
    public class LineGlyphCaret
    {
        /// <summary>1行に含まれているグリフ情報</summary>
        public GlyphCaret[] GlyphCaretData { get; set; }

        /// <summary>折返し込の行数</summary>
        public int WrapLineCount { get; set; }

        /// <summary>Debug用文字列</summary>
        public override string ToString()
        {
            return string.Format("[LineCaretData] glyphCaretData.length={0},  lineCount={1}", GlyphCaretData.Length, WrapLineCount);
        }
    }
}
