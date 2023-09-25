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
    /// <summary>
    /// CarouselTransitionの基本的な挙動を実装したクラス
    /// </summary>
    public class BasicCarouselTransition : CarouselTransitionBase
    {
#region constants

        // Tweenの各種Default値を定義します
        protected readonly Tween.MotionType DefaultMotionType = Jigbox.Tween.MotionType.Sine;
        protected readonly Tween.EasingType DefaultEasingType = Jigbox.Tween.EasingType.EaseIn;
        protected readonly float DefaultDuration = 0.2f;

#endregion

#region properties

        /// <summary>
        /// OnUpdateTween時に差分計算用に必要な変数
        /// </summary>
        protected Vector3 lastTweenPosition;

#endregion

#region public methods

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="carouselComponent">Carousel</param>
        /// <param name="viewBase">CarouselViewBase</param>
        public override void Initialize(Carousel carouselComponent, CarouselViewBase viewBase)
        {
            // 本体側のRelayout時にもInitializeが呼ばれるため、コールバックの重複を避けるため一度Removeする
            Tween.RemoveOnUpdate(OnUpdateTween);
            Tween.RemoveOnComplete(OnCompleteTween);

            carousel = carouselComponent;
            view = viewBase;

            // デフォルト挙動の設定
            Tween.MotionType = DefaultMotionType;
            Tween.EasingType = DefaultEasingType;
            Tween.Duration = DefaultDuration;
            Tween.OnComplete(OnCompleteTween);
            Tween.OnUpdate(OnUpdateTween);
        }

        /// <summary>
        /// Contentの位置を現在位置から指定位置へ移動します
        /// </summary>
        /// <param name="to">移動量</param>
        public override void MoveContent(Vector3 to)
        {
            // Transitionを停止する
            StopTransition();

            if (carousel == null || carousel.CellLayoutGroup == null)
            {
                return;
            }

            lastTweenPosition = Vector3.zero;
            Tween.Begin = Vector3.zero;
            Tween.Final = to;
            Tween.Start();
        }

        /// <summary>
        /// トランジションを停止させます
        /// </summary>
        public override void StopTransition()
        {
            Tween.Kill();
        }

#endregion

#region protected methods

        /// <summary>
        /// Tweenが完了した時に呼ばれます
        /// </summary>
        /// <param name="tw"></param>
        protected virtual void OnCompleteTween(Tween.ITween<Vector3> tw)
        {
            if (carousel != null)
            {
                carousel.OnCompleteTransition();
            }
        }
        /// <summary>
        /// TweenのOnUpdate時に呼ばれます
        /// </summary>
        /// <param name="tw">Jigbox.Tween.Itween<Vector3></param>
        protected virtual void OnUpdateTween(Tween.ITween<Vector3> tw)
        {
            var diff = tw.Value - lastTweenPosition;
            // Indexのoffsetを反映
            carousel.OnMoveFromTransition(diff);
            lastTweenPosition = tw.Value;
        }

#endregion
    }
}
