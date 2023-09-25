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
    /// Tweenアニメーションの状態を示す列挙型です
    /// </summary>
    public enum TweenState : int
    {
        /// <summary>状態なし</summary>
        None = 0,
        /// <summary>動作開始前</summary>
        Idle = 1,
        /// <summary>動作開始</summary>
        Start = 1 << 2,
        /// <summary>更新中</summary>
        Working = 1 << 3,
        /// <summary>再開</summary>
        Resume = 1 << 4,
        /// <summary>一時停止</summary>
        Paused = 1 << 5,
        /// <summary>強制停止</summary>
        Kill = 1 << 6,
        /// <summary>完了</summary>
        Complete = 1 << 7,
        /// <summary>強制完了</summary>
        ForceComplete = 1 << 8,
        /// <summary>動作終了</summary>
        Done = 1 << 9,
        /// <summary>マスク用</summary>
        All = 0xffff,
    }
}

