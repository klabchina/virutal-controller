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
    /// ポインタの移動によるジェスチャーで処理されるデータ
    /// </summary>
    public class DragGestureEventData : IGestureEventData
    {
#region properties

        /// <summary>Camera</summary>
        public Camera EventCamera { get; set; }

        /// <summary>入力イベントが発生したGameObjectのRectTransform</summary>
        public RectTransform EventTarget { get; set; }

        /// <summary>ポインタが移動する前の開始位置の座標</summary>
        public Vector2 BeginPosition { get; set; }

        /// <summary>ポインタの現在の座標</summary>
        public Vector2 Position { get; set; }
        
        /// <summary>ポインタの前回からの移動量</summary>
        public Vector2 Movement { get; set; }
        
        /// <summary>開始位置からの移動量</summary>
        public Vector2 MovementFromBegin { get { return Position - BeginPosition; } }

        /// <summary>開始位置からの移動量が表す角度(-π～π)</summary>
        public float Angle { get { return VectorAngleUtils.GetAngle(MovementFromBegin.normalized); } }

        /// <summary>開始位置からの移動量が表す角度(オイラー角)</summary>
        public float EulerAngle { get { return VectorAngleUtils.ToEuler(Angle); } }

#endregion
    }
}
