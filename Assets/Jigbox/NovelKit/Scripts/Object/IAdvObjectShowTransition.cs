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

using Jigbox.Tween;

namespace Jigbox.NovelKit
{
    public interface IAdvObjectShowTransition
    {
#region properties
        
        /// <summary>表示切替時のトランジションの時間</summary>
        float ShowTransitionTime { get; }

        /// <summary>表示切替時のアルファのトランジション用Tween</summary>
        TweenSingle AlphaTween { get; }

#endregion

#region public methods

        /// <summary>
        /// オブジェクトを表示状態にします。
        /// </summary>
        /// <param name="time">トランジションの時間</param>
        void Show(float time);

        /// <summary>
        /// オブジェクトを非表示状態にします。
        /// </summary>
        /// <param name="time">トランジションの時間</param>
        void Hide(float time);

#endregion
    }
}
