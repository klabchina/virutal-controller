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
    /// CarouselのHorizontal用View(Loop版)
    /// </summary>
    public class CarouselViewHorizontalLoop : CarouselViewLoopBase
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
        /// ContentのTransformがセンタリング(LocalPositionが0)になるまでの距離
        /// </summary>
        public override Vector3 DistanceToCenteringPosition()
        {
            return new Vector3(-tempDelta, 0f, 0f);
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

#endregion
    }
}
