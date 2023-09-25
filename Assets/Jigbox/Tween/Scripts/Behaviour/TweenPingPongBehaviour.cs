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
using Jigbox.Tween.Easing;

namespace Jigbox.Tween
{
    public class TweenPingPongBehaviour<T> : TweenYoyoBehaviour<T>
    {
#region properties

        /// <summary>反転させたモーション</summary>
        protected AnimationCurve mirroredMotion = null;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="periodicMovement">Tween</param>
        public TweenPingPongBehaviour(TweenBase<T> periodicMovement) : base(periodicMovement)
        {
            if (periodicMovement.MotionType == MotionType.Custom && periodicMovement.CustomMotion != null)
            {
                mirroredMotion = MirrorAnimationCurve(periodicMovement.CustomMotion);
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// 進捗度を返します。
        /// </summary>
        /// <param name="progress">0～1までの進捗度</param>
        /// <returns></returns>
        public override float Progress(float progress)
        {
            // ループ回数が0及び偶数の場合は往路、奇数の場合は復路
            // 復路の場合は、往路とは補間の状態が逆転する
            bool isReturn = LoopCount % 2 == 1;

            if (periodicMovement.MotionType == MotionType.Custom && periodicMovement.CustomMotion != null)
            {
                if (isReturn)
                {
                    return mirroredMotion.Evaluate(progress);
                }
                else
                {
                    return periodicMovement.CustomMotion.Evaluate(progress);
                }
            }

            EasingType easingType = periodicMovement.EasingType;

            if (isReturn)
            {
                switch (periodicMovement.EasingType)
                {
                    case EasingType.EaseIn:
                        easingType = EasingType.EaseOut;
                        break;
                    case EasingType.EaseOut:
                        easingType = EasingType.EaseIn;
                        break;
                    default:
                        break;
                }
            }

            return EasingCreator.GetPresetNormalized(periodicMovement.MotionType, easingType)(progress);
        }

        /// <summary>
        /// アニメーションカーブの補間状態を逆転させます。
        /// </summary>
        /// <param name="animationCurve">アニメーションカーブ</param>
        /// <returns></returns>
        protected AnimationCurve MirrorAnimationCurve(AnimationCurve animationCurve)
        {
            var allKeyframes = animationCurve.keys;

            for (int i = 1; i < allKeyframes.Length - 1; ++i)
            {
                allKeyframes[i].value = 1.0f - allKeyframes[i].value;
                allKeyframes[i].time = 1.0f - allKeyframes[i].time;
            }

            return new AnimationCurve(allKeyframes);
        }

#endregion
    }
}
