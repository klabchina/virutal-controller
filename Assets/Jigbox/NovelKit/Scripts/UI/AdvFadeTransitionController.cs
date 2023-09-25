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
using Jigbox.Tween;

namespace Jigbox.NovelKit
{
    public class AdvFadeTransitionController : MonoBehaviour
    {
#region properties
        
        /// <summary>自身の画像</summary>
        [SerializeField]
        protected MaskableGraphic image;

        /// <summary>アルファのトランジション用Tween</summary>
        protected TweenSingle alphaTween = new TweenSingle();

#endregion

#region public methods

        /// <summary>
        /// フェードに使用する色を指定します。
        /// </summary>
        /// <param name="color">フェードの色</param>
        public void SetColor(Color color)
        {
            image.color = color;
        }

        /// <summary>
        /// フェードアウトを行います。
        /// </summary>
        /// <param name="time">トランジションの時間</param>
        public virtual void FadeOut(float time)
        {
            gameObject.SetActive(true);
            alphaTween.Complete();
            if (time > 0.0f)
            {
                alphaTween.Begin = 0.0f;
                alphaTween.Final = 1.0f;
                alphaTween.Duration = time;
                alphaTween.Start();
            }
            else
            {
                Color color = image.color;
                color.a = 1.0f;
                image.color = color;
            }
        }

        /// <summary>
        /// フェードインを行います。
        /// </summary>
        /// <param name="time">トランジションの時間</param>
        public virtual void FadeIn(float time)
        {
            alphaTween.Complete();
            if (time > 0.0f)
            {
                alphaTween.Begin = image.color.a;
                alphaTween.Final = 0.0f;
                alphaTween.Duration = time;
                alphaTween.OnComplete(OnCompleteFadeIn);
                alphaTween.Start();
            }
            else
            {
                Color color = image.color;
                color.a = 0.0f;
                image.color = color;
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// フェードインが終了した際に呼び出されます。
        /// </summary>
        /// <param name="tween"></param>
        protected void OnCompleteFadeIn(ITween<float> tween)
        {
            gameObject.SetActive(false);
            alphaTween.RemoveOnComplete(OnCompleteFadeIn);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            image.raycastTarget = false;
            gameObject.SetActive(false);

            alphaTween.OnUpdate(tween =>
            {
                Color color = image.color;
                color.a = tween.Value;
                image.color = color;
            });
        }

        protected virtual void OnDisable()
        {
            alphaTween.Complete();
        }

#endregion
    }
}
