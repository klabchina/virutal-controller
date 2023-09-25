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

namespace Jigbox.Tween.Easing
{
    /// <summary>
    /// 正規化されたイージング関数を提供するユーティリティクラスです
    /// </summary>
    public static class EasingCreator
    {
        static Func<float, float> cachedIdentity = Identity;

        internal static float Identity(float x)
        {
            return x;
        }

        /// <summary>
        /// 単純な線形補間
        /// </summary>
        /// <param name="time">Time.</param>
        /// <param name="begin">Begin.</param>
        /// <param name="final">Final.</param>
        /// <param name="duration">Duration.</param>
        public static float Linear(float time, float begin, float final, float duration)
        {
            return (final - begin) * time / duration + begin;
        }


        /// <summary>
        /// プリセットの正規化されたイージング関数を返します
        /// </summary>
        /// <returns>The normalized easing.</returns>
        /// <param name="motionType">Motion type.</param>
        /// <param name="easingType">Easing type.</param>
        public static Func<float, float> GetPresetNormalized(MotionType motionType, EasingType easingType)
        {
            switch (motionType)
            {
                case MotionType.Linear:
                    return cachedIdentity;

                case MotionType.Quadratic:
                    return Quadratic.GetEasing(easingType);

                case MotionType.Exponential:
                    return Exponential.GetEasing(easingType);

                case MotionType.Cubic:
                    return Cubic.GetEasing(easingType);

                case MotionType.Quartic:
                    return Quartic.GetEasing(easingType);

                case MotionType.Quintic:
                    return Quintic.GetEasing(easingType);

                case MotionType.Circular:
                    return Circular.GetEasing(easingType);

                case MotionType.Sine:
                    return Sine.GetEasing(easingType);

                case MotionType.Elastic:
                    return Elastic.GetEasing(easingType);

                case MotionType.Bounce:
                    return Bounce.GetEasing(easingType);

                case MotionType.Back:
                    return Back.GetEasing(easingType);

                default: // Quadratic EaseInOut
                    return Quadratic.GetEasing(EasingType.EaseInOut);
            }
        }

        /// <summary>
        /// プリセットのイージング関数を返します
        /// </summary>
        /// <returns>The easing.</returns>
        /// <param name="motionType">Motion type.</param>
        /// <param name="easingType">Easing type.</param>
        public static Interpolate<float> GetPreset(MotionType motionType, EasingType easingType)
        {
            return CreateCustomEasing(GetPresetNormalized(motionType, easingType));
        }

        /// <summary>
        /// 正規化したイージング関数を元にユーザー定義のイージング関数を生成します
        /// </summary>
        /// <returns>The custom easing.</returns>
        /// <param name="easing">正規化されたイージング関数</param>
        public static Interpolate<float> CreateCustomEasing(Func<float, float> easing)
        {
            return (start, final, t) => (final - start) * easing(t) + start;
        }
    }
}
