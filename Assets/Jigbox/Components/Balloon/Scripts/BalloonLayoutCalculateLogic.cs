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
    /// レイアウトに応じた位置計算を行うユーティリティクラス
    /// </summary>
    public static class BalloonCalculateLayoutLogic
    {
#region private methods

        /// <summary>
        /// 与えられた座標とサイズから矩形の左上に当たる座標を返す
        /// </summary>
        /// <param name="position">座標</param>
        /// <param name="size">サイズ</param>
        /// <param name="pivot">pivot</param>
        /// <returns>矩形の左上の座標</returns>
        static Vector2 CalculateLeftTopPositionWithPivot(
            Vector2 position,
            Vector2 size,
            Vector2 lossyScale,
            Vector2 pivot)
        {
            var leftX = position.x - size.x * lossyScale.x * pivot.x;
            var topY = position.y + size.y * lossyScale.y * (1.0f - pivot.y);

            return new Vector2(leftX, topY);
        }

        /// <summary>
        /// 与えられた座標とサイズから矩形の右下に当たる座標を返す
        /// </summary>
        /// <param name="position">座標</param>
        /// <param name="size">サイズ</param>
        /// <param name="pivot">pivot</param>
        /// <returns>矩形の右下の座標</returns>
        static Vector2 CalculateRightBottomPositionWithPivot(
            Vector2 position,
            Vector2 size,
            Vector2 lossyScale,
            Vector2 pivot)
        {
            var rightX = position.x + size.x * lossyScale.x * (1.0f - pivot.x);
            var bottomY = position.y - size.y * lossyScale.y * pivot.y;

            return new Vector2(rightX, bottomY);
        }

#endregion

#region public methods

        /// <summary>
        /// 自動レイアウト矩形のサイズがバルーンのサイズより大きいかどうか
        /// </summary>
        /// <param name="necessarySize">バルーンのサイズ</param>
        /// <param name="autoLayoutAreaSize">自動レイアウト矩形のサイズ</param>
        /// <returns></returns>
        public static bool IsValidAutoLayoutAreaSize(Vector2 necessarySize, Vector2 autoLayoutAreaSize)
        {
            return (necessarySize.x <= autoLayoutAreaSize.x && necessarySize.y <= autoLayoutAreaSize.y);
        }

        /// <summary>
        /// 自動レイアウトでのオフセット値を計算する
        /// </summary>
        /// <param name="currentPosition">現在の計算されたワールド座標</param>
        /// <param name="property">計算に使用するプロパティ</param>
        /// <returns>オフセット値</returns>
        public static Vector2 CalculateAutoLayoutOffset(Vector2 currentPosition, IBalloonLayoutProperty property)
        {
            var contentLeftTop = CalculateLeftTopPositionWithPivot(
                currentPosition,
                property.ContentSize,
                property.ContentLossyScale,
                property.ContentPivot);

            var contentRightBottom = CalculateRightBottomPositionWithPivot(
                currentPosition,
                property.ContentSize,
                property.ContentLossyScale,
                property.ContentPivot);

            var layoutAreaLeftTop = CalculateLeftTopPositionWithPivot(
                property.AutoLayoutAreaPosition,
                property.AutoLayoutAreaSize,
                property.AutoLayoutAreaLossyScale,
                property.AutoLayoutAreaPivot);

            var layoutAreaRightBottom = CalculateRightBottomPositionWithPivot(
                property.AutoLayoutAreaPosition,
                property.AutoLayoutAreaSize,
                property.AutoLayoutAreaLossyScale,
                property.AutoLayoutAreaPivot);

            Vector2 offset = Vector2.zero;

            if (contentLeftTop.x < layoutAreaLeftTop.x)
            {
                offset.x = layoutAreaLeftTop.x - contentLeftTop.x;
            }

            if (contentRightBottom.x > layoutAreaRightBottom.x)
            {
                offset.x = layoutAreaRightBottom.x - contentRightBottom.x;
            }

            if (contentLeftTop.y > layoutAreaLeftTop.y)
            {
                offset.y = layoutAreaLeftTop.y - contentLeftTop.y;
            }

            if (contentRightBottom.y < layoutAreaRightBottom.y)
            {
                offset.y = layoutAreaRightBottom.y - contentRightBottom.y;
            }

            return offset;
        }

        /// <summary>
        /// バルーンのレイアウト方向に応じた計算を行う
        /// </summary>
        /// <param name="property">計算に使用するプロパティ</param>
        public static Vector2 CalculateLayout(IBalloonLayoutProperty property)
        {
            Vector2 offset = Vector2.zero;

            switch (property.BalloonLayout)
            {
                case BalloonLayout.Top:
                    offset = CalculateLayoutTop(property);
                    break;
                case BalloonLayout.Bottom:
                    offset = CalculateLayoutBottom(property);
                    break;
                case BalloonLayout.Right:
                    offset = CalculateLayoutRight(property);
                    break;
                case BalloonLayout.Left:
                    offset = CalculateLayoutLeft(property);
                    break;
                case BalloonLayout.Overlay:
                    offset = CalculateLayoutOverlay(property);
                    break;
            }

            return property.BasePosition + offset;
        }

        /// <summary>
        /// 上方向レイアウトの計算
        /// </summary>
        /// <param name="property">計算に使用するプロパティ</param>
        /// <returns>計算結果のオフセット座標</returns>
        public static Vector2 CalculateLayoutTop(IBalloonLayoutProperty property)
        {
            // Canvasの実スケールに合わせたサイズに補正
            var offset = Vector2.Scale(property.ContentSize, property.ContentLossyScale);

            // PivotとLayoutPositionRateの位置を加味した位置に補正
            offset.x *= property.ContentPivot.x - 0.5f + property.BalloonLayoutPositionRate;

            // Pivotの値を加味した位置に補正
            offset.y *= property.ContentPivot.y;

            // スペーシングの値を適用
            offset.y += property.Spacing * property.ContentLossyScale.y;

            return offset;
        }

        /// <summary>
        /// 下方向レイアウトの計算
        /// </summary>
        /// <param name="property">計算に使用するプロパティ</param>
        /// <returns>計算結果のオフセット座標</returns>
        public static Vector2 CalculateLayoutBottom(IBalloonLayoutProperty property)
        {
            // Canvasの実スケールに合わせたサイズに補正
            var offset = Vector2.Scale(property.ContentSize, property.ContentLossyScale);

            // PivotとLayoutPositionRateの位置を加味した位置に補正
            offset.x *= property.ContentPivot.x - 0.5f + property.BalloonLayoutPositionRate;

            // Pivotの値を加味した位置に補正
            offset.y *= 1.0f - property.ContentPivot.y;

            // スペーシングの値を適用
            offset.y += property.Spacing * property.ContentLossyScale.y;

            // 加算で補正するため符号反転する
            offset.y = -offset.y;

            return offset;
        }

        /// <summary>
        /// 左方向レイアウトの計算
        /// </summary>
        /// <param name="property">計算に使用するプロパティ</param>
        /// <returns>計算結果のオフセット座標</returns>
        public static Vector2 CalculateLayoutLeft(IBalloonLayoutProperty property)
        {
            // Canvasの実スケールに合わせたサイズに補正
            var offset = Vector2.Scale(property.ContentSize, property.ContentLossyScale);

            // Pivotの値を加味した位置に補正
            offset.x *= 1.0f - property.ContentPivot.x;

            // スペーシングの値を適用
            offset.x += property.Spacing * property.ContentLossyScale.x;

            // 加算で補正するため符号反転する
            offset.x = -offset.x;

            // PivotとPositionRateを加味した位置に補正
            offset.y *= property.ContentPivot.y - 0.5f + property.BalloonLayoutPositionRate;

            return offset;
        }

        /// <summary>
        /// 右方向レイアウトの計算
        /// </summary>
        /// <param name="property">計算に使用するプロパティ</param>
        /// <returns>計算結果のオフセット座標</returns>
        public static Vector2 CalculateLayoutRight(IBalloonLayoutProperty property)
        {
            // Canvasの実スケールに合わせたサイズに補正
            var offset = Vector2.Scale(property.ContentSize, property.ContentLossyScale);

            // Pivotの値を加味した位置に補正
            offset.x *= property.ContentPivot.x;

            // スペーシングの値を適用
            offset.x += property.Spacing * property.ContentLossyScale.x;

            // PivotとPositionRateを加味した位置に補正
            offset.y *= property.ContentPivot.y - 0.5f + property.BalloonLayoutPositionRate;

            return offset;
        }

        /// <summary>
        /// 重なるレイアウトの計算
        /// </summary>
        /// <param name="property">計算に使用するプロパティ</param>
        /// <returns>計算結果のオフセット座標</returns>
        public static Vector2 CalculateLayoutOverlay(IBalloonLayoutProperty property)
        {
            // Canvasの実スケールに合わせたサイズに補正
            var offset = Vector2.Scale(property.ContentSize, property.ContentLossyScale);

            // Pivotを加味して中央に補正
            offset.x *= property.ContentPivot.x - 0.5f;

            // Pivotを加味して中央に補正
            offset.y *= property.ContentPivot.y - 0.5f;

            return offset;
        }

#endregion
    }
}
