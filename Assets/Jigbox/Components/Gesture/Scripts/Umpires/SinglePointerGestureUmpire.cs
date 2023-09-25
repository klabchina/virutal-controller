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

using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using Jigbox.Components;

namespace Jigbox.Gesture
{
    /// <summary>
    /// 単一のポインタでジェスチャーを判別するための抽象クラス
    /// </summary>
    public abstract class SinglePointerGestureUmpire<T, U, V> : GestureUmpire<T, U, V> where T : IConvertible where U : IGestureEventData where V : GestureEventHandler<T>
    {
#region properties

        /// <summary>現在ジェスチャーを判定するのに利用しているポインタのID</summary>
        protected int currentPointerId = DisablePointeId;

        /// <summary>更新が行えるかどうか</summary>
        public override bool IsEnableUpdate { get { return currentPointerId != DisablePointeId; } }

        /// <summary>入力のロックを解除してもいいかどうか</summary>
        public override bool MayUnlock { get { return !IsEnableUpdate; } }

#endregion

#region public methods

        /// <summary>
        /// 入力のロックが自動解除された際に呼び出されます。
        /// </summary>
        public override void OnAutoUnlock()
        {
            currentPointerId = DisablePointeId;
        }

#endregion

#region protected methods

        /// <summary>
        /// 押下されたポインタが有効かどうかを返します。
        /// </summary>
        /// <param name="pointer">押下されたポインタの情報</param>
        /// <returns>ポインタが有効であれば<c>true</c>を、無効であれば<c>false</c>を返します。</returns>
        protected bool ValidatePress(PointerEventData pointer)
        {
            if (currentPointerId != DisablePointeId)
            {
                return false;
            }

            currentPointerId = pointer.pointerId;

#if UNITY_EDITOR
            if (!useScreenPosition && pointer.pressEventCamera == null)
            {
                GestureInputEmulator emulator = GetComponent<GestureInputEmulator>();
                if (emulator != null)
                {
                    UnityEngine.Debug.LogWarning("PointerEventData has nod EventCamera from GestureInputEmulator!\n"
                        + "Therefore, it may not be possible to obtain correct position!");
                }
            }
#endif

            return true;
        }

        /// <summary>
        /// 離されたポインタが有効化どうかを返します。
        /// </summary>
        /// <param name="pointer">離されたポインタの情報</param>
        /// <returns>ポインタが有効であれば<c>true</c>を、無効であれば<c>false</c>を返します。</returns>
        protected bool ValidateRelease(PointerEventData pointer)
        {
            if (currentPointerId != pointer.pointerId)
            {
                return false;
            }

            currentPointerId = DisablePointeId;
            return true;
        }

        /// <summary>
        /// 現在利用しているポインタの情報を取得します。
        /// </summary>
        /// <param name="pointers">押下されている全てのポインタ</param>
        /// <returns></returns>
        protected PointerEventData GetCurrentPointer(HashSet<PointerEventData> pointers)
        {
            foreach (PointerEventData pointer in pointers)
            {
                if (currentPointerId == pointer.pointerId)
                {
                    return pointer;
                }
            }
            return null;
        }

#endregion
    }
}
