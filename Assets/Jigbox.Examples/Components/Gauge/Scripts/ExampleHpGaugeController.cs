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
    public sealed class ExampleHpGaugeController : MonoBehaviour
    {
#region properties

        [SerializeField]
        Gauge gauge = null;

        [SerializeField]
        Text currentHp = null;

        [SerializeField]
        Text totalHp = null;

        [SerializeField]
        Image gaugeImage = null;

#endregion
        
#region private methods

        [AuthorizedAccess]
        void OnValueChanged(float value)
        {
            Color color;
            
            if (value > 0.8f)
            {
                color = Color.blue;
            }
            else if (value > 0.5f)
            {
                color = Color.green;
            }
            else if (value > 0.3f)
            {
                color = Color.yellow;
            }
            else
            {
                color = Color.red;
            }

            gaugeImage.color = color;
        }
        
        [AuthorizedAccess]
        void OnStepChanged(int step)
        {
            currentHp.text = step.ToString();
        }

#endregion

#region override unity methods

        void Awake()
        {
            totalHp.text = gauge.Steps.ToString();
            OnValueChanged(gauge.Value);
            OnStepChanged(gauge.CurrentStep);
        }

#endregion
    }
}
