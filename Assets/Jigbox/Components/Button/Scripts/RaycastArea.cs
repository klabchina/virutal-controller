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

namespace Jigbox.Components
{
#if UNITY_2019_4_OR_NEWER
    [RequireComponent(typeof(CanvasRenderer))]
#endif
    public class RaycastArea : Graphic
    {
#region properties

        /// <summary>RectTransform</summary>
        RectTransform rectTrs;

        /// <summary>RectTransform</summary>
        public RectTransform RectTransform
        {
            get
            {
                if (rectTrs == null)
                {
                    rectTrs = rectTransform;
                }

                return rectTrs;  
            }
        }

        /// <summary>矩形領域の大きさ</summary>
        public Vector2 Size
        {
            get
            {
                return RectTransform.rect.size;
            }
            set
            {
                if (RectTransform.rect.size != value)
                {
                    Vector2 lastSizeDelta = RectTransform.sizeDelta;

                    Vector2 anchorMargin = Vector2.zero;
                    anchorMargin.x = RectTransform.rect.width - RectTransform.sizeDelta.x;
                    anchorMargin.y = RectTransform.rect.height - RectTransform.sizeDelta.y;
                    RectTransform.sizeDelta = value - anchorMargin;

                    Vector2 sizeDeltaOffset = lastSizeDelta - RectTransform.sizeDelta;
                    Vector3 offset = RectTransform.localPosition;

                    offset.x -= (RectTransform.pivot.x - 0.5f) * sizeDeltaOffset.x;
                    offset.y -= (RectTransform.pivot.y - 0.5f) * sizeDeltaOffset.y;

                    RectTransform.localPosition = offset;
                }
            }
        }

        /// <summary>
        /// RaycastTargetを外部から操作可能にするかどうか
        /// デフォルトはtrue
        /// </summary>
        [HideInInspector]
        [SerializeField]
        bool isControllableRaycastTarget = true;

        /// <summary>
        /// RaycastTargetを外部から操作可能にするかどうか
        /// </summary>
        public bool IsControllableRaycastTarget
        {
            get { return isControllableRaycastTarget; }
            set { isControllableRaycastTarget = value; }
        }

        public override bool raycastTarget
        {
            get
            {
                return base.raycastTarget;
            }
            set
            {
                if (IsControllableRaycastTarget)
                {
                    base.raycastTarget = value;
                }
            }
        }

#endregion

#region override unity methods

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        protected override void Awake()
        {
            base.Awake();
            if (color.a != 0.0f)
            {
                color = Color.clear;
            }
        }

#endregion
    }
}
