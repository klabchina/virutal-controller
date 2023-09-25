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

namespace Jigbox.Components
{
    public class TileViewVertical : TileViewBase
    {
        [SerializeField]
        [HideInInspector]
        TileModelVertical tile = new TileModelVertical();

        protected override ITilingLayout Model { get { return tile; } }

        protected override Vector2 HeaderFooterSize
        {
            get
            {
                if (headerFooter == null)
                {
                    return Vector2.zero;
                }
                return new Vector2(0f, headerFooter.Size);
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
                return new Vector2(0f, headerFooter.HeaderSize);
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
                var scalar = Content.localPosition.y * Content.pivot.y;
                var total = Content.rect.height;
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
                var scalar = Content.localPosition.y + (Viewport.rect.height * 0.5f);
                var total = Content.rect.height;
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
                var y = Content.rect.height * (1.0f - Content.pivot.y) - parent.rect.height * (1.0f - parent.pivot.y);
                return new Vector2(x, -y);
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
                var sizeDelta = base.ContentFitSizeDelta;
                sizeDelta.x = 0;
                return sizeDelta;
            }
        }

        /// <summary>
        /// 指定されたインデックスに応じた正規化された割合(0.0f 〜 1.0f)を返します
        /// </summary>
        /// <returns>The by index.</returns>
        /// <param name="index">Index.</param>
        public override float RateByIndex(int index)
        {
            var y = Model.CellPosition(index).y;
            if (headerFooter != null)
            {
                // yはセルだけで計算された位置のため、ヘッダ含めた位置になるよう加算する
                y += headerFooter.HeaderSize;
            }
            return y / Content.rect.height;
        }

        protected override ScrollRect GetScrollRect()
        {
            var sr = base.GetScrollRect();
            sr.vertical = true;
            return sr;
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
                // セルの範囲だけに対して適用されているrateをヘッダフッタを含めたrate値にする
                // ヘッダ・フッタを含めたcellのポジションを割り出す
                var cellPosition = (Content.rect.height - headerFooter.Size) * rate + headerFooter.HeaderSize;
                rate = cellPosition / Content.rect.height;
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
            if (Content.rect.height <= Viewport.rect.height)
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
            var contentHeight = Content.rect.height;
            var pivot = Content.pivot.y;
            var viewportHeight = Viewport.rect.height;

            var y = contentHeight * Mathf.Clamp01(rate) - viewportHeight * 0.5f;
            var min = contentHeight * (1.0f - pivot) - viewportHeight * (1.0f - pivot);
            var max = contentHeight * pivot - viewportHeight * pivot;
            if (headerFooter != null && !withHeaderFooter)
            {
                // ヘッダ・フッタを除いた値にさせてセルだけが表示されるようにする
                min += headerFooter.HeaderSize;
                max -= headerFooter.FooterSize;
            }

            // セルの先頭、末尾あたりに飛んだ際にセルが無いところを表示しながらスプリングがかかってしまう
            // セルがないところを表示するスプリングをさけるためmin、maxにかける
            var position = new Vector2(ScrollOriginPoint.x, Mathf.Clamp(y, min, max));
            return position;
        }

        /// <summary>
        /// スクロールできるかどうかの設定を更新します
        /// </summary>
        protected override void UpdateScrollingEnabled()
        {
            ScrollRect.vertical = IsValidScroll;
        }
    }
}
