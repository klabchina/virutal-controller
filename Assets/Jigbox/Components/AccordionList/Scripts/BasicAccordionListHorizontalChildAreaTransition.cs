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
using Jigbox.UIControl;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// 横AccordionListのチャイルドエリアトランジション
    /// </summary>
    public class BasicAccordionListHorizontalChildAreaTransition : BasicAccordionListChildAreaTransitionBase
    {
        protected override Vector2 TransitionPivot
        {
            get { return new Vector2(0.0f, 0.5f); }
        }

        protected override void OnUpdateExpandTransition(ITween<float> tween)
        {
            foreach (var cell in childAreaCells)
            {
                var size = cell.CellReference.RectTransform.sizeDelta;
                size.x = cell.Size - changeSize + tween.Value;
                cell.CellReference.RectTransform.sizeDelta = size;
            }
        }

        protected override void OnUpdateCollapseTransition(ITween<float> tween)
        {
            foreach (var cell in childAreaCells)
            {
                var size = cell.CellReference.RectTransform.sizeDelta;
                size.x = cell.Size + changeSize - tween.Value;
                cell.CellReference.RectTransform.sizeDelta = size;
            }
        }
        
        protected override void OnCompleteTransition(ITween tween)
        {
            foreach (var cell in childAreaCells)
            {
                var size = cell.CellReference.RectTransform.sizeDelta;
                size.x = cell.Size;
                cell.CellReference.RectTransform.sizeDelta = size;
                RectTransformUtils.SetPivot(cell.CellReference.RectTransform, new Vector2(0.5f, 0.5f));
            }
        }
    }
}
