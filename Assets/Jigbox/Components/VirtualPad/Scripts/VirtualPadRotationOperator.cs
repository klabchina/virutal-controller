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
using Jigbox.VirtualPad;

namespace Jigbox.Components
{
    /// <summary>
    /// バーチャルパッドの値に応じてGameObjectを回転させるクラス
    /// </summary>
    public class VirtualPadRotationOperator : VirtualPadUpdateReceiver
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 回転させる軸
        /// </summary>
        public enum RotationAxis
        {
            /// <summary>x軸</summary>
            X,
            /// <summary>y軸</summary>
            Y,
            /// <summary>z軸</summary>
            Z,
        }

#endregion

#region properties

        /// <summary>バーチャルパッドの横方向の入力に対応する回転軸</summary>
        [SerializeField]
        protected RotationAxis horizontalRotationAxis = RotationAxis.Y;

        /// <summary>バーチャルパッドの縦方向の入力に対応する回転軸</summary>
        [SerializeField]
        protected RotationAxis verticalRotationAxis = RotationAxis.X;

        /// <summary>回転速度の最大値</summary>
        [SerializeField]
        protected float rotationSpeed = 1.0f;

        /// <summary>ローカル空間上での回転量</summary>
        protected Vector3 localAngle = Vector3.zero;

#endregion

#region public methods

        /// <summary>
        /// バーチャルパッドの更新タイミングで毎フレーム呼び出されます。
        /// </summary>
        /// <param name="data">バーチャルパッドで処理されるデータ</param>
        public override void OnUpdate(VirtualPadData data)
        {
            // ここでの計算はバーチャルパッドの操作に対する視覚的な操作感を優先したデフォルトの計算方式
            switch (horizontalRotationAxis)
            {
                case RotationAxis.X:
                    localAngle.x += rotationSpeed * data.Axis.x;
                    break;
                case RotationAxis.Y:
                    localAngle.y -= rotationSpeed * data.Axis.x;
                    break;
                case RotationAxis.Z:
                    localAngle.z -= rotationSpeed * data.Axis.x;
                    break;
            }

            switch (verticalRotationAxis)
            {
                case RotationAxis.X:
                    localAngle.x += rotationSpeed * data.Axis.y;
                    break;
                case RotationAxis.Y:
                    localAngle.y -= rotationSpeed * data.Axis.y;
                    break;
                case RotationAxis.Z:
                    localAngle.z -= rotationSpeed * data.Axis.y;
                    break;
            }

            transform.localEulerAngles = localAngle;
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            localAngle = transform.localEulerAngles;
        }

#endregion
    }
}
