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
using System;

namespace Jigbox.TextView
{
    public class TextModifierScope : IRubyTextModifierScope
    {
        public TextModifierScope Parent { get; protected set; }

        public TextModifier TextModifier { get; protected set; }

        public float? Spacing
        {
            get { return this.TextModifier.Spacing ?? (this.Parent != null ? this.Parent.Spacing : null); }
            set { this.TextModifier.Spacing = value; }
        }

        public float? RubyOffset
        {
            get { return this.TextModifier.RubyOffset ?? (this.Parent != null ? this.Parent.RubyOffset : null); }
            set { this.TextModifier.RubyOffset = value; }
        }

        public Color? RubyColor
        {
            get { return this.TextModifier.RubyColor ?? ((this.Parent != null) ? this.Parent.RubyColor : null); }
            set { this.TextModifier.RubyColor = value; }
        }

        public FontStyle? RubyFontStyle
        {
            get { return this.TextModifier.RubyFontStyle ?? ((this.Parent != null) ? this.Parent.RubyFontStyle : null); }
            set { this.TextModifier.RubyFontStyle = value; }
        }

        public float? RubyFontScale
        {
            get { return this.TextModifier.RubyFontScale ?? ((this.Parent != null) ? this.Parent.RubyFontScale : null); }
            set { this.TextModifier.RubyFontScale = value; }
        }

        public int? FontSize
        {
            get { return this.TextModifier.FontSize ?? ((this.Parent != null) ? this.Parent.FontSize : null); }
            set { this.TextModifier.FontSize = value; }
        }

        public FontStyle? FontStyle
        {
            get
            {
                FontStyle? fontStyle = null;

                if (this.TextModifier.FontStyle != null)
                {
                    fontStyle = this.TextModifier.FontStyle;
                }
                
                // 親が存在する場合は親の状態を反映する
                if (this.Parent != null)
                {
                    if (fontStyle != null)
                    {
                        fontStyle |= this.Parent.FontStyle;
                    }
                    else
                    {
                        fontStyle = this.Parent.FontStyle;
                    }
                }

                return fontStyle;
            }
            set { this.TextModifier.FontStyle = value; }
        }

        public Color? Color
        {
            get { return this.TextModifier.Color ?? ((this.Parent != null) ? this.Parent.Color : null); }
            set { this.TextModifier.Color = value; }
        }

        public Color? ModifiedColor
        {
            get
            {
                // 親がnullの場合、一番大枠でのスコープ(TextViewの状態相当)ということなので、
                // その色は無視してタグによって編集された色を返す
                return this.Parent == null ? null : this.TextModifier.Color ?? this.Parent.ModifiedColor;
            }
        }
        
        public LetterCase? LetterCase
        {
            get { return this.TextModifier.LetterCase ?? (this.Parent != null ? this.Parent.LetterCase : null);}
            set { this.TextModifier.LetterCase = value; }
        }

        public TextModifierScope(TextModifierScope parent, TextModifier modifier)
        {
            this.Parent = parent;
            this.TextModifier = new TextModifier();

            if (modifier != null)
            {
                this.Apply(modifier);
            }
        }

        /// <summary>
        /// 指定のTextModifierを適応する.
        /// </summary>
        /// <param name="modifier">TextModifier.</param>
        public void Apply(TextModifier modifier)
        {
            this.Spacing = modifier.Spacing;
            this.RubyOffset = modifier.RubyOffset;
            this.RubyColor = modifier.RubyColor;
            this.RubyFontStyle = modifier.RubyFontStyle;
            this.RubyFontScale = modifier.RubyFontScale;
            this.FontSize = modifier.FontSize;
            this.FontStyle = modifier.FontStyle;
            this.Color = modifier.Color;
            this.LetterCase = modifier.LetterCase;
        }

        public TextModifierScope RubyScope
        {
            get
            {
                return new TextModifierScope(this, new TextModifier() {
                    Color = this.RubyColor,
                    FontSize = (int) ((this.FontSize ?? 1) * (this.RubyFontScale ?? 1)),
                    FontStyle = this.RubyFontStyle
                });
            }
        }

        /// <summary>
        /// 横組で組まれた場合のルビ文字の基準になるオフセット座標を計算します
        /// </summary>
        /// <returns>The ruby offset.</returns>
        /// <param name="baseHeight">親文字列の中で最も高い文字の高さ</param>
        /// <remarks>将来的に、このメソッドが返す値が<c>float</c>型に変更される可能性があります</remarks>
        public int VerticalRubyOffset(int baseHeight)
        {
            if (this.RubyOffset == null || this.RubyFontScale == null)
            {
                return baseHeight;
            }

            float offsetRate = this.RubyOffset ?? 0.0f;
            float rubyScale = this.RubyFontScale ?? 0.0f;

            return (int) Math.Floor(baseHeight * (1.0f + offsetRate * rubyScale));
        }
    }
}
