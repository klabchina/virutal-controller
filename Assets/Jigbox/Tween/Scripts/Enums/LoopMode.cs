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
    /// Tweenアニメーションがループをするか、その種類を示す列挙型です
    /// </summary>
    public enum LoopMode
    {
        /// <summary>
        /// ループしません
        /// </summary>
        NoLoop = 0,

        /// <summary>
        /// ループの反復時に、初期値から再びアニメーションを開始します
        /// </summary>
        Restart,

        /// <summary>
        /// ループの反復時に、初期値と最終値を入れ替えて、元に戻るようなアニメーションを開始します
        /// </summary>
        PingPong,

        /// <summary>
        /// Yoyo will loop the tween and inverse the easing type when played backward
        /// </summary>
        Yoyo,
    }
}

