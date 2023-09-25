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

namespace Jigbox.TextView.HorizontalLayout
{
    public class HorizontalLayoutedImage : IHorizontalLayoutedElement
    {
#region IHorizontalLayoutedElement

        public GlyphPlacement GlyphPlacement { get; private set; }

        public float OffsetX { get { return this.TextLineOffsetX + this.JustifyShiftOffsetX; } }

        public float OffsetY { get { return this.TextLineOffsetY; } }

        public readonly int ImageOffsetY;

        /// <summary>
        /// TextLine単位で見た時のこの文字のX位置
        /// </summary>
        public float TextLineOffsetX { get; set; }

        /// <summary>
        /// TextLine単位で見た時のこの文字のY位置
        /// </summary>
        public float TextLineOffsetY { get; set; }

        /// <summary>
        /// アラインメント設定でTextAlign.JustifyもしくはTextAlign.JustifyAllを設定している時に発生するオフセット位置
        /// </summary>
        public float JustifyShiftOffsetX { get; set; }

        public float xMin { get { return GlyphPlacement.Glyph.xMin + this.OffsetX; } }
        public float xMax { get { return GlyphPlacement.Glyph.xMax + this.OffsetX; } }
        public float yMin { get { return GlyphPlacement.Glyph.yMax + this.OffsetY - this.ImageOffsetY; } }
        public float yMax { get { return GlyphPlacement.Glyph.yMin + this.OffsetY - this.ImageOffsetY; } }

        public float yMinOffsetSpecialAdjusted { get { return GlyphPlacement.Glyph.yMax + this.OffsetYSpecialAdjusted; } }
        public float yMaxOffsetSpecialAdjusted { get { return GlyphPlacement.Glyph.yMin + this.OffsetYSpecialAdjusted; } }

        protected float OffsetYSpecialAdjusted
        {
            get
            {
                var result = this.OffsetY;

                if (this.ImageOffsetY <= 0)
                {
                    result -= this.ImageOffsetY;
                }

                return result;
            }
        }

#endregion
        
        public HorizontalLayoutedImage(GlyphPlacement glyphPlacement)
        {
            this.GlyphPlacement = glyphPlacement;
            var imageGlyph = this.GlyphPlacement.Glyph as InlineImageGlyph;
            this.ImageOffsetY = (imageGlyph == null) ? 0 : imageGlyph.OffsetY;
        }
    }
}
