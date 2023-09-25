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
    public abstract class ListViewBase : VirtualCollectionView<IListLayout>
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

#region Serialize Fields

        [HideInInspector]
        [SerializeField]
        protected float cellSize = ListModelBase.DefaultCellSize;

        [HideInInspector]
        [SerializeField]
        protected Vector2 cellPivot = ListModelBase.DefaultCellPivot;

        [HideInInspector]
        [SerializeField]
        protected float spacing = ListModelBase.DefaultSpacing;

#endregion

#region Properties

        /// <summary>
        /// 敷き詰めるセルUIの幅を参照/指定します
        /// </summary>
        /// <remarks>水平方向に並ぶ場合ならwidthとして、垂直方向に並ぶ場合ならheightとして参照されます</remarks>
        /// <value>The size of the cell.</value>
        public float CellSize
        {
            get { return Model.CellSize; }
            set { Model.CellSize = cellSize = value; }
        }

        /// <summary>
        /// 敷き詰めるセルUIのピボットを参照/指定します
        /// </summary>
        /// <value>The cell pivot.</value>
        public Vector2 CellPivot
        {
            get { return Model.CellPivot; }
            set { Model.CellPivot = cellPivot = value; }
        }

        /// <summary>
        /// 敷き詰めるセルUI同士の間隔を参照/指定します
        /// </summary>
        /// <value>The spacing.</value>
        public float Spacing
        {
            get { return Model.Spacing; }
            set { Model.Spacing = spacing = value; }
        }

#endregion

#region Public Method

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

#region Unity Method

#if UNITY_EDITOR
        protected override void OnDelayValidate()
        {
            cellSize = Math.Max(1, cellSize);
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
