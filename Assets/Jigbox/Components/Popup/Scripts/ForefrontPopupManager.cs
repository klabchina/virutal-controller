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
    public class ForefrontPopupManager : PopupManagerBase
    {
#region properties

        /// <summary>インスタンス</summary>
        protected static ForefrontPopupManager instance;

        /// <summary>インスタンス</summary>
        public static ForefrontPopupManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ForefrontPopupManager();
                }
                return instance;
            }
        }

        /// <summary>PopupView</summary>
        protected PopupView view = null;

#endregion

#region protected methods

        /// <summary>
        /// PopupViewを取得します。
        /// </summary>
        /// <returns></returns>
        protected override PopupView GetView()
        {
            if (view == null)
            {
                if (viewProvider == null)
                {
                    Debug.LogError("ForefrontPopupManager.GetView : Can't create PopupView because not exist provider!");
                    return null;
                }
                view = viewProvider.Generate();
                // 最前面用のものは、シーンに依存しない情報を出すことを想定したものなので、
                // シーン遷移をしても破棄されないようにしておく
                GameObject.DontDestroyOnLoad(view.gameObject);
                view.gameObject.SetActive(false);
                view.SetDisposer(this);
            }

            return view;
        }

#endregion
    }
}
