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
using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.Tween
{
    /// <summary>
    /// トゥイーンアニメーションに必要なパラメータを持つ基底クラスです。利用する場合はこのクラスを継承する必要があります
    /// </summary>
    [Serializable]
    public abstract class TweenBase<T> : PeriodicMovement<T, TweenBase<T>>, ITween<T>
    {
#region configration properties

        /// <summary>アニメーションの緩急</summary>
        [SerializeField]
        [HideInInspector]
        MotionType motionType = MotionType.Linear;

        /// <summary>アニメーションの緩急</summary>
        public virtual MotionType MotionType
        {
            get
            {
                return motionType;
            }
            set
            {
                if (motionType != value)
                {
                    motionType = value;
                    // アニメーションカーブによるモーションが設定されている場合で、
                    // PingPongを使用している場合は動作用クラスの再生性が必要
                    if (motionType == MotionType.Custom && customMotion != null && LoopMode == LoopMode.PingPong)
                    {
                        behaviour = null;
                    }
                }
            }
        }

        /// <summary>アニメーションの緩急の付き方</summary>
        [SerializeField]
        [HideInInspector]
        EasingType easingType = EasingType.EaseInOut;

        /// <summary>アニメーションの緩急の付き方</summary>
        public virtual EasingType EasingType
        {
            get
            {
                return easingType;
            }
            set
            {
                easingType = value;
            }
        }

        /// <summary>アニメーションカーブを利用したアニメーションの緩急</summary>
        [SerializeField]
        [HideInInspector]
        AnimationCurve customMotion = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// <summary>アニメーションカーブを利用したアニメーションの緩急</summary>
        public virtual AnimationCurve CustomMotion
        {
            get
            {
                return customMotion;
            }
            set
            {
                if (customMotion != value)
                {
                    customMotion = value;
                    // アニメーションカーブによるモーションが設定されている場合で、
                    // PingPongを使用している場合は動作用クラスの再生性が必要
                    if (motionType == MotionType.Custom && customMotion != null && LoopMode == LoopMode.PingPong)
                    {
                        behaviour = null;
                    }
                }
            }
        }

        /// <summary>アニメーションの緩急を返すメソッド</summary>
        Func<float, float> easingFunc;

        /// <summary>Tweenでの補間の開始値</summary>
        [SerializeField]
        [HideInInspector]
        T begin;

        /// <summary>Tweenでの補間の開始値</summary>
        public virtual T Begin
        {
            get
            {
                return begin;
            }
            set
            {
                begin = value;
            }
        }

        /// <summary>Tweenでの補間の終了値</summary>
        [SerializeField]
        [HideInInspector]
        T final;

        /// <summary>Tweenでの補間の終了値</summary>
        public virtual T Final
        {
            get
            {
                return final;
            }
            set
            {
                final = value;
            }
        }

        /// <summary>トゥイーンの開始から終了までの変化量を示します</summary>
        public abstract T Change { get; set; }

#endregion

#region event handler field

        /// <summary>指定された時間の状態に変化した際のコールバック</summary>
        event Action<ITween<T>> onRewind;

        /// <summary>ラップしたクロージャーの削除ができるように、登録されたイベントハンドラーをキーにし、クロージャーと登録回数を値にして保存しておきます</summary>
        readonly Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter> onStartCallbacks = new Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter>();

        /// <summary>ラップしたクロージャーの削除ができるように、登録されたイベントハンドラーをキーにし、クロージャーと登録回数を値にして保存しておきます</summary>
        readonly Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter> onUpdateCallbacks = new Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter>();

        /// <summary>ラップしたクロージャーの削除ができるように、登録されたイベントハンドラーをキーにし、クロージャーと登録回数を値にして保存しておきます</summary>
        readonly Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter> onCompleteCallbacks = new Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter>();

        /// <summary>ラップしたクロージャーの削除ができるように、登録されたイベントハンドラーをキーにし、クロージャーと登録回数を値にして保存しておきます</summary>
        readonly Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter> onKillCallbacks = new Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter>();

        /// <summary>ラップしたクロージャーの削除ができるように、登録されたイベントハンドラーをキーにし、クロージャーと登録回数を値にして保存しておきます</summary>
        readonly Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter> onResumeCallbacks = new Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter>();

        /// <summary>ラップしたクロージャーの削除ができるように、登録されたイベントハンドラーをキーにし、クロージャーと登録回数を値にして保存しておきます</summary>
        readonly Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter> onPauseCallbacks = new Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter>();

        /// <summary>ラップしたクロージャーの削除ができるように、登録されたイベントハンドラーをキーにし、クロージャーと登録回数を値にして保存しておきます</summary>
        readonly Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter> onEndDelayCallbacks = new Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter>();

        /// <summary>ラップしたクロージャーの削除ができるように、登録されたイベントハンドラーをキーにし、クロージャーと登録回数を値にして保存しておきます</summary>
        readonly Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter> onLoopCompleteCallbacks = new Dictionary<Action<ITween<T>>, ActionTweenBaseTCounter>();

#endregion

#region constructor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected TweenBase() {}

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="onUpdate">Tweenにより補間状態が変化した際のコールバック</param>
        protected TweenBase(Action<ITween<T>> onUpdate) : this()
        {
            OnUpdate(onUpdate);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="other">パラメータのコピー元となるTween</param>
        protected TweenBase(ITween<T> other) : this()
        {
            MotionType = other.MotionType;
            EasingType = other.EasingType;
            FollowTimeScale = other.FollowTimeScale;
            Begin = other.Begin;
            Final = other.Final;
            Duration = other.Duration;
            Delay = other.Delay;
            Interval = other.Interval;
            LoopMode = other.LoopMode;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="other">パラメータのコピー元となるTween</param>
        /// <param name="onUpdate">Tweenにより補間状態が変化した際のコールバック</param>
        protected TweenBase(ITween<T> other, Action<ITween<T>> onUpdate) : this(other)
        {
            OnUpdate(onUpdate);
        }

#endregion

#region configuration utility methods

        /// <summary>
        /// 緩急（イージング）の状態を設定します。
        /// </summary>
        /// <param name="motionType">アニメーションの緩急</param>
        /// <param name="easingType">アニメーションの緩急の付き方</param>
        /// <returns><c>this</c></returns>
        public virtual ITween<T> EasingWith(MotionType motionType, EasingType easingType)
        {
            MotionType = motionType;
            EasingType = easingType;
            return this;
        }

        /// <summary>
        /// 緩急（イージング）の状態を設定します。
        /// </summary>
        /// <param name="motionType">アニメーションの緩急</param>
        /// <returns><c>this</c></returns>
        public virtual ITween<T> EasingWith(MotionType motionType)
        {
            MotionType = motionType;
            return this;
        }

        /// <summary>
        /// 緩急（イージング）の状態を設定します。
        /// </summary>
        /// <param name="easingType">アニメーションの緩急の付き方</param>
        /// <returns><c>this</c></returns>
        public virtual ITween<T> EasingWith(EasingType easingType)
        {
            EasingType = easingType;
            return this;
        }

        /// <summary>
        /// 緩急（イージング）の状態を設定します。
        /// </summary>
        /// <param name="easing">アニメーションの緩急を返すメソッド</param>
        /// <returns><c>this</c></returns>
        public virtual ITween<T> EasingWith(Func<float, float> easing)
        {
            MotionType = MotionType.Custom;
            EasingType = EasingType.Custom;
            easingFunc = easing;
            return this;
        }

        /// <summary>ループを指定します</summary>
        ITween<T> IPeriodicMovement<T, ITween<T>>.LoopWith(LoopMode loopMode, float interval)
        {
            return LoopWith(loopMode, interval);
        }

        /// <summary>ループを指定します</summary>
        ITween<T> IPeriodicMovement<T, ITween<T>>.LoopWith(LoopMode loopMode)
        {
            return LoopWith(loopMode);
        }

        /// <summary>ループを指定します</summary>
        IPeriodicMovement IPeriodicMovement.LoopWith(LoopMode loopMode)
        {
            return LoopWith(loopMode);
        }

        /// <summary>ループを指定します</summary>
        IPeriodicMovement IPeriodicMovement.LoopWith(LoopMode loopMode, float interval)
        {
            return LoopWith(loopMode, interval);
        }

        /// <summary>
        /// Tweenの値の変化を設定します。
        /// </summary>
        /// <param name="begin">補間の開始値</param>
        /// <param name="final">補間の終了値</param>
        /// <param name="duration">補間が行われる時間</param>
        /// <returns><c>this</c></returns>
        public ITween<T> FromTo(T begin, T final, float duration)
        {
            Begin = begin;
            Final = final;
            Duration = duration;
            return this;
        }

        /// <summary>
        /// Tweenの値の変化量を設定します。
        /// </summary>
        /// <param name="final">補間の終了値</param>
        /// <param name="duration">補間が行われる時間</param>
        /// <returns><c>this</c></returns>
        public ITween<T> To(T final, float duration)
        {
            Final = final;
            Duration = duration;
            return this;
        }

        /// <summary>
        /// Tweenの値の変化量を設定します。
        /// </summary>
        /// <param name="begin">補間の開始値</param>
        /// <param name="change">補間による値の変化量</param>
        /// <param name="duration">補間が行われる時間</param>
        /// <returns><c>this</c></returns>
        public virtual ITween<T> Toward(T begin, T change, float duration)
        {
            Begin = begin;
            Change = change;
            Duration = duration;
            return this;
        }

        /// <summary>
        /// Tweenの値の変化量を設定します。
        /// </summary>
        /// <param name="change">補間による値の変化量</param>
        /// <param name="duration">補間が行われる時間</param>
        /// <returns><c>this</c></returns>
        public virtual ITween<T> Toward(T change, float duration)
        {
            Change = change;
            Duration = duration;
            return this;
        }

#endregion

#region control method

        /// <summary>
        /// Tweenの状態を強制的に特定の状態に設定します。
        /// </summary>
        /// <param name="force">Tweenが開始してからの経過時間</param>
        public virtual void Rewind(float force = 0.0f)
        {
            Behaviour.UpdateDeltaTime(force);
            if (onRewind != null)
            {
                onRewind(this);
            }
            // Rewindのコールバックが設定されていない場合は
            // 値の更新が発生したとして、代わりにUpdateのコールバックを発火する
            NotifyOnUpdate(this);
        }

#endregion

#region continuation methods

        /// <summary>
        /// 開始値と終了値を入れ替えます
        /// </summary>
        /// <returns>ITweenインスタンス</returns>
        public ITween<T> PingPong()
        {
            var backTo = Begin;
            Begin = Final;
            Final = backTo;
            return this;
        }

        /// <summary>
        /// 開始値と終了値を入れ替えます
        /// </summary>
        /// <returns>ITweenインスタンス</returns>
        public ITween<T> Yoyo()
        {
            if (MotionType == MotionType.Custom)
            {
                customMotion = TweenUtils.MirrorAnimationCurve(customMotion);
                return this;
            }

            var backTo = Begin;
            Begin = Final;
            Final = backTo;

            switch (EasingType)
            {
                case EasingType.EaseIn:
                    EasingType = EasingType.EaseOut;
                    break;
                case EasingType.EaseOut:
                    EasingType = EasingType.EaseIn;
                    break;
                default:
                    break;
            }
            return this;
        }

#endregion

#region event handler methods

        /// <summary>
        /// Tweenの状態を強制的に指定した際のコールバックを設定します。
        /// </summary>
        /// <param name="callback">Tweenの状態を強制的に指定した際のコールバック</param>
        /// <returns><c>this</c></returns>
        public virtual ITween<T> OnRewind(Action<ITween<T>> callback)
        {
            onRewind += callback;
            return this;
        }

        /// <summary>
        /// Tweenの状態を強制的に指定した際のコールバックを破棄します。
        /// </summary>
        /// <param name="callback">Tweenの状態を強制的に指定した際のコールバック</param>
        /// <returns><c>this</c></returns>
        public virtual ITween<T> RemoveOnRewind(Action<ITween<T>> callback)
        {
            onRewind -= callback;
            return this;
        }

        /// <summary>
        /// Tweenの状態を強制的に指定した際のコールバックを全て破棄します。
        /// </summary>
        /// <returns><c>this</c></returns>
        public virtual ITween<T> RemoveAllOnRewind()
        {
            onRewind = null;
            return this;
        }

        /// <summary>開始時のイベントハンドラーを削除します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> OnStart(Action<ITween<T>> callback)
        {
            // C# 4.0 になれば Action<T> が Action<in T> になるので、
            // IMovement<out TValue, TDerived> の TDerived を out にしてやることができて、
            // ここで新たにクロージャーでくるまなくてよくなる
            ActionTweenBaseTCounter cb;
            if (!onStartCallbacks.TryGetValue(callback, out cb))
            {
                cb = new ActionTweenBaseTCounter(tw => callback(tw), 0);
                onStartCallbacks[callback] = cb;
            }

            cb.Count++;
            return OnStart(cb.Element);
        }

        /// <summary>開始時のイベントハンドラーを削除します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> RemoveOnStart(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onStartCallbacks.TryGetValue(callback, out cb))
            {
                return derived;
            }

            cb.Count--;
            if (cb.Count == 0)
            {
                onStartCallbacks.Remove(callback);
            }

            return RemoveOnStart(cb.Element);
        }

        /// <summary>開始時のイベントハンドラーを全て削除します</summary>
        /// <returns><c>this</c></returns>
        public override TweenBase<T> RemoveAllOnStart()
        {
            onStartCallbacks.Clear();
            return base.RemoveAllOnStart();
        }

        /// <summary>開始時のイベントハンドラーを全て削除します</summary>
        /// <returns><c>this</c></returns>
        ITween<T> IMovement<T, ITween<T>>.RemoveAllOnStart()
        {
            return RemoveAllOnStart();
        }

        /// <summary>値が変化した際のイベントハンドラーを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> OnUpdate(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onUpdateCallbacks.TryGetValue(callback, out cb))
            {
                cb = new ActionTweenBaseTCounter(tw => callback(tw), 0);
                onUpdateCallbacks[callback] = cb;
            }

            cb.Count++;
            return OnUpdate(cb.Element);
        }

        /// <summary>値が変化した際のイベントハンドラーを破棄します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> RemoveOnUpdate(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onUpdateCallbacks.TryGetValue(callback, out cb))
            {
                return derived;
            }

            cb.Count--;
            if (cb.Count == 0)
            {
                onUpdateCallbacks.Remove(callback);
            }

            return RemoveOnUpdate(cb.Element);
        }

        /// <summary>値が変化した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        public override TweenBase<T> RemoveAllOnUpdate()
        {
            onUpdateCallbacks.Clear();
            return base.RemoveAllOnUpdate();
        }

        /// <summary>値が変化した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        ITween<T> IMovement<T, ITween<T>>.RemoveAllOnUpdate()
        {
            return RemoveAllOnUpdate();
        }

        /// <summary>ループ指定なしにおいて完了した際のイベントハンドラを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> OnComplete(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onCompleteCallbacks.TryGetValue(callback, out cb))
            {
                cb = new ActionTweenBaseTCounter(tw => callback(tw), 0);
                onCompleteCallbacks[callback] = cb;
            }

            cb.Count++;
            return OnComplete(cb.Element);
        }

        /// <summary>ループ指定なしにおいて完了した際のイベントハンドラーを破棄します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> RemoveOnComplete(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onCompleteCallbacks.TryGetValue(callback, out cb))
            {
                return derived;
            }

            cb.Count--;
            if (cb.Count == 0)
            {
                onCompleteCallbacks.Remove(callback);
            }

            return RemoveOnComplete(cb.Element);
        }

        /// <summary>ループ指定なしにおいて完了した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        public override TweenBase<T> RemoveAllOnComplete()
        {
            onCompleteCallbacks.Clear();
            return base.RemoveAllOnComplete();
        }

        /// <summary>ループ指定なしにおいて完了した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        ITween<T> IMovement<T, ITween<T>>.RemoveAllOnComplete()
        {
            return RemoveAllOnComplete();
        }

        /// <summary>外部から強制的に終了した場合のイベントハンドラを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> OnKill(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onKillCallbacks.TryGetValue(callback, out cb))
            {
                cb = new ActionTweenBaseTCounter(tw => callback(tw), 0);
                onKillCallbacks[callback] = cb;
            }

            cb.Count++;
            return OnKill(cb.Element);
        }

        /// <summary>強制的に停止した際のイベントハンドラーを破棄します</summary>
        /// <returns><c>this</c></returns>
        public ITween<T> RemoveOnKill(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onKillCallbacks.TryGetValue(callback, out cb))
            {
                return derived;
            }

            cb.Count--;
            if (cb.Count == 0)
            {
                onKillCallbacks.Remove(callback);
            }

            return RemoveOnKill(cb.Element);
        }

        /// <summary>強制的に停止した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        public override TweenBase<T> RemoveAllOnKill()
        {
            onKillCallbacks.Clear();
            return base.RemoveAllOnKill();
        }

        /// <summary>強制的に停止した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        ITween<T> IMovement<T, ITween<T>>.RemoveAllOnKill()
        {
            return RemoveAllOnKill();
        }

        /// <summary>開始、再開時のイベントハンドラーを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> OnResume(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onResumeCallbacks.TryGetValue(callback, out cb))
            {
                cb = new ActionTweenBaseTCounter(tw => callback(tw), 0);
                onResumeCallbacks[callback] = cb;
            }

            cb.Count++;
            return OnResume(cb.Element);
        }

        /// <summary>再開時のイベントハンドラーを削除します。</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> RemoveOnResume(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onResumeCallbacks.TryGetValue(callback, out cb))
            {
                return derived;
            }

            cb.Count--;
            if (cb.Count == 0)
            {
                onResumeCallbacks.Remove(callback);
            }

            return RemoveOnResume(cb.Element);
        }

        /// <summary>再開時のイベントハンドラーを全て削除します。</summary>
        /// <returns><c>this</c></returns>
        public override TweenBase<T> RemoveAllOnResume()
        {
            onResumeCallbacks.Clear();
            return base.RemoveAllOnResume();
        }

        /// <summary>再開時のイベントハンドラーを全て削除します。</summary>
        /// <returns><c>this</c></returns>
        ITween<T> IMovement<T, ITween<T>>.RemoveAllOnResume()
        {
            return RemoveAllOnResume();
        }

        /// <summary>一時停止時のイベントハンドラーを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> OnPause(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onPauseCallbacks.TryGetValue(callback, out cb))
            {
                cb = new ActionTweenBaseTCounter(tw => callback(tw), 0);
                onPauseCallbacks[callback] = cb;
            }

            cb.Count++;
            return OnPause(cb.Element);
        }

        /// <summary>一時停止時のイベントハンドラーを削除します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> RemoveOnPause(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onPauseCallbacks.TryGetValue(callback, out cb))
            {
                return derived;
            }

            cb.Count--;
            if (cb.Count == 0)
            {
                onPauseCallbacks.Remove(callback);
            }

            return RemoveOnPause(cb.Element);
        }

        /// <summary>一時停止時のイベントハンドラーを全て削除します。</summary>
        /// <returns><c>this</c></returns>
        public override TweenBase<T> RemoveAllOnPause()
        {
            onPauseCallbacks.Clear();
            return base.RemoveAllOnPause();
        }

        /// <summary>一時停止時のイベントハンドラーを全て削除します。</summary>
        /// <returns><c>this</c></returns>
        ITween<T> IMovement<T, ITween<T>>.RemoveAllOnPause()
        {
            return RemoveAllOnPause();
        }

        /// <summary>開始してから値の変化が始まるまでの遅延が終了した際のイベントハンドラーを設定します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> OnEndDelay(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onEndDelayCallbacks.TryGetValue(callback, out cb))
            {
                cb = new ActionTweenBaseTCounter(tw => callback(tw), 0);
                onEndDelayCallbacks[callback] = cb;
            }

            cb.Count++;
            return OnEndDelay(cb.Element);
        }

        /// <summary>開始してから値の変化が始まるまでの遅延が終了した際のイベントハンドラーを破棄します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public ITween<T> RemoveOnEndDelay(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onEndDelayCallbacks.TryGetValue(callback, out cb))
            {
                return derived;
            }

            cb.Count--;
            if (cb.Count == 0)
            {
                onEndDelayCallbacks.Remove(callback);
            }

            return RemoveOnEndDelay(cb.Element);
        }

        /// <summary>開始してから値の変化が始まるまでの遅延が終了した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        public override TweenBase<T> RemoveAllOnEndDelay()
        {
            onEndDelayCallbacks.Clear();
            return base.RemoveAllOnEndDelay();
        }

        /// <summary>開始してから値の変化が始まるまでの遅延が終了した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        ITween<T> IMovement<T, ITween<T>>.RemoveAllOnEndDelay()
        {
            return RemoveAllOnEndDelay();
        }

        /// <summary>ループする設定であった場合の、一周期ごとのイベントハンドラを追加します</summary>
        /// <returns><c>this</c></returns>
        public ITween<T> OnLoopComplete(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onLoopCompleteCallbacks.TryGetValue(callback, out cb))
            {
                cb = new ActionTweenBaseTCounter(tw => callback(tw), 0);
                onLoopCompleteCallbacks[callback] = cb;
            }

            cb.Count++;
            return OnLoopComplete(cb.Element);
        }

        /// <summary>ループする設定であった場合の、一周期ごとのイベントハンドラを破棄します</summary>
        /// <returns><c>this</c></returns>
        public ITween<T> RemoveOnLoopComplete(Action<ITween<T>> callback)
        {
            ActionTweenBaseTCounter cb;
            if (!onLoopCompleteCallbacks.TryGetValue(callback, out cb))
            {
                return derived;
            }

            cb.Count--;
            if (cb.Count == 0)
            {
                onLoopCompleteCallbacks.Remove(callback);
            }

            return RemoveOnLoopComplete(cb.Element);
        }

        /// <summary>ループする設定であった場合の、一周期ごとのイベントハンドラを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        public override TweenBase<T> RemoveAllOnLoopComplete()
        {
            onLoopCompleteCallbacks.Clear();
            return base.RemoveAllOnLoopComplete();
        }

        /// <summary>ループする設定であった場合の、一周期ごとのイベントハンドラを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        ITween<T> IPeriodicMovement<T, ITween<T>>.RemoveAllOnLoopComplete()
        {
            return RemoveAllOnLoopComplete();
        }

#endregion

#region protected Methods

        /// <summary>
        /// Tweenの進捗度を0～1で返します。
        /// </summary>
        /// <param name="time">0～Durationの時間</param>
        /// <returns>進捗度</returns>
        protected float Progress(float time)
        {
            float progress = time / Duration;

            // easingFuncが設定されている場合はそれを優先
            if (MotionType == MotionType.Custom && easingFunc != null)
            {
                return easingFunc(progress);
            }

            return behaviour.Progress(progress);
        }

        /// <summary>動作用クラスを生成します</summary>
        protected override void CreateBehaviour()
        {
            switch (LoopMode)
            {
                case LoopMode.Restart:
                    behaviour = new TweenLoopBehaviour<T>(this);
                    break;
                case LoopMode.PingPong:
                    behaviour = new TweenPingPongBehaviour<T>(this);
                    break;
                case LoopMode.Yoyo:
                    behaviour = new TweenYoyoBehaviour<T>(this);
                    break;
                case LoopMode.NoLoop:
                default:
                    behaviour = new TweenBehaviour<T>(this);
                    break;
            }
            behaviour.Init();
        }

        /// <summary>
        /// <see cref="IPeriodicMovementLoopBehaviour{TValue, TDerived}"/> にキャストした
        /// <see cref="Behaviour" /> を返します
        /// </summary>
        protected override IPeriodicMovementLoopBehaviour<T, TweenBase<T>> LoopBehaviour
        {
            get { return behaviour as TweenLoopBehaviour<T>; }
        }

#endregion

#region implements ITween

        /// <summary>
        /// トゥイーンの変化の緩急を設定します
        /// </summary>
        ITween ITween.EasingWith(MotionType motionType, EasingType easingType)
        {
            return EasingWith(motionType, easingType);
        }

        /// <summary>
        /// トゥイーンの変化の緩急を設定します
        /// </summary>
        ITween ITween.EasingWith(MotionType motionType)
        {
            return EasingWith(motionType);
        }

        /// <summary>
        /// トゥイーンの変化の緩急を設定します
        /// </summary>
        ITween ITween.EasingWith(EasingType easingType)
        {
            return EasingWith(easingType);
        }

#endregion

#region classes

        /// <summary>
        /// <c>Action<TweenBase<T>></c> と対応する登録回数のペア
        /// </summary>
        class ActionTweenBaseTCounter
        {
            public Action<TweenBase<T>> Element { get; private set; }

            public int Count { get; set; }

            public ActionTweenBaseTCounter(Action<TweenBase<T>> element, int count)
            {
                Element = element;
                Count = count;
            }
        }

#endregion
    }
}
