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
using Jigbox.Delegatable;

using FillMethod = UnityEngine.UI.Image.FillMethod;
using FillTargetComponent = Jigbox.Components.GaugeModel.FillTargetComponent;

namespace Jigbox.Components
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class Gauge : MonoBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>
        /// ゲージの値が変更された際のデリゲート型
        /// </summary>
        public class GaugeValueChangedDelegate : EventDelegate<float>
        {
            public GaugeValueChangedDelegate(Callback eventDelegate) : base(eventDelegate)
            {
            }
        }

        /// <summary>
        /// ゲージの現在の階調が更新された際のデリゲート型
        /// </summary>
        public class GaugeStepChangedDelegate : EventDelegate<int>
        {
            public GaugeStepChangedDelegate(Callback eventDelegate) : base(eventDelegate)
            {
            }
        }

#endregion

#region properties

        /// <summary>ゲージの値(0～1)</summary>
        [HideInInspector]
        [SerializeField]
        protected float value = 1.0f;

        /// <summary>ゲージの値(0～1)</summary>
        public virtual float Value
        {
            get
            {
                return value;
            }
            set
            {
                if (model.Steps == 0)
                {
#if UNITY_EDITOR
                    // 使われないLimiterの設定しか無い場合は, 使われるLimiterがどれか警告をだす
                    if (CurrentStepLimiter != null && ValueLimiter == null)
                    {
                        Debug.LogWarning("Gauge.Value : Needed ValuLimiter when Steps is 0", this);
                    }
#endif
                    if (ValueLimiter != null)
                    {
                        // ValueのLimitは階調設定がされていないときに使う
                        value = ValueLimiter(this.value, value);
                    }
                    if (this.value == value)
                    {
                        return;
                    }
                }
                else
                {
#if UNITY_EDITOR
                    // 使われないLimiterの設定しか無い場合は, 使われるLimiterがどれか警告をだす
                    if (ValueLimiter != null && CurrentStepLimiter == null)
                    {
                        Debug.LogWarning("Gauge.Value : Needed CurrentSteplimiter when Steps > 0", this);
                    }
#endif
                    int step = model.GetStepFromValue(value);
                    if (CurrentStepLimiter != null)
                    {
                        step = CurrentStepLimiter(Steps, currentStep, step);
                    }
                    if (currentStep == step)
                    {
                        return;
                    }

                    currentStep = step;
                    value = model.GetValueFromStep(step);
                }

                this.value = value;
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    UpdateView();
                    ExecuteDelegates();
                }
#else
                UpdateView();
                ExecuteDelegates();
#endif
            }
        }

        /// <summary>フィリング対象コンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected UnityEngine.Object target;

        /// <summary>フィリング対象のRectTransformの状態を計算するための矩形領域(フィリング対象がRectTransformの場合のみ使用)</summary>
        [HideInInspector]
        [SerializeField]
        protected RectTransform fillRect;

        /// <summary>ゲージのモデル</summary>
        [HideInInspector]
        [SerializeField]
        protected GaugeModel model;

        /// <summary>ゲージ全体の階調数(0で無階調)</summary>
        public int Steps
        {
            get
            {
                return model.Steps;
            }
            set
            {
                if (model.Steps != value)
                {
                    if (value < 0)
                    {
#if UNITY_EDITOR
                        Debug.LogError("Gauge.Value : Can't use minus value!", this);
#endif
                        return;
                    }

                    model.Steps = value;

                    if (value == 0)
                    {
                        return;
                    }

                    CurrentStep = model.GetStepFromValue(this.value);
                }
            }
        }

        /// <summary>現在の階調</summary>
        [HideInInspector]
        [SerializeField]
        protected int currentStep;

        /// <summary>現在の階調</summary>
        public int CurrentStep
        {
            get
            {
                return Steps > 0 ? currentStep : 0;
            }
            set
            {
                if (Steps == 0)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("Gauge.CurrentStep : Can't use this property when Steps is 0!", this);
#endif
                    return;
                }
#if UNITY_EDITOR
                // 使われないLimiterの設定しか無い場合は, 使われるLimiterがどれか警告をだす
                if (ValueLimiter != null && CurrentStepLimiter == null)
                {
                    Debug.LogWarning("Gauge.CurrentStep : Needed CurrentStepLimiter when setting step!", this);
                }
#endif
                if (CurrentStepLimiter != null)
                {
                    value = CurrentStepLimiter(Steps, currentStep, value);
                }

                if (currentStep != value)
                {
                    currentStep = value;
                    this.value = model.GetValueFromStep(currentStep);
#if UNITY_EDITOR
                    if (Application.isPlaying)
                    {
                        UpdateView();
                        ExecuteDelegates();
                    }
#else
                    UpdateView();
                    ExecuteDelegates();
#endif
                }
            }
        }

        /// <summary>値が変更された際のコールバック</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onValueChanged = new DelegatableList();

        /// <summary>値が変更された際のコールバック</summary>
        public DelegatableList OnValueChanged { get { return onValueChanged; } }

        /// <summary>現在の階調が変更された際のコールバック(steps > 0の場合のみ有効)</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onStepChanged = new DelegatableList();

        /// <summary>現在の階調が変更された際のコールバック(steps > 0の場合のみ有効)</summary>
        public DelegatableList OnStepChanged { get { return onStepChanged; } }
        
        /// <summary>View</summary>
        protected GaugeViewBase view;

        /// <summary>値更新リミッターが定義されたターゲット</summary>
        [HideInInspector]
        [SerializeField]
        protected MonoBehaviour valueLimitTarget = null;

        /// <summary>Value値更新リミッター</summary>
        /// <param>現在の値</param>
        /// <param>設定しようとしている値</param>
        /// <returns>設定する値</returns>
        protected System.Func<float, float, float> valueLimiter;

        /// <summary>Value値更新リミッタープロパティ</summary>
        public System.Func<float, float, float> ValueLimiter
        {
            get { return valueLimiter; }
            set
            {
                if (valueLimiter == value)
                {
                    return;
                }
                valueLimiter = value;
                if (Steps == 0)
                {
                    // Limiterが更新されたのでValueの更新を行いLimiter制限にかけさせる
                    this.Value = this.Value;
                }
            }
        }

        /// <summary>CurrentStep値更新リミッター</summary>
        /// <param>ゲージ全体の階調数</param>
        /// <param>>現在の階調</param>
        /// <param>次の階調</param>
        /// <returns>設定する値</returns>
        protected System.Func<int, int, int, int> currentStepLimiter;

        /// <summary>CurrentStep値更新リミッタープロパティ</summary>
        public System.Func<int, int, int, int> CurrentStepLimiter
        {
            get { return currentStepLimiter;  }
            set
            {
                if (currentStepLimiter == value)
                {
                    return;
                }
                currentStepLimiter = value;
                if (Steps > 0)
                {
                    // Limiterが更新されたのでCurrentStepの更新を行いLimiter制限にかけさせる
                    this.CurrentStep = this.CurrentStep;
                }
            }
        }

#endregion

#region public methods

        /// <summary>
        /// 値が変更された際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(float)の関数</param>
        public void AddValueChangedEvent(GaugeValueChangedDelegate.Callback callback)
        {
            onValueChanged.Add(new GaugeValueChangedDelegate(callback));
        }

        /// <summary>
        /// 現在の階調が更新された際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(int)の関数</param>
        public void AddStepChangedEvent(GaugeStepChangedDelegate.Callback callback)
        {
            onStepChanged.Add(new GaugeStepChangedDelegate(callback));
        }

        /// <summary>
        /// Viewの状態を現在の値に合わせて更新します。
        /// </summary>
        public virtual void UpdateView()
        {
            if (view == null)
            {
                return;
            }

            view.UpdateView(value);
        }

#endregion

#region protected methods

        /// <summary>
        /// Viewを生成します。
        /// </summary>
        protected virtual void CreateView()
        {
            switch (model.FillMethod)
            {
                case (int) FillMethod.Horizontal:
                    if (model.FillTarget == FillTargetComponent.RectTransform)
                    {
                        HorizontalGaugeView horizontalView = new HorizontalGaugeView();
                        view = horizontalView;
                        horizontalView.InitView(fillRect, target, model.FillMethod, model.FillOrigin);
                    }
                    else
                    {
                        view = new FilledHorizontalGaugeView();
                    }
                    break;
                case (int) FillMethod.Vertical:
                    if (model.FillTarget == FillTargetComponent.RectTransform)
                    {
                        VerticalGaugeView verticalView = new VerticalGaugeView();
                        view = verticalView;
                        verticalView.InitView(fillRect, target, model.FillMethod, model.FillOrigin);
                    }
                    else
                    {
                        view = new FilledVerticalGaugeView();
                    }
                    break;
                case (int) FillMethod.Radial90:
                    view = new RadialGaugeView();
                    break;
                case (int) FillMethod.Radial180:
                    view = new RadialGaugeView();
                    break;
                case (int) FillMethod.Radial360:
                    view = new RadialGaugeView();
                    break;
            }

            if (model.FillTarget != FillTargetComponent.RectTransform)
            {
                view.InitView(target, model.FillMethod, model.FillOrigin);
            }
        }

        /// <summary>
        /// デリゲートを実行します。
        /// </summary>
        protected void ExecuteDelegates()
        {
            if (onValueChanged.Count > 0)
            {
                onValueChanged.Execute(Value);
            }
            if (Steps > 0 && onStepChanged.Count > 0)
            {
                onStepChanged.Execute(CurrentStep);
            }
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (target == null)
            {
                Debug.LogError("Gauge.Awake : Fill target component not found!", gameObject);
                UnityEditor.EditorApplication.isPaused = true;
            }

            if (model.FillTarget == (int) FillTargetComponent.RectTransform && fillRect == null)
            {
                Debug.LogError("Gauge.Awake : FillRect is not found!", gameObject);
                UnityEditor.EditorApplication.isPaused = true;
            }
#endif
            if (valueLimitTarget != null)
            {
                var valueLimiter = valueLimitTarget as IGaugeValueLimiter;
                if (valueLimiter != null)
                {
                    ValueLimiter = valueLimiter.LimitValue;
                }
                var stepLimiter = valueLimitTarget as IGaugeStepLimiter;
                if (stepLimiter != null)
                {
                    CurrentStepLimiter = stepLimiter.LimitCurrentStep;
                }
            }
        }

        protected virtual void Start()
        {
            // RectTransformをいじる場合もあるので、AwakeではなくStartで処理
            CreateView();
            UpdateView();
        }

#endregion
    }
}
