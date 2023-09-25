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
    /// セル情報クラス
    /// </summary>
    public class AccordionListCellInfo
    {
        /// <summary>セル情報の状態</summary>
        public enum CellInfoStatus
        {
            /// <summary>更新なし</summary>
            Normal,

            /// <summary>表示対象に追加された</summary>
            Insert,

            /// <summary>表示対象から削除された</summary>
            Remove,

            /// <summary>追加と削除により更新された</summary>
            Update,
        }

#region properties

        /// <summary>セルの番号</summary>
        int index;

        /// <summary>セルの番号の参照</summary>
        public virtual int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>サイズが可変するかどうか</summary>
        readonly bool isVariable;

        /// <summary>サイズが可変するかどうかへの参照</summary>
        public virtual bool IsVariable
        {
            get { return isVariable; }
        }

        /// <summary>セルの種類</summary>
        readonly AccordionListCellType cellType;

        /// <summary>セルの種類の参照</summary>
        public virtual AccordionListCellType CellType
        {
            get { return cellType; }
        }

        /// <summary>セル変更フラグ</summary>
        CellInfoStatus status = CellInfoStatus.Normal;

        /// <summary>セル状態</summary>
        public virtual CellInfoStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>セルインスタンス参照</summary>
        public virtual AccordionListCellBase CellReference { get; set; }

        /// <summary>セルインスタンス参照の有無</summary>
        public virtual bool HasCellReference
        {
            get { return CellReference != null; }
        }

        /// <summary>セルの座標</summary>
        public virtual Vector2 CellPosition { get; set; }

        /// <summary>セルのサイズ</summary>
        float size;

        /// <summary>セルのサイズの参照</summary>
        public virtual float Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>Prefabのハッシュコード</summary>
        readonly int prefabHash;

        /// <summary>Prefabのハッシュコード</summary>
        public virtual int PrefabHash
        {
            get { return prefabHash; }
        }

        /// <summary>セルのPrefab</summary>
        readonly AccordionListCellBase cellPrefab;

        /// <summary>セルのPrefabの参照</summary>
        public virtual AccordionListCellBase CellPrefab
        {
            get { return cellPrefab; }
        }

        /// <summary>ノード</summary>
        readonly AccordionListNode node;

        /// <summary>ノードの参照</summary>
        public virtual AccordionListNode Node
        {
            get { return node; }
        }

        /// <summary>開いている時の前方余白</summary>
        readonly float expandSpacingFront;

        /// <summary>開いている時の前方余白の参照</summary>
        protected virtual float ExpandSpacingFront
        {
            get { return expandSpacingFront; }
        }

        /// <summary>開いている時の後方余白</summary>
        readonly float expandSpacingBack;

        /// <summary>開いている時の後方余白の参照</summary>
        protected virtual float ExpandSpacingBack
        {
            get { return expandSpacingBack; }
        }

        /// <summary>閉じている時の前方余白</summary>
        readonly float collapseSpacingFront;

        /// <summary>閉じている時の前方余白の参照</summary>
        protected virtual float CollapseSpacingFront
        {
            get { return collapseSpacingFront; }
        }

        /// <summary>閉じている時の後方余白</summary>
        readonly float collapseSpacingBack;

        /// <summary>閉じている時の後方余白の参照</summary>
        protected virtual float CollapseSpacingBack
        {
            get { return collapseSpacingBack; }
        }

        /// <summary>
        /// 現在の開閉状態に合わせた前方Spacing
        /// </summary>
        public virtual float CurrentSpacingFront
        {
            get { return node.IsExpand ? ExpandSpacingFront : CollapseSpacingFront; }
        }

        /// <summary>
        /// 現在の開閉状態に合わせた後方Spacing
        /// </summary>
        public virtual float CurrentSpacingBack
        {
            get { return node.IsExpand ? ExpandSpacingBack : CollapseSpacingBack; }
        }

        /// <summary>Padding</summary>
        readonly Padding padding;

        /// <summary>Paddingの参照</summary>
        public virtual Padding Padding
        {
            get { return padding; }
        }

        /// <summary>Margin</summary>
        readonly Padding margin;

        /// <summary>Marginの参照</summary>
        protected virtual Padding Margin
        {
            get { return margin; }
        }

        /// <summary>Verticalで利用される右余白</summary>
        public virtual float SpacingLeft
        {
            get { return Padding.Left + Margin.Left; }
        }

        /// <summary>Verticalで利用される左余白</summary>
        public virtual float SpacingRight
        {
            get { return Padding.Right + Margin.Right; }
        }

        /// <summary>Horizontalで利用される上余白</summary>
        public virtual float SpacingTop
        {
            get { return Padding.Top + Margin.Top; }
        }

        /// <summary>Horizontalで利用される下余白</summary>
        public virtual float SpacingBottom
        {
            get { return Padding.Bottom + Margin.Bottom; }
        }

#endregion

#region constructor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="cellType">セルの種類</param>
        /// <param name="cellPrefab">Prefab</param>
        /// <param name="size">セルサイズ</param>
        /// <param name="isVariable">サイズ可変フラグ</param>
        /// <param name="expandSpacingFront">開いている時の前方余白</param>
        /// <param name="expandSpacingBack">閉じている時の後方余白</param>
        /// <param name="collapseSpacingFront">閉じている時の前方余白</param>
        /// <param name="collapseSpacingBack">閉じている時の後方余白</param>
        /// <param name="padding">Padding</param>
        /// <param name="margin">Margin</param>
        public AccordionListCellInfo(AccordionListNode node,
            AccordionListCellType cellType,
            AccordionListCellBase cellPrefab,
            float size,
            bool isVariable,
            float expandSpacingFront,
            float expandSpacingBack,
            float collapseSpacingFront,
            float collapseSpacingBack,
            Padding padding,
            Padding margin
        )
        {
            this.index = int.MinValue;
            this.node = node;
            this.cellPrefab = cellPrefab;
            this.cellType = cellType;
            this.size = size;
            this.isVariable = isVariable;
            this.expandSpacingFront = expandSpacingFront;
            this.expandSpacingBack = expandSpacingBack;
            this.collapseSpacingFront = collapseSpacingFront;
            this.collapseSpacingBack = collapseSpacingBack;
            this.padding = padding;
            this.margin = margin;
            this.prefabHash = cellPrefab != null ? cellPrefab.GetHashCode() : 0;
        }
    }

#endregion
}
