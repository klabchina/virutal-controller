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
    public class TileModelVertical : TileModelBase
    {
#region Static Constants

        public static readonly Vector2 DefaultViewPivot = new Vector2(0.5f, 1.0f);

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
                var column = Math.Max(VisibleColumnCount(false), 1);
                var cell = Math.Max(VirtualCellCount, 1);
                var row = (int) Math.Ceiling(cell / (float) column);
                return ComputePreferredSize(row, column);
            }
        }

        public override int VisibleCellCount(bool extendable)
        {
            int row = VisibleRowCount(extendable);
            int column = VisibleColumnCount(false);
            if (extendable)
            {
                // 継ぎ目なく見えるように一行分追加する。
                return (row + 1) * column;
            }
            return row * column;
        }

        public override int ColumnIndex(int cellIndex)
        {
            var columnCount = VisibleColumnCount(false);
            return columnCount > 0 ? cellIndex % columnCount : 0;
        }

        public override int RowIndex(int cellIndex)
        {
            var columnCount = VisibleColumnCount(false);
            return columnCount > 0 ? cellIndex / columnCount : 0;
        }

        protected virtual void ValidateColumnIndex(int columnIndex)
        {
            var columnCount = VisibleColumnCount(false);
            if (columnCount < 0 || columnIndex >= columnCount)
            {
                var message = string.Format("Limited Column Count: {0}, but requested column index: {1}", columnCount, columnIndex);
                throw new ArgumentOutOfRangeException("columnIndex", message);
            }
        }

        public override int CellIndex(int rowIndex, int columnIndex)
        {
            ValidateColumnIndex(columnIndex);
            var columnCount = VisibleColumnCount(false);
            return columnCount * rowIndex + columnIndex;
        }

        public override Vector2 CellPosition(int rowIndex, int columnIndex)
        {
            ValidateColumnIndex(columnIndex);
            return base.CellPosition(rowIndex, columnIndex);
        }

#endregion

#region Control

        public override void SyncScrollPosition(Vector2 position, Action<IndexedRectTransform, int> callback)
        {
            var visibleFirstRowIndex = RowIndex(position);
            var visibleFirstCellIndex = CellIndex(visibleFirstRowIndex, 0);
            SyncCellIndexes(visibleFirstCellIndex, callback);
        }

#endregion
    }
}
