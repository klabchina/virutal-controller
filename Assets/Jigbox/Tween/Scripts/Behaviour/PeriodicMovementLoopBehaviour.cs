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
    public abstract class PeriodicMovementLoopBehaviour<TValue, TPeriodicMovement>
        : PeriodicMovementBehaviour<TValue, TPeriodicMovement>,
          IPeriodicMovementLoopBehaviour<TValue, TPeriodicMovement>
        where TPeriodicMovement : IPeriodicMovement<TValue, TPeriodicMovement>
    {
#region properties

        /// <summary>動作の間隔</summary>
        public override float ProgressSpan { get { return periodicMovement.Duration + periodicMovement.Interval; } }

        /// <summary>初回の動作間隔</summary>
        public virtual float FirstProgressSpan { get { return periodicMovement.Duration + periodicMovement.Delay; } }

        /// <summary>
        /// <para>動作が完了するまでの時間の総量(秒)</para>
        /// <para>Tweenのループ回数を指定している状態でのみ正しい値を返します。</para>
        /// </summary>
        public virtual float TotalSpan { get { return FirstProgressSpan + (ProgressSpan * (periodicMovement.LoopCount - 1)); } }

        /// <summary>ループした回数</summary>
        public int LoopCount { get; protected set; }

        /// <summary>
        /// <para>最新のループ状態での補間が1回の更新で完了した回数</para>
        /// <para>差の大きなDeltaTimeを設定した場合、2回以上同時に補間が完了することがあるため、boolではなくintで保持</para>
        /// </summary>
        public int LastCompleteCount { get; protected set; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="periodicMovement">Tween</param>
        public PeriodicMovementLoopBehaviour(TPeriodicMovement periodicMovement) : base(periodicMovement)
        {
        }

        /// <summary>
        /// 初期化します。
        /// </summary>
        public override void Init()
        {
            base.Init();
            LoopCount = 0;
            LastCompleteCount = 0;
        }

        /// <summary>
        /// 経過時間を更新します。
        /// </summary>
        /// <param name="deltaTime">経過時間</param>
        /// <returns>経過時間の更新によって、動作が完了した場合<c>true</c>を返します。</returns>
        public override bool UpdateDeltaTime(float deltaTime)
        {
            bool isComplete = false;
            int lastLoopCount = LoopCount;

            if (deltaTime < 0)
            {
                deltaTime = 0.0f;
            }

            if (DeltaTime != deltaTime)
            {
                DeltaTime = deltaTime;

                // periodicMovement.LoopCountが0の場合はTotalSpan計算しても利用しない(できない)ため、一旦LoopCountのみでチェック
                if (periodicMovement.LoopCount > 0)
                {
                    float totalSpan = TotalSpan;
                    if (deltaTime >= totalSpan)
                    {
                        deltaTime = totalSpan;
                    }
                }

                // 1度目の更新が終わるまではIntervalの時間が介在しない
                if (DeltaTime < FirstProgressSpan)
                {
                    LoopCount = 0;
                }
                // 2度目以降は、Duration + Intervalの時間が経過する度に回数が増える
                else
                {
                    CalculateLoopCount();
                }

                float currentProgressTime = GetProgressTime();
                // 実際に値を計算するための時間変化した場合はキャッシュを破棄
                if (progressTime != currentProgressTime)
                {
                    progressTime = currentProgressTime;
                    IsCached = false;
                }
            }

            // 値の適用前よりループ回数が増加していれば完了が発生している
            isComplete = LoopCount > lastLoopCount;
            if (isComplete)
            {
                LastCompleteCount = LoopCount - lastLoopCount;
            }

            return isComplete;
        }

#endregion

#region protected methods

        /// <summary>
        /// 変化量を計算するための進捗度を表す時間を返します。
        /// </summary>
        /// <returns></returns>
        protected override float GetProgressTime()
        {
            if (LoopCount == 0)
            {
                if (DeltaTime <= periodicMovement.Delay)
                {
                    return 0.0f;
                }
                else
                {
                    return DeltaTime - periodicMovement.Delay;
                }
            }

            float progressDelta = (DeltaTime - FirstProgressSpan) % ProgressSpan;

            // Loopの終了時に小数点誤差でProgressDeltaが剰余できていないケースがあるため、ProgressSpanと近似値の場合終了したとみなす
            if (Mathf.Approximately(progressDelta, ProgressSpan))
            {
                return GetLoopedProgressTime(0.0f);
            }

            return GetLoopedProgressTime(progressDelta);
        }

        /// <summary>
        /// ループし始めてからの進捗度を表す時間を返します。
        /// </summary>
        /// <param name="progressDelta">0～ProgressSpanの間に丸められた経過時間</param>
        /// <returns></returns>
        protected virtual float GetLoopedProgressTime(float progressDelta)
        {
            if (progressDelta <= periodicMovement.Interval)
            {
                return periodicMovement.Duration;
            }
            else
            {
                return progressDelta - periodicMovement.Interval;
            }
        }

        /// <summary>
        /// 1回以上ループしている状態の経過時間に対するループした回数を計算します。
        /// </summary>
        protected void CalculateLoopCount()
        {
            // Tween側にループ回数が指定されている場合のみ、ループ回数による終了チェックを行う
            if (periodicMovement.LoopCount > 0)
            {
                float totalSpan = TotalSpan;
                // floatの計算誤差でLoopCountがズレるのを防ぐため、
                // 想定される時間を超える場合は直接設定
                if (DeltaTime >= totalSpan)
                {
                    DeltaTime = totalSpan;
                    LoopCount = periodicMovement.LoopCount;
                    return;
                }
            }

            LoopCount = 1 + Mathf.FloorToInt((DeltaTime - FirstProgressSpan) / ProgressSpan);
        }

#endregion
    }
}
