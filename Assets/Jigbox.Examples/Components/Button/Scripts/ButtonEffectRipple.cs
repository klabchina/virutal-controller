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
using Jigbox.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class ButtonEffectRipple : MonoBehaviour, IAdvancedButtonTransitionEffect
    {
        [SerializeField] Image circle;

        public void OnTransition(AdvancedButtonTransition transition, InputEventType type)
        {
            if (type == InputEventType.OnSelect)
            {
                // 自身のサイズを親のサイズと同じに設定
                var self = GetComponent<RectTransform>();
                self.anchorMin = Vector2.zero;
                self.anchorMax = Vector2.one;
                self.sizeDelta = Vector2.zero;

                // クリック位置にエフェクトを移動
                Vector3 position;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    circle.rectTransform,
#if UNITY_EDITOR || UNITY_STANDALONE
                    InputWrapper.GetMousePosition(),
#else
                    InputWrapper.GetTouchPosition(0),
#endif
                    circle.canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : circle.canvas.worldCamera,
                    out position
                );
                circle.transform.position = position;
            }
            
            // ボタンの各種イベントが Transition にも通知されていることを確認
            Debug.Log("入力イベントタイプ: "+type);
        }

        public void OnStopTransition(AdvancedButtonTransition transition)
        {
        }
    }
}
