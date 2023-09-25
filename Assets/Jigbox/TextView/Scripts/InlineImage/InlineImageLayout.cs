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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jigbox.TextView
{
    /// <summary>
    /// インライン画像のゲームオブジェクトの作成・削除を行うクラスです。
    /// コンストラクタに渡したゲームオブジェクトの子要素としてインライン画像を管理します。
    ///
    /// 以下のように利用します。
    /// 1. InlineImageLayoutのインスタンスを作成
    /// 2. Add()/Clear()を呼び出し、登録・クリアを予約する
    /// 3. 予約が完了したら、Place()を呼び出し、ゲームオブジェクトの生成・削除を行う
    ///
    /// Place()はすでに配置済みのインライン画像と予約されたインライン画像の差分を計算し、必要最低限の作成・削除を行います。
    /// </summary>
    public class InlineImageLayout
    {
#region internal class

        /// <summary>
        /// インライン画像の配置場所を記録する値オブジェクトのクラスです。
        /// </summary>
        internal class InlineImagePlacement
        {
            public float X { get; private set; }

            public float Y { get; private set; }

            public float Width { get; private set; }

            public float Height { get; private set; }

            public string Source { get; private set; }

            public string Name { get; private set; }

            internal InlineImagePlacement(float x, float y, float width, float height, string source, string name)
            {
                this.X = x;
                this.Y = y;
                this.Width = width;
                this.Height = height;
                this.Source = source;
                this.Name = name;
            }

            public override int GetHashCode()
            {
                return X.GetHashCode() ^
                Y.GetHashCode() ^
                Width.GetHashCode() ^
                Height.GetHashCode() ^
                (Source ?? string.Empty).GetHashCode() ^
                (Name ?? string.Empty).GetHashCode();
            }

            public override bool Equals(object other)
            {
                var o = other as InlineImagePlacement;
                if (o == null)
                {
                    return false;
                }

                var tolerance = 0.00001f;

                return Math.Abs(X - o.X) < tolerance &&
                Math.Abs(Y - o.Y) < tolerance &&
                Math.Abs(Width - o.Width) < tolerance &&
                Math.Abs(Height - o.Height) < tolerance &&
                Source == o.Source &&
                Name == o.Name;
            }
        }

#endregion

#region fields

        /// <summary>
        /// 画像のロードと <c>GameObject</c> の配置を担う <c>IndividualInlineImageLayout</c> のリスト。
        /// 完了するとここから要素が削除される。
        /// これと <c>requestedImages</c> は一対一対応がある。
        /// </summary>
        private readonly List<IndividualInlineImageLayout> individualLayouts;

        /// <summary>配置予約済み画像の配置情報。画像要求されると各々の要素は <c>requestingImages</c> にも追加される。</summary>
        private readonly HashSet<InlineImagePlacement> reservedImages;

        /// <summary>要求中画像の配置情報。画像ロードが完了すると各々の要素は <c>placedImages</c> に移動される。</summary>
        private readonly HashSet<InlineImagePlacement> requestedImages;

        /// <summary>ロードの完了した画像の配置情報。</summary>
        private readonly HashSet<InlineImagePlacement> placedImages;

        private IInlineImageProvider provider;
        private GameObject parentGameObject;
        private readonly InlineImageGameObjectPool pool;

        private const string ObjectName = "Cached Game Object for Inline Image";

        /// <summary>レイアウトが崩れているかどうか</summary>
        private bool isLayoutDirty = false;

#endregion

#region propeties

        /// <summary>
        /// 予約されているインライン画像数を返します。
        /// </summary>
        public int Count
        {
            get { return this.reservedImages.Count; }
        }

#endregion

#region constructors

        /// <summary>
        /// コンストラクタ
        /// </summary>
        ///
        /// <param name="parentGameObject">このオブジェクトの子要素としてインライン画像を配置します</param>
        /// <param name="provider">インライン画像を提供するプロバイダーです</param>
        public InlineImageLayout(GameObject parentGameObject, IInlineImageProvider provider)
        {
            this.individualLayouts = new List<IndividualInlineImageLayout>();
            this.placedImages = new HashSet<InlineImagePlacement>();
            this.reservedImages = new HashSet<InlineImagePlacement>();
            this.requestedImages = new HashSet<InlineImagePlacement>();
            this.provider = new InlineImageRepository(provider);
            this.parentGameObject = parentGameObject;
            this.pool = new InlineImageGameObjectPool();
            this.pool.Init(this.parentGameObject.transform);
        }

#endregion

#region public methods

        /// <summary>
        /// インライン画像の配置を予約します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="source">取得元。IInlineImageProviderのidentifierに渡されます</param>
        /// <param name="name">インライン画像用のゲームオブジェクトの名前</param>
        public bool Add(float x, float y, float width, float height, string source, string name)
        {
            var isAdded = this.reservedImages.Add(
                              new InlineImagePlacement(x, y, width, height, source, name)
                          );

            return isAdded;
        }

        /// <summary>
        /// インライン画像の配置予約をクリアします。
        /// </summary>
        public void ClearReservedImages()
        {
            this.reservedImages.Clear();
        }

        /// <summary>
        /// インライン画像のGameObjectと配置情報を全て解放します
        /// </summary>
        public void ClearInlineImageGameObjects()
        {
            CancelRequests();
            placedImages.Clear();
            this.ReleaseAllInlineImageGameObjects();
        }

        /// <summary>ロード中の要求を全てキャンセルする。</summary>
        public bool CancelRequests()
        {
            bool isCanceled = individualLayouts.Count > 0;
            foreach (var layout in individualLayouts)
            {
                layout.Cancel();
            }
            requestedImages.Clear();
            individualLayouts.Clear();

            return isCanceled;
        }

        /// <summary>
        /// インライン画像の配置予約に基づいて、ゲームオブジェクトの作成・削除を行います。
        /// </summary>
        public void Place()
        {
            if (isLayoutDirty)
            {
                ClearInlineImageGameObjects();
                isLayoutDirty = false;
                // 諸々の情報がクリアされるとimagesToBeAdd.IsSupersetOf(placedAndRequestingImages)が必ず正となる
            }

            HashSet<InlineImagePlacement> imagesToBeAdd =
                new HashSet<InlineImagePlacement>(reservedImages);
            var placedAndRequestingImages = placedImages.Union(requestedImages);

            if (imagesToBeAdd.SetEquals(placedAndRequestingImages))
            {
                // 配置予約のものは全て配置済みもしくはロード中
                return;
            }
            if (imagesToBeAdd.IsSupersetOf(placedAndRequestingImages))
            {
                imagesToBeAdd.ExceptWith(placedAndRequestingImages);
            }
            else
            {
                ClearInlineImageGameObjects();
            }

            foreach (var image in imagesToBeAdd)
            {
                LoadInlineImageLayout(image);
            }
        }

        /// <summary>
        /// 配置済みのインライン画像が崩れたことを設定し、再計算が行われるようにします。
        /// </summary>
        public void SetLayoutDirty()
        {
            isLayoutDirty = true;
        }

#if UNITY_EDITOR
        public void PoolDestroyAll()
        {
            this.pool.DestroyAll();
        }
#endif
#endregion

#region private methods

        private void LoadInlineImageLayout(InlineImagePlacement image)
        {
            IndividualInlineImageLayout layout = null;
            layout = new IndividualInlineImageLayout(
                provider: this.provider,
                identifier: image.Source,
                pool: pool,
                onInitializeGameObject: gameObject =>
                {
                    gameObject.name = image.Name;
                    var rectTranform = gameObject.transform as RectTransform;
                    rectTranform.sizeDelta = new Vector2(image.Width, image.Height);
                    rectTranform.localPosition = new Vector3(image.X, image.Y, 0);
                },
                onCompleteLayout: () =>
                {
                    individualLayouts.Remove(layout);
                    requestedImages.Remove(image);
                    placedImages.Add(image);
                },
                onError: message =>
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("loading inline image failed: {0}", message);
#endif
                    individualLayouts.Remove(layout);
                    requestedImages.Remove(image);
                });
            this.individualLayouts.Add(layout);
            this.requestedImages.Add(image);
            layout.Load();
        }

        private void ReleaseAllInlineImageGameObjects()
        {
            foreach (var layout in this.individualLayouts)
            {
                layout.Cancel();
            }
            this.individualLayouts.Clear();
            pool.ReleaseAll();
        }

#endregion
    }
}
