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

namespace Jigbox
{
    public class IndexedRectTransform
    {
        public int Index { get; set; }

        public RectTransform RectTransform { get; set; }

        public IndexedRectTransform(int i, RectTransform t)
        {
            Index = i;
            RectTransform = t;
        }

        public void SetCellTransform(Vector2 position, Vector2 sizeDelta)
        {
            if (this.RectTransform == null)
            {
                return;
            }
            this.RectTransform.anchoredPosition = position;
            this.RectTransform.sizeDelta = sizeDelta;
        }

        public void SetCellTransform(Vector2 position, Vector2 sizeDelta, Vector2 pivot)
        {
            if (this.RectTransform == null)
            {
                return;
            }
            this.RectTransform.anchoredPosition = position;
            this.RectTransform.pivot = pivot;
            this.RectTransform.sizeDelta = sizeDelta;
        }

        public void SetCellTransform(Vector2 position, Vector2 sizeDelta, Vector2 pivot, Vector2 anchorMin, Vector2 anchorMax)
        {
            if (this.RectTransform == null)
            {
                return;
            }
            this.RectTransform.anchorMin = anchorMin;
            this.RectTransform.anchorMax = anchorMax;
            this.RectTransform.anchoredPosition = position;
            this.RectTransform.pivot = pivot;
            this.RectTransform.sizeDelta = sizeDelta;
        }
    }
}
