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

namespace Jigbox.Components
{
    /// <summary>
    /// チャイルドエリアセルの基底クラス
    /// </summary>
    public class AccordionListChildAreaCell : AccordionListCellBase
    {
        public override AccordionListCellType CellType
        {
            get { return AccordionListCellType.ChildArea; }
        }

        public override Padding Margin
        {
            get { return Padding.zero; }
            set { }
        }

        public override void OnUpdateCell(AccordionListBase accordionList, AccordionListNode node)
        {
        }

        /// <summary>チャイルドエリアはサイズが子孫情報によって決まるのでサイズ計算は行われない</summary>
        public sealed override float OnUpdateCellSize(AccordionListBase accordionList, AccordionListNode node)
        {
            throw new NotImplementedException();
        }
    }
}
