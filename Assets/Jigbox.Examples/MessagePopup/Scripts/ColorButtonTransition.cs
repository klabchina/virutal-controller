/**
 * Jigbox
 * Copyright(c) 2016 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 *
 * Subject to the prior written consent of KLab, Inc (Licensor) and its terms and
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
    public class ColorButtonTransition : ButtonTransitionBase
    {
#region properties

        /// <summary>Tween</summary>
        protected Tween.TweenSingle tween = new Tween.TweenSingle();

        /// <summary>押下時のトランジション状態かどうか</summary>
        protected bool isPressTransition = false;

        /// <summary>ボタンの上にかぶさって、普段は透明だが押されると半透明の黒になる <c>Image</c></summary>
        [SerializeField]
        Image overlayingImage = null;

        float originalAlpha;
        
        readonly float pressedAlpha = 0.4f;

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
            this.tween.Complete();
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

            this.tween.Kill();

            this.tween.Begin = this.originalAlpha;
            this.tween.Final = this.pressedAlpha;
            this.tween.Duration = 0.12f;

            this.tween.Start();
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

            this.tween.Kill();

            this.tween.Begin = this.pressedAlpha;
            this.tween.Final = this.originalAlpha;
            this.tween.Duration = 0.12f;

            this.tween.Start();
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            this.originalAlpha = this.overlayingImage.color.a;
            this.tween.OnUpdate(_ =>
            {
                Color color = this.overlayingImage.color;
                color.a = tween.Value;
                this.overlayingImage.color = color;
            });
            this.tween.Begin = this.originalAlpha;
            this.tween.Final = this.pressedAlpha;
        }

#endregion
    }
}
