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

namespace Jigbox.NovelKit
{
    using PlayState = AdvPlayStateManager.PlayState;
    using PlayMode = AdvPlayModeManager.PlayMode;
    using TextInfo = AdvWindowTextController.TextInfo;

    public class AdvUIManager : MonoBehaviour, IAdvManagementComponent
    {
#region properties

        /// <summary>シナリオ制御統合コンポーネント</summary>
        protected AdvMainEngine engine;

        /// <summary>シナリオの状態管理コンポーネント</summary>
        protected AdvScenarioStatusManager statusManager;

        /// <summary>ウィンドウ管理コンポーネント</summary>
        [SerializeField]
        protected AdvWindowManager windowManager;

        /// <summary>ウィンドウ管理コンポーネント</summary>
        public AdvWindowManager WindowManager { get { return windowManager; } }

        /// <summary>バックログ管理コンポーネント</summary>
        [SerializeField]
        protected AdvBacklogManager backlogManager;

        /// <summary>バックログ管理コンポーネント</summary>
        public AdvBacklogManager BacklogManager { get { return backlogManager; } }

        /// <summary>選択肢管理コンポーネント</summary>
        [SerializeField]
        protected AdvSelectManager selectManager;

        /// <summary>選択肢管理コンポーネント</summary>
        public AdvSelectManager SelectManager { get { return selectManager; } }

        /// <summary>ボタンUI</summary>
        [SerializeField]
        protected List<GameObject> buttons;

        /// <summary>入力判定用</summary>
        [SerializeField]
        protected Components.ButtonBase touchHandler;

        /// <summary>クリックでボイスを停止させるかどうか</summary>
        protected bool stopVoiceWithClick = false;

        /// <summary>テキストコマンドを単一テキストとして認識するかどうか</summary>
        public bool IsSingleTextMode { get; protected set; }

        /// <summary>リピートボタン押下時に鳴らすサウンド情報</summary>
        protected string repeatSound = string.Empty;

        /// <summary>スタック状態のテキスト情報</summary>
        protected TextInfo stackTextInfo = null;

        /// <summary>
        /// <para>入力による進行をブロックする待機状態</para>
        /// <para>デフォルトでは、クリック待ち、テキスト表示待ち以外の全ての待機状態</para>
        /// </summary>
        // 0xffffffffがベタに書くとlong型扱いになってしまうので変換してint型にしている
        protected int blockProgressWait = BitConverter.ToInt32(BitConverter.GetBytes(0xFFFFFFFF), 0) & ~(
            (int) PlayState.WaitClick
            | (int) PlayState.WaitTextEnd);

        /// <summary>入力による進行をブロックする待機状態</summary>
        public int BlockProgressWait { get { return blockProgressWait; } set { blockProgressWait = value; } }

        /// <summary>
        /// ユーザからの入力で、現在実行途中のコマンドをクリック待ちまで進めて良いかを返します
        /// statusManager.IsExecuteがtrueの場合はAdvScenarioPlayer側のUpdateで実行されるExecuteNextCommandに実行を任せる想定
        /// </summary>
        protected bool IsExecuteCommandByInput { get { return !statusManager.IsExecute && (!windowManager.IsEndShowText || !statusManager.State.IsWait(PlayState.WaitClick)); } }

        /// <summary>PlayModeと表示速度のマップ</summary>
        Dictionary<PlayMode, float> textSpeedMap;

#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="engine">シナリオ制御統合コンポーネント</param>
        public virtual void Init(AdvMainEngine engine)
        {
            this.engine = engine;
            IsSingleTextMode = false;

            windowManager.Init(engine);

            statusManager = engine.StatusManager;
            statusManager.Mode.AddModeChangeCallback(OnPlayModeChange);

            backlogManager.Init(this);
            selectManager.Init(this);
            selectManager.AddSelectCallback(OnSelectSelection);
            
            AdvEngineSetting engineSetting = engine.Settings.EngineSetting;
            windowManager.SetTextCaptionSpeed(engineSetting.TextCaptionMargin);
            if (engine.Loader != null)
            {
                backlogManager.CreateItems(engine.Loader, engineSetting.BacklogResourcePath, engineSetting.BacklogLength);
            }
            stopVoiceWithClick = engineSetting.StopVoiceWithClick;

            textSpeedMap = new Dictionary<PlayMode, float>
            {
                {PlayMode.Normal, engineSetting.TextCaptionMargin},
                {PlayMode.Auto, engineSetting.TextCaptionMarginAuto},
                {PlayMode.Skip, engineSetting.TextCaptionMargin * engineSetting.SkipTimeScale}
            };
        }

        /// <summary>
        /// 初期化前の状態に戻します。
        /// </summary>
        public virtual void Uninit()
        {
            statusManager = null;
            stopVoiceWithClick = false;
            IsSingleTextMode = false;
            repeatSound = string.Empty;
            stackTextInfo = null;

            touchHandler.Clickable = true;

            windowManager.Uninit();
            backlogManager.Uninit();
            selectManager.Uninit();
            selectManager.gameObject.SetActive(false);
        }

        /// <summary>
        /// ウィンドウを表示します。
        /// </summary>
        public virtual void ShowWindow()
        {
            windowManager.ActiveWindow.RootObject.SetActive(true);
            SetButtonActive(true);
            if (statusManager.State.IsWait(PlayState.Select))
            {
                selectManager.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// ウィンドウを非表示にします。
        /// </summary>
        public virtual void HideWindow()
        {
            windowManager.ActiveWindow.RootObject.SetActive(false);
            SetButtonActive(false);
            selectManager.gameObject.SetActive(false);
        }

        /// <summary>
        /// テキストの表示が終了した際に呼び出されます。
        /// </summary>
        public bool OnEndShowText()
        {
            bool isWaitClick = statusManager.State.IsWait(PlayState.WaitClick);

            statusManager.State.WaitRelease(PlayState.WaitTextEnd);
            if (statusManager.Mode.IsAuto)
            {
                statusManager.State.WaitRelease(PlayState.WaitClick);
            }
            if (statusManager.Mode.IsSkip)
            {
                // スキップ状態ではテキストがすべて表示された時点で全ての動作を終了
                engine.MovementManager.EndAll();
                engine.Player.DelayManager.EndAll();

                statusManager.State.WaitRelease(PlayState.WaitClick);
                isWaitClick = false;
            }

            return isWaitClick;
        }

        /// <summary>
        /// テキスト、ラベルを設定します。
        /// </summary>
        /// <param name="info">テキスト情報</param>
        public void SetText(TextInfo info)
        {
            windowManager.SetText(info);

            // 単一テキストとしてみなしている場合、即バックログに転送せず一旦スタックする
            if (IsSingleTextMode)
            {
                if (stackTextInfo == null)
                {
                    stackTextInfo = info;
                    repeatSound = info.Sound;
                }
                else
                {
                    if (!string.IsNullOrEmpty(info.Sound))
                    {
                        repeatSound = info.Sound;
                    }
                    stackTextInfo = new TextInfo(stackTextInfo.Label, stackTextInfo.Text + info.Text, repeatSound);
                }
            }
            else
            {
                backlogManager.SetText(info);
                repeatSound = info.Sound;
            }
        }

        /// <summary>
        /// サウンドを再度再生させます。
        /// </summary>
        /// <param name="sound">サウンド情報</param>
        public void RepeatSound(string sound)
        {
            if (!string.IsNullOrEmpty(sound))
            {
                engine.SoundManager.StopVoice();
                engine.SoundManager.Post(sound);
            }
        }

        /// <summary>
        /// 入力判定の有効状態を設定します。
        /// </summary>
        /// <param name="isEnable">有効かどうか</param>
        public void SetTouchEnable(bool isEnable)
        {
            touchHandler.Clickable = isEnable;
        }

        /// <summary>
        /// テキストコマンドを単一のテキストとして認識するかどうかを設定します。
        /// </summary>
        /// <param name="isEnable">認識するかどうか</param>
        public void SetSingleTextMode(bool isEnable)
        {
            if (IsSingleTextMode == isEnable)
            {
                return;
            }

            IsSingleTextMode = isEnable;
            if (!IsSingleTextMode && stackTextInfo != null)
            {
                backlogManager.SetText(stackTextInfo);
                stackTextInfo = null;
                // 自動再生の音声待ち状態の場合、サウンドの停止待ちを行う
                if (statusManager.Mode.IsAuto && engine.Settings.EngineSetting.WaitVoiceEndWhenAuto)
                {
                    if (!string.IsNullOrEmpty(repeatSound))
                    {
                        engine.StatusManager.State.ConfirmReservation(PlayState.WaitSound);
                    }
                }
            }
        }

        /// <summary>
        /// 各プレイモードのテキスト表示間隔を設定します。
        /// </summary>
        /// <param name="mode">テキスト表示間隔を設定するプレイモード</param>
        /// <param name="speed">表示間隔(表示速度)(0に近づくほど高速)</param>
        public void SetTextCaptionSpeed(PlayMode mode, float speed)
        {
            textSpeedMap[mode] = speed;
        }

        /// <summary>
        /// 設定されたテキスト表示間隔をウインドウに適用します。
        /// </summary>
        /// <param name="mode">現在のプレイモード</param>
        public void ApplyTextCaptionspeed(PlayMode mode)
        {
            if (textSpeedMap.ContainsKey(mode))
            {
                windowManager.SetTextCaptionSpeed(textSpeedMap[mode]);
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// 画面がタッチされた際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnClickScreen()
        {
            if (!engine.IsPlaying)
            {
                return;
            }

            bool isAutoPlayWaiting = statusManager.Mode.IsAuto && !statusManager.Mode.IsExecute;
            // スキップ、自動再生状態でタッチした場合、再生モードを通常に戻す
            SwitchNormal(true);

            if (backlogManager.IsShow)
            {
                return;
            }
            if (!windowManager.IsShow)
            {
                ShowWindow();
                return;
            }

            if (statusManager.State.IsWait(BlockProgressWait))
            {
                return;
            }

            if (IsExecuteCommandByInput)
            {
                // 自動再生状態で既に進行待機状態になっている場合は、
                // コマンドの実行は終わっているはずなので実行しない
                if (!isAutoPlayWaiting)
                {
                    engine.Player.ExecuteToWaitClick();
                }

                // テキストの表示中にタップして全表示にする場合、
                // 動作中の全てのTweenをすべて完了させる
                engine.MovementManager.EndAll();
                engine.Player.DelayManager.EndAll();

                windowManager.ActiveWindow.TextController.ShowTextAll();                                
            }
            else
            {
                statusManager.State.WaitRelease(PlayState.WaitClick);
            }

            if (stopVoiceWithClick)
            {
                engine.SoundManager.StopVoice();
            }
        }

        /// <summary>
        /// オートボタンが押された際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnClickAuto()
        {
            if (!engine.IsPlaying)
            {
                return;
            }

            statusManager.Mode.SwitchMode(PlayMode.Auto);

            if (statusManager.Mode.IsAuto)
            {
                if (WindowManager.IsEndShowText && statusManager.State.IsWait(PlayState.WaitClick))
                {
                    statusManager.State.WaitRelease(PlayState.WaitClick);
                    statusManager.Mode.WaitRelease();
                }
            }
            else
            {
                SwitchNormal(false);
            }
        }

        /// <summary>
        /// スキップボタンが押された際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnClickSkip()
        {
            if (!engine.IsPlaying)
            {
                return;
            }

            statusManager.Mode.SwitchMode(PlayMode.Skip);

            if (statusManager.Mode.IsSkip)
            {
                if (WindowManager.IsEndShowText)
                {
                    statusManager.State.WaitRelease(PlayState.WaitClick);
                }
            }
            else
            {
                SwitchNormal(false);
            }
        }

        /// <summary>
        /// リピートボタンが押された際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnClickRepeat()
        {
            if (!engine.IsPlaying)
            {
                return;
            }

            SwitchNormal(false);
            RepeatSound(repeatSound);
        }

        /// <summary>
        /// バックログボタンが押された際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnClickBacklog()
        {
            if (!engine.IsPlaying)
            {
                return;
            }

            bool isAutoPlayWaiting = statusManager.Mode.IsAuto && !statusManager.Mode.IsExecute;
            SwitchNormal(false);

            if (!backlogManager.IsShow)
            {
                if (!statusManager.State.IsWait(BlockProgressWait))
                {
                    // テキストの表示途中でバックログを表示する場合は、現在実行中のコマンドをクリック待ちまで進める
                    if (IsExecuteCommandByInput)
                    {
                        // 自動再生状態で既に進行待機状態になっている場合は、
                        // コマンドの実行は終わっているはずなので実行しない
                        if (!isAutoPlayWaiting)
                        {
                            engine.Player.ExecuteToWaitClick();
                        }

                        // テキストの表示中にタップして全表示にする場合、
                        // 動作中の全てのTweenをすべて完了させる
                        engine.MovementManager.EndAll();
                        engine.Player.DelayManager.EndAll();

                        windowManager.ActiveWindow.TextController.ShowTextAll();
                    }
                }

                backlogManager.Show();
                HideWindow();
            }
            else
            {
                backlogManager.Hide();
                ShowWindow();
            }
        }

        /// <summary>
        /// ウィンドウを非表示にします。
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnHideWindow()
        {
            if (!engine.IsPlaying)
            {
                return;
            }

            SwitchNormal(false);
            HideWindow();
        }

        /// <summary>
        /// 再生モードが切り替わった際に呼び出されます。
        /// </summary>
        /// <param name="mode">再生モード</param>
        protected virtual void OnPlayModeChange(PlayMode mode)
        {
            ApplyTextCaptionspeed(mode);
        }

        /// <summary>
        /// 選択肢が選択された際に呼び出されます。
        /// </summary>
        protected virtual void OnSelectSelection()
        {
            AdvEngineSetting engineSetting = engine.Settings.EngineSetting;
            if (statusManager.Mode.IsAuto && engineSetting.IsReleaseAutoWhenSelect)
            {
                SwitchNormal(true);
            }
            if (statusManager.Mode.IsSkip && engineSetting.IsReleaseSkipWhenSelect)
            {
                SwitchNormal(true);
            }
        }

        /// <summary>
        /// 再生モードを通常状態に切り替えます。
        /// </summary>
        /// <param name="isProgressNext">次のスクリプトを進行させるかどうか</param>
        protected virtual void SwitchNormal(bool isProgressNext)
        {
            statusManager.Mode.SwitchMode(PlayMode.Normal);
            if (engine.Settings.EngineSetting.SavedSetting.WaitVoiceEndWhenAuto)
            {
                if (statusManager.State.IsWait(PlayState.WaitSound, true))
                {
                    statusManager.State.WaitRelease(PlayState.WaitSound, true);
                }
                if (statusManager.State.IsWait(PlayState.WaitSound))
                {
                    statusManager.State.WaitRelease(PlayState.WaitSound);
                }
            }

            // 進行させない場合に、進行可能になっていたら、クリック待ちを設定して進行を中断させる
            if (!isProgressNext && statusManager.IsExecute)
            {
                if (touchHandler.Clickable)
                {
                    statusManager.State.Wait(PlayState.WaitClick);
                }
            }
        }

        /// <summary>
        /// ボタンの表示状態を切り替えます。
        /// </summary>
        /// <param name="active">表示するかどうか</param>
        protected void SetButtonActive(bool active)
        {
            foreach (GameObject button in buttons)
            {
                if (button != null)
                {
                    button.SetActive(active);
                }
            }
        }

#if NOVELKIT_EDITOR || UNITY_EDITOR

        protected virtual void Update()
        {
            if (InputWrapper.GetKeyDown(KeyCode.LeftControl))
            {
                if (!statusManager.Mode.IsSkip)
                {
                    OnClickSkip();
                }
            }
            
            if (InputWrapper.GetKeyUp(KeyCode.LeftControl))
            {
                if (statusManager.Mode.IsSkip)
                {
                    OnClickSkip();
                }
            }
            
            if (InputWrapper.GetKeyDown(KeyCode.Return) || InputWrapper.GetKeyDown(KeyCode.Space))
            {
                if (touchHandler.Clickable)
                {
                    OnClickScreen();
                }
            }
        }

#endif

#endregion
    }
}
