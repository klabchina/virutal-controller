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
    public static class GaugeMenuEditor
    {
        /// <summary>生成時のゲージ横幅</summary>
        static readonly float GaugeWidth = 200.0f;

        /// <summary>生成時のゲージ縦幅</summary>
        static readonly float GaugeHeight = 40.0f;

        [MenuItem("GameObject/UI/Jigbox/Gauge")]
        public static void CreateGauge()
        {
            Vector2 size = new Vector2(GaugeWidth, GaugeHeight);

            // 本体
            GameObject gaugeObject = new GameObject("Gauge", typeof(RectTransform));
            RectTransform transform = gaugeObject.transform as RectTransform;
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

            // コンポーネント追加
            Gauge gauge = gaugeObject.AddComponent<Gauge>();
            SerializedObject serializedObject = new SerializedObject(gauge);

            // シリアライズ情報更新
            serializedObject.Update();

            SerializedProperty targetProperty = serializedObject.FindProperty("target");
            targetProperty.objectReferenceValue = foregroundTransform;

            SerializedProperty fillRectProperty = serializedObject.FindProperty("fillRect");
            fillRectProperty.objectReferenceValue = fillRectTransform;

            serializedObject.ApplyModifiedProperties();

            EditorMenuUtils.CreateUIObject(gaugeObject);
        }
    }
}
