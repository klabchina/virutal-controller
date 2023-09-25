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

using Jigbox.Components;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    /// <summary>
    /// Bulletのサンプル用クラス(DragBehaviourがRequireComponentになっているのはBulletをタップした時にCarouselのDrag周りの挙動に伝播させないようにブロックするため)
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DragBehaviour))]
    public sealed class ExampleBullet2 : BulletBase
    {
#region properties

        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField]
        Text text = null;

        /// <summary>
        /// このBulletのindex
        /// </summary>
        int index;

        /// <summary>
        /// タップされた時に行うアクション
        /// </summary>
        Action<int> onClick;

#endregion

#region override methods

        /// <summary>
        /// 初期化用メソッド
        /// </summary>
        public override void Initialize()
        {
            OnDeselect();
        }

        /// <summary>
        /// 選択された時に呼ばれます
        /// </summary>
        public override void OnSelect()
        {
            text.color = Color.green;
        }

        /// <summary>
        /// 非選択時になったときに呼ばれます
        /// </summary>
        public override void OnDeselect()
        {
            text.color = Color.gray;
        }

#endregion

#region public methods

        public void Initialize(int bulletIndex, Action<int> clickAction)
        {
            index = bulletIndex;
            onClick = clickAction;
            text.text = gameObject.name;
            Initialize();
        }

        /// <summary>
        /// タップされた時に呼ばれます
        /// </summary>
        [AuthorizedAccess]
        public void OnClick()
        {
            onClick(index);
        }

#endregion
    }
}
