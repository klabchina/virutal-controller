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

using Jigbox.Tween;

namespace Jigbox.Shake
{
    public class SpringShakeLoopBehaviour<TValue, TSpringShake>
        : PeriodicMovementLoopBehaviour<TValue, TSpringShake>
        where TSpringShake : SpringShake<TValue, TSpringShake>
    {
        /// <summary>コンストラクター</summary>
        /// <param name="periodicMovement">対象の <c>SpringShake</c></param>
        public SpringShakeLoopBehaviour(TSpringShake periodicMovement) : base(periodicMovement) {}

        /// <summary><see cref="PeriodicMovementBehaviour{TValue,TPeriodicMovement}.Value" /> の初期値</summary>
        protected override TValue InitValue
        {
            get { return periodicMovement.Origin; }
        }

        /// <summary>
        /// 進捗度を返します。
        /// </summary>
        /// <param name="progress">0～1の進捗度</param>
        /// <returns>0～1の進捗度</returns>
        public override float Progress(float progress)
        {
            return progress;
        }
    }
}
