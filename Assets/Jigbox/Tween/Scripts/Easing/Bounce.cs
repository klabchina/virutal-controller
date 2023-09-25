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
    public static class Bounce
    {
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
            return 1 - EaseOut(1 - t);
        }

        public static float EaseOut(float t)
        {
            const float b = 7.5625f;
            const float k = 2.75f;
            float a, h;

            if (t < 1 / k)
            {
                return b * Mathf.Pow(t, 2);
            }
            else if (t < 2 / k)
            {
                a = 1.5f;
                h = 0.75f;
            }
            else if (t < 2.5f / k)
            {
                a = 2.25f;
                h = 0.9375f;
            }
            else
            {
                a = 2.625f;
                h = 0.984375f;
            }
            return b * Mathf.Pow(t - a / k, 2) + h;
        }

        public static float EaseInOut(float t)
        {
            return 0.5f * (t < 0.5f ? EaseIn(2 * t) : EaseOut(2 * t - 1) + 1);
        }
    }
}
