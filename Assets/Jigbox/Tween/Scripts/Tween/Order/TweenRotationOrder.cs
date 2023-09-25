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
    public class TweenRotationOrder : TweenTransformOrder
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 回転更新用クラス
        /// </summary>
        protected class RotationUpdateCallback : UpdateCallback
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="transform">Transform</param>
            /// <param name="updateX">x値を更新するかどうか</param>
            /// <param name="updateY">y値を更新するかどうか</param>
            /// <param name="updateZ">z値を更新するかどうか</param>
            public RotationUpdateCallback(Transform transform, bool updateX, bool updateY, bool updateZ)
                : base(transform, updateX, updateY, updateZ)
            {
            }

            /// <summary>
            /// 回転状態を更新します。
            /// </summary>
            /// <param name="tween">Tween</param>
            public override void OnUpdate(ITween<Vector3> tween)
            {
                Vector3 value = tween.Value;
                Vector3 angle = new Vector3();

                angle.x = updateX ? value.x : transform.localEulerAngles.x;
                angle.y = updateY ? value.y : transform.localEulerAngles.y;
                angle.z = updateZ ? value.z : transform.localEulerAngles.z;

                transform.localEulerAngles = angle;
            }
        }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        public TweenRotationOrder(float duration) : base(duration)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x回転角</param>
        /// <param name="y">y回転角</param>
        /// <param name="z">z回転角</param>
        public TweenRotationOrder(float duration, float x, float y, float z) : base(duration, x, y, z)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="angle">回転角</param>
        public TweenRotationOrder(float duration, Vector3 angle) : base(duration, angle)
        {
        }

        /// <summary>
        /// Tweenの更新時に呼び出されるコールバックを生成します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <returns></returns>
        public override Action<ITween<Vector3>> CreateOnUpdate(Transform transform)
        {
            RotationUpdateCallback callback = new RotationUpdateCallback(
                transform,
                x.HasValue,
                y.HasValue,
                z.HasValue);

            return callback.OnUpdate;
        }

#endregion
    }
}
