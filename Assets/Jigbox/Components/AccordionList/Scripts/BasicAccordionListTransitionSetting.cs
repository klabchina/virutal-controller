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
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// トランジションの設定クラス
    /// </summary>
    [DisallowMultipleComponent]
    public class BasicAccordionListTransitionSetting : MonoBehaviour
    {
        /// <summary>展開時間</summary>
        [SerializeField]
        float expandDuration = 0.2f;

        /// <summary>展開時間の参照</summary>
        public virtual float ExpandDuration
        {
            get { return expandDuration; }
        }

        /// <summary>展開時のイージング</summary>
        [SerializeField]
        EasingType expandEasingType = EasingType.EaseIn;

        /// <summary>展開時のイージング参照</summary>
        public virtual EasingType ExpandEasingType
        {
            get { return expandEasingType; }
        }

        /// <summary>展開時のモーション</summary>
        [SerializeField]
        MotionType expandMotionType = MotionType.Quadratic;

        /// <summary>展開時のモーションの参照</summary>
        public virtual MotionType ExpandMotionType
        {
            get { return expandMotionType; }
        }

        /// <summary>折り畳み時間</summary>
        [SerializeField]
        float collapseDuration = 0.2f;

        /// <summary>折り畳み時間の参照</summary>
        public virtual float CollapseDuration
        {
            get { return collapseDuration; }
        }

        /// <summary>折り畳み時のイージング</summary>
        [SerializeField]
        EasingType collapseEasingType = EasingType.EaseOut;

        /// <summary>折り畳み時のイージングの参照</summary>
        public virtual EasingType CollapseEasingType
        {
            get { return collapseEasingType; }
        }

        /// <summary>折り畳み時のモーション</summary>
        [SerializeField]
        MotionType collapseMotionType = MotionType.Quadratic;

        /// <summary>折り畳み時のモーションの参照</summary>
        public virtual MotionType CollapseMotionType
        {
            get { return collapseMotionType; }
        }
    }
}
