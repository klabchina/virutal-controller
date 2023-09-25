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
    public class ScrollSelectVerticalNoneLoopLogic : ScrollSelectNoneLoopLogicBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property"></param>
        public ScrollSelectVerticalNoneLoopLogic(ScrollSelectLoopTypeLogicProperty property) : base(property)
        {
        }
        
        /// <summary>
        /// 指定されたindexのセルが選択状態のときのContentのポジションを返します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override Vector3 CalculateJumpPositionBySelectedIndex(int index)
        {
            return new Vector3(0f, (property.CellSize + property.Spacing) * index);
        }
    }
}
