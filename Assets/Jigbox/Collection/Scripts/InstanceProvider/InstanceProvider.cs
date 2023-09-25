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

namespace Jigbox
{
    /// <summary>
    /// インスタンスを生成するプロバイダーインターフェース
    /// </summary>
    public interface IInstanceProvider<T> where T : Object
    {
        /// <summary>
        /// インスタンスを生成します
        /// </summary>
        T Generate();
    }

    /// <summary>
    /// IInstanceProvider のデフォルト実装クラス
    /// </summary>
    public class InstanceProvider<T> : IInstanceProvider<T> where T : Object
    {
        public static InstanceProvider<T> FromResourcePath(string path)
        {
            return new InstanceProvider<T>(path);
        }

        protected T prefab;

        public virtual T Prefab
        {
            get { return prefab; }
            set { prefab = value; }
        }

        public InstanceProvider()
        {
        }

        public InstanceProvider(T prefab)
        {
            this.prefab = prefab;
        }

        public InstanceProvider(string path)
        {
            this.prefab = Resources.Load<T>(path);
        }

        /// <summary>
        /// リソースのパスからPrefabを読み込みます
        /// </summary>
        /// <returns>The from resource path.</returns>
        /// <param name="path">Path.</param>
        public virtual T LoadFromResourcePath(string path)
        {
            return prefab = Resources.Load<T>(path);
        }

        /// <summary>
        /// インスタンスを生成します
        /// </summary>
        public virtual T Generate()
        {
            if (prefab == null)
            {
                throw new MissingReferenceException("InstanceProvider have not any reference of prefab");
            }
            return Object.Instantiate(prefab);
        }
    }
}
