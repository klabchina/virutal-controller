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

namespace Jigbox.Gesture
{
    /// <summary>
    /// 2点間の距離の変化によるジェスチャーで処理されるデータ
    /// </summary>
    public class PinchGestureEventData : IGestureEventData
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
        
        /// <summary>2点間の距離</summary>
        public Vector2 Distance { get { return SecondaryPosition - PrimaryPosition; } }

        /// <summary>前回からの距離の変化量</summary>
        public Vector2 DistanceDelta { get; set; }

        /// <summary>2点間の距離の長さの2乗</summary>
        public float DistanceSqrMagnitude { get { return Distance.sqrMagnitude; } }

        /// <summary>2点間の距離の長さの2乗の変化量</summary>
        public float DistanceSqrMagnitudeDelta { get; set; }

        /// <summary>
        /// <para>2点間の距離のスカラー値</para>
        /// <para>計算コストが大きいため、本当に必要な場合を除いてDistanceSqrMagnitudeを利用して下さい。</para>
        /// </summary>
        public float Scalar { get { return Distance.magnitude; } }

#endregion
    }
}
