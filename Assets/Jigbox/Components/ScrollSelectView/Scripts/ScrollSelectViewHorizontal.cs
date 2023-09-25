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
using UnityEngine.UI;

namespace Jigbox.Components
{
    public class ScrollSelectViewHorizontal : ScrollSelectViewBase
    {
#region constants

        /// <summary>セルのAnchorのminに設定する値</summary>
        protected static readonly Vector2 CellAnchorMin = new Vector2(0, 0);

        /// <summary>セルのAnchorのmaxに設定する値</summary>
        protected static readonly Vector2 CellAnchorMax = new Vector2(0, 1);

#endregion

#region properties

        /// <summary>
        /// モデルクラス
        /// </summary>
        [HideInInspector]
        [SerializeField]
        ScrollSelectModelHorizontal model = new ScrollSelectModelHorizontal();

        /// <summary>
        /// モデルクラスへの参照プロパティ
        /// </summary>
        protected override IScrollSelectLayout Model { get { return model; } }
        
        /// <summary>
        /// コンテナがとるべきアンカーを基準にしたサイズを計算して返します
        /// </summary>
        /// <value>The content fit size delta.</value>
        public override Vector2 ContentFitSizeDelta
        {
            get
            {
                var current = Viewport.rect.size;
                var require = Model.ContentPreferredSize;
                var w = Math.Max(0, require - current.x);
                return new Vector2(w, 0);
            }
        }

        /// <summary>スクロールの速度</summary>
        protected override float Velocity { get { return ScrollRect.velocity.x; } }

        /// <summary>
        /// Scrollbarへ参照/指定します
        /// VerticalかHorizontalかで参照さきのScrollbarが変わります
        /// </summary>
        protected override Scrollbar ScrollBar
        {
            get { return ScrollRect.horizontalScrollbar; }
            set { ScrollRect.horizontalScrollbar = value; }
        }

#endregion

#region public methods

        /// <summary>
        /// セルのポジションを返します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override Vector2 CellPosition(int index)
        {
            var offset = Content.rect.width * Content.pivot.x;
            var x = Model.CellPosition(index);
            x += Model.GetOffsetScrollPosition();
            x += Model.GetOffsetForShiftToCenter();
            x += Model.GetSelectedSpacingOffset(index);
            var y = Padding.Bottom * (1 - CellPivot.y)  - Padding.Top * CellPivot.y;
            return new Vector2(x - offset, y);
        }

#endregion

#region protected methods

        /// <summary>
        /// セルの位置を更新します
        /// </summary>
        /// <param name="cell"></param>
        protected override void UpdateCellTransform(IndexedRectTransform cell)
        {
            var scrollSelectRectTransform = cell as ScrollSelectRectTransform;
            // セルの位置はScrollRectTransformのPositionIndexで決まる
            var position = CellPosition(scrollSelectRectTransform.PositionIndex);
            var sizeDelta = new Vector2(CellSize, -(Padding.Top + Padding.Bottom));
            cell.SetCellTransform(position, sizeDelta, CellPivot, CellAnchorMin, CellAnchorMax);
        }

        /// <summary>
        /// スクロールのenableを設定します
        /// スクロールセレクトビューでは「選択」を行う必要があるため常にtrueになります
        /// </summary>
        protected override void UpdateScrollingEnabled()
        {
            // スクロールできないと選択できないので常にtrueとなる
            ScrollRect.horizontal = true;
        }

        /// <summary>
        /// スクロール位置を補正します。
        /// </summary>
        protected override void CorrectScrollPosition()
        {
            // Horizontalの場合はScrollOriginPointは負の値になるため足し算を行う
            if (Mathf.Abs(ScrollOriginPoint.x + ScrollPosition.x) > Viewport.rect.size.x)
            {
                SlideContent();
            }
        }

        /// <summary>
        /// 選択セルからindexOffset分離れたセルの距離を求めます。
        /// </summary>
        /// <param name="indexOffset"></param>
        /// <returns></returns>
        protected override Vector3 CalculateChangeSelectCellDistance(int indexOffset)
        {
            Vector3 distance = Vector3.zero;
            // Horizontalはindexの増加方向に対してスクロールは逆側に進む必要があるためマイナスをつける
            distance.x = -((CellSize + Spacing) * indexOffset + Model.SelectedCellDelta);
            return distance;
        }

#endregion
    }
}
