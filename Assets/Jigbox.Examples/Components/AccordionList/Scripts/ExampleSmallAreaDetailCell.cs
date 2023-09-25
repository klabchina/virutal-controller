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
    public class ExampleSmallAreaDetailCell : AccordionListVerticalMainCell
    {
        [SerializeField]
        Components.TextView textView = null;

        [SerializeField]
        float paddingTop = 10.0f;

        [SerializeField]
        float paddingBottom = 10.0f;

        public override void OnUpdateCell(AccordionListBase accordionList, AccordionListNode node)
        {
            var detail = (ExampleSmallAreaDetailNode) node;
            textView.Text = detail.SmallArea.Detail;
            textView.CalculateLayout();
        }

        public override float OnUpdateCellSize(AccordionListBase accordionList, AccordionListNode node)
        {
            var detail = (ExampleSmallAreaDetailNode) node;
            textView.Text = detail.SmallArea.Detail;
            return textView.GetPreferredHeight() + paddingTop + paddingBottom;
        }
    }
}
