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
    /// <summary>
    /// バルーンのトランジションコンポーネントの基底クラス
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Balloon))]
    public abstract class BalloonTransitionBase : MonoBehaviour
    {
#region property

        /// <summary>
        /// バルーンのハンドラ
        /// </summary>
        protected IBalloonTransitionHandler Handler;

#endregion

#region protected methods

        /// <summary>
        /// ハンドラにBeginOpenを通知する
        /// </summary>
        protected virtual void NoticeOnBeginOpen()
        {
            if (Handler != null)
            {
                Handler.OnBeginOpen();
            }
        }

        /// <summary>
        /// ハンドラにCompleteOpenを通知する
        /// </summary>
        protected virtual void NoticeOnCompleteOpen()
        {
            if (Handler != null)
            {
                Handler.OnCompleteOpen();
            }
        }

        /// <summary>
        /// ハンドラにBeginCloseを通知する
        /// </summary>
        protected virtual void NoticeOnBeginClose()
        {
            if (Handler != null)
            {
                Handler.OnBeginClose();
            }
        }

        /// <summary>
        /// ハンドラにCompleteCloseを通知する
        /// </summary>
        protected virtual void NoticeOnCompleteClose()
        {
            if (Handler != null)
            {
                Handler.OnCompleteClose();
            }
        }

#endregion

#region public methods

        /// <summary>
        /// バルーンのハンドラのセットを行う
        /// </summary>
        /// <param name="handler">ハンドラ</param>
        public virtual void SetHandler(IBalloonTransitionHandler handler)
        {
            if (Handler != null)
            {
                return;
            }

            Handler = handler;
        }

        /// <summary>
        /// バルーンのOpen()から呼ばれるOpenトランジション開始メソッド
        /// </summary>
        public abstract void OpenTransition();

        /// <summary>
        /// バルーンのClose()から呼ばれるCloseトランジション開始メソッド
        /// </summary>
        public abstract void CloseTransition();

#endregion
    }
}
