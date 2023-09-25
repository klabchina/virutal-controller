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

using Jigbox.Components;
using UnityEngine;

namespace Jigbox
{
    public class ScrollSelectRectTransform : IndexedRectTransform
    {

        /// <summary>
        /// indexの初期値に使う数値
        /// </summary>
        protected static readonly int InvalidIndex = -999;
        
        /// <summary>
        /// セルの位置を決めるためのIndex
        /// </summary>
        public int PositionIndex { get; set; }

        /// <summary>
        /// セルが選択状態かどうかを返す
        /// </summary>
        protected bool isSelected;

        /// <summary>
        /// セルが選択状態かどうかを返すプロパティ
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected;}
            set
            {
                isSelected = value;
                if (scrollSelectCell != null)
                {
                    scrollSelectCell.IsSelected = isSelected;
                }
             }
        }

        /// <summary>
        /// セル側に付けられているIScrollSelectCellを保持する
        /// </summary>
        protected IScrollSelectCell scrollSelectCell;

        /// <summary>
        /// PositionIndexには渡されてきた数値をいれるが、Indexには初期化時は-1を設定させる
        /// </summary>
        /// <param name="i"></param>
        /// <param name="t"></param>
        public ScrollSelectRectTransform(int i, RectTransform t) : base(InvalidIndex, t)
        {
            PositionIndex = i;
            if (RectTransform != null)
            {
                scrollSelectCell = RectTransform.GetComponent<IScrollSelectCell>();
            }
        }

        /// <summary>
        /// scrollSelectCellがもつIndexの情報を更新します
        /// </summary>
        public void UpdateScrollSelectCellIndex()
        {
            if (scrollSelectCell == null)
            {
                return;
            }

            scrollSelectCell.CellIndex = Index;
        }
    }
}
