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
    /// 横アコーディオンのトランジションハンドラ
    /// </summary>
    public class AccordionListHorizontalTransitionHandler : AccordionListTransitionHandlerBase
    {
#region fields

        /// <summary>クリッピング領域のサイズ</summary>
        protected float clipAreaSize;

        /// <summary>Content領域のサイズ</summary>
        protected float contentSize;

#endregion

#region override methods

        public override void OnStartExpand()
        {
            clipAreaSize = ClipArea.sizeDelta.x;
            contentSize = Content.sizeDelta.x;
        }

        public override void OnUpdateExpand(float value, float rate)
        {
            ApplyExpand(value, rate);
        }

        public override void OnCompleteExpand(float value)
        {
            ApplyExpand(value, 1.0f);
            OnCompleteCallback.Invoke(CellInfo);

            NotClipArea.anchoredPosition = Vector2.zero;
        }

        public override void OnStartCollapse()
        {
            clipAreaSize = ClipArea.sizeDelta.x;
            contentSize = Content.sizeDelta.x;
        }

        public override void OnUpdateCollapse(float value, float rate)
        {
            ApplyCollapse(value, rate);
        }

        public override void OnCompleteCollapse(float value)
        {
            ApplyCollapse(value, 1.0f);

            OnCompleteCollapseCallback.Invoke(CellInfo);

            NotClipArea.anchoredPosition = Vector2.zero;
        }

#endregion

#region protected methods

        /// <summary>
        /// 展開トランジションで変化する値を設定する
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="rate">割合(0~1.0)</param>
        protected virtual void ApplyExpand(float value, float rate)
        {
            ClipArea.sizeDelta = new Vector2(value, ClipArea.sizeDelta.y);
            // クリッピング無しの領域を移動する（クリッピング有り領域に繋げて動かす）
            // 内部のセルは最終座標になっているのでクリッピング領域分移動元をずらしている
            NotClipArea.anchoredPosition = new Vector2(value - clipAreaSize, NotClipArea.anchoredPosition.y);
            // コンテンツの領域を大きくする
            // 差分はclipAreaSize分なのでclipAreaSize分ずらし、時間経過でずらした分を戻している
            // 最終値はcontentSize
            var x = contentSize - clipAreaSize + (clipAreaSize * rate);
            Content.sizeDelta = new Vector2(x, Content.sizeDelta.y);
        }

        /// <summary>
        /// 折り畳みトランジションで変化する値を設定する
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="rate">割合(0~1.0)</param>
        protected virtual void ApplyCollapse(float value, float rate)
        {
            ClipArea.sizeDelta = new Vector2(value, ClipArea.sizeDelta.y);
            // クリッピング無しの領域をクリッピング有りの領域が変化するのと同じく移動する
            NotClipArea.anchoredPosition = new Vector2(value, NotClipArea.anchoredPosition.y);
            // コンテンツの領域を小さくする
            // 差分はclipAreaSize分なのでclipAreaSize分ずらし、時間経過でずらした分を戻している
            // 最終値はcontentSize
            var x = contentSize + clipAreaSize - (clipAreaSize * rate);
            Content.sizeDelta = new Vector2(x, Content.sizeDelta.y);
        }

#endregion
    }
}
