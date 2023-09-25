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
    public class ScrollSelectLoopTypeLogicProperty
    {
        /// <summary>
        /// モデルクラスへの参照
        /// </summary>
        IScrollSelectLayout model;

        /// <summary>
        /// 表示させたい、仮想のセルの総数を参照/指定します
        /// </summary>
        /// <value>The virtual cell count.</value>
        public int VirtualCellCount
        {
            get { return model.VirtualCellCount; }
        }

        /// <summary>
        /// 可視領域の長さを返します
        /// </summary>
        public float ViewportLength
        {
            get { return model.ViewportLength; }
        }

        /// <summary>
        /// セルの大きさを参照/指定します
        /// </summary>
        /// <value>The size of the cell.</value>
        public float CellSize
        {
            get { return model.CellSize; }
        }

        /// <summary>
        /// セル同士の間隔を参照/指定します
        /// </summary>
        /// <value>The spacing.</value>
        public float Spacing
        {
            get { return model.Spacing; }
        }

        public ScrollSelectLoopTypeLogicProperty(IScrollSelectLayout model)
        {
            this.model = model;
        }
    }
}
