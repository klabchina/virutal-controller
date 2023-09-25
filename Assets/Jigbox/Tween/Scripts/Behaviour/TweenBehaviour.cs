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
    public class TweenBehaviour<T> : PeriodicMovementBehaviour<T, TweenBase<T>>
    {
#region properties

        /// <summary><see cref="PeriodicMovementBehaviour{TValue,TPeriodicMovement}.Value" /> の初期値</summary>
        protected override T InitValue { get { return periodicMovement.Begin; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tween">Tween</param>
        public TweenBehaviour(TweenBase<T> tween) : base(tween) {}

        /// <summary>
        /// 進捗度を返します。
        /// </summary>
        /// <param name="progress">0～1の進捗度</param>
        /// <returns>0〜1の進捗度</returns>
        public override float Progress(float progress)
        {
            if (periodicMovement.MotionType == MotionType.Custom && periodicMovement.CustomMotion != null)
            {
                return periodicMovement.CustomMotion.Evaluate(progress);
            }
            return Easing.EasingCreator.GetPresetNormalized(periodicMovement.MotionType, periodicMovement.EasingType)(progress);
        }

#endregion
    }
}
