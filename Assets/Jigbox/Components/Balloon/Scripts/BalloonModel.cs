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
    /// バルーンコンポーネントのモデル
    /// </summary>
    [System.Serializable]
    public class BalloonModel : IBalloonLayoutProperty
    {
#region serializefield

        /// <summary>
        /// バルーンのレイアウト位置をどこにするか決める列挙
        /// </summary>
        [SerializeField]
        [HideInInspector]
        BalloonLayout balloonLayout;

        /// <summary>
        /// バルーンで取り扱うオブジェクトの参照
        /// 位置計算はこのオブジェクト基準で行われる
        /// </summary>
        [SerializeField]
        [HideInInspector]
        RectTransform balloonContent;

        /// <summary>
        /// バルーンが表示できる領域を決める、自動レイアウト矩形オブジェクトの参照
        /// この矩形内に収まるようにバルーンが表示される
        /// </summary>
        [SerializeField]
        [HideInInspector]
        RectTransform autoLayoutArea;

        /// <summary>
        /// バルーンの基準座標と隣り合うContentの辺のどこを中心として位置計算を行うか
        /// 0.5の場合中心、0.0と1.0の場合端を基準にして計算を行う
        /// </summary>
        [SerializeField]
        [HideInInspector]
        float balloonLayoutPositionRate;

        /// <summary>
        /// 基準座標とバルーンの間にどれだけ空間を空けるか
        /// </summary>
        [SerializeField]
        [HideInInspector]
        float spacing;

        /// <summary>
        /// バルーンの位置を計算するクラスの参照
        /// </summary>
        [SerializeField]
        [HideInInspector]
        BalloonLayoutCalculator calculator;

        /// <summary>
        /// バルーンの位置計算の基準となるワールド座標
        /// </summary>
        [SerializeField]
        [HideInInspector]
        Vector2 basePosition;

        /// <summary>
        /// BasePositionの基準になるRectTransformの参照
        /// </summary>
        [SerializeField]
        [HideInInspector]
        RectTransform basePositionRectTransform;

#endregion

#region property

        /// <summary>
        /// レイアウト位置のアクセサ
        /// </summary>
        public virtual BalloonLayout BalloonLayout
        {
            get { return balloonLayout; }
            set { balloonLayout = value; }
        }

        /// <summary>
        /// バルーンで取り扱うオブジェクトのアクセサ
        /// </summary>
        public virtual RectTransform BalloonContent
        {
            get { return balloonContent; }
            set { balloonContent = value; }
        }

        /// <summary>
        /// バルーンで取り扱うオブジェクトのワールド座標
        /// </summary>
        public virtual Vector2 ContentPosition
        {
            get { return balloonContent.position; }
        }

        /// <summary>
        /// バルーンで取り扱うオブジェクトのサイズ
        /// </summary>
        public virtual Vector2 ContentSize
        {
            get { return balloonContent.rect.size; }
        }

        /// <summary>
        /// バルーンで取り扱うオブジェクトのPivot
        /// </summary>
        public virtual Vector2 ContentPivot
        {
            get { return balloonContent.pivot; }
        }

        /// <summary>
        /// バルーンで取り扱うオブジェクトの実Scale
        /// </summary>
        public virtual Vector2 ContentLossyScale
        {
            get { return balloonContent.lossyScale; }
        }

        /// <summary>
        /// バルーンが表示できる領域を決めるオブジェクトのアクセサ
        /// </summary>
        public virtual RectTransform AutoLayoutArea
        {
            get { return autoLayoutArea; }
            set { autoLayoutArea = value; }
        }

        /// <summary>
        /// 自動レイアウトを行うかどうか
        /// </summary>
        public virtual bool IsAutoLayout
        {
            get { return autoLayoutArea != null; }
        }

        /// <summary>
        /// 自動レイアウト矩形のワールド座標
        /// </summary>
        public virtual Vector2 AutoLayoutAreaPosition
        {
            get { return autoLayoutArea.position; }
        }

        /// <summary>
        /// 自動レイアウト矩形のサイズ
        /// </summary>
        public virtual Vector2 AutoLayoutAreaSize
        {
            get { return autoLayoutArea.rect.size; }
        }

        /// <summary>
        /// 自動レイアウト矩形のPivot
        /// </summary>
        public virtual Vector2 AutoLayoutAreaPivot
        {
            get { return autoLayoutArea.pivot; }
        }

        /// <summary>
        /// 自動レイアウト矩形の実スケール
        /// </summary>
        public virtual Vector2 AutoLayoutAreaLossyScale
        {
            get { return AutoLayoutArea.lossyScale; }
        }

        /// <summary>
        /// バルーンの基準座標と隣り合うContentの辺のどこを中心とするかのアクセサ
        /// </summary>
        public virtual float BalloonLayoutPositionRate
        {
            get { return balloonLayoutPositionRate; }
            set { balloonLayoutPositionRate = value; }
        }

        /// <summary>
        /// 基準座標とバルーンの間にどれだけ空間を空けるかのアクセサ
        /// </summary>
        public virtual float Spacing
        {
            get { return spacing; }
            set { spacing = value; }
        }

        /// <summary>
        /// バルーンの位置計算の基準となるワールド座標のアクセサ
        /// </summary>
        public virtual Vector2 BasePosition
        {
            get { return basePosition; }
            set { basePosition = value; }
        }

        /// <summary>
        /// BasePositionの基準になるRectTransformのアクセサ
        /// セッターでBasePositionも自動更新される
        /// </summary>
        public RectTransform BasePositionRectTransform
        {
            get { return basePositionRectTransform; }
            set
            {
                basePositionRectTransform = value;
                BasePosition = BalloonBasePositionUtil.GetBasePositionByLayout(basePositionRectTransform, BalloonLayout);
            }
        }

        /// <summary>
        /// バルーンの位置を計算するクラスのアクセサ
        /// </summary>
        public virtual BalloonLayoutCalculator Calculator
        {
            get
            {
                if (!calculator.IsSetProperty)
                {
                    calculator.SetProperty(this);
                }

                return calculator;
            }
            set { calculator = value; }
        }

#endregion

#region public methods

        /// <summary>
        /// レイアウト情報を加味した位置を計算しContentを移動させる
        /// </summary>
        public virtual void UpdateContentPosition()
        {
            var result = Calculator.CalculateLayout();

            // AutoLayoutAreaがあり、AutoLayoutAreaのサイズがバルーンサイズよりも小さい場合には
            // レイアウトが行えないためエラーを出す
            if (!result.IsSuccess)
            {
#if UNITY_EDITOR
                Debug.LogError("Not enough AutoLayoutArea size. Should be resize AutoLayoutArea.");
#endif
                return;
            }

            BalloonContent.position = result.ResultPosition;
        }

#endregion
    }
}
