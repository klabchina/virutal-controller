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
using Jigbox.HUD;
using System.Collections.Generic;
using Jigbox.Examples;

public class Easing3DTest : ExampleSceneBase
{
    static readonly MotionType[] motions = {
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

#region Fields

    [SerializeField]
    Transform objectRoot = null;

    [SerializeField]
    Canvas hudCanvas = null;

    [SerializeField]
    GameObject demoPrefab = null;

    [SerializeField]
    Vector3 begin = new Vector3(-8, 1, -10);

    [SerializeField]
    Vector3 final = new Vector3(8, 1, -10);

    [SerializeField]
    Vector3 span = new Vector3(0, 0, 2);

    [SerializeField]
    GameObject labelPrefab = null;

    [SerializeField]
    EasingType easingType = EasingType.EaseIn;

    [SerializeField]
    float duration = 2.0f;

    [SerializeField]
    float loopInterval = 1.0f;

    [SerializeField]
    LoopMode loopMode = LoopMode.PingPong;

    List<SampleTweenComponent> demos = new List<SampleTweenComponent>();

    protected override int TargetFrameRate
    {
        get { return 60; }
    }

#endregion

#region Unity Method

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < motions.Length; i++)
        {
            InitDemo(motions[i], begin + span * i, final + span * i);
        }
    }

#endregion

#region Private Method

    void InitDemo(MotionType motionType, Vector3 beginPosition, Vector3 finalPosition)
    {
        var demo = CreateTweenDemo(motionType, beginPosition, finalPosition);
        if (demo)
        {
            CreateHud(motionType, demo.transform);
        }
        this.demos.Add(demo);
    }

    SampleTweenComponent CreateTweenDemo(MotionType motionType, Vector3 beginPosition, Vector3 finalPosition)
    {
        var go = Instantiate(demoPrefab);
        if (go == null)
        {
            return null;
        }

        var demo = go.GetComponent<SampleTweenComponent>();
        if (demo)
        {
            demo.transform.SetParent(objectRoot, false);
            demo.name = string.Format("Demo Ball ({0})", motionType);

            demo.Tween
                .FromTo(beginPosition, finalPosition, duration) // (初期値, 終了値, 時間間隔(秒)) を指定
                .EasingWith(motionType, easingType)             // (緩急の種類, 緩急の入り, 抜き) を指定
                .LoopWith(loopMode, loopInterval)               // (ループの休止時間(秒), ループの種類) を指定
                .Start();                                       // トゥイーンの開始
        }
        return demo;
    }

    SimpleNameLabel CreateHud(MotionType motionType, Transform target)
    {
        var go = Instantiate(labelPrefab);
        if (go == null)
        {
            return null;
        }

        var hud = go.GetComponent<SimpleNameLabel>();
        if (hud && target)
        {
            hud.transform.SetParent(hudCanvas.transform, false);
            hud.name = string.Format("HUD Label ({0})", motionType);

            hud.SetHudTarget(target);
            hud.TextToDisplay = string.Format("{0}", motionType);
        }
        return hud;
    }

    [AuthorizedAccess]
    void OnChangeEasingTypeEaseInOut()
    {
        ChangeEasingType(EasingType.EaseInOut);
    }

    [AuthorizedAccess]
    void OnChangeEasingTypeEaseIn()
    {
        ChangeEasingType(EasingType.EaseIn);
    }

    [AuthorizedAccess]
    void OnChangeEasingTypeEaseOut()
    {
        ChangeEasingType(EasingType.EaseOut);
    }

    void ChangeEasingType(EasingType easingType)
    {
        foreach (var demo in this.demos)
        {
            demo.Tween.EasingType = easingType;
            demo.Tween.Rewind();
        }
    }

#endregion
}
