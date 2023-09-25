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

namespace Jigbox.TextView
{
    public class LineRenderingElements
    {
#region constants

        // 最上位要素なので、常に行頭でもあり行末でもある
        private const MainRenderingElementPosition position = MainRenderingElementPosition.LineHeadTail;

#endregion

#region fields and propeties

        private readonly List<ISplitDenyZone> zones;

        public IEnumerable<ISplitDenyZone> Zones
        {
            get { return this.zones; }
        }

        public readonly int ZonesCount;

        public float Width
        {
            get
            {
                var width = 0.0f;

                for (var i = 0; i < ZonesCount; i++)
                {
                    width += zones[i].Width(position.PositionAtIndex(i, ZonesCount));
                }

                return width;
            }
        }

#endregion

#region constructors

        public LineRenderingElements(IEnumerable<ISplitDenyZone> zones)
        {
            this.zones = new List<ISplitDenyZone>(zones);
            this.ZonesCount = this.zones.Count;
        }

        public ISplitDenyZone ElementAt(int index)
        {
            if (index < 0 || index >= ZonesCount)
            {
                throw new IndexOutOfRangeException();
            }

            return zones[index];
        }

#endregion
    }
}
