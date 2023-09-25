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
using System.Linq;

namespace Jigbox.TextView
{
    /// <summary>
    /// 本文一文字の要素(画像)を表すクラス
    /// </summary>
    public class MainRenderingImageElement : MainRenderingElement
    {
        protected static readonly IEnumerable<SubRenderingFontElement> SubsEmptyList = Enumerable.Empty<SubRenderingFontElement>();

        public override IGlyph IGlyph { get { return this.Glyph; }}
        public InlineImageGlyph Glyph { get; protected set; }

        private readonly float width;

        public override float Width(MainRenderingElementPosition position)
        {
            var result = this.MarginLeft + this.RubySpacingLeft + this.Glyph.Width + this.RubySpacingRight;

            if (position.IsLineTail() == false || this.Option.TrimLineTailSpacing == false)
            {
                result += this.CharacterSpacingRight;
            }

            if (position.IsLineTail() == false)
            {
                result += this.MarginRight;
            }

            return result;
        }

        public float Height { get; protected set; }
        public string Source { get; protected set; }
        public string Name { get; protected set; }
        // Note: CharacterSpacingLeft は常に0なのでプロパティ化しない
        public float CharacterSpacingRight { get; protected set; }
        public float RubySpacingLeft { get; protected set; }
        public float RubySpacingRight { get; protected set; }
        public int MarginLeft { get; protected set; }
        public int MarginRight { get; protected set; }
        public int OffsetY { get; protected set; }

        protected List<SubRenderingFontElement> subs = null;
        public IEnumerable<SubRenderingFontElement> Subs
        {
            get { return subs != null ? subs : SubsEmptyList; }
        }

        public readonly MainRenderingElementOption Option;

        public MainRenderingImageElement(
            float width,
            float height,
            string source,
            string name,
            float characterSpacingRight,
            float rubySpacingLeft,
            float rubySpacingRight,
            int marginLeft,
            int marginRight,
            int offsetY,
            IEnumerable<SubRenderingFontElement> subs,
            int glyphIndex,
            MainRenderingElementOption option
        )
        {
            this.width = width;
            this.Height = height;
            this.Source = source;
            this.Name = name;
            this.CharacterSpacingRight = characterSpacingRight;
            this.RubySpacingLeft = rubySpacingLeft;
            this.RubySpacingRight = rubySpacingRight;
            this.MarginLeft = marginLeft;
            this.MarginRight = marginRight;
            this.OffsetY = offsetY;
            this.GlyphIndex = glyphIndex;

            this.Glyph = new InlineImageGlyph(this.width, this.Height, this.Source, this.Name, this.OffsetY);
            if (subs != null)
            {
                this.subs = new List<SubRenderingFontElement>(subs);
            }

            this.Option = option;
        }
    }
}
