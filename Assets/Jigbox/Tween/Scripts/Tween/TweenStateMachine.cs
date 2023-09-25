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

using System;
using System.Collections.Generic;

namespace Jigbox.Tween
{
    public sealed class TweenStateMachine
    {
#region inner classes, enum, and structs

        /// <summary>
        /// ステートマシン上での状態
        /// </summary>
        class State
        {
            /// <summary>状態が切り替わった際に呼び出されるコールバック</summary>
            Action onEnter;

            /// <summary>状態の更新時に呼び出されるコールバック</summary>
            Action<float> onUpdate;

            /// <summary>状態が切り替わる際に呼び出されるコールバック</summary>
            Action onExit;

            /// <summary>状態を切り替える際の直前の状態</summary>
            int validateMask;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="onEnter">状態が切り替わった際に呼び出されるコールバック</param>
            /// <param name="onUpdate">状態の更新時に呼び出されるコールバック</param>
            /// <param name="onExit">状態が切り替わる際に呼び出されるコールバック</param>
            /// <param name="validateType">状態を切り替える際の直前の状態</param>
            public State(Action onEnter, Action<float> onUpdate, Action onExit, int ValidateMask)
            {
                this.onEnter = onEnter;
                this.onUpdate = onUpdate;
                this.onExit = onExit;
                this.validateMask = ValidateMask;
            }

            /// <summary>
            /// 状態が切り替わった際に呼び出されます。
            /// </summary>
            public void Enter()
            {
                if (onEnter != null)
                {
                    onEnter();
                }
            }

            /// <summary>
            /// 状態を更新する際に呼び出されます。
            /// </summary>
            public void Update(float value)
            {
                if (onUpdate != null)
                {
                    onUpdate(value);
                }
            }

            /// <summary>
            /// 状態が切り替わる際に呼び出されます。
            /// </summary>
            public void Exit()
            {
                if (onExit != null)
                {
                    onExit();
                }
            }

            /// <summary>
            /// 切り替えが可能かどうかを返します。
            /// </summary>
            /// <param name="state">状態</param>
            /// <returns></returns>
            public bool IsValid(TweenState state)
            {
                return (validateMask & (int) state) > 0;
            }
        }

#endregion

#region properties

        /// <summary>
        /// 管理されている状態
        /// enumをkeyにするとGCが発生するため、enumをキャストしてkeyにしている
        /// </summary>
        Dictionary<int, State> states = new Dictionary<int, State>();

        /// <summary>現在の状態</summary>
        State current = null;

        /// <summary>現在設定されている状態</summary>
        public TweenState StateType { get; private set; }

        /// <summary>状態が変更されたかどうか</summary>
        public bool Changed { get; private set; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TweenStateMachine()
        {
            StateType = TweenState.Idle;
            Changed = false;
        }

        /// <summary>
        /// 状態を追加します。
        /// </summary>
        /// <param name="stateType">状態</param>
        /// <param name="onEnter">状態が切り替わった際に呼び出されるコールバック</param>
        /// <param name="onUpdate">状態の更新時に呼び出されるコールバック</param>
        /// <param name="onExit">状態が切り替わる際に呼び出されるコールバック</param>
        /// <param name="validateMask">状態を切り替える際の直前の状態</param>
        /// <returns>追加に成功すれば<c>true</c>、すでに状態が追加済みなら<c>false</c>を返します。</returns>
        public bool Add(
            TweenState stateType,
            Action onEnter = null,
            Action<float> onUpdate = null,
            Action onExit = null,
            int validateMask = (int) TweenState.All)
        {
            if (!states.ContainsKey((int) stateType))
            {
                states.Add((int) stateType, new State(onEnter, onUpdate, onExit, validateMask));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 状態をクリアします。
        /// </summary>
        public void Clear()
        {
            states.Clear();
            current = null;
        }

        /// <summary>
        /// 状態を設定します。
        /// </summary>
        /// <param name="stateType">状態</param>
        /// <returns>設定に成功すれば<c>true</c>、状態が存在しないなら<c>false</c>を返します。</returns>
        public bool Set(TweenState stateType)
        {
            State before = current;
            State next = null;

            if (!states.TryGetValue((int) stateType, out next))
            {
                return false;
            }

            if (!next.IsValid(StateType) || stateType == StateType)
            {
                return false;
            }

            if (before != null)
            {
                before.Exit();
            }

            current = next;
            StateType = stateType;
            Changed = false;
            current.Enter();
            Changed = true;
            return true;
        }

        /// <summary>
        /// 状態を更新します。
        /// </summary>
        /// <param name="value">更新に用いる値</param>
        public void Update(float value)
        {
            Changed = false;
            if (current != null)
            {
                current.Update(value);
            }
        }

#endregion
    }
}
