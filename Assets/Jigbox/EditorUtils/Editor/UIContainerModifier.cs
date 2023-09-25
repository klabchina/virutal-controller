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
using UnityEngine.UI;
using UnityEditor;

namespace Jigbox.EditorUtils
{
    /// <summary>
    /// UIC ontainer modifier.
    /// </summary>
    public static class UIContainerModifier
    {
        /// <summary>
        /// 兄弟の<see cref="GameObject"/>インスタンスと名前が重複しないように、UIオブジェクトを生成します
        /// </summary>
        /// <returns>The rect transform object.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="name">Name.</param>
        public static GameObject CreateRectTransformObject(GameObject parent, string name)
        {
            var objectName = parent ? GameObjectUtility.GetUniqueNameForSibling(parent.transform, name) : name;
            var go = new GameObject(objectName, typeof(RectTransform));
            go.layer = parent ? parent.layer : LayerMask.NameToLayer("UI");

            if (parent)
            {
                GameObjectUtility.SetParentAndAlign(go, parent);
            }
            // TODO parent が居ない場合 メインの Canvas を探して自動的につっこむぐらいは気を効かせたほうがいい
            return go;
        }

        /// <summary>
        /// 兄弟の<see cref="GameObject"/>インスタンスと名前が重複しないように、UIオブジェクトを生成します
        /// </summary>
        /// <returns>The rect transform object.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="name">Name.</param>
        /// <param name="stretch">If set to <c>true</c> stretch.</param>
        public static GameObject CreateRectTransformObject(GameObject parent, string name, bool stretch)
        {
            var go = CreateRectTransformObject(parent, name);

            if (stretch)
            {
                SetStretchRectTransform(go);
            }

            return go;
        }

        /// <summary>
        /// 兄弟の<see cref="GameObject"/>インスタンスと名前が重複しないように、UIオブジェクトを生成します
        /// </summary>
        /// <returns>The rect transform object.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="name">Name.</param>
        /// <param name="pivot">Pivot.</param>
        public static GameObject CreateRectTransformObject(GameObject parent, string name, Vector2 pivot)
        {
            var go = CreateRectTransformObject(parent, name);
            SetStretchRectTransform(go, pivot);

            return go;
        }

        /// <summary>
        /// 親オブジェクトにぴったりと収まるように <see cref="RectTransform"/> を調整します。<see cref="RectTransform.pivot"/> の値は <c>(0.5f, 0.5f)</c> です
        /// </summary>
        /// <returns></returns>
        /// <param name="go">GameObject</param>
        public static RectTransform SetStretchRectTransform(GameObject go)
        {
            return SetStretchRectTransform(go, new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// 親オブジェクトにぴったりと収まるように <see cref="RectTransform"/> を調整します。
        /// </summary>
        /// <returns>The stretch rect transform.</returns>
        /// <param name="go">GameObject</param>
        /// <param name="pivot">Pivot</param>
        public static RectTransform SetStretchRectTransform(GameObject go, Vector2 pivot)
        {
            var rt = go.transform as RectTransform;
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.pivot = pivot;
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
            }
            return rt;
        }
        
        /// <summary>
        /// uGUI ビルトインの<c>InputFieldBackground</c>スプライトを<see cref="Image"/>コンポーネントとして<see cref="GameObject"/>に追加します
        /// </summary>
        /// <returns>The background sprite.</returns>
        /// <param name="go">Go.</param>
        public static Image AddInputFieldBackgroundSprite(GameObject go)
        {
            return SetBuiltInSprite(go, "UI/Skin/InputFieldBackground.psd", Image.Type.Sliced);
        }
        
        /// <summary>
        /// uGUI ビルトインの<c>InputFieldBackground</c>スプライトを<see cref="Image"/>コンポーネントとして<see cref="GameObject"/>に追加します
        /// </summary>
        /// <returns>The background sprite.</returns>
        /// <param name="go">Go.</param>
        /// <param name="spriteColor">Sprite color.</param>
        public static Image AddInputFieldBackgroundSprite(GameObject go, Color spriteColor)
        {
            var image = AddInputFieldBackgroundSprite(go);
            image.color = spriteColor;
            return image;
        }

        /// <summary>
        /// uGUI ビルトインの<c>Background</c>スプライトを<see cref="Image"/>コンポーネントとして<see cref="GameObject"/>に追加します
        /// </summary>
        /// <returns>The background sprite.</returns>
        /// <param name="go">Go.</param>
        public static Image AddBackgroundSprite(GameObject go)
        {
            return SetBuiltInSprite(go, "UI/Skin/Background.psd", Image.Type.Sliced);
        }

        /// <summary>
        /// uGUI ビルトインの<c>Background</c>スプライトを<see cref="Image"/>コンポーネントとして<see cref="GameObject"/>に追加します
        /// </summary>
        /// <returns>The background sprite.</returns>
        /// <param name="go">Go.</param>
        /// <param name="spriteColor">Sprite color.</param>
        public static Image AddBackgroundSprite(GameObject go, Color spriteColor)
        {
            var image = AddBackgroundSprite(go);
            image.color = spriteColor;
            return image;
        }

        /// <summary>
        /// uGUI ビルトインの<c>UISprite</c>スプライトを<see cref="Image"/>コンポーネントとして<see cref="GameObject"/>に追加します
        /// </summary>
        /// <returns>The default sprite.</returns>
        /// <param name="go">Go.</param>
        public static Image AddDefaultSprite(GameObject go)
        {
            return SetBuiltInSprite(go, "UI/Skin/UISprite.psd", Image.Type.Sliced);
        }

        /// <summary>
        /// uGUI ビルトインの<c>UISprite</c>スプライトを<see cref="Image"/>コンポーネントとして<see cref="GameObject"/>に追加します
        /// </summary>
        /// <returns>The default sprite.</returns>
        /// <param name="go">Go.</param>
        /// <param name="spriteColor">Sprite color.</param>
        public static Image AddDefaultSprite(GameObject go, Color spriteColor)
        {
            var image = AddDefaultSprite(go);
            image.color = spriteColor;
            return image;
        }

        /// <summary>
        /// uGUI ビルトインの<c>UIMask</c>スプライトを<see cref="Image"/>コンポーネントとして<see cref="GameObject"/>に追加します
        /// </summary>
        /// <returns>The UIM ask sprite.</returns>
        /// <param name="go">Go.</param>
        public static Image AddUIMaskSprite(GameObject go)
        {
            return SetBuiltInSprite(go, "UI/Skin/UIMask.psd", Image.Type.Sliced);
        }

        static Image SetBuiltInSprite(GameObject go, string spritePath, Image.Type imageType)
        {
            var image = go.AddComponent<Image>();
            {
                image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(spritePath);
                image.type = imageType;
            }
            return image;
        }

    }
}
