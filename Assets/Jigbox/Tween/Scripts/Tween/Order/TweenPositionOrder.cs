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
    public class TweenPositionOrder : TweenTransformOrder
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 位置更新用クラス
        /// </summary>
        protected class PositionUpdateCallback : UpdateCallback
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="transform">Transform</param>
            /// <param name="updateX">x値を更新するかどうか</param>
            /// <param name="updateY">y値を更新するかどうか</param>
            /// <param name="updateZ">z値を更新するかどうか</param>
            public PositionUpdateCallback(Transform transform, bool updateX, bool updateY, bool updateZ)
                : base(transform, updateX, updateY, updateZ)
            {
            }

            /// <summary>
            /// 位置を更新します。
            /// </summary>
            /// <param name="tween">Tween</param>
            public override void OnUpdate(ITween<Vector3> tween)
            {
                Vector3 value = tween.Value;
                Vector3 position = new Vector3();

                position.x = updateX ? value.x : transform.localPosition.x;
                position.y = updateY ? value.y : transform.localPosition.y;
                position.z = updateZ ? value.z : transform.localPosition.z;

                transform.localPosition = position;
            }
        }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        public TweenPositionOrder(float duration) : base(duration)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        public TweenPositionOrder(float duration, float x, float y) : base(duration, x, y)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        /// <param name="z">z座標</param>
        public TweenPositionOrder(float duration, float x, float y, float z) : base(duration, x, y, z)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="position">xy座標</param>
        public TweenPositionOrder(float duration, Vector2 position) : base(duration, position)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="position">座標</param>
        public TweenPositionOrder(float duration, Vector3 position) : base(duration, position)
        {
        }

        /// <summary>
        /// Tweenの更新時に呼び出されるコールバックを生成します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <returns></returns>
        public override Action<ITween<Vector3>> CreateOnUpdate(Transform transform)
        {
            PositionUpdateCallback callback = new PositionUpdateCallback(
                transform,
                x.HasValue,
                y.HasValue,
                z.HasValue);

            return callback.OnUpdate;
        }

#endregion
    }
}
