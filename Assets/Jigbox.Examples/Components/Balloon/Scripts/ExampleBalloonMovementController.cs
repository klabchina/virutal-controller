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

using System.Collections;
using System.Collections.Generic;
using Jigbox.Components;
using UnityEngine;

namespace Jigbox.Examples
{
    public class ExampleBalloonMovementController : MonoBehaviour
    {
        [SerializeField]
        Balloon balloon = null;

        [SerializeField]
        List<RectTransform> movementTargets = new List<RectTransform>();

        [SerializeField]
        float movementLerpRate = 0.1f;

        int currentTargetIndex;

        void Start()
        {
            balloon.BasePositionRectTransform = transform as RectTransform;
            balloon.Open();
        }

        void LateUpdate()
        {
            var nextPosition = Vector2.Lerp(transform.position, movementTargets[currentTargetIndex].position,
                movementLerpRate);

            transform.position = nextPosition;

            var distance = Vector2.Distance(transform.position, movementTargets[currentTargetIndex].position);

            if (distance <= 0.5f)
            {
                currentTargetIndex++;

                if (currentTargetIndex > movementTargets.Count - 1)
                {
                    currentTargetIndex = 0;
                }
            }

            balloon.BasePositionRectTransform = transform as RectTransform;
            balloon.UpdateLayout();
        }
    }
}
