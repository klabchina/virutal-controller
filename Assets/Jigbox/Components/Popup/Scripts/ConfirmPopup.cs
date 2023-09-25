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

namespace Jigbox.Components
{
    public class ConfirmPopup : PopupBase
    {
#region properties

        /// <summary>確認内容に対して、肯定相当のアクションを行った際のコールバック</summary>
        protected Action<PopupBase> onPositive = null;

        /// <summary>確認内容に対して、否定相当のアクションを行った際のコールバック</summary>
        protected Action<PopupBase> onNegative = null;

#endregion

#region public methods

        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="view">PopupView</param>
        /// <param name="order">ポップアップの表示要求</param>
        /// <param name="priority">エスケープキー(Androidのバックキー)の通知優先度/param>
        public override void Init(PopupView view, PopupOrder order, int priority)
        {
            if (order is ConfirmPopupOrder)
            {
                ConfirmPopupOrder confirmOrder = order as ConfirmPopupOrder;
                onPositive = confirmOrder.OnPositive;
                onNegative = confirmOrder.OnNegative;
            }

            base.Init(view, order, priority);
        }

#endregion

#region protected methods

        /// <summary>
        /// エスケープキー(Androidのバックキー)が押された際に呼び出されます。
        /// </summary>
        public override void OnEscape()
        {
            if (onEscape != null)
            {
                onEscape(this);
            }
            else if (onNegative != null)
            {
                onNegative(this);
            }
            // 何も処理がない場合は、ポップアップを閉じる
            else
            {
                closer.Close();
            }
        }

        /// <summary>
        /// 肯定ボタンが押下された際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnClickPositive()
        {
            if (onPositive != null)
            {
                onPositive(this);
            }
        }

        /// <summary>
        /// 否定ボタンが押下された際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnClickNegative()
        {
            if (onNegative != null)
            {
                onNegative(this);
            }
            // 否定相当のコールバックがない場合は、ポップアップを閉じる
            else
            {
                closer.Close();
            }
        }

#endregion
    }
}
