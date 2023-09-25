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
using System;
using Jigbox.Components;
using Jigbox.SceneTransition;

namespace Jigbox.Examples
{
    public sealed class ExampleSceneSelectPopup : PopupBase
    {
#region inner classes, enum, and structs

        public sealed class SceneSelectPopupOrder : ExampleConfirmPopupOrder
        {
            public bool IsOpenModal { get; set;}

            public SceneSelectPopupOrder(string path) : base(path)
            {
            }

            public override PopupBase Generate()
            {
                GameObject obj = GameObject.Instantiate(Resources.Load(path)) as GameObject;
                return obj.GetComponent<PopupBase>();
            }
        }

#endregion

#region properties

        [SerializeField]
        Text buttonText = null;

        bool isOpenModal = true;

#endregion

#region public methods

        public override void Init(PopupView view, PopupOrder order, int priority)
        {
            if (order is SceneSelectPopupOrder)
            {
                SceneSelectPopupOrder sceneSelectOrder = order as SceneSelectPopupOrder;
                isOpenModal = sceneSelectOrder.IsOpenModal;
            }

            buttonText.text = isOpenModal ? "Exampleシーン3を開く" : "Exampleシーン3を閉じる";

            base.Init(view, order, priority);
        }

#endregion

#region private methods

        [AuthorizedAccess]
        void OnClickGoExample1()
        {
            SceneManager.Instance.LoadScene("Popup1");
        }

        [AuthorizedAccess]
        void OnClickGoExample2()
        {
            SceneManager.Instance.LoadScene("Popup2");
        }

        [AuthorizedAccess]
        void OnClickOpenExample3()
        {
            if (isOpenModal)
            {
                SceneManager.Instance.OpenModalScene("Popup3");
            }
            else
            {
                SceneManager.Instance.CloseModalScene();
            }
        }

        [AuthorizedAccess]
        void OnClickClose()
        {
            closer.Close();
        }

#endregion
    }
}
