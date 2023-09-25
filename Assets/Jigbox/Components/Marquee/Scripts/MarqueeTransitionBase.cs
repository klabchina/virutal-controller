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

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Marquee))]
    public abstract class MarqueeTransitionBase : MonoBehaviour
    {
#region fields

        /// <summary>
        /// コールバック発火用Handler
        /// </summary>
        protected IMarqueeTransitionHandler handler;

        /// <summary>
        /// Transitionに必要な情報を持つプロパティクラス
        /// </summary>
        protected MarqueeTransitionProperty transitionProperty;

        /// <summary>
        /// ステートマシン
        /// </summary>
        protected MarqueeStateMachine stateMachine = new MarqueeStateMachine();

        /// <summary>
        /// Pause状態かどうかを返します。
        /// </summary>
        protected bool isPause;

#endregion

#region properties

        /// <summary>
        /// Axis方向のViewportの長さを返します
        /// </summary>
        protected abstract float ViewportLength { get; }

#endregion

#region abstract methods

        /// <summary>
        /// Containerの位置を移動開始位置に移動させます
        /// 注意：Layoutの計算が終わらないと正しい位置にいかない可能性があります
        /// </summary>
        public abstract void InitPosition();

        /// <summary>
        /// アクティブへの切り替え時に呼ばれます
        /// </summary>
        public abstract void EnableTransition();

        /// <summary>
        /// 非アクティブへの切り替え時に呼ばれます
        /// </summary>
        public abstract void DisableTransition();

#endregion

#region public methods

        /// <summary>
        /// 初期化
        /// ステートマシンの登録と、Containerの初期位置へと移動させる
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="transitionProperty"></param>
        public virtual void Init(IMarqueeTransitionHandler handler, MarqueeTransitionProperty transitionProperty)
        {
            this.handler = handler;
            this.transitionProperty = transitionProperty;
            InitStates();
            InitPosition();
        }

        /// <summary>
        /// トランジションを開始します
        /// </summary>
        public virtual void StartMovement()
        {
            isPause = false;
            stateMachine.Set(MarqueeState.Start);
        }

        /// <summary>
        /// 強制停止します
        /// </summary>
        public virtual void Kill()
        {
            isPause = false;
            stateMachine.Set(MarqueeState.Kill);
        }

        /// <summary>
        /// トランジションを一時停止します
        /// </summary>
        public virtual void PauseMovement()
        {
            // 既にPause状態の場合は何もしない
            if (isPause)
            {
                return;
            }

            isPause = true;
            handler.OnPause();
        }

        /// <summary>
        /// トランジションの一時停止を解除します
        /// </summary>
        public virtual void ResumeMovement()
        {
            // Pause状態でない場合は何もしない
            if (!isPause)
            {
                return;
            }

            isPause = false;
            handler.OnResume();
        }

        /// <summary>
        /// View側でレイアウトの計算が終わったときに呼ばれます
        /// Transitionがレイアウトの計算を待つステートの時だけ影響があります
        /// </summary>
        public virtual void OnCompleteLayout()
        {
            if (stateMachine.StateType == MarqueeState.Layout)
            {
                CompleteLayout();
            }

            if (stateMachine.StateType == MarqueeState.LoopLayout)
            {
                CompleteLoopLayout();
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// 初期化時にステートの登録を行うために呼び出されます
        /// ステートの流れは以下のルールでおこなれます
        ///
        /// * ループじゃない場合
        /// Start -> Layout -> EntranceDelay -> EntranceDuration -> DurationDelay -> Duration -> ExitDelay -> ExitDuration -> Done
        /// * ループの場合
        /// Start -> Layout -> EntranceDelay -> EntranceDuration -> DurationDelay -> Duration -> ExitDelay -> ExitDuration -> Interval -> LoopStart -> LoopLayout -> EntranceDelay -> EntranceDuration -> LoopDurationDelay -> Duration -> ExitDelay -> ExitDuration ...
        /// * IfNeededの条件にあう場合
        /// Start -> Layout -> IfNeeded -> Done
        ///
        /// DurationDelay, Interval, LoopDelay の3つのステートは時間が設定されていない場合は遷移しません。
        /// 例えば DurationDelay 用の時間が設定されていない場合は Layout -> DurationDelay ではなく、 Layout -> Duration とステートが遷移します。
        /// </summary>
        protected virtual void InitStates()
        {
            stateMachine.Add(MarqueeState.Start, OnEnterStart, null, null);
            stateMachine.Add(MarqueeState.Layout, null, null, null, (int) MarqueeState.Start);
            stateMachine.Add(MarqueeState.EntranceDelay, OnEnterEntranceDelay, null, OnExitEntranceDelay, (int) MarqueeState.Layout | (int) MarqueeState.LoopLayout);
            stateMachine.Add(MarqueeState.EntranceDuration, OnEnterEntranceDuration, null, OnExitEntranceDuration, (int) MarqueeState.Layout | (int) MarqueeState.LoopLayout | (int) MarqueeState.EntranceDelay);
            stateMachine.Add(MarqueeState.DurationDelay, OnEnterDurationDelay, null, OnExitDurationDelay, (int) MarqueeState.Layout | (int) MarqueeState.EntranceDuration);
            stateMachine.Add(MarqueeState.Duration, OnEnterDuration, null, OnExitDuration, (int) MarqueeState.Layout | (int) MarqueeState.LoopLayout | (int) MarqueeState.DurationDelay | (int) MarqueeState.LoopDurationDelay | (int) MarqueeState.EntranceDuration);
            stateMachine.Add(MarqueeState.ExitDelay, OnEnterExitDelay, null, OnExitExitDelay, (int) MarqueeState.Duration);
            stateMachine.Add(MarqueeState.ExitDuration, OnEnterExitDuration, null, OnExitExitDuration, (int) MarqueeState.ExitDelay |  (int) MarqueeState.Duration);
            stateMachine.Add(MarqueeState.Interval, OnEnterInterval, null, OnExitInterval, (int) MarqueeState.ExitDuration | (int) MarqueeState.Duration);
            stateMachine.Add(MarqueeState.LoopStart, OnEnterLoopStart, null, null, (int) MarqueeState.ExitDuration | (int) MarqueeState.Interval | (int) MarqueeState.Duration);
            stateMachine.Add(MarqueeState.LoopLayout, null, null, null, (int) MarqueeState.LoopStart);
            stateMachine.Add(MarqueeState.LoopDurationDelay, OnEnterLoopDurationDelay, null, OnExitLoopDurationDelay, (int) MarqueeState.LoopLayout | (int) MarqueeState.EntranceDuration);
            stateMachine.Add(MarqueeState.Kill, OnEnterKill, null, null, (int) MarqueeState.All & ~ (int) MarqueeState.Done);
            stateMachine.Add(MarqueeState.Done, OnEnterDone, null, null);
            stateMachine.Add(MarqueeState.IfNeeded, OnEnterIfNeeded, null, null, (int) MarqueeState.Layout | (int) MarqueeState.LoopLayout);
        }

        /// <summary>
        /// Startステートへ遷移時に呼び出されます
        /// </summary>
        protected virtual void OnEnterStart()
        {
            // Layoutの計算タイミング次第では、この時点では正しい位置に移動できていない可能性があるため、Layoutステート終了後にもInitPositionは呼ぶ
            InitPosition();
            handler.OnStartTransition();
            stateMachine.Set(MarqueeState.Layout);
        }

        /// <summary>
        /// Layoutステートが終わり、次のステートへと移ります
        /// </summary>
        protected virtual void CompleteLayout()
        {
            InitPosition();
            handler.OnCompleteLayout();
            handler.OnCompleteLayoutContent(!InvalidScroll());

            // スクロールを行わない場合はIfNeededステートへ遷移する
            if (InvalidScroll())
            {
                stateMachine.Set(MarqueeState.IfNeeded);
                return;
            }

            if (transitionProperty.EntranceAnimationProperty.Enable)
            {
                // 遅延時間が設定されているかどうかで次に移るステートを変える
                if (transitionProperty.EntranceAnimationProperty.HasDelay)
                {
                    stateMachine.Set(MarqueeState.EntranceDelay);
                }
                else
                {
                    stateMachine.Set(MarqueeState.EntranceDuration);
                }
            }
            else if (transitionProperty.HasDurationDelay) // 遅延時間が設定されているかどうかで次に移るステートを変える
            {
                stateMachine.Set(MarqueeState.DurationDelay);
            }
            else
            {
                stateMachine.Set(MarqueeState.Duration);
            }
        }

        /// <summary>
        /// DurationDelayステートへ遷移時に呼び出されます
        /// </summary>
        protected virtual void OnEnterDurationDelay()
        {
        }

        /// <summary>
        /// DurationDelayステートが終わり、次のステートへと移ります
        /// </summary>
        protected virtual void CompleteDurationDelay()
        {
            handler.OnCompleteDurationDelay();
            // 遅延処理が終われば次のステートに移る
            stateMachine.Set(MarqueeState.Duration);
        }

        /// <summary>
        /// DurationDelayステートから別のステートへ遷移時に呼び出されます
        /// </summary>
        protected virtual void OnExitDurationDelay()
        {
        }

        /// <summary>
        /// Durationステートへの遷移時に呼び出されます
        /// </summary>
        protected virtual void OnEnterDuration()
        {
        }

        /// <summary>
        /// Durationから次のステートへ遷移します
        /// </summary>
        protected virtual void CompleteDuration()
        {
            handler.OnCompleteDuration();
            if (transitionProperty.ExitAnimationProperty.Enable)
            {
                if (transitionProperty.ExitAnimationProperty.HasDelay)
                {
                    stateMachine.Set(MarqueeState.ExitDelay);
                }
                else
                {
                    stateMachine.Set(MarqueeState.ExitDuration);
                }
            }
            else
            {
                NextState();
            }
        }

        /// <summary>
        /// Durationステートから別のステートへ遷移時に呼び出されます
        /// </summary>
        protected virtual void OnExitDuration()
        {
        }
        
        /// <summary>
        /// EntranceDuration ステートへの遷移時に呼び出されます
        /// </summary>
        protected virtual void OnEnterEntranceDuration()
        {
        }
        
        /// <summary>
        /// EntranceDuration ステートへの遷移時に呼び出されます
        /// </summary>
        protected virtual void OnExitEntranceDuration()
        {
        }
        
        /// <summary>
        /// EntranceDuration から次のステートへ遷移します
        /// </summary>
        protected virtual void CompleteEntranceDuration()
        {
            handler.OnCompleteEntranceDuration();
            // 遅延時間が設定されているかどうかで次に移るステートを変える
            if (transitionProperty.HasDurationDelay)
            {
                stateMachine.Set(MarqueeState.DurationDelay);
            }
            else
            {
                stateMachine.Set(MarqueeState.Duration);
            }
        }
        
        /// <summary>
        /// EntranceDelay ステートへの遷移時に呼び出されます
        /// </summary>
        protected virtual void OnEnterEntranceDelay()
        {
        }
        
        /// <summary>
        /// EntranceDelay ステートへの遷移時に呼び出されます
        /// </summary>
        protected virtual void OnExitEntranceDelay()
        {
        }
        
        /// <summary>
        /// EntranceDelay ステートが終わり、次のステートへと移ります
        /// </summary>
        protected virtual void CompleteEntranceDelay()
        {
            handler.OnCompleteEntranceDelay();
            // 遅延処理が終わればDurationステートに移る
            stateMachine.Set(MarqueeState.EntranceDuration);
        }
        
        /// <summary>
        /// ExitDelay ステートへの遷移時に呼び出されます
        /// </summary>
        protected virtual void OnEnterExitDelay()
        {
        }
        
        /// <summary>
        /// ExitDelay ステートへの遷移時に呼び出されます
        /// </summary>
        protected virtual void OnExitExitDelay()
        {
        }
        
        /// <summary>
        /// ExitDelay から次のステートへ遷移します
        /// </summary>
        protected virtual void CompleteExitDelay()
        {
            handler.OnCompleteExitDelay();
            // 遅延処理が終わればExitDurationステートに移る
            stateMachine.Set(MarqueeState.ExitDuration);
        }
        
        /// <summary>
        /// ExitDuration ステートへの遷移時に呼び出されます
        /// </summary>
        protected virtual void OnEnterExitDuration()
        {
        }
        
        /// <summary>
        /// ExitDuration ステートへの遷移時に呼び出されます
        /// </summary>
        protected virtual void OnExitExitDuration()
        {
        }
        
        /// <summary>
        /// ExitDuration から次のステートへ遷移します
        /// </summary>
        protected virtual void CompleteExitDuration()
        {
            handler.OnCompleteExitDuration();
            NextState();
        }

        /// <summary>
        /// Intervalステートへの遷移時に呼び出されます
        /// </summary>
        protected virtual void OnEnterInterval()
        {
        }

        /// <summary>
        /// Intervalから次のステートへ遷移します
        /// </summary>
        protected virtual void CompleteInterval()
        {
            handler.OnCompleteInterval();
            if (transitionProperty.IsLoop)
            {
                stateMachine.Set(MarqueeState.LoopStart);
            }
            else
            {
                // Loopの設定がなければそもそもIntervalステートに遷移しないためここの処理には来ない想定
                stateMachine.Set(MarqueeState.Done);
            }
        }

        /// <summary>
        /// Intervalステートから別のステートへ遷移時に呼び出されます</summary>
        protected virtual void OnExitInterval()
        {
        }

        /// <summary>LoopStartステートへ遷移時に呼び出されます</summary>
        protected virtual void OnEnterLoopStart()
        {
            // Layoutの計算タイミング次第では、この時点では正しい位置に移動できていない可能性があるため、Layoutステート終了後にもInitPositionは呼ぶ
            InitPosition();
            handler.OnLoopStart();
            stateMachine.Set(MarqueeState.LoopLayout);
        }

        /// <summary>
        /// LoopLayoutステートが終わり、次のステートへと移ります
        /// </summary>
        protected virtual void CompleteLoopLayout()
        {
            InitPosition();
            handler.OnCompleteLoopLayout();
            handler.OnCompleteLayoutContent(!InvalidScroll());

            // スクロールを行わない場合はIfNeededステートへ遷移する
            if (InvalidScroll())
            {
                stateMachine.Set(MarqueeState.IfNeeded);
                return;
            }
            
            if (transitionProperty.EntranceAnimationProperty.Enable)
            {
                // 遅延時間が設定されているかどうかで次に移るステートを変える
                if (transitionProperty.EntranceAnimationProperty.HasDelay)
                {
                    stateMachine.Set(MarqueeState.EntranceDelay);
                }
                else
                {
                    stateMachine.Set(MarqueeState.EntranceDuration);
                }
            }
            else if (transitionProperty.HasLoopDelay) // 遅延時間が設定されているかどうかで次に移るステートを変える
            {
                stateMachine.Set(MarqueeState.LoopDurationDelay);
            }
            else
            {
                stateMachine.Set(MarqueeState.Duration);
            }
        }

        /// <summary>LoopDurationDelayステートへ遷移時に呼び出されます</summary>
        protected virtual void OnEnterLoopDurationDelay()
        {
        }

        /// <summary>
        /// LoopDurationDelayステートが終わり、次のステートへと移ります
        /// </summary>
        protected virtual void CompleteLoopDurationDelay()
        {
            handler.OnCompleteLoopDurationDelay();
            // 遅延処理が終われば次のステートに移る
            stateMachine.Set(MarqueeState.Duration);
        }

        /// <summary>LoopDurationDelayステートから別のステートへ遷移時に呼び出されます</summary>
        protected virtual void OnExitLoopDurationDelay()
        {
        }

        /// <summary>Killステート遷移時に呼びだされます</summary>
        protected virtual void OnEnterKill()
        {
            handler.OnKillTransition();
        }

        /// <summary>Doneステート遷移時に呼びだされます</summary>
        protected virtual void OnEnterDone()
        {
        }

        /// <summary>IfNeededステート遷移時に呼びだされます</summary>
        protected virtual void OnEnterIfNeeded()
        {
            stateMachine.Set(MarqueeState.Done);
        }

        /// <summary>
        /// ScrollType IfNeededの条件にかかるかどうかを返す
        /// </summary>
        /// <returns></returns>
        protected virtual bool InvalidScroll()
        {
            return MarqueeTransitionUtil.InvalidScroll(
                transitionProperty.Length,
                ViewportLength,
                transitionProperty.ScrollType);
        }
        
        /// <summary>
        /// スクロール完了後のState設定
        /// </summary>
        protected virtual void NextState()
        {
            if (!transitionProperty.IsLoop)
            {
                // ループ設定でない場合は処理は終了
                stateMachine.Set(MarqueeState.Done);
                return;
            }

            if (transitionProperty.HasInterval)
            {
                stateMachine.Set(MarqueeState.Interval);
            }
            else
            {
                stateMachine.Set(MarqueeState.LoopStart);
            }
        }

#endregion

#region unity methods

        protected virtual void Update()
        {
            stateMachine.Update(Time.deltaTime);
        }

#endregion
    }
}
