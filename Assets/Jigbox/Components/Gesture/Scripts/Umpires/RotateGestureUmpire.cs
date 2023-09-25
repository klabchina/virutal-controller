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
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Jigbox.Gesture;
using Jigbox.Delegatable;
using Jigbox.UIControl;

namespace Jigbox.Components
{
    /// <summary>
    /// 2点の相対的な角度の変化によるジェスチャーを判別するクラス
    /// </summary>
    public class RotateGestureUmpire : DualPointerGestureUmpire<RotateGestureType, RotateGestureEventData, RotateGestureEventHandler>
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 2点の相対的な角度の変化によるジェスチャーの標準デリゲート型
        /// </summary>
        public class RotateGestureDelegate : EventDelegate<RotateGestureEventData>
        {
            public RotateGestureDelegate(Callback eventDelegate) : base(eventDelegate)
            {
            }
        }

#endregion

#region public methods

        /// <summary>
        /// ポインタが押下された際に呼び出されます。
        /// </summary>
        /// <param name="pressedPointer">押下されたポインタの情報</param>
        /// <param name="pressedPointers">押下されている全てのポインタ</param>
        public override void OnPress(PointerEventData pressedPointer, HashSet<PointerEventData> pressedPointers)
        {
            if (!ValidatePress(pressedPointer))
            {
                return;
            }

            PointerEventData primaryPointer = GetPrimaryPointer(pressedPointers);
            PointerEventData secondaryPointer = GetSecondaryPointer(pressedPointers);

            if (primaryPointer != null)
            {
                data.EventCamera = primaryPointer.pressEventCamera;
                var secondCamera = false;
#if UNITY_EDITOR || UNITY_STANDALONE
                // 擬似入力でイベントを発行した場合、カメラの参照はnullになるので、
                // 通常の入力からカメラを取得する
                secondCamera = true;
#endif
                if (InputWrapper.IsInputSystem || secondCamera)
                {
                    if (secondaryPointer != null && data.EventCamera == null)
                    {
                        data.EventCamera = secondaryPointer.pressEventCamera;
                    }
                }
                data.EventTarget = transform as RectTransform;
                data.PrimaryPosition = GetPosition(primaryPointer);
            }

            if (secondaryPointer != null)
            {
                data.SecondaryPosition = GetPosition(secondaryPointer);
            }

            data.AngleDelta = 0.0f;
        }

        /// <summary>
        /// 押下後に指が離された際に呼び出されます。
        /// </summary>
        /// <param name="releasedPointer">離されたポインタの情報</param>
        /// <param name="pressedPointers">押下されている全てのポインタ</param>
        public override void OnRelease(PointerEventData releasedPointer, HashSet<PointerEventData> pressedPointers)
        {
            if (!ValidateRelease(releasedPointer))
            {
                return;
            }
        }

        /// <summary>
        /// ポインタの状態からジェスチャーの状態を判別して、更新します。
        /// </summary>
        /// <param name="pressedPointers">押下されている全てのポインタ</param>
        public override void OnUpdate(HashSet<PointerEventData> pressedPointers)
        {
            PointerEventData primaryPointer = GetPrimaryPointer(pressedPointers);
            PointerEventData secondaryPointer = GetSecondaryPointer(pressedPointers);

            if (primaryPointer == null || secondaryPointer == null)
            {
                return;
            }

            Vector3 lastDirection = data.Direction;
            data.PrimaryPosition = GetPosition(primaryPointer);
            data.SecondaryPosition = GetPosition(secondaryPointer);
            Vector3 direction = data.Direction;

            if (direction.x == lastDirection.x && direction.y == lastDirection.y)
            {
                return;
            }

            // data.Angle - lastAngleだと、角度の表現範囲以上の問題で-πからπの近似値やその逆など
            // 一気に変化するタイミングがあり、その場合、角度的な表現と実際の変化量に際が発生するので
            // 角度の差については2つのベクトルのなす角を用いる
            data.AngleDelta = VectorAngleUtils.GetAngle(lastDirection, direction);

            Dispatch(RotateGestureType.Rotate);

            if (data.AngleDelta > 0)
            {
                Dispatch(RotateGestureType.RotateCounterclockwise);
            }
            else
            {
                Dispatch(RotateGestureType.RotateClockwise);
            }
        }

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <param name="callback">void Func(PinchGestureEventData)の関数</param>
        /// <returns></returns>
        public override bool AddEvent(RotateGestureType type, RotateGestureDelegate.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("GestureUmpire.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(new RotateGestureDelegate(callback));
            AddEvent(type, delegatable);

            return true;
        }

#endregion

#region protected methods

        /// <summary>
        /// イベントハンドラを作成します
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <returns>作成されたイベントハンドラを返します。</returns>
        protected override RotateGestureEventHandler CreateHandler(RotateGestureType type)
        {
            return new RotateGestureEventHandler(type);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            data = new RotateGestureEventData();
        }

#endregion
    }
}
