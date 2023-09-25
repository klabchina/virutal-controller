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

namespace Jigbox.NovelKit
{
    [System.Serializable]
    public class AdvSerializedExtendManagerList
    {
#region properties[SerializeField]

        /// <summary>拡張用管理コンポーネント</summary>
        [SerializeField]
        protected List<AdvExtendManagementComponent> managers;

#endregion

#region public methods

        /// <summary>
        /// 拡張用管理コンポーネントを取得します。
        /// </summary>
        /// <param name="identifier">識別用の文字列</param>
        /// <returns>取得できればコンポーネントの参照を、取得できなければ<c>null</c>を返します。</returns>
        public AdvExtendManagementComponent Get(string identifier)
        {
            foreach (AdvExtendManagementComponent manager in managers)
            {
                if (manager.Identifier == identifier)
                {
                    return manager;
                }
            }
            return null;
        }

        /// <summary>
        /// 拡張用管理コンポーネントを追加します。
        /// </summary>
        /// <param name="manager">拡張用管理コンポーネント</param>
        public void Add(AdvExtendManagementComponent manager)
        {
            if (!Contains(manager.Identifier))
            {
                managers.Add(manager);
            }
        }

        /// <summary>
        /// 拡張用管理コンポーネントを除外します。
        /// </summary>
        /// <param name="identifier">識別用の文字列</param>
        public void Remove(string identifier)
        {
            AdvExtendManagementComponent manager = Get(identifier);
            if (manager != null)
            {
                managers.Remove(manager);
            }
        }

        /// <summary>
        /// 拡張用管理コンポーネントにすでに含まれているかどうかを取得します。
        /// </summary>
        /// <param name="identifier">識別用の文字列</param>
        /// <returns>すでに含まれていれば<c>true</c>を、含まれていなければ<c>false</c>を返します。</returns>
        public bool Contains(string identifier)
        {
            return Get(identifier) != null;
        }

#endregion
    }
}
