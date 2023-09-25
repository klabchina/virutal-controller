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
    /// 特定位置への入力によるジェスチャーを判別するクラス
    /// </summary>
    public class PointingGestureUmpire : SinglePointerGestureUmpire<PointingGestureType, PointingGestureEventData, PointingGestureEventHandler>
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 特定位置への入力によるジェスチャーの標準デリゲート型
        /// </summary>
        public class PointingGestureDelegate : EventDelegate<PointingGestureEventData>
        {
            public PointingGestureDelegate(Callback eventDelegate) : base(eventDelegate)
            {
            }
        }

#endregion

#region constants

        /// <summary>長押しと判断する時間のデフォルト</summary>
        protected static readonly float DefaultLongPressTime = 0.35f;

        /// <summary>ダブルクリックとしてみなされる時間のデフォルト</summary>
        protected static readonly float DefaultDoubleClickPermissiveTime = 0.8f;

        /// <summary>ポインタが動いていないと判断する際の誤差範囲のデフォルト</summary>
        protected static readonly float DefaultTolerance = 1.0f;

        /// <summary>ダブルクリックと判断する際の誤差範囲のデフォルト</summary>
        protected static readonly float DefaultDoubleClickTolerance = 20.0f;

#endregion

#region properties

        /// <summary>長押しと判断する時間</summary>
        [HideInInspector]
        [SerializeField]
        protected float longPressTime = DefaultLongPressTime;

        /// <summary>長押しと判断する時間</summary>
        public float LongPressTime { get { return longPressTime; } set { longPressTime = value; } }

        /// <summary>ダブルクリックとしてみなされる時間</summary>
        [HideInInspector]
        [SerializeField]
        protected float doubleClickPermissiveTime = DefaultDoubleClickPermissiveTime;

        /// <summary>ダブルクリックとしてみなされる時間</summary>
        public float DounbleClickPermissiveTime { get { return doubleClickPermissiveTime; } set { doubleClickPermissiveTime = value; } }

        /// <summary>ポインタが動いていないと判断する際の誤差範囲</summary>
        [HideInInspector]
        [SerializeField]
        protected float tolerance = DefaultTolerance;

        /// <summary>ポインタが動いていないと判断する際の誤差範囲</summary>
        public float Tolerance { get { return tolerance; } set { tolerance = value; } }

        /// <summary>ダブルクリックと判断する際の誤差範囲</summary>
        [HideInInspector]
        [SerializeField]
        protected float doubleClickTolerance = DefaultDoubleClickTolerance;

        /// <summary>ダブルクリックと判断する際の誤差範囲</summary>
        public float DoubleClickTolerance { get { return doubleClickTolerance; } set { doubleClickTolerance = value; } }

        /// <summary>押下し始めた時間</summary>
        protected float pressBeginTime = 0.0f;

        /// <summary>ダブルクリックの判定を始めた時間</summary>
        protected List<float> doubleClickBeginTimes = new List<float>();

        /// <summary>ダブルクリックの判定を初めた位置</summary>
        protected List<Vector2> doubleClickBeginPositions = new List<Vector2>();

        /// <summary>長押しが発生する時間が経過したかどうか</summary>
        protected bool elapsedLongPressTime = false;

        /// <summary>誤差範囲外にポインタが移動したかどうか</summary>
        protected bool movedOutOfTolerance = false;

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

            pressBeginTime = Time.realtimeSinceStartup;
            doubleClickBeginTimes.Insert(0, pressBeginTime);

            elapsedLongPressTime = false;
            movedOutOfTolerance = false;

            data.EventCamera = pressedPointer.pressEventCamera;
            data.EventTarget = transform as RectTransform;
            data.Position = GetPosition(pressedPointer);
            doubleClickBeginPositions.Insert(0, data.Position);

            Dispatch(PointingGestureType.Press);
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

            Vector2 lastPosition = data.Position;
            data.Position = GetPosition(releasedPointer);

            Dispatch(PointingGestureType.Release);

            // 押下されてから指が離されるまでの座標の変化が誤差範囲以下の場合はクリックとして扱う
            if (!movedOutOfTolerance && IsValidPosition(lastPosition, data.Position, tolerance))
            {
                Dispatch(PointingGestureType.Click);

                if (doubleClickBeginTimes.Count >= 2)
                {
                    // データ上、最新のPress時の情報が先頭に格納されるので、
                    // 1回前のデータは必ず要素番号1に格納されている前提で処理
                    float doubleClickDeltaTime = Time.realtimeSinceStartup - doubleClickBeginTimes[1];
                    if (doubleClickDeltaTime <= doubleClickPermissiveTime &&
                        IsValidPosition(doubleClickBeginPositions[1], data.Position, doubleClickTolerance))
                    {
                        Dispatch(PointingGestureType.DoubleClick);
                        doubleClickBeginTimes.Clear();
                        doubleClickBeginPositions.Clear();
                    }
                }
            }
            // クリックにならない判定の場合は、ダブルクリックも発生しないのでキューをクリア
            else
            {
                doubleClickBeginTimes.Clear();
                doubleClickBeginPositions.Clear();
            }

            if (doubleClickBeginTimes.Count >= 2)
            {
                doubleClickBeginTimes.RemoveAt(doubleClickBeginTimes.Count - 1);
            }
            if (doubleClickBeginPositions.Count >= 2)
            {
                doubleClickBeginPositions.RemoveAt(doubleClickBeginPositions.Count - 1);
            }
        }

        /// <summary>
        /// ポインタの状態からジェスチャーの状態を判別して、更新します。
        /// </summary>
        /// <param name="pressedPointers">押下されている全てのポインタ</param>
        public override void OnUpdate(HashSet<PointerEventData> pressedPointers)
        {
            if (elapsedLongPressTime && movedOutOfTolerance)
            {
                return;
            }

            PointerEventData target = GetCurrentPointer(pressedPointers);

            if (target == null)
            {
                return;
            }

            Vector2 position = GetPosition(target);
            bool isValid = IsValidPosition(data.Position, position, tolerance);

            if (!movedOutOfTolerance)
            {
                if (!isValid)
                {
                    movedOutOfTolerance = true;
                }
            }

            if (!elapsedLongPressTime && Time.realtimeSinceStartup - pressBeginTime >= longPressTime)
            {
                if (!movedOutOfTolerance && isValid)
                {
                    Dispatch(PointingGestureType.LongPress);
                }
                elapsedLongPressTime = true;
            }
        }

        /// <summary>
        /// 入力のロックが自動解除された際に呼び出されます。
        /// </summary>
        public override void OnAutoUnlock()
        {
            // ポインタが押下されていた場合は、強制的に離したものとして扱う
            if (currentPointerId != DisablePointeId)
            {
                Dispatch(PointingGestureType.Release);
            }

            base.OnAutoUnlock();

            pressBeginTime = 0.0f;
            doubleClickBeginTimes.Clear();
            doubleClickBeginPositions.Clear();
            elapsedLongPressTime = false;
        }

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <param name="callback">void Func(PointingGestureEventData)の関数</param>
        /// <returns></returns>
        public override bool AddEvent(PointingGestureType type, PointingGestureDelegate.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("GestureUmpire.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(new PointingGestureDelegate(callback));
            AddEvent(type, delegatable);

            return true;
        }

#endregion

#region protected methods

        /// <summary>
        /// 座標のズレが誤差範囲に収まっているかどうかを返します。
        /// </summary>
        /// <param name="p1">座標1</param>
        /// <param name="p2">座標2</param>
        /// <param name="tolerance">許容される誤差</param>
        /// <returns>ズレが許容範囲内なら<c>true</c>、範囲外なら<c>false</c>を返します。</returns>
        protected bool IsValidPosition(Vector2 p1, Vector2 p2, float tolerance)
        {
            Vector2 vector = p1 - p2;
            return vector.sqrMagnitude <= tolerance * tolerance;
        }

        /// <summary>
        /// イベントハンドラを作成します
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <returns>作成されたイベントハンドラを返します。</returns>
        protected override PointingGestureEventHandler CreateHandler(PointingGestureType type)
        {
            return new PointingGestureEventHandler(type);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            data = new PointingGestureEventData();
        }

#endregion
    }
}
