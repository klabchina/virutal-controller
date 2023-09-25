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

using System.Collections.Generic;
using Jigbox.Tween;
using Jigbox.UIControl;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// 縦AccordionListのチャイルドエリアトランジション
    /// </summary>
    public abstract class BasicAccordionListChildAreaTransitionBase : AccordionListChildAreaTransitionBase
    {
        /// <summary>トランジション設定</summary>
        protected BasicAccordionListTransitionSetting transitionSetting;

        /// <summary>展開時のTween</summary>
        protected TweenSingle expandTween;

        /// <summary>折り畳み時のTween</summary>
        protected TweenSingle collapseTween;

        /// <summary>チャイルドエリアセル</summary>
        protected readonly List<AccordionListCellInfo> childAreaCells = new List<AccordionListCellInfo>();

        /// <summary>開閉で変わるサイズ</summary>
        protected float changeSize;

        /// <summary>トランジション中のPivot</summary>
        protected abstract Vector2 TransitionPivot { get; }

        void Awake()
        {
            transitionSetting = GetComponent<BasicAccordionListTransitionSetting>();
            expandTween = new TweenSingle();
            expandTween.OnUpdate(OnUpdateExpandTransition);
            expandTween.OnComplete(OnCompleteTransition);

            collapseTween = new TweenSingle();
            collapseTween.OnUpdate(OnUpdateCollapseTransition);
            collapseTween.OnComplete(OnCompleteTransition);
        }

        public override void StartExpand(List<AccordionListCellInfo> cellInfos, float expandSize, float changeValue)
        {
            childAreaCells.Clear();
            foreach (var cellInfo in cellInfos)
            {
                if (cellInfo.HasCellReference)
                {
                    childAreaCells.Add(cellInfo);
                    RectTransformUtils.SetPivot(cellInfo.CellReference.RectTransform, TransitionPivot);
                }
            }

            changeSize = expandSize;

            expandTween.Kill();

            expandTween.Begin = 0;
            expandTween.Final = changeValue;
            expandTween.Duration = transitionSetting.ExpandDuration;
            expandTween.EasingType = transitionSetting.ExpandEasingType;
            expandTween.MotionType = transitionSetting.ExpandMotionType;

            expandTween.Start();
        }

        public override void StartCollapse(List<AccordionListCellInfo> cellInfos, float collapseSize)
        {
            childAreaCells.Clear();
            foreach (var cellInfo in cellInfos)
            {
                if (cellInfo.HasCellReference)
                {
                    childAreaCells.Add(cellInfo);
                    RectTransformUtils.SetPivot(cellInfo.CellReference.RectTransform, TransitionPivot);
                }
            }

            changeSize = collapseSize;

            collapseTween.Kill();

            collapseTween.Begin = 0;
            collapseTween.Final = changeSize;
            collapseTween.Duration = transitionSetting.CollapseDuration;
            collapseTween.EasingType = transitionSetting.CollapseEasingType;
            collapseTween.MotionType = transitionSetting.CollapseMotionType;

            collapseTween.Start();
        }

        public override void ForceComplete()
        {
            expandTween.Complete();
            collapseTween.Complete();
            childAreaCells.Clear();
        }

        /// <summary>
        /// 展開時のTweenUpdateコールバック
        /// </summary>
        /// <param name="tween"></param>
        protected abstract void OnUpdateExpandTransition(ITween<float> tween);

        /// <summary>
        /// 折り畳みのTweenUpdateコールバック
        /// </summary>
        /// <param name="tween"></param>
        protected abstract void OnUpdateCollapseTransition(ITween<float> tween);

        /// <summary>
        /// トランジションの完了コールバック
        /// </summary>
        /// <param name="tween"></param>
        protected abstract void OnCompleteTransition(ITween tween);
    }
}
