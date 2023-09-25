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
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class ScrollSelectItemViewSample : MonoBehaviour, IScrollSelectCell
    {
        [SerializeField]
        Text indexView = null;

        [SerializeField]
        Text colorLabel = null;

        [SerializeField]
        Text colorInfo = null;

        /// <summary>
        /// 選択セルどうか。
        /// IScrollSelectCellの挙動確認をEditor上で行う為に用意している。
        /// </summary>
        [SerializeField]
        bool isSelected = false;

        /// <summary>
        /// セルのindex。
        /// IScrollSelectCellの挙動確認をEditor上で行う為に用意している。
        /// </summary>
        [SerializeField]
        int cellIndex = 0;

        public bool IsSelected { get { return isSelected; } set { isSelected = value; } }

        public int CellIndex { get { return cellIndex; } set { cellIndex = value; } }

        public void BatchModel(ScrollSelectItemModelSample model)
        {
            if (indexView)
            {
                indexView.text = string.Format("#{0:D3}", model.Index);
                indexView.color = model.Color;
            }

            if (colorLabel)
            {
                colorLabel.color = model.Color;
            }

            if (colorInfo)
            {
                colorInfo.text = model.GetHexRGBA();
                colorInfo.color = model.Color;
            }
        }
    }
}
