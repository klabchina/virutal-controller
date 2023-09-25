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
using System.Collections.Generic;
using System.Globalization;

namespace Jigbox.TextView
{
    public abstract class LineBreakRuleCJK : LineBreakRule
    {
        protected HashSet<char> notAllowedAtBeginOfLine;
        protected HashSet<char> notAllowedAtEndOfLine;
        protected HashSet<char> notAllowedWrapPunctuation;

        protected LineBreakRuleCJK()
        {
            this.notAllowedAtBeginOfLine = new HashSet<char>(
                ")]>}" +
                "\u2019" +  // RIGHT SINGLE QUOTATION MARK
                "\u201D"    // RIGHT DOUBLE QUOTATION MARK
            );

            this.notAllowedAtEndOfLine = new HashSet<char>(
                "([<{" +
                "\u2018" +  // LEFT SINGLE QUOTATION MARK
                "\u201C"    // LEFT DOUBLE QUOTATION MARK
            );

            this.notAllowedWrapPunctuation = new HashSet<char>(
                ".,;:!?'\"-/"
            );
        }

        public override bool IsNotAllowAtBeginOfLine(IGlyph g)
        {
            return (g is Glyph) && this.notAllowedAtBeginOfLine.Contains(((Glyph) g).Character);
        }

        public override bool IsNotAllowAtEndOfLine(IGlyph g)
        {
            return (g is Glyph) && this.notAllowedAtEndOfLine.Contains(((Glyph) g).Character);
        }

        public override bool CanWrap(IGlyph firstLineTailGlyph, IGlyph secondLineBeginGlyph)
        {
            if (this.CanWrapInternal(firstLineTailGlyph))
            {
                return true;
            }

            if (this.CanWrapInternal(secondLineBeginGlyph))
            {
                return true;
            }

            return false;
        }

        protected virtual bool CanWrapInternal(IGlyph g)
        {
            if (g is InlineImageGlyph)
            {
                return true;
            }

            var character = ((Glyph) g).Character;

            var cat = Char.GetUnicodeCategory(character);
            if (cat == UnicodeCategory.DecimalDigitNumber)
            {
                return false;
            }
            if (cat == UnicodeCategory.LowercaseLetter)
            {
                return false;
            }
            if (cat == UnicodeCategory.UppercaseLetter)
            {
                return false;
            }
            if (this.notAllowedWrapPunctuation.Contains(character))
            {
                return false;
            }
            if (ArabicUtils.ArabicLetter.IsArabic((g as Glyph).Character))
            {
                return false;
            }

            return true;
        }
    }
}
