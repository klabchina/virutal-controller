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

using Jigbox.Delegatable;
using Jigbox.Tween;
using UnityEngine;

namespace Jigbox.Components
{
    public abstract class PeriodicMovementBase<TPeriodicMovement> : MonoBehaviour where TPeriodicMovement : IPeriodicMovement
    {
#region inner classes, enum, and structs

        /// <summary>
        /// Tweenが完了した際のデリゲート型
        /// </summary>
        public class TweenCompleteDelegate : EventDelegate<TPeriodicMovement>
        {
            public TweenCompleteDelegate(Callback callback) : base(callback)
            {
            }
        }
        
#endregion

#region properties

#pragma warning disable 414
        /// <summary>Inpsector上での初期化が行われているかどうか。この変数はinspectorから利用するためのものです。それ以外からは利用しないでください。</summary>
        [HideInInspector]
        [SerializeField]
        bool hasBeenInitialized = false;
#pragma warning restore 414

        /// <summary>生成と同時に実行を開始するかどうか</summary>
        [HideInInspector]
        [SerializeField]
        bool playOnStart = false;

        /// <summary>Tweenが完了した際に呼び出されるコールバック</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onComplete = new DelegatableList();

        /// <summary>Tweenが完了した際に呼び出されるコールバック</summary>
        public DelegatableList OnComplete { get { return onComplete; } }

        /// <summary>Tweenのインタフェース</summary>
        public abstract TPeriodicMovement PeriodicMovement { get; }

#endregion

#region public methods

        /// <summary>
        /// Tweenが完了した際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(ITween)の関数</param>
        public void AddCompleteEvent(TweenCompleteDelegate.Callback callback)
        {
#if UNITY_EDITOR
            if (PeriodicMovement.LoopMode != LoopMode.NoLoop)
            {
                Debug.LogWarning("TweenBase.AddCompleteEvent : Not call this event when tween is enabled to loop!", gameObject);
            }
#endif
            onComplete.Add(new TweenCompleteDelegate(callback));
        }

#endregion

#region protected methods

        /// <summary>
        /// Tweenが完了した際に呼び出されます。
        /// </summary>
        /// <param name="tween">Tween</param>
        protected void OnCompleteTween(TPeriodicMovement tween)
        {
            if (onComplete.Count > 0)
            {
                onComplete.Execute(tween);
            }
        }

#endregion

#region override unity methods

        protected virtual void Start()
        {
            if (playOnStart)
            {
                PeriodicMovement.Start();
            }
        }

        protected virtual void OnDestroy()
        {
            PeriodicMovement.Kill();
        }

#endregion
    }
}
