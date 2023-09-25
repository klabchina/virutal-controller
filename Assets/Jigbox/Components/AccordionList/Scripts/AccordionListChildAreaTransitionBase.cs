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

using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// チャイルドエリアのトランジション基底クラス
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AccordionListChildAreaTransitionBase : MonoBehaviour
    {
        /// <summary>
        /// 展開トランジションを開始する
        /// </summary>
        /// <param name="cellInfos">チャイルドエリアセル</param>
        /// <param name="expandSize">展開サイズ</param>
        /// <param name="changeValue">トランジションの変化サイズ (BasicTransitionの変化と合わせる為に必要)</param>
        public abstract void StartExpand(List<AccordionListCellInfo> cellInfos, float expandSize, float changeValue);

        /// <summary>
        /// 折り畳みトランジションを開始する
        /// </summary>
        /// <param name="cellInfos">チャイルドエリアセル</param>
        /// <param name="collapseSize">展開サイズ</param>
        public abstract void StartCollapse(List<AccordionListCellInfo> cellInfos, float collapseSize);

        /// <summary>
        /// 他のトランジションに合わせて終了させる
        /// </summary>
        public abstract void ForceComplete();
    }
}
