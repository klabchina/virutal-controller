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
    public static class Elastic
    {
        public const float defaultAmplitude = 1.0f;

        public const float defaultPeriod = 0.3f;

        public const float twoPi = Mathf.PI * 2;

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
            return EaseIn(t, defaultAmplitude, defaultPeriod);
        }

        public static float EaseIn(float t, float amplitude, float period)
        {
            if (t <= 0)
            {
                return 0;
            }
            if (t >= 1)
            {
                return 1;
            }

            if (period <= 0)
            {
                period = defaultPeriod;
            }

            float s;
            if (amplitude <= 1)
            {
                amplitude = defaultAmplitude;
                s = period / 4.0f;
            }
            else
            {
                s = period / twoPi * Mathf.Asin(1 / amplitude);
            }
            t -= 1;
            return -amplitude * Mathf.Pow(2, 10 * t) * Mathf.Sin((t - s) * twoPi / period);
        }

        public static float EaseOut(float t)
        {
            return EaseOut(t, defaultAmplitude, defaultPeriod);
        }

        public static float EaseOut(float t, float amplitude, float period)
        {
            if (t <= 0)
            {
                return 0;
            }
            if (t >= 1)
            {
                return 1;
            }

            if (period <= 0)
            {
                period = defaultPeriod;
            }

            float s;
            if (amplitude <= 1)
            {
                amplitude = defaultAmplitude;
                s = period / 4.0f;
            }
            else
            {
                s = period / twoPi * Mathf.Asin(1 / amplitude);
            }

            return 1 + amplitude * Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * twoPi / period);
        }

        public static float EaseInOut(float t)
        {
            return EaseInOut(t, defaultAmplitude, defaultPeriod);
        }

        public static float EaseInOut(float t, float amplitude, float period)
        {
            if (t <= 0)
            {
                return 0;
            }
            if (t >= 1)
            {
                return 1;
            }

            if (period <= 0)
            {
                period = defaultPeriod * 1.5f;
            }

            float s;

            if (amplitude <= 1)
            {
                amplitude = defaultAmplitude;
                s = period / 4;
            }
            else
            {
                s = period / twoPi * Mathf.Asin(1 / amplitude);
            }

            t = 2 * t - 1;
            if (t < 0)
            {
                return -0.5f * amplitude * Mathf.Pow(2, 10 * t) * Mathf.Sin((t - s) * twoPi / period);
            }
            return 1 + 0.5f * amplitude * Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * twoPi / period);
        }
    }
}

