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
    /// ボタントランジションの標準的な処理を担います
    /// </summary>
    /// <remarks>
    /// スケールとカラーとエフェクト生成をサポートしています。
    /// 設定をプリセット化することも出来ます。
    /// </remarks>
    public class AdvancedButtonTransition : ButtonTransitionBase
    {
#region Fields & Properties - Base

        [SerializeField] protected BasicButton button;
        [SerializeField] protected AdvancedButtonTransitionPreset preset;

        /// <summary>ボタンコンポーネントへの参照</summary>
        public virtual BasicButton Button { get { return button; } }

#endregion

#region Fields & Properties - Scale

        [SerializeField] protected bool scaleEnabled = true;
        [SerializeField] protected float scalePressDuration = 0.12f;
        [SerializeField] protected EasingType scalePressEasingType = EasingType.EaseIn;
        [SerializeField] protected MotionType scalePressMotionType = MotionType.Linear;
        [SerializeField] protected Vector2 scalePressValue = new Vector2(0.95f, 0.95f);
        [SerializeField] protected float scaleReleaseDuration = 0.12f;
        [SerializeField] protected EasingType scaleReleaseEasingType = EasingType.EaseIn;
        [SerializeField] protected MotionType scaleReleaseMotionType = MotionType.Linear;
        [SerializeField] protected Vector2 scaleReleaseValue = new Vector2(1, 1);

        public virtual bool ScaleEnabled { get { return preset ? preset.ScaleEnabled : scaleEnabled; } }
        public virtual float ScalePressDuration { get { return preset ? preset.ScalePressDuration : scalePressDuration; } }
        public virtual EasingType ScalePressEasingType { get { return preset ? preset.ScalePressEasingType : scalePressEasingType; } }
        public virtual MotionType ScalePressMotionType { get { return preset ? preset.ScalePressMotionType : scalePressMotionType; } }
        public virtual Vector2 ScalePressValue { get { return preset ? preset.ScalePressValue : scalePressValue; } }
        public virtual float ScaleReleaseDuration { get { return preset ? preset.ScaleReleaseDuration : scaleReleaseDuration; } }
        public virtual EasingType ScaleReleaseEasingType { get { return preset ? preset.ScaleReleaseEasingType : scaleReleaseEasingType; } }
        public virtual MotionType ScaleReleaseMotionType { get { return preset ? preset.ScaleReleaseMotionType : scaleReleaseMotionType; } }
        public virtual Vector2 ScaleReleaseValue { get { return preset ? preset.ScaleReleaseValue : scaleReleaseValue; } }

        protected TweenVector2 scaleTween = new TweenVector2();

#endregion

#region Fields & Properties - Color

        [SerializeField] protected bool colorEnabled = false;
        [SerializeField] protected float colorPressDuration = 0.12f;
        [SerializeField] protected EasingType colorPressEasingType = EasingType.EaseIn;
        [SerializeField] protected MotionType colorPressMotionType = MotionType.Linear;
        [SerializeField] protected Color colorPressValue = new Color(0.9f, 0.9f, 0.9f, 1);
        [SerializeField] protected float colorReleaseDuration = 0.12f;
        [SerializeField] protected EasingType colorReleaseEasingType = EasingType.EaseIn;
        [SerializeField] protected MotionType colorReleaseMotionType = MotionType.Linear;
        [SerializeField] protected Color colorReleaseValue = new Color(1, 1, 1, 1);

        public virtual bool ColorEnabled { get { return preset ? preset.ColorEnabled : colorEnabled; } }
        public virtual float ColorPressDuration { get { return preset ? preset.ColorPressDuration : colorPressDuration; } }
        public virtual EasingType ColorPressEasingType { get { return preset ? preset.ColorPressEasingType : colorPressEasingType; } }
        public virtual MotionType ColorPressMotionType { get { return preset ? preset.ColorPressMotionType : colorPressMotionType; } }
        public virtual Color ColorPressValue { get { return preset ? preset.ColorPressValue : colorPressValue; } }
        public virtual float ColorReleaseDuration { get { return preset ? preset.ColorReleaseDuration : colorReleaseDuration; } }
        public virtual EasingType ColorReleaseEasingType { get { return preset ? preset.ColorReleaseEasingType : colorReleaseEasingType; } }
        public virtual MotionType ColorReleaseMotionType { get { return preset ? preset.ColorReleaseMotionType : colorReleaseMotionType; } }
        public virtual Color ColorReleaseValue { get { return preset ? preset.ColorReleaseValue : colorReleaseValue; } }

        protected Tween.TweenColor colorTween = new Tween.TweenColor();

#endregion

#region Fields & Properties - Effect

        [SerializeField] protected bool effectEnabled = false;
        [SerializeField] protected GameObject effectTemplate;
        protected IAdvancedButtonTransitionEffectProvider effectProvider;

        public virtual bool EffectEnabled { get { return preset ? preset.EffectEnabled : effectEnabled; } }
        public virtual GameObject EffectTemplate { get { return preset ? preset.EffectTemplate : effectTemplate; } }

#endregion

        /// <summary>Selectイベントの通知時に呼び出されます</summary>
        protected override void OnSelect()
        {
            PressTransitionScale();
            PressTransitionColor();
        }

        /// <summary>Deselectイベントの通知時に呼び出されます</summary>
        protected override void OnDeselect()
        {
            ReleaseTransitionScale();
            ReleaseTransitionColor();
        }

        /// <summary>ボタンのイベントの通知を受け取ります</summary>
        protected override void OnNoticeEvent(InputEventType type)
        {
            if (effectProvider != null)
            {
                effectProvider.OnTransition(this, type);
            }
        }

        /// <summary>自動アンロックされた際に呼び出されます</summary>
        public override void NoticeAutoUnlock()
        {
            ReleaseTransitionScale();
            ReleaseTransitionColor();
            if (effectProvider != null)
            {
                effectProvider.OnNoticeAutoUnlock(this);
            }
        }

        /// <summary>Transition の停止</summary>
        protected override void StopTransition()
        {
            scaleTween.Complete();
            colorTween.Complete();
            if (effectProvider != null)
            {
                effectProvider.OnStopTransition(this);
            }
        }

        /// <summary>Press のトランジションを開始（Scale）</summary>
        protected virtual void PressTransitionScale()
        {
            if (ScaleEnabled)
            {
                scaleTween.Kill();
                scaleTween.Begin = transform.localScale;
                scaleTween.Final = ScalePressValue;
                scaleTween.Duration = ScalePressDuration;
                scaleTween.MotionType = ScalePressMotionType;
                scaleTween.EasingType = ScalePressEasingType;
                scaleTween.Start();
            }
        }

        /// <summary>Press のトランジションを開始（Color）</summary>
        protected virtual void PressTransitionColor()
        {
            if (ColorEnabled)
            {
                colorTween.Kill();
                colorTween.Begin = ColorReleaseValue;
                colorTween.Final = ColorPressValue;
                colorTween.Duration = ColorPressDuration;
                colorTween.MotionType = ColorPressMotionType;
                colorTween.EasingType = ColorPressEasingType;
                colorTween.Start();
            }
        }

        /// <summary>Release のトランジションを開始（Scale）</summary>
        protected virtual void ReleaseTransitionScale()
        {
            if (ScaleEnabled)
            {
                scaleTween.Kill();
                scaleTween.Begin = transform.localScale;
                scaleTween.Final = ScaleReleaseValue;
                scaleTween.Duration = ScaleReleaseDuration;
                scaleTween.MotionType = ScaleReleaseMotionType;
                scaleTween.EasingType = ScaleReleaseEasingType;
                scaleTween.Start();
            }
        }

        /// <summary>Release のトランジションを開始（Color）</summary>
        protected virtual void ReleaseTransitionColor()
        {
            if (ColorEnabled)
            {
                colorTween.Kill();
                colorTween.Begin = ColorPressValue;
                colorTween.Final = ColorReleaseValue;
                colorTween.Duration = ColorReleaseDuration;
                colorTween.MotionType = ColorReleaseMotionType;
                colorTween.EasingType = ColorReleaseEasingType;
                colorTween.Start();
            }
        }

        /// <summary>MonoBehaviour.Awake()</summary>
        protected virtual void Awake()
        {
            if (button == null)
            {
                button = GetComponent<BasicButton>();
            }

            AwakeScale();
            AwakeColor();
            AwakeEffect();
        }

        /// <summary>Scale に関する Awake 処理</summary>
        protected virtual void AwakeScale()
        {
            scaleTween.OnUpdate(tween => transform.localScale = new Vector3(tween.Value.x, tween.Value.y, transform.localScale.z));
        }

        /// <summary>Color に関する Awake 処理</summary>
        protected virtual void AwakeColor()
        {
            colorTween.OnUpdate(tween =>
            {
                if (button.Clickable)
                {
                    button.SetColorMultiply(tween.Value);
                }
            });
        }

        /// <summary>Effect に関する Awake 処理</summary>
        protected virtual void AwakeEffect()
        {
            if (EffectEnabled && EffectTemplate != null)
            {
                effectProvider = CreateEffectProvider();
            }
            if (effectProvider != null)
            {
                effectProvider.OnAwake(this);
            }
        }

        /// <summary>EffectProvider の生成処理</summary>
        protected virtual AdvancedButtonTransitionEffectProvider CreateEffectProvider()
        {
            return new AdvancedButtonTransitionEffectProvider(EffectTemplate);
        }

        /// <summary>MonoBehaviour.OnDestroy()</summary>
        protected virtual void OnDestroy()
        {
            OnDestroyScale();
            OnDestroyColor();
            OnDestroyEffect();
        }

        /// <summary>Scale に関する OnDestroy 処理</summary>
        protected virtual void OnDestroyScale()
        {
        }

        /// <summary>Color に関する OnDestroy 処理</summary>
        protected virtual void OnDestroyColor()
        {
        }

        /// <summary>Effect に関する OnDestroy 処理</summary>
        protected virtual void OnDestroyEffect()
        {
            if (effectProvider != null)
            {
                effectProvider.OnDestroy(this);
            }
        }
    }
}
