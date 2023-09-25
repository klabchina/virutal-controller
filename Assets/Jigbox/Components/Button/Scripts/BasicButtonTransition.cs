﻿/**
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
    public class BasicButtonTransition : ButtonTransitionBase
    {
#region properties

        /// <summary>Tween</summary>
        protected Tween.TweenVector3 scaleTween = new Tween.TweenVector3();

        /// <summary>押下時のトランジション状態かどうか</summary>
        protected bool isPressTransition = false;
        
#endregion

#region public methods

        public override void NoticeAutoUnlock()
        {
            ReleaseTransition();
        }
        
#endregion

#region protected methods
        
        protected override void OnSelect()
        {
            PressTransition();
        }

        protected override void OnDeselect()
        {
            ReleaseTransition();
        }

        protected override void StopTransition()
        {
            scaleTween.Complete();
        }

#endregion

#region protected methods

        /// <summary>
        /// 押下時のトランジションを行います。
        /// </summary>
        protected virtual void PressTransition()
        {
            if (isPressTransition)
            {
                return;
            }
            isPressTransition = true;

            scaleTween.Kill();

            scaleTween.Begin = transform.localScale;
            scaleTween.Final = new Vector3(0.95f, 0.95f, 0.95f);
            scaleTween.Duration = 0.12f;

            scaleTween.Start();
        }

        /// <summary>
        /// ポインタが離された際のトランジションを行います。
        /// </summary>
        protected virtual void ReleaseTransition()
        {
            if (!isPressTransition)
            {
                return;
            }
            isPressTransition = false;

            scaleTween.Kill();

            scaleTween.Begin = transform.localScale;
            scaleTween.Final = Vector3.one;
            scaleTween.Duration = 0.12f;

            scaleTween.Start();
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            scaleTween.OnUpdate(tween => transform.localScale = tween.Value);
            scaleTween.Begin = transform.localScale;
            scaleTween.Final = transform.localScale;
        }

#endregion
    }
}