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
using System.Reflection;
using Jigbox.EditorUtils;
using Jigbox.UIControl;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    public class MarqueeMenuEditor
    {
#region constants

        static readonly bool defaultPlayOnStart = true;

        static readonly float defaultSpeed = 100;

        static readonly bool defaultIsLoop = true;

        static readonly Vector2 verticalSize = new Vector2(40f, 200f);

        static readonly Vector2 horizontalSize = new Vector2(200f, 40f);

        static readonly Vector2 verticalPivot = new Vector2(0.5f, 1f);

        static readonly Vector2 horizontalPivot = new Vector2(0f, 0.5f);

        static readonly float exampleImageSize = 150f;

        static readonly Vector2 verticalExampleImageSize = new Vector2(30f, 150f);

        static readonly Vector2 horizontalExampleImageSize = new Vector2(150f, 30f);

        static readonly string defaultDllFilePath = "Library/ScriptAssemblies/Assembly-CSharp.dll";

        static readonly string jigboxDllFilePath = "Library/ScriptAssemblies/JigboxRuntime.dll";

#endregion

#region public methos

        [MenuItem("GameObject/UI/Jigbox/Marquee/Vertical")]
        public static void CreateMarqueeVertical()
        {
            // Marqueeコンポーネント本体
            Marquee marquee = CreateMarquee("MarqueeVertical", verticalSize);
            MarqueeTransitionBase transition = marquee.gameObject.AddComponent<MarqueeTransitionVertical>();

            // 背景
            GameObject background = CreateBackground();
            background.transform.SetParent(marquee.transform, false);

            // Viewport
            RectTransform viewport = CreateViewport(verticalPivot);
            viewport.transform.SetParent(marquee.transform, false);

            // Container(LayoutGroup)
            MarqueeViewVertical container = CreateMarqueeViewVertical();
            container.transform.SetParent(viewport.transform, false);

            // Example用となるContentを最初から配置する
            CreateExampleContent(container.transform, verticalExampleImageSize, RectTransform.Axis.Vertical);

            UpdateProperty(marquee, container, transition, viewport);
            EditorMenuUtils.CreateUIObject(marquee.gameObject);
        }

        [MenuItem("GameObject/UI/Jigbox/Marquee/Horizontal")]
        public static void CreateMarqueeHorizontal()
        {
            // Marqueeコンポーネント本体
            Marquee marquee = CreateMarquee("MarqueeHorizontal", horizontalSize);
            MarqueeTransitionBase transition = marquee.gameObject.AddComponent<MarqueeTransitionHorizontal>();

            // 背景
            GameObject background = CreateBackground();
            background.transform.SetParent(marquee.transform, false);

            // Viewport
            RectTransform viewport = CreateViewport(horizontalPivot);
            viewport.transform.SetParent(marquee.transform, false);

            // Container(LayoutGroup)
            MarqueeViewHorizontal container = CreateMarqueeViewHorizontal();
            container.transform.SetParent(viewport.transform, false);

            // Example用となるContentを最初から配置する
            CreateExampleContent(container.transform, horizontalExampleImageSize, RectTransform.Axis.Horizontal);

            UpdateProperty(marquee, container, transition, viewport);
            EditorMenuUtils.CreateUIObject(marquee.gameObject);
        }

        /// <summary>
        /// Marqueeコンポーネントの作成
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        static Marquee CreateMarquee(string name, Vector2 size)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            RectTransform marqueeTransform = go.transform as RectTransform;
            RectTransformUtils.SetSize(marqueeTransform, size);
            return go.AddComponent<Marquee>();
        }

        /// <summary>
        /// 背景用GameObjectの作成
        /// </summary>
        /// <returns></returns>
        public static GameObject CreateBackground()
        {
            GameObject background = new GameObject("Background");
            Image image = background.AddComponent<Image>();
            image.color = Color.white;
            RectTransform backgroundTransform = background.transform as RectTransform;
            RectTransformUtils.SetAnchor(backgroundTransform, RectTransformUtils.AnchorPoint.StretchFull);
            RectTransformUtils.SetSize(backgroundTransform, Vector2.zero);
            return background;
        }

        /// <summary>
        /// Viewportの作成
        /// </summary>
        /// <param name="pivot"></param>
        /// <returns></returns>
        static RectTransform CreateViewport(Vector2 pivot)
        {
            GameObject viewportGo = new GameObject("Viewport", typeof(RectTransform));
            viewportGo.AddComponent<RectMask2D>();
            viewportGo.AddComponent<CanvasGroup>();
            RectTransform viewportRectTransform = viewportGo.transform as RectTransform;
            RectTransformUtils.SetAnchor(viewportRectTransform, RectTransformUtils.AnchorPoint.StretchFull);
            RectTransformUtils.SetSize(viewportRectTransform, Vector2.zero);
            RectTransformUtils.SetPivot(viewportRectTransform, pivot);
            return viewportRectTransform;
        }

        /// <summary>
        /// VerticalのView用GameObject作成　
        /// </summary>
        /// <returns></returns>
        static MarqueeViewVertical CreateMarqueeViewVertical()
        {
            GameObject containerGo = new GameObject("Container", typeof(MarqueeViewVertical));
            MarqueeViewVertical layoutGroup = containerGo.GetComponent<MarqueeViewVertical>();
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childControlWidth = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandHeight = false;
            ContentSizeFitter contentSizeFitter = containerGo.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            RectTransform layoutGroupRectTransform = layoutGroup.transform as RectTransform;
            RectTransformUtils.SetAnchor(layoutGroupRectTransform, RectTransformUtils.AnchorPoint.StretchFull);
            RectTransformUtils.SetSize(layoutGroupRectTransform, Vector2.zero);
            RectTransformUtils.SetPivot(layoutGroupRectTransform, verticalPivot);
            return layoutGroup;
        }

        /// <summary>
        /// HorizontalのView用GameObject作成
        /// </summary>
        /// <returns></returns>
        static MarqueeViewHorizontal CreateMarqueeViewHorizontal()
        {
            GameObject containerGo = new GameObject("Container", typeof(MarqueeViewHorizontal));
            MarqueeViewHorizontal layoutGroup = containerGo.GetComponent<MarqueeViewHorizontal>();
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandHeight = true;
            ContentSizeFitter contentSizeFitter = containerGo.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            RectTransform layoutGroupRectTransform = layoutGroup.transform as RectTransform;
            RectTransformUtils.SetAnchor(layoutGroupRectTransform, RectTransformUtils.AnchorPoint.StretchFull);
            RectTransformUtils.SetSize(layoutGroupRectTransform, Vector2.zero);
            RectTransformUtils.SetPivot(layoutGroupRectTransform, horizontalPivot);
            return layoutGroup;
        }

        /// <summary>
        /// Example用のContentをつくり、Container配下になるよう設定します
        /// TextViewがあればTextViewを使用したContentを作りますが、ない場合はImageを置くようにします
        /// </summary>
        /// <param name="containerTransform"></param>
        static void CreateExampleContent(Transform containerTransform, Vector2 size, RectTransform.Axis axis)
        {
            GameObject textViewGo = CreateExampleTextView(axis);
            // TextViewがある場合はTextViewをContentとして配置して終わる
            if (textViewGo != null)
            {
                textViewGo.transform.SetParent(containerTransform, false);
                return;
            }

            // TextViewが無い場合はImageを2つ配置する
            Image image1 = CreateExampleImage("image1", Color.red, size, axis);
            image1.transform.SetParent(containerTransform, false);
            Image image2 = CreateExampleImage("image2", Color.blue, size, axis);
            image2.transform.SetParent(containerTransform, false);
        }

        static GameObject CreateExampleTextView(RectTransform.Axis axis)
        {
            Assembly asm = LoadTextViewAssembly();
            if (asm == null)
            {
                return null;
            }

            Type textViewType = asm.GetType("Jigbox.Components.TextView");
            Type textViewLayoutType = asm.GetType("Jigbox.Components.TextViewLayoutElement");
            // TextViewがない場合は作らない
            if (textViewType == null || textViewLayoutType == null)
            {
                return null;
            }

            GameObject go = new GameObject("TextView");
            go.AddComponent<MarqueeContent>();
            Component textView = go.AddComponent(textViewType);
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            // シリアライズでなく、プロパティ経由で設定する。プロパティのsetterにある処理を走らせたいため
            textViewType.GetProperty("Text", bindingFlags).SetValue(textView, "Marquee Text Example", null);
            // 背景を白にしているため文字の色は黒にする
            textViewType.GetProperty("color", bindingFlags).SetValue(textView, Color.black, null);

            RectTransform rectTransform = textViewType.GetProperty("rectTransform", bindingFlags).GetValue(textView, null) as RectTransform;
            if (axis == RectTransform.Axis.Horizontal)
            {
                // サイズ設定しているが、横幅はLayoutGroupにより調整されるため数値は変わる
                rectTransform.sizeDelta = horizontalSize;
            }
            else
            {
                // サイズを設定しているが、縦幅はLayoutGroupにより調整されため数値は変わる
                rectTransform.sizeDelta = verticalSize;
            }

            Component textViewLayout = go.AddComponent(textViewLayoutType);
            textViewLayoutType.GetField("textView", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(textViewLayout, textView);
            return go;
        }

        /// <summary>
        /// TextViewを含んでいるDllファイルを読み込みます
        /// </summary>
        /// <returns></returns>
        static Assembly LoadTextViewAssembly()
        {
            // Jigbox用のDLLにTextViewがあるかを確認
            var asm = LoadTextViewAssembly(jigboxDllFilePath);
            if (asm.GetType("Jigbox.Components.TextView") != null)
            {
                return asm;
            }

            // Assembly-CSharp.dll側にTextViewがあるかを確認
            asm = LoadTextViewAssembly(defaultDllFilePath);
            if (asm.GetType("Jigbox.Components.TextView") != null)
            {
                return asm;
            }

            return null;
        }

        /// <summary>
        /// 指定されたDLLファイルを読み込みます
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static Assembly LoadTextViewAssembly(string filePath)
        {
            Assembly asm;

            try
            {
                asm = Assembly.LoadFrom(filePath);
            }
            catch
            {
                return null;
            }

            return asm;
        }

        /// <summary>
        /// Example用のImageを設定します
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        static Image CreateExampleImage(string name, Color color, Vector2 size, RectTransform.Axis axis)
        {
            GameObject imageGo = new GameObject(name, typeof(Image));
            imageGo.AddComponent<MarqueeContent>();
            Image image = imageGo.GetComponent<Image>();
            image.color = color;
            RectTransform imageRectTransform = image.transform as RectTransform;
            RectTransformUtils.SetSize(imageRectTransform, size);
            LayoutElement layout = imageGo.AddComponent<LayoutElement>();
            // heightとwidthの両方を設定しているが、Axisの向きが一致していないものはLayoutGroupに調整されるためこの設定は片方に適用される
            if (axis == RectTransform.Axis.Horizontal)
            {
                layout.preferredWidth = exampleImageSize;
            }
            else
            {
                layout.preferredHeight = exampleImageSize;
            }

            return image;
        }

        /// <summary>
        /// MarqueeコンポーネントのSerializeFieldへ参照をつける
        /// </summary>
        /// <param name="marquee"></param>
        /// <param name="layoutGroup"></param>
        /// <param name="transition"></param>
        /// <param name="viewport"></param>
        static void UpdateProperty(Marquee marquee, HorizontalOrVerticalLayoutGroup layoutGroup, MarqueeTransitionBase transition, RectTransform viewport)
        {
            SerializedObject marqueeProperty = new SerializedObject(marquee);
            SerializedProperty layoutGroupProperty = marqueeProperty.FindProperty("layoutGroup");
            SerializedProperty transitionProperty = marqueeProperty.FindProperty("transition");
            SerializedProperty playOnStartProperty = marqueeProperty.FindProperty("playOnStart");
            SerializedProperty canvasGroupProperty = marqueeProperty.FindProperty("canvasGroup");
            SerializedProperty transitionPropertyProperty = marqueeProperty.FindProperty("transitionProperty");
            SerializedProperty speedProperty = transitionPropertyProperty.FindPropertyRelative("speed");
            SerializedProperty isLoopProperty = transitionPropertyProperty.FindPropertyRelative("isLoop");
            SerializedProperty viewportProperty = transitionPropertyProperty.FindPropertyRelative("viewport");

            marqueeProperty.Update();
            layoutGroupProperty.objectReferenceValue = layoutGroup;
            transitionProperty.objectReferenceValue = transition;
            playOnStartProperty.boolValue = defaultPlayOnStart;
            viewportProperty.objectReferenceValue = viewport;
            canvasGroupProperty.objectReferenceValue = viewport.GetComponent<CanvasGroup>();
            speedProperty.floatValue = defaultSpeed;
            isLoopProperty.boolValue = defaultIsLoop;
            marqueeProperty.ApplyModifiedProperties();
        }

#endregion
    }
}
