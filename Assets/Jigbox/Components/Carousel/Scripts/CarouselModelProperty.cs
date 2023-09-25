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

using UnityEngine.UI;

namespace Jigbox.Components
{
    public class CarouselModelProperty
    {
#region properties

        protected Carousel carousel;

        /// <summary>
        /// 動作させる方向を取得します
        /// </summary>
        public virtual GridLayoutGroup.Axis Axis { get { return carousel.CellLayoutGroup.startAxis; } }

        /// <summary>
        /// loopTypeがループ設定になっているかどうかを返します
        /// </summary>
        public virtual bool IsLoop { get { return carousel.IsLoop; } }

        /// <summary>
        /// Cellの数を返します
        /// </summary>
        public virtual int CellCount { get { return carousel.CellCount; } }

        /// <summary>
        /// 現在表示中のCellのIndexを返します
        /// </summary>
        public virtual int ShowIndex { get { return carousel.ShowIndex; } }

#endregion

#region public methods

        public CarouselModelProperty(Carousel carousel)
        {
            this.carousel = carousel;
        }

#endregion
    }
}
