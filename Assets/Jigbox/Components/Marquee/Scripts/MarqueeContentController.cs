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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Jigbox.Components
{
    public class MarqueeContentController
    {
#region fields

        /// <summary>
        /// 子がもつIMarqueeContentを保持しているか
        /// </summary>
        bool isCachedMarqueeContents;

        /// <summary>
        /// マージンの合計値を再計算する必要があるかどうか
        /// </summary>
        bool isCachedTotalMargin;

        /// <summary>
        /// 管理下にあるIMarqueeContentのキャッシュ情報
        /// </summary>
        protected List<IMarqueeContent> cachedMarqueeContents = new List<IMarqueeContent>();

#endregion

#region properties

        /// <summary>
        /// コンテンツ全体のマージンを返します
        /// </summary>
        public virtual int TotalMargin { get; protected set; }

#endregion

#region public methods

        /// <summary>
        /// 各Contentをマージンをふまえたポジションに並べます
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="rectChildren"></param>
        public virtual void SetLayout(RectTransform.Axis axis, List<RectTransform> rectChildren)
        {
#if UNITY_EDITOR || JIGBOX_DEBUG
            // CalculateLayoutInputHorizontalの後に呼ばれるためisCachedがfalseになることは無い
            Assert.IsTrue(isCachedMarqueeContents);
#endif
            // 適用した計算途中のマージンを保持するための変数
            var offsetMargin = 0;
            // 1つ前のContent
            IMarqueeContent frontContent = null;
            for (var i = 0; i < rectChildren.Count; i++)
            {
                var rect = rectChildren[i];
                var backContent = cachedMarqueeContents[i];
                if (backContent == null)
                {
#if UNITY_EDITOR
                    Debug.LogFormat("Can't GetComponent IMarqueeContent {0}", rect.name);
#endif
                    continue;
                }

                // 1つ前のContentと比べさせて適用するマージンを計算する
                var margin = CalculateMargin(frontContent, backContent);
                // axisの設定でずらすポジションの値を求める
                var deltaPosition = CalculatePosition(axis, offsetMargin, margin);
                var newPosition = rect.anchoredPosition + deltaPosition;
                rect.anchoredPosition = newPosition;
                // マージンの合計を足す
                offsetMargin += margin;
                frontContent = backContent;
            }
        }

        /// <summary>
        /// LayoutGroup管理下に置かれているRectTransformから必要な情報をキャッシュします
        /// マージンの合計値もこのタイミングで計算します
        /// </summary>
        /// <param name="rectChildren"></param>
        public virtual void CacheMarqueeContents(List<RectTransform> rectChildren)
        {
            if (isCachedMarqueeContents && isCachedTotalMargin)
            {
                return;
            }

            if (!isCachedMarqueeContents)
            {
                CatchUpMarqueeContents(rectChildren);
            }

            CalculateTotalMargin();

            isCachedMarqueeContents = true;
            isCachedTotalMargin = true;
        }

        /// <summary>
        /// キャッシュのフラグをfalseにします
        /// IMarqueeContentのキャッシュをし直したいときに呼ばれます
        /// </summary>
        public virtual void ClearCache()
        {
            isCachedMarqueeContents = false;
            isCachedTotalMargin = false;
        }

        /// <summary>
        /// マージンの合計値用のキャッシュのフラグをfalseにします
        /// IMarqueeContentのキャッシュはそのままで、マージンの合計値だけを再計算したいときに呼ばれます
        /// </summary>
        public virtual void ClearCachedTotalMargin()
        {
            isCachedTotalMargin = false;
        }

#endregion

#region protected methods

        /// <summary>
        /// マージンを踏まえた位置計算をします
        /// </summary>
        /// <returns></returns>
        protected virtual Vector2 CalculatePosition(RectTransform.Axis axis, float offsetMargin, float frontMargin)
        {
            return axis == RectTransform.Axis.Horizontal ? new Vector2(offsetMargin + frontMargin, 0) : new Vector2(0, -offsetMargin - frontMargin);
        }

        /// <summary>
        /// LayoutGroup管理下に置かれているRectTransformからMarqueeContentの情報をキャッシュします
        /// MarqueeContentがついていないGameObjectにはMarqueeContentをつけます
        /// </summary>
        /// <param name="rectChildren"></param>
        protected virtual void CatchUpMarqueeContents(List<RectTransform> rectChildren)
        {
            cachedMarqueeContents.Clear();
            for (var i = 0; i < rectChildren.Count; i++)
            {
                var rect = rectChildren[i];
                var marqueeContent = rect.GetComponent<IMarqueeContent>();
                if (marqueeContent == null)
                {
#if UNITY_EDITOR
                    if (Application.isPlaying)
                    {
                        Debug.LogErrorFormat("{0} hasn't IMarqueeContent component. Do AddComponent MarqueeContent to {0}.", rect.name);
                    }
                    else
                    {
                        // Editモードの場合でも自動でつけることをログに表示する
                        Debug.LogFormat("Do AddComponent MarqueeContent to {0}.", rect.name);
                    }
#endif
                    marqueeContent = rect.gameObject.AddComponent<MarqueeContent>();
                }

                // マージンの変更があった場合はLayoutの再計算を走らせる必要がある
                marqueeContent.OnMarginChanged = ClearCachedTotalMargin;
                cachedMarqueeContents.Add(marqueeContent);
            }
        }

        /// <summary>
        /// マージンの合計を計算する
        /// </summary>
        protected virtual void CalculateTotalMargin()
        {
            TotalMargin = 0;
            IMarqueeContent frontContent = null;
            foreach (var backContent in cachedMarqueeContents)
            {
                TotalMargin += CalculateMargin(frontContent, backContent);
                frontContent = backContent;
            }

            // 最後のContentのBack分を計算する
            if (frontContent != null)
            {
                TotalMargin += frontContent.MarginBack;
            }
        }

        /// <summary>
        /// ２つのコンテンツの間に適用するマージンを返します
        /// </summary>
        /// <param name="frontContent"></param>
        /// <param name="backContent"></param>
        /// <returns></returns>
        protected virtual int CalculateMargin(IMarqueeContent frontContent, IMarqueeContent backContent)
        {
            var margin1 = 0;
            var margin2 = 0;
            if (frontContent != null)
            {
                margin1 = frontContent.MarginBack;
            }

            if (backContent != null)
            {
                margin2 = backContent.MarginFront;
            }

            // 数値が大きい方のマージンを適用する
            return Mathf.Max(margin1, margin2);
        }

#endregion
    }
}
