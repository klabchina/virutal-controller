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

namespace Jigbox.Components
{
    /// <summary>
    /// CarouselのViewで扱うプロパティの参照クラス
    /// </summary>
    public class CarouselViewProperty
    {
#region fields

        /// <summary>
        /// Carouselクラス
        /// </summary>
        protected Carousel carousel;

#endregion

#region properties

        /// <summary>
        /// GridLayoutGroup
        /// </summary>
        public virtual GridLayoutGroup Layout { get { return this.carousel.CellLayoutGroup; } }

        /// <summary>
        /// 動作させる方向を取得します
        /// </summary>
        public virtual GridLayoutGroup.Axis Axis { get { return this.carousel.CellLayoutGroup.startAxis; } }

        /// <summary>
        /// 現在表示中のCellのIndexを返します
        /// </summary>
        public virtual int ShowIndex { get { return carousel.ShowIndex; } }

        /// <summary>
        /// Cellの数を返します
        /// </summary>
        public virtual int CellCount { get { return this.carousel.CellCount; } }

        /// <summary>
        /// ContentのLocalPositionを取得します
        /// </summary>
        public virtual Vector3 ContentLocalPosition { get { return this.carousel.CellLayoutGroup.transform.localPosition; } }

        /// <summary>
        /// CellSizeのxの値を返します
        /// </summary>
        public virtual float CellSizeXValue { get { return this.carousel.CellLayoutGroup.cellSize.x; } }

        /// <summary>
        /// CellSizeのyの値を返します
        /// </summary>
        public virtual float CellSizeYValue { get { return this.carousel.CellLayoutGroup.cellSize.y; } }

        /// <summary>
        /// CellSizeのxの値を返します
        /// </summary>
        public virtual float CellSpacingXValue { get { return this.carousel.CellLayoutGroup.spacing.x; } }

        /// <summary>
        /// CellSizeのyの値を返します
        /// </summary>
        public virtual float CellSpacingYValue { get { return this.carousel.CellLayoutGroup.spacing.y; } }

#endregion

#region constractor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="carousel">Carouselクラス</param>
        public CarouselViewProperty(Carousel carousel)
        {
            this.carousel = carousel;
        }

#endregion
    }
}
