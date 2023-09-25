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
using Jigbox.Tween;

namespace Jigbox.Components
{
    /// <summary>
    /// Tweenを使ってマーキーを動かすTransitionコンポーネント
    /// </summary>
    public abstract class MarqueeTweenTransition : MarqueeTransitionBase
    {
#region fields

        /// <summary>
        /// トランジションに使用するTween
        /// </summary>
        protected TweenSingle tween = new TweenSingle();

#endregion

#region public methods

        /// <summary>
        /// アクティブへの切り替え時に呼ばれます
        /// 必要であればTweenをPauseにしますが、コールバックの発火はしません
        /// </summary>
        public override void EnableTransition()
        {
            // Pause状態の場合は何もしない
            if (isPause)
            {
                return;
            }

            // tweenがPause中の場合は動作をResumeさせる
            if (tween.State == TweenState.Paused)
            {
                tween.Resume();
            }
        }

        /// <summary>
        /// 非アクティブへの切り替え時に呼ばれます
        /// 必要であればTweenをResumeしますが、コールバックの発火はしません
        /// </summary>
        public override void DisableTransition()
        {
            // Pause状態の場合は何もしない
            if (isPause)
            {
                return;
            }

            // tweenが動作中の場合はPauseにさせる
            if (tween.State == TweenState.Working)
            {
                // アクティブ切り替えではResumeのコールバックは発火させない
                tween.Pause();
            }
        }

        /// <summary>
        /// トランジションを一時停止します
        /// </summary>
        public override void PauseMovement()
        {
            // 初期状態の場合は何もしない
            if (stateMachine.StateType == MarqueeState.Idle)
            {
                return;
            }

            // 既にPause状態の場合は何もしない
            if (isPause)
            {
                return;
            }

            // tweenが動作中の場合はtweenをPause状態にさせる
            if (tween.State == TweenState.Working)
            {
                tween.Pause();
            }

            base.PauseMovement();
        }

        /// <summary>
        /// トランジションの一時停止を解除します
        /// </summary>
        public override void ResumeMovement()
        {
            // 初期状態の場合は何もしない
            if (stateMachine.StateType == MarqueeState.Idle)
            {
                return;
            }

            // 先にPauseが呼ばれていない場合は何もしない
            if (!isPause)
            {
                return;
            }

            // ヒエラルキー上でアクティブでない状態でResumeが呼ばれた場合の処理
            if (!gameObject.activeInHierarchy)
            {
                // Pauseが呼ばれた後のResumeなのでコールバックを発火する
                base.ResumeMovement();
                // 非アクティブ状態のためTweenのResumeは行わない
                return;
            }

            // TweenがPause状態だった場合はResumeする
            if (tween.State == TweenState.Paused)
            {
                tween.Resume();
            }

            base.ResumeMovement();
        }

        /// <summary>
        /// 強制停止します
        /// </summary>
        public override void Kill()
        {
            // 初期状態の場合は何もしない
            if (stateMachine.StateType == MarqueeState.Idle)
            {
                return;
            }

            tween.Kill();
            base.Kill();
        }

#endregion


#region protected methods

        /// <summary>
        /// DurationDelayステートへ遷移時に呼び出されます
        /// </summary>
        protected override void OnEnterDurationDelay()
        {
            tween.Duration = transitionProperty.DurationDelay;
            SetTweenOnComplete(OnCompleteDurationDelay);
            StartTween();
        }
        
        /// <summary>
        /// EntranceDurationステートへの遷移時に呼び出されます
        /// </summary>
        protected override void OnEnterEntranceDuration()
        {
            tween.Duration = transitionProperty.EntranceAnimationProperty.Duration;
            tween.MotionType = transitionProperty.EntranceAnimationProperty.MotionType;
            tween.EasingType = transitionProperty.EntranceAnimationProperty.EasingType;
            SetTweenOnComplete(OnCompleteEntranceDuration, OnUpdateTweenDuration);
            tween.Begin = CalculateEnterEntranceBeginPositionAxis();
            tween.Final = CalculateBeginPositionAxis();
            StartTween();
        }
        
        /// <summary>
        /// EntranceDelayステートへの遷移時に呼び出されます
        /// </summary>
        protected override void OnEnterEntranceDelay()
        {
            tween.Duration = transitionProperty.EntranceAnimationProperty.Delay;
            SetTweenOnComplete(OnCompleteEntranceDelay);
            StartTween();
        }

        /// <summary>
        /// Durationステートへの遷移時に呼び出されます
        /// </summary>
        protected override void OnEnterDuration()
        {
            tween.Duration = CalculateDurationTime();
            
            // 文字スクロール中はタイプを初期状態に戻す
            tween.MotionType = MotionType.Linear;
            tween.EasingType = EasingType.EaseIn;
            SetTweenOnComplete(OnCompleteDuration, OnUpdateTweenDuration);
            tween.Begin = CalculateBeginPositionAxis();
            tween.Final = CalculateEndPositionAxis();
            StartTween();
        }
        
        /// <summary>
        /// ExitDelayステートへの遷移時に呼び出されます
        /// </summary>
        protected override void OnEnterExitDelay()
        {
            tween.Duration = transitionProperty.ExitAnimationProperty.Delay;
            SetTweenOnComplete(OnCompleteExitDelay);
            StartTween();
        }
        
        /// <summary>
        /// ExitDurationステートへの遷移時に呼び出されます
        /// </summary>
        protected override void OnEnterExitDuration()
        {
            tween.Duration = transitionProperty.ExitAnimationProperty.Duration;
            tween.MotionType = transitionProperty.ExitAnimationProperty.MotionType;
            tween.EasingType = transitionProperty.ExitAnimationProperty.EasingType;
            SetTweenOnComplete(OnCompleteExitDuration, OnUpdateTweenDuration);
            tween.Begin = CalculateEndPositionAxis();
            tween.Final = CalculateEnterExitEndPositionAxis();
            StartTween();
        }

        /// <summary>
        /// Intervalステートへの遷移時に呼び出されます
        /// </summary>
        protected override void OnEnterInterval()
        {
            tween.Duration = transitionProperty.Interval;
            SetTweenOnComplete(OnCompleteInterval);
            StartTween();
        }

        /// <summary>
        /// LoopDurationDelayステートへ遷移時に呼び出されます
        /// </summary>
        protected override void OnEnterLoopDurationDelay()
        {
            // Durationの時間が別なだけで、それ以降の処理はDurationDelayと同じ
            tween.Duration = transitionProperty.LoopDurationDelay;
            SetTweenOnComplete(OnCompleteLoopDurationDelay);
            StartTween();
        }

        /// <summary>
        /// Tweenに必要なコールバックを登録します
        /// </summary>
        /// <param name="onComplete"></param>
        /// <param name="onUpdate"></param>
        protected virtual void SetTweenOnComplete(Action<ITween<float>> onComplete, Action<ITween<float>> onUpdate = null)
        {
            tween.RemoveAllOnComplete();
            tween.RemoveAllOnUpdate();
            tween.OnComplete(onComplete);
            if (onUpdate != null)
            {
                tween.OnUpdate(onUpdate);
            }
        }

        /// <summary>
        /// TweenのStartを行います。
        /// 必要な場合はTweenをStartした後にPause状態にします
        /// </summary>
        protected virtual void StartTween()
        {
            tween.Start();
            // Pause状態、もしくは非アクティブの場合はTweenをStartした後にPause状態にさせる
            if (isPause || !gameObject.activeInHierarchy)
            {
                tween.Pause();
            }
        }

        /// <summary>
        /// DurationDelayステートが終わり、次のステートへと移ります
        /// </summary>
        /// <param name="t"></param>
        protected virtual void OnCompleteDurationDelay(ITween<float> t)
        {
            CompleteDurationDelay();
        }

        /// <summary>
        /// Durationから次のステートへ遷移します
        /// </summary>
        protected virtual void OnCompleteDuration(ITween<float> t)
        {
            CompleteDuration();
        }
        
        /// <summary>
        /// Durationから次のステートへ遷移します
        /// </summary>
        protected virtual void OnCompleteEntranceDuration(ITween<float> t)
        {
            CompleteEntranceDuration();
        }
        
        /// <summary>
        /// DurationDelayステートが終わり、次のステートへと移ります
        /// </summary>
        /// <param name="t"></param>
        protected virtual void OnCompleteEntranceDelay(ITween<float> t)
        {
            CompleteEntranceDelay();
        }
        
        /// <summary>
        /// DurationDelayステートが終わり、次のステートへと移ります
        /// </summary>
        /// <param name="t"></param>
        protected virtual void OnCompleteExitDelay(ITween<float> t)
        {
            CompleteExitDelay();
        }
        
        /// <summary>
        /// Durationから次のステートへ遷移します
        /// </summary>
        protected virtual void OnCompleteExitDuration(ITween<float> t)
        {
            CompleteExitDuration();
        }

        /// <summary>
        /// Intervalから次のステートへ遷移します
        /// </summary>
        /// <param name="t"></param>
        protected virtual void OnCompleteInterval(ITween<float> t)
        {
            CompleteInterval();
        }

        /// <summary>
        /// LoopDurationDelayステートが終わり、次のステートへと移ります
        /// </summary>
        /// <param name="t"></param>
        protected virtual void OnCompleteLoopDurationDelay(ITween<float> t)
        {
            CompleteLoopDurationDelay();
        }

        /// <summary>
        /// 指定された速度で移動するために必要なDurationの時間を計算する
        /// </summary>
        /// <returns></returns>
        protected virtual float CalculateDurationTime()
        {
            return MarqueeTransitionUtil.CalculateDuration(
                transitionProperty.Speed,
                transitionProperty.Length,
                ViewportLength,
                transitionProperty.StartPositionRate,
                transitionProperty.EndPositionRate,
                transitionProperty.ScrollType);
        }

        /// <summary>
        /// Containerの開始位置を返します
        /// x軸,y軸への対応は継承先で判断します
        /// </summary>
        /// <returns></returns>
        protected virtual float CalculateBeginPosition()
        {
            return MarqueeTransitionUtil.CalculateBeginPosition(
                transitionProperty.Length,
                ViewportLength,
                transitionProperty.StartPositionRate,
                transitionProperty.ScrollType,
                transitionProperty.ScrollDirectionType);
        }

        /// <summary>
        /// Containerの終了位置を返します
        /// x軸,y軸への対応は継承先で判断します
        /// </summary>
        /// <returns></returns>
        protected virtual float CalculateEndPosition()
        {
            return MarqueeTransitionUtil.CalculateEndPosition(
                transitionProperty.Length,
                ViewportLength,
                transitionProperty.EndPositionRate,
                transitionProperty.ScrollType,
                transitionProperty.ScrollDirectionType);
        }

        /// <summary>
        /// マーキーの初期位置を取得
        /// </summary>
        /// <returns></returns>
        protected virtual float CalculateInitPosition()
        {
            if (MarqueeTransitionUtil.InvalidScroll(transitionProperty.Length, ViewportLength, transitionProperty.ScrollType))
            {
                return 0;
            }

            if (transitionProperty.EntranceAnimationProperty.Enable)
            {
                return CalculateEntranceBeginPositionAxis();
            }

            return CalculateBeginPositionAxis();
        }

        /// <summary>
        /// 入場時の開始位置を取得
        /// </summary>
        /// <returns></returns>
        protected virtual float CalculateEnterEntranceBeginPositionAxis()
        {
            if (transitionProperty.ScrollDirectionType == MarqueeScrollDirectionType.Normal)
            {
                return CalculateEntranceBeginPositionAxis();
            }
            else
            {
                return CalculateExitEndPositionAxis();
            }
        }
        
        /// <summary>
        /// 退場時の終了位置を取得
        /// </summary>
        /// <returns></returns>
        protected virtual float CalculateEnterExitEndPositionAxis()
        {
            if (transitionProperty.ScrollDirectionType == MarqueeScrollDirectionType.Normal)
            {
                return CalculateExitEndPositionAxis();
            }
            else
            {
                return CalculateEntranceBeginPositionAxis();
            }
        }

#endregion

#region abstract methods

        /// <summary>
        /// Containerのトランジション開始位置を計算します
        /// </summary>
        /// <returns></returns>
        protected abstract float CalculateBeginPositionAxis();

        /// <summary>
        /// Containerのトランジション終了位置を計算します
        /// </summary>
        /// <returns></returns>
        protected abstract float CalculateEndPositionAxis();

        /// <summary>
        /// Durationステート中にTweenの更新で呼ばれます
        /// </summary>
        /// <param name="t"></param>
        protected abstract void OnUpdateTweenDuration(ITween<float> t);
        
        /// <summary>
        /// 入場時の開始位置を取得
        /// </summary>
        /// <returns>開始位置</returns>
        protected abstract float CalculateEntranceBeginPositionAxis();
        
        /// <summary>
        /// 退場時の終了位置を取得
        /// </summary>
        /// <returns>終了位置</returns>
        protected abstract float CalculateExitEndPositionAxis();
#endregion
    }
}
