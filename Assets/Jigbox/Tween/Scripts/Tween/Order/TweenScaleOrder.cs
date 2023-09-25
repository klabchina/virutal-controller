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

namespace Jigbox.Tween
{
    public class TweenScaleOrder : TweenTransformOrder
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 拡縮更新用クラス
        /// </summary>
        protected class ScaleUpdateCallback : UpdateCallback
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="transform">Transform</param>
            /// <param name="updateX">x値を更新するかどうか</param>
            /// <param name="updateY">y値を更新するかどうか</param>
            /// <param name="updateZ">z値を更新するかどうか</param>
            public ScaleUpdateCallback(Transform transform, bool updateX, bool updateY, bool updateZ)
                : base(transform, updateX, updateY, updateZ)
            {
            }

            /// <summary>
            /// 拡縮を更新します。
            /// </summary>
            /// <param name="tween">Tween</param>
            public override void OnUpdate(ITween<Vector3> tween)
            {
                Vector3 value = tween.Value;
                Vector3 scale = new Vector3();

                scale.x = updateX ? value.x : transform.localScale.x;
                scale.y = updateY ? value.y : transform.localScale.y;
                scale.z = updateZ ? value.z : transform.localScale.z;

                transform.localScale = scale;
            }
        }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        public TweenScaleOrder(float duration) : base(duration)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x拡縮率</param>
        /// <param name="y">y拡縮率</param>
        public TweenScaleOrder(float duration, float x, float y) : base(duration, x, y)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x拡縮率</param>
        /// <param name="y">y拡縮率</param>
        /// <param name="z">z拡縮率</param>
        public TweenScaleOrder(float duration, float x, float y, float z) : base(duration, x, y, z)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="position">xy拡縮率</param>
        public TweenScaleOrder(float duration, Vector2 position) : base(duration, position)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="position">拡縮率</param>
        public TweenScaleOrder(float duration, Vector3 position) : base(duration, position)
        {
        }

        /// <summary>
        /// Tweenの更新時に呼び出されるコールバックを生成します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <returns></returns>
        public override Action<ITween<Vector3>> CreateOnUpdate(Transform transform)
        {
            ScaleUpdateCallback callback = new ScaleUpdateCallback(
                transform,
                x.HasValue,
                y.HasValue,
                z.HasValue);

            return callback.OnUpdate;
        }

#endregion
    }
}
