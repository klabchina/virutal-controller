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

namespace Jigbox.Components
{
    public enum MarqueeState
    {
        /// <summary>状態なし</summary>
        None = 0,
        /// <summary>動作開始前</summary>
        Idle = 1 << 1,
        /// <summary>動作開始</summary>
        Start = 1 << 2,
        /// <summary> Viwe側のLayoutの計算終了を待つ状態</summary>
        Layout = 1 << 3,
        /// <summary>移動が始まる前の待機時間</summary>
        DurationDelay = 1 << 4,
        /// <summary>移動中</summary>
        Duration = 1 << 5,
        /// <summary>移動が終わっての待機時間</summary>
        Interval = 1 << 6,
        /// <summary>ループによる動作開始</summary>
        LoopStart = 1 << 7,
        /// <summary>ループ時のLayoutの計算終了を待つ状態</summary>
        LoopLayout = 1 << 8,
        /// <summary>ループによる移動が始まる前の状態</summary>
        LoopDurationDelay = 1 << 9,
        /// <summary>強制停止</summary>
        Kill = 1 << 10,
        /// <summary>動作が完了した状態</summary>
        Done = 1 << 11,
        /// <summary>スクロールを行わないで良い状態</summary>
        IfNeeded = 1 << 12,
        /// <summary>退場中</summary>
        ExitDuration = 1 << 13,
        /// <summary>入場中</summary>
        EntranceDuration = 1 << 14,
        /// <summary>入場後の待機時間</summary>
        EntranceDelay = 1 << 15,
        /// <summary>退場前の待機時間</summary>
        ExitDelay = 1 << 16,
        /// <summary>マスク用</summary>
        All = 0xffff,
    }
}
