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
    /// <summary>
    /// ヘッダ用オブジェクトとの処理を中継するクラスです
    /// </summary>
    public class VirtualCollectionHeaderProxy : VirtualCollectionHeaderFooterProxy<VirtualCollectionHeader>
    {
    }
    
    /// <summary>
    /// フッタ用オブジェクトとの処理を中継するクラスです
    /// </summary>
    public class VirtualCollectionFooterProxy : VirtualCollectionHeaderFooterProxy<VirtualCollectionFooter>
    {
    }

    public abstract class VirtualCollectionHeaderFooterProxy<T>  where T : VirtualCollectionHeaderFooterObject
    {

        /// <summary>管理下にあるヘッダorフッタ用オブジェクト</summary>
        VirtualCollectionHeaderFooterObject managedHeaderFooter;

        protected IInstanceProvider<GameObject> provider;

        protected IInstanceDisposer<GameObject> disposer = new DefaultInstanceDisposer();

        /// <summary>
        /// 任意の方法でGameObjectを渡すプロバイダーを参照/指定します
        /// </summary>
        public virtual IInstanceProvider<GameObject> Provider
        {
            get
            {
                if (provider == null)
                {
                    provider = new InstanceProvider<GameObject>();
                }
                return provider;
            }
            set { provider = value;  }
        }

        /// <summary>
        /// 任意の方法でGameObjectを処分するディスポーザを参照/指定します
        /// </summary>
        public virtual IInstanceDisposer<GameObject> Disposer
        {
            get { return disposer; }
            set { disposer = value; }
        }

        /// <summary>
        /// フィールドにあるproviderから生成されるGameObjectを管理下に置きます
        /// </summary>
        /// <param name="contentTrans"></param>
        /// <param name="padding"></param>
        public virtual void SetViewObject(RectTransform contentTrans, Padding padding)
        {
            if (provider == null)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("[{0}] Provider can't provide {1} instance.", GetType(), typeof(T));
#endif
                return;
            }
            SetViewObject(provider.Generate(), contentTrans, padding);
        }

        /// <summary>
        /// 引数より渡されたproviderから生成されるGameObjectを管理下に置きます
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="contentTrans"></param>
        /// <param name="padding"></param>
        public virtual void SetViewObject(Func<GameObject> provider, RectTransform contentTrans, Padding padding)
        {
            if (provider == null)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("[{0}] {1} provider is null.", GetType(), typeof(T));
#endif
                return;
            }
            SetViewObject(provider(), contentTrans, padding);
        }

        /// <summary>
        /// 引数で渡されたGameObjectを管理下に置きます
        /// </summary>
        /// <param name="go"></param>
        /// <param name="contentTrans"></param>
        /// <param name="padding"></param>
        public virtual void SetViewObject(GameObject go, RectTransform contentTrans, Padding padding)
        {
            if (go == null)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("[{0}] Object is null.", GetType());
#endif
                return;
            }
            // 同じものをセットしようとしている場合は何もしない
            if (managedHeaderFooter != null && managedHeaderFooter.gameObject == go)
            {
                return;
            }
            // セットされていたObjectを削除してからSetする
            Remove();
            managedHeaderFooter = go.GetComponent<T>();
            if (managedHeaderFooter == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("[{0}] {1} hasn't {2}.", GetType(), go.name, typeof(T));
#endif
                return;
            }
            managedHeaderFooter.SetParent(contentTrans, padding);
        }

        /// <summary>
        /// paddingを含めたサイズを返します
        /// managedHeaderFooterがない場合はサイズは0とします
        /// </summary>
        /// <param name="padding"></param>
        /// <returns></returns>
        public float GetViewSize(Padding padding)
        {
            if (managedHeaderFooter == null)
            {
                return 0f;
            }
            return managedHeaderFooter.GetViewSize(padding);
        }

        /// <summary>
        /// オブジェクトの位置を更新します
        /// </summary>
        /// <param name="padding"></param>
        public virtual void UpdatePosition(Padding padding)
        {
            if (managedHeaderFooter == null)
            {
                return;
            }
            managedHeaderFooter.UpdateTransform(padding);
        }

        /// <summary>
        /// 管理下にあるオブジェクトを削除します
        /// </summary>
        public void Remove()
        {
            if (managedHeaderFooter == null)
            {
                return;
            }
            Disposer.Dispose(managedHeaderFooter.gameObject);
            managedHeaderFooter = null;
        }
    }
}
