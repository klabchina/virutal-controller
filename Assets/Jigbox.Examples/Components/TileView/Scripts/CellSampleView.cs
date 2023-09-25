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
    [RequireComponent(typeof(Image))]
    public class CellSampleView : MonoBehaviour
    {
        Text textComponent;

        public Text TextComponent
        {
            get
            {
                if (textComponent == null)
                {
                    textComponent = GetComponentInChildren<Text>();
                }
                return textComponent;
            }
        }

        Image imageComponent;

        public Image ImageComponent
        {
            get
            {
                if (imageComponent == null)
                {
                    imageComponent = GetComponent<Image>();
                }
                return imageComponent;
            }
        }

        public string Text
        {
            get { return TextComponent.text; }
            set { TextComponent.text = value; }
        }

        public Color BackgroundColor
        {
            get { return ImageComponent.color; }
            set { ImageComponent.color = value; }
        }

        public Color ForegroundColor
        {
            get { return TextComponent.color; }
            set { TextComponent.color = value; }
        }

        public void BatchModel(CellSampleModel model)
        {
            if (model == null)
            {
                throw new NullReferenceException("model");
            }
            Text = model.text;
            BackgroundColor = model.background;
            ForegroundColor = model.foreground;
        }
    }
}
