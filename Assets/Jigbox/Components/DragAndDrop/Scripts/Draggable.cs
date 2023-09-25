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
using Jigbox.Delegatable;
using Jigbox.UIControl;

namespace Jigbox.Components
{
    using Direction = VectorDirectionUtils.Direction;

    [RequireComponent(typeof(DragBehaviour))]
    public class Draggable : UIBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>
        /// ドラッグの状態
        /// </summary>
        protected enum DragState
        {
            /// <summary>なし</summary>
            None,
            /// <summary>押下されてからドラッグとして判定されるまでの状態</summary>
            Standby,
            /// <summary>ドラッグ中</summary>
            Dragging,
            /// <summary>押下されたがドラッグとして許容されなかった際の状態</summary>
            Fumble,
        }

        /// <summary>
        /// ドラッグ終了時の位置の戻し方の種類
        /// </summary>
        public enum PositionRestoreType
        {
            /// <summary>戻さない</summary>
            None,
            /// <summary>常に戻す</summary>
            Always,
            /// <summary>ドラッグに失敗した際に戻す</summary>
            FailedDrop,
        }

        /// <summary>
        /// ドラッグ開始時にドラッグが許容される方向
        /// </summary>
        public enum BeginDragPermissiveDirection
        {
            /// <summary>全方向</summary>
            All,
            /// <summary>左</summary>
            Left,
            /// <summary>右</summary>
            Right,
            /// <summary>上</summary>
            Up,
            /// <summary>下</summary>
            Down,
            /// <summary>左右</summary>
            Vetical,
            /// <summary>上下</summary>
            Horizontal,
        }

        /// <summary>
        /// ドラッグの標準デリゲート型
        /// </summary>
        public class DraggableEventDelegate : EventDelegate<DraggableEventData>
        {
            public DraggableEventDelegate(DraggableEventDelegate.Callback eventDelegate)
                : base(eventDelegate)
            {
            }
        }

#endregion

#region properties

        /// <summary>有効かどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool isEnable = true;
        
        /// <summary>有効かどうか</summary>
        public bool IsEnable { get { return isEnable; } set { isEnable = value; } }

        /// <summary>排他制御のためのButtonコンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected ButtonBase button;

        /// <summary>ドラッグ中にオブジェクトを移動させるためのTransform空間</summary>
        [HideInInspector]
        [SerializeField]
        protected RectTransform dragArea;
        
        /// <summary>ドラッグ中にオブジェクトを移動させるためのTransform空間</summary>
        public RectTransform DragArea { get { return dragArea; } set { dragArea = value; } }

        /// <summary>ドラッグ終了時の位置の戻し方</summary>
        [HideInInspector]
        [SerializeField]
        protected PositionRestoreType restoreType = PositionRestoreType.FailedDrop;

        /// <summary>ドラッグ終了時の位置の戻し方</summary>
        public PositionRestoreType RestoreType { get { return restoreType; } set { restoreType = value; } }

        /// <summary>ドラッグが開始されるまでに誤差として扱われる移動量</summary>
        [HideInInspector]
        [SerializeField]
        protected float threshold = 0.0f;

        /// <summary>ドラッグが開始されるまでに誤差として扱われる移動量</summary>
        public float Threshold { get { return threshold; } set { threshold = value; } }

        /// <summary>ドラッグ開始時にドラッグ用コンポーネントのドラッグとして許容されるドラッグの方向</summary>
        [HideInInspector]
        [SerializeField]
        protected BeginDragPermissiveDirection permissiveDirection = BeginDragPermissiveDirection.All;

        /// <summary>ドラッグ開始時にドラッグ用コンポーネントのドラッグとして許容されるドラッグの方向</summary>
        public BeginDragPermissiveDirection PermissiveDirection { get { return permissiveDirection; } set { permissiveDirection = value; } }

        /// <summary>ドラッグ開始時のコールバック</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onBeginDrag = new DelegatableList();

        /// <summary>ドラッグ開始時のコールバック</summary>
        public DelegatableList OnBeginDragDelegates { get { return onBeginDrag; } }

        /// <summary>ドラッグ中にポインタが移動した際のコールバック</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onDrag = new DelegatableList();

        /// <summary>ドラッグ終了時のコールバック</summary>
        public DelegatableList OnDragDelegates { get { return onDrag; } }

        /// <summary>ドラッグ終了時のコールバック</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onEndDrag = new DelegatableList();

        /// <summary>ドラッグ終了時のコールバック</summary>
        public DelegatableList OnEndDragDelegates { get { return onEndDrag; } }

        /// <summary>ドラッグの状態</summary>
        protected DragState state = DragState.None;

        /// <summary>ドラッグ中にドラッグに追従させるオブジェクト</summary>
        protected Transform dragObject = null;

        /// <summary>ドラッグ開始直前の位置</summary>
        protected Vector3 basePosition = Vector3.zero;

        /// <summary>ドラッグ開始直前の親の参照</summary>
        protected Transform baseParent;

        /// <summary>ドラッグ開始直前のHierarchy上の同一階層内でのインデックス</summary>
        protected int baseSiblingIndex = 0;

        /// <summary>親オブジェクトでドラッグによる入力を受け付けることができるコンポーネントの参照</summary>
        protected IDragHandler parentDraggableObject = null;

        /// <summary>ドラッグのイベント情報</summary>
        protected DraggableEventData dragEventData = new DraggableEventData();

        /// <summary>ドラッグ中かどうか</summary>
        public bool IsDragging { get { return state == DragState.Dragging; } }

#endregion

#region public methods

        /// <summary>
        /// ドラッグ開始時のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(DraggableEventData)の関数</param>
        public void AddBeginDragEvent(DraggableEventDelegate.Callback callback)
        {
            onBeginDrag.Add(new DraggableEventDelegate(callback));
        }

        /// <summary>
        /// ドラッグ中のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(DraggableEventData)の関数</param>
        public void AddDragEvent(DraggableEventDelegate.Callback callback)
        {
            onDrag.Add(new DraggableEventDelegate(callback));
        }

        /// <summary>
        /// ドラッグ終了時のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(DraggableEventData)の関数</param>
        public void AddEndDragEvent(DraggableEventDelegate.Callback callback)
        {
            onEndDrag.Add(new DraggableEventDelegate(callback));
        }

        /// <summary>
        /// ドロップ用オブジェクトの範囲内に入った際に呼び出されます。
        /// </summary>
        /// <param name="droppable"></param>
        public virtual void OnEnterDroppable(Droppable droppable)
        {
            dragEventData.Droppable = droppable;
        }

        /// <summary>
        /// ドロップ用オブジェクトの範囲内から出た際に呼び出されます。
        /// </summary>
        public virtual void OnExitDroppable()
        {
            dragEventData.Droppable = null;
        }

        /// <summary>
        /// ドロップに成功した際に呼び出されます。
        /// </summary>
        public virtual void OnSucceedDrop()
        {
            dragEventData.IsSucceedDrop = true;
        }

#endregion

#region protected methods

        /// <summary>
        /// ドラッグ対象が見つかった際に呼び出されます。(実質押下と同タイミング)
        /// </summary>
        /// <param name="eventData">入力イベントの情報</param>
        protected virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (isEnable)
            {
                basePosition = transform.position;

                dragEventData.Draggable = this;
                dragEventData.Droppable = null;
                dragEventData.EventData = eventData;
            }

            if (parentDraggableObject != null)
            {
                IInitializePotentialDragHandler handler = parentDraggableObject as IInitializePotentialDragHandler;
                if (handler != null)
                {
                    // OnInitializePotentialDragは、押下時の値の初期化などが主なので
                    // ドラッグ・アンド・ドロップの動作如何に関わらず、親にも通知する
                    handler.OnInitializePotentialDrag(eventData);
                }
            }
        }

        /// <summary>
        /// ドラッグが開始された際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力イベントの情報</param>
        protected virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (isEnable)
            {
                state = DragState.Standby;
                dragEventData.EventData = eventData;

                TryBeginDrag(eventData);
            }
            else
            {
                state = DragState.Fumble;
                if (parentDraggableObject != null)
                {
                    IBeginDragHandler handler = parentDraggableObject as IBeginDragHandler;
                    if (handler != null)
                    {
                        handler.OnBeginDrag(eventData);
                    }
                }
            }
        }

        /// <summary>
        /// ドラッグ中にポインタが移動した際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力イベントの情報</param>
        protected virtual void OnDrag(PointerEventData eventData)
        {
            dragEventData.EventData = eventData;

            switch (state)
            {
                case DragState.Standby:
                    TryBeginDrag(eventData);
                    break;
                case DragState.Dragging:
                    UpdateDrag();
                    break;
                case DragState.Fumble:
                    if (parentDraggableObject != null)
                    {
                        parentDraggableObject.OnDrag(eventData);
                    }
                    break;
            }
        }

        /// <summary>
        /// ドラッグが終了した際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力イベントの情報</param>
        protected virtual void OnEndDrag(PointerEventData eventData)
        {
            dragEventData.EventData = eventData;

            if (state == DragState.Dragging)
            {
                EndDrag();
            }
            if (state == DragState.Fumble)
            {
                IEndDragHandler handler = parentDraggableObject as IEndDragHandler;
                if (handler != null)
                {
                    handler.OnEndDrag(eventData);
                }
            }
        }

        /// <summary>
        /// <para>ドラッグ用コンポーネントとしてドラッグが開始できるかを試し、開始することに失敗した場合、</para>
        /// <para>親のドラッグ可能オブジェクトのイベントを発火させます。</para>
        /// </summary>
        /// <param name="eventData">入力イベントの情報</param>
        protected virtual void TryBeginDrag(PointerEventData eventData)
        {
            Vector2 moveVector = eventData.position - eventData.pressPosition;

            if (moveVector.sqrMagnitude < threshold * threshold)
            {
                return;
            }

            Direction direction = GetDirection(moveVector);
            bool isValid;

            switch (permissiveDirection)
            {
                case BeginDragPermissiveDirection.Left:
                    isValid = direction == Direction.Left;
                    break;
                case BeginDragPermissiveDirection.Right:
                    isValid = direction == Direction.Right;
                    break;
                case BeginDragPermissiveDirection.Up:
                    isValid = direction == Direction.Up;
                    break;
                case BeginDragPermissiveDirection.Down:
                    isValid = direction == Direction.Down;
                    break;
                case BeginDragPermissiveDirection.Vetical:
                    isValid = direction == Direction.Up || direction == Direction.Down;
                    break;
                case BeginDragPermissiveDirection.Horizontal:
                    isValid = direction == Direction.Left || direction == Direction.Right;
                    break;
                case BeginDragPermissiveDirection.All:
                default:
                    isValid = true;
                    break;
            }

            if (isValid)
            {
                BeginDrag();
            }
            else
            {
                state = DragState.Fumble;

                if (parentDraggableObject != null)
                {
                    IBeginDragHandler handler = parentDraggableObject as IBeginDragHandler;
                    if (handler != null)
                    {
                        handler.OnBeginDrag(eventData);
                    }
                }
            }
        }

        /// <summary>
        /// ドラッグ用コンポーネントとして、ドラッグを開始します。
        /// </summary>
        protected virtual void BeginDrag()
        {
            state = DragState.Dragging;
            button.RaycastArea.enabled = false;

            GetDragObject();

            if (dragObject == transform)
            {
                baseParent = transform.parent;
                baseSiblingIndex = transform.GetSiblingIndex();
            }

            if (dragArea != null)
            {
                dragObject.SetParent(dragArea, false);
            }

            dragObject.position = PointerEventDataUtils.GetWorldPoint(dragEventData.EventData);

            if (onBeginDrag.Count > 0)
            {
                onBeginDrag.Execute(dragEventData);
            }
        }

        /// <summary>
        /// ドラッグ状態を更新します。
        /// </summary>
        protected virtual void UpdateDrag()
        {
            dragObject.position = PointerEventDataUtils.GetWorldPoint(dragEventData.EventData);

            if (onDrag.Count > 0)
            {
                onDrag.Execute(dragEventData);
            }
        }

        /// <summary>
        /// ドラッグ用コンポーネントとして、ドラッグを終了します。
        /// </summary>
        protected virtual void EndDrag()
        {
            if (onEndDrag.Count > 0)
            {
                onEndDrag.Execute(dragEventData);
            }

            if (dragObject == transform)
            {
                if (dragArea != null)
                {
                    dragObject.SetParent(baseParent, false);
                    dragObject.SetSiblingIndex(baseSiblingIndex);
                }
            }

            switch (restoreType)
            {
                case PositionRestoreType.Always:
                    dragObject.position = basePosition;
                    break;
                case PositionRestoreType.FailedDrop:
                    if (dragEventData.Droppable == null)
                    {
                        dragObject.position = basePosition;
                    }
                    break;
                case PositionRestoreType.None:
                default:
                    break;
            }

            ReleaseDragObject();

            button.RaycastArea.enabled = true;
            state = DragState.None;

            dragEventData.Droppable = null;
            dragEventData.IsSucceedDrop = false;
        }

        /// <summary>
        /// 引数に指定されたベクトルの向きを取得します。
        /// </summary>
        /// <param name="vector">ベクトル</param>
        /// <returns></returns>
        protected virtual Direction GetDirection(Vector2 vector)
        {
            // デフォルトでは、上下左右を90度ずつとして判定
            return VectorDirectionUtils.GetDirection(vector);
        }

        /// <summary>
        /// ドラッグ時にドラッグ位置に追従させるオブジェクトを取得します。
        /// </summary>
        protected virtual void GetDragObject()
        {
            dragObject = transform;
        }

        /// <summary>
        /// ドラッグ時にドラッグ位置に追従させるオブジェクトを解放します。
        /// </summary>
        protected virtual void ReleaseDragObject()
        {
            dragObject = null;
        }

#endregion

#region override unity methods

        protected override void OnTransformParentChanged()
        {
            parentDraggableObject = null;
            if (transform.parent != null)
            {
                parentDraggableObject = transform.parent.GetComponentInParent<IDragHandler>();
            }
        }

        protected override void Awake()
        {
            if (transform.parent != null)
            {
                parentDraggableObject = transform.parent.GetComponentInParent<IDragHandler>();
            }

            button.AddEvent(InputEventType.OnInitDrag, OnInitializePotentialDrag);
            button.AddEvent(InputEventType.OnBeginDrag, OnBeginDrag);
            button.AddEvent(InputEventType.OnDrag, OnDrag);
            button.AddEvent(InputEventType.OnEndDrag, OnEndDrag);
        }

        protected virtual void OnApplicationPause(bool pause)
        {
            // バックグラウンドから戻ってきたタイミングでドラッグ中の場合は
            // イベントの発火状況が通常と異なる可能性があるので、
            // ドラッグ状態が残っていれば、それを戻すように処理する

            if (!pause || !isEnable)
            {
                return;
            }

            if (state != DragState.Dragging)
            {
                return;
            }

            // バッググラウンドから戻ってきた際にドロップイベントが発火しないので
            // 仮にドロップ可能オブジェクトの上にあったとしても、ドロップ対象なしとして処理する
            OnExitDroppable();
            EndDrag();
        }

#endregion
    }
}
