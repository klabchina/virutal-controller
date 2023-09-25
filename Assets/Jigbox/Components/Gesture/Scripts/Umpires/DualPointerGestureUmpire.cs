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

namespace Jigbox.Gesture
{
    /// <summary>
    /// 2つのポインタでジェスチャーを判別するための抽象クラス
    /// </summary>
    public abstract class DualPointerGestureUmpire<T, U, V> : GestureUmpire<T, U, V> where T : IConvertible where U : IGestureEventData where V : GestureEventHandler<T>
    {
#region properties

        /// <summary>ジェスチャーを判定するのに利用している1番目のポインタのID</summary>
        protected int primaryPointerId = DisablePointeId;

        /// <summary>ジェスチャーを判定するのに利用している2番目のポインタのID</summary>
        protected int secondaryPointerId = DisablePointeId;

        /// <summary>更新が行えるかどうか</summary>
        public override bool IsEnableUpdate { get { return primaryPointerId != DisablePointeId && secondaryPointerId != DisablePointeId; } }

        /// <summary>入力のロックを解除してもいいかどうか</summary>
        public override bool MayUnlock { get { return primaryPointerId == DisablePointeId && secondaryPointerId == DisablePointeId; } }

#endregion

#region public methods

        /// <summary>
        /// 入力のロックが自動解除された際に呼び出されます。
        /// </summary>
        public override void OnAutoUnlock()
        {
            primaryPointerId = DisablePointeId;
            secondaryPointerId = DisablePointeId;
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
            // 2番目のポインタが設定されている状態では、そのポインタが解除されない限り、入力は全て無効となる
            if (secondaryPointerId != DisablePointeId)
            {
                return false;
            }

            if (primaryPointerId == DisablePointeId)
            {
                primaryPointerId = pointer.pointerId;
            }
            else
            {
                secondaryPointerId = pointer.pointerId;
            }
            return true;
        }

        /// <summary>
        /// 離されたポインタが有効化どうかを返します。
        /// </summary>
        /// <param name="pointer">離されたポインタの情報</param>
        /// <returns>ポインタが有効であれば<c>true</c>を、無効であれば<c>false</c>を返します。</returns>
        protected bool ValidateRelease(PointerEventData pointer)
        {
            int pointerId = pointer.pointerId;
            if (primaryPointerId != pointerId && secondaryPointerId != pointerId)
            {
                return false;
            }

            if (primaryPointerId == pointerId)
            {
                primaryPointerId = DisablePointeId;
            }
            else
            {
                secondaryPointerId = DisablePointeId;
            }
            return true;
        }

        /// <summary>
        /// 1番目のポインタの情報を取得します。
        /// </summary>
        /// <param name="pointers">押下されている全てのポインタ</param>
        /// <returns></returns>
        protected PointerEventData GetPrimaryPointer(HashSet<PointerEventData> pointers)
        {
            foreach (PointerEventData pointer in pointers)
            {
                if (primaryPointerId == pointer.pointerId)
                {
                    return pointer;
                }
            }
            return null;
        }

        /// <summary>
        /// 2番目のポインタの情報を取得します。
        /// </summary>
        /// <param name="pointers">押下されている全てのポインタ</param>
        /// <returns></returns>
        protected PointerEventData GetSecondaryPointer(HashSet<PointerEventData> pointers)
        {
            foreach (PointerEventData pointer in pointers)
            {
                if (secondaryPointerId == pointer.pointerId)
                {
                    return pointer;
                }
            }
            return null;
        }

#endregion
    }
}
