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
    public sealed class GoldGaugeController : MonoBehaviour, IGaugeStepLimiter
    {
#region properties

        [SerializeField]
        Gauge slider = null;

        [SerializeField]
        Text goldText = null;

        float onePrice = 100;

        float goldAmount = 3003;

        float payGolds = 0;

#endregion

#region private methods

        [AuthorizedAccess]
        void OnStepChanged(int step)
        {
            if (slider != null)
            {
                payGolds = step * onePrice;
            }
            UpdateText();
        }

        [AuthorizedAccess]
        void OnClickPlus()
        {
            if (slider != null)
            {
                slider.CurrentStep = slider.CurrentStep + 1;
                UpdateText();
            }
        }

        [AuthorizedAccess]
        void OnClickMinus()
        {
            if (slider != null)
            {
                slider.CurrentStep = slider.CurrentStep - 1;
                UpdateText();
            }
        }

        void UpdateText()
        {
            if (goldText != null)
            {
                goldText.text = (goldAmount - payGolds).ToString();
            }
        }

#endregion

#region override unity methods

        void Awake()
        {
            OnStepChanged(slider.CurrentStep);
            UpdateText();
        }

#endregion

        public int LimitCurrentStep(int steps, int currentStep, int nextStep)
        {
            if (nextStep < 0)
            {
                return 0;
            }
            // お金が足りない場合
            if (onePrice * nextStep > goldAmount)
            {
                // 買える最大量にする
                return Mathf.FloorToInt(goldAmount / onePrice);
            }
            // お金が足りる場合はnextStepにして問題ない
            return nextStep;
        }
    }
}
