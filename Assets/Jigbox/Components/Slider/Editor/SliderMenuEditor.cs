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
using UnityEditor;
using UnityEngine.UI;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    public static class SliderMenuEditor
    {
        /// <summary>生成時のスライダーの横幅</summary>
        static readonly float SliderWidth = 200.0f;

        /// <summary>生成時のスライダーの縦幅</summary>
        static readonly float SliderHeight = 40.0f;

        /// <summary>生成時のハンドルのサイズ</summary>
        static readonly float HandleSize = 50.0f;

        [MenuItem("GameObject/UI/Jigbox/Slider")]
        public static void CreateSlider()
        {
            Vector2 size = new Vector2(SliderWidth, SliderHeight);

            // 本体
            GameObject sliderObject = new GameObject("Slider", typeof(RectTransform));
            RectTransform transform = sliderObject.transform as RectTransform;
            transform.sizeDelta = size;

            // 背景
            GameObject background = new GameObject("Background");
            Image image = UIContainerModifier.AddDefaultSprite(background);
            image.color = Color.gray;
            RectTransform backgroundTransform = background.transform as RectTransform;
            backgroundTransform.sizeDelta = size;
            backgroundTransform.SetParent(transform, false);

            // FillRect
            GameObject fillRect = new GameObject("FillRect", typeof(RectTransform));
            RectTransform fillRectTransform = fillRect.transform as RectTransform;
            fillRectTransform.sizeDelta = size;
            fillRectTransform.SetParent(transform, false);
            
            // ゲージ画像
            GameObject foreground = new GameObject("Foreground");
            UIContainerModifier.AddDefaultSprite(foreground);
            RectTransform foregroundTransform = foreground.transform as RectTransform;
            foregroundTransform.sizeDelta = size;
            foregroundTransform.SetParent(fillRectTransform, false);

            // ハンドル
            GameObject handle = new GameObject("Handle");
            UIContainerModifier.AddDefaultSprite(handle);
            RectTransform handleTransform = handle.transform as RectTransform;
            handleTransform.sizeDelta = new Vector2(HandleSize, HandleSize);
            handleTransform.SetParent(transform, false);

            // コンポーネント追加
            Slider slider = sliderObject.AddComponent<Slider>();
            SerializedObject serializedObject = new SerializedObject(slider);

            // シリアライズ情報更新
            serializedObject.Update();

            SerializedProperty targetProperty = serializedObject.FindProperty("target");
            targetProperty.objectReferenceValue = foregroundTransform;

            SerializedProperty fillRectProperty = serializedObject.FindProperty("fillRect");
            fillRectProperty.objectReferenceValue = fillRectTransform;

            SerializedProperty handleProperty = serializedObject.FindProperty("handle");
            handleProperty.objectReferenceValue = handleTransform;

            serializedObject.ApplyModifiedProperties();

            EditorMenuUtils.CreateUIObject(sliderObject);
        }
    }
}
