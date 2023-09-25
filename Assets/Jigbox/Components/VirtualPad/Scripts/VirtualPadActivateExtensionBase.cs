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
using Jigbox.VirtualPad;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jigbox.Components
{
    /// <summary>
    /// バーチャルパッドの有効と無効を操作するための拡張コンポーネントの基底クラス
    /// </summary>
    [RequireComponent(typeof(VirtualPadController))]
    public abstract class VirtualPadActivateExtensionBase : MonoBehaviour
    {
#region fields

        /// <summary>
        /// ポインタの情報
        /// </summary>
        protected PointerEventData pointerEventData = null;

        /// <summary>
        /// バーチャルパッド本体の情報
        /// </summary>
        protected VirtualPadData virtualPadData = null;

        /// <summary>
        /// 本体で行うタッチ時の処理のキャッシュ
        /// </summary>
        protected Action<PointerEventData> pressRequest = null;

#endregion

#region abstract methods

        /// <summary>
        /// タッチ中、必要であれば時間経過でバーチャルパッドのタッチ処理を呼び出します
        /// </summary>
        public abstract void OnUpdate();

#endregion

#region public methods

        /// <summary>
        /// 必要なデータを渡して初期化します、初期化が複数回行われた場合エラーを出します。
        /// </summary>
        /// <param name="virtualPadData">バーチャルパッドの情報</param>
        /// <param name="pressRequest">本体で行うタッチ時の処理</param>
        public virtual void Initialize(VirtualPadData virtualPadData, Action<PointerEventData> pressRequest)
        {
            // 既に初期化されている状態のため、エラーを出して弾く
            if (this.virtualPadData != null)
            {
#if UNITY_EDITOR || JIGBOX_DEBUG

                Debug.LogError("VirtualPadActivateExtension can't register with more than one component");

#endif
                return;
            }

            this.virtualPadData = virtualPadData;
            this.pressRequest = pressRequest;
        }

        /// <summary>
        /// タッチ時に必要なデータをセットし、動作を有効化します
        /// </summary>
        /// <param name="pointerEventData">ポインタの情報</param>
        public virtual void Activate(PointerEventData pointerEventData)
        {
            if (this.pointerEventData != null)
            {
                return;
            }

            this.pointerEventData = pointerEventData;
        }

        /// <summary>
        /// 動作を無効化します
        /// </summary>
        public virtual void Deactivate()
        {
            if (pointerEventData == null)
            {
                return;
            }

            pointerEventData = null;
            virtualPadData.IsEnabled = false;
        }

#endregion

#region protected methods

        /// <summary>
        /// バーチャルパッドを有効化する際の処理を行います
        /// </summary>
        protected virtual void EnableVirtualPad()
        {
            virtualPadData.IsEnabled = true;
            pressRequest(pointerEventData);
        }

#endregion
    }
}
