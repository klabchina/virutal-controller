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
using Jigbox.Tween;

namespace Jigbox.Examples
{
    public class ExampleToggleSwitchTransition2 : ToggleSwitchTransitionBase
    {
#region properties

        protected TweenSingle tween;

        protected float current = 0.0f;

#endregion

#region public methods

        public override void Init(bool toggleStatus)
        {
            base.Init(toggleStatus);
            current = toggleStatus ? 1.0f : 0.0f;

            Vector3 rotation = Vector3.zero;
            rotation.y = -180 * current;
            knob.transform.localEulerAngles = rotation;
        }

#endregion

#region protected methods

        protected override void StartTransition()
        {
            tween.Kill();

            if (status)
            {
                tween.Begin = current;
                tween.Final = 1.0f;
            }
            else
            {
                tween.Begin = current;
                tween.Final = 0.0f;
            }

            tween.Start();
        }

        protected override void StopTransition()
        {
            tween.Complete();
        }

        protected virtual void OnUpdateTween(ITween<float> tween)
        {
            current = tween.Value;

            Vector3 position = Vector3.Lerp(positionOff, positionOn, current);
            Vector3 rotation = Vector3.zero;
            rotation.y = -180 * current;

            knob.transform.localPosition = position;
            knob.transform.localEulerAngles = rotation;
        }

#endregion

#region override unity methods

        void Awake()
        {
            tween = new TweenSingle();
            tween.Begin = 0.0f;
            tween.Final = 1.0f;
            tween.Duration = duration;
            tween.OnUpdate(OnUpdateTween);
        }

#endregion
    }
}
