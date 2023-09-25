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
    /// AdvancedButtonTransition のプリセットとなる ScriptableObject
    /// </summary>
    public class AdvancedButtonTransitionPreset : ScriptableObject
    {
#region Fields & Properties - Preset

        [SerializeField] protected bool isDefault;

        public virtual bool IsDefault { get { return isDefault; } }

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

        public virtual bool ScaleEnabled { get { return scaleEnabled; } }
        public virtual float ScalePressDuration { get { return scalePressDuration; } }
        public virtual EasingType ScalePressEasingType { get { return scalePressEasingType; } }
        public virtual MotionType ScalePressMotionType { get { return scalePressMotionType; } }
        public virtual Vector2 ScalePressValue { get { return scalePressValue; } }
        public virtual float ScaleReleaseDuration { get { return scaleReleaseDuration; } }
        public virtual EasingType ScaleReleaseEasingType { get { return scaleReleaseEasingType; } }
        public virtual MotionType ScaleReleaseMotionType { get { return scaleReleaseMotionType; } }
        public virtual Vector2 ScaleReleaseValue { get { return scaleReleaseValue; } }

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

        public virtual bool ColorEnabled { get { return colorEnabled; } }
        public virtual float ColorPressDuration { get { return colorPressDuration; } }
        public virtual EasingType ColorPressEasingType { get { return colorPressEasingType; } }
        public virtual MotionType ColorPressMotionType { get { return colorPressMotionType; } }
        public virtual Color ColorPressValue { get { return colorPressValue; } }
        public virtual float ColorReleaseDuration { get { return colorReleaseDuration; } }
        public virtual EasingType ColorReleaseEasingType { get { return colorReleaseEasingType; } }
        public virtual MotionType ColorReleaseMotionType { get { return colorReleaseMotionType; } }
        public virtual Color ColorReleaseValue { get { return colorReleaseValue; } }

#endregion

#region Fields & Properties - Effect

        [SerializeField] protected bool effectEnabled;
        [SerializeField] protected GameObject effectTemplate;

        public virtual bool EffectEnabled { get { return effectEnabled; } }
        public virtual GameObject EffectTemplate { get { return effectTemplate; } }

#endregion
    }
}
