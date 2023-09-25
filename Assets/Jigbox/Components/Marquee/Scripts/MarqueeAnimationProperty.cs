/**
s * Jigbox
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
using UnityEngine;
using Jigbox.Tween;

namespace Jigbox.Components
{
    [Serializable]
    public class MarqueeAnimationProperty
    {
#region fields

        /// <summary>
        /// アニメーションするか
        /// </summary>
        [SerializeField]
        [HideInInspector]
        bool enable = false;

        /// <summary>
        /// モーションタイプ
        /// </summary>
        [SerializeField]
        [HideInInspector]
        MotionType motionType = MotionType.Cubic;
        
        /// <summary>
        /// イージングタイプ
        /// </summary>
        [SerializeField]
        [HideInInspector]
        EasingType easingType = EasingType.EaseOut;

        /// <summary>
        /// トランジションの時間
        /// </summary>
        [SerializeField]
        [HideInInspector]
        float duration = 1.0f;
        
        /// <summary>
        /// 待機時間
        /// </summary>
        [SerializeField]
        [HideInInspector]
        float delay = 0.0f;

        #endregion

#region properties

        public virtual bool Enable { get { return enable; } set { enable = value; } }

        public virtual MotionType MotionType { get { return motionType; } set { motionType = value; } }

        public virtual EasingType EasingType { get { return easingType; } set { easingType = value; } }

        public virtual float Duration { get { return duration; } set { duration = value; } }
        
        public virtual float Delay { get { return delay; } set { delay = value; } }
        
        public virtual bool HasDelay { get { return delay > 0; } }
#endregion
    }
}

