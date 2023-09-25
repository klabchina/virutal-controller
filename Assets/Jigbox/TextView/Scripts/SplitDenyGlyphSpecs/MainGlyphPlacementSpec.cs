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

namespace Jigbox.TextView
{
    public class MainGlyphPlacementSpec
    {
        public IGlyphSpec GlyphSpec { get; private set; }
        public int MarginLeft { get; private set; }
        public int MarginRight { get; private set; }
        public int OffsetY { get; private set; }
        public float BaseSpacing { get; private set; }
        public int Index { get; private set; }

        public MainGlyphPlacementSpec(
            IGlyphSpec glyphSpec,
            int marginLeft,
            int marginRight,
            int offsetY,
            int index,
            float baseSpacing
        )
        {
            this.GlyphSpec = glyphSpec;
            this.MarginLeft = marginLeft;
            this.MarginRight = marginRight;
            this.OffsetY = offsetY;
            this.Index = index;
            this.BaseSpacing = baseSpacing;
        }
    }
}
