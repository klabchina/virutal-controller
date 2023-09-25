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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Jigbox.Components;

namespace Jigbox.Examples
{
    public sealed class ImitateInputExampleController : ExampleSceneBase
    {
#region properties

        [SerializeField]
        Text text = null;

        [SerializeField]
        Components.ToggleGroup toggleGroup = null;

#endregion

        void Start()
        {
            var eventSystem = GameObject.Find("JigboxEventSystem");
            if (eventSystem)
            {
                eventSystem.AddComponent<ImitateInputModule>();
            }
        }

#region private methods

        [AuthorizedAccess]
        void OnPress(PointerEventData eventData)
        {
            text.text = "Pointer Id : " + eventData.pointerId;
        }

        [AuthorizedAccess]
        void OnNext()
        {
            int next = toggleGroup.ActiveToggleIndex + 1;
            if (next < toggleGroup.Count)
            {
                toggleGroup.SetActive(next);
            }
        }

        [AuthorizedAccess]
        void OnPrev()
        {
            int next = toggleGroup.ActiveToggleIndex -1;
            if (next >= 0)
            {
                toggleGroup.SetActive(next);
            }
        }

#endregion
    }
}
