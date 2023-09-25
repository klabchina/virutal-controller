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

namespace Jigbox.UIControl
{
    using SubmitGroup = SubmitMediator.SubmitGroup;

    public class SubmitLockInfo
    {
#region properties

        /// <summary>入力ロックの確認用クラス</summary>
        protected SubmitColleague colleague;

        /// <summary>ID</summary>
        public int Id { get { return colleague.Id; } }

        /// <summary>入力グループ</summary>
        public SubmitGroup Group { get { return colleague.Group; } }

        /// <summary>アンロック後に該当グループが再度ロック可能になるまでの待機時間</summary>
        protected float CoolTime { get { return colleague.CoolTime; } }

        /// <summary>アンロックされた時間</summary>
        protected float unlockedTime = 0.0f;

        /// <summary>待機時間の経過待ちを行っているかどうか</summary>
        public bool IsWaitCoolTime { get { return unlockedTime > 0.0f; } }

        /// <summary>待機時間が経過したかどうか</summary>
        public bool IsElapsedCoolTime { get { return unlockedTime + CoolTime <= Time.realtimeSinceStartup; } }

        /// <summary>自動的にアンロックされた際のコールバック</summary>
        protected Action autoUnlockCallback;
        
#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="colleague"></param>
        /// <param name="autoUnlockCallback">自動的にアンロックされた際のコールバック</param>
        public SubmitLockInfo(SubmitColleague colleague, Action autoUnlockCallback)
        {
            this.colleague = colleague;
            this.autoUnlockCallback = autoUnlockCallback;
        }

        /// <summary>
        /// アンロックされたことを設定します。
        /// </summary>
        public void Unlock()
        {
            unlockedTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 自動的にアンロックされたことを設定します。
        /// </summary>
        public void AutoUnlock()
        {
            autoUnlockCallback();
        }

#endregion
    }
}
