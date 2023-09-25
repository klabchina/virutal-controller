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
    [RequireComponent(typeof(BasicButton))]
    public sealed class ButtonTransitionColorExample : ButtonTransitionBase
    {
#region properties

        /// <summary>情報を取得するボタンコンポーネント</summary>
        ButtonBase buttonComponent;

        /// <summary>Tween</summary>
        Tween.TweenColor colorTween = new Tween.TweenColor();

        /// <summary>現在のカラー</summary>
        Color currentColor;

        /// <summary>押下時のトランジション状態かどうか</summary>
        bool isPressTransition = false;

        /// <summary>押下時のカラー</summary>
        [SerializeField]
        Color pressedColor = new Color(0.8f, 0.8f, 0.8f);

        /// <summary>アニメーション時間</summary>
        [SerializeField]
        float duration = 0.12f;

#endregion

#region public methods

        public override void NoticeAutoUnlock()
        {
            ReleaseTransition();
        }

#endregion

#region protected methods

        protected override void OnSelect()
        {
            PressTransition();
            base.OnSelect();
        }

        protected override void OnDeselect()
        {
            ReleaseTransition();
            base.OnDeselect();
        }

        protected override void StopTransition()
        {
            colorTween.Complete();
        }

#endregion

#region private methods

        /// <summary>
        /// 押下時のトランジションを行います。
        /// </summary>
        void PressTransition()
        {
            if (isPressTransition)
            {
                return;
            }
            isPressTransition = true;

            colorTween.Kill();

            colorTween.Begin = Color.white;
            colorTween.Final = pressedColor;
            colorTween.Duration = duration;

            colorTween.Start();
        }

        /// <summary>
        /// ポインタが離された際のトランジションを行います。
        /// </summary>
        void ReleaseTransition()
        {
            if (!isPressTransition)
            {
                return;
            }
            isPressTransition = false;

            colorTween.Kill();

            colorTween.Begin = currentColor;
            colorTween.Final = Color.white;
            colorTween.Duration = duration;

            colorTween.Start();
        }

#endregion

#region override unity methods

        void Awake()
        {
            buttonComponent = GetComponent<ButtonBase>();
            colorTween.OnUpdate(tween =>
            {
                currentColor = tween.Value;
                buttonComponent.ImageInfo.SetColorMultiply(tween.Value);
            });
        }

#endregion
    }
}
