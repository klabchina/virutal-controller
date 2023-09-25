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

namespace Jigbox.NovelKit
{
    public class AdvPlayStateManager
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 再生状態
        /// </summary>
        public enum PlayState : int
        {
            /// <summary>通常</summary>
            Normal = 0,
            /// <summary>時間指定による待機状態</summary>
            WaitTime = 1,
            /// <summary>クリック待ちによる待機状態</summary>
            WaitClick = 1 << 1,
            /// <summary>テキスト表示待ちによる待機状態</summary>
            WaitTextEnd = 1 << 2,
            /// <summary>サウンドの終了待ちによる待機状態</summary>
            WaitSound = 1 << 3,
            /// <summary>選択肢選択待ちによる待機状態</summary>
            Select = 1 << 4,
            /// <summary>処理の一時中断状態</summary>
            Suspend = 1 << 5,
            /// <summary>リソースの事前読み込み中</summary>
            WaitPreload = 1 << 24,

            // 以下の領域は拡張のための予約分
            // 元々用意しているもの以外で公式にサポートする進行停止状態が追加された場合、
            // 以下の領域を使って定義を行うようにする
            [Obsolete("Can't use this, because it was prepared for expansion.")]
            _Reserved1 = 1 << 25,
            [Obsolete("Can't use this, because it was prepared for expansion.")]
            _Reserved2 = 1 << 26,
            [Obsolete("Can't use this, because it was prepared for expansion.")]
            _Reserved3 = 1 << 27,
            [Obsolete("Can't use this, because it was prepared for expansion.")]
            _Reserved4 = 1 << 28,
            [Obsolete("Can't use this, because it was prepared for expansion.")]
            _Reserved5 = 1 << 29,
            [Obsolete("Can't use this, because it was prepared for expansion.")]
            _Reserved6 = 1 << 30,
            [Obsolete("Can't use this, because it was prepared for expansion.")]
            _Reserved7 = 1 << 31,
        }

#endregion

#region properties
        
        /// <summary>コマンドが実行可能かどうか</summary>
        public virtual bool IsExecute { get { return state == (int) PlayState.Normal; } }
        
        /// <summary>シナリオの状態管理コンポーネント</summary>
        protected AdvScenarioStatusManager statusManager;

        /// <summary>シナリオの再生状態</summary>
        protected int state = (int) PlayState.Normal;

        /// <summary>再生状態の予約</summary>
        protected int reservationState = (int) PlayState.Normal;

        /// <summary>待機状態の時間</summary>
        protected float waitTime = 0.0f;

        /// <summary>待機状態になってからの経過時間</summary>
        protected float waitDeltaTime = 0.0f;

        /// <summary>自動再生に次のテキストへの進行待ちに移行する条件となる再生状態</summary>
        protected virtual int AutoProgressTriggerState { get { return (int) PlayState.WaitClick | (int) PlayState.WaitSound; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="statusManager">シナリオの状態管理コンポーネント</param>
        public AdvPlayStateManager(AdvScenarioStatusManager statusManager)
        {
            this.statusManager = statusManager;
        }

        /// <summary>
        /// 時間を指定して待機状態にします。
        /// </summary>
        /// <param name="seconds">待機状態にする時間(秒)</param>
        public void WaitTime(float seconds)
        {
            state |= (int) PlayState.WaitTime;
            waitTime = seconds;
            waitDeltaTime = 0.0f;
        }

        /// <summary>
        /// 状態を指定して待機状態にします。
        /// </summary>
        /// <param name="state">状態</param>
        /// <param name="isReservation">予約かどうか</param>
        public void Wait(PlayState state, bool isReservation = false)
        {
            Wait((int) state, isReservation);
        }

        /// <summary>
        /// 状態を指定して待機状態にします。
        /// </summary>
        /// <param name="state">状態</param>
        /// <param name="isReservation">予約かどうか</param>
        public void Wait(int state, bool isReservation = false)
        {
            if (!isReservation)
            {
                this.state |= state;
            }
            else
            {
                reservationState |= state;
            }
        }

        /// <summary>
        /// 指定した状態の待機を解除します。
        /// </summary>
        /// <param name="state">状態</param>
        /// <param name="isReservation">予約かどうか</param>
        public void WaitRelease(PlayState state, bool isReservation = false)
        {
            WaitRelease((int) state, isReservation);
        }

        /// <summary>
        /// 指定した状態の待機を解除します。
        /// </summary>
        /// <param name="state">状態</param>
        /// <param name="isReservation">予約かどうか</param>
        public void WaitRelease(int state, bool isReservation = false)
        {
            if (!isReservation)
            {
                bool isWaitModeManager = false;
                if (statusManager.Mode.IsAuto)
                {
                    isWaitModeManager = IsWait(AutoProgressTriggerState);
                }

                this.state &= ~state;

                // 自動再生中に、クリック待ちやサウンドの終了待ち状態だった状態から、
                // その状態が解除されて、次のコマンドが実行可能になった際には
                // 自動再生時の待機時間分の間隔を開けるための処理を走らせるために
                // 状態管理モジュールを待機状態に設定する
                if (IsExecute && isWaitModeManager)
                {
                    statusManager.Mode.Wait();
                }
            }
            else
            {
                reservationState &= ~state;
            }
        }

        /// <summary>
        /// 予約状態になっているの再生状態を確定させます。
        /// </summary>
        /// <param name="state">状態</param>
        public void ConfirmReservation(PlayState state)
        {
            ConfirmReservation((int) state);
        }

        /// <summary>
        /// 予約状態になっているの再生状態を確定させます。
        /// </summary>
        /// <param name="state">状態</param>
        public void ConfirmReservation(int state)
        {
            if (IsWait(state, true))
            {
                Wait(state);
                WaitRelease(state, true);
            }
        }

        /// <summary>
        /// サウンドの再生が終了した際に呼び出されます。
        /// </summary>
        public void OnEndSound()
        {
            bool isReserved = IsWait(PlayState.WaitSound, true);
            WaitRelease(PlayState.WaitSound, isReserved);
        }

        /// <summary>
        /// 指定した状態で待機しているかどうかを返します。
        /// </summary>
        /// <param name="state">状態</param>
        /// <param name="isReservation">予約かどうか</param>
        /// <returns></returns>
        public bool IsWait(PlayState state, bool isReservation = false)
        {
            return IsWait((int) state, isReservation);
        }

        /// <summary>
        /// 指定した状態で待機しているかどうかを返します。
        /// </summary>
        /// <param name="state">状態</param>
        /// <param name="isReservation">予約かどうか</param>
        /// <returns></returns>
        public bool IsWait(int state, bool isReservation = false)
        {
            int checkState = !isReservation ? this.state : reservationState;
            return (checkState & state) > 0;
        }

        /// <summary>
        /// 更新を行います。
        /// </summary>
        public virtual void Update()
        {
            if (IsWait(PlayState.WaitTime))
            {
                waitDeltaTime += Time.deltaTime;
                if (waitDeltaTime >= waitTime)
                {
                    waitTime = 0.0f;
                    waitDeltaTime = 0.0f;
                    state &= (int) ~PlayState.WaitTime;
                }
            }
        }

#endregion
    }
}
