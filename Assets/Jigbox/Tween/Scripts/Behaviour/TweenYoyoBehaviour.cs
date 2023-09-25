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
    public class TweenYoyoBehaviour<T> : TweenLoopBehaviour<T>
    {
#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="periodicMovement">Tween</param>
        public TweenYoyoBehaviour(TweenBase<T> periodicMovement) : base(periodicMovement)
        {
        }

#endregion

#region protected methods

        /// <summary>
        /// ループし始めてからの進捗度を表す時間を返します。
        /// </summary>
        /// <param name="progressDelta">0～ProgressSpanの間に丸められた経過時間</param>
        /// <returns></returns>
        protected override float GetLoopedProgressTime(float progressDelta)
        {
            float progressTime = 0.0f;

            if (progressDelta <= periodicMovement.Interval)
            {
                progressTime = 0.0f;
            }
            else
            {
                progressTime = progressDelta - periodicMovement.Interval;
            }

            // ループ回数が0及び偶数の場合は往路、奇数の場合は復路
            // 復路の場合は、往路とは進捗度が逆転する
            return LoopCount % 2 == 0 ? progressTime : periodicMovement.Duration - progressTime;
        }

#endregion
    }
}
