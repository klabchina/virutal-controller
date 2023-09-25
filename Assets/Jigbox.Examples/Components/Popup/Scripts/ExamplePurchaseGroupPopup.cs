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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Jigbox.Components;

namespace Jigbox.Examples
{
    public class ExamplePurchaseGroupPopup : PopupGroupBase
    {
#region properties

        [SerializeField]
        ButtonBase buyButton = null;

        [SerializeField]
        Components.Slider slider = null;

        [SerializeField]
        Text countLabel = null;

#endregion

#region public methods

        public override void Init(PopupView view, PopupOrder order, int priority)
        {
            buyButton.Clickable = false;
            base.Init(view, order, priority);
        }

#endregion

#region private methods

        [AuthorizedAccess]
        void OnClickPositive()
        {
            var order = new ExampleConfirmPopupGroupOrder(GroupName, "PurchaseConfirmPopup");
            order.OnPositive = _ => closer.CloseAll();
            PopupGroupManager.Instance.Open(order);
        }

        [AuthorizedAccess]
        void OnClickClose()
        {
            closer.Close();
        }

        [AuthorizedAccess]
        void OnStepChanged(int step)
        {
            countLabel.text = step.ToString();
            buyButton.Clickable = step > 0;
        }

        [AuthorizedAccess]
        void OnClickIncrement()
        {
            int step = slider.CurrentStep + 1;
            step = Mathf.Min(step, slider.Steps);
            slider.CurrentStep = step;
        }

        [AuthorizedAccess]
        void OnClickDecrement()
        {
            int step = slider.CurrentStep - 1;
            step = Mathf.Clamp(step, 0, slider.Steps);
            slider.CurrentStep = step;
        }

#endregion
    }
}
