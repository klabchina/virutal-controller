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
    public abstract class ScrollSelectLoopLogicBase : IScrollSelectLoopTypeLogic
    {
        /// <summary>
        /// Modelクラスから必要な情報を参照するためのクラス
        /// </summary>
        protected ScrollSelectLoopTypeLogicProperty property;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property"></param>
        public ScrollSelectLoopLogicBase(ScrollSelectLoopTypeLogicProperty property)
        {
            this.property = property;
        }
        
        /// <summary>
        /// スクロールの開始点の座標を返します
        /// </summary>
        /// <value>The scroll origin point.</value>
        public abstract Vector2 ScrollOriginPoint { get; }
        
        /// <summary>
        /// リスト全域の必要な大きさを返します
        /// </summary>
        public virtual float ContentPreferredSize
        {
            get
            {
                // タップによるContentの最大移動量を画面一つ分としたときに
                // Viewport分の3倍あれば足りる計算だが余裕を持たせて4倍にする
                return property.ViewportLength * 4f;
            }
        }

        /// <summary>
        /// 指定されたindexからoffsetの分だけずらしたindexを返します
        /// 先頭と末尾のセルが繋がるようにします
        /// </summary>
        /// <param name="index"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual int GetCellIndexByOffset(int index, int offset)
        {
            return GetNormalizedIndex(index + offset);
        }

        /// <summary>
        /// 指定されたindexが選択セルのindexとして有効な範囲内におさまる値にします
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual int GetValidCellIndex(int index)
        {
            // ループ版は正規化されたindexの値を返す
            return GetNormalizedIndex(index);
        }

        /// <summary>
        /// 指定されたindexのセルが選択状態のときのContentのポジションを返します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual Vector3 CalculateJumpPositionBySelectedIndex(int index)
        {
            // ループ版の場合はジャンプの時にContentはスライドさせられるため選択セルのポジションはScrollOriginPointになる
            return ScrollOriginPoint;
        }

        /// <summary>
        /// 正規化されたindexの値を返します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual int GetNormalizedIndex(int index)
        {
            if (property.VirtualCellCount <= 0)
            {
                return 0;
            }
            var normalizedIndex = index % property.VirtualCellCount;
            if (normalizedIndex < 0)
            {
                return property.VirtualCellCount + normalizedIndex;
            }

            return normalizedIndex;
        }

    }
}
