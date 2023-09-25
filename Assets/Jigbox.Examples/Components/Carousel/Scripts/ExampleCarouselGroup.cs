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
    public class ExampleCarouselGroup : MonoBehaviour
    {
        /// <summary>カルーセル</summary>
        [SerializeField]
        Carousel carousel = null;

        /// <summary>ループ設定を表示するテキスト</summary>
        [SerializeField]
        Text loopText = null;

        /// <summary>ループ設定</summary>
        bool isLoop = false;

        /// <summary>BulletControllerが設定されてるか</summary>
        public bool IsExistBulletController { get { return carousel.BulletController != null; } }

        /// <summary>セル数</summary>
        public int CellCount { get { return carousel.CellCount; } }

        void Awake()
        {
            isLoop = carousel.IsLoop;
        }

        /// <summary>
        /// ループフラグを入れ替える
        /// </summary>
        public void SwitchLoop()
        {
            isLoop = !isLoop;
            loopText.text = isLoop ? "Loop On" : "Loop Off";
            loopText.color = isLoop ? Color.green : Color.red;
        }

        /// <summary>
        /// カルーセルのレイアウトを初期化します
        /// </summary>
        public void Relayout()
        {
            carousel.Relayout(isLoop && carousel.CanLoop);
        }

        /// <summary>
        /// カルーセルにセルを追加します
        /// </summary>
        /// <param name="exampleCell"></param>
        public void AddCell(ExampleCell exampleCell)
        {
            if (carousel.AddCell(exampleCell.gameObject))
            {
                exampleCell.transform.localScale = Vector3.one;
            }
            else
            {
                Destroy(exampleCell);
            }
        }

        /// <summary>
        /// カルーセルからセルを全て消します
        /// </summary>
        public void RemoveAllCell()
        {
            carousel.RemoveAllCell();
        }

        /// <summary>
        /// Bulletを追加します
        /// </summary>
        /// <param name="bullet"></param>
        public void AddBullet(GameObject bullet)
        {
            var bulletController = carousel.BulletController;
            if (bulletController != null)
            {
                carousel.BulletController.AddBullet(bullet);
            }
        }

        /// <summary>
        /// Bulletを全て削除します
        /// </summary>
        public void RemoveAllBullet()
        {
            var bulletController = carousel.BulletController;
            if (bulletController != null)
            {
                bulletController.RemoveAllBullet();
            }
        }
    }
}
