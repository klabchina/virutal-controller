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
    public class DragBehaviour : MonoBehaviour, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
    {
#region properties

        /// <summary>入力検知用コンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected InputBehaviour behaviour;

#endregion

#region public methods
        
        /// <summary>
        /// ドラッグ対象が見つかった際に呼び出されます。(実質押下と同タイミング)
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (behaviour != null)
            {
                behaviour.OnInitializePotentialDrag(eventData);
            }
        }

        /// <summary>
        /// ドラッグが開始された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (behaviour != null)
            {
                behaviour.OnBeginDrag(eventData);
            }
        }

        /// <summary>
        /// ドラッグ中にポインタが移動した際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (behaviour != null)
            {
                behaviour.OnDrag(eventData);
            }
        }

        /// <summary>
        /// ドラッグが終了した際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (behaviour != null)
            {
                behaviour.OnEndDrag(eventData);
            }
        }

#endregion
        
#region override unity methods

        protected virtual void Awake()
        {
            if (behaviour == null)
            {
                behaviour = GetComponent<InputBehaviour>();
            }
        }

#endregion
    }
}
