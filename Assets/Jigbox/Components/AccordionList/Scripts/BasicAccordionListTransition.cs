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

using Jigbox.Tween;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// アコーディオンリストの開閉時に使用するトランジション
    /// </summary>
    public class BasicAccordionListTransition : AccordionListTransitionBase
    {
#region serialize fields & properties

        /// <summary>トランジションの設定</summary>
        protected BasicAccordionListTransitionSetting transitionSetting = null;

        /// <summary>開く時のTween</summary>
        readonly TweenSingle expandTween = new TweenSingle();

        /// <summary>開く時のTweenの参照</summary>
        protected virtual TweenSingle ExpandTween
        {
            get { return expandTween; }
        }

        /// <summary>閉じる時のTween</summary>
        readonly TweenSingle collapseTween = new TweenSingle();

        /// <summary>閉じる時のTweenの参照</summary>
        protected virtual TweenSingle CollapseTween
        {
            get { return collapseTween; }
        }

#endregion

#region unity event

        void Awake()
        {
            transitionSetting = GetComponent<BasicAccordionListTransitionSetting>();
        }

#endregion

#region override methods

        public override void SetHandler(AccordionListTransitionHandlerBase transitionHandler)
        {
            ExpandTween.OnStart(t => transitionHandler.OnStartExpand());
            ExpandTween.OnUpdate(t => transitionHandler.OnUpdateExpand(t.Value, t.DeltaTime / t.Duration));
            ExpandTween.OnComplete(t => transitionHandler.OnCompleteExpand(t.Value));
            CollapseTween.OnStart(t => transitionHandler.OnStartCollapse());
            CollapseTween.OnUpdate(t => transitionHandler.OnUpdateCollapse(t.Value, t.DeltaTime / t.Duration));
            CollapseTween.OnComplete(t => transitionHandler.OnCompleteCollapse(t.Value));
        }

        public override void ExpandTransition(float begin, float final)
        {
            ExpandTween.Kill();
            ExpandTween.Begin = begin;
            ExpandTween.Final = final;
            ExpandTween.Duration = transitionSetting.ExpandDuration;
            ExpandTween.EasingType = transitionSetting.ExpandEasingType;
            ExpandTween.MotionType = transitionSetting.ExpandMotionType;
            ExpandTween.Start();
        }

        public override void CollapseTransition(float begin, float final)
        {
            CollapseTween.Kill();
            CollapseTween.Begin = begin;
            CollapseTween.Final = final;
            CollapseTween.Duration = transitionSetting.CollapseDuration;
            CollapseTween.EasingType = transitionSetting.CollapseEasingType;
            CollapseTween.MotionType = transitionSetting.CollapseMotionType;
            CollapseTween.Start();
        }

#endregion
    }
}
