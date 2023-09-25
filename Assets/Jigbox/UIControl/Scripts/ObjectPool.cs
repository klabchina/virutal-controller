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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.UIControl
{
    /// <summary>
    /// オブジェクトのプーリングを行う基底クラス
    /// </summary>
    /// <typeparam name="T">プール対象</typeparam>
    public class ObjectPool<T>
    {
#region fields & properties

        /// <summary>
        /// 使用可能なインスタンス
        /// </summary>
        protected Stack<T> free = new Stack<T>();

        /// <summary>
        /// 使用されているインスタンス
        /// </summary>
        protected HashSet<T> used = new HashSet<T>();

        /// <summary>
        /// 使用可能なインスタンスの参照
        /// </summary>
        public virtual Stack<T> Free
        {
            get { return free; }
        }

        /// <summary>
        /// 使用されているインスタンスの参照
        /// </summary>
        public virtual HashSet<T> Used
        {
            get { return used; }
        }

        /// <summary>
        /// プーリングされているインスタンスの総数
        /// </summary>
        public virtual int PoolCount
        {
            get { return free.Count + used.Count; }
        }

        /// <summary>
        /// Takeメソッドでインスタンスを取得できるかどうか
        /// </summary>
        public virtual bool IsAvailableTake
        {
            get { return free.Count != 0; }
        }

#endregion

#region public methods

        /// <summary>
        /// 外部から使用可能なインスタンスの参照を貰い、プールとして管理します
        /// </summary>
        /// <param name="instance">使用可能なインスタンス</param>
        public virtual void Bring(T instance)
        {
            free.Push(instance);
        }

        /// <summary>
        /// 使用可能なインスタンスを取得し、使用済みにします
        /// </summary>
        /// <returns>使用可能なインスタンス</returns>
        public virtual T Take()
        {
            if (free.Count == 0)
            {
                Debug.LogError("it's not found that available instance in pool, use Bring methods.");
                return default(T);
            }

            var instance = free.Pop();
            used.Add(instance);

            return instance;
        }

        /// <summary>
        /// 使用していたインスタンスを返却します
        /// </summary>
        /// <param name="instance"></param>
        public virtual void Release(T instance)
        {
            if (!used.Contains(instance))
            {
                Debug.LogError("it's not found that instance in an used instance list.");
                return;
            }

            used.Remove(instance);
            free.Push(instance);
        }

        /// <summary>
        /// 使用していたインスタンスを全て解放します
        /// </summary>
        public virtual void ReleaseAll()
        {
            foreach (var instance in used)
            {
                free.Push(instance);
            }

            used.Clear();
        }

#endregion
    }
}
