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

namespace Jigbox.Components
{
    /// <summary>
    /// 2点間の距離の変化によるジェスチャーを判別するクラス
    /// </summary>
    public class PinchGestureUmpire : DualPointerGestureUmpire<PinchGestureType, PinchGestureEventData, PinchGestureEventHandler>
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 2点間の距離の変化によるジェスチャーの標準デリゲート型
        /// </summary>
        public class PinchGestureDelegate : EventDelegate<PinchGestureEventData>
        {
            public PinchGestureDelegate(Callback eventDelegate) : base(eventDelegate)
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

            data.DistanceDelta = Vector2.zero;
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

            Vector2 lastDistance = data.Distance;
            data.PrimaryPosition = GetPosition(primaryPointer);
            data.SecondaryPosition = GetPosition(secondaryPointer);
            Vector2 distance = data.Distance;

            if (distance.x == lastDistance.x && distance.y == lastDistance.y)
            {
                return;
            }

            data.DistanceDelta = distance - lastDistance;
            float distanceMag = distance.sqrMagnitude;
            float lastDistanceMag = lastDistance.sqrMagnitude;
            data.DistanceSqrMagnitudeDelta = distanceMag - lastDistanceMag;

            Dispatch(PinchGestureType.Pinch);

            if (distanceMag > lastDistanceMag)
            {
                Dispatch(PinchGestureType.PinchOut);
            }
            else
            {
                Dispatch(PinchGestureType.PinchIn);
            }
        }

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <param name="callback">void Func(PinchGestureEventData)の関数</param>
        /// <returns></returns>
        public override bool AddEvent(PinchGestureType type, PinchGestureDelegate.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("GestureUmpire.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(new PinchGestureDelegate(callback));
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
        protected override PinchGestureEventHandler CreateHandler(PinchGestureType type)
        {
            return new PinchGestureEventHandler(type);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            data = new PinchGestureEventData();
        }

#endregion
    }
}
