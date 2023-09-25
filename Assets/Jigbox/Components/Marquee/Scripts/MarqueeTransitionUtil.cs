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

namespace Jigbox.Components
{
    public static class MarqueeTransitionUtil
    {
#region public methods

        /// <summary>
        /// 移動させる必要のある長さ全体の計算をします
        /// startPositionRateとendPositionRateによる指定の影響分も加えた長さを返します
        /// </summary>
        /// <param name="contentLength"></param>
        /// <param name="viewportLength"></param>
        /// <param name="startPositionRate"></param>
        /// <param name="endPositionRate"></param>
        /// <returns></returns>
        public static float CalculateTotalLength(float contentLength, float viewportLength, float startPositionRate, float endPositionRate)
        {
            // viewportに対しての割合で指定される前方のスペース分
            var frontOffset = startPositionRate * viewportLength;
            // viewportに対しての割合で指定される後方のスペース分
            var backOffset = endPositionRate * viewportLength;
            // 移動させるContent全体の長さを計算
            return contentLength + frontOffset - backOffset;
        }

        /// <summary>
        /// マーキーをTweenで動かす際に指定された速度で動かすためのDurationの値を計算します
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="contentLength"></param>
        /// <param name="viewportLength"></param>
        /// <param name="startPositionRate"></param>
        /// <param name="endPositionRate"></param>
        /// <returns></returns>
        public static float CalculateDuration(float speed, float contentLength, float viewportLength, float startPositionRate, float endPositionRate, MarqueeScrollType scrollType)
        {
            if (speed <= 0)
            {
                return 0f;
            }

            // スクロール処理がいらない場合は動かないため0になる
            if (InvalidScroll(contentLength, viewportLength, scrollType))
            {
                return 0f;
            }

            // 移動させるContent全体の長さを計算
            var totalLength = CalculateTotalLength(contentLength, viewportLength, startPositionRate, endPositionRate);

            if (totalLength < 0)
            {
                return 0f;
            }
            // 指定されたスピードで流すためのDurationTimeを返す
            return totalLength / speed;
        }

        /// <summary>
        /// 開始する位置を計算します
        /// </summary>
        /// <param name="contentLength"></param>
        /// <param name="viewportLength"></param>
        /// <param name="startPositionRate"></param>
        /// <param name="scrollType"></param>
        /// <param name="directionType">スクロールの向き</param>
        /// <returns></returns>
        public static float CalculateBeginPosition(float contentLength, float viewportLength, float startPositionRate, MarqueeScrollType scrollType, MarqueeScrollDirectionType directionType = MarqueeScrollDirectionType.Normal)
        {
            // スクロール処理がいらない場合は開始位置は変更しない
            if (InvalidScroll(contentLength, viewportLength, scrollType))
            {
                return 0f;
            }

            var beginPosition = 0.0f;
            
            switch (directionType)
            {
                case MarqueeScrollDirectionType.Reverse:
                    beginPosition = -contentLength + viewportLength + startPositionRate * -viewportLength;
                    break;
                default:
                    beginPosition = startPositionRate * viewportLength;
                    break;
            }
            
            return beginPosition;
        }

        /// <summary>
        /// 終了する位置を計算します
        /// </summary>
        /// <param name="contentLength"></param>
        /// <param name="viewportLength"></param>
        /// <param name="endPositionRate"></param>
        /// <param name="scrollType"></param>
        /// <param name="directionType">スクロールの向き</param>
        /// <returns></returns>
        public static float CalculateEndPosition(float contentLength, float viewportLength, float endPositionRate, MarqueeScrollType scrollType, MarqueeScrollDirectionType directionType = MarqueeScrollDirectionType.Normal)
        {
            // スクロール処理がいらない場合は終了位置は変更しない
            if (InvalidScroll(contentLength, viewportLength, scrollType))
            {
                return 0f;
            }
            
            var endPosition = 0.0f;

            switch (directionType)
            {
                case MarqueeScrollDirectionType.Reverse:
                    
                    endPosition = -viewportLength - endPositionRate * -viewportLength;
                    if (viewportLength > contentLength)
                    {
                        endPosition += contentLength - viewportLength;
                    }
                    break;
                default:
                    endPosition = contentLength - endPositionRate * viewportLength;
                    endPosition = endPosition < 0 ? 0 : endPosition;
                    break;
            }
            
            return endPosition;
        }

        /// <summary>
        /// スクロール処理をしないでよいか返す
        /// </summary>
        /// <param name="contentLength"></param>
        /// <param name="viewportLength"></param>
        /// <param name="scrollType"></param>
        /// <returns></returns>
        public static bool InvalidScroll(float contentLength, float viewportLength, MarqueeScrollType scrollType)
        {
            // スクロールタイプがIfNeededかつ、ビューポートサイズがコンテンツ全体の長さ以上ある場合スクロールしないでよい
            if (scrollType == MarqueeScrollType.IfNeeded && contentLength <= viewportLength)
            {
                return true;
            }

            return false;
        }

#endregion
    }
}
