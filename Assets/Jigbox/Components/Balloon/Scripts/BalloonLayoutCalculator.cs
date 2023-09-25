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
    /// バルーンの位置計算を行うクラス
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Balloon))]
    public class BalloonLayoutCalculator : MonoBehaviour
    {
#region property

        /// <summary>
        /// 計算で使用するプロパティ
        /// </summary>
        protected IBalloonLayoutProperty property;

        /// <summary>
        /// プロパティが設定されているかどうか
        /// </summary>
        public bool IsSetProperty
        {
            get { return property != null; }
        }

#endregion

#region protected methods

        /// <summary>
        /// 自動レイアウトを行える状態かどうかを返す
        /// 自動レイアウト矩形の参照がない or 自動レイアウト矩形がバルーンのサイズより小さい場合は行えない
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsValidAutoLayoutAreaSize()
        {
            if (!property.IsAutoLayout)
            {
                return true;
            }

            return BalloonCalculateLayoutLogic.IsValidAutoLayoutAreaSize(property.ContentSize,
                property.AutoLayoutAreaSize);
        }

        /// <summary>
        /// レイアウトに応じた位置計算を行う
        /// </summary>
        /// <returns>計算結果の座標</returns>>
        protected virtual Vector2 Calculate()
        {
            return BalloonCalculateLayoutLogic.CalculateLayout(property);
        }

        /// <summary>
        /// 自動レイアウト矩形を使用して計算後の位置の補正を行う
        /// </summary>
        /// <returns>オフセット座標</returns>>
        protected virtual Vector2 RePositionAutoLayout(Vector2 currentPosition)
        {
            if (!property.IsAutoLayout)
            {
                return Vector2.zero;
            }

            return BalloonCalculateLayoutLogic.CalculateAutoLayoutOffset(currentPosition, property);
        }

#endregion

#region public methods

        /// <summary>
        /// プロパティを設定します
        /// </summary>
        /// <param name="property">レイアウト計算に使用するプロパティ</param>
        public virtual void SetProperty(IBalloonLayoutProperty property)
        {
            this.property = property;
        }

        /// <summary>
        /// バルーンの位置を計算して結果を返す
        /// 計算が失敗した場合は座標は初期状態のまま
        /// </summary>
        /// <returns>計算結果</returns>
        public virtual BalloonLayoutCalculateResult CalculateLayout()
        {
            BalloonLayoutCalculateResult result = new BalloonLayoutCalculateResult();

            // 自動レイアウト矩形のサイズがバルーンのサイズより小さい場合
            if (!IsValidAutoLayoutAreaSize())
            {
                result.IsSuccess = false;
                return result;
            }

            result.ResultPosition = Calculate();
            result.ResultPosition += RePositionAutoLayout(result.ResultPosition);

            result.IsSuccess = true;

            return result;
        }

#endregion
    }
}
