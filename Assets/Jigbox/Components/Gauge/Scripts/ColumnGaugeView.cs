﻿/**
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
    public abstract class ColumnGaugeView : ColumnGaugeViewBase
    {
#region properties

        /// <summary>フィリング対象のRectTransformの状態を計算するための矩形領域</summary>
        protected RectTransform fillRect;

        /// <summary>フィリングを行う際に基準となる点</summary>
        protected int fillOrigin = 0;

        /// <summary>ゲージの起点となる座標</summary>
        public override Vector3 OriginPoint { get { return fillRect.position; } }

#endregion

#region public methods

        /// <summary>
        /// ビューの状態を初期化します。
        /// </summary>
        /// <param name="fillTarget">フィリング対象となるオブジェクト</param>
        /// <param name="fillMethod">フィリング方法</param>
        /// <param name="fillOrigin">フィリングを行う際に基準となる点</param>
        public override void InitView(Object fillTarget, int fillMethod, int fillOrigin)
        {
            target = fillTarget as RectTransform;
            this.fillOrigin = fillOrigin;
        }

        /// <summary>
        /// ビューの状態を初期化します。
        /// </summary>
        /// <param name="fillRect">フィリング対象のRectTransformの状態を計算するための矩形領域</param>
        /// <param name="fillTarget">フィリング対象となるオブジェクト</param>
        /// <param name="fillMethod">フィリング方法</param>
        /// <param name="fillOrigin">フィリングを行う際に基準と鳴る点</param>
        public abstract void InitView(RectTransform fillRect, Object fillTarget, int fillMethod, int fillOrigin);

#endregion
    }
}