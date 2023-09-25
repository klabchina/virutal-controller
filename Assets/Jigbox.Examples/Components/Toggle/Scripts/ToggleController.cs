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
using Jigbox.Components;

namespace Jigbox.Examples
{
    public class ToggleController : ExampleSceneBase
    {
        public BasicToggle BasicToggle;
        public Graphic BasicToggleGraphics;

        [AuthorizedAccess]
        void OnValueChanged(bool value)
        {
            Debug.Log("OnValueChanged Event called.(value:" + value + ")");
        }

        [AuthorizedAccess]
        void OnBasicToggleValueChanged(bool value)
        {
            Debug.Log("OnBasicToggleValueChanged Event called.(value:" + value + ")");
            UpdateBasicToggle(value);
        }

        void UpdateBasicToggle(bool value)
        {
            if (value)
            {
                BasicToggleGraphics.color = Color.white;
            }
            else
            {
                BasicToggleGraphics.color = Color.black;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            UpdateBasicToggle(BasicToggle.IsOn);
        }
    }
}
