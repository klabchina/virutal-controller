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
    public abstract class InputBehaviour : MonoBehaviour, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler
    {
#region public methods

        /// <summary>
        /// ボタンが押下された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
        }

        /// <summary>
        /// ボタン押下後に指が離された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
        }

        /// <summary>
        /// ボタンを押下し、ボタン上で指が離された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
        }

        /// <summary>
        /// ボタンの判定領域内に指が入った際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
        }

        /// <summary>
        /// ボタンの判定領域内から指が出た際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
        }

        /// <summary>
        /// ドラッグが終了した際に、その位置に別なオブジェクトが存在していた場合、そのオブジェクトで呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrop(PointerEventData eventData)
        {
        }

        /// <summary>
        /// ドラッグ対象が見つかった際に呼び出されます。(実質押下と同タイミング)
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
        }

        /// <summary>
        /// ドラッグが開始された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
        }

        /// <summary>
        /// ドラッグ中にポインタが移動した際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
        }

        /// <summary>
        /// ドラッグが終了した際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
        }

#endregion
    }
}
