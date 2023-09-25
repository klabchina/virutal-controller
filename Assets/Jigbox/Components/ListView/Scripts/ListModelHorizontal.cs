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

namespace Jigbox.Components
{
    [Serializable]
    public class ListModelHorizontal : ListModelBase
    {
        #region Static Constants

        public static readonly Vector2 DefaultViewPivot = new Vector2(0.0f, 0.5f);

        #endregion

        #region Fields & Properties

        public override float ContentPreferredSize
        {
            get
            {
                var count = Math.Max(1, VirtualCellCount);
                return Padding.Left + Padding.Right + (CellSize + Spacing) * count - Spacing;
            }
        }

        [SerializeField]
        Vector2 viewPivot = DefaultViewPivot;

        public override Vector2 ViewPivot
        {
            get { return viewPivot; }
            set { viewPivot = value; }
        }

        #endregion

        #region Information API

        public override int VisibleCellCount(bool extendable)
        {
            var count = VisibleUnitCount(ViewportSize.x, Padding.Left, Padding.Right, CellSize, Spacing, extendable);
            if (extendable)
            {
                return count + 1;
            }
            return count;
        }

        public override int CellIndex(float span)
        {
            if (span < Padding.Left + CellSize + Spacing)
            {
                return 0;
            }
            return (int) Math.Floor((span - Padding.Left) / (CellSize + Spacing));
        }

        public override int CellIndex(Vector2 position)
        {
            return CellIndex(position.x);
        }

        public override float CellPosition(int index)
        {
            if (index < 0)
            {
                var message = string.Format("[ListModelHorizontal] Illegal index:{0}", index);
                throw new ArgumentOutOfRangeException(message);
            }
            return Padding.Left + (CellSize + Spacing) * index + CellSize * CellPivot.x;
        }

        #endregion
    }
}
