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
    [RequireComponent(typeof(BasicButton))]
    public class ColorToggle : BasicToggle
    {
#region properties

        /// <summary>ボタン</summary>
        [HideInInspector]
        protected BasicButton button;

#endregion

#region protected methods

        protected override void OnUpdateIsOn()
        {
            UpdateDisplay();
        }

        public override void UpdateDisplay()
        {
            button.IsEnableColor = isOn;
        }

#endregion

#region override unity methods

        protected override void Awake()
        {
            base.Awake();

            button = GetComponent<BasicButton>();

            UpdateDisplay();
        }

#endregion
    }
}
