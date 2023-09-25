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

using System;
using System.Collections;
using System.Collections.Generic;
using Jigbox.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{

    public class TabbedPaneController : ExampleSceneBase, ITabbedPaneContents
    {
        public List<MonoBehaviour> contents;

        private float[] deferTime = { 0.0f, 0.3f, 1.0f, 3.0f, 5.0f };

#region ITabbedPaneContents

        public void TabChanged(TabbedPane tabbedPane, BasicToggle activeTab, int activeTabIndex)
        {
            Debug.Log("ITabChangeDelegater called. New Tab is " + activeTab.name);

            SetActive(activeTabIndex);

            if (deferTime[activeTabIndex] != 0.0f)
            {
                tabbedPane.LockTab();

                var textComponnet = contents[activeTabIndex].GetComponent<Text>();
                var originalText = textComponnet.text;
                textComponnet.text += "\nLoading";

                StartCoroutine(Defer(deferTime[activeTabIndex], () =>
                {
                    tabbedPane.UnlockTab();
                    textComponnet.text = originalText;
                }));
            }
        }

#endregion

#region private methods

        void SetActive(int index)
        {
            for (int i = 0; i < contents.Count; i++)
            {
                contents[i].enabled = (i == index);
            }
        }

        IEnumerator Defer(float deferTime, Action action)
        {
            yield return new WaitForSeconds(deferTime);
            action();
        }

#endregion

#region unity method

        protected override void Awake()
        {
            base.Awake();
            SetActive(0);
        }

#endregion
    }
}
