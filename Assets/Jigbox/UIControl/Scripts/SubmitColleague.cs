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
    
    [Serializable]
    public class SubmitColleague
    {
#region properties
        
        /// <summary>ID</summary>
        [SerializeField]
        protected int id = 0;

        /// <summary>ID</summary>
        public int Id { get { return id; } }

        /// <summary>入力グループ</summary>
        [SerializeField]
        protected SubmitGroup group = SubmitGroup._1;
        
        /// <summary>入力のグループ</summary>
        public SubmitGroup Group { get { return group; } }

        /// <summary>アンロック後に該当グループが再度ロック可能になるまでの待機時間</summary>
        [SerializeField]
        protected float coolTime = 0.0f;

        /// <summary>アンロック後に該当グループが再度ロック可能になるまでの待機時間</summary>
        public float CoolTime { get { return coolTime; } }
        
        /// <summary>ロックが有効かどうか</summary>
        public bool IsEnable { get; protected set; }

        /// <summary>自動アンロックされた際のコールバック</summary>
        protected Action onAutoUnlock = null;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="target">制御を行いたいコンポーネントの参照</param>
        /// <param name="group">入力グループ</param>
        /// <param name="coolTime">アンロック後に該当グループが再度ロック可能になるまでの待機時間</param>
        public SubmitColleague(Component target, SubmitGroup group = SubmitGroup._1, float coolTime = 0.0f)
        {
            id = target.GetInstanceID();
            this.group = group;
            this.coolTime = coolTime;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">制御用のID(※必ずユニークであること)</param>
        /// <param name="group">ロックするグループ</param>
        /// <param name="coolTime">アンロック後に該当グループが再度ロック可能になるまでの待機時間</param>
        public SubmitColleague(int id, SubmitGroup group = SubmitGroup._1, float coolTime = 0.0f)
        {
            this.id = id;
            this.group = group;
            this.coolTime = coolTime;

        }
        
        /// <summary>
        /// <para>ロックを行います。</para>
        /// <para>ロックに成功した場合、IsEnableがtrueになります。</para>
        /// </summary>
        public void GetLock()
        {
            GetLock(group, coolTime);
        }

        /// <summary>
        /// <para>ロックを行います。</para>
        /// <para>ロックに成功した場合、IsEnableがtrueになります。</para>
        /// </summary>
        /// <param name="group">ロックするグループ</param>
        /// <param name="coolTime">アンロック後に該当グループが再度ロック可能になるまでの待機時間</param>
        public void GetLock(SubmitGroup group, float coolTime)
        {
            if (IsEnable)
            {
                return;
            }

            IsEnable = SubmitMediator.Instance.GetLock(this, group, OnAutoUnlock);
            if (IsEnable)
            {
                // アンロック後に再ロック可能になるまでの待機時間を設定できる関係上、
                // ロック中に時間やグループを変更されると管理が破綻するため、
                // インスタンス生成時以外ではロックに成功したタイミングでのみ、
                // これらのパラメータを変更可能としている
                this.group = group;
                this.coolTime = coolTime;
            }
        }

        /// <summary>
        /// ロックを解除します。
        /// </summary>
        public void Unlock()
        {
            if (!IsEnable)
            {
                return;
            }

            IsEnable = !SubmitMediator.Instance.Unlock(this);
        }

        /// <summary>
        /// 自動アンロックされた際のコールバックを設定します。
        /// </summary>
        /// <param name="onAutoUnlock"></param>
        public void SetAutoUnlockCallback(Action onAutoUnlock)
        {
            this.onAutoUnlock = onAutoUnlock;
        }

#endregion

#region protected methods

        /// <summary>
        /// 自動アンロックされた際に呼び出されます。
        /// </summary>
        protected void OnAutoUnlock()
        {
            IsEnable = false;
            if (onAutoUnlock != null)
            {
                onAutoUnlock();
            }
        }
        
#endregion
    }
}
