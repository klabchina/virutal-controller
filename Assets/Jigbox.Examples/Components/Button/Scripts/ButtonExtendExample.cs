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

using UnityEngine;
using Jigbox.Components;

namespace Jigbox.Examples
{
    public sealed class ButtonExtendExample : BasicButton
    {
#region properties

        /// <summary>デフォルトのトランジション用のコンポーネントの型</summary>
        public override System.Type DefaultTransitionClass { get { return typeof(ButtonTransitionExample); } }

        /// <summaryデフォルトのサウンド再生用のコンポーネントの型</summary>
        public override System.Type DefaultSoundClass { get { return typeof(ButtonSoundExample); } }

        /// <summary>KeyRepeatの時間を初期化する際の時間</summary>
        protected override float RepeatIntervalInitializeTime { get { return 1.0f; } }

        /// <summary>最速状態のKeyRepeat間隔</summary>
        protected override float RepeatIntervalFastest { get { return 0.0025f; } }

        /// <summary>KeyRepeat間隔の加速度</summary>
        protected override float RepeatIntervalAcceleration { get { return 0.25f; } }

#endregion
    }
}
