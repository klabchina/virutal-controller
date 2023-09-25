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
using Jigbox.SceneTransition;

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public abstract class PopupBase : MonoBehaviour, IBackKeyNoticeTarget, IPopupTransitionHandler
    {
#region properties

        /// <summary>トランジションコンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected PopupTransitionBase transition;

        /// <summary>ポップアップを開くトランジションを開始する際のコールバック</summary>
        protected Action<PopupBase> onBeginOpen = null;

        /// <summary>ポップアップを開くトランジションが完了した際のコールバック</summary>
        protected Action<PopupBase> onCompleteOpen = null;

        /// <summary>ポップアップを閉じるトランジションを開始する際のコールバック</summary>
        protected Action<PopupBase> onBeginClose = null;

        /// <summary>ポップアップを閉じるトランジションが完了した際のコールバック</summary>
        protected Action<PopupBase> onCompleteClose = null;

        /// <summary>エスケープキーが(Androidのバックキー)が押された際のコールバック</summary>
        protected Action<PopupBase> onEscape = null;

        /// <summary>通知の優先度</summary>
        public virtual int Priority { get; protected set; }

        /// <summary>通知が有効かどうか</summary>
        public virtual bool Enable { get { return true; } }

        /// <summary>ポップアップを閉じるモジュール</summary>
        protected IPopupCloser closer = null;

        /// <summary>トランジション中かどうか</summary>
        protected bool isTransitioning = false;

#endregion

#region public methods

        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="view">PopupView</param>
        /// <param name="order">ポップアップの表示要求</param>
        /// <param name="priority">エスケープキー(Androidのバックキー)の通知優先度/param>
        public virtual void Init(PopupView view, PopupOrder order, int priority)
        {
            closer = view;
            transition.SetHandler(this, view);

            onBeginOpen = order.OnBeginOpen;
            onCompleteOpen = order.OnCompleteOpen;
            onBeginClose = order.OnBeginClose;
            onCompleteClose = order.OnCompleteClose;
            onEscape = order.OnEscape;

            Priority = priority;

            if (order.OnInit != null)
            {
                order.OnInit(this);
            }
        }

        /// <summary>
        /// ポップアップを開きます。
        /// </summary>
        public virtual void Open()
        {
            if (isTransitioning)
            {
                return;
            }

            transition.Open();
        }

        /// <summary>
        /// ポップアップを閉じます。
        /// </summary>
        public virtual void Close()
        {
            if (isTransitioning)
            {
                return;
            }

            transition.Close();
        }

        /// <summary>
        /// エスケープキー(Androidのバックキー)が押された際に呼び出されます。
        /// </summary>
        public virtual void OnEscape()
        {
            if (onEscape != null)
            {
                onEscape(this);
            }
            else
            {
                closer.Close();
            }
        }

        /// <summary>
        /// ポップアップを開くトランジションを開始する際に呼び出されます。
        /// </summary>
        public virtual void OnBeginOpen()
        {
            isTransitioning = true;
            if (onBeginOpen != null)
            {
                onBeginOpen(this);
            }
        }

        /// <summary>
        /// ポップアップを開くトランジションが完了した際に呼び出されます。
        /// </summary>
        public virtual void OnCompleteOpen()
        {
            isTransitioning = false;
            if (onCompleteOpen != null)
            {
                onCompleteOpen(this);
            }
        }

        /// <summary>
        /// ポップアップを閉じるトランジションを開始する際に呼び出されます。
        /// </summary>
        public virtual void OnBeginClose()
        {
            isTransitioning = true;
            if (onBeginClose != null)
            {
                onBeginClose(this);
            }
        }

        /// <summary>
        /// ポップアップを閉じるトランジションが完了した際に呼び出されます。
        /// </summary>
        public virtual void OnCompleteClose()
        {
            isTransitioning = false;
            if (onCompleteClose != null)
            {
                onCompleteClose(this);
            }
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            BackKeyManager.Instance.RegisterNoticeTarget(this);
        }

        protected virtual void OnDisable()
        {
            BackKeyManager.Instance.UnregisterNoticeTarget(this);
        }

#endregion
    }
}
