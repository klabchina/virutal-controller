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
using System;
using Jigbox.Tween;

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public class ScrollSelectViewTransition : MonoBehaviour
    {
#region constants

        /// <summary>位置の補完にかかる時間のデフォルト値</summary>
        protected static readonly float DefaultDuration = 0.2f;

        /// <summary>Transitionで使う移動量を0とみなすToleranceのデフォルト値</summary>
        protected static readonly float DefaultTolerance = 0.01f;

#endregion

#region Serialize Fields

        /// <summary>Transitionで使う移動量で0とみなす誤差範囲</summary>
        [SerializeField]
        protected float tolerance = DefaultTolerance;

#endregion

#region properties

        /// <summary>スクロールさせているオブジェクト(RectTransform)</summary>
        protected RectTransform content;

        /// <summary>Tween</summary>
        protected TweenVector3 tween = new TweenVector3();

        /// <summary>補完した座標の基準となる位置</summary>
        protected Vector3 basePosition = Vector3.zero;

        /// <summary>Tweenによる補完が完了した際に呼び出されるコールバック</summary>
        protected Action onCompleteAdjust = null;

        /// <summary>位置の補完にかかる時間</summary>
        protected virtual float Duration { get { return DefaultDuration; } }

        /// <summary> Tweenが動作しているかを返します </summary>
        public virtual bool IsWorkingAdjust
        {
            get { return tween.State == TweenState.Working; }
        }

#endregion

#region public methods

        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="content">スクロールさせているオブジェクトの参照</param>
        /// <param name="onCompleteAdjust">Tweenによる補完が完了した際に呼び出されるコールバック</param>
        public virtual void Init(RectTransform content, Action onCompleteAdjust)
        {
            this.content = content;
            tween.Begin = Vector3.zero;
            tween.Duration = Duration;
            tween.OnUpdate(OnUpdate);
            tween.OnComplete(OnComplete);
            this.onCompleteAdjust = onCompleteAdjust;
        }

        /// <summary>
        /// 指定された移動量をDurationの時間をかけて移動します。
        /// 選択セルが正しい位置に来るよう位置補完に使用します。
        /// </summary>
        /// <param name="delta"></param>
        public virtual void Adjust(Vector3 delta)
        {
            Adjust(delta, Duration);
        }

        /// <summary>
        /// 指定された移動量を指定された時間をかけて移動します。
        /// 選択セルが正しい位置に来るよう位置補完に使用します。
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="duration"></param>
        public virtual void Adjust(Vector3 delta, float duration)
        {
            // 先に動かしているTransitoinがある場合は止める
            Stop();
            // 移動距離がない場合はonCompleteAdjustを発火するだけ
            if (Mathf.Abs(delta.x) < tolerance && Mathf.Abs(delta.y) < tolerance)
            {
                if (onCompleteAdjust != null)
                {
                    onCompleteAdjust();
                }

                return;
            }

            basePosition = content.localPosition;
            tween.Duration = duration;
            tween.Final = delta;
            tween.Start();
        }

        /// <summary>
        /// 位置の補完を停止します。
        /// </summary>
        public virtual void Stop()
        {
            tween.Kill();
        }

        /// <summary>
        /// 補完の際の基準位置を更新します。
        /// </summary>
        public virtual void UpdateBasePosition()
        {
            if (tween.State != TweenState.Working)
            {
                return;
            }

            basePosition = content.localPosition - tween.Value;
        }

#endregion

#region protected methods

        /// <summary>
        /// Tweenの更新時に呼び出されます。
        /// </summary>
        /// <param name="tween">Tween</param>
        protected virtual void OnUpdate(ITween<Vector3> tween)
        {
            content.localPosition = basePosition + tween.Value;
        }

        /// <summary>
        /// Tweeenによる補完が完了した際に呼び出されます。
        /// </summary>
        /// <param name="tween">Tween</param>
        protected virtual void OnComplete(ITween<Vector3> tween)
        {
            if (onCompleteAdjust != null)
            {
                onCompleteAdjust();
            }
        }

#endregion
    }
}
