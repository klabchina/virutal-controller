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

using Callback = Jigbox.Delegatable.DelegatableObject.Callback;

namespace Jigbox.Delegatable
{
    [System.Serializable]
    public class DelegatableList
    {
#region properties

        /// <summary>デリゲート用オブジェクトのリスト</summary>
        [SerializeField]
        List<DelegatableObject> delegatableList = new List<DelegatableObject>();

        /// <summary>デリゲート用オブジェクトのリスト</summary>
        public List<DelegatableObject> List { get { return delegatableList; } }

        /// <summary>要素数</summary>
        public int Count { get { return List.Count; } }

#endregion

#region public methods

        /// <summary>
        /// 全てのイベントを実行します。
        /// </summary>
        public void Execute()
        {
            DelegatableObject.Executes(delegatableList);
        }

        /// <summary>
        /// 全てのイベントを実行します。
        /// </summary>
        /// <param name="parameter">特定の型の引数が存在した場合のパラメータ</param>
        public void Execute<T>(T parameter)
        {
            DelegatableObject.Executes<T>(delegatableList, parameter);
        }

        /// <summary>
        /// 全てのイベントを実行します。
        /// </summary>
        /// <param name="args">引数リスト</param>
        public void Execute(params object[] args)
        {
            DelegatableObject.Executes(delegatableList, args);
        }

        /// <summary>
        /// インデックスを指定して要素を取得します。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns></returns>
        public DelegatableObject Get(int index)
        {
            return delegatableList[index];
        }

        /// <summary>
        /// インデックスを指定して要素を設定します。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <param name="delegatable">デリゲート用オブジェクト</param>
        public void Set(int index, DelegatableObject delegatable)
        {
            delegatableList[index] = delegatable;
        }

        /// <summary>
        /// 指定したデリゲート用オブジェクトがリストに含まれるかを返します。
        /// </summary>
        /// <param name="del">デリゲート用オブジェクト</param>
        /// <returns>リストに含まれていた場合、trueを返します。</returns>
        public bool Contains(DelegatableObject del)
        {
            return delegatableList.Contains(del);
        }

        /// <summary>
        /// 指定したデリゲートを持つデリゲート用オブジェクトがリストに含まれるかを返します。
        /// </summary>
        /// <param name="callback">デリゲート</param>
        /// <returns>リストに含まれていた場合、trueを返します。</returns>
        public bool Contains(Callback callback)
        {
            if (callback == null)
            {
                return false;
            }

            for (int i = 0; i < delegatableList.Count; ++i)
            {
                if (delegatableList[i].Equals(callback))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 指定した対象、メソッド名を持つデリゲート用オブジェクトがリストに含まれるかを返します。
        /// </summary>
        /// <param name="target">イベントの発行対象</param>
        /// <param name="methodName">メソッド名</param>
        /// <returns>リストに含まれていた場合、trueを返します。</returns>
        public bool Contains(MonoBehaviour target, string methodName)
        {
            if (target == null || string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            DelegatableObject del = new DelegatableObject();
            del.SetEvent(target, methodName);
            for (int i = 0; i < delegatableList.Count; ++i)
            {
                if (delegatableList[i].Equals(del))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// デリゲート用オブジェクトを追加します。
        /// </summary>
        /// <param name="del">デリゲート用オブジェクト</param>
        /// <returns>追加に成功した場合、trueを返します。</returns>
        public bool Add(DelegatableObject del)
        {
            if (del == null)
            {
                return false;
            }
            if (del.Target != null || del.IsValid)
            {
                delegatableList.Add(del);
                return true;
            }
            return false;
        }

        /// <summary>
        /// デリゲートを追加します。
        /// </summary>
        /// <param name="callback">デリゲート</param>
        /// <returns>追加に成功した場合、trueを返します。</returns>
        public bool Add(Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("DelegatableList.Add : Can't call the override! The method is enable when playing!");
                return false;
            }
#endif

            if (callback == null)
            {
                return false;
            }
            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(callback);
            if (delegatable.IsValid)
            {
                delegatableList.Add(delegatable);
                return true;
            }
            return false;
        }

        /// <summary>
        /// デリゲートを追加します。
        /// </summary>
        /// <param name="target">イベントの発行対象</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="args">引数</param>
        /// <returns>追加に成功した場合、trueを返します。</returns>
        public bool Add(Object target, string methodName, params object[] args)
        {
            if (target == null || string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(target, methodName, args);
            if (delegatable.IsValid)
            {
                delegatableList.Add(delegatable);
                return true;
            }
            return false;
        }

        /// <summary>
        /// デリゲートを追加します。
        /// </summary>
        /// <param name="eventDelegate">デリゲート</param>
        /// <returns></returns>
        public bool Add(IEventDelegate eventDelegate)
        {
            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(eventDelegate);
            if (delegatable.IsValid)
            {
                delegatableList.Add(delegatable);
                return true;
            }
            return false;
        }

        /// <summary>
        /// デリゲート用オブジェクトを破棄します。
        /// </summary>
        /// <param name="delegatable">デリゲート用オブジェクト</param>
        /// <returns>破棄に成功した場合、trueを返します。</returns>
        public bool Remove(DelegatableObject delegatable)
        {
            if (delegatable == null)
            {
                return false;
            }

            if (delegatableList.Contains(delegatable))
            {
                delegatableList.Remove(delegatable);
                return true;
            }
            return false;
        }

        /// <summary>
        /// デリゲート用オブジェクトを破棄します。
        /// </summary>
        /// <param name="callback">デリゲート</param>
        /// <returns>破棄に成功した場合、trueを返します。</returns>
        public bool Remove(Callback callback)
        {
            if (callback == null)
            {
                return false;
            }

            for (int i = 0; i < delegatableList.Count; ++i)
            {
                if (delegatableList[i].Equals(callback))
                {
                    delegatableList.Remove(delegatableList[i]);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// デリゲート用オブジェクトを破棄します。
        /// </summary>
        /// <param name="target">イベントの発行対象</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="args">引数</param>
        /// <returns>破棄に成功した場合、trueを返します。</returns>
        public bool Remove(MonoBehaviour target, string methodName, params object[] args)
        {
            if (target == null || string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(target, methodName, args);
            for (int i = 0; i < delegatableList.Count; ++i)
            {
                if (delegatableList[i].Equals(delegatable))
                {
                    delegatableList.Remove(delegatableList[i]);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// デリゲート用オブジェクトを破棄します。
        /// </summary>
        /// <param name="callback">デリゲート</param>
        /// <returns>破棄に成功した場合、trueを返します。</returns>
        public bool Remove(IEventDelegate eventDelegate)
        {
            for (int i = 0; i < delegatableList.Count; ++i)
            {
                if (delegatableList[i].Equals(eventDelegate))
                {
                    delegatableList.Remove(delegatableList[i]);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// デリゲート用オブジェクトをクリアします。
        /// </summary>
        public void Clear()
        {
            delegatableList.Clear();
        }

#endregion
    }
}
