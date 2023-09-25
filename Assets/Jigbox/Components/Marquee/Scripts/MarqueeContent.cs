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
    [DisallowMultipleComponent]
    public class MarqueeContent : MonoBehaviour, IMarqueeContent
    {
#region fields

        /// <summary>
        /// 前方のマージン
        /// </summary>
        [SerializeField]
        int marginFront;

        /// <summary>
        /// 後方のマージン
        /// </summary>
        [SerializeField]
        int marginBack;

        protected RectTransform rectTransform;

#endregion

#region properties

        protected virtual RectTransform RectTrans
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = transform as RectTransform;
                }

                return rectTransform;
            }
        }

        /// <summary>
        /// マーキーの値が変更されたときに呼ばれます
        /// </summary>
        public Action OnMarginChanged { get; set; }

        /// <summary>
        /// 前方のマージン
        /// </summary>
        public int MarginFront
        {
            get { return marginFront; }
            set
            {
                if (marginFront == value)
                {
                    return;
                }

                marginFront = value;
                NotifyMarginChanged();
                // Dirtyにして親のLayoutGroupにLayoutの再計算をさせる
                LayoutRebuilder.MarkLayoutForRebuild(RectTrans);
            }
        }

        /// <summary>
        /// 後方のマージン
        /// </summary>
        public int MarginBack
        {
            get { return marginBack; }
            set
            {
                if (marginBack == value)
                {
                    return;
                }

                marginBack = value;
                NotifyMarginChanged();
                // Dirtyにして親のLayoutGroupにLayoutの再計算をさせる
                LayoutRebuilder.MarkLayoutForRebuild(RectTrans);
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// マージンが変わったことを通知します
        /// </summary>
        protected virtual void NotifyMarginChanged()
        {
            if (OnMarginChanged != null)
            {
                OnMarginChanged();
            }
        }

#endregion
    }
}
