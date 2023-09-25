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

namespace Jigbox.Tween
{
    public interface IPeriodicMovement : IMovement
    {
#region configuration properies

        /// <summary>何秒かけて変化するかの時間を指定することができます</summary>
        float Duration { get; set; }

        /// <summary>終了後の余韻を秒単位の時間で指定できます。ループ動作の間隔時間や、終了時コールバックの遅延時間としても用いられます</summary>
        float Interval { get; set; }

        /// <summary>ループするか、しないかを指定することができます</summary>
        LoopMode LoopMode { get; set; }

        /// <summary>
        /// <para>ループする際に何回ループを行うか</para>
        /// <para>1回以上を指定することで指定回数ループしたら動作を終了させます。</para>
        /// </summary>
        int LoopCount { get; set; }

#endregion

#region state property

        /// <summary>
        /// <para>開始してから終了するまでに掛かる全ての時間間隔（秒）を示します</para>
        /// <para>ループする場合、Duration + Interval、しない場合、Duration + Delayとなります。</para>
        /// </summary>
        float TotalSpan { get; }

#endregion

#region configuration methods

        /// <summary>ループを指定します</summary>
        IPeriodicMovement LoopWith(LoopMode loopMode, float interval);

        /// <summary>ループを指定します</summary>
        IPeriodicMovement LoopWith(LoopMode loopMode);

#endregion
    }

    public interface IPeriodicMovement<out TValue, TDerived>
        : IPeriodicMovement, IMovement<TValue, TDerived>
        where TDerived : IPeriodicMovement<TValue, TDerived>
    {
#region event handler methods

        /// <summary>ループする設定であった場合の、一周期ごとのイベントハンドラを追加します</summary>
        TDerived OnLoopComplete(Action<TDerived> callback);

        /// <summary>ループする設定であった場合の、一周期ごとのイベントハンドラを破棄します</summary>
        TDerived RemoveOnLoopComplete(Action<TDerived> callback);

        /// <summary>ループする設定であった場合の、一周期ごとのイベントハンドラを全て破棄します</summary>
        TDerived RemoveAllOnLoopComplete();
        
#endregion

#region configuration methods

        /// <summary>ループを指定します</summary>
        new TDerived LoopWith(LoopMode loopMode, float interval);

        /// <summary>ループを指定します</summary>
        new TDerived LoopWith(LoopMode loopMode);

#endregion
    }
}
