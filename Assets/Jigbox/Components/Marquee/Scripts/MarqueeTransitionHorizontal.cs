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

using Jigbox.Tween;

namespace Jigbox.Components
{
    public class MarqueeTransitionHorizontal : MarqueeTweenTransition
    {
#region properties

        /// <summary>
        /// Axis方向のViewportの長さを返します
        /// </summary>
        protected override float ViewportLength { get { return transitionProperty.ViewportSize.x; } }

#endregion

#region public methods

        /// <summary>
        /// Containerの位置をTransition開始位置に移動させます
        /// 注意：Layoutの計算が終わらないと正しい位置にいかない可能性があります
        /// </summary>
        public override void InitPosition()
        {
            var initPosition = transitionProperty.Container.localPosition;
            initPosition.x = CalculateInitPosition();
            transitionProperty.Container.localPosition = initPosition;
        }

#endregion

#region protected methods

        /// <summary>
        /// Containerのトランジション開始位置を計算します
        /// </summary>
        /// <returns></returns>
        protected override float CalculateBeginPositionAxis()
        {
            var startX = CalculateBeginPosition();
            return startX;
        }

        /// <summary>
        /// Containerのトランジション終了位置を計算します
        /// </summary>
        /// <returns></returns>
        protected override float CalculateEndPositionAxis()
        {
            var endX = CalculateEndPosition();
            return -endX;
        }

        /// <summary>
        /// Durationステート中にTweenの更新で呼ばれます
        /// </summary>
        /// <param name="t"></param>
        protected override void OnUpdateTweenDuration(ITween<float> t)
        {
            var newPosition = transitionProperty.Container.localPosition;
            // Horizontalのためxだけ変更する
            newPosition.x = t.Value;
            transitionProperty.Container.localPosition = newPosition;
        }
        
        /// <summary>
        /// 入場時の開始位置を取得
        /// </summary>
        /// <returns>開始位置</returns>
        protected override float CalculateEntranceBeginPositionAxis()
        {
            return ViewportLength;
        }
        
        /// <summary>
        /// 退場時の終了位置を取得
        /// </summary>
        /// <returns>終了位置</returns>
        protected override float CalculateExitEndPositionAxis()
        {
            return -transitionProperty.Length;
        }

#endregion
    }
}
