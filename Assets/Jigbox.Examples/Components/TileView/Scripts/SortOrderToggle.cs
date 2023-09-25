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
using System.Collections;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    [RequireComponent(typeof(Toggle))]
    public class SortOrderToggle : MonoBehaviour
    {
        [SerializeField]
        Color unselected = new Color(0.25f, 0.25f, 0.25f, 1.0f);

        [SerializeField]
        Color selected = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        Toggle toggle;

        Image mark;

        Text label;

        void OnToggleValueChanged(bool isOn)
        {
            var color = isOn ? selected : unselected;
            if (mark)
            {
                mark.color = color;
            }
            if (label)
            {
                label.color = color;
            }
        }

        void OnEnable()
        {
            if (toggle)
            {
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
            }
        }

        void OnDisable()
        {
            if (toggle)
            {
                toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            }
        }

        void Awake()
        {
            if (!toggle)
            {
                toggle = GetComponent<Toggle>();
            }
            if (!mark)
            {
                mark = GetComponentInChildren<Image>();
            }
            if (!label)
            {
                label = GetComponentInChildren<Text>();
            }

            if (toggle)
            {
                OnToggleValueChanged(toggle.isOn);
            }
        }

    }
}

