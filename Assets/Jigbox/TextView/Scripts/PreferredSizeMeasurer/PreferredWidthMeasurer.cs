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

namespace Jigbox.TextView
{
    using PreferredSizeMeasurerProperty = PreferredSizeMeasurer.PreferredSizeMeasurerProperty;

    public class PreferredWidthMeasurer : PreferredBaseMeasurer
    {
#region properties

        /// <summary>必要な幅を求める方法</summary>
        public virtual PreferredWidthType Type { get { return PreferredWidthType.FirstLine; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property">TextViewの値を取得するためのプロパティ</param>
        public PreferredWidthMeasurer(PreferredSizeMeasurerProperty property)
        {
            this.property = property;
            IsCacheInvalid = true;
        }

        /// <summary>
        /// 横幅を計算します。
        /// </summary>
        /// <param name="renderingElements">配置情報まで含めた状態の論理行</param>
        public virtual void CalculateWidth(List<LineRenderingElements> renderingElements)
        {
            IsCacheInvalid = false;
            if (renderingElements == null || renderingElements.Count == 0)
            {
                Value = 0.0f;
                return;
            }

            Value = renderingElements[0].Width;
        }

#endregion
    }
}
