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
using Jigbox.Tween;

namespace Jigbox.Examples
{
    public sealed class ExampleToggleSwitchTransition3 : ExampleToggleSwitchTransition2
    {
#region properties

        [SerializeField]
        GameObject onTarget = null;

        [SerializeField]
        GameObject offTarget = null;

#endregion

#region public methods

        public override void Init(bool toggleStatus)
        {
            base.Init(toggleStatus);

            // Example用で余計なことしてるから戻す
            knob.transform.localEulerAngles = Vector3.zero;

            Vector3 onScale = new Vector3(current, 1.0f, 1.0f);
            Vector3 offScale = new Vector3(1.0f - current, 1.0f, 1.0f);
            onTarget.transform.localScale = onScale;
            offTarget.transform.localScale = offScale;
        }

#endregion

#region protected methods

        protected override void OnUpdateTween(ITween<float> tween)
        {
            current = tween.Value;

            Vector3 position = Vector3.Lerp(positionOff, positionOn, current);
            Vector3 onScale = new Vector3(current, current, 1.0f);
            Vector3 offScale = new Vector3(1.0f - current, 1.0f - current, 1.0f);

            knob.transform.localPosition = position;
            onTarget.transform.localScale = onScale;
            offTarget.transform.localScale = offScale;
        }

#endregion
    }
}
