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
    /// <summary>
    /// アニメーションの緩急の種類を示します
    /// </summary>
    public enum MotionType
    {
        /// <summary>
        /// 単純な線形補間を行います
        /// </summary>
        Linear,

        /// <summary>
        /// 二次関数 p(t) = t ^ 2 をベースに緩やかな緩急をつけます
        /// </summary>
        Quadratic,

        /// <summary>
        /// 指数関数 p(t) = 2 ^ (10 * t) をベースに極端な緩急をつけます
        /// </summary>
        Exponential,

        /// <summary>
        /// 三次関数 p(t) = t ^ 3 をベースに抑揚のある緩急をつけます
        /// </summary>
        Cubic,

        /// <summary>
        /// 四次関数 p(t) = t ^ 4 をベースに強めの緩急をつけます
        /// </summary>
        Quartic,

        /// <summary>
        /// 五次関数 p(t) = t ^ 5 をベースに強い緩急をつけます
        /// </summary>
        Quintic,

        /// <summary>
        /// 円弧 p(t) = √(1 - t ^ 2) をベースに極端な緩急をつけます
        /// </summary>
        Circular,

        /// <summary>
        /// 三角関数 p(t) = sin(t * π / 2) をベースに緩やかな緩急をつけます
        /// </summary>
        Sine,

        /// <summary>
        /// バネやゴムのような伸び縮みをする動きをつけます
        /// </summary>
        Elastic,

        /// <summary>
        /// 物体が衝突し跳ね返りながら静止するような動きをつけます
        /// </summary>
        Bounce,

        /// <summary>
        /// 勢いをつける為に後ずさりしたり、勢いを余らせて飛び外すような動きをつけます
        /// </summary>
        Back,

        /// <summary>
        /// ユーザー定義のイージング関数を元に緩急をつけます
        /// </summary>
        Custom
    }
}
