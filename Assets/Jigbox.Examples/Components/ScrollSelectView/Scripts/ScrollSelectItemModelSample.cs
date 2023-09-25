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

namespace Jigbox.Examples
{
    public class ScrollSelectItemModelSample
    {
        public int Index { get; private set; }

        public Color Color { get; private set; }

        public ScrollSelectItemModelSample(int index, Color color)
        {
            Index = index;
            Color = color;
        }
        
        public string GetHexRGBA()
        {
            var buf = "#";
            foreach (var item in new[] { Color.r, Color.g, Color.b, Color.a })
            {
                var hex = (int) Math.Round(item * 0xFF);
                buf += hex.ToString("X2");
            }
            return buf;
        }
    }
}
