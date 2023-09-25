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
using UnityEngine;

namespace Jigbox.Shake
{
    [Serializable]
    public class WaveShake : Movement<float, WaveShake>, IShake<float, Wave, WaveShake>
    {
#region configuration properties

        /// <summary>基本となる正弦波</summary>
        [SerializeField]
        [HideInInspector]
        protected Wave wave = new Wave();

        /// <summary>基本となる正弦波</summary>
        public Wave Wave
        {
            get { return wave; }
            set { wave = value; }
        }

        /// <summary>振動の原点</summary>
        [SerializeField]
        [HideInInspector]
        protected float origin;

        /// <summary>振動の原点</summary>
        public float Origin
        {
            get { return origin; }
            set { origin = value; }
        }

#endregion

#region state properties

        /// <summary>キャッシュされた値</summary>
        protected float value;

        /// <summary>参照時点での状態に応じた値を示します</summary>
        /// <value>値</value>
        public override float Value
        {
            get
            {
                if (isValueCached)
                {
                    return value;
                }
                value = ValueAt(progressTime);
                isValueCached = true;
                return value;
            }
        }

        /// <summary><see cref="value"/>の値が有効かどうか</summary>
        protected bool isValueCached;

        /// <summary>振動の開始からの経過時間（<see cref="Movement{TValue,TDerived}.Delay" /> を含まない）</summary>
        protected float progressTime;

        /// <summary><see cref="IMovement.Delay"/>を含め<see cref="IMovement.Start"/>からの経過時間（秒）</summary>
        protected float deltaTime;

        /// <summary><see cref="IMovement.Delay"/>を含め<see cref="IMovement.Start"/>からの経過時間（秒）</summary>
        public override float DeltaTime { get { return deltaTime; } }

#endregion

#region functions for callback of state machine

        /// <summary>動作開始時に呼び出されます。</summary>
        protected override void OnEnterStart()
        {
            Init();
            TweenWorker.Add(this);
            UpdateDeltaTime(0);
            NotifyOnStart(this);
            // Startを実行するタイミング次第では、
            // そのフレームでの更新処理が行われない可能性があるので、
            // Startと同時に一度だけ更新用のコールバックを発火する
            NotifyOnUpdate(this);

            if (!stateMachine.Changed)
            {
                stateMachine.Set(TweenState.Working);
            }
        }

        /// <summary>更新中に呼び出されます。</summary>
        /// <param name="deltaUpdateTime">前回更新時からの時間差分</param>
        protected override void OnUpdateWorking(float deltaUpdateTime)
        {
            var lastDeltaTime = DeltaTime;
            UpdateDeltaTime(lastDeltaTime + deltaUpdateTime);

            if (Delay > 0.0f && lastDeltaTime < Delay && DeltaTime >= Delay)
            {
                NotifyOnEndDelay(this);
            }
            if (!isValueCached)
            {
                NotifyOnUpdate(this);
            }
        }

        /// <summary>完了した際に呼び出されます</summary>
        protected override void OnEnterComplete()
        {
            NotifyOnComplete(derived);

            // コールバック内でStart()等を叩かれた場合、そちらの状態遷移を優先
            if (!stateMachine.Changed)
            {
                stateMachine.Set(TweenState.Done);
            }
        }

        /// <summary>停止させられた際に呼び出されます。</summary>
        protected override void OnEnterForceComplete()
        {
            // 振動が停止させられると元の位置に戻る
            value = origin;
            isValueCached = true;

            if (Delay > 0 && DeltaTime < Delay)
            {
                NotifyOnEndDelay(derived);
            }

            if (!stateMachine.Changed)
            {
                stateMachine.Set(TweenState.Complete);
            }
        }

#endregion

#region constructers

        /// <summary>コンストラクター</summary>
        public WaveShake() {}

#endregion

#region value method

        /// <summary>引数に与えられた経過時間での値を計算します</summary>
        /// <returns>経過時間での値</returns>
        /// <param name="time">秒単位の時間</param>
        public override float ValueAt(float time)
        {
            return Wave.Calculate(time);
        }

#endregion

#region protected methods

        /// <summary>開始からの経過時間をもらい各経過時間を更新します。</summary>
        protected void UpdateDeltaTime(float deltaTime)
        {
            deltaTime = Mathf.Max(0, deltaTime);
            if (this.deltaTime == deltaTime)
            {
                return;
            }
            this.deltaTime = deltaTime;
            progressTime = Mathf.Max(0, deltaTime - Delay);

            isValueCached = false;
        }

        /// <summary>初期化します。</summary>
        protected void Init()
        {
            deltaTime = 0;
            progressTime = 0;
            value = 0;
            isValueCached = true;
            UpdateDeltaTime(0);
        }

#endregion
    }
}
