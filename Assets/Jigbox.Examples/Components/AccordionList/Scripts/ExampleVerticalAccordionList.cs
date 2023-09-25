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

using Jigbox.Components;
using UnityEngine;

namespace Jigbox.Examples
{
    public class ExampleVerticalAccordionList : AccordionListVertical
    {
        ExampleAccordionListFocusTransition focusTransition;

        protected ExampleAccordionListFocusTransition FocusTransition
        {
            get
            {
                if (focusTransition == null)
                {
                    focusTransition = GetComponent<ExampleAccordionListFocusTransition>();
                }

                return focusTransition;
            }
        }

        protected override float ExpandVisibleSize
        {
            get
            {
                var visibleSize = base.ExpandVisibleSize;
                foreach (var visibleCell in VisibleCellHashSet)
                {
                    if (visibleSize < visibleCell.CellSize)
                    {
                        visibleSize = visibleCell.CellSize;
                    }
                }

                visibleSize += Padding.Bottom;
                return visibleSize;
            }
        }

        protected override float ExpandClippingAreaSize
        {
            get { return ClippingArea.sizeDelta.y; }
        }

        protected override void StartExpandTransition(AccordionListCellInfo cellInfo)
        {
            var scroll = ContentPositionByIndex(cellInfo.Index, -1.0f);
            if (FocusTransition != null)
            {
                FocusTransition.StartTransition(Content, scroll);
            }

            base.StartExpandTransition(cellInfo);
        }

        protected override void StartCollapseTransition(AccordionListCellInfo cellInfo)
        {
            var scroll = ContentPositionByIndex(cellInfo.Index, -1.0f);
            if (FocusTransition != null)
            {
                FocusTransition.StartTransition(Content, scroll);
            }

            base.StartCollapseTransition(cellInfo);
        }

        protected override float ExpandSimplifyScrollPosition(AccordionListCellInfo targetCellInfo)
        {
            if (targetCellInfo == null)
            {
                return base.ExpandSimplifyScrollPosition(targetCellInfo);
            }

            return ContentPositionByIndex(targetCellInfo.Index, -1.0f).y;
        }

        protected override float CollapsedSimplifyScrollPosition(AccordionListCellInfo targetCellInfo)
        {
            if (targetCellInfo == null)
            {
                return base.CollapsedSimplifyScrollPosition(targetCellInfo);
            }

            return ContentPositionByIndex(targetCellInfo.Index, -1.0f).y;
        }
    }
}
