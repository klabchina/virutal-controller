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
    public interface IPeriodicMovementBehaviour<out TValue, in TPeriodicMovement>
        where TPeriodicMovement : IPeriodicMovement<TValue, TPeriodicMovement>
    {
#region properties

        /// <summary>動作の間隔</summary>
        float ProgressSpan { get; }

        /// <summary>経過時間</summary>
        float DeltaTime { get; }

        /// <summary>値</summary>
        TValue Value { get; }

        /// <summary>値がキャッシュされているかどうか</summary>
        bool IsCached { get; }

#endregion

#region public methods

        /// <summary>
        /// 初期化します。
        /// </summary>
        void Init();

        /// <summary>
        /// 経過時間を更新します。
        /// </summary>
        /// <param name="deltaTime">経過時間</param>
        /// <returns>経過時間の更新によって、動作が完了した場合<c>true</c>を返します。</returns>
        bool UpdateDeltaTime(float deltaTime);

        /// <summary>
        /// 進捗度を返します。
        /// </summary>
        /// <param name="progress">0～1までの進捗度</param>
        /// <returns></returns>
        float Progress(float progress);

#endregion
    }
}
