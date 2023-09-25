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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// セルの情報クラス
    /// </summary>
    public class VariableListCellInfo
    {
#region fields & properties

        /// <summary>
        /// セルの番号
        /// </summary>
        protected int index;

        /// <summary>
        /// セルの番号の参照
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// サイズが可変するかどうか
        /// </summary>
        protected bool isVariable;

        /// <summary>
        /// サイズが可変するかどうかへの参照
        /// </summary>
        public bool IsVariable
        {
            get { return isVariable; }
        }

        /// <summary>
        /// セルのサイズ
        /// </summary>
        protected float size;

        /// <summary>
        /// セルのサイズの参照
        /// </summary>
        public float Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Prefabのハッシュコード
        /// </summary>
        public int PrefabHash
        {
            get { return CellPrefab.GetHashCode(); }
        }

        /// <summary>
        /// セルのPrefab
        /// </summary>
        protected VariableListCell cellPrefab;

        /// <summary>
        /// セルのPrefabの参照
        /// </summary>
        public VariableListCell CellPrefab
        {
            get { return cellPrefab; }
            set { cellPrefab = value; }
        }

        /// <summary>
        /// セルの前方間隔
        /// </summary>
        protected float spacingFront;

        /// <summary>
        /// セルの前方間隔への参照
        /// </summary>
        public float SpacingFront
        {
            get { return spacingFront; }
            set { spacingFront = value; }
        }

        /// <summary>
        /// セルの後方間隔
        /// </summary>
        protected float spacingBack;

        /// <summary>
        /// セルの後方間隔への参照
        /// </summary>
        public float SpacingBack
        {
            get { return spacingBack; }
            set { spacingBack = value; }
        }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index">セルの番号</param>
        /// <param name="cellPrefab">セルのPrefab</param>
        public VariableListCellInfo(int index, VariableListCell cellPrefab)
        {
            this.index = index;
            this.cellPrefab = cellPrefab;
            this.size = cellPrefab.CellSize;
            this.isVariable = cellPrefab.IsVariable;
            this.SpacingFront = cellPrefab.SpacingFront;
            this.SpacingBack = cellPrefab.SpacingBack;
        }

#endregion
    }
}
