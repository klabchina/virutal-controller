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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Jigbox.UIControl;

namespace Jigbox.TextView
{
#region inner classes, enum, and structs

    /// <summary>
    /// インライン画像プール
    /// </summary>
    /// <c>Take</c> で取得し、<c>Release</c> で返却します。
    public class InlineImageGameObjectPool
    {
        /// <summary>
        /// インライン画像用のオブジェクトプール
        /// </summary>
        /// <typeparam name="T">MaskableGraphicを継承したGraphicコンポーネント</typeparam>
        protected class Pool<T>: ObjectPool<T> where T : MaskableGraphic
        {
            /// <summary>
            /// 使用可能なインスタンスを取得し、使用済みにします。
            /// </summary>
            /// <returns>使用可能なインスタンス</returns>
            public override T Take()
            {
                T instance = base.Take();
                SetAlpha(instance, true);
                return instance;
            }

            /// <summary>
            /// 使用していたインスタンスを返却します。
            /// </summary>
            /// <param name="instance">返却するインスタンス</param>
            public override void Release(T instance)
            {
                base.Release(instance);
                SetAlpha(instance, false);
            }

            /// <summary>
            /// 使用していたインスタンスを全て解放します。
            /// </summary>
            public override void ReleaseAll()
            {
                // HashSetを直接foreachで回すとInlineImageの描画順番が変わってしまうため
                // 一度リストにコピーして順番を保証しつつ順次解放する
                var list = new List<T>(used);
                foreach (T instance in list)
                {
                    free.Push(instance);
                    used.Remove(instance);
                    SetAlpha(instance, false);
                }
            }
            
#if UNITY_EDITOR
            /// <summary>
            /// 使用していたインスタンスを全て削除します。
            /// </summary>
            public void DestroyAll()
            {
                foreach (T instance in free)
                {
                    GameObject.DestroyImmediate(instance.gameObject);
                }
                free.Clear();
            }
#endif
            private void SetAlpha(T component, bool isActive)
            {
                // SetActiveでの切り替えを行うとレイアウト更新中のタイミングでエラーが起きるため
                // アルファを更新して可視化 or 不可視化する
                var color = component.color;
                color.a = isActive ? 1.0f : 0.0f;
                component.color = color;
            }
        }

#endregion

#region porperties

        protected Transform parent = null;
        protected readonly Pool<Image> imagePool = new Pool<Image>();
        protected readonly Pool<RawImage> rawImagePool = new Pool<RawImage>();

#endregion

#region public methods

        /// <summary>
        /// 渡された親オブジェクトを使用して初期化を行います
        /// </summary>
        /// <param name="parent"></param>
        public void Init(Transform parent)
        {
            this.parent = parent;
            Bring();
        }

        /// <summary>
        /// 指定された型情報でPoolからオブジェクトを取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Take<T>() where T : MaskableGraphic
        {
            T instance = null;
            if (typeof(T) == typeof(Image))
            {
                if (!imagePool.IsAvailableTake)
                {
                    imagePool.Bring(CreateObject<T>() as Image);
                }
                instance = imagePool.Take() as T;
            }
            else
            {
                if (!rawImagePool.IsAvailableTake)
                {
                    rawImagePool.Bring(CreateObject<T>() as RawImage);
                }
                instance = rawImagePool.Take() as T;
            }

            return instance;
        }

        /// <summary>
        /// 指定された型情報でPoolにオブジェクトを返却します
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        public void Release<T>(T instance) where T : MaskableGraphic
        {
            if (instance is Image)
            {
                imagePool.Release(instance as Image);
            }
            else
            {
                rawImagePool.Release(instance as RawImage);
            }
        }

        /// <summary>
        /// 全てのPoolのオブジェクトを返却します
        /// </summary>
        public void ReleaseAll()
        {
            imagePool.ReleaseAll();
            rawImagePool.ReleaseAll();
        }
#if UNITY_EDITOR
        /// <summary>
        /// 全てのPoolのオブジェクトを削除します
        /// </summary>
        public void DestroyAll()
        {
            ReleaseAll();
            imagePool.DestroyAll();
            rawImagePool.DestroyAll();
        }
#endif
#endregion

#region private methods

        private void Bring()
        {
            foreach (Transform child in parent)
            {
                MaskableGraphic graphic = child.gameObject.GetComponent<MaskableGraphic>();
                if (graphic == null)
                {
                    continue;
                }

                InlineImageColorChangeFilter filter = child.gameObject.GetComponent<InlineImageColorChangeFilter>();

                if (filter == null)
                {
                    child.gameObject.AddComponent<InlineImageColorChangeFilter>();
                }

                if (graphic is Image)
                {
                    ResetRectTransform(child.transform as RectTransform);
                    imagePool.Bring(graphic as Image);
                }
                else if (graphic is RawImage)
                {
                    ResetRectTransform(child.transform as RectTransform);
                    rawImagePool.Bring(graphic as RawImage);
                }
            }
        }

        private T CreateObject<T>() where T : MaskableGraphic
        {
            GameObject gameObject = new GameObject("", typeof(RectTransform));
            gameObject.hideFlags |= HideFlags.DontSaveInEditor;
#if !JIGBOX_DEBUG && UNITY_EDITOR
            gameObject.hideFlags |= HideFlags.HideInHierarchy;
#endif
            gameObject.transform.SetParent(parent, false);
            ResetRectTransform(gameObject.transform as RectTransform);
            gameObject.AddComponent<InlineImageColorChangeFilter>();
            return gameObject.AddComponent<T>();
        }

        private static void ResetRectTransform(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                return;
            }
            rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
            rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.localScale = new Vector3(1, 1, 1);
        }

#endregion
    }
}
