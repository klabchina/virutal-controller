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
    /// Carousel用のView(Loopなし版)のベースとなる抽象クラス
    /// </summary>
    public abstract class CarouselViewNoLoopBase : CarouselViewBase
    {
#region override methods

        /// <summary>
        /// ドラッグのdelta値を受け取ってContentの位置を移動します
        /// </summary>
        /// <param name="delta">delta</param>
        public override int MoveContentByDelta(Vector3 delta)
        {
            // 計算前のindexのoffsetを保持
            var prevOffsetIndex = offsetIndex;
            var oneContentSize = new Vector2(this.property.CellSizeXValue + this.property.CellSpacingXValue, this.property.CellSizeYValue + this.property.CellSpacingYValue);
            var calcIndex = 0;
            // 閾値(CellSizeの半分)
            var halfCellSize = GetHalfCellSize(oneContentSize);

            tempDelta += GetValueByAxis(delta);

            var diffTempDelta = tempDelta - startTempDelta;

            if (diffTempDelta > 0)
            {
                // Horizontalの場合はPrev方向、Verticalの場合はNext方向
                var halfIndex = Mathf.FloorToInt(diffTempDelta / halfCellSize);
                calcIndex = halfIndex > 0 ? ((halfIndex - 1) / 2) + 1 : 0;
            }
            else if (diffTempDelta <= 0)
            {
                // Horizontalの場合はNext方向、Verticalの場合はPrev方向
                var halfIndex = Mathf.CeilToInt(diffTempDelta / halfCellSize);
                calcIndex = halfIndex < 0 ? ((halfIndex + 1) / 2) - 1 : 0;
            }

            // calcIndexの値は、Horizontalの場合は前のIndex、Verticalの場合は次のIndexのoffset値を示している
            offsetIndex = ChangeOffsetIndexByCalcIndex(calcIndex);

            // localPositionを移動する
            var pos = CalculateNewContentLocalPosition();
            if (this.property.Layout != null)
            {
                this.property.Layout.transform.localPosition = pos;
            }

            return prevOffsetIndex - offsetIndex;
        }

        /// <summary>
        /// Loopなし版に必要な各種データを初期化します
        /// </summary>
        public override void InitNoLoopViewData()
        {
            startTempDelta = tempDelta;
            offsetIndex = 0;
        }

#endregion

#region private methods

        /// <summary>
        /// calcIndexの値を適正な符号で変換した値にしてoffsetIndexとして返します
        /// </summary>
        /// <param name="calcIndex"></param>
        /// <returns></returns>
        protected abstract int ChangeOffsetIndexByCalcIndex(int calcIndex);

#endregion
    }
}
