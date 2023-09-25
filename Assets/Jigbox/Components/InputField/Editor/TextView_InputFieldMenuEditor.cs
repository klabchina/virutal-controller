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
    public static class TextView_InputFieldMenuEditor
    {
        public static readonly Color DefaultTextColor = new Color(50.0f / 255.0f, 50.0f / 255.0f, 50.0f / 255.0f, 1.0f);
        public static readonly Vector2 DefaultTextAreaOffsetMin = new Vector2(10, 6);
        public static readonly Vector2 DefaultTextAreaOffsetMax = new Vector2(-10, -7);
        public static readonly string DefaultPlaceholderText = "Enter text...";

        [MenuItem("GameObject/UI/Jigbox/InputField(TextView)")]
        public static void CreateTextView()
        {
            var go = CreateInputField();

            EditorMenuUtils.CreateUIObject(go);
        }

        public static GameObject CreateInputField()
        {
            GameObject root = new GameObject("InputField (TextView)", new[]
            {
                typeof(RectTransform),
                typeof(TextView_InputField),
            });

            var inputField = root.GetComponent<TextView_InputField>();
            inputField.InputField = inputField.GetComponent<InputFieldSub>();

            var background = UIContainerModifier.CreateRectTransformObject(root, "Background");
            UIContainerModifier.AddInputFieldBackgroundSprite(background);
            UIContainerModifier.SetStretchRectTransform(background);

            var textArea = UIContainerModifier.CreateRectTransformObject(root, "TextArea");
            var textAreaRect = UIContainerModifier.SetStretchRectTransform(textArea);
            textAreaRect.offsetMin = DefaultTextAreaOffsetMin;
            textAreaRect.offsetMax = DefaultTextAreaOffsetMax;
            textArea.AddComponent<RectMask2D>();
            inputField.TextViewport = textAreaRect;

            var placeholder = UIContainerModifier.CreateRectTransformObject(textArea, "Placeholder").AddComponent<TextView>();
            UIContainerModifier.SetStretchRectTransform(placeholder.gameObject);
            placeholder.Text = DefaultPlaceholderText;
            placeholder.IsItalic = true;
            var placeholderColor = DefaultTextColor;
            placeholderColor.a *= 0.5f;
            placeholder.color = placeholderColor;
            inputField.InputField.placeholder = placeholder;

            var text = UIContainerModifier.CreateRectTransformObject(textArea, "Text").AddComponent<Text>();
            UIContainerModifier.SetStretchRectTransform(text.gameObject);
            inputField.InputField.textComponent = text;
            text.supportRichText = false;
            text.color = Color.clear;
            var canvasGroup = text.gameObject.AddComponent<CanvasGroup>();
            // 頂点を生成しないように非表示
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            var textView = UIContainerModifier.CreateRectTransformObject(textArea, "TextView").AddComponent<InputTextView>();
            UIContainerModifier.SetStretchRectTransform(textView.gameObject);
            inputField.TextViewComponent = textView;
            textView.color = DefaultTextColor;
            textView.HorizontalOverflow = HorizontalWrapMode.Overflow;

            var extensibilityProvider = textView.gameObject.AddComponent<InputExtensibilityProvider>();
            textView.ExtensibilityProvider = extensibilityProvider;
            var textViewCaretController = textView.gameObject.AddComponent<TextViewCaretController>();
            textViewCaretController.TextView = textView;

            return root;
        }
    }
}
