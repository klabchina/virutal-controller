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

namespace Jigbox.HUD
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class ObjectStalker : MonoBehaviour
    {
        [SerializeField]
        Transform target;

        [SerializeField]
        Vector3 offset = Vector3.zero;

        [SerializeField]
        Camera uiCamera;

        RectTransform selfRect;
        RectTransform parentRect;

#region UnityMethod

        void Start()
        {
            selfRect = GetComponent<RectTransform>();
            parentRect = (RectTransform) selfRect.parent;

            if (uiCamera == null)
            {
                FindCanvasCamera();
            }
        }

        void Update()
        {
            UpdateSelfPosition();
        }

#endregion

#region Public Mothod

        public void SetHudTarget(Transform target)
        {
            this.target = target;
        }

#endregion

#region Private Method

        void FindCanvasCamera()
        {
            var canvases = GetComponentsInParent<Canvas>();
            foreach (var canvas in canvases)
            {
                if (canvas.isRootCanvas)
                {
                    uiCamera = canvas.worldCamera;
                }
            }
        }

        void UpdateSelfPosition()
        {
            if (target != null && selfRect != null && parentRect != null && uiCamera != null)
            {
                var screenPosition = Camera.main.WorldToScreenPoint(target.position + offset);
                Vector2 localPosition;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform) selfRect.parent,
                    screenPosition,
                    uiCamera,
                    out localPosition
                );
                selfRect.localPosition = localPosition;
            }
        }

#endregion

    }
}
