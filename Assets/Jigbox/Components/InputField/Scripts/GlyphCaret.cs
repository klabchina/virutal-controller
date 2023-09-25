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
    /// <summary>
    /// グリフ単位のキャレット情報管理クラス
    /// </summary>
    public class GlyphCaret
    {
        /// <summary>GlyphPlacementの所属しているTextLine</summary>
        public TextLine TextLine { get; set; }

        /// <summary>行頭の場合はnull、それ以外はTextLineのGlyphPlacementと同じもの</summary>
        public GlyphPlacement GlyphPlacement { get; set; }

        /// <summary>TextLine.LineX</summary>
        public float LineX { get; set; }

        /// <summary>TextLine.LineYにTextViewのVerticalAlignのOffsetYを加えたY</summary>
        public float LineY { get; set; }

        /// <summary>折返し行インデックス 0開始</summary>
        public int WrapIndex { get; set; }

        /// <summary>自動折返しマーカー</summary>
        public bool WrapMarker { get; set; }

        /// <summary>キャレットのX座標</summary>
        public float X
        {
            get
            {
                var result = LineX;
                if (GlyphPlacement != null)
                {
                    result += GlyphPlacement.X + GlyphPlacement.JustifyShiftOffsetX;
                }

                return result;
            }
        }

        /// <summary>幅</summary>
        public float Width
        {
            get
            {
                if (GlyphPlacement != null)
                {
                    return GlyphPlacement.Width;
                }

                return 0f;
            }
        }

        /// <summary>
        /// デバッグ用文字列
        /// </summary>
        public override string ToString()
        {
            return string.Format("[GlyphCaret] glyph={0}, lineX={1}, lineY={2}, wrap={3}, wrapIndex={4}", GlyphPlacement, LineX, LineY, WrapMarker, WrapIndex);
        }
    }
}
