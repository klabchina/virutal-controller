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
    /// <summary>
    /// ListView、TileViewのVertical用のフッタにつけるコンポーネントです
    /// </summary>
    public class VirtualCollectionFooterVertical : VirtualCollectionFooter
    {
        /// <summary>anchorMinに設定する値</summary>
        protected override Vector2 AnchorMin { get { return new Vector2(0, 0); } }

        /// <summary>anchorMaxに設定する値</summary>
        protected override Vector2 AnchorMax { get { return new Vector2(1, 0); }}

        /// <summary>
        /// offsetの設定を行います
        /// </summary>
        /// <param name="padding"></param>
        protected override void UpdateOffset(Padding padding)
        {
            var size = RectTransform.rect.size.y;
            RectTransform.offsetMin = new Vector2(padding.Left, padding.Bottom);
            RectTransform.offsetMax = new Vector2(-padding.Right, padding.Bottom + size);
        }

        /// <summary>
        /// paddingを含めたサイズを返します
        /// </summary>
        /// <param name="padding"></param>
        public override float GetViewSize(Padding padding)
        {
            return RectTransform.rect.size.y + padding.Top + padding.Bottom;
        }
    }
}
