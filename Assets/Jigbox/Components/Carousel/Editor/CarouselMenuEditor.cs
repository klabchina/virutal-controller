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

using Jigbox.EditorUtils;
using Jigbox.UIControl;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    /// <summary>
    /// CarouselのMenuからの生成用のEditorScript
    /// </summary>
    public static class CarouselMenuEditor
    {
        static readonly Vector2 DefaultPivot = new Vector2(0.5f, 0.5f);
        static readonly Vector2 DefaultViewportSize = new Vector2(220f, 120f);
        static readonly Vector2 DefaultContentSize = new Vector2(200f, 100f);
        static readonly Vector2 DefaultContentSpacing = new Vector2(4f, 4f);

        [MenuItem("GameObject/UI/Jigbox/Carousel")]
        public static void CreateMenuItem()
        {
            // Carousel
            var carouselObj = CreateCarouselObject();

            // Viewport
            var viewportObj = CreateViewportObject(carouselObj);

            // Content
            CreateContentObject(viewportObj);

            // Hierarchy上に反映
            EditorMenuUtils.CreateUIObject(carouselObj);
        }

        /// <summary>
        /// CarouselのGameObjectを生成して返します
        /// </summary>
        /// <returns>CarouselコンポーネントがついたGameObject</returns>
        static GameObject CreateCarouselObject()
        {
            var obj = UIContainerModifier.CreateRectTransformObject(null, "Carousel");
            obj.AddComponent<Carousel>();
            obj.AddComponent<BasicCarouselTransition>();
            var rectTransform = obj.transform as RectTransform;
            RectTransformUtils.SetAnchor(rectTransform, DefaultPivot, DefaultPivot);
            RectTransformUtils.SetSize(rectTransform, DefaultViewportSize);
            return obj;
        }

        /// <summary>
        /// ViewportのGameObjectを生成して返します
        /// </summary>
        /// <returns>RectMask2DコンポーネントがついたGameObject</returns>
        static GameObject CreateViewportObject(GameObject parentObj)
        {
            var obj = UIContainerModifier.CreateRectTransformObject(parentObj, "Viewport", DefaultPivot);
            var rectTransform = obj.transform as RectTransform;
            RectTransformUtils.SetAnchor(rectTransform, DefaultPivot, DefaultPivot);
            RectTransformUtils.SetSize(rectTransform, DefaultViewportSize);
            obj.AddComponent<RectMask2D>();
            return obj;
        }

        /// <summary>
        /// ContentのGameObjectを生成して返します
        /// </summary>
        /// <returns>GridLayoutGroupコンポーネントがついたGameObject</returns>
        static GameObject CreateContentObject(GameObject parentObj)
        {
            var obj = UIContainerModifier.CreateRectTransformObject(parentObj, "Content", DefaultPivot);
            var rectTransform = obj.transform as RectTransform;
            RectTransformUtils.SetAnchor(rectTransform, DefaultPivot, DefaultPivot);
            RectTransformUtils.SetSize(rectTransform, DefaultViewportSize);
            var layout = obj.AddComponent<GridLayoutGroup>();
            layout.cellSize = DefaultContentSize;
            layout.spacing = DefaultContentSpacing;
            return obj;
        }
    }
}
