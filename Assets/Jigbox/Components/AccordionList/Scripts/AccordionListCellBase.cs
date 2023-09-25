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
    /// アコーディオンリストセルの基底クラス
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AccordionListCellBase : MonoBehaviour
    {
#region fields & properties

        /// <summary>ノードID</summary>
        int nodeId = int.MinValue;

        /// <summary>ノードIDへの参照</summary>
        public virtual int NodeId
        {
            get { return nodeId; }
            set { nodeId = value; }
        }

        /// <summary>セルのRectTransform</summary>
        RectTransform rectTransform;

        /// <summary>セルのRectTransformへの参照</summary>
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }

                return rectTransform;
            }
        }

        /// <summary>サイズが可変するかどうかの参照</summary>
        public virtual bool IsVariable
        {
            get { return false; }
        }

        /// <summary>セルのサイズへの参照</summary>
        public virtual float CellSize { get; set; }

        /// <summary>セルの前方間隔への参照</summary>
        public virtual float SpacingFront { get; set; }

        /// <summary>セルの後方間隔への参照</summary>
        public virtual float SpacingBack { get; set; }

        /// <summary>セルの種類</summary>
        public abstract AccordionListCellType CellType { get; }

        /// <summary>AccordionListインターフェース</summary>
        protected virtual IAccordionList AccordionList { get; private set; }

#endregion


#region public methods

        /// <summary>
        /// AccordionListのインターフェースを設定します
        /// </summary>
        /// <param name="accordionList">IAccordionListインターフェース</param>
        public void SetAccordionList(IAccordionList accordionList)
        {
            AccordionList = accordionList;
        }

        /// <summary>
        /// セルの生成方法
        /// セルの生成方法を変える場合はoverrideすること
        /// </summary>
        public virtual AccordionListCellBase Generate(Transform parent)
        {
            return Instantiate(this, parent, false);
        }

        /// <summary>
        /// セル更新用のイベント
        /// </summary>
        /// <param name="accordionList">アコーディオンリスト</param>
        /// <param name="node">セルに紐づくノード</param>
        public abstract void OnUpdateCell(AccordionListBase accordionList, AccordionListNode node);

        /// <summary>
        /// セルサイズ変更用のイベント
        /// </summary>
        /// <param name="accordionList">アコーディオンリスト</param>
        /// <param name="node">セルに紐づくノード</param>
        /// <returns>表示するセルサイズ</returns>
        public abstract float OnUpdateCellSize(AccordionListBase accordionList, AccordionListNode node);

        /// <summary>
        /// ノードの開く開始時イベント
        /// </summary>
        /// <param name="accordionList">アコーディオンリスト</param>
        /// <param name="node">開くノード</param>
        public virtual void OnStartExpand(AccordionListBase accordionList, AccordionListNode node)
        {
        }

        /// <summary>
        /// ノードの開く終了時イベント
        /// </summary>
        /// <param name="accordionList">アコーディオンリスト</param>
        /// <param name="node">開くノード</param>
        public virtual void OnCompleteExpand(AccordionListBase accordionList, AccordionListNode node)
        {
        }

        /// <summary>
        /// ノードの閉じる開始時イベント
        /// </summary>
        /// <param name="accordionList">アコーディオンリスト</param>
        /// <param name="node">開くノード</param>
        public virtual void OnStartCollapse(AccordionListBase accordionList, AccordionListNode node)
        {
        }

        /// <summary>
        /// ノードの閉じる終了時イベント
        /// </summary>
        /// <param name="accordionList">アコーディオンリスト</param>
        /// <param name="node">開くノード</param>
        public virtual void OnCompleteCollapse(AccordionListBase accordionList, AccordionListNode node)
        {
        }

#endregion

#region abstract properties

        /// <summary>Marginの参照</summary>
        public abstract Padding Margin { get; set; }

#endregion
    }
}
