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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.Components
{
    public class VariableListHorizontal : VariableListBase
    {
#region constants

        static readonly Vector2 cellAnchorMin = new Vector2(0, 0);

        static readonly Vector2 cellAnchorMax = new Vector2(0, 1);

#endregion

#region override properties & methods

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
            get { return Model.TotalSize() + Padding.Left + Padding.Right; }
        }

        protected override float SimplifyScrollPosition
        {
            get { return ScrollPosition.x; }
        }

        protected override Vector2 ContentFitSizeDelta
        {
            get
            {
                var current = Viewport.rect.size;
                var require = ContentPreferredSize;
                var w = Math.Max(0, require - current.x);

                return new Vector2(w, 0);
            }
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
                var parent = Content.parent as RectTransform;
                var x = Content.rect.width * Content.pivot.x - parent.rect.width * parent.pivot.x;
                var y = Content.rect.height * Content.pivot.y - parent.rect.height * parent.pivot.y;
                return new Vector2(x, y);
            }
        }

        protected override float SimplifyCellPosition(int index)
        {
            return CellPosition(index).x;
        }

        protected override void InitializeCellTransform(VariableListCell instance)
        {
            var rectTrans = instance.RectTransform;
            // 初期化のタイミングではpaddingだけ適用できれば良い
            var sizeDelta = new Vector2(rectTrans.sizeDelta.x, -(Padding.Top + Padding.Bottom));

            InitializeCellTransform(rectTrans, sizeDelta);
        }

        protected override void UpdateCellTransform(VariableListCell instance)
        {
            var cellPosition = CellPosition(instance.Index);
            var rectTrans = instance.RectTransform;
            var sizeDelta = new Vector2(Model.Get(instance.Index).Size, -(Padding.Top + Padding.Bottom));

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
                    var cellPosition = new Vector2(-SimplifyViewportSize, 0.0f);
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
            var contentWidth = Content.rect.width;
            var pivot = Content.pivot.x;
            var viewportWidth = Viewport.rect.width;

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

        public override Vector2 CellPosition(int index)
        {
            if (Model.Count() == 0)
            {
                return Vector2.zero;
            }

            var firstCellInfo = Model.Get(0);
            var x = Padding.Left + firstCellInfo.Size * cellPivot.x;
            var spacing = firstCellInfo.SpacingBack;

            for (var i = 1; i <= index; i++)
            {
                var cellInfo = Model.Get(i);
                if (cellInfo.SpacingFront > spacing)
                {
                    spacing = cellInfo.SpacingFront;
                }

                x += Model.Get(i - 1).Size * (1.0f - cellPivot.x) + cellInfo.Size * cellPivot.x + spacing;

                spacing = cellInfo.SpacingBack;
            }

            var y = Padding.Bottom * (1.0f - cellPivot.y) - Padding.Top * cellPivot.y;
            return new Vector2(x, y);
        }

#endregion
    }
}
