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

namespace Jigbox.Components
{
    public class PopupGroupView : PopupView
    {

#region serialize fields & properties

        /// <summary>
        /// PopupViewのソートを行うコンポーネント
        /// </summary>
        [SerializeField]
        [HideInInspector]
        IPopupViewSorter sorter;

        /// <summary>
        /// PopupViewのソートを行うコンポーネントへの参照
        /// </summary>
        protected IPopupViewSorter Sorter
        {
            get
            {
                if (sorter == null)
                {
                    sorter = GetComponent<IPopupViewSorter>();
                    if (sorter == null)
                    {
                        Debug.LogError("IPopupViewSorter not found.");
                    }
                }

                return sorter;
            }
        }

        /// <summary>
        /// 現在最前面に表示されているViewかどうか
        /// </summary>
        public virtual bool IsFront
        {
            get { return Sorter.IsFront; }
        }

        /// <summary>
        /// このViewの背面に存在するViewに、いくつポップアップが表示されているか
        /// </summary>
        public virtual int BackViewPopupCount { get; set; }

        /// <summary>
        /// Viewのグループ名を返します
        /// </summary>
        public virtual string GroupName { get; set; }

        protected override int backKeyNoticePriority
        {
            get { return base.backKeyNoticePriority + BackViewPopupCount; }
        }

#endregion

#region public methods

        /// <summary>
        /// Viewが最前面でなくなった時の処理を行います
        /// </summary>
        public virtual void MoveToBack()
        {
            inputBlocker.Back.SetActive(false);
            Sorter.MoveToBack();
        }

        /// <summary>
        /// Viewが最前面になった時の処理を行います
        /// </summary>
        public virtual void MoveToFront()
        {
            inputBlocker.Back.SetActive(true);
            Sorter.MoveToFront();
        }

#endregion

#region protected methods

        protected override void OnCloseAllPopup()
        {
            Destroy(gameObject);
        }

#endregion

#region override unity methods

        protected override void OnDestroy()
        {
            if (onDestory != null)
            {
                onDestory(GroupName);
            }
        }

#endregion
    }
}
