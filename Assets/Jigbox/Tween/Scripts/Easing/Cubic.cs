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
    /// <summary>
    /// 三次関数 p(t) = t ^ 3 をベースとするイージング関数のヘルパークラスです
    /// </summary>
    public static class Cubic
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
            return Mathf.Pow(t, 3);
        }

        public static float EaseOut(float t)
        {
            return 1 + Mathf.Pow(t - 1, 3);
        }

        public static float EaseInOut(float t)
        {
            if (t < 0.5f)
            {
                return 0.5f * Mathf.Pow(2 * t, 3);
            }
            return 1 + 0.5f * Mathf.Pow(2 * t - 2, 3);
        }
    }
}
