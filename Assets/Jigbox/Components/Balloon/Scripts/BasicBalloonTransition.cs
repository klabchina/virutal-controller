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
    /// <summary>
    /// スケーリングを行うバルーンの基本トランジション拡張クラス
    /// </summary>
    public class BasicBalloonTransition : BalloonTransitionBase
    {
#region serializefield

        /// <summary>
        /// 遷移の単位
        /// </summary>
        [SerializeField]
        float duration = 0.12f;

#endregion

#region property

        /// <summary>
        /// スケーリング用のTween
        /// </summary>
        protected Tween.TweenVector3 TweenScale = new TweenVector3();

        /// <summary>
        /// 遷移の単位のアクセサ
        /// </summary>
        public float Duration
        {
            get { return duration; }
            set { duration = Mathf.Max(0.0f, value); }
        }

        protected Vector3 DefaultScale { get; set; }

#endregion

#region protected methods

        protected virtual void SetTween()
        {
            TweenScale.Complete();

            TweenScale = new TweenVector3();

            TweenScale.OnUpdate(tween => transform.localScale = tween.Value);
            TweenScale.Duration = Duration;
        }

#endregion

#region public methods

        public override void SetHandler(IBalloonTransitionHandler handler)
        {
            if (Handler != null)
            {
                return;
            }

            base.SetHandler(handler);

            DefaultScale = transform.localScale;
        }

        public override void OpenTransition()
        {
            SetTween();

            TweenScale.Begin = Vector3.zero;
            TweenScale.Final = DefaultScale;
            TweenScale.OnComplete(_ => NoticeOnCompleteOpen());

            gameObject.SetActive(true);

            NoticeOnBeginOpen();

            TweenScale.Start();
        }

        public override void CloseTransition()
        {
            SetTween();

            TweenScale.Begin = DefaultScale;
            TweenScale.Final = Vector3.zero;
            TweenScale.OnComplete(_ =>
            {
                NoticeOnCompleteClose();
                transform.localScale = DefaultScale;
                gameObject.SetActive(false);
            });

            NoticeOnBeginClose();

            TweenScale.Start();
        }

#endregion
    }
}
