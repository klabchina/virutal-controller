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

namespace Jigbox.Components
{
    /// <summary>
    /// アコーディオンリストのルートノード
    /// </summary>
    public class AccordionListRootNode : AccordionListNode
    {
        public AccordionListRootNode() : base(null)
        {
        }

#region override properties

        public override int Id
        {
            get { return int.MinValue; }
        }

        public override int ParentId
        {
            get { return int.MinValue + 1; }
        }

        public override float SpacingBack
        {
            get { return 0; }
        }

        public override float SpacingFront
        {
            get { return 0; }
        }

        public override Padding Margin
        {
            get { return Padding.zero; }
        }

        public override Padding ChildAreaPadding
        {
            get { return Padding.zero; }
        }

        public override Padding OptionalCellMargin
        {
            get { return Padding.zero; }
        }

        public override bool HasOptionalCell
        {
            get { return false; }
        }

        public override bool IsExpand
        {
            get { return true; }
        }

#endregion

        /// <summary>
        /// セル情報に変換する
        /// </summary>
        /// <returns></returns>
        public AccordionListCellInfo ToCellInfo()
        {
            return new AccordionListCellInfo(this, AccordionListCellType.Main, null, 0, false, 0, 0, 0, 0, Padding.zero, Padding.zero);
        }
    }
}
