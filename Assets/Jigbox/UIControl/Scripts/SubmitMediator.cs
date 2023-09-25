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
using System;
using System.Collections.Generic;

namespace Jigbox.UIControl
{
    public class SubmitMediator : MonoBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>入力グループ</summary>
        /// <remarks>
        /// 現在のグループ数はスマートフォンのマルチタッチの上限数を決めとして設定しています。
        /// 列挙子の名前に意味はありません。
        /// </remarks>
        public enum SubmitGroup
        {
            _1,
            _2,
            _3,
            _4,
            _5,
            _6,
            _7,
            _8,
            _9,
            _10,
        }

#endregion

#region constants

        /// <summary>入力がなくなってから自動的にアンロックが行われるまでの時間</summary>
        protected static readonly float AutoUnlockTime = 0.5f;

#endregion

#region properties

        /// <summary>インスタンス</summary>
        static SubmitMediator instance;

        /// <summary>インスタンス</summary>
        public static SubmitMediator Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("SubmitMediator");
                    instance = obj.AddComponent<SubmitMediator>();
                    obj.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                }
                return instance;
            }
        }

        /// <summary>入力グループのロック情報</summary>
        protected List<SubmitLockInfo> lockList = new List<SubmitLockInfo>();

        /// <summary>入力がなくなってからの経過時間</summary>
        protected float noneInputDeltaTime = 0.0f;

        /// <summary>ロック済みのグループがいくつ存在するか</summary>
        public int LockCount { get { return lockList.Count; } }

#endregion

#region public methods

        /// <summary>
        /// ロックを取得します。
        /// </summary>
        /// <param name="colleague">入力ロックの確認用クラス</param>
        /// <param name="group"></param>
        /// <param name="autoUnlockCallback"></param>
        /// <returns>ロックに成功した場合、trueを返します。</returns>
        public bool GetLock(SubmitColleague colleague, SubmitGroup group, Action autoUnlockCallback)
        {
            if (autoUnlockCallback == null)
            {
#if UNITY_EDITOR
                Debug.LogError("SubmitMediator.GetLock : Callback is not allowed null!");
#endif
                return false;
            }

            foreach (SubmitLockInfo info in lockList)
            {
                if (info.Group == group)
                {
                    return false;
                }
            }
            lockList.Add(new SubmitLockInfo(colleague, autoUnlockCallback));
            return true;
        }

        /// <summary>
        /// アンロックします。
        /// </summary>
        /// <param name="colleague">入力ロックの確認用クラス</param>
        /// <returns>アンロックに成功した場合、trueを返します。</returns>
        public bool Unlock(SubmitColleague colleague)
        {
            for (int i = 0; i < lockList.Count; ++i)
            {
                SubmitLockInfo info = lockList[i];
                if (info.Id == colleague.Id && info.Group == colleague.Group)
                {
                    info.Unlock();
                    // 待機時間が0の場合は、この時点で再ロック可能となるため
                    // ロック中のリストから削除する
                    if (info.IsElapsedCoolTime)
                    {
                        lockList.RemoveAt(i);
                    }
                    return true;
                }
            }
            return false;
        }

#endregion
        
#region override unity methods

        protected virtual void Update()
        {
            if (lockList.Count == 0)
            {
                return;
            }

            List<SubmitLockInfo> unlockList = null;

            foreach (SubmitLockInfo info in lockList)
            {
                if (info.IsWaitCoolTime && info.IsElapsedCoolTime)
                {
                    if (unlockList == null)
                    {
                        unlockList = new List<SubmitLockInfo>();
                    }
                    unlockList.Add(info);
                }
            }

            int inputCount = 0;
            
#if UNITY_EDITOR || UNITY_STANDALONE
            inputCount = InputWrapper.GetMouseButton(0) ? 1 : 0;
#else
            inputCount = InputWrapper.GetTouchCount();
#endif

            // 無入力状態になっていて、ロックが残っている場合
            if (inputCount == 0)
            {
                noneInputDeltaTime += Time.deltaTime;
                if (noneInputDeltaTime >= AutoUnlockTime)
                {
                    foreach (SubmitLockInfo info in lockList)
                    {
                        // すでに待機時間待ちになっているものは自動アンロックしない
                        if (!info.IsWaitCoolTime)
                        {
                            if (unlockList == null)
                            {
                                unlockList = new List<SubmitLockInfo>();
                            }
                            info.AutoUnlock();
                            unlockList.Add(info);
                        }
                    }
                    noneInputDeltaTime = 0.0f;
                }
            }
            else
            {
                noneInputDeltaTime = 0.0f;
            }

            if (unlockList == null)
            {
                return;
            }

            foreach (SubmitLockInfo unlockInfo in unlockList)
            {
                lockList.Remove(unlockInfo);
            }
        }

#endregion
    }
}
