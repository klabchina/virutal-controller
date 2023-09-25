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

namespace Jigbox.Components
{
    public class BasicToggleSwitchTransition : ToggleSwitchTransitionBase
    {
#region properties

        /// <summary>Tween</summary>
        protected TweenVector3 tween;

#endregion

#region protected methods

        /// <summary>
        /// トランジションを開始します。
        /// </summary>
        protected override void StartTransition()
        {
            tween.Kill();

            if (status)
            {
                tween.Begin = knob.transform.localPosition;
                tween.Final = positionOn;
            }
            else
            {
                tween.Begin = knob.transform.localPosition;
                tween.Final = positionOff;
            }

            tween.Start();
        }

        /// <summary>
        /// トランジションを停止します。
        /// </summary>
        protected override void StopTransition()
        {
            tween.Complete();
        }

        /// <summary>
        /// Tweenの更新時に呼び出されます。
        /// </summary>
        /// <param name="tween">Tween</param>
        protected virtual void OnUpdateTween(ITween<Vector3> tween)
        {
            knob.transform.localPosition = tween.Value;
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            tween = new TweenVector3();
            tween.Begin = positionOn;
            tween.Final = positionOff;
            tween.Duration = duration;
            tween.OnUpdate(OnUpdateTween);
        }

#endregion
    }
}
