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
using UnityEngine;

namespace Jigbox.Tween
{
    public abstract class PeriodicMovement<TValue, TDerived> :
        Movement<TValue, TDerived>, IPeriodicMovement<TValue, TDerived>
        where TDerived : PeriodicMovement<TValue, TDerived>
    {
#region configuration properties

        [SerializeField]
        [HideInInspector]
        float duration;

        /// <summary>何秒かけて変化するかの時間を指定することができます</summary>
        public float Duration
        {
            get { return duration; }

            set
            {
                if (value < 0)
                {
                    var msg = string.Format("Duration should be over 0. but ({0})", value);
                    throw new ArgumentOutOfRangeException("value", msg);
                }
                duration = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        float interval;

        /// <summary>終了後の余韻を秒単位の時間で指定できます。ループ動作の間隔時間や、終了時コールバックの遅延時間としても用いられます</summary>
        public float Interval
        {
            get { return interval; }

            set
            {
                if (value < 0)
                {
                    var msg = string.Format("Grace should be over 0. but ({0})", value);
                    throw new ArgumentOutOfRangeException("value", msg);
                }
                interval = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        LoopMode loopMode;

        /// <summary>ループするか、しないかを指定することができます</summary>
        public virtual LoopMode LoopMode
        {
            get { return loopMode; }

            set
            {
                if (loopMode != value)
                {
                    loopMode = value;
                    // ループ状態が変更された場合は、動作用クラスの再生成が必須
                    behaviour = null;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        int loopCount;

        /// <summary>
        /// <para>ループする際に何回ループを行うか</para>
        /// <para>1回以上を指定することで指定回数ループしたら動作を終了させます。</para>
        /// </summary>
        public int LoopCount
        {
            get { return loopCount; }

            set
            {
                if (value < 0)
                {
                    var msg = string.Format("LoopCount should be over 0. but ({0})", value);
                    throw new ArgumentOutOfRangeException("value", msg);
                }
                loopCount = value;
            }
        }

#endregion

#region state properties

        /// <summary>現在のフレームにおける値</summary>
        public override TValue Value { get { return behaviour.Value; } }

        /// <summary>
        /// <para>開始してから終了するまでに掛かる全ての時間間隔（秒）を示します</para>
        /// <para>ループする場合、Duration + Interval、しない場合、Duration + Delayとなります。</para>
        /// </summary>
        public float TotalSpan { get { return behaviour.ProgressSpan; } }

        /// <summary><see cref="Delay"/>を含め<see cref="Start"/>からの経過時間（秒）</summary>
        public override float DeltaTime { get { return behaviour.DeltaTime; } }

#endregion

#region behaviour properties

        /// <summary>動作用クラス</summary>
        protected IPeriodicMovementBehaviour<TValue, TDerived> behaviour;

        /// <summary>動作用クラス</summary>
        protected IPeriodicMovementBehaviour<TValue, TDerived> Behaviour
        {
            get
            {
                if (behaviour == null)
                {
                    // コンストラクタタイミングではシリアライズ情報が正しく展開されておらず、
                    // 正しい動作用クラスが生成できないため、プロパティアクセス時に生成
                    CreateBehaviour();
                }
                return behaviour;
            }
        }

        /// <summary>
        /// <see cref="IPeriodicMovementLoopBehaviour{TValue, TDerived}"/> にキャストした
        /// <see cref="Behaviour" /> を返します
        /// </summary>
        protected abstract IPeriodicMovementLoopBehaviour<TValue, TDerived> LoopBehaviour { get; }

#endregion

#region event handler field

#pragma warning disable 67 // 抽象クラス下のイベントハンドラは CS0067 警告を出すため

        /// <summary>ループしているTweenの1回分の補間が完了した際のコールバック</summary>
        event Action<TDerived> onLoopComplete;

#pragma warning restore 67

#endregion

#region event handler methods

        /// <summary>ループする設定であった場合の、一周期ごとのイベントハンドラを追加します</summary>
        /// <returns><c>this</c></returns>
        public TDerived OnLoopComplete(Action<TDerived> callback)
        {
            onLoopComplete += callback;
            return derived;
        }

        /// <summary>ループする設定であった場合の、一周期ごとのイベントハンドラを破棄します</summary>
        /// <returns><c>this</c></returns>
        public TDerived RemoveOnLoopComplete(Action<TDerived> callback)
        {
            onLoopComplete -= callback;
            return derived;
        }

        /// <summary>ループする設定であった場合の、一周期ごとのイベントハンドラを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        public virtual TDerived RemoveAllOnLoopComplete()
        {
            onLoopComplete = null;
            return derived;
        }

#endregion

#region methods for callbacks of state machine

        /// <summary>動作開始時に呼び出されます。</summary>
        protected override void OnEnterStart()
        {
            var behaviour = Behaviour;
            behaviour.Init();
            behaviour.UpdateDeltaTime(0.0f);
            TweenWorker.Add(this);
            NotifyOnStart(derived);
            // Tween.Startを実行するタイミング次第では、
            // そのフレームでの更新処理が行われない可能性があるので、
            // Startと同時に一度だけ更新用のコールバックを発火する
            NotifyOnUpdate(derived);

            if (!stateMachine.Changed)
            {
                stateMachine.Set(TweenState.Working);
            }
        }

        /// <summary>更新中に呼び出されます</summary>
        /// <param name="deltaTime">経過時間</param>
        protected override void OnUpdateWorking(float deltaTime)
        {
            var behaviour = Behaviour;
            float lastDeltaTime = behaviour.DeltaTime;
            bool isComplete = behaviour.UpdateDeltaTime(behaviour.DeltaTime + deltaTime);

            if (Delay > 0.0f && lastDeltaTime < Delay && behaviour.DeltaTime >= Delay)
            {
                NotifyOnEndDelay(derived);
            }
            if (!behaviour.IsCached)
            {
                NotifyOnUpdate(derived);
            }

            if (loopMode == LoopMode.NoLoop)
            {
                if (isComplete && !stateMachine.Changed)
                {
                    stateMachine.Set(TweenState.Complete);
                }
            }
            else
            {
                if (isComplete)
                {
                    var loopBehaviour = LoopBehaviour;
                    if (onLoopComplete != null)
                    {
                        for (int i = 0; i < loopBehaviour.LastCompleteCount; ++i)
                        {
                            onLoopComplete(derived);
                        }
                    }

                    // loopCountが0以上でなければそもそも状態は遷移しないが、
                    // この箇所に到達している時点で、loopBehaviour.LoopCountは1以上なので
                    // loopCount > 0は条件から省略
                    if (loopCount == loopBehaviour.LoopCount && !stateMachine.Changed)
                    {
                        stateMachine.Set(TweenState.Complete);
                    }
                }
            }
        }

        /// <summary>完了した際に呼び出されます</summary>
        protected override void OnEnterComplete()
        {
            // ループ状態で回数指定がない場合は、完了の定義がないため、コールバックは発火しない
            if ((loopMode == LoopMode.NoLoop || LoopCount > 0))
            {
                NotifyOnComplete(derived);
            }

            // コールバック内でStart()等を叩かれた場合、そちらの状態遷移を優先
            if (!stateMachine.Changed)
            {
                stateMachine.Set(TweenState.Done);
            }
        }

        /// <summary>強制的に完了させられた際に呼び出されます</summary>
        protected override void OnEnterForceComplete()
        {
            var behaviour = Behaviour;
            float lastDeltaTime = behaviour.DeltaTime;

            bool isLooped = LoopMode != LoopMode.NoLoop;

            // 直接Completeを呼んだ場合は経過時間に総処理時間を設定して
            // 何らかの変化があれば、一度更新処理を呼び出す
            // ループ状態の場合は、完了の定義がないため、特に何もしない
            // ループ状態でもループ回数が指定されている場合は、
            // 指定回数分ループした状態まで進める
            if (!isLooped || LoopCount > 0)
            {
                if (isLooped)
                {
                    var loopBehaviour = LoopBehaviour;
                    loopBehaviour.UpdateDeltaTime(loopBehaviour.TotalSpan);
                }
                else
                {
                    behaviour.UpdateDeltaTime(TotalSpan);
                }

                // 参照した際の値がズレないように、Delayの終了コールバックは
                // DeltaTimeを更新してからコールバックを呼び出す
                if (Delay > 0.0f && lastDeltaTime < Delay)
                {
                    NotifyOnEndDelay(derived);
                }
                if (!behaviour.IsCached)
                {
                    NotifyOnUpdate(derived);
                }

                if (isLooped && onLoopComplete != null)
                {
                    var loopBehaviour = LoopBehaviour;
                    for (int i = 0; i < loopBehaviour.LastCompleteCount; ++i)
                    {
                        onLoopComplete(derived);
                    }
                }
            }
            else
            {
                if (Delay > 0.0f && lastDeltaTime < Delay)
                {
                    NotifyOnEndDelay(derived);
                }
            }

            if (!stateMachine.Changed)
            {
                stateMachine.Set(TweenState.Complete);
            }
        }

#endregion

#region configuration utility methods

        /// <summary>ループを指定します</summary>
        public TDerived LoopWith(LoopMode loopMode, float interval)
        {
            Interval = interval;
            LoopMode = loopMode;
            return derived;
        }

        /// <summary>ループを指定します</summary>
        public TDerived LoopWith(LoopMode loopMode)
        {
            LoopMode = loopMode;
            return derived;
        }

        /// <summary>ループを指定します</summary>
        IPeriodicMovement IPeriodicMovement.LoopWith(LoopMode loopMode, float interval)
        {
            return LoopWith(loopMode, interval);
        }

        /// <summary>ループを指定します</summary>
        IPeriodicMovement IPeriodicMovement.LoopWith(LoopMode loopMode)
        {
            return LoopWith(loopMode);
        }

#endregion

#region value methods

        /// <summary>
        /// 動作時間における値を返します。
        /// </summary>
        /// <param name="time">0～Durationの時間</param>
        /// <returns>値</returns>
        public abstract override TValue ValueAt(float time);

#endregion

#region protected methods

        /// <summary>動作用クラスを生成します</summary>
        protected abstract void CreateBehaviour();

#endregion
    }
}
