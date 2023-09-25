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
using TweenVector3 = Jigbox.Tween.TweenVector3;

namespace Jigbox.Components
{
    /// <summary>
    /// CarouselのTransitionを司る抽象クラス
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class CarouselTransitionBase : MonoBehaviour
    {
#region properties

        /// <summary>
        /// tween用インスタンス
        /// </summary>
        protected TweenVector3 tween = new TweenVector3();

        /// <summary>
        /// Carousel
        /// </summary>
        protected Carousel carousel;

        /// <summary>
        /// View
        /// </summary>
        protected CarouselViewBase view;

        /// <summary>
        /// tween用インスタンス
        /// </summary>
        public virtual TweenVector3 Tween { get { return tween; } }

#endregion

#region public methods

        /// <summary>
        /// トランジションを停止させます
        /// </summary>
        public abstract void StopTransition();

        /// <summary>
        /// Contentの位置を現在位置から指定位置へ移動します
        /// </summary>
        /// <param name="to">移動量</param>
        public abstract void MoveContent(Vector3 to);

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="carouselComponent">Carousel</param>
        /// <param name="viewBase">CarouselViewBase</param>
        public abstract void Initialize(Carousel carouselComponent, CarouselViewBase viewBase);

#endregion

#region override unity methods

        protected virtual void OnDisable()
        {
            StopTransition();
            if (carousel != null)
            {
                carousel.OnCompleteTransition();
            }
        }

#endregion
    }
}
