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
    /// アコーディオンリストオプショナルセル
    /// </summary>
    public abstract class AccordionListOptionalCell : AccordionListCellBase
    {
#region override properties & methods
        
        /// <summary>サイズが可変するかどうか</summary>
        [SerializeField]
        bool isVariable = true;

        /// <summary>サイズが可変するかどうかの参照</summary>
        public override bool IsVariable
        {
            get { return isVariable; }
        }

        /// <summary>セルのサイズ</summary>
        [SerializeField]
        float cellSize;

        /// <summary>セルのサイズへの参照</summary>
        public override float CellSize
        {
            get { return cellSize; }
            set { cellSize = value; }
        }

        /// <summary>セルの種類</summary>
        public override AccordionListCellType CellType
        {
            get { return AccordionListCellType.Optional; }
        }

        /// <summary>
        /// セル更新用のイベント
        /// </summary>
        /// <param name="accordionList">アコーディオンリスト</param>
        /// <param name="node">セルに紐づくノード</param>
        public override void OnUpdateCell(AccordionListBase accordionList, AccordionListNode node)
        {
        }

        /// <summary>
        /// セルサイズ変更用のイベント
        /// </summary>
        /// <param name="accordionList">アコーディオンリスト</param>
        /// <param name="node">セルに紐づくノード</param>
        /// <returns>表示するセルサイズ</returns>
        public override float OnUpdateCellSize(AccordionListBase accordionList, AccordionListNode node)
        {
            return CellSize;
        }

#endregion
    }
}
