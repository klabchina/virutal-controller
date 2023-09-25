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

using System;
using Jigbox.Examples;
using UnityEngine;
using UnityEngine.UI;
using Jigbox.Tween;

public class TweenUIDemoScene : ExampleSceneBase
{
#region Properties

    [SerializeField]
    EasingType easingType = EasingType.EaseOut;

    [SerializeField]
    float maxDuration = 10.0f;

    [SerializeField]
    Vector2 buttonScaleOnClick;

    [SerializeField]
    Vector2 maxButtonScale = new Vector2(16.0f, 16.0f);

    [SerializeField]
    float duration = 1.0f;

    [SerializeField]
    InputField durationInput = null;

    [SerializeField]
    InputField scaleXInput = null;

    [SerializeField]
    InputField scaleYInput = null;

    [SerializeField]
    TweenSlider linearSlider = null;

    [SerializeField]
    TweenSlider easingSlider = null;

    [SerializeField]
    Button[] easingButtons = null;

    MotionType[] motions = {
        MotionType.Linear,
        MotionType.Quadratic,
        MotionType.Cubic,
        MotionType.Quartic,
        MotionType.Quintic,
        MotionType.Exponential,
        MotionType.Circular,
        MotionType.Sine,
        MotionType.Bounce,
        MotionType.Back,
        MotionType.Elastic
    };

    protected override int TargetFrameRate
    {
        get { return 60; }
    }

#endregion

#region Unity Method

    void Start()
    {
        if (durationInput)
        {
            InitDurationInput();
        }

        if (scaleXInput)
        {
            scaleXInput.onEndEdit.AddListener(OnEndEditScaleX);
            scaleXInput.text = buttonScaleOnClick.x.ToString("0.0");
        }

        if (scaleYInput)
        {
            scaleYInput.onEndEdit.AddListener(OnEndEditScaleY);
            scaleYInput.text = buttonScaleOnClick.y.ToString("0.0");
        }

        for (int i = 0; i < motions.Length; i++)
        {
            if (i < easingButtons.Length)
            {
                InitButton(easingButtons[i], motions[i]);
            }
        }
    }

#endregion

#region Private Method

    void InitDurationInput()
    {
        durationInput.onEndEdit.AddListener(value =>
        {
            duration = Mathf.Clamp(float.Parse(value), 0.0f, maxDuration);
            durationInput.text = duration.ToString("0.00");
        });
        durationInput.text = duration.ToString("0.00");
    }

    void InitButton(Button button, MotionType motionType)
    {
        if (!button)
        {
            return;
        }

        var textLabel = button.GetComponentInChildren<Text>();
        if (textLabel)
        {
            textLabel.text = motionType.ToString();
        }

        var scaleTween = new TweenVector2(tween =>
        {
            if (button)
            {
                var s = tween.Value;
                button.transform.localScale = new Vector3(s.x, s.y, 1);
            }
        }).OnStart(tween =>
        {
            tween.FromTo(buttonScaleOnClick, Vector2.one, duration).EasingWith(motionType, easingType);
            return;
        });

        button.onClick.AddListener(() =>
        {
            scaleTween.Start();

            linearSlider.Tween.Duration = duration;
            linearSlider.StartTween(0f, 0.8f);

            easingSlider.Tween.MotionType = motionType;
            easingSlider.Tween.EasingType = easingType;
            easingSlider.Tween.Duration = duration;
            easingSlider.StartTween(0f, 0.8f);
        });

        linearSlider.Tween.EasingWith(MotionType.Linear);
    }


    void OnEndEditScaleX(string input)
    {
        var before = buttonScaleOnClick;
        var value = Mathf.Clamp(float.Parse(input), 0, maxButtonScale.x);
        buttonScaleOnClick = new Vector2(value, before.y);
        scaleXInput.text = value.ToString("0.0");
    }

    void OnEndEditScaleY(string input)
    {
        var before = buttonScaleOnClick;
        var value = Mathf.Clamp(float.Parse(input), 0, maxButtonScale.y);
        buttonScaleOnClick = new Vector2(before.x, value);
        scaleYInput.text = value.ToString("0.0");
    }

#endregion
}

