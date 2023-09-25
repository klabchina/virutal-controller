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
    public abstract class TweenTransformOrder : TweenOrder
    {
#region inner classes, enum, and structs

        /// <summary>
        /// Tweenの更新用クラス
        /// </summary>
        protected abstract class UpdateCallback
        {
            /// <summary>Transform</summary>
            protected Transform transform;

            /// <summary>x値を更新するかどうか</summary>
            protected bool updateX;

            /// <summary>y値を更新するかどうか</summary>
            protected bool updateY;

            /// <summary>z値を更新するかどうか</summary>
            protected bool updateZ;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="transform">Transform</param>
            /// <param name="updateX">x値を更新するかどうか</param>
            /// <param name="updateY">y値を更新するかどうか</param>
            /// <param name="updateZ">z値を更新するかどうか</param>
            public UpdateCallback(Transform transform, bool updateX, bool updateY, bool updateZ)
            {
                this.transform = transform;
                this.updateX = updateX;
                this.updateY = updateY;
                this.updateZ = updateZ;
            }

            /// <summary>
            /// 位置を更新します。
            /// </summary>
            /// <param name="tween">Tween</param>
            public abstract void OnUpdate(ITween<Vector3> tween);
        }

#endregion

#region properties

        /// <summary>x値</summary>
        protected float? x;

        /// <summary>x値</summary>
        public float? X { get { return x; } set { x = value; } }

        /// <summary>y値</summary>
        protected float? y;

        /// <summary>y値</summary>
        public float? Y { get { return y; } set { y = value; } }

        /// <summary>z値</summary>
        protected float? z;

        /// <summary>z値</summary>
        public float? Z { get { return z; } set { z = value; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        public TweenTransformOrder(float duration) : base(duration)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x値</param>
        /// <param name="y">y値</param>
        public TweenTransformOrder(float duration, float x, float y) : base(duration)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x値</param>
        /// <param name="y">y値</param>
        /// <param name="z">z値</param>
        public TweenTransformOrder(float duration, float x, float y, float z) : base(duration)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="vector">xy値</param>
        public TweenTransformOrder(float duration, Vector2 vector) : base(duration)
        {
            this.x = vector.x;
            this.y = vector.y;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="vector">xyz値</param>
        public TweenTransformOrder(float duration, Vector3 vector) : base(duration)
        {
            this.x = vector.x;
            this.y = vector.y;
            this.z = vector.z;
        }

        /// <summary>
        /// ToメソッドでのTween.Finalに設定する値を取得します。
        /// </summary>
        /// <returns></returns>
        public Vector3 GetToValue()
        {
            Vector3 to = Vector3.zero;
            if (x.HasValue)
            {
                to.x = x.Value;
            }
            if (y.HasValue)
            {
                to.y = y.Value;
            }
            if (z.HasValue)
            {
                to.z = z.Value;
            }
            return to;
        }

        /// <summary>
        /// FromメソッドでのTween.Beginに設定する値を取得します。
        /// </summary>
        /// <returns></returns>
        public Vector3 GetFromValue()
        {
            Vector3 from = Vector3.zero;
            if (x.HasValue)
            {
                from.x = x.Value;
            }
            if (y.HasValue)
            {
                from.y = y.Value;
            }
            if (z.HasValue)
            {
                from.z = z.Value;
            }
            return from;
        }

        /// <summary>
        /// ByメソッドでのTween.Finalに設定する値を取得します。
        /// </summary>
        /// <param name="baseValue">元となる値</param>
        /// <returns></returns>
        public Vector3 GetByValue(Vector3 baseValue)
        {
            Vector3 to = baseValue;
            if (x.HasValue)
            {
                to.x += x.Value;
            }
            if (y.HasValue)
            {
                to.y += y.Value;
            }
            if (z.HasValue)
            {
                to.z += z.Value;
            }
            return to;
        }

        /// <summary>
        /// Tweenの更新時に呼び出されるコールバックを生成します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <returns></returns>
        public abstract Action<ITween<Vector3>> CreateOnUpdate(Transform transform);

#endregion
    }
}
