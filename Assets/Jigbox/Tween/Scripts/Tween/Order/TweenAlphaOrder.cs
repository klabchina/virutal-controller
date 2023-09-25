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
using UnityEngine.UI;
using System;

namespace Jigbox.Tween
{
    public class TweenAlphaOrder : TweenOrder
    {
#region properties

        /// <summary>アルファ値</summary>
        protected float? alpha;

        /// <summary>アルファ値(alpha)</summary>
        public float? A { get { return alpha; } set { alpha = value; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        public TweenAlphaOrder(float duration) : base(duration)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">アルファ値</param>
        public TweenAlphaOrder(float duration, float alpha) : base(duration)
        {
            this.alpha = alpha;
        }

        /// <summary>
        /// ToメソッドでのTween.Finalに設定する値を取得します。
        /// </summary>
        /// <returns></returns>
        public float GetToValue()
        {
            return alpha.HasValue ? alpha.Value : 0.0f;
        }

        /// <summary>
        /// FromメソッドでのTween.Beginに設定する値を取得します。
        /// </summary>
        /// <returns></returns>
        public float GetFromValue()
        {
            return alpha.HasValue ? alpha.Value : 0.0f;
        }

        /// <summary>
        /// ByメソッドでのTween.Finalに設定する値を取得します。
        /// </summary>
        /// <param name="baseColor">元となる色</param>
        /// <returns></returns>
        public float GetByValue(float baseAlpha)
        {
            return baseAlpha + (alpha.HasValue ? alpha.Value : 0.0f);
        }

        /// <summary>
        /// Tweenの更新時に呼び出されるコールバックを生成します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <returns></returns>
        public Action<ITween<float>> CreateOnUpdate(Graphic graphic)
        {
            return new Action<ITween<float>>(tween =>
            {
                Color color = graphic.color;
                color.a = tween.Value;
                graphic.color = color;
            });
        }

        /// <summary>
        /// Tweenの更新時に呼び出されるコールバックを生成します。
        /// </summary>
        /// <param name="canvasGroup">CanvasGroup</param>
        /// <returns></returns>
        public Action<ITween<float>> CreateOnUpdate(CanvasGroup canvasGroup)
        {
            return new Action<ITween<float>>(tween => canvasGroup.alpha = tween.Value);
        }

#endregion
    }
}
