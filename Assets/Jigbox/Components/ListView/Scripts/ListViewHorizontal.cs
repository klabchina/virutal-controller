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
using UnityEngine.UI;
using System;

namespace Jigbox.Components
{
    public class ListViewHorizontal : ListViewBase
    {
        static readonly Vector2 cellAnchorMin = new Vector2(0, 0);

        static readonly Vector2 cellAnchorMax = new Vector2(0, 1);

        [SerializeField]
        [HideInInspector]
        ListModelHorizontal model = new ListModelHorizontal();

        protected override IListLayout Model { get { return model; } }

        protected override Vector2 HeaderFooterSize
        {
            get
            {
                if (headerFooter == null)
                {
                    return Vector2.zero;
                }
                return new Vector2(headerFooter.Size, 0f);
            }
        }

        protected override Vector2 HeaderSize
        {
            get
            {
                if (headerFooter == null)
                {
                    return Vector2.zero;
                }
                return new Vector2(headerFooter.HeaderSize, 0f);
            }
        }

        /// <summary>
        /// 現在のスクロール位置を正規化された割合(0.0f 〜 1.0f)で計算します
        /// </summary>
        /// <value>The scroll rate.</value>
        public override float ScrollRate
        {
            get
            {
                var scalar = Content.localPosition.x * (Content.pivot.x - 1.0f);
                var total = Content.rect.width;
                return Mathf.Clamp01(scalar / total);
            }
        }

        /// <summary>
        /// Viewport中央を基準とした、Contentの現在のスクロール位置を割合で返します
        /// </summary>
        public override float ContentPositionRate
        {
            get
            {
                // Viewportの中心までのサイズを加味して現在位置を算出する
                var scalar = -Content.localPosition.x + (Viewport.rect.width * 0.5f);
                var total = Content.rect.width;
                return Mathf.Clamp01(scalar / total);
            }
        }

        /// <summary>
        /// スクロールの開始点の座標を返します
        /// </summary>
        /// <value>The scroll origin point.</value>
        public override Vector2 ScrollOriginPoint
        {
            get
            {
                var parent = Content.parent as RectTransform;
                var x = Content.rect.width * Content.pivot.x - parent.rect.width * parent.pivot.x;
                var y = Content.rect.height * Content.pivot.y - parent.rect.height * parent.pivot.y;
                return new Vector2(x, y);
            }
        }

        /// <summary>
        /// 表示させたい、仮想のセルの総数を参照/指定します
        /// </summary>
        /// <value>The virtual cell count.</value>
        public override int VirtualCellCount
        {
            get { return base.VirtualCellCount; }
            set
            {
                base.VirtualCellCount = value;
            }
        }

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
                if (headerFooter != null)
                {
                    require += headerFooter.Size;
                }
                var w = Math.Max(0, require - current.x);
                return new Vector2(w, 0);
            }
        }

        /// <summary>
        /// 指定されたインデックスに応じた正規化された割合(0.0f 〜 1.0f)を返します
        /// </summary>
        /// <returns>The by index.</returns>
        /// <param name="index">Index.</param>
        public override float RateByIndex(int index)
        {
            var scalar = Model.CellPosition(index);
            if (headerFooter != null)
            {
                // scalarはセルだけで計算された位置のため、ヘッダ含めた位置になるよう加算する
                scalar += headerFooter.HeaderSize;
            }
            return Mathf.Clamp01(scalar / Content.rect.width);
        }

        /// <summary>
        /// 親コンテナのアンカーからの相対位置座標を元に、セルの全体からのインデックスを計算します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="position">Position.</param>
        public override int CellIndex(Vector2 position)
        {
            var offset = Content.rect.width * Content.pivot.x;
            return Model.CellIndex(position.x + offset);
        }

        /// <summary>
        /// セルのインデックスから、親コンテナのアンカーからの、セルの相対位置座標を計算します
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="index">Index.</param>
        public override Vector2 CellPosition(int index)
        {
            var offset = Content.rect.width * Content.pivot.x;
            var x = Model.CellPosition(index);
            if (headerFooter != null)
            {
                x += headerFooter.HeaderSize;
            }
            var y = Padding.Bottom * (1 - CellPivot.y)  - Padding.Top * CellPivot.y;
            return new Vector2(x - offset, y);
        }

        protected override void UpdateCellTransform(IndexedRectTransform cell)
        {
            var position = CellPosition(cell.Index);
            var sizeDelta = new Vector2(CellSize, -(Padding.Top + Padding.Bottom));
            cell.SetCellTransform(position, sizeDelta, CellPivot, cellAnchorMin, cellAnchorMax);
        }

        /// <summary>
        /// 指定された正規化された割合(0.0f 〜 1.0f)に応じたスクロール位置をスライドします
        /// ヘッダ・フッタを除いたセルの範囲だけに適用したrate値でスライドしたいときに使用します
        /// </summary>
        /// <param name="rate">Rate.</param>
        public override void JumpByRate(float rate)
        {
            if (headerFooter != null)
            {
                // セルの範囲だけに対して適用されているrate値のため、ヘッダフッタを含めたrate値にする
                // ヘッダ・フッタを含めたcellのポジションを割り出す
                var cellPosition = (Content.rect.width - headerFooter.Size) * rate + headerFooter.HeaderSize;
                rate = cellPosition / Content.rect.width;
            }
            JumpByRate(rate, false);
        }

        /// <summary>
        /// 指定された正規化された割合(0.0f 〜 1.0f)に応じたスクロール位置をスライドします
        /// 表示する範囲にヘッダ・フッタを含めるかどうかを指定できます
        /// </summary>
        /// <param name="rate">Rate.</param>
        /// <param name="withHeaderFooter">trueの場合表示する範囲を範囲をヘッダ・フッタ含めます。falseの場合はセルだけを表示するようにします</param>
        protected override void JumpByRate(float rate, bool withHeaderFooter)
        {
            if (Content.rect.width <= Viewport.rect.width)
            {
                // 表示内容がviewportに収まっている状態なのでスクロール不要
                return;
            }
            ScrollRect.StopMovement();

            Content.localPosition = CalculateJumpPositionByRate(rate, withHeaderFooter);
        }

        /// <summary>
        /// 指定された正規化された割合(0.0f ~ 1.0f)に応じたスクロール位置を返します
        /// 表示する範囲にヘッダ・フッタを含めるかどうかを指定できます
        /// </summary>
        /// <param name="rate">Rate.</param>
        /// <param name="withHeaderFooter">trueの場合表示する範囲を範囲をヘッダ・フッタ含めます。falseの場合はセルだけを表示するようにします</param>
        /// <returns></returns>
        protected virtual Vector3 CalculateJumpPositionByRate(float rate, bool withHeaderFooter)
        {
            var contentWidth = Content.rect.width;
            var pivot = Content.pivot.x;
            var viewportWidth = Viewport.rect.width;

            var x = -(contentWidth * Mathf.Clamp01(rate) - viewportWidth * 0.5f);
            var min = -(contentWidth * (1.0f - pivot) - viewportWidth * (1.0f - pivot));
            var max = -(contentWidth * pivot - viewportWidth * pivot);

            if (headerFooter != null && !withHeaderFooter)
            {
                // ヘッダ・フッタを除いた値にさせてセルだけが表示されるようにする
                min += headerFooter.FooterSize;
                max -= headerFooter.HeaderSize;
            }

            // セルの先頭、末尾あたりに飛んだ際にセルが無いところを表示しながらスプリングがかかってしまう
            // セルがないところを表示するスプリングをさけるためmin、maxにかける
            var position = new Vector2(Mathf.Clamp(x, min, max), ScrollOriginPoint.y);
            return position;
        }

        /// <summary>
        /// スクロールできるかどうかの設定を更新します
        /// </summary>
        protected override void UpdateScrollingEnabled()
        {
            ScrollRect.horizontal = IsValidScroll;
        }
    }
}
