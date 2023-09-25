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

namespace Jigbox.Examples
{
    /// <summary>
    /// BulletControllerのサンプル用クラス
    /// </summary>
    public class ExampleBulletController : BulletControllerBase
    {
#region public override methods

        /// <summary>
        /// 再レイアウト用メソッド
        /// </summary>
        public override void Relayout()
        {
            if (ContentTransform != null)
            {
                bulletList = ContentTransform.GetComponentsInChildren<BulletBase>();
                for (int i = 0; i < bulletList.Length; i++)
                {
                    var bullet = bulletList[i] as ExampleBullet2;
                    bullet.Initialize(i, BulletClicked);
                    bullet.IsSelected = false;
                }

#if UNITY_EDITOR
                if (bulletList.Length != carousel.CellCount)
                {
                    Debug.LogWarning("The number of cell and bullet do not match.");
                }
#endif
            }
            ChangeBulletAt(carousel.CurrentIndex);
        }

#endregion

#region public methods

        /// <summary>
        /// Bulletがタップされた時に呼ばれます
        /// </summary>
        /// <param name="index"></param>
        public void BulletClicked(int index)
        {
            carousel.ShowCellAt(index);
        }

#endregion
    }
}
