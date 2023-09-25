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

namespace Jigbox.Components
{
    /// <summary>
    /// バルーンの基準座標を計算するユーティリティクラス
    /// </summary>
    public static class BalloonBasePositionUtil
    {
#region public methods

        /// <summary>
        /// バルーンの基準座標を基準となるRectTransformとBalloonLayoutを元に計算する
        /// </summary>
        /// <param name="baseRectTransform">基準となるRectTransform</param>
        /// <param name="balloonLayout">バルーンのレイアウト</param>
        /// <returns>基準座標</returns>
        public static Vector2 GetBasePositionByLayout(RectTransform baseRectTransform, BalloonLayout balloonLayout)
        {
            var worldPosition = (Vector2)baseRectTransform.position;
            var lossyScale = baseRectTransform.lossyScale;
            var pivot = baseRectTransform.pivot;
            var lossyScaleSize = Vector2.Scale(baseRectTransform.rect.size, lossyScale);
            var offset = Vector2.zero;

            switch (balloonLayout)
            {
                case BalloonLayout.Top:
                    offset = GetOffsetTopLayout(pivot, lossyScaleSize);
                    break;
                case BalloonLayout.Bottom:
                    offset = GetOffsetBottomLayout(pivot, lossyScaleSize);
                    break;
                case BalloonLayout.Right:
                    offset = GetOffsetRightLayout(pivot, lossyScaleSize);
                    break;
                case BalloonLayout.Left:
                    offset = GetOffsetLeftLayout(pivot, lossyScaleSize);
                    break;
                case BalloonLayout.Overlay:
                    offset = GetOffsetOverlayLayout(pivot, lossyScaleSize);
                    break;
            }

            return worldPosition + offset;
        }

        /// <summary>
        /// 上方向レイアウトの計算を行う
        /// </summary>
        /// <param name="pivot">基準のRectTransformのPivot</param>
        /// <param name="lossyScaleSize">基準のRectTransformのsize</param>
        /// <returns>オフセット座標</returns>
        public static Vector2 GetOffsetTopLayout(Vector2 pivot, Vector2 lossyScaleSize)
        {
            var offset = Vector2.zero;

            offset.x += lossyScaleSize.x * (0.5f - pivot.x);
            offset.y += lossyScaleSize.y * (1.0f - pivot.y);

            return offset;
        }

        /// <summary>
        /// 下方向レイアウトの計算を行う
        /// </summary>
        /// <param name="pivot">基準のRectTransformのPivot</param>
        /// <param name="lossyScaleSize">基準のRectTransformのsize</param>
        /// <returns>オフセット座標</returns>
        public static Vector2 GetOffsetBottomLayout(Vector2 pivot, Vector2 lossyScaleSize)
        {
            var offset = Vector2.zero;

            offset.x = lossyScaleSize.x * (0.5f - pivot.x);
            offset.y = -(lossyScaleSize.y * pivot.y);

            return offset;
        }

        /// <summary>
        /// 左方向レイアウトの計算を行う
        /// </summary>
        /// <param name="pivot">基準のRectTransformのPivot</param>
        /// <param name="lossyScaleSize">基準のRectTransformのsize</param>
        /// <returns>オフセット座標</returns>
        public static Vector2 GetOffsetLeftLayout(Vector2 pivot, Vector2 lossyScaleSize)
        {
            var offset = Vector2.zero;

            offset.x = -(lossyScaleSize.x * pivot.x);
            offset.y = lossyScaleSize.y * (0.5f - pivot.y);

            return offset;
        }

        /// <summary>
        /// 右方向レイアウトの計算を行う
        /// </summary>
        /// <param name="pivot">基準のRectTransformのPivot</param>
        /// <param name="lossyScaleSize">基準のRectTransformのsize</param>
        /// <returns>オフセット座標</returns>
        public static Vector2 GetOffsetRightLayout(Vector2 pivot, Vector2 lossyScaleSize)
        {
            var offset = Vector2.zero;

            offset.x = lossyScaleSize.x * (1.0f - pivot.x);
            offset.y = lossyScaleSize.y * (0.5f - pivot.y);

            return offset;
        }

        /// <summary>
        /// 重なるレイアウトの計算を行う
        /// </summary>
        /// <param name="pivot">基準のRectTransformのPivot</param>
        /// <param name="lossyScaleSize">基準のRectTransformのsize</param>
        /// <returns>オフセット座標</returns>
        public static Vector2 GetOffsetOverlayLayout(Vector2 pivot, Vector2 lossyScaleSize)
        {
            var offset = Vector2.zero;

            offset.x = lossyScaleSize.x * (0.5f - pivot.x);
            offset.y = lossyScaleSize.y * (0.5f - pivot.y);

            return offset;
        }

#endregion
    }
}
