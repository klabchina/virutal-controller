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
using System.Reflection;

namespace Jigbox.Delegatable
{
    public interface IEventDelegate
    {
#region properties

        /// <summary>イベントの対象</summary>
        Object Target { get; }

        /// <summary>イベント発行時に呼び出されるメソッド情報</summary>
        MethodInfo Method { get; }

#endregion

#region public methods

        /// <summary>
        /// デリゲートを実行します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>メソッドが実行できれば<c>true</c>を返します。</returns>
        bool Invoke(params object[] args);

        /// <summary>
        /// 対象のデリゲートが同一のものであるかを返します。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool Equals(IEventDelegate other);

#endregion
    }

    public abstract class EventDelegate<T> : IEventDelegate
    {
#region inner classes, enum, and structs

        /// <summary>デリゲート型</summary>
        public delegate void Callback(T arg);

#endregion

#region properties

        /// <summary>デリゲート</summary>
        protected Callback eventDelegate;

        /// <summary>対象オブジェクト</summary>
        public Object Target { get { return eventDelegate.Target as Object; } }

        /// <summary>対象メソッド</summary>
        public MethodInfo Method { get { return eventDelegate.Method; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="eventDelegate">デリゲート</param>
        public EventDelegate(Callback eventDelegate)
        {
            this.eventDelegate = eventDelegate;
        }

        /// <summary>
        /// デリゲートを実行します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>メソッドが実行できれば<c>true</c>を返します。</returns>
        public bool Invoke(params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return false;
            }

            var args0Type = args[0].GetType();
            var TType = typeof(T);

            // argsと指定された型が違い、argsの型がサブクラスでもないときは型指定違いのため実行しない
            if (args0Type != TType && !args0Type.IsSubclassOf(TType))
            {
                return false;
            }

            eventDelegate((T) args[0]);
            return true;
        }

        /// <summary>
        /// 対象のデリゲートが同一のものであるかを返します。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IEventDelegate other)
        {
            EventDelegate<T> otherEvent = other as EventDelegate<T>;
            if (otherEvent == null)
            {
                return false;
            }
            return eventDelegate == otherEvent.eventDelegate;
        }

#endregion
    }

    public abstract class EventDelegate<T0, T1> : IEventDelegate
    {
#region inner classes, enum, and structs

        /// <summary>デリゲート型</summary>
        public delegate void Callback(T0 arg1, T1 arg2);

#endregion

#region properties

        /// <summary>デリゲート</summary>
        protected Callback eventDelegate;

        /// <summary>対象オブジェクト</summary>
        public Object Target { get { return eventDelegate.Target as Object; } }

        /// <summary>対象メソッド</summary>
        public MethodInfo Method { get { return eventDelegate.Method; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="eventDelegate">デリゲート</param>
        public EventDelegate(Callback eventDelegate)
        {
            this.eventDelegate = eventDelegate;
        }

        /// <summary>
        /// デリゲートを実行します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>メソッドが実行できれば<c>true</c>を返します。</returns>
        public bool Invoke(params object[] args)
        {
            if (args == null || args.Length < 2)
            {
                return false;
            }

            var args0Type = args[0].GetType();
            var args1Type = args[1].GetType();
            var T0Type = typeof(T0);
            var T1Type = typeof(T1);

            // argsと指定された型が違い、argsの型がサブクラスでもないときは型指定違いのため実行しない
            if ((args0Type != T0Type && !args0Type.IsSubclassOf(T0Type))
                ||
                (args1Type != T1Type && !args1Type.IsSubclassOf(T1Type)))
            {
                return false;
            }

            eventDelegate((T0) args[0], (T1) args[1]);
            return true;
        }

        /// <summary>
        /// 対象のデリゲートが同一のものであるかを返します。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IEventDelegate other)
        {
            EventDelegate<T0, T1> otherEvent = other as EventDelegate<T0, T1>;
            if (otherEvent == null)
            {
                return false;
            }
            return eventDelegate == otherEvent.eventDelegate;
        }

#endregion
    }

    public abstract class EventDelegate<T0, T1, T2> : IEventDelegate
    {
#region inner classes, enum, and structs

        /// <summary>デリゲート型</summary>
        public delegate void Callback(T0 arg1, T1 arg2, T2 arg3);

#endregion

#region properties

        /// <summary>デリゲート</summary>
        protected Callback eventDelegate;

        /// <summary>対象オブジェクト</summary>
        public Object Target { get { return eventDelegate.Target as Object; } }

        /// <summary>対象メソッド</summary>
        public MethodInfo Method { get { return eventDelegate.Method; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="eventDelegate">デリゲート</param>
        public EventDelegate(Callback eventDelegate)
        {
            this.eventDelegate = eventDelegate;
        }

        /// <summary>
        /// デリゲートを実行します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>メソッドが実行できれば<c>true</c>を返します。</returns>
        public bool Invoke(params object[] args)
        {
            if (args == null || args.Length < 3)
            {
                return false;
            }

            var args0Type = args[0].GetType();
            var args1Type = args[1].GetType();
            var args2Type = args[2].GetType();
            var T0Type = typeof(T0);
            var T1Type = typeof(T1);
            var T2Type = typeof(T2);

            // argsと指定された型が違い、argsの型がサブクラスでもないときは型指定違いのため実行しない
            if ((args0Type != T0Type && !args0Type.IsSubclassOf(T0Type))
                ||
                (args1Type != T1Type && !args1Type.IsSubclassOf(T1Type))
                ||
                (args2Type != T2Type && !args2Type.IsSubclassOf(T2Type)))
            {
                return false;
            }
            eventDelegate((T0) args[0], (T1) args[1], (T2) args[2]);
            return true;
        }

        /// <summary>
        /// 対象のデリゲートが同一のものであるかを返します。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IEventDelegate other)
        {
            EventDelegate<T0, T1, T2> otherEvent = other as EventDelegate<T0, T1, T2>;
            if (otherEvent == null)
            {
                return false;
            }
            return eventDelegate == otherEvent.eventDelegate;
        }

#endregion
    }
}
