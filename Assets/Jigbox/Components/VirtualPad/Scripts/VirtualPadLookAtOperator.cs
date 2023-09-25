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
    using RotationAxis = VirtualPadRotationOperator.RotationAxis;

    /// <summary>
    /// バーチャルパッドの値に応じてGameObjectを特定軸を基準に回転させるクラス
    /// </summary>
    public class VirtualPadLookAtOperator : VirtualPadUpdateReceiver
    {
#region properties

        /// <summary>バーチャルパッドの入力で回転させる軸</summary>
        [SerializeField]
        protected RotationAxis axis = RotationAxis.Y;

#endregion

#region public methods

        /// <summary>
        /// バーチャルパッドの更新タイミングで毎フレーム呼び出されます。
        /// </summary>
        /// <param name="data">バーチャルパッドで処理されるデータ</param>
        public override void OnUpdate(VirtualPadData data)
        {
            Vector3 angle = Vector3.zero;

            // ここでの計算はバーチャルパッドの操作に対する視覚的な操作感を優先したデフォルトの計算方式
            switch (axis)
            {
                case RotationAxis.X:
                    angle.x = -data.EulerAngle;
                    break;
                case RotationAxis.Y:
                    angle.y = -data.EulerAngle;
                    break;
                case RotationAxis.Z:
                    angle.z = data.EulerAngle;
                    break;
            }

            transform.localEulerAngles = angle;
        }

#endregion
    }
}
