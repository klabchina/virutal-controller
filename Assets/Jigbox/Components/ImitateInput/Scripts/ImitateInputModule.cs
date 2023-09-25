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

namespace Jigbox.Components
{
    // 基本的にはuGUIのInputModule(StandaloneInputModule、TouchInputModule)に準拠した実装になっています。
    // 標準のInputModuleとの差異は、以下のようなものになります。
    //  - 通常のInputModuleのように、EventSystem.Updateで処理されず、自身Updateで動作する
    //  - PointerEventData.pointerIdが標準のものと重複しないように+100される
    //  - ImitateInputTargetがアタッチされていないオブジェクトは入力イベントの対象とならない
    //  - ISubmitHandlerなどの一部のイベントハンドラはサポートしていない

    /// <summary>
    /// 入力イベントの偽装用モジュール
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ImitateInputModule : PointerInputModule
    {
#region constants

        /// <summary>PointerEventDataのIDが従来のEventSystemと被らないようにずらす値</summary>
        static readonly int ShiftId = 100;

#endregion

#region public methods

        public override void Process()
        {
            ImitateInputTargetManager.Instance.ChangeValidate(true);

            if (InputWrapper.IsTouchSupported())
            {
                UpdateTouchEvents();
            }
            else
            {
                UpdateMouseEvents();
            }

            ImitateInputTargetManager.Instance.ChangeValidate(false);
        }

        public override bool IsModuleSupported()
        {
            // InputModuleではあるがuGUIのEventSystemの更新では動かさないため
            // モジュールとしてのサポート状態は常にfalseを返す
            return false;
        }

#endregion

#region private methods

        /// <summary>
        /// タッチ情報から入力イベントの判定を行います。
        /// </summary>
        void UpdateTouchEvents()
        {
            for (int i = 0; i < InputWrapper.GetTouchCount(); ++ i)
            {
                bool pressed;
                bool released;
                PointerEventData eventData = GetPointerEventData(i, out pressed, out released);

                ProcessPress(eventData, pressed, released);

                if (released)
                {
                    RemovePointerData(eventData);
                }
                else
                {
                    ProcessMove(eventData);
                    ProcessDrag(eventData);
                }
            }
        }

        /// <summary>
        /// マウスによる入力イベントの判定を行います。
        /// </summary>
        void UpdateMouseEvents()
        {
            bool pressed = InputWrapper.GetMouseButtonDown(0);
            bool released = InputWrapper.GetMouseButtonUp(0);

            if (!pressed && !released && !InputWrapper.GetMouseButton(0))
            {
                return;
            }

            PointerEventData eventData;
            bool hasCreated = GetPointerData(ShiftId, out eventData, true);

            eventData.Reset();

            Vector2 mousePosition = InputWrapper.GetMousePosition();
            
            Vector2 lastPosition = hasCreated ? mousePosition : eventData.position;

            eventData.delta = pressed ? Vector2.zero : mousePosition - lastPosition;
            eventData.position = mousePosition;
            eventData.button = PointerEventData.InputButton.Left;

            eventSystem.RaycastAll(eventData, m_RaycastResultCache);
            eventData.pointerCurrentRaycast = GetFirstRaycast();
            m_RaycastResultCache.Clear();

            ProcessPress(eventData, pressed, released);

            if (released)
            {
                RemovePointerData(eventData);
            }
            else
            {
                ProcessMove(eventData);
                ProcessDrag(eventData);
            }
        }

        /// <summary>
        /// タッチ情報からPointerEventDataを取得します。
        /// </summary>
        /// <param name="index">タッチ情報のインデックス</param>
        /// <param name="pressed">押下されたかどうか</param>
        /// <param name="released">離されたかどうか</param>
        /// <returns></returns>
        PointerEventData GetPointerEventData(int index, out bool pressed, out bool released)
        {
            PointerEventData eventData;
            bool hasCreated = GetPointerData(InputWrapper.GetTouchFingerId(index) + ShiftId, out eventData, true);

            eventData.Reset();

            pressed = hasCreated || InputWrapper.IsTouchPhase(index,TouchPhase.Began);
            released = InputWrapper.IsTouchPhase(index,TouchPhase.Canceled) || InputWrapper.IsTouchPhase(index,TouchPhase.Ended);

            Vector2 lastPosition = hasCreated ? InputWrapper.GetTouchPosition(index) : eventData.position;

            eventData.delta = pressed ? Vector2.zero : InputWrapper.GetTouchPosition(index) - lastPosition;
            eventData.position = InputWrapper.GetTouchPosition(index);
            eventData.button = PointerEventData.InputButton.Left;

            eventSystem.RaycastAll(eventData, m_RaycastResultCache);
            eventData.pointerCurrentRaycast = GetFirstRaycast();
            m_RaycastResultCache.Clear();

            return eventData;
        }

        /// <summary>
        /// 最初にRayが当たった対象を取得します。
        /// </summary>
        /// <returns></returns>
        RaycastResult GetFirstRaycast()
        {
            for (int i = 0; i < m_RaycastResultCache.Count; ++i)
            {
                GameObject targetObject = m_RaycastResultCache[i].gameObject;
                // Rayが当たっていて、ImitateInputTargetが付いているもののみ対象として扱う
                if (targetObject != null && ImitateInputTargetManager.Instance.IsValidTarget(targetObject))
                {
                    return m_RaycastResultCache[i];
                }
            }
            return new RaycastResult();
        }

        /// <summary>
        /// 画面を押下、離した際のイベントを処理します。
        /// </summary>
        /// <param name="eventData">入力イベント情報</param>
        /// <param name="pressed">押下されたかどうか</param>
        /// <param name="released">離されたかどうか</param>
        void ProcessPress(PointerEventData eventData, bool pressed, bool released)
        {
            // このメソッド内の処理は基本uGUIのInputModuleと同じ

            GameObject currentRaycastObject = eventData.pointerCurrentRaycast.gameObject;

            if (pressed)
            {
                eventData.eligibleForClick = true;
                eventData.delta = Vector2.zero;
                eventData.dragging = false;
                eventData.useDragThreshold = true;
                eventData.pressPosition = eventData.position;
                eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentRaycastObject, eventData);

                GameObject pressedObject = ExecuteEvents.ExecuteHierarchy(currentRaycastObject, eventData, ExecuteEvents.pointerDownHandler);

                if (pressedObject == null)
                {
                    pressedObject = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentRaycastObject);
                }

                float time = Time.unscaledTime;

                if (pressedObject == eventData.lastPress)
                {
                    float differenceTime = time - eventData.clickTime;
                    if (differenceTime < 0.3f)
                    {
                        ++eventData.clickCount;
                    }
                    else
                    {
                        eventData.clickCount = 1;
                    }
                }
                else
                {
                    eventData.clickCount = 1;
                }

                eventData.pointerPress = pressedObject;
                eventData.rawPointerPress = currentRaycastObject;

                eventData.clickTime = time;

                eventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentRaycastObject);

                if (eventData.pointerDrag != null)
                {
                    ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.initializePotentialDrag);
                }
            }

            if (released)
            {
                ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

                GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentRaycastObject);

                if (eventData.pointerPress == pointerUpHandler && eventData.eligibleForClick)
                {
                    ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerClickHandler);
                }
                else if (eventData.pointerDrag != null && eventData.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy(currentRaycastObject, eventData, ExecuteEvents.dropHandler);
                }

                eventData.eligibleForClick = false;
                eventData.pointerPress = null;
                eventData.rawPointerPress = null;

                if (eventData.pointerDrag != null && eventData.dragging)
                {
                    ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
                }

                eventData.dragging = false;
                eventData.pointerDrag = null;

                ExecuteEvents.ExecuteHierarchy(eventData.pointerEnter, eventData, ExecuteEvents.pointerExitHandler);
                eventData.pointerEnter = null;
            }
        }

#endregion

#region override unity methods

        void Update()
        {
            Process();
        }

#endregion
    }
}
