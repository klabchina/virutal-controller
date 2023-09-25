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
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using Jigbox.Components;
using Jigbox.Delegatable;

namespace Jigbox.Gesture
{
    /// <summary>
    /// ジェスチャーを判別するためのクラス
    /// </summary>
    public abstract class GestureUmpire<T, U, V> : GestureUmpireBehaviour where T : IConvertible where U : IGestureEventData where V : GestureEventHandler<T>
    {
#region properties

        /// <summary>ジェスチャーで処理されるデータ</summary>
        protected U data;

        /// <summary>イベントハンドラ</summary>
        [HideInInspector]
        [SerializeField]
        protected List<V> handlers = new List<V>();

#endregion

#region public methods

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <param name="callback">void Func(void)の関数</param>
        public bool AddEvent(T type, DelegatableObject.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("GestureUmpire.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(callback);
            AddEvent(type, delegatable);

            return true;
        }

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <param name="target">イベントの発行対象</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="args">引数</param>
        /// <returns></returns>
        public bool AddEvent(T type, MonoBehaviour target, string methodName, params object[] args)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("GestureUmpire.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(target, methodName, args);
            AddEvent(type, delegatable);

            return true;
        }

        public abstract bool AddEvent(T type, EventDelegate<U>.Callback callback);

        /// <summary>
        /// イベントハンドラを取得します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <returns>イベントハンドラの取得に成功すればハンドラを返し、失敗すれば<c>null</c>を返します。</returns>
        public V GetHandler(T type)
        {
            foreach (V handler in handlers)
            {
                if (handler.Type.Equals(type))
                {
                    return handler;
                }
            }
            return null;
        }
        
        /// <summary>
        /// イベントハンドラを追加します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        public V AddHandler(T type)
        {
            V handler;
#if UNITY_EDITOR
            handler = GetHandler(type);
            if (handler != null)
            {
                Debug.LogWarning("GestureUmpire.AddHandler : Already exist handler!");
                return handler;
            }
#endif
            handler = CreateHandler(type);
            handlers.Add(handler);
            return handler;
        }

        /// <summary>
        /// イベントハンドラを破棄します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        public void RemoveHandler(T type)
        {
            for (int i = 0; i < handlers.Count; ++i)
            {
                if (handlers[i].Type.Equals(type))
                {
                    handlers.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// イベントハンドラを返します。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<V> GetHandlers()
        {
            for (int i = 0; i < handlers.Count; ++i)
            {
                yield return handlers[i];
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// 入力情報から座標を取得します。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        /// <returns>スクリーン座標で計算する場合はスクリーン座標、そうでない場合はローカル座標を返します。</returns>
        protected override Vector2 GetPosition(PointerEventData eventData)
        {
            if (useScreenPosition)
            {
                return eventData.position;
            }
            else
            {
                Vector2 localPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(data.EventTarget, eventData.position, data.EventCamera, out localPosition);
                return localPosition;
            }
        }

        /// <summary>
        /// 指定されたジェスチャーの種類に紐づくイベントを発火させます。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        protected void Dispatch(T type)
        {
            V handler = GetHandler(type);
            if (handler != null)
            {
                handler.Delegates.Execute(data);
            }
        }

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <param name="delegatable">DelegatableObject</param>
        protected void AddEvent(T type, DelegatableObject delegatable)
        {
            V handler = GetHandler(type);
            if (handler == null)
            {
                handler = AddHandler(type);
            }

            handler.Delegates.Add(delegatable);
        }

        /// <summary>
        /// イベントハンドラを作成します
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        /// <returns>作成されたイベントハンドラを返します。</returns>
        protected abstract V CreateHandler(T type);

#endregion
    }
}
