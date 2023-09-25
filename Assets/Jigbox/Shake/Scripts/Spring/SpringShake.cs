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
using Jigbox.Tween;

namespace Jigbox.Shake
{
    [Serializable]
    public abstract class SpringShake<TValue, TDerived>
        : PeriodicMovement<TValue, TDerived>, IShake<TValue, EnvelopedWave, TDerived>
        where TDerived : SpringShake<TValue, TDerived>
    {
#region properties

        /// <summary>ループするか、しないかを指定することができます</summary>
        /// <exception cref="NotSupportedException"><see cref="Tween.LoopMode.PingPong"/> もしくは <see cref="Tween.LoopMode.Yoyo"/> を set しようとした場合</exception>
        public override LoopMode LoopMode
        {
            get { return base.LoopMode; }
            set
            {
                switch (value)
                {
                    case LoopMode.PingPong:
                    case LoopMode.Yoyo:
                        throw new NotSupportedException();
                    default:
                        base.LoopMode = value;
                        break;
                }
            }
        }

        /// <summary>基本となる正弦波</summary>
        [HideInInspector]
        [SerializeField]
        protected EnvelopedWave wave = new EnvelopedWave();

        /// <summary>基本となる正弦波</summary>
        public virtual EnvelopedWave Wave
        {
            get { return wave; }
            set { wave = value; }
        }

        /// <summary振動の原点</summary>
        [HideInInspector]
        [SerializeField]
        protected TValue origin;

        /// <summary振動の原点</summary>
        public virtual TValue Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        /// <summary>
        /// <see cref="IPeriodicMovementLoopBehaviour{TValue, TDerived}"/> にキャストした
        /// <see cref="Behaviour" /> を返します
        /// </summary>
        protected override IPeriodicMovementLoopBehaviour<TValue, TDerived> LoopBehaviour
        {
            get { return Behaviour as SpringShakeLoopBehaviour<TValue, TDerived>; }
        }

#endregion

#region constructors

        /// <summary>コンストラクター</summary>
        protected SpringShake()
        {
            InitializeProperties();
        }

        /// <summary><c>OnUpdate</c> のイベントハンドラーの登録を行うコンストラクター</summary>
        /// <param name="onUpdate"><c>OnUpdate</c> のイベントハンドラー</param>
        protected SpringShake(Action<TDerived> onUpdate) : this()
        {
            OnUpdate(onUpdate);
        }

        /// <summary>コピーコンストラクター</summary>
        /// <param name="other">コピー元のインスタンス</param>
        protected SpringShake(TDerived other) : this()
        {
            FollowTimeScale = other.FollowTimeScale;
            Duration = other.Duration;
            Delay = other.Delay;
            Interval = other.Interval;
            LoopMode = other.LoopMode;
            Wave = other.Wave;
            Origin = other.Origin;
        }

        /// <summary><c>OnUpdate</c> のイベントハンドラーの登録を行うコピーコンストラクター</summary>
        /// <param name="other">コピー元のインスタンス</param>
        /// <param name="onUpdate"><c>OnUpdate</c> のイベントハンドラー</param>
        protected SpringShake(TDerived other, Action<TDerived> onUpdate) : this(other)
        {
            OnUpdate(onUpdate);
        }

#endregion

#region protected methods

        /// <summary>プロパティを初期化する</summary>
        protected virtual void InitializeProperties()
        {
            Wave.Progress = time => time / Duration;
        }

        /// <summary>包絡線の乗算された正弦波関数</summary>
        /// <param name="time">経過時間</param>
        /// <returns>関数の値</returns>
        protected float CalculateWave(float time)
        {
            return Wave.Calculate(time);
        }

        /// <summary>動作用クラスを生成します</summary>
        protected override void CreateBehaviour()
        {
            switch (LoopMode)
            {
                case LoopMode.NoLoop:
                    behaviour = new SpringShakeBehaviour<TValue, TDerived>(derived);
                    break;
                default:
                    // Spring Shake が Ping Pong, Yoyo をサポートする場合はここの場合分けを増やす
                    behaviour = new SpringShakeLoopBehaviour<TValue, TDerived>(derived);
                    break;
            }
            behaviour.Init();
        }

#endregion
    }
}
