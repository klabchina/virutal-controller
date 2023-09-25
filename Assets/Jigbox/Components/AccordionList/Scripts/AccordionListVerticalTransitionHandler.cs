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
    /// 縦アコーディオンのトランジションハンドラ
    /// </summary>
    public class AccordionListVerticalTransitionHandler : AccordionListTransitionHandlerBase
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
            clipAreaSize = ClipArea.sizeDelta.y;
            contentSize = Content.sizeDelta.y;
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
            clipAreaSize = ClipArea.sizeDelta.y;
            contentSize = Content.sizeDelta.y;
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
            ClipArea.sizeDelta = new Vector2(ClipArea.sizeDelta.x, value);
            // クリッピング無し領域を移動する
            // 中のセルは移動後の位置にあるので、増えた領域分ずらした位置から開始する
            NotClipArea.anchoredPosition = new Vector2(NotClipArea.anchoredPosition.x, -value + clipAreaSize);
            // コンテンツの領域を大きくする
            // 差分はclipAreaSize分なのでclipAreaSize分ずらし、時間経過でずらした分を戻している
            // 最終値はcontentSize
            var y = contentSize - clipAreaSize + (clipAreaSize * rate);
            Content.sizeDelta = new Vector2(Content.sizeDelta.x, y);
        }

        /// <summary>
        /// 折り畳みトランジションで変化する値を設定する
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="rate">割合(0~1.0)</param>
        protected virtual void ApplyCollapse(float value, float rate)
        {
            ClipArea.sizeDelta = new Vector2(ClipArea.sizeDelta.x, value);
            // クリッピング無し領域を移動する
            // 中のセルは移動後の位置にあるので、増えた領域分ずらした位置から開始する
            NotClipArea.anchoredPosition = new Vector2(NotClipArea.anchoredPosition.x, -value);
            // コンテンツの領域を小さくする
            // 差分はclipAreaSize分なのでclipAreaSize分ずらし、時間経過でずらした分を戻している
            // 最終値はcontentSize
            var y = contentSize + clipAreaSize - (clipAreaSize * rate);
            Content.sizeDelta = new Vector2(Content.sizeDelta.x, y);
        }

#endregion
    }
}
