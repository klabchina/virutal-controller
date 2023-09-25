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
using System.Collections.Generic;

namespace Jigbox.Components
{
    /// <summary>
    /// ジェスチャーの判別用クラスをシリアライズしたコンポーネント
    /// </summary>
    public abstract class GestureUmpireBehaviour : MonoBehaviour
    {
#region constants

        /// <summary>ポインタIDが無効な状態の値</summary>
        protected static readonly int DisablePointeId = int.MinValue;

#endregion

#region properties

        /// <summary>ジェスチャーの判別が有効かどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool isEnable = true;

        /// <summary>ジェスチャーの判別が有効かどうか</summary>
        public bool IsEnable { get { return isEnable; } set { isEnable = value; } }

        /// <summary>計算する座標系をスクリーン座標系で行うかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool useScreenPosition = false;

        /// <summary>計算する座標系をスクリーン座標系で行うかどうか</summary>
        public virtual bool UseScreenPosition { get { return useScreenPosition; } set { useScreenPosition = value; } }

        /// <summary>更新が行えるかどうか</summary>
        public abstract bool IsEnableUpdate { get; }

        /// <summary>入力のロックを解除してもいいかどうか</summary>
        public abstract bool MayUnlock { get; }

#endregion

#region public methods

        /// <summary>
        /// ポインタが押下された際に呼び出されます。
        /// </summary>
        /// <param name="pressedPointer">押下されたポインタの情報</param>
        /// <param name="pressedPointers">押下されている全てのポインタ</param>
        public virtual void OnPress(PointerEventData pressedPointer, HashSet<PointerEventData> pressedPointers)
        {
        }

        /// <summary>
        /// 押下後に指が離された際に呼び出されます。
        /// </summary>
        /// <param name="releasedPointer">離されたポインタの情報</param>
        /// <param name="pressedPointers">押下されている全てのポインタ</param>
        public virtual void OnRelease(PointerEventData releasedPointer, HashSet<PointerEventData> pressedPointers)
        {
        }

        /// <summary>
        /// ポインタの状態からジェスチャーの状態を判別して、更新します。
        /// </summary>
        /// <param name="pressedPointers">押下されている全てのポインタ</param>
        public virtual void OnUpdate(HashSet<PointerEventData> pressedPointers)
        {
        }

        /// <summary>
        /// 入力のロックが自動解除された際に呼び出されます。
        /// </summary>
        public virtual void OnAutoUnlock()
        {
        }

#endregion

#region protected methods

        /// <summary>
        /// 入力情報から座標を取得します。
        /// </summary>
        /// <param name="eventData">入力情報</param>
        /// <returns>スクリーン座標で計算する場合はスクリーン座標、そうでない場合はローカル座標を返します。</returns>
        protected abstract Vector2 GetPosition(PointerEventData eventData);

#endregion
    }
}
