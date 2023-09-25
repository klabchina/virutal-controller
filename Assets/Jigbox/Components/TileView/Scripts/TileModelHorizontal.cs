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
    public class TileModelHorizontal : TileModelBase
    {
#region Static Constants

        public static readonly Vector2 DefaultViewPivot = new Vector2(0.0f, 0.5f);

#endregion

#region Information

        [SerializeField]
        Vector2 viewPivot = DefaultViewPivot;

        public override Vector2 ViewPivot
        {
            get { return viewPivot; }
            set { viewPivot = value; }
        }

        public override Vector2 ContentPreferredSize
        {
            get
            {
                var row = Math.Max(VisibleRowCount(false), 1);
                var cell = Math.Max(VirtualCellCount, 1);
                var column = (int) Math.Ceiling(cell / (float) row);
                return ComputePreferredSize(row, column);
            }
        }

        public override int VisibleCellCount(bool extendable)
        {
            var row = VisibleRowCount(false);
            var column = VisibleColumnCount(extendable);
            if (extendable)
            {
                // 継ぎ目なく見えるように一列分追加する
                return row * (column + 1);
            }
            return row * column;
        }

        public override int ColumnIndex(int cellIndex)
        {
            var rowCount = VisibleRowCount(false);
            return rowCount > 0 ? cellIndex / rowCount : 0;
        }

        public override int RowIndex(int cellIndex)
        {
            var rowCount = VisibleRowCount(false);
            return rowCount > 0 ? cellIndex % rowCount : 0;
        }

        protected virtual void ValidateRowIndex(int rowIndex)
        {
            var rowCount = VisibleRowCount(false);
            if (rowIndex < 0 || rowIndex >= rowCount)
            {
                var message = string.Format("Limited Row Count: {0}, but requested Row Index: {1}", rowCount, rowIndex);
                throw new ArgumentOutOfRangeException("rowIndex", message);
            }
        }

        public override int CellIndex(int rowIndex, int columnIndex)
        {
            ValidateRowIndex(rowIndex);
            return VisibleRowCount(false) * columnIndex + rowIndex;
        }

        public override Vector2 CellPosition(int rowIndex, int columnIndex)
        {
            ValidateRowIndex(rowIndex);
            return base.CellPosition(rowIndex, columnIndex);
        }

#endregion

#region

        public override void SyncScrollPosition(Vector2 position, Action<IndexedRectTransform, int> callback)
        {
            var visibleFirstColumnIndex = ColumnIndex(position);
            var visibleFirstCellIndex = CellIndex(0, visibleFirstColumnIndex);
            SyncCellIndexes(visibleFirstCellIndex, callback);
        }

#endregion
    }
}
