/**
 * Jigbox
 * Copyright(c) 2016 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 *
 * Subject to the prior written consent of KLab, Inc (Licensor) and its terms and
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

using Jigbox.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    /// <summary>
    /// メッセージを表示するポップアップです。
    /// </summary>
    public class MessagePopup : InformationPopup
    {
#region private properties

        /// <summary>
        /// メッセージを表示する <c>Text</c> です。
        /// </summary>
        [SerializeField]
        Text text = null;

        /// <summary>
        /// 「クリップボードにコピーしました」の <c>Text</c>
        /// </summary>
        [SerializeField]
        Text toastText = null;

        /// <summary>
        /// 「クリップボードにコピーしました」の <c>TweenAlpha</c>
        /// </summary>
        [SerializeField]
        TweenAlpha toastAlpha = null;

#endregion

#region private methods

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ForefrontPopupManager.Instance.ViewProvider == null)
            {
                ForefrontPopupManager.Instance.ViewProvider = new ExampleForefrontPopupViewProvider();
            }
        }

        /// <summary>
        /// クリップボードにコピーし、完了のトーストを表示します。
        /// </summary>
        [AuthorizedAccess]
        void OnClickCopyToClipboard()
        {
            GUIUtility.systemCopyBuffer = this.text.text;
            this.toastAlpha.Tween.Start();
        }

#endregion

#region public methods

        /// <summary>
        /// 表示するメッセージを登録します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void SetMessage(string message)
        {
            this.text.text = message;
        }

        /// <summary>
        /// メッセージを表示するポップアップを開きます。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public static void Open(string message)
        {
            ForefrontPopupManager.Instance.OpenQueue(new MessagePopupOrder(message));
        }

#endregion

#region Unity methods

        void Awake()
        {
            this.toastAlpha.Tween.OnStart(_ => { this.toastText.enabled = true; });
            this.toastAlpha.Tween.OnComplete(_ => { this.toastText.enabled = false; });
        }

#endregion
    }
}
