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
using UnityEngine.UI;
using System;

namespace Jigbox.Examples
{
    public class ListItemViewSample : MonoBehaviour
    {
        [SerializeField]
        Text indexView = null;

        [SerializeField]
        Text colorLabel = null;

        [SerializeField]
        Text colorInfo = null;

        [SerializeField]
        Image colorView = null;

        string HexRGBA(Color color)
        {
            var buf = "#";
            foreach (var item in new[] { color.r, color.g, color.b, color.a })
            {
                var hex = (int) Math.Round(item * 0xFF);
                buf += hex.ToString("X2");
            }
            return buf;
        }

        public void BatchModel(ListItemModelSample model)
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
                colorInfo.text = HexRGBA(model.Color);
                colorInfo.color = model.Color;
            }
            if (colorView)
            {
                colorView.color = model.Color;
            }
        }
    }
}
