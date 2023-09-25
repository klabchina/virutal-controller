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
using Jigbox.Components;
using System.Collections.Generic;

namespace Jigbox.Examples
{
    public sealed class MaskExampleController : ExampleSceneBase
    {
#region constants

        static readonly int SoftnessMax = 20;

        static readonly float SoftnessDefaultValue = 0.5f;

#endregion

#region properties

        [SerializeField]
        ListViewVertical listView = null;

        [SerializeField]
        TileViewVertical tileView = null;

        [SerializeField]
        Slider slider = null;

        [SerializeField]
        GameObject listItem = null;

        [SerializeField]
        GameObject tileItem = null;

        [SerializeField]
        List<SoftnessRectMask> masks = null;

#endregion

#region private methods

        [AuthorizedAccess]
        void OnValueChanged(float value)
        {
            float softness = SoftnessMax * value;

            foreach (SoftnessRectMask mask in masks)
            {
                mask.Softness = new Vector2(0.0f, softness);
            }
        }

#endregion

#region override unity methods

        void Start()
        {
            listView.VirtualCellCount = 32;
            listView.FillCells(listItem);

            tileView.VirtualCellCount = 64;
            tileView.FillCells(tileItem);
            
            slider.Value = SoftnessDefaultValue;
        }

#endregion
    }
}
