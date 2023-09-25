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
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    /// <summary>
    /// Bulletのサンプル用クラス
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ExampleBullet : BulletBase
    {
#region properties
        
        /// <summary>
        /// 選択された時に表示するSprite
        /// </summary>
        [SerializeField]
        Sprite selectImage = null;

        /// <summary>
        /// 選択されてない時に表示するSprite
        /// </summary>
        [SerializeField]
        Sprite deselectImage = null;

        /// <summary>
        /// Spriteを表示させるimage
        /// </summary>
        [SerializeField]
        Image image = null;

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
            image.sprite = selectImage;
        }

        /// <summary>
        /// 非選択時になったときに呼ばれます
        /// </summary>
        public override void OnDeselect()
        {
            image.sprite = deselectImage;
        }

#endregion
    }
}
