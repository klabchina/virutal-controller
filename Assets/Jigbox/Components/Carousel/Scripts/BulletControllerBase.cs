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
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// Bulletを管理するクラス
    /// </summary>
    [DisallowMultipleComponent]
    public class BulletControllerBase : MonoBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>
        /// Bulletの親になるオブジェクト(Content)のTransform
        /// </summary>
        [SerializeField]
        protected Transform contentTransform;

        /// <summary>
        /// Bulletの親になるオブジェクト(Content)のTransformを返します
        /// </summary>
        public virtual Transform ContentTransform { get { return contentTransform; } }

        /// <summary>
        /// Relayoutが叩かれた時点でcontentTransformの子になっているBulletBaseのリスト
        /// </summary>
        protected BulletBase[] bulletList;

        /// <summary>
        /// Carousel
        /// </summary>
        protected Carousel carousel;

        /// <summary>
        /// Bullet生成用のInstanceProvider
        /// </summary>
        protected IInstanceProvider<GameObject> bulletProvider;

        /// <summary>
        /// Bullet生成用のプロバイダーを設定/取得します
        /// </summary>
        /// <value>BulletProvider</value>
        public virtual IInstanceProvider<GameObject> BulletProvider { get { return bulletProvider; } set { bulletProvider = value; } }

        /// <summary>
        /// Bullet処分用のInstanceDisposer
        /// </summary>
        protected IInstanceDisposer<GameObject> bulletDisposer = new CarouselInstanceDisposer();

        /// <summary>
        /// Bullet処分用のディスポーザを設定/取得します
        /// </summary>
        public virtual IInstanceDisposer<GameObject> BulletDisposer { get { return bulletDisposer; } set { bulletDisposer = value; } }

#endregion

#region public methods

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        /// <param name="carouselComponent">Carousel</param>
        public virtual void Initialize(Carousel carouselComponent)
        {
            carousel = carouselComponent;
        }

        /// <summary>
        /// 再レイアウト用メソッド
        /// </summary>
        public virtual void Relayout()
        {
            if (ContentTransform != null)
            {
                bulletList = ContentTransform.GetComponentsInChildren<BulletBase>();
                for (int i = 0; i < bulletList.Length; i++)
                {
                    bulletList[i].Initialize();
                    bulletList[i].IsSelected = false;
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

        /// <summary>
        /// 指定したIndexのBulletを選択します
        /// </summary>
        /// <param name="selectIndex">index</param>
        public virtual void ChangeBulletAt(int selectIndex)
        {
            for (int i = 0; i < bulletList.Length; i++)
            {
                bulletList[i].IsSelected = i == selectIndex;
            }
        }

        /// <summary>
        /// Bulletを追加します
        /// </summary>
        public virtual void AddBullet(GameObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.transform.SetParent(ContentTransform, false);
        }

        /// <summary>
        /// Bulletを追加します
        /// </summary>
        public virtual void AddBullet(Func<GameObject> generator)
        {
            if (generator == null)
            {
                throw new ArgumentNullException("generator");
            }

            var obj = generator();
            obj.transform.SetParent(ContentTransform, false);
        }

        /// <summary>
        /// Bulletを追加します(InstanceProvider使用)
        /// </summary>
        public virtual void AddBullet()
        {
            if (bulletProvider == null)
            {
                throw new ArgumentNullException("bulletProvider");
            }

            var obj = bulletProvider.Generate();
            obj.transform.SetParent(ContentTransform, false);
        }

        /// <summary>
        /// Bulletを全て削除します
        /// </summary>
        public virtual void RemoveAllBullet()
        {
            for (int i = ContentTransform.childCount - 1; i >= 0; i--)
            {
                bulletDisposer.Dispose(ContentTransform.GetChild(i).gameObject);
            }
        }

#endregion
    }
}
