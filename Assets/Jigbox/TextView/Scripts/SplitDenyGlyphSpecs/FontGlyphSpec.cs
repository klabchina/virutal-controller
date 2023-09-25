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

using System;
using UnityEngine;

namespace Jigbox.TextView
{
    /// <summary>
    /// グリフ(文字)の各種情報を保持するクラス
    /// </summary>
    public class FontGlyphSpec : IGlyphSpec, IEquatable<FontGlyphSpec>
    {
        public readonly char Character;
        public readonly Font Font;
        public readonly int Size;
        public readonly FontStyle Style;
        public readonly Color? Color;
        public readonly float GlyphScaleX;

        int hashCode;

        public FontGlyphSpec(char character, Font font, int size, FontStyle style, Color? color, float glyphScaleX)
        {
            this.Character = character;
            this.Font = font;
            this.Size = size;
            this.Style = style;
            this.Color = color;
            this.GlyphScaleX = glyphScaleX;

            hashCode = Character.GetHashCode();
            // 本来はFontも含めて比較するのが正しいが、
            // そもそもDictionary等に格納する際に、Fontが違うものが
            // 同一のコレクションに格納されるケースが現状存在し得ないため、
            // 微量とは言え、コスト削減のために計算からは省く
            //hashCode = (hashCode * 397) ^ (Font != null ? Font.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Size;
            hashCode = (hashCode * 397) ^ (int) Style;
            // Colorがnullの場合は、Color.clear相当のハッシュで扱う
            hashCode = (hashCode * 397) ^ (Color != null ? Color.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ GlyphScaleX.GetHashCode();
        }

        public bool Equals(FontGlyphSpec other)
        {
            if (other == null)
            {
                return false;
            }

            return hashCode == other.hashCode && Font == other.Font;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is FontGlyphSpec))
            {
                return false;
            }

            FontGlyphSpec other = obj as FontGlyphSpec;

            return hashCode == other.hashCode && Font == other.Font;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
