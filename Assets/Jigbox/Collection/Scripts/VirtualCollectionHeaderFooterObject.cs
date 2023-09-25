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

using Jigbox.UIControl;
using UnityEngine;

namespace Jigbox.Components
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public abstract class VirtualCollectionHeaderFooterObject : MonoBehaviour
    {
        /// <summary>anchorMinに設定する値</summary>
        protected abstract Vector2 AnchorMin { get; }

        /// <summary>anchorMaxに設定する値</summary>
        protected abstract Vector2 AnchorMax { get; }

        /// <summary>自身のRectTransform</summary>
        protected RectTransform rectTrans;

        /// <summary>親のRectTransform</summary>
        protected RectTransform parentContent;

        /// <summary>自身のRectTransform</summary>
        protected RectTransform RectTransform
        {
            get
            {
                if (rectTrans == null)
                {
                    rectTrans = GetComponent<RectTransform>();
                }
                return rectTrans;
            }
        }

        /// <summary>
        /// offsetの設定を行います
        /// </summary>
        /// <param name="padding"></param>
        protected abstract void UpdateOffset(Padding padding);

        /// <summary>
        /// anchorの更新を行います
        /// </summary>
        protected virtual void UpdateAnchor()
        {
            RectTransform.anchorMin = AnchorMin;
            RectTransform.anchorMax = AnchorMax;
        }

        /// <summary>
        /// paddingを含めたサイズを返します
        /// </summary>
        /// <param name="padding"></param>
        public abstract float GetViewSize(Padding padding);

        /// <summary>
        /// 親の設定をしてポジションの初期化とanchorの設定も行います
        /// </summary>
        /// <param name="parent"></param>
        public virtual void SetParent(RectTransform parent, Padding padding)
        {
            parentContent = parent;
            RectTransform.SetParent(parentContent, false);
            UpdateTransform(padding);
        }

        /// <summary>
        /// Anchorとポジションの更新を行います
        /// </summary>
        public virtual void UpdateTransform(Padding padding)
        {
            UpdateAnchor();
            UpdateOffset(padding);
        }

    }
}
