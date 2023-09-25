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
using System.Collections.Generic;

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public abstract class PopupTransitionBase : MonoBehaviour
    {
#region properties

        /// <summary>トランジションの状態の通知を受け取るモジュール</summary>
        protected List<IPopupTransitionHandler> handlers = new List<IPopupTransitionHandler>();

#endregion

#region public methods

        /// <summary>
        /// ポップアップのトランジションの状態の通知を受け取る対象を設定します。
        /// </summary>
        /// <param name="handlers">トランジションの状態の通知を受け取るモジュール</param>
        public void SetHandler(params IPopupTransitionHandler[] handlers)
        {
            this.handlers.Clear();
            this.handlers.AddRange(handlers);
        }

        /// <summary>
        /// ポップアップを開きます。
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// ポップアップを閉じます。
        /// </summary>
        public abstract void Close();

#endregion

#region protected methods

        /// <summary>
        /// ポップアップを開くトランジションを開始する事を通知します。
        /// </summary>
        protected void NotifyOnBeginOpen()
        {
            foreach (IPopupTransitionHandler handler in handlers)
            {
                handler.OnBeginOpen();
            }
        }

        /// <summary>
        /// ポップアップを開くトランジションが完了した事を通知します。
        /// </summary>
        protected void NotifyOnCompleteOpen()
        {
            foreach (IPopupTransitionHandler handler in handlers)
            {
                handler.OnCompleteOpen();
            }
        }

        /// <summary>
        /// ポップアップを閉じるトランジションを開始する事を通知します。
        /// </summary>
        protected void NotifyOnBeginClose()
        {
            foreach (IPopupTransitionHandler handler in handlers)
            {
                handler.OnBeginClose();
            }
        }

        /// <summary>
        /// ポップアップを閉じるトランジションが完了した事を通知します。
        /// </summary>
        protected void NotifyOnCompleteClose()
        {
            foreach (IPopupTransitionHandler handler in handlers)
            {
                handler.OnCompleteClose();
            }
        }

#endregion
    }
}
