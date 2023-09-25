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
using UnityEngine;

namespace Jigbox.Components
{
    public class TabbedPane: MonoBehaviour
    {
#region properties

        /// <summary>タブ</summary>
        [HideInInspector]
        [SerializeField]
        protected ToggleGroup tabs;

        /// <summary>タブ</summary>
        public ToggleGroup Tabs
        {
            get
            {
                return tabs;
            }
        }

        /// <summary>コンテンツ</summary>
        [HideInInspector]
        [SerializeField]
        protected MonoBehaviour contents = null;

        /// <summary>タブ切り替え後のコールバック/summary>
        public Action<TabbedPane, BasicToggle, int> TabChangedCallback { get; set; }

        /// <summary>ロックするCanvasGroup</summary>
        [HideInInspector]
        [SerializeField]
        protected CanvasGroup lockCanvasGroup;

        /// <summary>ロックするCanvasGroup</summary>
        public CanvasGroup LockCanvasGroup
        {
            get { return lockCanvasGroup; }
        }

#endregion

#region protected methods

        [AuthorizedAccess]
        protected void OnTabChanged()
        {
            if (TabChangedCallback != null)
            {
                TabChangedCallback(this, Tabs.ActiveToggle, Tabs.ActiveToggleIndex);
            }
        }

#endregion

#region public methods

        /// <summary>
        /// タブの切り替えをロックします
        /// </summary>
        public void LockTab()
        {
            if (lockCanvasGroup != null)
            {
                lockCanvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// タブの切り替えをアンロックします
        /// </summary>
        public void UnlockTab()
        {
            if (lockCanvasGroup != null)
            {
                lockCanvasGroup.blocksRaycasts = true;
            }
        }

#endregion

#region unity method

        protected virtual void Awake()
        {
            if (lockCanvasGroup == null && tabs != null)
            {
                lockCanvasGroup = tabs.GetComponent<CanvasGroup>();
                if (lockCanvasGroup == null)
                {
                    lockCanvasGroup = tabs.gameObject.AddComponent<CanvasGroup>();
                }
            }

            if (contents != null)
            {
                var handler = contents as ITabbedPaneContents;
                if (handler != null)
                {
                    TabChangedCallback = handler.TabChanged;
                }
            }
        }

#endregion
    }
}
