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

namespace Jigbox.Tween
{
    public abstract class TweenOrder
    {
#region properties

        /// <summary>アニメーションの緩急</summary>
        protected MotionType motionType = MotionType.Linear;

        /// <summary>アニメーションの緩急</summary>
        public MotionType MotionType { get { return motionType; } set { motionType = value; } }

        /// <summary>アニメーションの緩急の付き方</summary>
        protected EasingType easingType = EasingType.EaseInOut;

        /// <summary>アニメーションの緩急の付き方</summary>
        public EasingType EasingType { get { return easingType; } set { easingType = value; } }

        /// <summary>Tweenによる補間が行われる時間(秒)</summary>
        protected float duration = 1.0f;

        /// <summary>Tweenによる補間が行われる時間(秒)</summary>
        public float Duration { get { return duration; } set { duration = value; } }

        /// <summary>Tweenでの補間が開始されるまでの遅延時間(秒)</summary>
        protected float delay = 0.0f;

        /// <summary>Tweenでの補間が開始されるまでの遅延時間(秒)</summary>
        public float Delay { get { return delay; } set { delay = value; } }

        /// <summary>ループする場合に補間が完了してから次の補間が始まるまでの待機時間</summary>
        protected float interval = 0.0f;

        /// <summary>ループする場合に補間が完了してから次の補間が始まるまでの待機時間</summary>
        public float Interval { get { return interval; } set { interval = value; } }

        /// <summary>タイムスケールによる影響を受けるかどうか</summary>
        protected bool followTimeScale = true;

        /// <summary>タイムスケールによる影響を受けるかどうか</summary>
        public bool FollowTimeScale { get { return followTimeScale; } set { followTimeScale = value; } }

        /// <summary>ループの状態</summary>
        protected LoopMode loopMode = LoopMode.NoLoop;

        /// <summary>ループの状態</summary>
        public LoopMode LoopMode { get { return loopMode; } set { loopMode = value; } }

        /// <summary>Tweenをループする際に何回ループを行うか</summary>
        protected int loopCount = 0;

        /// <summary>
        /// <para>Tweenをループする際に何回ループを行うか</para>
        /// <para>1回以上を指定することで指定回数ループしたらTweenを終了させます。</para>
        /// </summary>
        public int LoopCount { get { return loopCount; } set { loopCount = value; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        public TweenOrder(float duration)
        {
            this.duration = duration;
        }

        /// <summary>
        /// Tweenに各種値を設定します。
        /// </summary>
        /// <param name="tween">Tween</param>
        public void SetProperties(ITween tween)
        {
            tween.MotionType = motionType;
            tween.EasingType = easingType;
            tween.Duration = duration;
            tween.Delay = delay;
            tween.Interval = interval;
            tween.FollowTimeScale = followTimeScale;
            tween.LoopMode = loopMode;
            tween.LoopCount = loopCount;
        }

#endregion
    }
}
