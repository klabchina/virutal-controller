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
using Jigbox.Shake;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class ExampleSpringShakeVector3 : MonoBehaviour
    {
        public AnimationCurve envelopeCurve;
        public Text buttonText;
        [NonSerialized]
        public SpringShakeVector3 shake;

        public int Dimension
        {
            set
            {
                switch (value)
                {
                    case 1:
                        shake.XAxis = false;
                        shake.YAxis = true;
                        shake.ZAxis = false;
                        break;
                    case 2:
                        shake.XAxis = true;
                        shake.YAxis = true;
                        shake.ZAxis = false;
                        break;
                    case 3:
                        shake.XAxis = true;
                        shake.YAxis = true;
                        shake.ZAxis = true;
                        break;
                }
            }
        }

        void Awake()
        {
            var duration = 3;
            shake = new SpringShakeVector3()
            {
                Wave =
                {
                    Amplitude = 3,
                    Frequency = 5,
                    EnvelopeCurve = envelopeCurve
//                    EnvelopeFunc = a => Mathf.Sin(a * Mathf.PI)
                },
                Duration = duration,
                XAxis = true,
                YAxis = true,
//                ZAxis = true,
                AngleRandomness = 60
            };
            var pos = gameObject.transform.localPosition;
            shake.OnUpdate(_ =>
            {
                gameObject.transform.localPosition = pos + shake.Value;
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
