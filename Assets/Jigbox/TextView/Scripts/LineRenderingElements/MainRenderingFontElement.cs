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
    /// 本文一文字の要素(文字)を表すクラス
    /// </summary>
    public class MainRenderingFontElement : MainRenderingElement
    {
        protected static readonly IEnumerable<SubRenderingFontElement> SubsEmptyList = Enumerable.Empty<SubRenderingFontElement>();

        public override IGlyph IGlyph { get { return this.Glyph; }}
        public Glyph Glyph { get; protected set; }

        private readonly List<SubRenderingFontElement> subs;
        public IEnumerable<SubRenderingFontElement> Subs(MainRenderingElementPosition position)
        {
            return this.subs != null ? subs : SubsEmptyList;
        }

        public int SubsCount
        {
            get { return subs != null ? subs.Count : 0; }
        }

        public override float Width(MainRenderingElementPosition position)
        {
            var glyphWidth = Glyph.Width;

            if (!(RubySpacingLeft > 0f || RubySpacingRight > 0f))
            {
                var isHalfWidth = this.IsHalfElement(position);
                if (isHalfWidth) 
                {
                    glyphWidth *= 0.5f;
                }
            }

            var result = this.RubySpacingLeft + glyphWidth + this.RubySpacingRight;

            if (position.IsLineTail() == false || this.Option.TrimLineTailSpacing == false)
            {
                result += this.CharacterSpacingRight;
            }

            return result;
        }

        // Note: CharacterSpacingLeft は常に0なのでプロパティ化しない
        public float CharacterSpacingRight { get; protected set; }
        public float RubySpacingLeft { get; protected set; }
        public float RubySpacingRight { get; protected set; }
        
        /// <summary>タイ語用拡張</summary>
        public int OffsetX { get; set; }
        /// <summary>タイ語用拡張</summary>
        public int OffsetY { get; set; }

        public readonly MainRenderingElementOption Option;

        public MainRenderingFontElement(
            Glyph glyph,
            float characterSpacingRight,
            float rubySpacingLeft,
            float rubySpacingRight,
            IEnumerable<SubRenderingFontElement> subs,
            int glyphIndex,
            MainRenderingElementOption option
        )
        {
            this.GlyphIndex = glyphIndex;

            this.Glyph = glyph;
            if (subs != null)
            {
                this.subs = new List<SubRenderingFontElement>(subs);
            }

            this.CharacterSpacingRight = characterSpacingRight;
            this.RubySpacingLeft = rubySpacingLeft;
            this.RubySpacingRight = rubySpacingRight;

            this.Option = option;
        }
    }
}
