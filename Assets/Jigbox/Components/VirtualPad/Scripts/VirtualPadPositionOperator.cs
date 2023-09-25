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
    /// バーチャルパッドの値に応じてGameObjectを移動させるクラス
    /// </summary>
    public class VirtualPadPositionOperator : VirtualPadUpdateReceiver
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 移動を行う3次元空間上の面
        /// </summary>
        public enum MotionPlane
        {
            /// <summary>xy平面</summary>
            XY,
            /// <summary>xz平面</summary>
            XZ,
        }

#endregion

#region properties

        /// <summary>移動を行う３次元空間上の面</summary>
        [SerializeField]
        protected MotionPlane motionPlane = MotionPlane.XY;

        /// <summary>速度の最大値</summary>
        [SerializeField]
        protected float velocityMax = 1.0f;

#endregion

#region public methods

        /// <summary>
        /// バーチャルパッドの更新タイミングで毎フレーム呼び出されます。
        /// </summary>
        /// <param name="data">バーチャルパッドで処理されるデータ</param>
        public override void OnUpdate(VirtualPadData data)
        {
            Vector3 movement = Vector3.zero;
            if (motionPlane == MotionPlane.XY)
            {
                movement.x = data.Axis.x * velocityMax;
                movement.y = data.Axis.y * velocityMax;
            }
            else
            {
                movement.x = data.Axis.x * velocityMax;
                movement.z = data.Axis.y * velocityMax;
            }

            transform.localPosition += movement;
        }

#endregion
    }
}
