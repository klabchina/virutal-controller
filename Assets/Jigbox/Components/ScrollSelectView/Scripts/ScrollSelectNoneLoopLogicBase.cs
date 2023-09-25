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

namespace Jigbox.Components
{
    public abstract class ScrollSelectNoneLoopLogicBase : IScrollSelectLoopTypeLogic
    {
        /// <summary>
        /// ScrollOriginPoint用にVector2.zeroをキャッシュしておきます
        /// </summary>
        protected static readonly Vector2 CachedScrollOriginPoint = Vector2.zero;

        /// <summary>
        /// Modelクラスから必要な情報を参照するためのクラス
        /// </summary>
        protected ScrollSelectLoopTypeLogicProperty property;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property"></param>
        public ScrollSelectNoneLoopLogicBase(ScrollSelectLoopTypeLogicProperty property)
        {
            this.property = property;
        }
        
        /// <summary>
        /// スクロールの開始点の座標を返します
        /// </summary>
        /// <value>The scroll origin point.</value>
        public Vector2 ScrollOriginPoint
        {
            get { return CachedScrollOriginPoint; }
        }

        /// <summary>
        /// リスト全域の必要な大きさを返します
        /// </summary>
        public virtual float ContentPreferredSize
        {
            get
            {
                // 選択セルが1つある状態はViewportの領域のままになる
                // 選択セルが2つ以上ある場合は、その分の領域を加える
                
                // 全体のセルの数から1つ引いた値から必要な領域を求める
                var count = Mathf.Max(0, property.VirtualCellCount - 1);
                var cellContentSize = (property.CellSize + property.Spacing) * count;
                // Viewportの領域に(全体セル - 1)分を加えた値を返す
                return property.ViewportLength + cellContentSize;
            }
        }

        /// <summary>
        /// 指定されたindexからoffsetの分だけずらしたindexを返します
        /// Loop版ではないためoffset分を加えるだけになります
        /// </summary>
        /// <param name="index"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual int GetCellIndexByOffset(int index, int offset)
        {
            return index + offset;
        }

        /// <summary>
        /// 指定されたindexが選択セルのindexとして有効な範囲内におさまる値にします
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetValidCellIndex(int index)
        {
            return Mathf.Clamp(index, 0, property.VirtualCellCount - 1);
        }

        /// <summary>
        /// 指定されたindexのセルが選択状態のときのContentのポジションを返します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract Vector3 CalculateJumpPositionBySelectedIndex(int index);
    }
}
