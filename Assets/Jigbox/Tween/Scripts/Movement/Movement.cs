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
    public abstract class Movement<TValue, TDerived>
        : IMovement<TValue, TDerived>
        where TDerived : Movement<TValue, TDerived>
    {
#region configuration properties

        [SerializeField]
        [HideInInspector]
        float delay;

        /// <summary>開始までに遅延時間を秒単位で指定することができます</summary>
        /// <value>遅延時間</value>
        public float Delay
        {
            get
            {
                return delay;
            }
            set
            {
                delay = Mathf.Max(value, 0f);
            }
        }

        [SerializeField]
        [HideInInspector]
        bool followTimeScale;

        /// <summary>値の変化を<see cref="UnityEngine.Time.timeScale"/>に依存するか否かを指定することができます</summary>
        /// <value><c>true</c> if this instance is follow time scale; otherwise, <c>false</c>.</value>
        public bool FollowTimeScale
        {
            get { return followTimeScale; }
            set { followTimeScale = value; }
        }

#endregion

#region state properties

        /// <summary>参照時点での状態に応じた値を示します</summary>
        /// <value>値</value>
        public abstract TValue Value { get; }

        /// <summary><see cref="Delay"/>を含め<see cref="Start"/>からの経過時間（秒）</summary>
        public abstract float DeltaTime { get; }

        /// <summary>現在の状態を示します</summary>
        public TweenState State { get { return stateMachine.StateType; } }

#endregion

#region event handler fields

#pragma warning disable 67 // 抽象クラス下のイベントハンドラは CS0067 警告を出すため

        /// <summary>開始した際のコールバック</summary>
        event Action<TDerived> onStart;

        /// <summary>開始してから値の変化が始まるまでの遅延が終了した際のコールバック</summary>
        event Action<TDerived> onEndDelay;

        /// <summary>値が変化した際のコールバック</summary>
        event Action<TDerived> onUpdate;

        /// <summary>動作が完了した際のコールバック</summary>
        event Action<TDerived> onComplete;

        /// <summary>強制的に停止した際のコールバック</summary>
        event Action<TDerived> onKill;

        /// <summary>一時停止状態から再開した際のコールバック</summary>
        event Action<TDerived> onResume;

        /// <summary>一時停止した際のコールバック</summary>
        event Action<TDerived> onPause;

#pragma warning restore 67

#endregion

#region fields

        /// <summary><c>TDerived</c> 型にキャストした <c>this</c></summary>
        protected readonly TDerived derived;

        /// <summary>ステートマシン</summary>
        protected TweenStateMachine stateMachine = new TweenStateMachine();

#endregion

#region event handler methods

        /// <summary>開始時のイベントハンドラーを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived OnStart(Action<TDerived> callback)
        {
            onStart += callback;
            return derived;
        }

        /// <summary>開始時のイベントハンドラーを削除します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived RemoveOnStart(Action<TDerived> callback)
        {
            onStart -= callback;
            return derived;
        }

        /// <summary>開始時のイベントハンドラーを全て削除します</summary>
        /// <returns><c>this</c></returns>
        public virtual TDerived RemoveAllOnStart()
        {
            onStart = null;
            return derived;
        }

        /// <summary>開始イベントを発火する</summary>
        /// <param name="self"><c>this</c></param>
        protected void NotifyOnStart(TDerived self)
        {
            if (onStart != null)
            {
                onStart(self);
            }
        }

        /// <summary>値が変化した際のイベントハンドラーを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived OnUpdate(Action<TDerived> callback)
        {
            onUpdate += callback;
            return derived;
        }

        /// <summary>値が変化した際のイベントハンドラーを破棄します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived RemoveOnUpdate(Action<TDerived> callback)
        {
            onUpdate -= callback;
            return derived;
        }

        /// <summary>値が変化した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        public virtual TDerived RemoveAllOnUpdate()
        {
            onUpdate = null;
            return derived;
        }

        /// <summary>値の更新イベントを発火する</summary>
        /// <param name="self"><c>this</c></param>
        protected void NotifyOnUpdate(TDerived self)
        {
            if (onUpdate != null)
            {
                onUpdate(self);
            }
        }

        /// <summary>ループ指定なしにおいて完了した際のイベントハンドラを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived OnComplete(Action<TDerived> callback)
        {
            onComplete += callback;
            return derived;
        }

        /// <summary>ループ指定なしにおいて完了した際のイベントハンドラーを破棄します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived RemoveOnComplete(Action<TDerived> callback)
        {
            onComplete -= callback;
            return derived;
        }

        /// <summary>ループ指定なしにおいて完了した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        public virtual TDerived RemoveAllOnComplete()
        {
            onComplete = null;
            return derived;
        }

        /// <summary>完了イベントを発火する</summary>
        /// <param name="self"><c>this</c></param>
        protected void NotifyOnComplete(TDerived self)
        {
            if (onComplete != null)
            {
                onComplete(self);
            }
        }

        /// <summary>外部から強制的に終了した場合のイベントハンドラを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived OnKill(Action<TDerived> callback)
        {
            onKill += callback;
            return derived;
        }

        /// <summary>強制的に停止した際のイベントハンドラーを破棄します</summary>
        /// <returns><c>this</c></returns>
        public TDerived RemoveOnKill(Action<TDerived> callback)
        {
            onKill -= callback;
            return derived;
        }

        /// <summary>強制的に停止した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        public virtual TDerived RemoveAllOnKill()
        {
            onKill = null;
            return derived;
        }

        /// <summary>強制停止イベントを発火する</summary>
        /// <param name="self"><c>this</c></param>
        protected void NotifyOnKill(TDerived self)
        {
            if (onKill != null)
            {
                onKill(self);
            }
        }

        /// <summary>開始、再開時のイベントハンドラーを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived OnResume(Action<TDerived> callback)
        {
            onResume += callback;
            return derived;
        }

        /// <summary>再開時のイベントハンドラーを削除します。</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived RemoveOnResume(Action<TDerived> callback)
        {
            onResume -= callback;
            return derived;
        }

        /// <summary>再開時のイベントハンドラーを全て削除します。</summary>
        /// <returns><c>this</c></returns>
        public virtual TDerived RemoveAllOnResume()
        {
            onResume = null;
            return derived;
        }

        /// <summary>再開イベントを発火する</summary>
        /// <param name="self"><c>this</c></param>
        protected void NotifyOnResume(TDerived self)
        {
            if (onResume != null)
            {
                onResume(self);
            }
        }

        /// <summary>一時停止時のイベントハンドラーを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived OnPause(Action<TDerived> callback)
        {
            onPause += callback;
            return derived;
        }

        /// <summary>一時停止時のイベントハンドラーを削除します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived RemoveOnPause(Action<TDerived> callback)
        {
            onPause -= callback;
            return derived;
        }

        /// <summary>一時停止時のイベントハンドラーを全て削除します。</summary>
        /// <returns><c>this</c></returns>
        public virtual TDerived RemoveAllOnPause()
        {
            onPause = null;
            return derived;
        }

        /// <summary>一時停止イベントを発火する</summary>
        /// <param name="self"><c>this</c></param>
        protected void NotifyOnPause(TDerived self)
        {
            if (onPause != null)
            {
                onPause(self);
            }
        }

        /// <summary>開始してから値の変化が始まるまでの遅延が終了した際のイベントハンドラーを設定します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived OnEndDelay(Action<TDerived> callback)
        {
            onEndDelay += callback;
            return derived;
        }

        /// <summary>開始してから値の変化が始まるまでの遅延が終了した際のイベントハンドラーを破棄します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        public TDerived RemoveOnEndDelay(Action<TDerived> callback)
        {
            onEndDelay -= callback;
            return derived;
        }

        /// <summary>開始してから値の変化が始まるまでの遅延が終了した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        public virtual TDerived RemoveAllOnEndDelay()
        {
            onEndDelay = null;
            return derived;
        }

        /// <summary>遅延終了イベントを発火する</summary>
        /// <param name="self"><c>this</c></param>
        protected void NotifyOnEndDelay(TDerived self)
        {
            if (onEndDelay != null)
            {
                onEndDelay(self);
            }
        }

#endregion

#region control methods

        /// <summary>有効になってる際に、フレームごとに前フレーム描画からの差分時間を累積し、状態を更新させます</summary>
        /// <remarks><see cref="Tween.TweenWorker"/>以外からこのAPIは呼び出さないようにしてください</remarks>
        /// <param name="deltaTime">seconds from last update</param>
        public virtual void Update(float deltaTime)
        {
            stateMachine.Update(deltaTime);
        }

        /// <summary>初期状態から稼働させます</summary>
        public virtual void Start()
        {
            stateMachine.Set(TweenState.Start);
        }

        /// <summary>休止状態から稼働させます</summary>
        public virtual void Resume()
        {
            stateMachine.Set(TweenState.Resume);
        }

        /// <summary>一時停止させます</summary>
        public virtual void Pause()
        {
            stateMachine.Set(TweenState.Paused);
        }

        /// <summary>最終状態にして終了します</summary>
        public virtual void Complete()
        {
            stateMachine.Set(TweenState.ForceComplete);
        }

        /// <summary>内部の進行状態に関わらず、動作を終了させます</summary>
        public virtual void Kill()
        {
            stateMachine.Set(TweenState.Kill);
        }

#endregion

#region value method

        /// <summary>引数に与えられた経過時間での値を計算します</summary>
        /// <returns>経過時間での値</returns>
        /// <param name="time">秒単位の時間</param>
        public abstract TValue ValueAt(float time);

#endregion

#region constructors

        /// <summary>コンストラクター</summary>
        protected Movement()
        {
            derived = this as TDerived;
#if UNITY_EDITOR
            if (derived == null)
            {
                Debug.LogError("the type of \"this\" must be \"TDerived\"");
            }
#endif
            InitStates();
        }

#endregion

#region protected methods

        /// <summary>各種状態を初期化します</summary>
        protected virtual void InitStates()
        {
            stateMachine.Add(TweenState.Start, OnEnterStart, null, null);
            stateMachine.Add(TweenState.Working, null, OnUpdateWorking, null, (int) TweenState.Start | (int) TweenState.Resume);
            stateMachine.Add(TweenState.Resume, OnEnterResume, null, null, (int) TweenState.Paused);
            stateMachine.Add(TweenState.Paused, OnEnterPause, null, null, (int) TweenState.Working);
            stateMachine.Add(TweenState.Kill, OnEnterKill, null, null, (int) TweenState.All & ~((int) TweenState.Done));
            stateMachine.Add(TweenState.Complete, OnEnterComplete, null, null, (int) TweenState.All & ~((int) TweenState.Done));
            stateMachine.Add(TweenState.ForceComplete, OnEnterForceComplete, null, null,
                (int) TweenState.All & ~((int) TweenState.Complete | (int) TweenState.Done));
            stateMachine.Add(TweenState.Done);
        }

#endregion

#region methods for callbacks of state machine

        /// <summary>動作開始時に呼び出されます。</summary>
        protected abstract void OnEnterStart();

        /// <summary>更新中に呼び出されます</summary>
        /// <param name="deltaTime">経過時間</param>
        protected abstract void OnUpdateWorking(float deltaTime);

        /// <summary>一時停止状態から再開された際に呼び出されます</summary>
        protected virtual void OnEnterResume()
        {
            NotifyOnResume(derived);

            if (!stateMachine.Changed)
            {
                stateMachine.Set(TweenState.Working);
            }
        }

        /// <summary>一時停止状態になった際に呼び出されます</summary>
        protected virtual void OnEnterPause()
        {
            NotifyOnPause(derived);
        }

        /// <summary>強制的に終了させられた際に呼び出されます</summary>
        protected virtual void OnEnterKill()
        {
            NotifyOnKill(derived);

            // コールバック内でStart()等を叩かれた場合、そちらの状態遷移を優先
            if (!stateMachine.Changed)
            {
                stateMachine.Set(TweenState.Done);
            }
        }

        /// <summary>完了した際に呼び出されます</summary>
        protected abstract void OnEnterComplete();

        /// <summary>強制的に完了させられた際に呼び出されます</summary>
        protected abstract void OnEnterForceComplete();

        #endregion
    }
}
