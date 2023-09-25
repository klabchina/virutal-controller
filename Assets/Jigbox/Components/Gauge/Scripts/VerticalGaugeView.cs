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
    public class VerticalGaugeView : ColumnGaugeView
    {
#region public methods

        /// <summary>
        /// ビューの状態を初期化します。
        /// </summary>
        /// <param name="fillRect">フィリング対象のRectTransformの状態を計算するための矩形領域</param>
        /// <param name="fillTarget">フィリング対象となるオブジェクト</param>
        /// <param name="fillMethod">フィリング方法</param>
        /// <param name="fillOrigin">フィリングを行う際に基準となる点</param>
        public override void InitView(RectTransform fillRect, Object fillTarget, int fillMethod, int fillOrigin)
        {
            this.fillRect = fillRect;
            InitView(fillTarget, fillMethod, fillOrigin);

            Vector2 pivot = GetPivot(fillOrigin);

            RectTransformUtils.SetPivot(fillRect, pivot);
            RectTransformUtils.SetPivot(target, pivot);

            target.localPosition = Vector3.zero;

            CalculatePoint();
        }

        /// <summary>
        /// 表示を更新します。
        /// </summary>
        /// <param name="value">ゲージの値</param>
        public override void UpdateView(float value)
        {
            Vector2 size = fillRect.rect.size;
            size.y *= value;
            RectTransformUtils.SetSize(target, size, false);
            CalculateCurrentPoint(value);
        }

        /// <summary>
        /// ビューの座標情報を計算します。
        /// </summary>
        /// <param name="value">ゲージの値</param>
        public override void CalculatePoint(float value = 1.0f)
        {
            Bounds bounds = RectTransformUtils.GetBounds(fillRect);
            if (fillOrigin == (int) OriginVertical.Bottom)
            {
                distance.y = bounds.max.y - bounds.min.y;
            }
            else if (fillOrigin == (int) OriginVertical.Top)
            {
                distance.y = bounds.min.y - bounds.max.y;
            }

            CalculateCurrentPoint(value);
        }

        /// <summary>
        /// 設定するPivotを取得します。
        /// </summary>
        /// <param name="fillOrigin">フィリングを行う際に基準となる点</param>
        /// <returns></returns>
        public static Vector2 GetPivot(int fillOrigin)
        {
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            if (fillOrigin == (int) OriginVertical.Bottom)
            {
                pivot.y = 0.0f;
            }
            else if (fillOrigin == (int) OriginVertical.Top)
            {
                pivot.y = 1.0f;
            }
            return pivot;
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
