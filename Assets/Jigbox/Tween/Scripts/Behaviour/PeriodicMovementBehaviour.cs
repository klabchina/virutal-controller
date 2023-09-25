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

namespace Jigbox.Tween
{
    public abstract class PeriodicMovementBehaviour<TValue, TPeriodicMovement>
        : IPeriodicMovementBehaviour<TValue, TPeriodicMovement>
        where TPeriodicMovement : IPeriodicMovement<TValue, TPeriodicMovement>
    {
#region properties

        /// <summary>Tweenの参照</summary>
        protected TPeriodicMovement periodicMovement;

        /// <summary>動作の間隔</summary>
        public virtual float ProgressSpan { get { return periodicMovement.Duration + periodicMovement.Delay; } }

        /// <summary>経過時間</summary>
        public float DeltaTime { get; protected set; }

        /// <summary>変化量を計算するための進捗度を表す時間</summary>
        protected float progressTime;

        /// <summary>値</summary>
        protected TValue value;

        /// <summary>値</summary>
        public virtual TValue Value
        {
            get
            {
                if (!IsCached)
                {
                    value = periodicMovement.ValueAt(progressTime);
                    IsCached = true;
                }
                return value;
            }
        }

        /// <summary>値がキャッシュされているかどうか</summary>
        public bool IsCached { get; protected set; }

        /// <summary><see cref="Value" /> の初期値</summary>
        protected abstract TValue InitValue { get; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="periodicMovement">Tween</param>
        public PeriodicMovementBehaviour(TPeriodicMovement periodicMovement)
        {
            this.periodicMovement = periodicMovement;
        }

        /// <summary>
        /// 初期化します。
        /// </summary>
        public virtual void Init()
        {
            DeltaTime = 0.0f;
            progressTime = 0.0f;
            value = InitValue;
            IsCached = true;
        }

        /// <summary>
        /// 経過時間を更新します。
        /// </summary>
        /// <param name="deltaTime">経過時間</param>
        /// <returns>経過時間の更新によって、動作が完了した場合<c>true</c>を返します。</returns>
        public virtual bool UpdateDeltaTime(float deltaTime)
        {
            bool isComplete = false;

            if (deltaTime < 0)
            {
                deltaTime = 0.0f;
            }
            if (deltaTime > ProgressSpan)
            {
                deltaTime = ProgressSpan;
                isComplete = true;
            }
            if (DeltaTime != deltaTime)
            {
                DeltaTime = deltaTime;

                float currentProgressTime = GetProgressTime();
                // 実際に値を計算するための時間変化した場合はキャッシュを破棄
                if (progressTime != currentProgressTime)
                {
                    progressTime = currentProgressTime;
                    IsCached = false;
                }
            }

            return isComplete;
        }

        /// <summary>
        /// 進捗度を返します。
        /// </summary>
        /// <param name="progress">0～1の進捗度</param>
        /// <returns>0～1の進捗度</returns>
        public abstract float Progress(float progress);

#endregion

#region protected methods

        /// <summary>
        /// 変化量を計算するための進捗度を表す時間を返します。
        /// </summary>
        /// <returns></returns>
        protected virtual float GetProgressTime()
        {
            if (DeltaTime <= periodicMovement.Delay)
            {
                return 0.0f;
            }
            else if (DeltaTime >= ProgressSpan)
            {
                return periodicMovement.Duration;
            }
            else
            {
                return DeltaTime - periodicMovement.Delay;
            }
        }

#endregion
    }
}
