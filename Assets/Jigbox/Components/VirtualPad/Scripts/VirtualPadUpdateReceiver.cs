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

namespace Jigbox.VirtualPad
{
    /// <summary>
    /// バーチャルパッドの値の変化を受け取るための抽象クラス
    /// </summary>
    public abstract class VirtualPadUpdateReceiver : MonoBehaviour
    {
#region public methods

        /// <summary>
        /// バーチャルパッドの状態が有効になった際に呼び出されます。
        /// </summary>
        /// <param name="data">バーチャルパッドで処理されるデータ</param>
        public virtual void OnActivate(VirtualPadData data)
        {
        }

        /// <summary>
        /// バーチャルパッドの移動量が変化した際に呼び出されます。
        /// </summary>
        /// <param name="data">バーチャルパッドで処理されるデータ</param>
        public virtual void OnAxisChanged(VirtualPadData data)
        {
        }

        /// <summary>
        /// バーチャルパッドの更新タイミングで毎フレーム呼び出されます。
        /// </summary>
        /// <param name="data">バーチャルパッドで処理されるデータ</param>
        public virtual void OnUpdate(VirtualPadData data)
        {
        }

        /// <summary>
        /// バーチャルパッドへの操作が終了して無効になった際に呼び出されます。
        /// </summary>
        /// <param name="data">バーチャルパッドで処理されるデータ</param>
        public virtual void OnDeactivate(VirtualPadData data)
        {
        }

#endregion
    }
}
