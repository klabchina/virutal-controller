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
using Jigbox.Tween;

namespace Jigbox.Components
{
    public class TweenAlpha : TweenBase
    {
#region inner classes, enum, and structs

        /// <summary>
        /// アルファの変更対象の種類
        /// </summary>
        public enum TargetType
        {
            /// <summary>自身のRendererを対象とする</summary>
            Renderer,
            /// <summary>自身のGraphicを対象とする</summary>
            Graphic,
            /// <summary>自身のCanvasGroupを対象とする</summary>
            CanvasGroup,
        }

#endregion

#region properties

        /// <summary>Tween</summary>
        [HideInInspector]
        [SerializeField]
        protected TweenSingle tween;

        /// <summary>Tween</summary>
        public TweenSingle Tween { get { return tween; } }

        /// <summary>Tweenのインタフェース</summary>
        protected override ITween ITween { get { return tween; } }

        /// <summary>アルファの変更対象の種類</summary>
        [HideInInspector]
        [SerializeField]
        protected TargetType type;

        /// <summary>アルファの変更対象の種類</summary>
        public TargetType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type != value)
                {
                    type = value;
#if UNITY_EDITOR
                    // エディタ実行時のみ再生状態を確認
                    if (Application.isPlaying)
                    {
                        InitTarget();
                    }
#else
                    InitTarget();
#endif
                }
            }
        }

        /// <summary>アルファの変更対象</summary>
        protected ITweenAlphaTarget target;

#endregion

#region private Method

        /// <summary>
        /// 色の変更対象の初期化を行います。
        /// </summary>
        void InitTarget()
        {
            switch (type)
            {
                case TargetType.Renderer:
                    target = new TweenColorTargetRenderer(gameObject);
                    break;
                case TargetType.Graphic:
                    target = new TweenColorTargetGraphic(gameObject);
                    break;
                case TargetType.CanvasGroup:
                    target = new TweenAlphaTargetCanvasGroup(gameObject);
                    break;
            }
        }

#endregion

#region override unity method

        protected virtual void Awake()
        {
            InitTarget();
            tween.OnUpdate(tw => target.Alpha = tw.Value);
            tween.OnComplete(OnCompleteTween);
        }

#endregion
    }
}
