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
    /// CarouselのHorizontal用View(NoLoop版)
    /// </summary>
    public class CarouselViewHorizontal : CarouselViewNoLoopBase
    {
#region override methods

        /// <summary>
        /// tempDeltaとcontentBasePosの値を加味したContentのLocalPositionを返します
        /// </summary>
        /// <returns></returns>
        public override Vector3 CalculateNewContentLocalPosition()
        {
            return new Vector3(tempDelta + contentBasePos.x, ContentLocalPosition.y, ContentLocalPosition.z);
        }

        /// <summary>
        /// Cellを引数分ずらす時のontentの移動量を返します
        /// </summary>
        /// <param name="offset">ずらしたいCellの個数</param>
        /// <returns>移動量(Vector3)</returns>
        public override Vector3 GetAmountOfMovement(int offset)
        {
            return new Vector3((this.property.CellSizeXValue + this.property.CellSpacingXValue) * offset, 0f);
        }

        /// <summary>
        /// ContentのTransformがセンタリングされるまでの距離
        /// </summary>
        public override Vector3 DistanceToCenteringPosition()
        {
            var offsetTo = GetAmountOfMovement(this.property.ShowIndex);
            var distance = startTempDelta - offsetTo.x - tempDelta;
            return new Vector3(distance, 0f, 0f);
        }

        /// <summary>
        /// VectorがPrev方向に向いてるかどうかを返します
        /// </summary>
        /// <param name="vec">Vector3</param>
        /// <returns>プラス方向かどうか</returns>
        public override bool CheckDeltaPrevVector(Vector3 vec)
        {
            return vec.x > 0f;
        }

        /// <summary>
        /// VectorがNext方向に向いてるかどうかを返します
        /// </summary>
        /// <param name="vec">Vector3</param>
        /// <returns>Next方向かどうか</returns>
        public override bool CheckDeltaNextVector(Vector3 vec)
        {
            return vec.x < 0f;
        }

        /// <summary>
        /// 引数がPrev方向に動けるかどうかチェックし、最大量を引数分として動かせる分だけのDelta値を返します
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public override Vector3 GetValidDeltaPrev(Vector3 delta)
        {
            if (tempDelta + delta.x > startTempDelta)
            {
                // Prevの場合はstartTempDeltaのところが限界位置となるので、startTempDeltaからtempDeltaを引いた値を返す
                return new Vector3(startTempDelta - tempDelta, delta.y, delta.z);
            }
            return delta;
        }

        /// <summary>
        /// 引数がNext方向に動けるかどうかをチェックし、最大量を引数分として動かせる分だけのDelta値を返します
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public override Vector3 GetValidDeltaNext(Vector3 delta)
        {
            var maxDelta = startTempDelta - GetAmountOfMovement(this.property.CellCount - 1).x;
            if (tempDelta + delta.x < maxDelta)
            {
                // Nextの場合はstartTempDeltaからContentのサイズ*CellCount分が限界位置となるので、maxDeltaXからtempDeltaを引いた値を返す
                return new Vector3(maxDelta - tempDelta, delta.y, delta.z);
            }
            return delta;
        }

#endregion

#region protected methods

        /// <summary>
        /// ContentのBaseとなるPositionを計算して返します
        /// </summary>
        /// <param name="cellCount">Cellの個数</param>
        /// <returns></returns>
        protected override Vector3 CalculateContentBasePosition(int cellCount)
        {
            // 偶数の場合のPosition
            var halfPosition = GetAmountOfMovement(1) / 2;
            var evenPosition = new Vector3(halfPosition.x, 0f, 0f);
            return cellCount % 2 == 0 ? evenPosition : Vector3.zero;
        }

        /// <summary>
        /// calcIndexの値を適正な符号で変換した値にしてoffsetIndexとして返します
        /// </summary>
        /// <param name="calcIndex"></param>
        /// <returns></returns>
        protected override int ChangeOffsetIndexByCalcIndex(int calcIndex)
        {
            // Horizontalはそのまま返す
            return calcIndex;
        }

#endregion
    }
}
