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
    /// Carouselシーン動作用サンプルクラス
    /// </summary>
    public sealed class ExampleCarouselController : ExampleSceneBase
    {
#region constants

        /// <summary>
        /// デフォルトのセルの数
        /// </summary>
        static readonly int DefaultCellCount = 2;

#endregion

#region properties

        /// <summary>
        /// カルーセルGroup
        /// </summary>
        [SerializeField]
        ExampleCarouselGroup[] carouselGroups = null;

        /// <summary>
        /// セル用のprefab
        /// </summary>
        [SerializeField]
        GameObject cellPrefab = null;

        /// <summary>
        /// バレット用のprefab
        /// </summary>
        [SerializeField]
        GameObject bulletPrefab = null;

        /// <summary>
        /// セルの数を表示するためのテキスト
        /// </summary>
        [SerializeField]
        UnityEngine.UI.Text cellCountText = null;

        /// <summary>
        /// セルの個数を変動させるためのスライダー
        /// </summary>
        [SerializeField]
        Slider slider = null;

        /// <summary>
        /// セルの数
        /// </summary>
        int cellCount = DefaultCellCount;

#endregion

#region private methods

        [AuthorizedAccess]
        void OnStepChanged(int step)
        {
            cellCount = DefaultCellCount + step;
            cellCountText.text = cellCount.ToString();
        }

        [AuthorizedAccess]
        void OnClickRelayout()
        {
            FillCells();
        }

        [AuthorizedAccess]
        void OnClickRelayoutLoopTypeToggle()
        {
            FillCells2();
        }

        [AuthorizedAccess]
        void OnClickRelayoutLoopTypeToggle2()
        {
            foreach (var carouselGroup in carouselGroups)
            {
                carouselGroup.SwitchLoop();
                carouselGroup.Relayout();
            }
        }

        void FillCells2()
        {
            foreach (var carouselGroup in carouselGroups)
            {
                carouselGroup.RemoveAllCell();
                carouselGroup.RemoveAllBullet();

                for (int i = 0; i < cellCount; i++)
                {
                    // Cellの生成
                    var cell = Instantiate(cellPrefab) as GameObject;
                    var cellComponent = cell.GetComponent<ExampleCell>();
                    cellComponent.name = "Cell" + i;
                    cellComponent.Initialize("#" + i);
                    carouselGroup.AddCell(cellComponent);

                    // Bulletの生成
                    if (carouselGroup.IsExistBulletController)
                    {
                        var bullet = Instantiate(bulletPrefab) as GameObject;
                        bullet.name = "Bullet" + i;
                        carouselGroup.AddBullet(bullet);
                        bullet.transform.localScale = Vector3.one;
                    }
                }

                carouselGroup.SwitchLoop();
                carouselGroup.Relayout();
            }
        }

        void FillCells()
        {
            foreach (var carouselGroup in carouselGroups)
            {
                carouselGroup.RemoveAllCell();
                carouselGroup.RemoveAllBullet();

                for (int i = 0; i < cellCount; i++)
                {
                    // Cellの生成
                    var cell = Instantiate(cellPrefab) as GameObject;
                    var cellComponent = cell.GetComponent<ExampleCell>();
                    cellComponent.name = "Cell" + i;
                    cellComponent.Initialize("#" + i);
                    carouselGroup.AddCell(cellComponent);

                    // Bulletの生成
                    if (carouselGroup.IsExistBulletController)
                    {
                        var bullet = Instantiate(bulletPrefab) as GameObject;
                        bullet.name = "Bullet" + i;
                        carouselGroup.AddBullet(bullet);
                        bullet.transform.localScale = Vector3.one;
                    }
                }

                carouselGroup.Relayout();
            }
        }

#endregion

#region unity override methods

        void Start()
        {
            OnStepChanged(slider.CurrentStep);
            FillCells();
        }

#endregion
    }
}
