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

using Jigbox.Shake;
using UnityEngine;

public class ExampleWaveShakeComponent : MonoBehaviour
{
    [SerializeField]
    public WaveShake shake;

    [SerializeField]
    float amplitude = 0.0f;

    [SerializeField]
    float frequency = 0.0f;

    void Awake()
    {
        shake = new WaveShake()
        {
            Wave =
            {
                Amplitude = amplitude,
                Frequency = frequency
            },
            FollowTimeScale = true
        };
        var origRotEuler = transform.localRotation.eulerAngles;
        shake.OnUpdate(_ =>
        {
            transform.localRotation = Quaternion.Euler(origRotEuler.x + shake.Value, origRotEuler.y, origRotEuler.z + shake.Value);
        });
        shake.OnComplete(_ =>
        {
            transform.localRotation = Quaternion.Euler(origRotEuler);
        });
    }
}
