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
using Jigbox.UIControl;

namespace Jigbox.Gesture
{
    /// <summary>
    /// 2点の相対的な角度の変化によるジェスチャーで処理されるデータ
    /// </summary>
    public class RotateGestureEventData : IGestureEventData
    {
#region properties

        /// <summary>Camera</summary>
        public Camera EventCamera { get; set; }

        /// <summary>入力イベントが発生したGameObjectのRectTransform</summary>
        public RectTransform EventTarget { get; set; }

        /// <summary>1番目のポインタの座標</summary>
        public Vector2 PrimaryPosition { get; set; }

        /// <summary>2番目のポインタの座標</summary>
        public Vector2 SecondaryPosition { get; set; }

        /// <summary>1点目のポインタから2点目のポインタへの向きを表す単位ベクトル</summary>
        public Vector2 Direction { get { return (SecondaryPosition - PrimaryPosition).normalized; } }
        
        /// <summary>2点のポインタの相対的な角度(-π～π)</summary>
        public float Angle { get { return VectorAngleUtils.GetAngle(Direction); } }

        /// <summary>2点のポインタの相対的な角度の前回からの変化量</summary>
        public float AngleDelta { get; set; }

        /// <summary>2点のポインタの相対的な角度(オイラー角)</summary>
        public float EulerAngle { get { return VectorAngleUtils.ToEuler(Angle); } }

        /// <summary>2点のポインタの相対的な角度の前回からの変化量(オイラー角)</summary>
        public float EulerAngleDelta { get { return VectorAngleUtils.ToEuler(AngleDelta); } }

#endregion
    }
}
