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
using System.Collections.ObjectModel;

namespace Jigbox.Components
{
    /// <summary>
    /// アコーディオンリストノード
    /// </summary>
    public abstract class AccordionListNode
    {
#region field & properties

        /// <summary>子ノード</summary>
        List<AccordionListNode> childrenNode;

        /// <summary>子ノードの読み取り用</summary>
        ReadOnlyCollection<AccordionListNode> readOnlyChildrenNode;

        /// <summary>子ノードの読み取り参照</summary>
        public virtual ReadOnlyCollection<AccordionListNode> ChildrenNode
        {
            get
            {
                if (childrenNode == null)
                {
                    childrenNode = new List<AccordionListNode>();
                }

                if (readOnlyChildrenNode == null)
                {
                    readOnlyChildrenNode = childrenNode.AsReadOnly();
                }

                return readOnlyChildrenNode;
            }
        }

        /// <summary>ID</summary>
        int id = int.MinValue;

        /// <summary>IDの参照</summary>
        public virtual int Id
        {
            get { return id; }
        }

        /// <summary>親ノードID</summary>
        int parentId = int.MinValue;

        /// <summary>親ノードIDの参照</summary>
        public virtual int ParentId
        {
            get { return parentId; }
        }

        /// <summary>メインセルPrefab</summary>
        readonly AccordionListMainCell mainCellPrefab;

        /// <summary>メインセルPrefabの参照</summary>
        public virtual AccordionListCellBase MainCellPrefab
        {
            get { return mainCellPrefab; }
        }

        /// <summary>前方向の余白</summary>
        public virtual float SpacingFront
        {
            get { return mainCellPrefab.SpacingFront; }
        }

        /// <summary>後ろ方向の余白</summary>
        public virtual float SpacingBack
        {
            get { return mainCellPrefab.SpacingBack; }
        }

        /// <summary>セルMarginの参照</summary>
        public virtual Padding Margin
        {
            get { return mainCellPrefab.Margin; }
        }

        /// <summary>チャイルドエリアのPaddingの参照</summary>
        public virtual Padding ChildAreaPadding
        {
            get { return mainCellPrefab.ChildAreaPadding; }
        }

        /// <summary>チャイルドエリアセルPrefab</summary>
        readonly AccordionListChildAreaCell childAreaCellPrefab;

        /// <summary>チャイルドエリアセルPrefabの参照</summary>
        public virtual AccordionListChildAreaCell ChildAreaCellPrefab
        {
            get { return childAreaCellPrefab; }
        }

        /// <summary>オプショナルセルPrefab</summary>
        readonly AccordionListOptionalCell optionalCellPrefab;

        /// <summary>オプショナルセルPrefabの参照</summary>
        public virtual AccordionListCellBase OptionalCellPrefab
        {
            get { return optionalCellPrefab; }
        }

        /// <summary>オプショナルセルのMargin参照</summary>
        public virtual Padding OptionalCellMargin
        {
            get { return optionalCellPrefab.Margin; }
        }

        /// <summary>チャイルドエリアセルを持っているか</summary>
        public virtual bool HasChildAreaCell
        {
            get { return ChildAreaCellPrefab != null; }
        }

        /// <summary>オプショナルセルを持っているか</summary>
        public virtual bool HasOptionalCell
        {
            get { return OptionalCellPrefab != null; }
        }

        /// <summary>子要素の数</summary>
        public virtual int ChildCount
        {
            get { return childrenNode != null ? childrenNode.Count : 0; }
        }

        /// <summary>子ノード開閉フラグ</summary>
        bool isExpand;

        /// <summary>子ノード開閉フラグの参照、子がいない場合false</summary>
        public virtual bool IsExpand
        {
            get
            {
                if (ChildCount == 0)
                {
                    return false;
                }

                return isExpand;
            }
            set { isExpand = value; }
        }

#endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mainCellPrefab">メインセルPrefab</param>
        /// <param name="childAreaCellPrefab">チャイルドエリアセルPrefab</param>
        /// <param name="optionalCellPrefab">オプショナルセルPrefab</param>
        /// <param name="isDefaultExpand">初期開閉フラグ</param>
        public AccordionListNode(AccordionListMainCell mainCellPrefab,
            AccordionListChildAreaCell childAreaCellPrefab = null,
            AccordionListOptionalCell optionalCellPrefab = null,
            bool isDefaultExpand = false)
        {
            this.mainCellPrefab = mainCellPrefab;
            this.childAreaCellPrefab = childAreaCellPrefab;
            this.optionalCellPrefab = optionalCellPrefab;
            this.isExpand = isDefaultExpand;
        }

#region public methods

        /// <summary>
        /// ノードを子ノードリストに追加します
        /// </summary>
        /// <param name="child">ノード</param>
        public virtual void AddChild(AccordionListNode child)
        {
            if (childrenNode == null)
            {
                childrenNode = new List<AccordionListNode>();
            }

            childrenNode.Add(child);
        }

        /// <summary>
        /// ノード配列を子ノードリストに追加します
        /// </summary>
        /// <param name="children">子ノードに追加するノード配列</param>
        public virtual void AddChildren(IEnumerable<AccordionListNode> children)
        {
            if (childrenNode == null)
            {
                childrenNode = new List<AccordionListNode>();
            }

            childrenNode.AddRange(children);
        }

        /// <summary>
        /// 子ノードをクリアします
        /// </summary>
        public virtual void ClearChildren()
        {
            if (childrenNode == null)
            {
                return;
            }

            childrenNode.Clear();
        }

        /// <summary>
        /// ノードにIDを設定する
        /// </summary>
        /// <param name="id">ID</param>
        public virtual void SetID(int id)
        {
            this.id = id;
        }

        /// <summary>
        /// 親ノードIDを設定します
        /// </summary>
        /// <param name="parentId">親ノードID</param>
        public virtual void SetParentId(int parentId)
        {
            this.parentId = parentId;
        }

#endregion
    }
}
