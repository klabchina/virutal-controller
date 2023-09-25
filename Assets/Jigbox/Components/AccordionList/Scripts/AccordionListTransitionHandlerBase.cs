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
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// トランジションハンドラ基底クラス
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AccordionListTransitionHandlerBase : MonoBehaviour
    {
#region fileds & properties

        /// <summary>クリッピング領域</summary>
        RectTransform clipArea;

        /// <summary>クリッピング領域の参照</summary>
        protected virtual RectTransform ClipArea
        {
            get { return clipArea; }
        }

        /// <summary>クリッピングなしの領域</summary>
        RectTransform notClipArea;

        /// <summary>クリッピングなしの領域の参照</summary>
        protected virtual RectTransform NotClipArea
        {
            get { return notClipArea; }
        }

        /// <summary>ScrollRectのContent領域</summary>
        RectTransform content;

        /// <summary>ScrollRectのContent領域</summary>
        protected virtual RectTransform Content
        {
            get { return content; }
        }

        /// <summary>展開完了時のコールバック</summary>
        Action<AccordionListCellInfo> onCompleteExpandCallback;

        /// <summary>展開完了時のコールバック参照</summary>
        protected virtual Action<AccordionListCellInfo> OnCompleteCallback
        {
            get { return onCompleteExpandCallback; }
        }

        /// <summary>折り畳み完了時のコールバック</summary>
        Action<AccordionListCellInfo> onCompleteCollapseCallback;

        /// <summary>折り畳み完了時のコールバックの参照</summary>
        protected virtual Action<AccordionListCellInfo> OnCompleteCollapseCallback
        {
            get { return onCompleteCollapseCallback; }
        }

        /// <summary>完了時コールバックに渡すセル情報</summary>
        AccordionListCellInfo cellInfo;

        /// <summary>完了時コールバックに渡すセル情報の参照</summary>
        protected virtual AccordionListCellInfo CellInfo
        {
            get { return cellInfo; }
        }

#endregion

#region public methods

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="clipArea">クリッピング領域</param>
        /// <param name="notClipArea">クリッピングなし領域</param>
        /// <param name="content">ScrollRectのContent</param>
        /// <param name="onCompleteExpandCallback">展開完了時コールバック</param>
        /// <param name="onCompleteCollapseCallback">折り畳み完了時コールバック</param>
        public virtual void Init(RectTransform clipArea, RectTransform notClipArea, RectTransform content,
            Action<AccordionListCellInfo> onCompleteExpandCallback, Action<AccordionListCellInfo> onCompleteCollapseCallback)
        {
            this.clipArea = clipArea;
            this.notClipArea = notClipArea;
            this.content = content;
            this.onCompleteExpandCallback = onCompleteExpandCallback;
            this.onCompleteCollapseCallback = onCompleteCollapseCallback;
        }

        /// <summary>
        /// 完了コールバック時に渡すcellInfo
        /// </summary>
        /// <param name="cellInfo"></param>
        public virtual void SetCellInfo(AccordionListCellInfo cellInfo)
        {
            this.cellInfo = cellInfo;
        }

#endregion

#region abstract methods

        /// <summary>
        /// 展開トランジション開始時イベント
        /// </summary>
        public abstract void OnStartExpand();

        /// <summary>
        /// 展開トランジション更新時イベント
        /// </summary>
        /// <param name="value">現在の値</param>
        /// <param name="rate">割合(0~1.0)</param>
        public abstract void OnUpdateExpand(float value, float rate);

        /// <summary>
        /// 展開トランジション完了時イベント
        /// </summary>
        /// <param name="value">現在の値</param>
        public abstract void OnCompleteExpand(float value);

        /// <summary>
        /// 折り畳みトランジション開始時イベント
        /// </summary>
        public abstract void OnStartCollapse();

        /// <summary>
        /// 折り畳みトランジション更新時イベント
        /// </summary>
        /// <param name="value">現在値</param>
        /// <param name="rate">割合(0~1.0)</param>
        public abstract void OnUpdateCollapse(float value, float rate);

        /// <summary>
        /// 折り畳みトランジション完了時イベント
        /// </summary>
        /// <param name="value"></param>
        public abstract void OnCompleteCollapse(float value);

#endregion
    }
}
