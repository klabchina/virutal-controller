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
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Jigbox.Components
{
    public class PopupManager : PopupManagerBase
    {
#region properties

        /// <summary>インスタンス</summary>
        protected static PopupManager instance;

        /// <summary>インスタンス</summary>
        public static PopupManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PopupManager();
                }
                return instance;
            }
        }

        /// <summary>シーンごとのPopupView</summary>
        protected Dictionary<string, PopupView> views = new Dictionary<string, PopupView>();

#endregion

#region protected methods

        /// <summary>
        /// PopupViewを取得します。
        /// </summary>
        /// <returns></returns>
        protected override PopupView GetView()
        {
            return GetView(GetCurrentSceneName());
        }

        /// <summary>
        /// シーンに対応したPopupViewを取得します。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <returns></returns>
        protected virtual PopupView GetView(string sceneName)
        {
            PopupView view;
            if (!views.TryGetValue(sceneName, out view))
            {
                if (viewProvider == null)
                {
                    Debug.LogError("PopupManager.GetView : Can't create PopupView because not exist provider!");
                    return null;
                }
                view = viewProvider.Generate();
                view.gameObject.SetActive(false);
                views.Add(sceneName, view);
                view.SetDisposer(this);
                view.OnDestoryCallback = OnDestoryPopupView;
            }
            return view;
        }

        /// <summary>
        /// 現在表示されているシーン名を取得します。
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        /// <summary>
        /// PopupViewが破棄された際に呼び出されます。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        protected void OnDestoryPopupView(string sceneName)
        {
            if (views.ContainsKey(sceneName))
            {
                views.Remove(sceneName);
            }
        }

#endregion
    }
}
