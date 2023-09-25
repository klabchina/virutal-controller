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
using UnityEngine.Serialization;

namespace Jigbox.Components
{
    [Serializable]
    public class MarqueeTransitionProperty
    {
#region fields

        /// <summary>
        /// Viewportへの参照
        /// </summary>
        [SerializeField]
        [HideInInspector]
        RectTransform viewport;

        /// <summary>
        /// モーションのタイプ
        /// </summary>
        [SerializeField]
        [HideInInspector]
        MarqueeScrollType scrollType;
        
        /// <summary>
        /// アニメーションのタイプ
        /// </summary>
        [SerializeField]
        [HideInInspector]
        MarqueeScrollDirectionType scrollDirectionType;
        
        /// <summary>
        /// 入場時のアニメーションプロパティ
        /// </summary>
        [SerializeField]
        [HideInInspector]
        MarqueeAnimationProperty entranceAnimationProperty;
        
        /// <summary>
        /// 退場時のアニメーションプロパティ
        /// </summary>
        [SerializeField]
        [HideInInspector]
        MarqueeAnimationProperty exitAnimationProperty;

        /// <summary>
        /// トランジションのスピード
        /// </summary>
        [SerializeField]
        [HideInInspector]
        float speed;

        /// <summary>
        /// トランジション開始時の動き出すまでの遅延時間
        /// </summary>
        [FormerlySerializedAs("startDelay")]
        [SerializeField]
        [HideInInspector]
        float durationDelay;

        /// <summary>
        /// トランジションによる移動後の待機時間
        /// </summary>
        [SerializeField]
        [HideInInspector]
        float interval;

        /// <summary>
        /// ループを行った際の動き出すまでの遅延時間
        /// </summary>
        [FormerlySerializedAs("loopStartDelay")]
        [SerializeField]
        [HideInInspector]
        float loopDurationDelay;

        /// <summary>
        /// コンテンツの先頭のトランジション開始位置
        /// Viewportの大きさに対しての割合で入力される
        /// </summary>
        [SerializeField]
        [HideInInspector]
        float startPositionRate;

        /// <summary>
        /// コンテンツの末尾がトランジション終了位置
        /// Viewportの大きさに対しての割合で入力される
        /// </summary>
        [SerializeField]
        [HideInInspector]
        float endPositionRate;

        /// <summary>
        /// ループを行うかどうか
        /// </summary>
        [SerializeField]
        [HideInInspector]
        bool isLoop;

        protected IMarqueeViewProperty viewProperty;

#endregion

#region properties

        public virtual MarqueeScrollType ScrollType { get { return scrollType; } set { scrollType = value; } }

        public virtual MarqueeScrollDirectionType ScrollDirectionType { get { return scrollDirectionType; } set { scrollDirectionType = value; } }

        public virtual MarqueeAnimationProperty EntranceAnimationProperty { get { return entranceAnimationProperty; } set { entranceAnimationProperty = value; } }

        public virtual MarqueeAnimationProperty ExitAnimationProperty { get { return exitAnimationProperty; } set { exitAnimationProperty = value; } }

        public virtual float Speed { get { return speed; } set { speed = value; } }

        public virtual float DurationDelay { get { return this.durationDelay; } set { this.durationDelay = value; } }

        public virtual bool HasDurationDelay { get { return DurationDelay > 0; } }

        public virtual float Interval { get { return interval; } set { interval = value; } }

        public virtual bool HasInterval { get { return Interval > 0; } }

        public virtual float LoopDurationDelay { get { return loopDurationDelay; } set { loopDurationDelay = value; } }

        public virtual bool HasLoopDelay { get { return LoopDurationDelay > 0; } }

        public virtual float StartPositionRate
        {
            get
            {
                // 入場時のアニメーションがある場合は0で固定
                if (this.entranceAnimationProperty.Enable)
                {
                    return 0;
                }
                
                return startPositionRate;
            }
            set
            {
                startPositionRate = value;
            }
        }

        public virtual float EndPositionRate
        {
            get
            {
                // 退場時のアニメーションがある場合は1で固定
                if (exitAnimationProperty.Enable)
                {
                    return 1;
                }
                
                return endPositionRate;
            }
            set
            {
                endPositionRate = value;
            }
        }

        public virtual bool IsLoop { get { return isLoop; } set { isLoop = value; } }

        public virtual RectTransform Viewport { get { return viewport; } set { viewport = value; } }

        public virtual RectTransform Container { get { return viewProperty.Container; } }

        public virtual Vector2 ViewportSize { get { return viewport.rect.size; } }

        public float Length { get { return viewProperty.Length; } }

#endregion

#region public methods

        /// <summary>
        /// Transitionに必要なLayout側が持つ情報を扱うpropertyをセットする
        /// </summary>
        /// <param name="property"></param>
        public virtual void SetMarqueeViewProperty(IMarqueeViewProperty property)
        {
            this.viewProperty = property;
        }

#endregion
    }
}
