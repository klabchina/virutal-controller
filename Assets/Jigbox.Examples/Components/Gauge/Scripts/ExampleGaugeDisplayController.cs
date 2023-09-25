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

using Jigbox.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class ExampleGaugeDisplayController : MonoBehaviour
    {
        [SerializeField]
        Gauge gauge = null;

        [SerializeField]
        Text currentValueText = null;

        [SerializeField]
        Text currentStepText = null;

        [SerializeField]
        Text stepsText = null;

#region private methods

        [AuthorizedAccess]
        void OnValueChanged(int value)
        {
            UpdateText();
        }

        [AuthorizedAccess]
        void OnStepChanged(int step)
        {
            UpdateText();
        }

        void UpdateText()
        {
            if (currentValueText != null)
            {
                currentValueText.text = string.Format("{0:f2}", gauge.Value);
            }
            if (stepsText != null)
            {
                stepsText.text = gauge.Steps.ToString();
            }
            if (currentStepText != null)
            {
                currentStepText.text = gauge.CurrentStep.ToString();
            }
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            UpdateText();
        }

#endregion
    }
}
