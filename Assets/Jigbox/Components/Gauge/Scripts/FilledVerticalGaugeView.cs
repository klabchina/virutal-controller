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
using Jigbox.UIControl;

using OriginVertical = UnityEngine.UI.Image.OriginVertical;

namespace Jigbox.Components
{
    public class FilledVerticalGaugeView : FilledColumnGaugeView
    {
#region public methods

        /// <summary>
        /// ビューの状態を初期化します。
        /// </summary>
        /// <param name="fillTarget">フィリング対象となるオブジェクト</param>
        /// <param name="fillMethod">フィリング方法</param>
        /// <param name="fillOrigin">フィリングを行う際に基準となる点</param>
        public override void InitView(Object fillTarget, int fillMethod, int fillOrigin)
        {
            base.InitView(fillTarget, fillMethod, fillOrigin);

            Vector2 pivot = VerticalGaugeView.GetPivot(fillOrigin);
            RectTransformUtils.SetPivot(targetTransform, pivot);

            CalculatePoint();
        }

        /// <summary>
        /// ビューの座標情報を計算します。
        /// </summary>
        /// <param name="value">ゲージの値</param>
        public override void CalculatePoint(float value = 1.0f)
        {
            Bounds bounds = RectTransformUtils.GetBounds(targetTransform);
            if (target.fillOrigin == (int) OriginVertical.Bottom)
            {
                distance.y = bounds.max.y - bounds.min.y;
            }
            else if (target.fillOrigin == (int) OriginVertical.Top)
            {
                distance.y = bounds.min.y - bounds.max.y;
            }

            CalculateCurrentPoint(value);
        }

#endregion

#region protected methods

        /// <summary>
        /// ゲージの値から現在のゲージ量における座標位置を求めます。
        /// </summary>
        /// <param name="value">ゲージの値</param>
        protected override void CalculateCurrentPoint(float value)
        {
            currentPoint = OriginPoint;
            currentPoint.y += distance.y * value;
        }

#endregion
    }
}
