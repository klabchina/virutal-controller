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


namespace Jigbox.Tween
{
    /// <summary>
    /// イージング、緩急のつくタイミングを示す列挙型です
    /// </summary>
    public enum EasingType
    {
        /// <summary>
        /// アニメーションが進むに合わせて加速するような緩急を示します
        /// </summary>
        EaseIn,

        /// <summary>
        /// アニメーションが進むに合わせて減速するような緩急を示します
        /// </summary>
        EaseOut,

        /// <summary>
        /// アニメーションの前半は加速し、後半は減速するような緩急を示します
        /// </summary>
        EaseInOut,

        /// <summary>
        /// ユーザー定義のイージング関数を元に緩急をつけます
        /// </summary>
        Custom,
    }
}
