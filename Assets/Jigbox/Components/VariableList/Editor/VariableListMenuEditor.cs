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
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    public static class VariableListMenuEditor
    {
#region Static Constants

        const int scrollbarSize = 16;

        const int scrollbarMargin = 8;

        static readonly Vector2 viewSize = new Vector2(400, 300);

        static readonly Padding padding = new Padding(scrollbarSize + scrollbarMargin);

        static readonly Vector2 verticalPivot = new Vector2(0.5f, 1.0f);

        static readonly Vector2 horizontalPivot = new Vector2(0.0f, 0.5f);

        static readonly Color backgroundImageColor = new Color(1f, 1f, 1f, (float) 0x20 / 0xFF);

#endregion

#region Internal

        static Image AddBackgroundImageSprite(GameObject go)
        {
            return UIContainerModifier.AddBackgroundSprite(go, backgroundImageColor);
        }

        static GameObject CreateVariableListGameObject(string name)
        {
            var go = UIContainerModifier.CreateRectTransformObject(null, name);
            go.AddComponent<CanvasRenderer>();
            EditorMenuUtils.CreateUIObject(go);

            var rt = go.transform as RectTransform;
            rt.sizeDelta = viewSize;

            AddBackgroundImageSprite(go);
            return go;
        }

        static RectTransform CreateViewport(GameObject parent, Vector2 pivot)
        {
            var go = UIContainerModifier.CreateRectTransformObject(parent, "Viewport", pivot);
            go.AddComponent<RectMask2D>();
            return go.transform as RectTransform;
        }

        static RectTransform CreateContent(GameObject parent, Vector2 pivot)
        {
            var go = UIContainerModifier.CreateRectTransformObject(parent, "Content", pivot);
            return go.transform as RectTransform;
        }

        static ScrollRect CreateScrollRect(GameObject go, Vector2 pivot)
        {
            var scrollRect = go.AddComponent<ScrollRect>();
            var viewport = CreateViewport(go, pivot);
            var content = CreateContent(viewport.gameObject, pivot);

            scrollRect.viewport = viewport;
            scrollRect.content = content;

            return scrollRect;
        }

        static GameObject CreateScrollbarHandle(GameObject parent)
        {
            var slidingArea = UIContainerModifier.CreateRectTransformObject(parent, "Sliding Area", true);
            var handle = UIContainerModifier.CreateRectTransformObject(slidingArea, "Handle", true);
            UIContainerModifier.AddDefaultSprite(handle);

            return handle;
        }

        static void SetScrollbarRectTransformVertical(GameObject go)
        {
            var rt = go.transform as RectTransform;
            rt.anchorMin = Vector2.right;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(1, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(scrollbarSize, 0);
        }

        static void SetScrollbarRectTransformHorizontal(GameObject go)
        {
            var rt = go.transform as RectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.right;
            rt.pivot = new Vector2(0.5f, 0.0f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(0, scrollbarSize);
        }

        static Scrollbar CreateScrollbarVertical(GameObject parent)
        {
            var go = UIContainerModifier.CreateRectTransformObject(parent, "Scrollbar Vertical");
            SetScrollbarRectTransformVertical(go);
            AddBackgroundImageSprite(go);

            var handle = CreateScrollbarHandle(go);

            var scrollbar = go.AddComponent<Scrollbar>();
            scrollbar.targetGraphic = handle.GetComponent<Image>();
            scrollbar.handleRect = handle.transform as RectTransform;
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            return scrollbar;
        }

        static Scrollbar CreateScrollbarHorizontal(GameObject parent)
        {
            var go = UIContainerModifier.CreateRectTransformObject(parent, "Scrollbar Horizontal");
            SetScrollbarRectTransformHorizontal(go);
            AddBackgroundImageSprite(go);

            var handle = CreateScrollbarHandle(go);

            var scrollbar = go.AddComponent<Scrollbar>();
            scrollbar.targetGraphic = handle.GetComponent<Image>();
            scrollbar.handleRect = handle.transform as RectTransform;
            scrollbar.direction = Scrollbar.Direction.LeftToRight;

            return scrollbar;
        }

        static void SetVariableListSetting(VariableListBase variableList)
        {
            variableList.Padding = padding;
        }

#endregion

        [MenuItem("GameObject/UI/Jigbox/VariableList/Vertical")]
        public static void CreateVariableListVertical()
        {
            var go = CreateVariableListGameObject("VariableList Vertical");

            var scrollRect = CreateScrollRect(go, verticalPivot);
            scrollRect.verticalScrollbar = CreateScrollbarVertical(go);

            var variableList = go.AddComponent<VariableListVertical>();
            SetVariableListSetting(variableList);
        }

        [MenuItem("GameObject/UI/Jigbox/VariableList/Horizontal")]
        public static void CreateVariableListHorizontal()
        {
            var go = CreateVariableListGameObject("VariableList Horizontal");

            var scrollRect = CreateScrollRect(go, horizontalPivot);
            scrollRect.horizontalScrollbar = CreateScrollbarHorizontal(go);

            var variableList = go.AddComponent<VariableListHorizontal>();
            SetVariableListSetting(variableList);
        }
    }
}
