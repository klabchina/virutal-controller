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
    public interface IMarqueeTransitionHandler
    {
        /// <summary>
        /// トランジション開始時用のデリゲートを発火します
        /// </summary>
        void OnStartTransition();

        /// <summary>
        /// トランジション側のLayoutステート終了時用のデリゲートを発火します
        /// </summary>
        void OnCompleteLayout();

        /// <summary>
        /// トランジション開始時の遅延時間終了時のデリゲートを発火します
        /// </summary>
        void OnCompleteDurationDelay();

        /// <summary>
        /// 移動終了時に呼ばれるデリゲートを発火します
        /// </summary>
        void OnCompleteDuration();
        
        /// <summary>
        /// 入場開始時の遅延時間終了時のデリゲートを発火します
        /// </summary>
        void OnCompleteEntranceDelay();

        /// <summary>
        /// 入場終了時に呼ばれるデリゲートを発火します
        /// </summary>
        void OnCompleteEntranceDuration();
        
        /// <summary>
        /// 退場開始時の遅延時間終了時のデリゲートを発火します
        /// </summary>
        void OnCompleteExitDelay();

        /// <summary>
        /// 退場終了時に呼ばれるデリゲートを発火します
        /// </summary>
        void OnCompleteExitDuration();

        /// <summary>
        /// 移動終了後の待機時間終了時に呼ばれるデリゲートを発火します
        /// </summary>
        void OnCompleteInterval();

        /// <summary>
        /// トランジションのループ開始時用のデリゲートを発火します
        /// </summary>
        void OnLoopStart();

        /// <summary>
        /// トランジション側のLoopLayoutステート終了時用のデリゲートを発火します
        /// </summary>
        void OnCompleteLoopLayout();

        /// <summary>
        /// ループ後の移動開始前の遅延時間終了時に呼ばれるデリゲートを発火します
        /// </summary>
        void OnCompleteLoopDurationDelay();

        /// <summary>
        /// スクロールするかどうかが確定したタイミングで呼ばれるデリゲートを発火します
        /// </summary>
        /// <param name="isScroll">スクロールするかどうか</param>
        void OnCompleteLayoutContent(bool isScroll);

        /// <summary>
        /// 一時停止時に呼ばれるデリゲートを発火します
        /// </summary>
        void OnPause();

        /// <summary>
        /// 再開時に呼ばれるデリゲートを発火します
        /// </summary>
        void OnResume();

        /// <summary>
        /// トランジション停止時用のデリゲートを発火します
        /// </summary>
        void OnKillTransition();
    }
}
