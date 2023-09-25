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
using UnityEngine;
using Jigbox.Components;
using Jigbox.Tween;

namespace Jigbox.Examples
{
    public class ExampleBalloonChangeController : MonoBehaviour
    {
        [SerializeField]
        Balloon balloon = null;

        [SerializeField]
        List<RectTransform> contents = null;

        [SerializeField]
        float contentChangeInterval = 3.0f;

        int currentIndex = 0;

        void OnEnable()
        {
            balloon.BasePositionRectTransform = transform as RectTransform;

            foreach (var content in contents)
            {
                content.gameObject.SetActive(false);
            }

            contents[currentIndex].gameObject.SetActive(true);

            balloon.Open();

            StartCoroutine(BalloonChangeUpdate());
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator BalloonChangeUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(contentChangeInterval);
                ChangeContent();
            }
        }

        void ChangeContent()
        {
            contents[currentIndex].gameObject.SetActive(false);

            currentIndex++;

            if (currentIndex > contents.Count - 1)
            {
                currentIndex = 0;
            }

            contents[currentIndex].gameObject.SetActive(true);

            balloon.SetBalloonContent(contents[currentIndex]);
        }
    }
}
