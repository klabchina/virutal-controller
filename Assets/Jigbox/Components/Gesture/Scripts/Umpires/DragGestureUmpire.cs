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
    /// ポインタの移動によるジェスチャーを判別するクラス
    /// </summary>
    public class DragGestureUmpire : SinglePointerGestureUmpire<DragGestureType, DragGestureEventData, DragGestureEventHandler>
    {
#region inner classes, enum, and structs

        /// <summary>
        /// ポインタの移動によるジェスチャーの標準デリゲート型
        /// </summary>
        public class DragGestureDelegate : EventDelegate<DragGestureEventData>
        {
            public DragGestureDelegate(Callback eventDelegate) : base(eventDelegate)
            {
            }
        }

#endregion

#region properties

        /// <summary>ポインタが押下後、移動したかどうか</summary>
        protected bool isMoved = false;

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

            isMoved = false;

            data.EventCamera = pressedPointer.pressEventCamera;
            data.EventTarget = transform as RectTransform;
            data.BeginPosition = GetPosition(pressedPointer);
            data.Position = data.BeginPosition;
            data.Movement = Vector2.zero;
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

            if (isMoved)
            {
                Dispatch(DragGestureType.EndDrag);
            }
            isMoved = false;
        }

        /// <summary>
        /// ポインタの状態からジェスチャーの状態を判別して、更新します。
        /// </summary>
        /// <param name="pressedPointers">押下されている全てのポインタ</param>
        public override void OnUpdate(HashSet<PointerEventData> pressedPointers)
        {
            PointerEventData target = GetCurrentPointer(pressedPointers);

            if (target == null)
            {
                return;
            }

            Vector2 position = GetPosition(target);
            Vector2 movement = position - data.Position;
            if (movement.x == 0.0f && movement.y == 0.0f)
            {
                return;
            }

            data.Position = position;
            data.Movement = movement;
            Direction direction = VectorDirectionUtils.GetDirection(movement);

            if (!isMoved)
            {
                Dispatch(DragGestureType.BeginDrag);
                isMoved = true;
            }

            Dispatch(DragGestureType.Drag);

            switch (direction)
            {
                case Direction.Left:
                    Dispatch(DragGestureType.DragLeft);
                    break;
                case Direction.Right:
                    Dispatch(DragGestureType.DragRight);
                    break;
                case Direction.Up:
                    Dispatch(DragGestureType.DragUp);
                    break;
                case Direction.Down:
                    Dispatch(DragGestureType.DragDown);
                    break;
            }
        }

        /// <summary>
        /// 入力のロックが自動解除された際に呼び出されます。
        /// </summary>
        public override void OnAutoUnlock()
        {
            // ポインタが押下されていた場合は、強制的に離したものとして扱う
            if (currentPointerId != DisablePointeId && isMoved)
            {
                Dispatch(DragGestureType.EndDrag);
            }

            base.OnAutoUnlock();

            isMoved = false;
        }

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <param name="callback">void Func(DragGestureEventData)の関数</param>
        /// <returns></returns>
        public override bool AddEvent(DragGestureType type, DragGestureDelegate.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("GestureUmpire.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(new DragGestureDelegate(callback));
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
        protected override DragGestureEventHandler CreateHandler(DragGestureType type)
        {
            return new DragGestureEventHandler(type);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            data = new DragGestureEventData();
        }

#endregion
    }
}
