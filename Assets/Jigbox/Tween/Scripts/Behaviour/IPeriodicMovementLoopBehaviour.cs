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
    public interface IPeriodicMovementLoopBehaviour<out TValue, in TPeriodicMovement>
        : IPeriodicMovementBehaviour<TValue, TPeriodicMovement>
        where TPeriodicMovement : IPeriodicMovement<TValue, TPeriodicMovement>
    {
        /// <summary>初回の動作間隔</summary>
        float FirstProgressSpan { get; }

        /// <summary>
        /// <para>動作が完了するまでの時間の総量(秒)</para>
        /// <para>Tweenのループ回数を指定している状態でのみ正しい値を返します。</para>
        /// </summary>
        float TotalSpan { get; }

        /// <summary>ループした回数</summary>
        int LoopCount { get; }

        /// <summary>
        /// <para>最新のループ状態での補間が1回の更新で完了した回数</para>
        /// <para>差の大きなDeltaTimeを設定した場合、2回以上同時に補間が完了することがあるため、boolではなくintで保持</para>
        /// </summary>
        int LastCompleteCount { get; }
    }
}
