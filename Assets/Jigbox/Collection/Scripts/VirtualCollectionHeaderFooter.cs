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
using UnityEngine;

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public class VirtualCollectionHeaderFooter : MonoBehaviour
    {
        /// <summary>VirtualCollectionHeaderProxy</summary>
        [SerializeField]
        [HideInInspector]
        protected VirtualCollectionHeaderProxy headerProxy = new VirtualCollectionHeaderProxy();

        /// <summary>VirtualCollectionFooterProxy</summary>
        [SerializeField]
        [HideInInspector]
        protected VirtualCollectionFooterProxy footerProxy = new VirtualCollectionFooterProxy();

        /// <summary>ヘッダのpadding</summary>
        [SerializeField]
        [HideInInspector]
        protected Padding headerPadding;

        /// <summary>フッタのpadding</summary>
        [SerializeField]
        [HideInInspector]
        protected Padding footerPadding;

        /// <summary>ヘッダ・フッタの親となるRectTransform</summary>
        [SerializeField]
        [HideInInspector]
        protected RectTransform contentTrans;

        /// <summary>
        /// ヘッダのpaddingを参照/指定します
        /// </summary>
        public Padding HeaderPadding
        {
            get { return headerPadding; }
            set { headerPadding = value; }
        }

        /// <summary>
        /// フッタのpaddingを参照/指定します
        /// </summary>
        public Padding FooterPadding
        {
            get { return footerPadding; }
            set { footerPadding = value; }
        }

        /// <summary>
        /// 任意の方法でヘッダとなるGameObjectを渡すプロバイダーを参照/指定します
        /// </summary>
        public virtual IInstanceProvider<GameObject> HeaderProvider
        {
            get { return headerProxy.Provider; }
            set { headerProxy.Provider = value; }
        }

        /// <summary>
        /// 任意の方法でヘッダを処分するディスポーザを参照/指定します
        /// </summary>
        public virtual IInstanceDisposer<GameObject> HeaderDisposer
        {
            get { return headerProxy.Disposer; }
            set { headerProxy.Disposer = value; }
        }

        /// <summary>
        /// 任意の方法でフッタとなるGameObjectを渡すプロバイダーを参照/指定します 
        /// </summary>
        public virtual IInstanceProvider<GameObject> FooterProvider
        {
            get { return footerProxy.Provider; }
            set { footerProxy.Provider = value; }
        }

        /// <summary>
        /// 任意の方法でヘッダを処分するディスポーザを参照/指定します
        /// </summary>
        public virtual IInstanceDisposer<GameObject> FooterDisposer
        {
            get { return footerProxy.Disposer; }
            set { footerProxy.Disposer = value; }
        }

        /// <summary>
        /// ヘッダの幅のサイズを参照します
        /// </summary>
        /// <remarks>
        /// 横方向に並ぶ場合は横幅(width)、縦方向に並ぶ場合は縦幅(height)として用いられます
        /// </remarks>
        public virtual float HeaderSize { get { return headerProxy.GetViewSize(headerPadding); } }

        /// <summary>
        /// フッタの幅のサイズを参照します
        /// </summary>
        /// <remarks>
        /// 横方向に並ぶ場合は横幅(width)、縦方向に並ぶ場合は縦幅(height)として用いられます
        /// </remarks>
        public virtual float FooterSize { get { return footerProxy.GetViewSize(footerPadding); } }

        /// <summary>
        /// ヘッダとフッタの幅の合計サイズを参照します
        /// </summary>
        /// <remarks>
        /// 横方向に並ぶ場合は横幅(width)、縦方向に並ぶ場合は縦幅(height)として用いられます
        /// </remarks>
        public virtual float Size { get { return HeaderSize + FooterSize; } }

#region Public Method

        /// <summary>
        /// ヘッダ・フッタの配置を更新します
        /// VirtualCollectionViewから呼ばれることを想定しています
        /// </summary>
        public virtual void Relayout()
        {
            headerProxy.UpdatePosition(headerPadding);
            footerProxy.UpdatePosition(footerPadding);
        }

        /// <summary>
        /// ヘッダを設定します
        /// </summary>
        public virtual void SetHeader()
        {
            headerProxy.SetViewObject(contentTrans, headerPadding);
        }

        /// <summary>
        /// ヘッダを設定します
        /// </summary>
        public virtual void SetHeader(GameObject headerGo)
        {
            headerProxy.SetViewObject(headerGo, contentTrans, headerPadding);
        }

        /// <summary>
        /// ヘッダを設定します
        /// </summary>
        public virtual void SetHeader(Func<GameObject> provider)
        {
            headerProxy.SetViewObject(provider, contentTrans, headerPadding);
        }

        /// <summary>
        /// フッタを設定します
        /// </summary>
        public virtual void SetFooter()
        {
            footerProxy.SetViewObject(contentTrans, footerPadding);
        }
        
        /// <summary>
        /// フッタを設定します
        /// </summary>
        public virtual void SetFooter(GameObject headerGo)
        {
            footerProxy.SetViewObject(headerGo, contentTrans, footerPadding);
        }

        /// <summary>
        /// フッタを設定します
        /// </summary>
        public virtual void SetFooter(Func<GameObject> provider)
        {
            footerProxy.SetViewObject(provider, contentTrans, footerPadding);
        }

        /// <summary>
        /// ヘッダを削除します
        /// </summary>
        public virtual void RemoveHeader()
        {
            headerProxy.Remove();
        }

        /// <summary>
        /// フッタを削除します
        /// </summary>
        public virtual void RemoveFooter()
        {
            footerProxy.Remove();
        }
#endregion
    }
}
