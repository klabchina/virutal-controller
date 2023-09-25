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
using System.Collections.Generic;
using Jigbox.Examples;

namespace Jigbox.Example
{
    public sealed class ExamplePositionTransformUtilsController : ExampleSceneBase
    {
#region constants

        [SerializeField]
        List<GameObject> worldToUISamples = new List<GameObject>();

        [SerializeField]
        List<GameObject> UIToWorldSamples = new List<GameObject>();

        bool isShowWorldToUI = true;

#endregion

#region private methods

        [AuthorizedAccess]
        void OnSwitch()
        {
            isShowWorldToUI = !isShowWorldToUI;
            if (isShowWorldToUI)
            {
                foreach (GameObject gameObject in worldToUISamples)
                {
                    gameObject.SetActive(true);
                }
                foreach (GameObject gameObject in UIToWorldSamples)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject gameObject in worldToUISamples)
                {
                    gameObject.SetActive(false);
                }
                foreach (GameObject gameObject in UIToWorldSamples)
                {
                    gameObject.SetActive(true);
                }
            }
        }

#endregion
    }
}
