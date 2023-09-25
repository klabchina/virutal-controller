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
using Jigbox.Tween;

namespace Jigbox.Components
{
    public class BasicPopupTransition : PopupTransitionBase
    {
#region constants

        /// <summary>トランジションの時間のデフォルト値</summary>
        protected static readonly float DefaultDuration = 0.15f;

#endregion

#region properties

        /// <summary>トランジションの時間</summary>
        [SerializeField]
        protected float duration = DefaultDuration;

        /// <summary>LacalScale更新用のTween</summary>
        protected TweenSingle scaleTween = new TweenSingle();

#endregion

#region public methods

        /// <summary>
        /// ポップアップを開きます。
        /// </summary>
        public override void Open()
        {
            scaleTween.Begin = 0.0f;
            scaleTween.Final = 1.0f;
            scaleTween.RemoveAllOnComplete();
            scaleTween.OnComplete(OnCompleteOpen);

            NotifyOnBeginOpen();

            scaleTween.Start();
        }

        /// <summary>
        /// ポップアップを閉じます。
        /// </summary>
        public override void Close()
        {
            scaleTween.Begin = 1.0f;
            scaleTween.Final = 0.0f;
            scaleTween.RemoveAllOnComplete();
            scaleTween.OnComplete(OnCompleteClose);

            NotifyOnBeginClose();

            scaleTween.Start();
        }

#endregion

#region protected methods

        /// <summary>
        /// Tweenの値が変更された際に呼び出されます。
        /// </summary>
        /// <param name="tween"></param>
        protected virtual void OnUpdateTween(ITween<float> tween)
        {
            float value = tween.Value;
            transform.localScale = new Vector3(value, value, value);
        }

        /// <summary>
        /// ポップアップを開くトランジションが完了した際に呼び出されます。
        /// </summary>
        /// <param name="tween"></param>
        protected virtual void OnCompleteOpen(ITween<float> tween)
        {
            NotifyOnCompleteOpen();
        }

        /// <summary>
        /// ポップアップを閉じるトランジションが完了した際に呼び出されます。
        /// </summary>
        /// <param name="tween"></param>
        protected virtual void OnCompleteClose(ITween<float> tween)
        {
            NotifyOnCompleteClose();
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            scaleTween.Duration = duration;
            scaleTween.OnUpdate(OnUpdateTween);
        }

#endregion
    }
}
