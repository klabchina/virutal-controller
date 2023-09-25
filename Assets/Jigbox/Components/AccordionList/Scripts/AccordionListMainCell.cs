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
    public abstract class AccordionListMainCell : AccordionListCellBase
    {
#region serialize fields

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

        /// <summary>セルの前方間隔</summary>
        [SerializeField]
        float spacingFront;

        /// <summary>セルの前方間隔への参照</summary>
        public override float SpacingFront
        {
            get { return spacingFront; }
            set { spacingFront = value; }
        }

        /// <summary>セルの後方間隔</summary>
        [SerializeField]
        float spacingBack;

        /// <summary>セルの後方間隔への参照</summary>
        public override float SpacingBack
        {
            get { return spacingBack; }
            set { spacingBack = value; }
        }

        /// <summary>チャイルドエリアのPadding</summary>
        [SerializeField]
        Padding childAreaPadding = Padding.zero;

        /// <summary>チャイルドエリアのPadding参照</summary>
        public virtual Padding ChildAreaPadding
        {
            get { return childAreaPadding; }
            set { childAreaPadding = value; }
        }

        /// <summary>セルの種類</summary>
        public override AccordionListCellType CellType
        {
            get { return AccordionListCellType.Main; }
        }

#endregion

#region public methods

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
