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

using UnityEditor;
using UnityEngine;

namespace Jigbox.Components
{
    [CustomEditor(typeof(AccordionListHorizontal), true)]
    public class AccordionListHorizontalEditor : AccordionListEditor
    {
        protected override Vector2 ViewPivot
        {
            get { return new Vector2(0.0f, 0.5f); }
        }

        protected override Vector2 ContentAnchorMin
        {
            get { return Vector2.zero; }
        }

        protected override Vector2 ContentAnchorMax
        {
            get { return new Vector2(0.0f, 1.0f); }
        }
    }
}
