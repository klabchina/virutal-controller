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
    public class MarqueeViewHorizontal : HorizontalLayoutGroup, IMarqueeView
    {
#region fields

        /// <summary>
        /// 子に置かれるIMarqueeContentを扱うクラス
        /// </summary>
        MarqueeContentController contentController;

#endregion

#region properties

        /// <summary>
        /// contentControllerのプロパティ
        /// </summary>
        protected virtual MarqueeContentController ContentController
        {
            get
            {
                if (contentController == null)
                {
                    contentController = new MarqueeContentController();
                }

                return contentController;
            }
        }

        /// <summary>
        /// レイアウトの計算が終わったタイミングで呼ばれるコールバック
        /// </summary>
        public Action CompleteLayoutCallback { get; set; }

        /// <summary>
        /// Content全体の長さを返す
        /// </summary>
        public virtual float Length { get { return rectTransform.rect.size.x; } }

        /// <summary>
        /// Containerへの参照
        /// </summary>
        public virtual RectTransform Container { get { return rectTransform; } }

        /// <summary>
        /// minWidthをマージン含めた値にする
        /// ContentSizeFitterにマージン分を含めた値で認識させるため
        /// </summary>
        public override float minWidth { get { return base.minWidth + ContentController.TotalMargin; } }

        /// <summary>
        /// preferredWidthをマージン含めた値にする
        /// ContentSizeFitterにマージン分を含めた値で認識させるため
        /// </summary>
        public override float preferredWidth { get { return base.preferredWidth + ContentController.TotalMargin; } }

        /// <summary>
        /// flexibleWidthをマージン含めた値にする
        /// ContentSizeFitterにマージン分を含めた値で認識させるため
        /// </summary>
        public override float flexibleWidth { get { return base.flexibleWidth + ContentController.TotalMargin; } }

#endregion

#region public methods

        public virtual void MarkLayoutForRebuild()
        {
            SetDirty();
        }

        /// <summary>
        /// 引数で渡されたGameObjectを子として置く
        /// </summary>
        /// <param name="content"></param>
        public virtual void AddContent(GameObject content)
        {
            content.transform.SetParent(transform, false);
        }

        /// <summary>
        /// LayoutGroupのLayoutシステムで一番始めに呼ばれるメソッド
        /// IMarqueeContentの情報のキャッシュを行わせる
        /// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            // CalculateLayoutInputHorizontalでrectChildrenの計算が終わった後にIMarqueeContentのキャッシュを行う
            ContentController.CacheMarqueeContents(rectChildren);
        }

        /// <summary>
        /// LayoutGroupの自動レイアウト機能
        /// LayoutGroupによって並べられたContentを、それぞれがマージンを踏まえた位置になるよう調整する
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            base.SetLayoutHorizontal();
            ContentController.SetLayout(RectTransform.Axis.Horizontal, rectChildren);
            if (CompleteLayoutCallback != null)
            {
                 CompleteLayoutCallback();
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// 子に変更が入った場合に検知する
        /// </summary>
        protected override void OnTransformChildrenChanged()
        {
            base.OnTransformChildrenChanged();
            // IMarqueeContentのキャッシュを再計算させる
            ContentController.ClearCache();
        }

#endregion
    }
}
