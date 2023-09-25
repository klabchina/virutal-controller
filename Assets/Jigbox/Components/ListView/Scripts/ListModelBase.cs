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
using System.Collections.Generic;

namespace Jigbox.Components
{
    [Serializable]
    public abstract class ListModelBase : IListLayout
    {
#region Static Constants

        /// <summary>
        /// The default padding.
        /// </summary>
        public static readonly Padding DefaultPadding = Padding.zero;

        /// <summary>
        /// The default size of the cell.
        /// </summary>
        public static readonly float DefaultCellSize = 100f;

        /// <summary>
        /// The default cell pivot.
        /// </summary>
        public static readonly Vector2 DefaultCellPivot = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// The default spacing.
        /// </summary>
        public static readonly float DefaultSpacing;

#endregion

#region Abstract

        /// <summary>
        /// 可視領域のピボット(基準点)を参照/指定します
        /// </summary>
        /// <value>The view pivot.</value>
        public abstract Vector2 ViewPivot { get; set; }

        /// <summary>
        /// リスト全域の必要な大きさを返します
        /// </summary>
        /// <value>The size of the content preferred.</value>
        public abstract float ContentPreferredSize { get; }

        /// <summary>
        /// 可視領域から視認できるセルの個数を返します
        /// </summary>
        /// <returns>The cell count.</returns>
        /// <param name="extendable">If set to <c>true</c> extendable.</param>
        public abstract int VisibleCellCount(bool extendable);

        /// <summary>
        /// 座標に相当するセルの最小インデックスを返します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="span">Span.</param>
        public abstract int CellIndex(float span);

        /// <summary>
        /// 座標に相当するセルの最小インデックスを返します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="position">Position.</param>
        public abstract int CellIndex(Vector2 position);

        /// <summary>
        /// 渡されたインデックスに相当する座標を返します
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="index">Index.</param>
        public abstract float CellPosition(int index);

#endregion

#region Fields & Properties

        [SerializeField]
        Padding padding = DefaultPadding;

        /// <summary>
        /// リスト外周の余白を参照/指定します
        /// </summary>
        /// <value>The padding.</value>
        public virtual Padding Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        [SerializeField]
        float cellSize = DefaultCellSize;

        /// <summary>
        /// セル一つあたりの"幅"を参照/指定します
        /// </summary>
        /// <remarks>
        /// 横方向に並ぶ場合は横幅(width)、縦方向に並ぶ場合は縦幅(height)として用いられます
        /// </remarks>
        /// <value>The size of the cell.</value>
        public virtual float CellSize
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
        public virtual Vector2 CellPivot
        {
            get { return cellPivot; }
            set { cellPivot = value; }
        }

        [SerializeField]
        float spacing = DefaultSpacing;

        /// <summary>
        /// セル同士の余白を参照/指定します
        /// </summary>
        /// <value>The spacing.</value>
        public virtual float Spacing
        {
            get { return spacing; }
            set { spacing = value; }
        }

        int virtualCellCount = -1;

        /// <summary>
        /// リストが表示する仮想のセル総数を参照/指定します
        /// </summary>
        /// <value>The virtual cell count.</value>
        public virtual int VirtualCellCount
        {
            get { return virtualCellCount; }
            set { virtualCellCount = value; }
        }

        [SerializeField]
        readonly List<IndexedRectTransform> managedCells = new List<IndexedRectTransform>();

        /// <summary>
        /// リストが保持管理しているセルのリストを参照します
        /// </summary>
        /// <value>The managed cells.</value>
        public virtual List<IndexedRectTransform> ManagedCells
        {
            get { return managedCells; }
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

        /// <summary>
        /// 描画領域外にある交換対象のセルを入れます
        /// </summary>
        static readonly Queue<IndexedRectTransform> swap = new Queue<IndexedRectTransform>();

        /// <summary>
        /// 描画領域内にある交換対象としないセルをいれます
        /// </summary>
        static readonly HashSet<int> ignore = new HashSet<int>();

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
        /// <param name="rectTransform">Cell.</param>
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

#endregion

#region Control API

        /// <summary>
        /// セルを新たに追加します
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="cellTransform">Cell transform.</param>
        public virtual IndexedRectTransform AddCell(RectTransform cellTransform)
        {
            var newCell = new IndexedRectTransform(CurrentAddibleCellIndex, cellTransform);
            ManagedCells.Add(newCell);
            return newCell;
        }

        /// <summary>
        /// スクロール量に応じてリストの状態を同期させます
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="callback">Callback.</param>
        public virtual void SyncScrollPosition(Vector2 position, Action<IndexedRectTransform, int> callback)
        {
            var first = CellIndex(position);
            var last = first + VisibleCellCount(true) - 1;
            swap.Clear();
            ignore.Clear();

            foreach (var cell in ManagedCells)
            {
                var index = cell.Index;
                if (index < first || index > last)
                {
                    // セルが描画領域の外にいるので交換の対象にする
                    swap.Enqueue(cell);
                }
                else
                {
                    // 描画領域の内側にいるので交換しない
                    ignore.Add(index);
                }
            }
            for (int i = first; swap.Count > 0 && i <= last; i++)
            {
                if (ignore.Contains(i))
                {
                    continue;
                }
                var cell = swap.Dequeue();
                callback(cell, i);
            }
        }

#endregion
    }
}
