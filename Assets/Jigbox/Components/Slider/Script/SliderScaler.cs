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

namespace Jigbox.Components
{
    public class SliderScaler : MonoBehaviour
    {
#region properties

        /// <summary>対象となるSlider</summary>
        [HideInInspector]
        [SerializeField]
        protected Slider target;

        /// <summary>対象となるSliderのTransform</summary>
        protected Transform targetTransform;

        /// <summary>対象となるSliderのスケール値</summary>
        protected Vector3 lastScale = Vector3.one;

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            if (target == null)
            {
                // 参照がない場合は更新しないように非アクティブ化する
                enabled = false;
#if UNITY_EDITOR
                Debug.LogWarning("SliderScaler : Target slider not found!");
#endif
                return;
            }

            targetTransform = target.transform;
            lastScale = targetTransform.lossyScale;
        }

        protected virtual void Update()
        {
            // ワールド空間におけるスケール状態が変更されたら、
            // Sliderに対して再計算を走らせる
            if (targetTransform.lossyScale != lastScale)
            {
                target.RecalculateView();
                lastScale = targetTransform.lossyScale;
            }
        }

#endregion
    }
}
