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
    public class AdvPlayModeManager
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 再生モード
        /// </summary>
        public enum PlayMode
        {
            /// <summary>通常</summary>
            Normal,
            /// <summary>自動再生</summary>
            Auto,
            /// <summary>スキップ</summary>
            Skip,
        }

#endregion

#region properties

        /// <summary>自動再生中かどうか</summary>
        public bool IsAuto { get { return mode == PlayMode.Auto; } }

        /// <summary>スキップ中かどうか</summary>
        public bool IsSkip { get { return mode == PlayMode.Skip; } }

        /// <summary>コマンドが実行可能かどうか</summary>
        public bool IsExecute { get { return !isWait; } }

        /// <summary>現在の再生モード</summary>
        public PlayMode CurrentMode { get { return mode; } }

        /// <summary>シナリオの状態管理コンポーネント</summary>
        protected AdvScenarioStatusManager statusManager;

        /// <summary>シナリオの再生モード</summary>
        protected PlayMode mode = PlayMode.Normal;

        /// <summary>自動再生状態での次のテキストへの待ち時間</summary>
        protected float autoPlayWait = 0.0f;

        /// <summary>待機状態になってからの経過時間</summary>
        protected float deltaTime = 0.0f;

        /// <summary>待機状態かどうか</summary>
        protected bool isWait = false;

        /// <summary>再生モードが切り替わった際のコールバック</summary>
        protected Action<PlayMode> modeChangeCallback;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="statusManager">シナリオの状態管理コンポーネント</param>
        public AdvPlayModeManager(AdvScenarioStatusManager statusManager)
        {
            this.statusManager = statusManager;
        }

        /// <summary>
        /// 再生モードが切り替わった際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">再生モードが切り替わった際に呼び出されるコールバック</param>
        public void AddModeChangeCallback(Action<PlayMode> callback)
        {
            if (modeChangeCallback == null)
            {
                modeChangeCallback = callback;
            }
            else
            {
                modeChangeCallback += callback;
            }
        }

        /// <summary>
        /// 自動再生状態での次のテキストへの待ち時間を設定します。
        /// </summary>
        /// <param name="waitTime">自動再生状態での次のテキストへの待ち時間</param>
        public void SetAutoPlayWait(float waitTime)
        {
            autoPlayWait = waitTime;
        }

        /// <summary>
        /// 待機状態に設定します。
        /// </summary>
        public void Wait()
        {
            isWait = true;
            deltaTime = 0.0f;
        }

        /// <summary>
        /// <para>待機状態を解除します。</para>
        /// <para>基本的にオート時、クリック待ちかつテキスト表示が終了している状態で呼び出されます。</para>
        /// </summary>
        public void WaitRelease()
        {
            isWait = false;
            deltaTime = 0.0f;
        }

        /// <summary>
        /// <para>再生モードを設定します。</para>
        /// <para>現在の再生モードと同様のモードを設定した場合、モードは通常状態へと切り替わります。</para>
        /// </summary>
        /// <param name="mode"></param>
        public void SwitchMode(PlayMode mode)
        {
            PlayMode last = this.mode;

            // 同一モードで設定した場合は、通常状態に戻る
            if (this.mode == mode)
            {
                this.mode = PlayMode.Normal;
            }
            else
            {
                this.mode = mode;
            }

            if (this.mode == PlayMode.Normal)
            {
                WaitRelease();
            }

            if (this.mode != last && modeChangeCallback != null)
            {
                modeChangeCallback(this.mode);
            }
        }
        
        /// <summary>
        /// 更新を行います。
        /// </summary>
        public virtual void Update()
        {
            if (!statusManager.State.IsExecute)
            {
                // 待機状態に選択肢が含まれる場合は、待機状態の解除後に待ちを挟む必要がないのでフラグを切っておく
                if (isWait && statusManager.State.IsWait(AdvPlayStateManager.PlayState.Select))
                {
                    isWait = false;
                }
                return;
            }

            if (isWait)
            {
                deltaTime += Time.deltaTime;
                if (deltaTime >= autoPlayWait)
                {
                    isWait = false;
                    deltaTime = 0.0f;
                }
            }
        }

#endregion
    }
}
