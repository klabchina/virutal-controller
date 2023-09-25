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
    using Direction = VectorDirectionUtils.Direction;

    /// <summary>
    /// 特定方向への移動によるジェスチャーを判別するクラス
    /// </summary>
    public class SwipeGestureUmpire : SinglePointerGestureUmpire<SwipeGestureType, SwipeGestureEventData, SwipeGestureEventHandler>
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 特定方向への移動によるジェスチャーの標準デリゲート型
        /// </summary>
        public class SwipeGestureDelegate : EventDelegate<SwipeGestureEventData>
        {
            public SwipeGestureDelegate(Callback eventDelegate) : base(eventDelegate)
            {
            }
        }

#endregion

#region properties

        /// <summary>無効扱いとなる移動量</summary>
        [HideInInspector]
        [SerializeField]
        protected float invalidMovement = 0.0f;

        /// <summary>無効扱いとなる移動量</summary>
        public float InvalidMovement { get { return invalidMovement; } set { invalidMovement = value; } }

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

            data.EventCamera = pressedPointer.pressEventCamera;
            data.EventTarget = transform as RectTransform;
            data.BeginPosition = GetPosition(pressedPointer);
            data.Position = data.BeginPosition;
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

            data.Position = GetPosition(releasedPointer);
            Vector2 movement = data.MovementFromBegin;
            if (movement.sqrMagnitude > invalidMovement * invalidMovement)
            {
                Direction direction = VectorDirectionUtils.GetDirection(movement);

                switch (direction)
                {
                    case Direction.Left:
                        Dispatch(SwipeGestureType.SwipeLeft);
                        break;
                    case Direction.Right:
                        Dispatch(SwipeGestureType.SwipeRight);
                        break;
                    case Direction.Up:
                        Dispatch(SwipeGestureType.SwipeUp);
                        break;
                    case Direction.Down:
                        Dispatch(SwipeGestureType.SwipeDown);
                        break;
                }
            }
        }

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <param name="callback">void Func(SwipeGestureEventData)の関数</param>
        /// <returns></returns>
        public override bool AddEvent(SwipeGestureType type, SwipeGestureDelegate.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("GestureUmpire.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(new SwipeGestureDelegate(callback));
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
        protected override SwipeGestureEventHandler CreateHandler(SwipeGestureType type)
        {
            return new SwipeGestureEventHandler(type);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            data = new SwipeGestureEventData();
        }

#endregion
    }
}
