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
using System;

namespace Jigbox.Tween.Easing
{
    public static class Back
    {
        public const float defaultOvershoot = 1.70158f;

        static Func<float, float> cachedEaseIn = EaseIn;
        static Func<float, float> cachedEaseOut = EaseOut;
        static Func<float, float> cachedEaseInOut = EaseInOut;

        public static Func<float, float> GetEasing(EasingType easeMode)
        {
            switch (easeMode)
            {
                case EasingType.EaseIn:
                    return cachedEaseIn;
                case EasingType.EaseOut:
                    return cachedEaseOut;
                case EasingType.EaseInOut:
                default:
                    return cachedEaseInOut;
            }
        }

        public static float EaseIn(float t)
        {
            return EaseIn(t, defaultOvershoot);
        }

        public static float EaseIn(float t, float overshoot)
        {
            return Mathf.Pow(t, 2) * ((overshoot + 1) * t - overshoot);
        }

        public static float EaseOut(float t)
        {
            return EaseOut(t, defaultOvershoot);
        }

        public static float EaseOut(float t, float overshoot)
        {
            t -= 1;
            return Mathf.Pow(t, 2) * ((overshoot + 1) * t + overshoot) + 1;
        }

        public static float EaseInOut(float t)
        {
            return EaseInOut(t, defaultOvershoot);
        }

        public static float EaseInOut(float t, float overshoot)
        {
            t *= 2;
            overshoot *= 1.525f;
            if (t < 1)
            {
                return 0.5f * Mathf.Pow(t, 2) * ((overshoot + 1) * t - overshoot);
            }
            t -= 2;
            return 0.5f * Mathf.Pow(t, 2) * ((overshoot + 1) * t + overshoot) + 1;
        }
    }
}

