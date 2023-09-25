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
    [RequireComponent(typeof(RaycastArea))]
    public class Droppable : UIBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IDropHandler,
        ICanvasRaycastFilter
    {
#region inner classes, enum, and structs

        /// <summary>
        /// ドロップの標準デリゲート型
        /// </summary>
        public class DroppableEventDelegate : EventDelegate<DroppableEventData>
        {
            public DroppableEventDelegate(DroppableEventDelegate.Callback eventDelegate)
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

        /// <summary>判定領域内にポインタが入った際のコールバック</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onEnter = new DelegatableList();

        /// <summary>判定領域内にポインタが入った際のコールバック</summary>
        public DelegatableList OnEnterDelegates { get { return onEnter; } }

        /// <summary>判定領域内からポインタが出た際のコールバック</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onExit = new DelegatableList();

        /// <summary>判定領域内からポインタが出た際のコールバック</summary>
        public DelegatableList OnExitDelegates { get { return onExit; } }

        /// <summary>ドラッグ終了時に、ポインタ位置が判定領域内に存在している際のコールバック</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onDrop = new DelegatableList();

        /// <summary>ドラッグ終了時に、ポインタ位置が判定領域内に存在している際のコールバック</summary>
        public DelegatableList OnDropDelegates { get { return onDrop; } }

        /// <summary>ドロップのイベント情報</summary>
        protected DroppableEventData dropEventData = new DroppableEventData();

        /// <summary>RectTransform</summary>
        protected RectTransform rectTransform;

        /// <summary>RectTransform</summary>
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = transform as RectTransform;
                }
                return rectTransform;
            }
        }

        /// <summary>実描画領域の左下、右上の座標</summary>
        protected Bounds cachedBounds;

        /// <summary>Transform.positionのキャッシュ</summary>
        protected Vector3 cachedPosition = Vector3.zero;

        /// <summary>Transform.lossyScaleのキャッシュ</summary>
        protected Vector3 cachedScale = Vector3.one;

        /// <summary>RectTransformに変化があったかどうか</summary>
        protected bool rectTransformChanged = false;

#endregion

#region public methods

        /// <summary>
        /// 判定領域内にポインタが入った際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(DroppableEventData)の関数</param>
        public void AddEnterEvent(DroppableEventDelegate.Callback callback)
        {
            onEnter.Add(new DroppableEventDelegate(callback));
        }

        /// <summary>
        /// 判定領域内からポインタが出た際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(DroppableEventData)の関数</param>
        public void AddExitEvent(DroppableEventDelegate.Callback callback)
        {
            onExit.Add(new DroppableEventDelegate(callback));
        }

        /// <summary>
        /// ドラッグ終了時に、ポインタ位置が判定領域内に存在している際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(DroppableEventData)の関数</param>
        public void AddDropEvent(DroppableEventDelegate.Callback callback)
        {
            onDrop.Add(new DroppableEventDelegate(callback));
        }

        /// <summary>
        /// 判定領域内にポインタが入った際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力イベントの情報</param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!isEnable)
            {
                return;
            }

            if (eventData.pointerPress == null)
            {
                return;
            }

            // ドラッグされた対象がDraggableを持っていない、
            // もしくは、持っているが有効でない場合やドラッグ中でない場合は無効
            Draggable draggable = eventData.pointerPress.GetComponent<Draggable>();
            if (draggable == null)
            {
                return;
            }
            if (!draggable.IsEnable || !draggable.IsDragging)
            {
                return;
            }

            dropEventData.DraggableInRectangle = true;
            dropEventData.Draggable = draggable;
            dropEventData.Droppable = this;
            dropEventData.Draggable.OnEnterDroppable(this);
            dropEventData.EventData = eventData;

            if (onEnter.Count > 0)
            {
                onEnter.Execute(dropEventData);
            }
        }

        /// <summary>
        /// 判定領域内からポインタが出た際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力イベントの情報</param>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!isEnable || !dropEventData.DraggableInRectangle)
            {
                return;
            }

            dropEventData.DraggableInRectangle = false;
            dropEventData.Draggable.OnExitDroppable();
            dropEventData.Draggable = null;
            dropEventData.EventData = eventData;

            if (onExit.Count > 0)
            {
                onExit.Execute(dropEventData);
            }
        }

        /// <summary>
        /// ドラッグ終了時に、ポインタ位置が判定領域内に存在している際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力イベントの情報</param>
        public virtual void OnDrop(PointerEventData eventData)
        {
            if (!isEnable || !dropEventData.DraggableInRectangle)
            {
                return;
            }

            if (dropEventData.Draggable.gameObject != eventData.pointerPress)
            {
                return;
            }

            dropEventData.Draggable.OnSucceedDrop();
            dropEventData.EventData = eventData;

            if (onDrop.Count > 0)
            {
                onDrop.Execute(dropEventData);
            }

            dropEventData.DraggableInRectangle = false;
        }

        /// <summary>
        /// レイキャストが有効かどうかを返します。
        /// </summary>
        /// <param name="screenPoint">入力の画面上における座標</param>
        /// <param name="eventCamera">カメラ</param>
        /// <returns></returns>
        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            // 矩形範囲外の入力を受けないようにするために定義
            // (uGUI的にGraphicが子にあると当たり判定発生するため)

            if (!isActiveAndEnabled || !isEnable)
            {
                return true;
            }

            // 状態が変わった場合のみ再計算
            if (rectTransformChanged
                || RectTransform.position != cachedPosition
                || RectTransform.lossyScale != cachedScale)
            {
                cachedBounds = RectTransformUtils.GetBounds(RectTransform);
                cachedPosition = RectTransform.position;
                cachedScale = RectTransform.lossyScale;
                rectTransformChanged = false;
            }

            Vector3 world = screenPoint;
            if (eventCamera != null)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, screenPoint, eventCamera, out world);
            }
            if (world.x >= cachedBounds.min.x
                && world.x <= cachedBounds.max.x
                && world.y >= cachedBounds.min.y
                && world.y <= cachedBounds.max.y)
            {
                return true;
            }

            return false;
        }

#endregion

#region override unity methods

        protected override void OnRectTransformDimensionsChange()
        {
            // サイズやpivotの変化はここでキャッチする
            base.OnRectTransformDimensionsChange();
            rectTransformChanged = true;
        }

        protected virtual void OnApplicationPause(bool pause)
        {
            // バックグラウンドから戻ってきたタイミングでドラッグ中の場合は
            // イベントの発火状況が通常と異なる可能性があるので、
            // ドラッグ状態が残っていれば、それを戻すように処理する

            if (!pause || !isEnable || !dropEventData.DraggableInRectangle)
            {
                return;
            }

            dropEventData.DraggableInRectangle = false;
            dropEventData.Draggable = null;
            if (onExit.Count > 0)
            {
                onExit.Execute(dropEventData);
            }
        }

#endregion
    }
}
