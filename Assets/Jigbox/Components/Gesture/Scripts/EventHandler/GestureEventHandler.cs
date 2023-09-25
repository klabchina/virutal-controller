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
using Jigbox.Delegatable;

namespace Jigbox.Gesture
{
    /// <summary>
    /// ジェスチャーが発火した際のイベントハンドラ
    /// </summary>
    /// <typeparam name="T">ジェスチャーの種類を定義した列挙型</typeparam>
    [System.Serializable]
    public class GestureEventHandler<T> where T : System.IConvertible
    {
#region properties

        /// <summary>ジェスチャーの種類</summary>
        [HideInInspector]
        [SerializeField]
        protected T type;

        /// <summary>ジェスチャーの種類</summary>
        public T Type { get { return type; } }

        /// <summary>デリゲート</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList delegates = new DelegatableList();

        /// <summary>デリゲート</summary>
        public DelegatableList Delegates { get { return delegates; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type">ジェスチャーの種類</param>
        public GestureEventHandler(T type)
        {
            this.type = type;
        }

#endregion
    }
}
