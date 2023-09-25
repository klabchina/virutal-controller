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
using UnityEngine;

namespace Jigbox.Components
{
    [Serializable]
    public abstract class TileModelBase : ITilingLayout
    {
#region Static Constants

        /// <summary>
        /// デフォルトの外周余白の値です
        /// </summary>
        public static readonly Padding DefaultPadding = Padding.zero;

        /// <summary>
        /// デフォルトのセルサイズです
        /// </summary>
        public static readonly Vector2 DefaultCellSize = new Vector2(100, 100);

        /// <summary>
        /// デフォルトのセルのピボットです
        /// </summary>
        public static readonly Vector2 DefaultCellPivot = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// デフォルトのセル間の余白です
        /// </summary>
        public static readonly Vector2 DefaultSpacing = Vector2.zero;

#endregion

#region Abstract

        /// <summary>
        /// 可視領域のピボット(基準点)を参照/指定します
        /// </summary>
        /// <value>The view pivot.</value>
        public abstract Vector2 ViewPivot { get; set; }

        /// <summary>
        /// タイル全域の必要な大きさを返します
        /// </summary>
        /// <value>The size of the content preferred.</value>
        public abstract Vector2 ContentPreferredSize { get; }

        /// <summary>
        /// 指定したセルのインデックスに対応する列のインデックスを返します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="cellIndex">Cell index.</param>
        public abstract int ColumnIndex(int cellIndex);

        /// <summary>
        /// 指定したセルのインデックスに対応する行のインデックスを返します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="cellIndex">Cell index.</param>
        public abstract int RowIndex(int cellIndex);

        /// <summary>
        /// 行、列それぞれのインデックスから対応するセルのインデックスを返します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="columnIndex">Column index.</param>
        public abstract int CellIndex(int rowIndex, int columnIndex);

        /// <summary>
        /// スクロール量に応じてタイルの状態を同期させます
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="callback">Callback.</param>
        public abstract void SyncScrollPosition(Vector2 position, Action<IndexedRectTransform, int> callback);

#endregion

#region Field & Properties

        [SerializeField]
        Padding padding = DefaultPadding;

        /// <summary>
        /// タイル外周の余白を参照/指定します
        /// </summary>
        /// <value>The padding.</value>
        public Padding Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        [SerializeField]
        Vector2 cellSize = DefaultCellSize;

        /// <summary>
        /// セル一つあたりの面積を参照/指定します
        /// </summary>
        /// <value>The size of the cell.</value>
        public Vector2 CellSize
        {
            get { return cellSize; }
            set { cellSize = value; }
        }

        [SerializeField]
        Vector2 cellPivot = DefaultCellPivot;

        /// <summary>
        /// セルのピボット(基準点)を参照/指定します
        /// </summary>
        /// <value>The cell pivot.</value>
        public Vector2 CellPivot
        {
            get { return cellPivot; }
            set { cellPivot = value; }
        }

        [SerializeField]
        Vector2 spacing = DefaultSpacing;

        /// <summary>
        /// セル同士の余白(X軸方向/Y軸方向)を参照/指定します
        /// </summary>
        /// <value>The spacing.</value>
        public Vector2 Spacing
        {
            get { return spacing; }
            set { spacing = value; }
        }

        [SerializeField]
        readonly List<IndexedRectTransform> managedCells = new List<IndexedRectTransform>();

        /// <summary>
        /// タイルが保持管理しているセルのリストを参照します
        /// </summary>
        /// <value>The managed cells.</value>
        public List<IndexedRectTransform> ManagedCells
        {
            get { return managedCells; }
        }

        int bufferSize = -1;

        /// <summary>
        /// タイルが表示する仮想のセル総数を参照/指定します
        /// </summary>
        /// <value>The size of the buffer.</value>
        public int VirtualCellCount
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }

        [SerializeField]
        Vector2 viewportSize;

        /// <summary>
        /// 可視領域のサイズを参照/指定します
        /// </summary>
        /// <value>The size of the viewport.</value>
        public Vector2 ViewportSize
        {
            get { return viewportSize; }
            set { viewportSize = value; }
        }

        /// <summary>
        /// 描画領域外にある交換対象のセルを入れます
        /// </summary>
        static readonly Queue<IndexedRectTransform> swapCellQueue = new Queue<IndexedRectTransform>();

        /// <summary>
        /// 描画領域内にある交換対象としないセルをいれます
        /// </summary>
        static readonly HashSet<int> ignoreIndexes = new HashSet<int>();

#endregion

#region Information API

        /// <summary>
        /// 指定したセルを示す<see cref="IndexedRectTransform"/>インスタンスが管理下に含まれているかを返します
        /// </summary>
        /// <param name="cell">Cell.</param>
        public virtual bool Contains(IndexedRectTransform cell)
        {
            return cell != null && ManagedCells.Contains(cell);
        }

        /// <summary>
        /// 指定したセルを示す<see cref="RectTransform"/>インスタンスが管理下に含まれているかを返します
        /// </summary>
        /// <param name="rectTransform">Rect transform.</param>
        public virtual bool Contains(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                return false;
            }
            foreach (var cell in ManagedCells)
            {
                if (cell.RectTransform == rectTransform)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 指定したインデックスに対応するセルが管理下に含まれているかを返します
        /// </summary>
        /// <param name="index">Index.</param>
        public virtual bool Contains(int index)
        {
            foreach (var cell in ManagedCells)
            {
                if (cell.Index == index)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 行数、列数から最適なタイル全域の面積を計算します
        /// </summary>
        /// <returns>The preferred size.</returns>
        /// <param name="row">Row.</param>
        /// <param name="column">Column.</param>
        public Vector2 ComputePreferredSize(int row, int column)
        {
            var w = Padding.Left + Padding.Right + column * (CellSize.x + Spacing.x) - Spacing.x;
            var h = Padding.Top + Padding.Bottom + row * (CellSize.y + Spacing.y) - Spacing.y;
            return new Vector2(w, h);
        }

        protected int VisibleUnitCount(float totalSpan, float paddingHead, float paddingTail, float cellSpan, float cellMargin, bool extendable)
        {
            var remain = totalSpan + cellMargin;
            var unit = cellSpan + cellMargin;
            if (!extendable)
            {
                remain -= paddingHead + paddingTail;
            }
            if (Math.Abs(unit) < float.Epsilon)
            {
                return -1;
            }
            var div = remain / unit;
            return (int) (extendable ? Math.Ceiling(div) : Math.Floor(div));
        }

        /// <summary>
        /// 可視領域から視認できるセルの総数を計算します
        /// </summary>
        /// <returns>The cell count.</returns>
        /// <param name="extendable">タイルがセル総数に応じて伸縮するか否かを渡します</param>
        public virtual int VisibleCellCount(bool extendable)
        {
            return VisibleRowCount(false) * VisibleColumnCount(false);
        }

        /// <summary>
        /// 可視領域から視認できる列数を計算します
        /// </summary>
        /// <returns>The column count.</returns>
        /// <param name="extendable">タイルがセル総数に応じて伸縮するか否かを渡します</param>
        public virtual int VisibleColumnCount(bool extendable)
        {
            return VisibleUnitCount(ViewportSize.x, Padding.Left, Padding.Right, CellSize.x, Spacing.x, extendable);
        }

        /// <summary>
        /// 可視領域から視認できる行数を計算します
        /// </summary>
        /// <returns>The row count.</returns>
        /// <param name="extendable">タイルがセル総数に応じて伸縮するか否かを渡します</param>
        public virtual int VisibleRowCount(bool extendable)
        {
            return VisibleUnitCount(ViewportSize.y, Padding.Top, Padding.Bottom, CellSize.y, Spacing.y, extendable);
        }

        /// <summary>
        /// 指定された座標上にある列の最小インデックスを返します
        /// </summary>
        /// <remarks>
        /// 余白なくセルが隣接している場合、境界上の座標ではインデックスの小さい方が結果になります
        /// </remarks>
        /// <returns>The index.</returns>
        /// <param name="point">Point.</param>
        public virtual int ColumnIndex(Vector2 point)
        {
            if (point.x < Padding.Left + CellSize.x + Spacing.x)
            {
                return 0;
            }
            return (int) Math.Floor((point.x - Padding.Left) / (CellSize.x + Spacing.x));
        }

        /// <summary>
        /// 指定された座標上にある行の最小インデックスを返します
        /// </summary>
        /// <remarks>
        /// 余白なくセルが隣接している場合、境界上の座標ではインデックスの小さい方が結果になります
        /// </remarks>
        /// <returns>The index.</returns>
        /// <param name="point">Point.</param>
        public virtual int RowIndex(Vector2 point)
        {
            if (point.y < Padding.Top + CellSize.y + Spacing.y)
            {
                return 0;
            }
            return (int) Math.Floor((point.y - Padding.Top) / (CellSize.y + Spacing.y));
        }

        /// <summary>
        /// 指定された座標上にあるセルの最小インデックスを返します
        /// </summary>
        /// <remarks>
        /// 余白なくセルが隣接している場合、境界上の座標ではインデックスの小さい方が結果になります
        /// </remarks>
        /// <returns>The index.</returns>
        /// <param name="position">Position.</param>
        public virtual int CellIndex(Vector2 position)
        {
            var c = ColumnIndex(position);
            var r = RowIndex(position);
            return CellIndex(r, c);
        }

        /// <summary>
        /// 指定されたセルのインデックスに対応するセルの基準点座標を返します
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="cellIndex">Cell index.</param>
        public virtual Vector2 CellPosition(int cellIndex)
        {
            var rowIndex = RowIndex(cellIndex);
            var columnIndex = ColumnIndex(cellIndex);
            return CellPosition(rowIndex, columnIndex);
        }

        /// <summary>
        /// 指定された行列のそれぞれのインデックスに対応するセルの基準点座標を返します
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="columnIndex">Column index.</param>
        public virtual Vector2 CellPosition(int rowIndex, int columnIndex)
        {
            var x = Padding.Left + (CellSize.x + Spacing.x) * columnIndex + CellSize.x * CellPivot.x;
            var y = Padding.Top + (CellSize.y + Spacing.y) * rowIndex + CellSize.y * (1 - CellPivot.y);
            return new Vector2(x, y);
        }

        protected virtual int CurrentAddibleCellIndex
        {
            get
            {
                if (ManagedCells.Count == 0)
                {
                    return 0;
                }
                var exists = new HashSet<int>();
                int minIndex = 0;
                int maxIndex = 0;

                foreach (var cell in ManagedCells)
                {
                    var index = cell.Index;
                    minIndex = Math.Min(minIndex, index);
                    maxIndex = Math.Max(maxIndex, index);
                    exists.Add(index);
                }
                for (int i = minIndex + 1; i < maxIndex; i++)
                {
                    // ManagedCells の中で欠番になっている index があった場合
                    if (!exists.Contains(i))
                    {
                        return i;
                    }
                }
                return maxIndex + 1;
            }
        }

#endregion

#region Control

        /// <summary>
        /// 指定した行数、列数に応じて可視領域の面積を最適化します
        /// </summary>
        /// <returns>最適化された可視領域の面積</returns>
        /// <param name="rowSize">Row size.</param>
        /// <param name="columnSize">Column size.</param>
        public virtual Vector2 SetMatrix(int rowSize, int columnSize)
        {
            return ViewportSize = ComputePreferredSize(rowSize, columnSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="cellTransform">Cell transform.</param>
        public virtual IndexedRectTransform AddCell(RectTransform cellTransform)
        {
            var index = CurrentAddibleCellIndex;
            var newCell = new IndexedRectTransform(index, cellTransform);

            ManagedCells.Add(newCell);

            return newCell;
        }

        protected virtual void SyncCellIndexes(int visibleFirstCellIndex, Action<IndexedRectTransform, int> callback)
        {
            var visibleLastCellIndex = visibleFirstCellIndex + VisibleCellCount(true) - 1;
            swapCellQueue.Clear();
            ignoreIndexes.Clear();

            foreach (var cell in ManagedCells)
            {
                var index = cell.Index;
                if (index < visibleFirstCellIndex || index > visibleLastCellIndex)
                {
                    // セルが描画領域の外にいるので交換の対象にする
                    swapCellQueue.Enqueue(cell);
                }
                else
                {
                    // 描画領域の内側にいるので交換しない
                    ignoreIndexes.Add(index);
                }
            }

            for (int i = visibleFirstCellIndex; swapCellQueue.Count > 0 && i <= visibleLastCellIndex; i++)
            {
                if (ignoreIndexes.Contains(i))
                {
                    continue;
                }
                var cell = swapCellQueue.Dequeue();
                callback(cell, i);
            }
        }

#endregion

    }
}
