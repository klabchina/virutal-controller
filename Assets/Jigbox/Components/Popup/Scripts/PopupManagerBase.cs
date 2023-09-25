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
    public abstract class PopupManagerBase : IPopupDisposer
    {
#region properties

        /// <summary>PopupViewを生成するためのプロバイダ</summary>
        protected IInstanceProvider<PopupView> viewProvider;

        /// <summary>PopupViewを生成するためのプロバイダ</summary>
        public IInstanceProvider<PopupView> ViewProvider { get { return viewProvider; } set { viewProvider = value; } }

        /// <summary>ポップアップを破棄するためのディスポーザ</summary>
        protected IInstanceDisposer<GameObject> disposer;

        /// <summary>ポップアップを破棄するためのディスポーザ</summary>
        public IInstanceDisposer<GameObject> Disposer { get { return disposer; } set { disposer = value; } }

#endregion

#region public methods

        /// <summary>
        /// <para>ポップアップを開きます。</para>
        /// <para>既に開いているポップアップが存在する場合は、最前面に表示されます。</para>
        /// </summary>
        /// <param name="order"></param>
        public virtual void Open(PopupOrder order)
        {
            PopupView view = GetView();
            if (view != null)
            {
                view.Open(order);
            }
        }

        /// <summary>
        /// <para>ポップアップを開きます。</para>
        /// <para>既に開いているポップアップが存在する場合は、ポップアップが閉じてから表示されます。</para>
        /// </summary>
        /// <param name="order"></param>
        public virtual void OpenQueue(PopupOrder order)
        {
            PopupView view = GetView();
            if (view != null)
            {
                view.OpenQueue(order);
            }
        }

        /// <summary>
        /// 開いている最前面のポップアップを閉じます。
        /// </summary>
        public virtual void Close()
        {
            PopupView view = GetView();
            if (view != null)
            {
                view.Close();
            }
        }

        /// <summary>
        /// 開いているポップアップを全て閉じます。
        /// </summary>
        public virtual void CloseAll()
        {
            PopupView view = GetView();
            if (view != null)
            {
                view.CloseAll();
            }
        }

        /// <summary>
        /// 未表示のポップアップの表示要求をクリアします。
        /// </summary>
        public virtual void ClearOrder()
        {
            PopupView view = GetView();
            if (view != null)
            {
                view.ClearOrder();
            }
        }

        /// <summary>
        /// ポップアップを破棄します。
        /// </summary>
        /// <param name="popup">破棄するポップアップ</param>
        public virtual void Dispose(PopupBase popup)
        {
            if (disposer == null)
            {
                disposer = new DefaultInstanceDisposer();
            }

            disposer.Dispose(popup.gameObject);
        }

        /// <summary>
        /// ポップアップが現在開いているかどうかを返します
        /// </summary>
        /// <returns>ポップアップの開閉状態</returns>
        public virtual bool IsOpenPopup()
        {
            var view = GetView();

            if (view == null)
            {
                return false;
            }

            return view.IsOpenPopup();
        }

        /// <summary>
        /// ポップアップが開かれる予定かどうかを返します
        /// </summary>
        /// <returns>ポップアップが開かれる予定かどうか</returns>
        public virtual bool IsStandbyPopup()
        {
            var view = GetView();

            if (view == null)
            {
                return false;
            }

            return view.IsStandbyPopup();
        }

#endregion

#region protected methods

        /// <summary>
        /// PopupViewを取得します。
        /// </summary>
        /// <returns></returns>
        protected abstract PopupView GetView();

#endregion
    }
}
