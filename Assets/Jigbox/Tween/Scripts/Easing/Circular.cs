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
    /// 円弧 p(t) = √(1 - t ^ 2) をベースとするイージング関数のヘルパークラスです
    /// </summary>
    public static class Circular
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
            return 1 - Mathf.Sqrt(1 - t * t);
        }

        public static float EaseOut(float t)
        {
            t -= 1;
            return Mathf.Sqrt(1 - t * t);
        }

        public static float EaseInOut(float t)
        {
            t *= 2;
            if (t < 1)
            {
                return 0.5f * (1 - Mathf.Sqrt(1 - t * t));
            }
            t -= 2;
            return 0.5f * (1 + Mathf.Sqrt(1 - t * t));
        }
    }
}
