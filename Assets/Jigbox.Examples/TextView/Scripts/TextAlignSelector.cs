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
using Jigbox.TextView;

namespace Jigbox.Examples
{
    [RequireComponent(typeof(Toggle))]
    public class TextAlignSelector : MonoBehaviour
    {
        [SerializeField]
        Components.TextView targetTextView = null;

        [SerializeField]
        TextAlign align = TextAlign.Left;

        [SerializeField]
        Text label = null;

        Toggle toggle;

        [SerializeField]
        Color unselected = new Color(0.25f, 0.25f, 0.25f, 1.0f);

        [SerializeField]
        Color selected = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        // Use this for initialization
        void Start()
        {
            if (toggle == null)
            {
                toggle = GetComponent<Toggle>();
            }
            if (toggle != null)
            {
                toggle.image.color = unselected;
                toggle.graphic.color = selected;
                
                if (label != null) {
                    label.color = toggle.isOn ? selected : unselected;
                }
                toggle.onValueChanged.AddListener(OnValueChanged);
            }
        }

        void OnValueChanged(bool isChecked)
        {
            if (label != null)
            {
                label.color = isChecked ? selected : unselected;
            }
            if (isChecked && targetTextView != null && targetTextView.Alignment != align)
            {
                targetTextView.Alignment = align;
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
