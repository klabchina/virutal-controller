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
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public class PopupView : MonoBehaviour, IPopupTransitionHandler, IPopupCloser
    {
#region inner classes, enum, and structs

        protected class PopupStack
        {
            /// <summary>ポップアップのリスト</summary>
            protected List<PopupBase> list = new List<PopupBase>();

            /// <summary>要素数</summary>
            public int Count { get { return list.Count; } }

            /// <summary>
            /// 要素を追加します。
            /// </summary>
            /// <param name="popup">ポップアップ</param>
            public void Push(PopupBase popup)
            {
                list.Insert(0, popup);
            }

             /// <summary>
             /// 要素を取り出します。
             /// </summary>
             /// <returns></returns>
            public PopupBase Pop()
            {
                if (list.Count == 0)
                {
                    return null;
                }

                PopupBase popup = list[0];
                list.RemoveAt(0);
                return popup;
            }

            /// <summary>
            /// 現在先頭にある要素を返します。
            /// </summary>
            /// <returns></returns>
            public PopupBase Peek()
            {
                return list.Count > 0 ? list[0] : null;
            }

            /// <summary>
            /// 要素をクリアします。
            /// </summary>
            public void Clear()
            {
                list.Clear();
            }

            /// <summary>
            /// 先頭の要素からインデックス分ずらした位置にある要素を取得します。
            /// </summary>
            /// <param name="index">インデックス</param>
            /// <returns></returns>
            public PopupBase Get(int index)
            {
                if (list.Count <= index)
                {
                    return null;
                }
                return list[index];
            }
        }

        [Serializable]
        public class InputBlocker
        {
            /// <summary>最前面のポップアップより後ろへの入力をブロックするオブジェクト</summary>
            [SerializeField]
            protected GameObject back;

            /// <summary>最前面のポップアップより後ろへの入力をブロックするオブジェクト</summary>
            public GameObject Back { get { return back; } }

            /// <summary>トランジション中の入力をブロックするオブジェクト</summary>
            [SerializeField]
            protected GameObject transition;

            /// <summary>トランジション中の入力をブロックするオブジェクト</summary>
            public GameObject Transition { get { return transition; } }
        }

#endregion

#region constants

        /// <summary>エスケープキー(Androidのバックキー)が押された際の通知の優先度のデフォルト値</summary>
        public static readonly int BackKeyNoticePriorityDefault = 100;

#endregion

#region properties

        /// <summary>ポップアップを配置するオブジェクト</summary>
        [SerializeField]
        protected GameObject popupContainer;

        /// <summary>ポップアップの状態などに応じて不要な入力をブロックするためのオブジェクト</summary>
        [SerializeField]
        protected InputBlocker inputBlocker;

        /// <summary>エスケープキー(Androidのバックキー)の通知優先度の基準値</summary>
        [SerializeField]
        protected int backKeyNoticePriorityBase = BackKeyNoticePriorityDefault;

        /// <summary>Viewが属するシーン名</summary>
        public string BelongSceneName { get; protected set; }

        /// <summary>Viewが破棄された際のコールバック</summary>
        protected Action<string> onDestory = null;

        /// <summary>Viewが破棄された際のコールバック</summary>
        public Action<string> OnDestoryCallback { get { return onDestory; } set { onDestory = value; } }

        /// <summary>未表示のポップアップの表示要求</summary>
        protected Queue<PopupOrder> orders = new Queue<PopupOrder>();

        /// <summary>現在表示されているポップアップ</summary>
        protected PopupStack popups = new PopupStack();

        /// <summary>表示しようとしてトランジションの終了を待っている状態のポップアップの表示要求</summary>
        protected Queue<PopupOrder> standbyOrders = new Queue<PopupOrder>();

        /// <summary>ポップアップを破棄するためのディスポーザ</summary>
        protected IPopupDisposer disposer = null;

        /// <summary>全てのポップアップを閉じるかどうか</summary>
        protected bool shouldCloseAll = false;

        /// <summary>Escキーの通知優先度の実数値</summary>
        protected virtual int backKeyNoticePriority
        {
            get { return backKeyNoticePriorityBase + PopupCount; }
        }

        /// <summary>現在出ているポップアップの数</summary>
        public virtual int PopupCount
        {
            get { return popups.Count; }
        }

#endregion

#region public methods

        /// <summary>
        /// <para>ポップアップを開きます。</para>
        /// <para>既に開いているポップアップが存在する場合は、最前面に表示されます。</para>
        /// </summary>
        /// <param name="order"></param>
        public virtual void Open(PopupOrder order)
        {
            if (inputBlocker.Transition.activeSelf)
            {
                standbyOrders.Enqueue(order);
            }
            else
            {
                OpenPopup(order);
            }
        }

        /// <summary>
        /// <para>ポップアップを開きます。</para>
        /// <para>既に開いているポップアップが存在する場合は、ポップアップが閉じてから表示されます。</para>
        /// </summary>
        /// <param name="order"></param>
        public virtual void OpenQueue(PopupOrder order)
        {
            if (orders.Count == 0 && popups.Count == 0)
            {
                OpenPopup(order);
            }
            else
            {
                orders.Enqueue(order);
            }
        }

        /// <summary>
        /// 開いている最前面のポップアップを閉じます。
        /// </summary>
        public virtual void Close()
        {
            // トランジション中は受け付けない
            if (inputBlocker.Transition.activeSelf)
            {
#if UNITY_EDITOR
                Debug.LogWarning("PopupView.Close : Can't close when transitioning!");
#endif
                return;
            }
            // 開くためにトランジションの終了待ちをしているポップアップがあると
            // 状態に矛盾が生まれるので受け付けない
            if (standbyOrders.Count > 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning("PopupView.Close : Can't close, it is going to open other popup!");
#endif
                return;
            }

            if (popups.Count > 0)
            {
                popups.Peek().Close();
            }
        }

        /// <summary>
        /// 開いているポップアップを全て閉じます。
        /// </summary>
        public virtual void CloseAll()
        {
            // トランジション中は受け付けない
            if (inputBlocker.Transition.activeSelf)
            {
#if UNITY_EDITOR
                Debug.LogWarning("PopupView.CloseAll : Can't close when transitioning!");
#endif
                return;
            }
            // 開くためにトランジションの終了待ちをしているポップアップがあると
            // 状態に矛盾が生まれるので受け付けない
            if (standbyOrders.Count > 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning("PopupView.CloseAll : Can't close, it is going to open other popup!");
#endif
                return;
            }

            if (popups.Count > 0)
            {
                shouldCloseAll = true;
                popups.Peek().Close();
            }
        }

        /// <summary>
        /// 未表示のポップアップの表示要求をクリアします。
        /// </summary>
        public virtual void ClearOrder()
        {
            orders.Clear();
        }

        /// <summary>
        /// ポップアップを破棄するためのディスポーザを設定します。
        /// </summary>
        /// <param name="disposer">ポップアップを破棄するためのディスポーザ</param>
        public void SetDisposer(IPopupDisposer disposer)
        {
            this.disposer = disposer;
        }

        /// <summary>
        /// ポップアップを開くトランジションを開始する際に呼び出されます。
        /// </summary>
        public virtual void OnBeginOpen()
        {
            inputBlocker.Transition.SetActive(true);
        }

        /// <summary>
        /// ポップアップを開くトランジションが完了した際に呼び出されます。
        /// </summary>
        public virtual void OnCompleteOpen()
        {
            inputBlocker.Transition.SetActive(false);

            if (standbyOrders.Count > 0)
            {
                OpenPopup(standbyOrders.Dequeue());
            }
        }

        /// <summary>
        /// ポップアップを閉じるトランジションを開始する際に呼び出されます。
        /// </summary>
        public virtual void OnBeginClose()
        {
            inputBlocker.Transition.SetActive(true);

            // 最前面のポップアップは閉じようとしているので
            // その一つ後ろにあるポップアップの後ろに入力ブロック用のオブジェクトを移動させる
            PopupBase popup = popups.Get(1);
            int siblingIndex = popup != null ? popup.transform.GetSiblingIndex() : 0;
            inputBlocker.Back.transform.SetSiblingIndex(siblingIndex);
        }

        /// <summary>
        /// ポップアップを閉じるトランジションが完了した際に呼び出されます。
        /// </summary>
        public virtual void OnCompleteClose()
        {
            PopupBase popup = popups.Pop();
            disposer.Dispose(popup);
            
            // 全て閉じる場合、ポップアップが残っている間は繰り返す
            if (shouldCloseAll && popups.Count > 0)
            {
                popups.Peek().Close();
                return;
            }

            inputBlocker.Transition.SetActive(false);
            shouldCloseAll = false;

            if (standbyOrders.Count > 0)
            {
                OpenPopup(standbyOrders.Dequeue());
                return;
            }

            if (popups.Count == 0)
            {
                if (orders.Count > 0)
                {
                    OpenPopup(orders.Dequeue());
                }
                else
                {
                    OnCloseAllPopup();
                }
            }
        }

        /// <summary>
        /// ポップアップが現在開かれる予定かどうかを返します
        /// </summary>
        /// <returns>ポップアップの開閉状態</returns>
        public virtual bool IsStandbyPopup()
        {
            return orders.Count > 0 || standbyOrders.Count > 0;
        }

        /// <summary>
        /// ポップアップが現在開かれているかどうかを返します
        /// </summary>
        /// <returns>ポップアップが開かれているかどうか</returns>
        public virtual bool IsOpenPopup()
        {
            return popups.Count > 0;
        }

#endregion

#region protected methods

        /// <summary>
        /// ポップアップを開きます。
        /// </summary>
        /// <param name="order">ポップアップの表示要求</param>
        protected virtual void OpenPopup(PopupOrder order)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            PopupBase popup = order.Generate();
            popups.Push(popup);

            popup.transform.SetParent(popupContainer.transform, false);
            popup.transform.SetAsLastSibling();
            // ポップアップより後ろへの入力をブロックするためのオブジェクトの位置を
            // 今開いた最前面に位置するポップアップの後ろにする
            inputBlocker.Back.transform.SetSiblingIndex(Mathf.Max(0, popup.transform.GetSiblingIndex() - 1));

            popup.Init(this, order, backKeyNoticePriority);
            popup.Open();
        }

        /// <summary>
        /// Viewで管理しているポップアップが全て閉じられた時に呼ばれます。
        /// </summary>
        protected virtual void OnCloseAllPopup()
        {
            gameObject.SetActive(false);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            BelongSceneName = SceneManager.GetActiveScene().name;
            inputBlocker.Transition.SetActive(false);
        }

        protected virtual void OnDestroy()
        {
            if (onDestory != null)
            {
                onDestory(BelongSceneName);
            }
        }

#endregion
    }
}
