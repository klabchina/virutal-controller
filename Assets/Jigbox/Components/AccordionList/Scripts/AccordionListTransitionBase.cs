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

using System;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// トランジションインターフェース
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AccordionListTransitionBase : MonoBehaviour
    {
        /// <summary>
        /// 展開トランジションハンドラを登録
        /// </summary>
        /// <param name="transitionHandler">トランジションハンドラ</param>
        public abstract void SetHandler(AccordionListTransitionHandlerBase transitionHandler);

        /// <summary>
        /// 展開トランジションを開始する
        /// </summary>
        /// <param name="begin">開始値</param>
        /// <param name="final">終了値</param>
        public abstract void ExpandTransition(float begin, float final);

        /// <summary>
        /// 折り畳みトランジションを開始する
        /// </summary>
        /// <param name="begin">開始点</param>
        /// <param name="final">終了点</param>
        public abstract void CollapseTransition(float begin, float final);
    }
}
