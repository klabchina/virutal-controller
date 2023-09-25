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
    /// <summary>
    /// AdvancedButtonTransition のエフェクト生成処理を担います
    /// </summary>
    public interface IAdvancedButtonTransitionEffectProvider
    {
        /// <summary>AdvancedButtonTransition.Awake のタイミングで呼ばれます</summary>
        void OnAwake(AdvancedButtonTransition transition);

        /// <summary>ボタンイベントに紐づくトランジション開始処理</summary>
        void OnTransition(AdvancedButtonTransition transition, InputEventType type);

        /// <summary>自動アンロックされた際の処理</summary>
        void OnNoticeAutoUnlock(AdvancedButtonTransition transition);

        /// <summary>トランジションの停止処理</summary>
        void OnStopTransition(AdvancedButtonTransition transition);

        /// <summary>AdvancedButtonTransition.OnDestroy のタイミングで呼ばれます</summary>
        void OnDestroy(AdvancedButtonTransition transition);
    }
}
