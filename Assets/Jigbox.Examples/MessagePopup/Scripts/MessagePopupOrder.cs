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

namespace Jigbox.Examples
{
    /// <summary>
    /// メッセージを表示するポップアップを開く注文です。
    /// </summary>
    public class MessagePopupOrder : PopupOrder
    {
#region private properties

        /// <summary>
        /// 表示するメッセージです。
        /// </summary>
        string message;

#endregion
        
        
#region constructors

        /// <summary>
        /// ポップアップに表示するメッセージを受け取るコンストラクターです。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public MessagePopupOrder(string message)
        {
            this.message = message;
        }

#endregion
#region public methods


        /// <summary>
        /// ポップアップを生成します。
        /// </summary>
        /// <returns>ポップアップ</returns>
        public override PopupBase Generate()
        {
            var popup = LoadFromPath("MessagePopup") as MessagePopup;
            popup.SetMessage(this.message);
            return popup;
        }

#endregion
    }
}
