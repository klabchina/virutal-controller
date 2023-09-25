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
    public class AccordionListHorizontal : AccordionListBase
    {
#region constants

        static readonly Vector2 cellAnchorMin = new Vector2(0, 0);

        static readonly Vector2 cellAnchorMax = new Vector2(0, 1);

#endregion

#region override properties & methods

        /// <summary>セルの情報リスト</summary>
        readonly AccordionListCellInfoModelHorizontal model = new AccordionListCellInfoModelHorizontal();

        /// <summary>セルの情報リスト参照</summary>
        protected override AccordionListCellInfoModel Model
        {
            get { return model; }
        }

        public override float ContentPositionRate
        {
            get
            {
                // Viewportの中心までのサイズを加味して現在位置を算出する
                var scalar = SimplifyScrollPosition + (Viewport.rect.width * 0.5f);
                var total = Content.rect.width;
                return Mathf.Clamp01(scalar / total);
            }
        }

        public override float ContentPreferredSize
        {
            get
            {
                var cellInfos = Model.CellInfos;
                if (cellInfos.Count == 0)
                {
                    return Padding.Left + Padding.Right;
                }

                return Mathf.Abs(Model.SimplifyCellBackPosition(cellInfos[cellInfos.Count - 1], false)) + Padding.Right;
            }
        }

        protected override float SimplifyScrollPosition
        {
            get { return ScrollPosition.x; }
        }

        protected override Vector2 ContentFitSizeDelta
        {
            get { return new Vector2(ContentPreferredSize, 0); }
        }

        protected override float SimplifyViewportSize
        {
            get { return Viewport.rect.width; }
        }

        protected override float SimplifyContentSize
        {
            get { return Content.rect.width; }
        }

        protected override float PaddingFront
        {
            get { return Padding.Left; }
        }

        protected override Vector2 ScrollOriginPoint
        {
            get
            {
                var parent = (RectTransform) Content.parent;
                var x = Content.rect.width * Content.pivot.x - parent.rect.width * parent.pivot.x;
                var y = Content.rect.height * Content.pivot.y - parent.rect.height * parent.pivot.y;
                return new Vector2(x, y);
            }
        }

        protected override float ExpandClippingAreaSize
        {
            get { return Mathf.Min(ClippingArea.sizeDelta.x, Viewport.rect.width - (ClippingArea.anchoredPosition.x + Content.anchoredPosition.x)); }
        }

        protected override float CollapseClippingAreaSize
        {
            get { return Mathf.Min(ClippingArea.sizeDelta.x, Viewport.rect.width - (ClippingArea.anchoredPosition.x + Content.anchoredPosition.x)); }
        }

        protected override float CollapseSize
        {
            get { return ClippingArea.sizeDelta.x; }
        }

        protected override float SimplifyCellPosition(int index)
        {
            return Model.CellInfos[index].CellPosition.x;
        }

        protected override void UpdateCellTransformInCalculateSize(AccordionListCellBase instance)
        {
            var rectTrans = instance.RectTransform;
            var cellInfo = Model.FindCellInfo(instance.NodeId, instance.CellType);
            var sizeDelta = new Vector2(rectTrans.sizeDelta.x, -(Padding.Top + Padding.Bottom + cellInfo.SpacingTop + cellInfo.SpacingBottom));

            SetCellTransform(rectTrans, sizeDelta);
        }

        protected override void UpdateCellTransform(AccordionListCellBase instance, AccordionListCellInfo cellInfo)
        {
            var cellPosition = cellInfo.CellPosition;
            var sizeDelta = new Vector2(cellInfo.Size, -(Padding.Top + Padding.Bottom + cellInfo.SpacingTop + cellInfo.SpacingBottom));
            var rectTrans = instance.RectTransform;

            SetCellTransform(rectTrans, cellPosition, sizeDelta);
        }

        protected override void RelegateToOutOfContent(AccordionListCellBase instance, float viewportSize)
        {
            // Viewport領域外(Viewportのサイズ分だけ外側)へ設定する
            var cellPosition = new Vector2(-viewportSize, 0);
            var sizeDelta = new Vector2(0.0f, 0.0f);
            SetCellTransform(instance.RectTransform, cellPosition, sizeDelta);
        }

        protected override void SetCellTransform(RectTransform rectTrans, Vector2 sizeDelta)
        {
            rectTrans.anchorMin = cellAnchorMin;
            rectTrans.anchorMax = cellAnchorMax;

            base.SetCellTransform(rectTrans, sizeDelta);
        }

        protected override void SetCellTransform(RectTransform rectTrans, Vector2 pos, Vector2 sizeDelta)
        {
            rectTrans.anchorMin = cellAnchorMin;
            rectTrans.anchorMax = cellAnchorMax;

            base.SetCellTransform(rectTrans, pos, sizeDelta);
        }

        protected override Vector3 CalculateJumpPositionByRate(float rate, float offsetSize = 0.0f)
        {
            var pivot = Content.pivot.x;
            var viewportWidth = Viewport.rect.width;
            var contentWidth = Mathf.Max(viewportWidth, Content.rect.width);

            var x = -(contentWidth * Mathf.Clamp01(rate) - viewportWidth * 0.5f) + offsetSize;
            var min = -(contentWidth * (1.0f - pivot) - viewportWidth * (1.0f - pivot));
            var max = -(contentWidth * pivot - viewportWidth * pivot);

            // セルの先頭、末尾あたりに飛んだ際にセルが無いところを表示しながらスプリングがかかってしまう
            // セルがないところを表示するスプリングをさけるためmin、maxにかける
            var position = new Vector2(Mathf.Clamp(x, min, max), ScrollOriginPoint.y);
            return position;
        }

        protected override void UpdateScrollingEnabled()
        {
            ScrollRect.horizontal = IsValidScroll;
        }

        protected override void UpdateContentAnchoredPosition(float scrollPosition)
        {
            Content.anchoredPosition = new Vector2(-scrollPosition, 0);
        }

        protected override float UpdateScrollPositionByCollapse(Vector2 firstPosition, Vector2 lastPosition, float simplifyScrollPosition)
        {
            Rect collapseRect = new Rect(firstPosition, lastPosition - firstPosition);

            // 閉じる領域が元のスクロール位置より前だった場合はずらす
            if (collapseRect.x < simplifyScrollPosition)
            {
                return Mathf.Max(0, simplifyScrollPosition - collapseRect.width);
            }

            return simplifyScrollPosition;
        }

#endregion
    }
}
