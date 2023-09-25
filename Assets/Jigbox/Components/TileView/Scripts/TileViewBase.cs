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
using UnityEngine.UI;
using System;

namespace Jigbox.Components
{
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class TileViewBase : VirtualCollectionView<ITilingLayout>
    {
#region Abstract

        /// <summary>
        /// 現在のスクロール位置を正規化された割合(0.0f 〜 1.0f)で計算します
        /// </summary>
        /// <value>The scroll rate.</value>
        public abstract float ScrollRate { get; }

        /// <summary>
        /// Viewport中央を基準とした、Contentの現在のスクロール位置を割合で返します
        /// </summary>
        public abstract float ContentPositionRate { get; }

        /// <summary>
        /// 指定されたインデックスに応じた正規化された割合(0.0f 〜 1.0f)を返します
        /// </summary>
        /// <returns>The by index.</returns>
        /// <param name="index">Index.</param>
        public abstract float RateByIndex(int index);

        /// <summary>
        /// 指定された正規化された割合(0.0f 〜 1.0f)に応じたスクロール位置をスライドします
        /// ヘッダ・フッタを除いたセルの範囲だけに適用したrate値でスライドしたいときに使用します
        /// </summary>
        /// <param name="rate">Rate.</param>
        public abstract void JumpByRate(float rate);

        /// <summary>
        /// 指定された正規化された割合(0.0f 〜 1.0f)に応じたスクロール位置をスライドします
        /// minとmaxの値をセルの範囲だけに適用するか指定できます
        /// </summary>
        /// <param name="rate">Rate.</param>
        /// <param name="withHeaderFooter">minとmaxにかかる範囲をヘッダ・フッタを除いたセルの範囲だけするか指定できます</param>
        protected abstract void JumpByRate(float rate, bool withHeaderFooter);

#endregion

#region SerializeField

        [HideInInspector]
        [SerializeField]
        Vector2 cellSize = TileModelBase.DefaultCellSize;

        [HideInInspector]
        [SerializeField]
        Vector2 cellPivot = TileModelBase.DefaultCellPivot;

        [HideInInspector]
        [SerializeField]
        Vector2 spacing = TileModelBase.DefaultSpacing;

#endregion

#region Properties

        /// <summary>
        /// タイルに敷き詰めるセルUIの面積を参照/指定します
        /// </summary>
        /// <value>The size of the cell.</value>
        public Vector2 CellSize
        {
            get { return Model.CellSize; }
            set { Model.CellSize = cellSize = value; }
        }

        /// <summary>
        /// タイルに敷き詰めるセルUIのピボットを参照/指定します
        /// </summary>
        /// <value>The cell pivot.</value>
        public Vector2 CellPivot
        {
            get { return Model.CellPivot; }
            set { Model.CellPivot = cellPivot = value; }
        }

        /// <summary>
        /// タイルに敷き詰めるセルUI同士のX軸、Y軸それぞれの方向の間隔を参照/指定します
        /// </summary>
        /// <value>The spacing.</value>
        public Vector2 Spacing
        {
            get { return Model.Spacing; }
            set { Model.Spacing = spacing = value; }
        }

        /// <summary>
        /// タイルの並びが視認できる領域(ビューポート)の、幅高さの大きさを返します
        /// </summary>
        /// <value>The size of the viewport.</value>
        public virtual Vector2 ViewportSize
        {
            get { return Viewport.rect.size; }
        }

        /// <summary>
        /// コンテナがタイル全域を表示する為に必要な面積を示します
        /// </summary>
        /// <value>The size of the content preferred.</value>
        public virtual Vector2 ContentPreferredSize
        {
            get { return Model.ContentPreferredSize; }
        }

        /// <summary>
        /// コンテナがとるべきアンカーを基準にしたサイズを計算して返します
        /// </summary>
        /// <value>The content fit size delta.</value>
        public override Vector2 ContentFitSizeDelta
        {
            get
            {
                var current = Viewport.rect.size;
                var require = Model.ContentPreferredSize + HeaderFooterSize;
                var w = Math.Max(0, require.x - current.x);
                var h = Math.Max(0, require.y - current.y);

                return new Vector2(w, h);
            }
        }

#endregion

#region Public Methods

        /// <summary>
        /// セルのインデックスから、列のインデックスを計算します
        /// </summary>
        /// <returns>The index by cell index.</returns>
        /// <param name="cellIndex">Cell index.</param>
        public virtual int ColumnIndex(int cellIndex)
        {
            return Model.ColumnIndex(cellIndex);
        }

        /// <summary>
        /// セルのインデックスから、行のインデックスを計算します
        /// </summary>
        /// <returns>The index by cell index.</returns>
        /// <param name="cellIndex">Cell index.</param>
        public virtual int RowIndex(int cellIndex)
        {
            return Model.RowIndex(cellIndex);
        }

        /// <summary>
        /// 親コンテナのアンカーからの相対位置座標を元に、セルの全体からのインデックスを計算します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="position">Position.</param>
        public override int CellIndex(Vector2 position)
        {
            var offsetX = Content.rect.width * Content.pivot.x;
            var offsetY = Content.rect.height * Content.pivot.y;

            var normalized = new Vector2(position.x + offsetX, -position.y + offsetY);
            return Model.CellIndex(normalized);
        }

        /// <summary>
        /// 行列のそれぞれのインデックスから、セルの全体からのインデックスを計算します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="columnIndex">Column index.</param>
        public virtual int CellIndex(int rowIndex, int columnIndex)
        {
            return Model.CellIndex(rowIndex, columnIndex);
        }

        /// <summary>
        /// セルのインデックスから、親コンテナのアンカーからの、セルの相対位置座標を計算します
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="index">Index.</param>
        public override Vector2 CellPosition(int index)
        {
            var p = Model.CellPosition(index) + HeaderSize;
            return OffsetPosition(p);
        }

        /// <summary>
        /// 行列のそれぞれのインデックスから、親コンテナのアンカーからの、セルの相対位置座標を計算します
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="rowIndex">Row.</param>
        /// <param name="columnIndex">Column.</param>
        public virtual Vector2 CellPosition(int rowIndex, int columnIndex)
        {
            var p = Model.CellPosition(rowIndex, columnIndex);
            return OffsetPosition(p);
        }

        /// <summary>
        /// 指定されたインデックスの位置までスクロール位置をスライドします
        /// </summary>
        /// <param name="index">Index.</param>
        public virtual void JumpByIndex(int index)
        {
            JumpByRate(RateByIndex(index), false);
        }

        /// <summary>
        /// 指定された正規化された割合(0.0f 〜 1.0f)に応じたスクロール位置をスライドします
        /// ヘッダ・フッタ含めた範囲に適用されます
        /// </summary>
        /// <param name="rate">Rate.</param>
        public virtual void JumpByRateWithHeaderFooter(float rate)
        {
            JumpByRate(rate, true);
        }

        /// <summary>
        /// シリアライズフィールドとモデル層の状態を同期させます
        /// </summary>
        public override void BatchSerializeField()
        {
            Model.CellSize = cellSize;
            Model.CellPivot = cellPivot;
            Model.Spacing = spacing;
            base.BatchSerializeField();
        }

#endregion

#region Protected Method

        protected override void UpdateCellTransform(IndexedRectTransform cell)
        {
            var position = CellPosition(cell.Index);
            cell.SetCellTransform(position, Model.CellSize, Model.CellPivot, Content.pivot, Content.pivot);
        }

        protected Vector2 OffsetPosition(Vector2 position)
        {
            var offsetX = Content.rect.width * Content.pivot.x;
            var offsetY = Content.rect.height * (1.0f - Content.pivot.y);
            return new Vector2(position.x - offsetX, -position.y + offsetY);
        }

#endregion

#region Unity Method
#if UNITY_EDITOR

        protected override void OnDelayValidate()
        {
            cellSize = new Vector2
            {
                x = Math.Max(1, cellSize.x),
                y = Math.Max(1, cellSize.y)
            };
            cellPivot = new Vector2
            {
                x = Mathf.Clamp01(cellPivot.x),
                y = Mathf.Clamp01(cellPivot.y)
            };
            base.OnDelayValidate();
        }
#endif

#endregion
    }
}
