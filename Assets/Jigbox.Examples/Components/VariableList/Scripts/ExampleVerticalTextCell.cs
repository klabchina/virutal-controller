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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Jigbox.Components;

namespace Jigbox.Examples
{
    public class ExampleVerticalTextCell : VariableListCell
    {
        [SerializeField]
        GameObject ownView = null;

        [SerializeField]
        GameObject otherView = null;

        [SerializeField]
        float cellTopPadding = 0.0f;

        [SerializeField]
        float cellBottomPadding = 0.0f;

        [SerializeField]
        Components.TextView ownText = null;

        [SerializeField]
        Components.TextView otherText = null;

        public virtual void SetText(string text,bool isOwn)
        {
            SwitchView(isOwn);

            if (isOwn)
            {
                ownText.Text = text;
                return;
            }

            otherText.Text = text;
        }

        public virtual void OnSizeUpdate(bool isOwn)
        {
            if (isOwn)
            {
                CellSize = ownText.GetPreferredHeight() + cellTopPadding + cellBottomPadding;
                return;
            }

            CellSize = otherText.GetPreferredHeight() + cellTopPadding + cellBottomPadding;
        }

        void SwitchView(bool isOwn)
        {
            ownView.SetActive(isOwn);
            otherView.SetActive(!isOwn);
        }
    }
}
