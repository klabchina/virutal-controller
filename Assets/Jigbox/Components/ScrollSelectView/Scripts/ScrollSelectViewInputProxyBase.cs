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
    public class ScrollSelectViewInputProxyBase : MonoBehaviour,
        IInitializePotentialDragHandler, 
        IBeginDragHandler, 
        IDragHandler, 
        IEndDragHandler
    {
#region constants

        protected static readonly int InvalidPointerId = -999;
        
#endregion
        
#region propeties

        /// <summary>ScrollSelectView</summary>
        protected ScrollSelectViewBase ScrollSelectView;

        /// <summary>入力状態となっているポインタのID</summary>
        protected int PressedPointerId = InvalidPointerId;

        /// <summary>前回のpointerId</summary>
        protected int PrevPressedPointerId = InvalidPointerId;
        
        /// <summary>ドラッグしているかどうか</summary>
        public bool IsDragging { get; protected set; }
        
        /// <summary>PointerIdがDragを受け付けない値かどうか</summary>
        protected bool IsValidPointerId
        {
            get { return PressedPointerId != InvalidPointerId; }
        }

#endregion
        
#region public methods
        
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="scrollSelectView">ScrollSelectView</param>
        public virtual void Init(ScrollSelectViewBase scrollSelectView)
        {
            ScrollSelectView = scrollSelectView;
        }

        /// <summary>
        /// ドラッグ対象が見つかった際に呼び出されます。(実質押下と同タイミング)
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (IsValidPointerId)
            {
                return;
            }
            
            // ドラッグ始めもContentのスライドをさせる
            ScrollSelectView.StopAdjustTransition();
            ScrollSelectView.SlideContent();
            
            SetNewPointerId(InputWrapper.GetEventDataTouchId(eventData));
        }

        /// <summary>
        /// ドラッグが開始された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (PressedPointerId != InputWrapper.GetEventDataTouchId(eventData))
            {
                return;
            }

            IsDragging = true;
        }

        /// <summary>
        /// ドラッグ中にポインタが移動した際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            // 非アクティブからアクティブに戻った際に、指を離さなかった場合は位置を補正するため
            // OnDragが呼ばれるようにする必要がある。
            // そのため、一時的にキャッシュしておいた前回のpointerIdを確認してDrag可能な状態に戻す
            if (!IsDragging && InputWrapper.GetEventDataTouchId(eventData) == PrevPressedPointerId)
            {
                IsDragging = true;
                SetNewPointerId(InputWrapper.GetEventDataTouchId(eventData));
            }
        }

        /// <summary>
        /// ドラッグが終了した際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (PressedPointerId != InputWrapper.GetEventDataTouchId(eventData))
            {
                return;
            }

            // ドラッグの終わりでもContentのスライドをさせる
            ScrollSelectView.SlideContent();
            IsDragging = false;
            SetNewPointerId(InvalidPointerId);
        }

        /// <summary>
        /// Proxyの状態をDragしていない状態にリフレッシュします。
        /// </summary>
        public virtual void Refresh()
        {
            IsDragging = false;
            SetNewPointerId(InvalidPointerId);
        }
        
#endregion
        
#region protected methods

        /// <summary>
        /// PointerIdを更新します。
        /// </summary>
        /// <param name="newId">次のPointerId</param>
        protected virtual void SetNewPointerId(int newId)
        {
            PrevPressedPointerId = PressedPointerId;
            PressedPointerId = newId;
        }

        /// <summary>
        /// LateUpdateで監視しているPress判定が終了した際に呼ばれます。
        /// </summary>
        protected virtual void OnPressReleased()
        {
            SetNewPointerId(InvalidPointerId);
        }
        
#endregion
        
#region override unity methods

        protected virtual void LateUpdate()
        {
            // 指を離した際に少しでも動いていれば、OnEndDragの方が先に呼び出されるので、
            // このタイミングでクリック判定が発生するのは、ポインタが動いていない場合のみとなる
            if (!IsValidPointerId)
            {
                return;
            }

            bool isPressing = false;

#if UNITY_EDITOR || UNITY_STANDALONE
            isPressing = InputWrapper.GetMouseButton(0);
#else
            for (int i = 0; i < InputWrapper.GetTouchCount(); i++)
            {
                isPressing |= InputWrapper.GetTouchFingerId(i) == this.PressedPointerId;
            }
#endif

            if (!isPressing)
            {
                OnPressReleased();
            }
        }
        
#endregion
    }
}
