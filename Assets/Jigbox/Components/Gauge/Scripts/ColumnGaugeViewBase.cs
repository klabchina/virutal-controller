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
    public abstract class ColumnGaugeViewBase : GaugeViewBase
    {
#region properties

        /// <summary>フィリング対象のRectTransform</summary>
        protected RectTransform target;

        /// <summary>ゲージの起点となる座標</summary>
        public virtual Vector3 OriginPoint { get { return target.position; } }

        /// <summary>ゲージの終点となる座標</summary>
        public virtual Vector3 TerminatePoint { get { return OriginPoint + distance; } }

        /// <summary>現在のゲージ量での座標</summary>
        protected Vector3 currentPoint = Vector3.zero;

        /// <summary>現在のゲージ量での座標</summary>
        public virtual Vector3 CurrentPoint { get { return currentPoint; } }
        
        /// <summary>ゲージの起点から終点までの距離</summary>
        protected Vector3 distance = Vector3.zero;

#endregion

#region public methods

        /// <summary>
        /// ビューの座標情報を計算します。
        /// </summary>
        /// <param name="value">ゲージの値</param>
        public abstract void CalculatePoint(float value = 1.0f);

#endregion

#region preotected methods

        /// <summary>
        /// ゲージの値から現在のゲージ量における座標位置を求めます。
        /// </summary>
        /// <param name="value">ゲージの値</param>
        protected abstract void CalculateCurrentPoint(float value);

#endregion

    }
}
