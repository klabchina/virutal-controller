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
using Jigbox.SceneTransition;

namespace Jigbox.Examples
{
    public sealed class PopupExampleController : BaseSceneController
    {
#region protected methods

        protected override void OnFinishSceneOutTransition(string nextSceneName)
        {
            ExamplePopupOrder order = new ExamplePopupOrder("SceneChangePopup");
            order.OnCompleteClose = _ => base.OnFinishSceneOutTransition(nextSceneName);
            ForefrontPopupManager.Instance.OpenQueue(order);
        }

#endregion

#region private methods

        [RuntimeInitializeOnLoadMethod]
        static void InitManager()
        {
            // 最初にPopupViewのプロバイダを入れておく
            PopupManager.Instance.ViewProvider = new ExamplePopupViewProvider();
            ForefrontPopupManager.Instance.ViewProvider = new ExampleForefrontPopupViewProvider();
            PopupGroupManager.Instance.ViewProvider = new ExamplePopupGroupViewProvider();

            SceneTransitionManager.Instance.DefaultTransitionPrefabPath = "PopupExsampleFadeTransition";
        }

        [AuthorizedAccess]
        void OpenInformation()
        {
            ExamplePopupOrder order = new ExamplePopupOrder("InformationPopup");
            PopupManager.Instance.OpenQueue(order);
            // 閉じてから出るのを確認するために2回分開く
            PopupManager.Instance.OpenQueue(order);
        }

        [AuthorizedAccess]
        void OpenConfirm()
        {
            ExampleConfirmPopupOrder order = new ExampleConfirmPopupOrder("ConfirmPopup");
            order.OnPositive = _ => OpenConfirm();
            PopupManager.Instance.Open(order);
        }

        [AuthorizedAccess]
        void OpenPurchase()
        {
            ExamplePopupOrder order = new ExamplePopupOrder("PurchasePopup");
            PopupManager.Instance.Open(order);
        }

        [AuthorizedAccess]
        void OpenSceneSelect()
        {
            ExampleSceneSelectPopup.SceneSelectPopupOrder order = new ExampleSceneSelectPopup.SceneSelectPopupOrder("SceneSelectPopup");
            order.IsOpenModal = string.IsNullOrEmpty(SceneManager.Instance.CurrentModalSceneName);
            PopupManager.Instance.Open(order);
        }

        [AuthorizedAccess]
        void OpenInformationGroup()
        {
            // PopupManagerと同じ使い方をPopupGroupManagerでも行う
            var order = new ExamplePopupGroupOrder("InformationPopup", "InformationPopup");
            PopupGroupManager.Instance.OpenQueue(order);
            PopupGroupManager.Instance.OpenQueue(order);
        }

        [AuthorizedAccess]
        void OpenConfirmGroup()
        {
            OpenConfirmGroup(0);
        }

        void OpenConfirmGroup(int groupCount)
        {
            // グループが異なるPopupを表示する場合Viewを新たに生成します
            var order = new ExampleConfirmPopupGroupOrder("ConfirmGroup:" + groupCount, "ConfirmPopup");
            order.OnPositive = _ => OpenConfirmGroup(groupCount + 1);
            PopupGroupManager.Instance.OpenQueue(order);
        }

        [AuthorizedAccess]
        void OpenConfirmSameGroup()
        {
            // グループが同じ場合、CloseAllで全てのViewを閉じます
            var order = new ExampleConfirmPopupGroupOrder("ConfirmGroup", "ConfirmPopup");
            order.OnPositive = _ => OpenConfirmSameGroup();
            order.OnNegative = _ => PopupGroupManager.Instance.CloseAll("ConfirmGroup");
            PopupGroupManager.Instance.Open(order);
        }

        [AuthorizedAccess]
        void OpenPurchaseGroup()
        {
            // PopupBaseにグループ名が受け渡されます
            var order = new ExamplePopupGroupOrder("PurchaseGroupPopup", "PurchaseGroupPopup");
            PopupGroupManager.Instance.Open(order);
        }

#endregion
    }
}
