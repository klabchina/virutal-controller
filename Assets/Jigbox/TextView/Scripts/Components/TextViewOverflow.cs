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

using Jigbox.TextView;

namespace Jigbox.Components
{
    public class TextViewOverflow
    {
        /// <summary>
        /// TextViewに表示しきれなかった最初の行を取得します
        /// </summary>
        /// <value>The index of the overflow text line.</value>
        public int OverflowTextLineIndex { get; private set; }

        /// <summary>
        /// TextViewに表示可能な文字のうち、最初のグリフのインデックスを取得します
        /// </summary>
        /// <value>The index of the visible glyph minimum.</value>
        public int VisibleGlyphMinIndex { get; private set; }

        /// <summary>
        /// TextViewに表示可能な文字のうち、最後のグリフのインデックスを取得します
        /// </summary>
        /// <value>The index of the visible glyph max.</value>
        public int VisibleGlyphMaxIndex { get; private set; }

        /// <summary>
        /// TextViewに表示できなかった残りの整形済みテキストを取得します
        /// </summary>
        /// <value>The overflowtext lines.</value>
        public TextLine[] OverflowtextLines { get; private set; }
        
        /// <summary>
        /// Ellipsisを行ったかどうかのフラグを取得します
        /// </summary>
        public bool IsEllipsis { get; private set; }

        public TextViewOverflow(int overflowLineIndex, int visibleGlyphMinIndex, int visibleGlyphMaxIndex, TextLine[] textLines, bool isEllipsis = false)
        {
            this.OverflowTextLineIndex = overflowLineIndex;
            this.VisibleGlyphMinIndex = visibleGlyphMinIndex;
            this.VisibleGlyphMaxIndex = visibleGlyphMaxIndex;
            this.OverflowtextLines = textLines;
            this.IsEllipsis = isEllipsis;
        }
    }
}
