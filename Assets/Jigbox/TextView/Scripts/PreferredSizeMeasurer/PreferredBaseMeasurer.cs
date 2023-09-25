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

namespace Jigbox.TextView
{
    using PreferredSizeMeasurerProperty = PreferredSizeMeasurer.PreferredSizeMeasurerProperty;

    public abstract class PreferredBaseMeasurer
    {
#region properties

        /// <summary>TextViewの値を取得するためのプロパティ</summary>
        protected PreferredSizeMeasurerProperty property;

        /// <summary>表示に必要な幅</summary>
        public float Value { get; protected set; }

        /// <summary>キャッシュが有効化どうか</summary>
        public bool IsCacheInvalid { get; set; }

        /// <summary>値がキャッシュされているかどうか</summary>
        public virtual bool HasCache { get { return !IsCacheInvalid; } }

#endregion
    }
}
