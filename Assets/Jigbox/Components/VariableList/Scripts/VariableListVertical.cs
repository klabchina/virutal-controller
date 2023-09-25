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
    public class VariableListVertical : VariableListBase
    {
#region constants

        static readonly Vector2 cellAnchorMin = new Vector2(0, 1);

        static readonly Vector2 cellAnchorMax = new Vector2(1, 1);

#endregion

#region override properties & methods

        public override float ContentPositionRate
        {
            get
            {
                // Viewportの中心までのサイズを加味して現在位置を算出する
                var scalar = SimplifyScrollPosition + (Viewport.rect.height * 0.5f);
                var total = Content.rect.height;
                return Mathf.Clamp01(scalar / total);
            }
        }

        public override float ContentPreferredSize
        {
            get { return Model.TotalSize() + Padding.Top + Padding.Bottom; }
        }

        protected override float SimplifyScrollPosition
        {
            get { return ScrollPosition.y; }
        }

        protected override Vector2 ContentFitSizeDelta
        {
            get
            {
                Vector2 current = Viewport.rect.size;
                float require = ContentPreferredSize;

                return new Vector2(0, Mathf.Clamp(require - current.y, 0, require - current.y));
            }
        }

        protected override float SimplifyViewportSize
        {
            get { return Viewport.rect.height; }
        }

        protected override float SimplifyContentSize
        {
            get { return Content.rect.height; }
        }

        protected override float PaddingFront
        {
            get { return Padding.Top; }
        }

        protected override Vector2 ScrollOriginPoint
        {
            get
            {
                var parent = Content.parent as RectTransform;
                var x = Content.rect.width * Content.pivot.x - parent.rect.width * parent.pivot.x;
                var y = Content.rect.height * (1.0f - Content.pivot.y) - parent.rect.height * (1.0f - parent.pivot.y);
                return new Vector2(x, -y);
            }
        }

        protected override float SimplifyCellPosition(int index)
        {
            return -CellPosition(index).y;
        }

        protected override void InitializeCellTransform(VariableListCell instance)
        {
            var rectTrans = instance.RectTransform;
            // 初期化のタイミングではpaddingだけ適用できれば良い
            var sizeDelta = new Vector2(-(Padding.Left + Padding.Right), rectTrans.sizeDelta.y);

            InitializeCellTransform(rectTrans, sizeDelta);
        }

        protected override void UpdateCellTransform(VariableListCell instance)
        {
            var cellPosition = CellPosition(instance.Index);
            var rectTrans = instance.RectTransform;
            var sizeDelta = new Vector2(-(Padding.Left + Padding.Right), Model.Get(instance.Index).Size);

            SetCellTransform(rectTrans, cellPosition, sizeDelta);
        }

        protected override void RelegateToOutOfContent()
        {
            // GC回避のためにPairで抜き出す
            foreach (var poolPair in CellPools)
            {
                var pool = poolPair.Value;
                var instances = pool.Free;

                foreach (var instance in instances)
                {
                    // Viewport領域外(Viewportのサイズ分だけ外側)へ設定する
                    var cellPosition = new Vector2(0, SimplifyViewportSize);
                    var sizeDelta = new Vector2(0.0f, 0.0f);
                    SetCellTransform(instance.RectTransform, cellPosition, sizeDelta);
                }
            }
        }

        protected override void InitializeCellTransform(RectTransform rectTrans, Vector2 sizeDelta)
        {
            rectTrans.anchorMin = cellAnchorMin;
            rectTrans.anchorMax = cellAnchorMax;
            base.InitializeCellTransform(rectTrans, sizeDelta);
        }

        protected override Vector3 CalculateJumpPositionByRate(float rate, float offsetSize = 0.0f)
        {
            var contentHeight = Content.rect.height;
            var pivot = Content.pivot.y;
            var viewportHeight = Viewport.rect.height;

            var y = contentHeight * Mathf.Clamp01(rate) - viewportHeight * 0.5f - offsetSize;
            var min = contentHeight * (1.0f - pivot) - viewportHeight * (1.0f - pivot);
            var max = contentHeight * pivot - viewportHeight * pivot;

            // セルの先頭、末尾あたりに飛んだ際にセルが無いところを表示しながらスプリングがかかってしまう
            // セルがないところを表示するスプリングをさけるためmin、maxにかける
            var position = new Vector2(ScrollOriginPoint.x, Mathf.Clamp(y, min, max));
            return position;
        }

        protected override void UpdateScrollingEnabled()
        {
            ScrollRect.vertical = IsValidScroll;
        }

        public override Vector2 CellPosition(int index)
        {
            if (Model.Count() == 0)
            {
                return Vector2.zero;
            }

            var y = Padding.Top + Model.Get(0).Size * (1.0f - cellPivot.y);
            var spacing = Model.Get(0).SpacingBack;

            for (var i = 1; i <= index; i++)
            {
                var cellInfo = Model.Get(i);

                if (cellInfo.SpacingFront > spacing)
                {
                    spacing = cellInfo.SpacingFront;
                }

                y += Model.Get(i - 1).Size * (1.0f - cellPivot.y) + cellInfo.Size * cellPivot.y + spacing;

                spacing = cellInfo.SpacingBack;
            }

            var x = Padding.Left * (1.0f - cellPivot.x) - Padding.Right * cellPivot.x;
            return new Vector2(x, -y);
        }

#endregion
    }
}
