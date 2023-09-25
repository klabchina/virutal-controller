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
using UnityEngine.UI;
using Jigbox.Shake;

namespace Jigbox.Examples
{
    public class ExampleSpringShakeSingle : MonoBehaviour
    {
        public AnimationCurve envelopeCurve;
        public Text buttonText;
        public SpringShakeSingle shake;

        void Awake()
        {
            var duration = 3;
            shake = new SpringShakeSingle
            {
                Duration = duration,
                Wave =
                {
                    Amplitude = 3,
                    Frequency = 5,
                    EnvelopeCurve = envelopeCurve
//                    EnvelopeFunc = a => Mathf.Sin(a * Mathf.PI)
                },
            };
            var pos = gameObject.transform.localPosition;
            shake.OnUpdate(_ =>
            {
                gameObject.transform.localPosition = new Vector3(pos.x + shake.Value, pos.y, pos.z);
            });
            shake.OnComplete(_ =>
            {
                buttonText.text = "Start";
            });
            shake.OnPause(_ =>
            {
                buttonText.text = "Resume";
            });
            shake.OnResume(_ =>
            {
                buttonText.text = "Pause";
            });
            shake.OnStart(_ =>
            {
                buttonText.text = "Pause";
            });
        }
    }
}
