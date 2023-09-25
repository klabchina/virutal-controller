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
    /// 特定方向への加速度の大きな移動によるジェスチャーを判別するクラス
    /// </summary>
    public class FlickGestureUmpire : SinglePointerGestureUmpire<FlickGestureType, FlickGestureEventData, FlickGestureEventHandler>
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 特定方向への加速度の大きな移動によるジェスチャーの標準デリゲート型
        /// </summary>
        public class FlickGestureDelegate : EventDelegate<FlickGestureEventData>
        {
            public FlickGestureDelegate(Callback eventDelegate) : base(eventDelegate)
            {
            }
        }

#endregion

#region constants

        /// <summary>移動していないという扱いになる移動量のデフォルト</summary>
        protected static readonly float DefaultInvalidMovement = 1.0f;

        /// <summary>無効扱いとなる加速度のデフォルト</summary>
        protected static readonly float DefaultInvalidAcceleration = 0.5f;

        /// <summary>フリックとみなされる時間のデフォルト</summary>
        protected static readonly float DefaultFlickPermissiveTime = 0.3f;

        /// <summary>加速度を割り出すための仮想フレームレート</summary>
        protected static int VirtualFrameRate = 60;
        
        /// <summary>補正値のデフォルト</summary>
        protected static readonly Vector2 DefaultCorrectionValue = Vector2.one;
        
        /// <summary>Smooth時の追尾補正値のデフォルト</summary>
        protected static readonly float DefaultSmoothPositionCorrection = 0.0f;

#endregion

#region properties

        /// <summary>移動していないという扱いになる移動量</summary>
        [HideInInspector]
        [SerializeField]
        protected float invalidMovement = DefaultInvalidMovement;

        /// <summary>移動していないという扱いになる移動量</summary>
        public float InvalidMovement { get { return invalidMovement; } set { invalidMovement = value; } }

        /// <summary>無効扱いとなる加速度</summary>
        [HideInInspector]
        [SerializeField]
        protected float invalidAcceleration = DefaultInvalidAcceleration;

        /// <summary>無効扱いとなる加速度</summary>
        public float InvalidAcceleration { get { return invalidAcceleration; } set { invalidAcceleration = value; } }

        /// <summary>フリックとみなされる時間</summary>
        [HideInInspector]
        [SerializeField]
        protected float flickPermissiveTime = DefaultFlickPermissiveTime;

        /// <summary>フリックとみなされる時間</summary>
        public float FlickPermissiveTime { get { return flickPermissiveTime; } set { flickPermissiveTime = value; } }

        /// <summary>ポインタが移動し始めた時間</summary>
        protected float moveBeginTime = 0.0f;

        /// <summary>補正値</summary>
        [HideInInspector]
        [SerializeField]
        protected Vector2 correctionValue = DefaultCorrectionValue;

        /// <summary>補正値</summary>
        public Vector2 CorrectionValue => correctionValue;

        /// <summary>Smooth時の追尾補正値</summary>
        [HideInInspector]
        [SerializeField]
        protected float smoothPositionCorrection = DefaultSmoothPositionCorrection;
        
        /// <summary>Smooth時の追尾補正値(数値が大きいほど現在のタッチ位置に近づく)</summary>
        public float SmoothPositionCorrection { get { return smoothPositionCorrection; } set { smoothPositionCorrection = value; } }

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

            moveBeginTime = 0.0f;

            data.EventCamera = pressedPointer.pressEventCamera;
            data.EventTarget = transform as RectTransform;
            data.BeginPosition = GetPosition(pressedPointer);
            data.Position = data.BeginPosition;
            data.SmoothPosition = data.BeginPosition;
            data.Acceleration = Vector2.zero;
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

            float deltaTime = Time.realtimeSinceStartup - moveBeginTime;

            if (moveBeginTime == 0.0f || deltaTime > flickPermissiveTime)
            {
                return;
            }

            data.Position = GetPosition(releasedPointer);
            Vector2 acceleration = (data.Position - data.SmoothPosition) / VirtualFrameRate * CorrectionValue;
            data.Acceleration = acceleration;
            if (acceleration.sqrMagnitude >= invalidAcceleration * invalidAcceleration)
            {
                Dispatch(FlickGestureType.Flick);

                Direction direction = VectorDirectionUtils.GetDirection(acceleration);

                switch (direction)
                {
                    case Direction.Left:
                        Dispatch(FlickGestureType.FlickLeft);
                        break;
                    case Direction.Right:
                        Dispatch(FlickGestureType.FlickRight);
                        break;
                    case Direction.Up:
                        Dispatch(FlickGestureType.FlickUp);
                        break;
                    case Direction.Down:
                        Dispatch(FlickGestureType.FlickDown);
                        break;
                }
            }
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
            data.SmoothPosition = Vector2.Lerp(data.SmoothPosition, position, Time.deltaTime * SmoothPositionCorrection);
            
            if (moveBeginTime > 0.0f)
            {
                return;
            }
            
            if (movement.sqrMagnitude < invalidMovement * invalidMovement)
            {
                return;
            }

            moveBeginTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <param name="callback">void Func(FlickGestureEventData)の関数</param>
        /// <returns></returns>
        public override bool AddEvent(FlickGestureType type, FlickGestureDelegate.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("GestureUmpire.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(new FlickGestureDelegate(callback));
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
        protected override FlickGestureEventHandler CreateHandler(FlickGestureType type)
        {
            return new FlickGestureEventHandler(type);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            data = new FlickGestureEventData();
        }

#endregion
}
}
