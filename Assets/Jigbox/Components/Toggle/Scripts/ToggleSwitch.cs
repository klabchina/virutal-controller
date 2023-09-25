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
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BasicButton))]
    public class ToggleSwitch : BasicToggle
    {
#region constants

        /// <summary>フリックとして扱われる時間</summary>
        protected static readonly float FlickPermissionTime = 0.3f;

#endregion

#region properties

        /// <summary>状態切替時のトランジション制御コンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected ToggleSwitchTransitionBase transition;

        /// <summary>Button</summary>
        [HideInInspector]
        [SerializeField]
        protected BasicButton button;

        /// <summary>RectTransform</summary>
        protected RectTransform rectTransform;

        /// <summary>動作が有効かどうか</summary>
        protected virtual bool IsEnable { get { return transition != null; } }

        /// <summary>ドラッグされているかどうか</summary>
        protected bool isDragged = false;

        /// <summary>入力された座標でのトグルの状態</summary>
        protected bool inputPointStatus = false;

        /// <summary>ドラッグ開始位置</summary>
        protected Vector3 beginDragPoint = Vector3.zero;
        
        /// <summary>ドラッグ開始時間</summary>
        protected float beginDragTime = 0.0f;
        
#endregion
        
#region protected methods

        /// <summary>
        /// クリックされた際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        protected override void OnClick()
        {
            if (!IsEnable)
            {
                return;
            }

            // ドラッグ中だった場合、クリックイベントが発火しても無視
            if (isDragged)
            {
                return;
            }

            base.OnClick();
        }

        /// <summary>
        /// IsOnの値が更新され、OnValueChangedデリゲート呼び出し前に呼び出されます。
        /// </summary>
        protected override void OnUpdateIsOn()
        {
            if (!IsEnable)
            {
                return;
            }

            transition.Switch();
        }

        /// <summary>
        /// ドラッグが開始された際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        [AuthorizedAccess]
        protected virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsEnable)
            {
                return;
            }

            isDragged = true;
            // 現在の状態と逆の位置でドラッグを開始した場合、
            // 状態が切り替わるようなドラッグ動作でなくても状態が切り替わるので、
            // 状態をキャッシュして状態が切り替わったかどうかを判定するため
            inputPointStatus = InputPointToToggleStatus(eventData);

            beginDragPoint = eventData.position;
            beginDragPoint.z = 0.0f;
            beginDragTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// ドラッグ中にポインタが移動した際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        [AuthorizedAccess]
        protected virtual void OnDrag(PointerEventData eventData)
        {
            if (!IsEnable)
            {
                return;
            }
            
            bool status = InputPointToToggleStatus(eventData);
            if (status != inputPointStatus)
            {
                inputPointStatus = status;
                IsOn = status;
            }
        }

        /// <summary>
        /// ドラッグが終了した際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        [AuthorizedAccess]
        protected virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!IsEnable)
            {
                return;
            }

            isDragged = false;

            float deltaTime = Time.realtimeSinceStartup - beginDragTime;
            if (deltaTime > FlickPermissionTime)
            {
                return;
            }
            Vector3 position = eventData.position;
            position.z = 0.0f;
            Vector3 dragDirection = Vector3.Normalize(position - beginDragPoint);
            Vector3 toggleDirection = Vector3.Normalize(transition.PositionOn - transition.PositionOff);

            // ベクトルの内積によってベクトルの方向関係が分かるので、そこから判定する
            IsOn = Vector3.Dot(dragDirection, toggleDirection) >= 0;
        }

        /// <summary>
        /// 入力された座標からトグルでの状態に変換します。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        /// <returns></returns>
        protected virtual bool InputPointToToggleStatus(PointerEventData eventData)
        {
            Vector3 pointerPosition = UIControl.PointerEventDataUtils.GetWorldPoint(eventData);
            Vector3 positionOn = transition.PositionOnWorld;
            Vector3 positionOff = transition.PositionOffWorld;
            
            // ピタゴラスの定理によってベクトルの距離を比較する
            float distanceOn = Vector3.SqrMagnitude(pointerPosition - positionOn);
            float distanceOff = Vector3.SqrMagnitude(pointerPosition - positionOff);
            return distanceOn <= distanceOff;
        }

        /// <summary>
        /// Buttonに各イベントでの動作を登録します。
        /// </summary>
        protected virtual void RegisterEvents()
        {
            button.AddEvent(InputEventType.OnClick, OnClick);
            button.AddEvent(InputEventType.OnBeginDrag, OnBeginDrag);
            button.AddEvent(InputEventType.OnDrag, OnDrag);
            button.AddEvent(InputEventType.OnEndDrag, OnEndDrag);
        }

#endregion

#region override unity methods

        protected override void Awake()
        {
            base.Awake();

            rectTransform = transform as RectTransform;

            if (transition == null)
            {
                transition = GetComponent<ToggleSwitchTransitionBase>();
            }

            if (transition == null)
            {
#if UNITY_EDITOR
                Debug.LogError("ToggleSwitch.Awake : Not found transition!", this);
#endif
                return;
            }

            transition.Init(IsOn);
            
            RegisterEvents();
        }

#endregion
    }
}
