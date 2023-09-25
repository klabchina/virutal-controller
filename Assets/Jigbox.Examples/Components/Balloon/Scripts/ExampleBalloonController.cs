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
using Jigbox.Components;

namespace Jigbox.Examples
{
    public class ExampleBalloonController : ExampleSceneBase
    {
        [SerializeField]
        Balloon balloonPrefab = null;

        [SerializeField]
        BalloonLayer balloonLayerPrefab = null;

        [SerializeField]
        RectTransform autoLayoutArea = null;

        [SerializeField]
        BalloonLayout balloonLayout = BalloonLayout.None;

        [SerializeField]
        bool useInputBlocker = false;

        [SerializeField]
        bool closeOnClickInputBlocker = false;

        protected Balloon balloon;
        BalloonLayer balloonLayer;

        [AuthorizedAccess]
        void OnOpenBalloon()
        {
            balloonLayer = Instantiate(balloonLayerPrefab, transform.parent);

            balloon = Instantiate(balloonPrefab, transform.parent);
            balloon.BalloonLayout = balloonLayout;
            balloon.UseInputBlocker = useInputBlocker;
            balloon.CloseOnClickInputBlocker = closeOnClickInputBlocker;
            balloon.AutoLayoutArea = autoLayoutArea;
            balloon.BalloonLayer = balloonLayer;
            balloon.BasePositionRectTransform = transform as RectTransform;

            balloon.AddBeginOpenEvent((b) => Debug.Log("BeginOpen : " + gameObject.name));
            balloon.AddCompleteOpenEvent((b) => Debug.Log("CompleteOpen : " + gameObject.name));
            balloon.AddBeginCloseEvent((b) => Debug.Log("BeginClose : " + gameObject.name));
            balloon.AddCompleteCloseEvent((b) => Debug.Log("CompleteClose : " + gameObject.name));

            balloon.Open();
        }

        [AuthorizedAccess]
        void OnCloseBalloon()
        {
            balloon.Close();
        }
    }
}
