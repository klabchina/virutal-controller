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

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Jigbox.Gesture;
using Jigbox.VirtualPad;
using Jigbox.Delegatable;
using Jigbox.UIControl;

namespace Jigbox.Components
{
    /// <summary>
    /// 入力情報からバーチャルパッドの値の制御を行うクラス
    /// </summary>
    public class VirtualPadController : SinglePointerGestureUmpire<VirtualPadEventType, VirtualPadData, VirtualPadEventHandler>
    {
#region inner classes, enum, and structs

        /// <summary>
        /// バーチャルパッドの標準デリゲート型
        /// </summary>
        public class VirtualPadDelegate : EventDelegate<VirtualPadData>
        {
            public VirtualPadDelegate(Callback eventDelegate) : base(eventDelegate)
            {
            }
        }

#endregion

#region constants

        /// <summary>計算する座標系をスクリーン座標系で行うかどうか</summary>
        public override bool UseScreenPosition
        {
            get
            {
                return false;
            }
            set
            {
#if UNITY_EDITOR
                // バーチャルパッドについては座標系の切り替えは無効
                Debug.LogWarning("VirtualPadController.UseScreenPosition : Can't set value!");
#endif
            }
        }

        /// <summary>バーチャルパッドの中心からの移動率が0扱いになる範囲のデフォルト</summary>
        protected static readonly float DefaultDeadZone = 0.1f;

#endregion

#region properties

        /// <summary>バーチャルパッドの表示を構成するクラス</summary>
        [HideInInspector]
        [SerializeField]
        protected VirtualPadView view;

        /// <summary>バーチャルパッドの中心からの移動率が0扱いになる範囲(0～1)</summary>
        [HideInInspector]
        [SerializeField]
        protected float deadZone = DefaultDeadZone;

        /// <summary>バーチャルパッドの中心からの移動率が0扱いになる範囲(0～1)</summary>
        public float DeadZone { get { return deadZone; } set { deadZone = Mathf.Clamp01(value); } }

        /// <summary>横方向に動かないようにするかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool freezeHorizontal = false;

        /// <summary>横方向に動かないようにするかどうか</summary>
        public bool FreezeHorizontal { get { return freezeHorizontal; } set { freezeHorizontal = value; } }

        /// <summary>縦方向に動かないようにするかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool freezeVertical = false;

        /// <summary>縦方向に動かないようにするかどうか</summary>
        public bool FreezeVertical { get { return freezeVertical; } set { freezeVertical = value; } }

        /// <summary>バーチャルパッドの値の変更を受け取るコンポーネントの参照</summary>
        [HideInInspector]
        [SerializeField]
        protected List<VirtualPadUpdateReceiver> receivers = new List<VirtualPadUpdateReceiver>();

        /// <summary>バーチャルパッドの各軸上の移動率(-1～1)</summary>
        public Vector2 Axis { get { return data.Axis; } }

        /// <summary>拡張コンポーネントの参照</summary>
        [SerializeField]
        VirtualPadActivateExtensionBase virtualPadActivateExtension;

        /// <summary>拡張コンポーネントの参照</summary>
        public VirtualPadActivateExtensionBase VirtualPadActivateExtension
        {
            get { return virtualPadActivateExtension; }
            set
            {
                virtualPadActivateExtension = value;
                InitializeActivateExtension();
            }
        }

#endregion

#region fields

        /// <summary>
        /// GC回避のための、拡張コンポーネントに渡すActionのキャッシュ
        /// </summary>
        protected Action<PointerEventData> pressRequestCache = null;

#endregion

#region public methods

        /// <summary>
        /// ポインタが押下された際に呼び出されます。
        /// </summary>
        /// <param name="pressedPointer">押下されたポインタの情報</param>
        /// <param name="pressedPointers">押下されている全てのポインタ</param>
        public override void OnPress(PointerEventData pressedPointer, HashSet<PointerEventData> pressedPointers)
        {
            if (!ValidatePositionOnView(pressedPointer))
            {
                return;
            }
            if (!ValidatePress(pressedPointer))
            {
                return;
            }

            data.EventCamera = pressedPointer.pressEventCamera;
            data.EventTarget = transform as RectTransform;
            data.Axis = Vector2.zero;
            data.AxisDelta = Vector2.zero;
            data.AngleDelta = 0.0f;

            if (virtualPadActivateExtension != null)
            {
                virtualPadActivateExtension.Activate(pressedPointer);
                return;
            }

            data.BeginPosition = GetPosition(pressedPointer);
            data.Position = data.BeginPosition;

            data.IsEnabled = true;
            view.Activate(GetViewPosition(pressedPointer));
            Dispatch(VirtualPadEventType.OnActivate);
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

            ResetPerformData();

            view.Diactivate();

            Dispatch(VirtualPadEventType.OnDeactivate);
        }

        /// <summary>
        /// ポインタの状態からバーチャルパッドを更新します。
        /// </summary>
        /// <param name="pressedPointers">押下されている全てのポインタ</param>
        public override void OnUpdate(HashSet<PointerEventData> pressedPointers)
        {
            PointerEventData target = GetCurrentPointer(pressedPointers);

            if (target == null)
            {
                return;
            }

            if (virtualPadActivateExtension != null && !data.IsEnabled)
            {
                virtualPadActivateExtension.OnUpdate();
            }

            if (!data.IsEnabled)
            {
                return;
            }

            Vector2 lastPosition = data.Position;
            Vector2 lastDirection = data.Direction;
            Vector2 position = GetPosition(target);
            data.Position = position;
            Vector2 direction = data.Direction;

            if (position.x == lastPosition.x && position.y == lastPosition.y)
            {
                Dispatch(VirtualPadEventType.OnUpdate);
                return;
            }

            // 各軸の移動率は、移動方向のベクトルのスカラーとパッドの可動範囲から割り出す
            float scalar = (position - data.BeginPosition).magnitude;
            float rangeOfMotion = view.RangeOfMotion;
            float rate = Mathf.Clamp(scalar / rangeOfMotion - deadZone, 0.0f, 1.0f);

            // 各軸の移動率と表示上の移動状態は、DeadZone分のずれがあるので等値ではない
            float angle = data.Angle;
            float motionRate = Mathf.Clamp(scalar / rangeOfMotion, 0.0f, 1.0f);
            Vector2 handlePosition = Vector2.zero;

            Vector2 lastAxis = data.Axis;
            Vector2 axis = Vector2.zero;

            if (!freezeHorizontal)
            {
                handlePosition.x = rangeOfMotion * -Mathf.Sin(angle) * motionRate;
                axis.x = direction.x * rate;
            }
            if (!freezeVertical)
            {
                handlePosition.y = rangeOfMotion * Mathf.Cos(angle) * motionRate;
                axis.y = direction.y * rate;
            }
            data.Axis = axis;
            data.AxisDelta = axis - lastAxis;
            data.AngleDelta = VectorAngleUtils.GetAngle(lastDirection, direction);

            view.UpdateHandle(handlePosition);

            if (data.AxisDelta.x != 0.0f || data.AxisDelta.y != 0.0f)
            {
                Dispatch(VirtualPadEventType.OnAxisChanged);
            }

            Dispatch(VirtualPadEventType.OnUpdate);
        }

        /// <summary>
        /// 入力のロックが自動解除された際に呼び出されます。
        /// </summary>
        public override void OnAutoUnlock()
        {
            // ポインタが押下されていた場合は、強制的に終了
            if (currentPointerId != DisablePointeId)
            {
                ResetPerformData();

                Dispatch(VirtualPadEventType.OnDeactivate);
                view.Diactivate();
            }

            base.OnAutoUnlock();
        }

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="type">バーチャルパッドのイベントの種類</param>
        /// <param name="callback">void Func(VirtualPadData)の関数</param>
        /// <returns></returns>
        public override bool AddEvent(VirtualPadEventType type, VirtualPadDelegate.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("GestureUmpire.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(new VirtualPadDelegate(callback));
            AddEvent(type, delegatable);

            return true;
        }

#endregion

#region protected methods

        /// <summary>
        /// ポインタがバーチャルパッドの表示上で有効範囲内に存在するかどうかを返します。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        /// <returns></returns>
        protected bool ValidatePositionOnView(PointerEventData eventData)
        {
            if (eventData.pressEventCamera != null)
            {
                return RectTransformUtility.RectangleContainsScreenPoint(
                    view.ValidRect,
                    eventData.position,
                    eventData.pressEventCamera);
            }
            else
            {
                Bounds bounds = RectTransformUtils.GetBounds(view.ValidRect);
                Vector2 position = eventData.position;
                return (position.x >= bounds.min.x && position.x <= bounds.max.x)
                    && (position.y >= bounds.min.y && position.y <= bounds.max.y);
            }
        }

        /// <summary>
        /// バーチャルパッドの表示位置を取得します。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        /// <returns></returns>
        protected Vector2 GetViewPosition(PointerEventData eventData)
        {
            RectTransform viewParent = view.RectTransform.parent as RectTransform;
            Vector2 localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewParent, eventData.position, data.EventCamera, out localPosition);
            return localPosition;
        }

        /// <summary>
        /// イベントハンドラを作成します
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <returns>作成されたイベントハンドラを返します。</returns>
        protected override VirtualPadEventHandler CreateHandler(VirtualPadEventType type)
        {
            return new VirtualPadEventHandler(type);
        }

        /// <summary>
        /// バーチャルパッドの動作情報をリセットします
        /// </summary>
        protected virtual void ResetPerformData()
        {
            if (virtualPadActivateExtension != null)
            {
                virtualPadActivateExtension.Deactivate();
                return;
            }

            data.IsEnabled = false;
        }

        /// <summary>
        /// Actionのキャッシュを行います
        /// </summary>
        protected virtual void SetPressRequestCache()
        {
            if (pressRequestCache != null)
            {
                return;
            }

            pressRequestCache = pointerEventData =>
            {
                data.BeginPosition = GetPosition(pointerEventData);
                data.Position = data.BeginPosition;
                view.Activate(GetViewPosition(pointerEventData));

                Dispatch(VirtualPadEventType.OnActivate);
            };
        }

        /// <summary>
        /// 拡張コンポーネントの初期化を行います
        /// </summary>
        protected virtual void InitializeActivateExtension()
        {
            if (VirtualPadActivateExtension == null)
            {
                return;
            }

            SetPressRequestCache();
            VirtualPadActivateExtension.Initialize(data, pressRequestCache);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            if (view == null)
            {
#if UNITY_EDITOR
                Debug.LogError("VirtualPadView not found!", gameObject);
#endif
                isEnable = false;
            }
            useScreenPosition = false;
            data = new VirtualPadData();

            InitializeActivateExtension();

            foreach (VirtualPadUpdateReceiver receiver in receivers)
            {
                if (receiver != null)
                {
                    AddEvent(VirtualPadEventType.OnActivate, receiver.OnActivate);
                    AddEvent(VirtualPadEventType.OnAxisChanged, receiver.OnAxisChanged);
                    AddEvent(VirtualPadEventType.OnUpdate, receiver.OnUpdate);
                    AddEvent(VirtualPadEventType.OnDeactivate, receiver.OnDeactivate);
                }
            }
        }

#endregion
    }
}
