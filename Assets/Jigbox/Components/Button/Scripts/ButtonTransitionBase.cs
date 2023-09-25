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

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public abstract class ButtonTransitionBase : MonoBehaviour, IButtonExtendComponent
    {
#region public methods

        /// <summary>
        /// ボタンのイベントの通知を受け取ります。
        /// </summary>
        /// <param name="type">発火したイベントの種類</param>
        public void NoticeEvent(InputEventType type)
        {
            switch (type)
            {
                case InputEventType.OnPress:
                    OnPress();
                    break;
                case InputEventType.OnRelease:
                    OnRelease();
                    break;
                case InputEventType.OnClick:
                    OnClick();
                    break;
                case InputEventType.OnLongPress:
                    OnLongPress();
                    break;
                case InputEventType.OnKeyRepeat:
                    OnKeyRepeat();
                    break;
                case InputEventType.OnInitDrag:
                    OnInitDrag();
                    break;
                case InputEventType.OnBeginDrag:
                    OnBeginDrag();
                    break;
                case InputEventType.OnDrag:
                    OnDrag();
                    break;
                case InputEventType.OnEndDrag:
                    OnEndDrag();
                    break;
                case InputEventType.OnDrop:
                    OnDrop();
                    break;
                case InputEventType.OnSelect:
                    OnSelect();
                    break;
                case InputEventType.OnDeselect:
                    OnDeselect();
                    break;
                case InputEventType.OnUpdateSelected:
                    OnUpdateSelected();
                    break;
            }
            OnNoticeEvent(type);
        }

        /// <summary>
        /// ボタンのロックが自動アンロックされた通知を受け取ります。
        /// </summary>
        public abstract void NoticeAutoUnlock();

#endregion

#region protected methods

        /// <summary>
        /// トランジションを停止させます。
        /// </summary>
        protected abstract void StopTransition();

        protected virtual void OnPress() { }
        protected virtual void OnRelease() { }
        protected virtual void OnClick() { }
        protected virtual void OnLongPress() { }
        protected virtual void OnKeyRepeat() { }
        protected virtual void OnInitDrag() { }
        protected virtual void OnBeginDrag() { }
        protected virtual void OnDrag() { }
        protected virtual void OnEndDrag() { }
        protected virtual void OnDrop() { }
        protected virtual void OnSelect() { }
        protected virtual void OnDeselect() { }
        protected virtual void OnUpdateSelected() { }
        protected virtual void OnNoticeEvent(InputEventType type) { }

#endregion

#region override unity methods

        protected virtual void OnDisable()
        {
            StopTransition();
        }

#endregion
    }
}
