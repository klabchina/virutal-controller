﻿/**
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
    public class ScrollSelectVerticalLoopLogic : ScrollSelectLoopLogicBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property"></param>
        public ScrollSelectVerticalLoopLogic(ScrollSelectLoopTypeLogicProperty property) : base(property)
        {
        }

        /// <summary>
        /// スクロールの開始点の座標を返します
        /// </summary>
        /// <value>The scroll origin point.</value>
        public override Vector2 ScrollOriginPoint
        {
            get
            {
                return new Vector2(0f, ContentPreferredSize * 0.5f - property.ViewportLength * 0.5f);
            }
        }
    }
}