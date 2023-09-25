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
using Jigbox.Components;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public sealed class GaugeLimitController : ExampleSceneBase, IGaugeValueLimiter, IGaugeStepLimiter
    {
#region properties

        [SerializeField]
        float minValue = 0.0f;

        [SerializeField]
        float maxValue = 0.0f;

        [SerializeField]
        Gauge valueGauge = null;

        [SerializeField]
        int minStep = 0;

        [SerializeField]
        int maxStep = 0;

        [SerializeField]
        Gauge stepGauge = null;

        [SerializeField]
        Text valueText = null;

        [SerializeField]
        Text stepText = null;

#endregion

#region unity methods

        void Start()
        {
            valueText.text = string.Format("Limit : minValue {0} maxValue {1}", minValue, maxValue);
            stepText.text = string.Format("Limit : minStep {0} maxStep {1}", minStep, maxStep);
        }

#endregion

#region private methods

        [AuthorizedAccess]
        void SetValueToGauge(float value)
        {
            if (valueGauge != null)
            {
                valueGauge.Value = value;
            }
        }

        [AuthorizedAccess]
        void OnStepChanged(int step)
        {
            Debug.Log(step);
        }

        [AuthorizedAccess]
        void SetStepToGauge(int step)
        {
            if (stepGauge != null)
            {
                stepGauge.CurrentStep = step;
            }
        }

#endregion

        public float LimitValue(float currentValue, float newValue)
        {
            return Mathf.Clamp(newValue, minValue, maxValue);
        }

        public int LimitCurrentStep(int steps, int currentStep, int nextStep)
        {
            return Mathf.Clamp(nextStep, minStep, maxStep);
        }
    }
}
