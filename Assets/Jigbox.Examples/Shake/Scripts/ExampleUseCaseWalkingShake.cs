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
using Jigbox.Shake;
using Jigbox.Tween;

public class ExampleUseCaseWalkingShake : MonoBehaviour
{
    SpringShakeVector3 shake;

    public float amplitude = 3;
    public float frequency = 3.5f;
    public float duration = 1.7f;
    public AnimationCurve envelopeCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

    void Awake()
    {
        shake = new SpringShakeVector3
        {
            Wave =
            {
                Amplitude = amplitude,
                Frequency = frequency,
                EnvelopeCurve = envelopeCurve
            },
            XAxis = true,
            YAxis = true,
            Origin = transform.rotation.eulerAngles,
            AngleRandomness = 50,
            LoopMode = LoopMode.Restart,
            LoopCount = 0,
            Duration = duration
        };
        shake.OnUpdate(_ =>
        {
            Debug.Log(shake.Value);
            transform.rotation = Quaternion.Euler(shake.Value);
        });
    }

    void Start()
    {
        shake.Start();
    }
}
