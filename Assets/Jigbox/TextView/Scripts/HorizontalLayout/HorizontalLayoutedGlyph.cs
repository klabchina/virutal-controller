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

using UnityEngine;

namespace Jigbox.TextView.HorizontalLayout
{
    public class HorizontalLayoutedGlyph : IHorizontalLayoutedElement
    {
#region IHorizontalLayoutedElement

        public GlyphPlacement GlyphPlacement { get; private set; }

        public float OffsetX { get { return this.TextLineOffsetX + this.JustifyShiftOffsetX; } }

        public float OffsetY { get { return this.TextLineOffsetY; } }

        public float TextLineOffsetX { get; set; }

        public float TextLineOffsetY { get; set; }

        public float JustifyShiftOffsetX { get; set; }

        public float xMin { get { return GlyphPlacement.Glyph.xMin + this.OffsetX; } }
        public float xMax { get { return GlyphPlacement.Glyph.xMax + this.OffsetX; } }
        public float yMin { get { return GlyphPlacement.Glyph.yMax + this.OffsetY; } }
        public float yMax { get { return GlyphPlacement.Glyph.yMin + this.OffsetY; } }

        // オフセット値を持たないため、yMin・yMaxと同じ値を返す
        public float yMinOffsetSpecialAdjusted { get { return this.yMin; } }
        public float yMaxOffsetSpecialAdjusted { get { return this.yMax; } }

#endregion

        public HorizontalLayoutedGlyph(GlyphPlacement glyphPlacement)
        {
            this.GlyphPlacement = glyphPlacement;
        }

        /// <summary>
        /// 頂点情報を取得します。
        /// </summary>
        /// <param name="vertices">頂点情報を格納する領域</param>
        /// <param name="offsetY">y方向のオフセット値</param>
        /// <param name="color">色</param>
        /// <param name="halfType"></param>
        public void GetVertices(ref UIVertex[] vertices, float offsetY, Color color, PunctuationHalfType halfType)
        {
            Glyph glyph = GlyphPlacement.Glyph as Glyph;
            glyph.GetVertices(ref vertices, OffsetX + GlyphPlacement.X, offsetY + OffsetY - GlyphPlacement.Y, color, halfType);
        }

        /// <summary>
        /// アラビア語用に修正された頂点情報を取得します
        /// 文字列全体を鏡表示にしてさらに文字ごとにy軸を反転させた表示になる頂点情報を取得する
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="offsetY"></param>
        /// <param name="color"></param>
        /// <param name="mirrorOffsetX"></param>
        public void GetMirrorVertices(ref UIVertex[] vertices, float offsetY, Color color, float mirrorOffsetX)
        {
            Glyph glyph = GlyphPlacement.Glyph as Glyph;
            glyph.GetMirrorVertices(ref vertices, OffsetX + GlyphPlacement.X, offsetY + OffsetY - GlyphPlacement.Y, color, mirrorOffsetX);
        }
    }
}
