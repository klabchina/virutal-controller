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
    /// Carousel用のView(Loopあり版)のベースとなる抽象クラス
    /// </summary>
    public abstract class CarouselViewLoopBase : CarouselViewBase
    {
#region override methods

        /// <summary>
        /// ドラッグのdelta値を受け取ってContentの位置を移動します
        /// </summary>
        /// <param name="delta"></param>
        /// <returns>indexのoffset値</returns>
        public override int MoveContentByDelta(Vector3 delta)
        {
            // 計算前のindexのoffsetを保持
            var prevOffsetIndex = offsetIndex;
            var oneContentSize = new Vector2(this.property.CellSizeXValue + this.property.CellSpacingXValue, this.property.CellSizeYValue + this.property.CellSpacingYValue);
            var calcIndex = 0;
            // 閾値(CellSizeの半分)
            var halfCellSize = GetHalfCellSize(oneContentSize);

            tempDelta += GetValueByAxis(delta);

            var isPlusVector = false;

            // 閾値を越えたらindexチェックを行う
            if (tempDelta >= halfCellSize)
            {
                // Horizontalの場合はPrev方向、Verticalの場合はNext方向
                var halfIndex = Mathf.FloorToInt(tempDelta / halfCellSize);
                calcIndex = halfIndex > 0 ? ((halfIndex - 1) / 2) + 1 : 0;
                isPlusVector = true;
            }
            else if (tempDelta <= -halfCellSize)
            {
                // Horizontalの場合はNext方向、Verticalの場合はPrev方向
                var halfIndex = Mathf.CeilToInt(tempDelta / halfCellSize);
                calcIndex = halfIndex < 0 ? ((halfIndex + 1) / 2) - 1 : 0;
            }

            tempDelta -= calcIndex * GetValueByAxis(oneContentSize);
            ChangeCellSiblingIndex(calcIndex, isPlusVector);

            // localPositionを移動する
            var pos = CalculateNewContentLocalPosition();
            if (this.property.Layout != null)
            {
                this.property.Layout.transform.localPosition = pos;
            }

            return prevOffsetIndex - offsetIndex;
        }

        /// <summary>
        /// 引数がPrev方向に動けるかどうかチェックし、最大量を引数分として動かせる分だけのDelta値を返します
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public override Vector3 GetValidDeltaPrev(Vector3 delta)
        {
#if UNITY_EDITOR
            // Loop版のViewでは呼ばれないのでここにアクセスしてきた場合はExceptionを吐く
            throw new System.Exception("You do not need to call this method in the loop version.");
#else
            return Vector3.zero;
#endif
        }

        /// <summary>
        /// 引数がNext方向に動けるかどうかをチェックし、最大量を引数分として動かせる分だけのDelta値を返します
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public override Vector3 GetValidDeltaNext(Vector3 delta)
        {
#if UNITY_EDITOR
            // Loop版のViewでは呼ばれないのでここにアクセスしてきた場合はExceptionを吐く
            throw new System.Exception("You do not need to call this method in the loop version.");
#else
            return Vector3.zero;
#endif
        }

        /// <summary>
        /// Loopなし版に必要な各種データを初期化します
        /// </summary>
        public override void InitNoLoopViewData()
        {
#if UNITY_EDITOR
            // Loop版のViewでは呼ばれないのでここにアクセスしてきた場合はExceptionを吐く
            throw new System.Exception("You do not need to call this method in the loop version.");
#endif
        }

#endregion

#region private methods

        /// <summary>
        /// CellのSiblingIndexを変更します
        /// </summary>
        /// <param name="loopCount">移動したい希望のindex</param>
        /// <param name="isPlusVector">プラス方向(Horizontalだと右方向、Verticalだと上方向)にスライドさせる場合</param>
        protected void ChangeCellSiblingIndex(int loopCount, bool isPlusVector)
        {
            if (this.property.Layout == null)
            {
                return;
            }

            if (loopCount == 0)
            {
                return;
            }

            var count = Mathf.Abs(loopCount);

            if ((this.property.Axis == GridLayoutGroup.Axis.Horizontal && isPlusVector) ||
                (this.property.Axis == GridLayoutGroup.Axis.Vertical && !isPlusVector))
            {
                var lastIndex = this.property.CellCount - 1;
                for (int i = 0; i < count; i++)
                {
                    this.property.Layout.transform.GetChild(lastIndex).SetAsFirstSibling();
                    offsetIndex++;
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    this.property.Layout.transform.GetChild(0).SetAsLastSibling();
                    offsetIndex--;
                }
            }
        }

#endregion
    }
}
