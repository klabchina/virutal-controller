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
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class ExampleSpringShakeController : ExampleSceneBase
    {
        public Text buttonText;
        public ExampleSpringShakeVector3 ExampleSpringShakeVector3;
        public ExampleSpringShakeSingle ExampleSpringShakeSingle;
        public ExampleSpringShakeVector3 ExampleSpringShakeVector3Enveloped;
        public ExampleSpringShakeSingle ExampleSpringShakeSingleEnveloped;

        IMovement shake;
        IMovement shakeEnveloped;

        void Start()
        {
            OnToggleChanged(1);
        }

        [AuthorizedAccess]
        void OnClickButton()
        {
            switch (shake.State)
            {
                case TweenState.None:
                case TweenState.Complete:
                case TweenState.Done:
                case TweenState.ForceComplete:
                case TweenState.Idle:
                    shake.Start();
                    shakeEnveloped.Start();
                    break;
                case TweenState.Paused:
                    shake.Resume();
                    shakeEnveloped.Resume();
                    break;
                case TweenState.Working:
                    shake.Pause();
                    shakeEnveloped.Pause();
                    break;
            }
        }

        [AuthorizedAccess]
        void OnToggleChanged(int index)
        {
            switch (index)
            {
                case 0:
                    shake = ExampleSpringShakeVector3.shake;
                    shakeEnveloped = ExampleSpringShakeVector3Enveloped.shake;
                    ExampleSpringShakeVector3.Dimension = 1;
                    ExampleSpringShakeVector3Enveloped.Dimension = 1;
                    break;
                case 1:
                    shake = ExampleSpringShakeVector3.shake;
                    shakeEnveloped = ExampleSpringShakeVector3Enveloped.shake;
                    ExampleSpringShakeVector3.Dimension = 2;
                    ExampleSpringShakeVector3Enveloped.Dimension = 2;
                    break;
                case 2:
                    shake = ExampleSpringShakeVector3.shake;
                    shakeEnveloped = ExampleSpringShakeVector3Enveloped.shake;
                    ExampleSpringShakeVector3.Dimension = 3;
                    ExampleSpringShakeVector3Enveloped.Dimension = 3;
                    break;
                case 3:
                    shake = ExampleSpringShakeSingle.shake;
                    shakeEnveloped = ExampleSpringShakeSingleEnveloped.shake;
                    break;
            }
        }
    }
}
