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
using Jigbox.UIControl;

namespace Jigbox.Components
{
    [RequireComponent(typeof(BasicButton))]
    [RequireComponent(typeof(DragBehaviour))]
    public class Slider : Gauge
    {
#region inner classes, enum, and structs

        /// <summary>
        /// スライダーにおけるフィリング方法
        /// </summary>
        public enum SliderFillMethod
        {
            Horizontal = UnityEngine.UI.Image.FillMethod.Horizontal,
            Vertical = UnityEngine.UI.Image.FillMethod.Vertical,
        }

#endregion

#region properties

        /// <summary>Button</summary>
        [HideInInspector]
        [SerializeField]
        protected BasicButton button;

        /// <summary>ハンドル</summary>
        [HideInInspector]
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("Thumb")]
        protected RectTransform handle;

        /// <summary>柱状ゲージのView</summary>
        protected ColumnGaugeViewBase columnView;

#endregion

#region public methods
        
        /// <summary>
        /// Viewの状態を現在の値に合わせて更新します。
        /// </summary>
        public override void UpdateView()
        {
            base.UpdateView();
            UpdateHandle();
        }

        /// <summary>
        /// Viewの状態を再計算します。
        /// </summary>
        public void RecalculateView()
        {
            columnView.CalculatePoint(Value);
            UpdateHandle();
        }

#endregion

#region protected methods

        /// <summary>
        /// Viewを生成します。
        /// </summary>
        protected override void CreateView()
        {
            base.CreateView();
            columnView = view as ColumnGaugeViewBase;
        }

        /// <summary>
        /// ハンドルの状態を更新します。
        /// </summary>
        protected virtual void UpdateHandle()
        {
            if (handle != null && columnView != null)
            {
                handle.position = columnView.CurrentPoint;
            }
        }

        /// <summary>
        /// ドラッグ対象が見つかった際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        [AuthorizedAccess]
        protected virtual void OnInitDrag(PointerEventData eventData)
        {
            Value = GetValueFromEventData(eventData);
        }

        /// <summary>
        /// ドラッグが開始された際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        [AuthorizedAccess]
        protected virtual void OnBeginDrag(PointerEventData eventData)
        {
            Value = GetValueFromEventData(eventData);
        }

        /// <summary>
        /// ドラッグ中にポインタが移動した際に呼び出されます。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        [AuthorizedAccess]
        protected virtual void OnDrag(PointerEventData eventData)
        {
            Value = GetValueFromEventData(eventData);
        }

        /// <summary>
        /// 入力情報をゲージ上での値に変換して取得します。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        /// <returns>ゲージの値</returns>
        protected virtual float GetValueFromEventData(PointerEventData eventData)
        {
            float value = 0.0f;

            Vector3 inputPoint = PointerEventDataUtils.GetWorldPoint(eventData);
            Vector3 originPoint = columnView.OriginPoint;
            Vector3 terminatePoint = columnView.TerminatePoint;

            // 起点から入力点、起点から終点までの割合で値を計算
            if (model.FillMethod == (int) SliderFillMethod.Horizontal)
            {
                value = (inputPoint.x - originPoint.x) / (terminatePoint.x - originPoint.x);
            }
            else if (model.FillMethod == (int) SliderFillMethod.Vertical)
            {
                value = (inputPoint.y - originPoint.y) / (terminatePoint.y - originPoint.y);
            }
            
            return Mathf.Clamp01(value);
        }

        /// <summary>
        /// Buttonに各イベントでの動作を登録します。
        /// </summary>
        protected virtual void RegisterEvents()
        {
            // 引数を受け取るために型情報必要となるので、そのためのダミー変数
            PointerEventData dummy = null;

            button.AddEvent(InputEventType.OnInitDrag, this, "OnInitDrag", dummy);
            button.AddEvent(InputEventType.OnBeginDrag, this, "OnBeginDrag", dummy);
            button.AddEvent(InputEventType.OnDrag, this, "OnDrag", dummy);
        }

#endregion

#region override unity methods

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            if (button == null)
            {
                Debug.LogError("Slider.Awake : button component not found!", this);
                UnityEditor.EditorApplication.isPaused = true;
            }
#endif

            RegisterEvents();
        }

#endregion
    }
}
