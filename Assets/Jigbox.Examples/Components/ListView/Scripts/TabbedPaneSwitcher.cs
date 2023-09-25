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

namespace Jigbox.Examples
{
    public class TabbedPaneSwitcher : MonoBehaviour, ITabbedPaneContents
    {
        [SerializeField]
        GameObject[] contents = null;

        public void TabChanged(TabbedPane tabbedPane, BasicToggle activeTab, int activeTabIndex)
        {
            SetActive(activeTabIndex);
        }

        void SetActive(int index)
        {
            for (int i = 0; i < contents.Length; i++)
            {
                contents[i].gameObject.SetActive(i == index);
            }
        }

        void Awake()
        {
            SetActive(0);
        }
    }
}
