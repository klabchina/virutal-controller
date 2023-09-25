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

using Jigbox.Delegatable;
using Jigbox.EditorUtils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    [CustomEditor(typeof(TextView_InputField))]
    public class TextView_InputFieldEditor : Editor
    {
        protected TextView_InputField inputField;

        protected float textAreaWidth;

        protected SerializedProperty textViewProperty;
        protected SerializedObject textViewObject;
        protected SerializedProperty languageTypeProperty;
        protected SerializedProperty textViewportProperty;

        protected SerializedProperty inputFieldSubProperty;
        protected SerializedObject inputFieldSubObject;
        protected SerializedProperty textComponentProperty;
        protected SerializedProperty inputTypeProperty;
        protected SerializedProperty keyboardTypeProperty;
        protected SerializedProperty characterLimitProperty;
        protected SerializedProperty caretBlinkRateProperty;
        protected SerializedProperty caretWidthProperty;
        protected SerializedProperty caretColorProperty;
        protected SerializedProperty customCaretColorProperty;
        protected SerializedProperty selectionColorProperty;
        protected SerializedProperty placeholderProperty;
        protected SerializedProperty readOnlyProperty;
        protected SerializedProperty shouldActivateOnSelectProperty;
        protected AnimBool customColorProperty;

        protected virtual void OnEnable()
        {
            inputField = (TextView_InputField) target;

            inputFieldSubProperty = serializedObject.FindProperty("inputFieldSub");
            if (inputFieldSubProperty.objectReferenceValue == null)
            {
                inputFieldSubProperty.objectReferenceValue = inputField.GetComponent<InputField>();
                serializedObject.ApplyModifiedProperties();
            }

            textViewProperty = serializedObject.FindProperty("textView");
            if (textViewProperty.objectReferenceValue != null)
            {
                textViewObject = new SerializedObject(textViewProperty.objectReferenceValue);
                languageTypeProperty = textViewObject.FindProperty("languageType");
            }

            textViewportProperty = serializedObject.FindProperty("textViewport");

            // InputFieldのプロパティを取得する
            inputFieldSubObject = new SerializedObject(inputFieldSubProperty.objectReferenceValue);
            textComponentProperty = inputFieldSubObject.FindProperty("m_TextComponent");
            inputTypeProperty = inputFieldSubObject.FindProperty("m_InputType");
            keyboardTypeProperty = inputFieldSubObject.FindProperty("m_KeyboardType");
            characterLimitProperty = inputFieldSubObject.FindProperty("m_CharacterLimit");
            caretBlinkRateProperty = inputFieldSubObject.FindProperty("m_CaretBlinkRate");
            caretWidthProperty = inputFieldSubObject.FindProperty("m_CaretWidth");
            caretColorProperty = inputFieldSubObject.FindProperty("m_CaretColor");
            customCaretColorProperty = inputFieldSubObject.FindProperty("m_CustomCaretColor");
            selectionColorProperty = inputFieldSubObject.FindProperty("m_SelectionColor");
            placeholderProperty = inputFieldSubObject.FindProperty("m_Placeholder");
            readOnlyProperty = inputFieldSubObject.FindProperty("m_ReadOnly");
#if UNITY_2020_3_OR_NEWER
            shouldActivateOnSelectProperty = inputFieldSubObject.FindProperty("m_ShouldActivateOnSelect");
#endif

            customColorProperty = new AnimBool(customCaretColorProperty.boolValue);
            customColorProperty.valueChanged.AddListener(Repaint);
        }

        protected virtual void OnDisable()
        {
            customColorProperty.valueChanged.RemoveListener(Repaint);
        }

        /// <summary>
        /// 色編集
        /// </summary>
        protected virtual void EditColor()
        {
            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f);

            EditorGUILayout.BeginVertical(GUI.skin.button);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    inputField.IsEnableColorChange = EditorGUILayout.ToggleLeft("Color", inputField.IsEnableColorChange, GUILayout.Width(120.0f));

                    if (inputField.IsEnableColorChange)
                    {
                        inputField.IsSyncColor = EditorGUILayout.ToggleLeft("Sync color", inputField.IsSyncColor);
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (inputField.IsEnableColorChange)
                {
                    inputField.DisableColor = EditorGUILayout.ColorField("DisableColor", inputField.DisableColor);
                }
            }
            EditorGUILayout.EndVertical();

            GUI.backgroundColor = Color.white;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            inputFieldSubObject.Update();
            textViewObject.Update();
            var textViewInputField = (TextView_InputField) target;

            if (inputFieldSubProperty.objectReferenceValue == null)
            {
                textViewInputField.InputField = textViewInputField.GetComponent<InputFieldSub>();
            }

            EditorGUI.BeginChangeCheck();
            var interactive = EditorGUILayout.Toggle("Enable", inputField.Interactive);
            if (EditorGUI.EndChangeCheck())
            {
                inputField.Interactive = interactive;
            }

            EditorGUILayout.PropertyField(inputFieldSubProperty, new GUIContent("Input Field"));

            EditorGUILayout.PropertyField(textViewProperty);
            InputTextView textView = null;
            if (textViewProperty.objectReferenceValue != null)
            {
                textView = (InputTextView) textViewProperty.objectReferenceValue;
            }

            EditorGUILayout.PropertyField(textViewportProperty);
            EditorGUILayout.PropertyField(textComponentProperty);

            EditColor();

            using (new EditorGUI.DisabledScope(textComponentProperty == null || textComponentProperty.objectReferenceValue == null))
            {
                EditorGUILayout.LabelField("Text");
                EditorGUI.BeginChangeCheck();
                var style = new GUIStyle(EditorStyles.textArea)
                {
                    wordWrap = true,
                };
                // CalcHeightを使用すると改行とテキストの折り返しを考慮した高さを返してくれる
                var height = EditorStyles.textArea.CalcHeight(new GUIContent(inputField.Text), textAreaWidth);
                height = Mathf.Max(style.lineHeight * 3, height);
                var text = EditorGUILayout.TextArea(inputField.Text, style, GUILayout.Height(height));
                if (EditorGUI.EndChangeCheck())
                {
                    inputField.Text = text;
                    if (textView != null)
                    {
                        textView.Text = inputField.Text;
                    }
                }

                Rect rect = GUILayoutUtility.GetLastRect();
                // いくつかの条件で、幅、高さが1のRect(デフォルト値っぽい)が返ってくるので
                // 正しい値が返ってきている場合のみ処理
                if (rect.width != 1.0f)
                {
                    // CalcHeightには引数としてwidth(TextAreaの横幅)が必要だが、レイアウト作成時に取得しようとしてもその情報は存在しないので
                    // 今回の描画結果の横幅を保持しておき、次回描画時の計算に使用する(結果1描画分だけ表示が遅れるが特別問題ないと認識している)
                    textAreaWidth = rect.width;
                }

                EditorGUILayout.PropertyField(characterLimitProperty);

                EditorGUILayout.Space();

                inputField.ContentType = (InputField.ContentType) EditorGUILayout.EnumPopup("Content Type", inputField.ContentType);
                {
                    EditorGUI.indentLevel++;

                    if (inputField.ContentType == InputField.ContentType.Standard ||
                        inputField.ContentType == InputField.ContentType.Autocorrected ||
                        inputField.ContentType == InputField.ContentType.Custom)
                    {
                        inputField.LineType = (InputField.LineType) EditorGUILayout.EnumPopup("Line Type", inputField.LineType);
                    }

                    if (inputField.ContentType == InputField.ContentType.Custom)
                    {
                        EditorGUILayout.PropertyField(inputTypeProperty);
                        EditorGUILayout.PropertyField(keyboardTypeProperty);
                        inputField.CharacterValidation = (InputField.CharacterValidation) EditorGUILayout.EnumPopup("Character Validation", inputField.CharacterValidation);
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(placeholderProperty);
                EditorGUILayout.PropertyField(caretBlinkRateProperty);
                EditorGUILayout.PropertyField(caretWidthProperty);

                EditorGUILayout.PropertyField(customCaretColorProperty);

                customColorProperty.target = customCaretColorProperty.boolValue;

                if (EditorGUILayout.BeginFadeGroup(customColorProperty.faded))
                {
                    EditorGUILayout.PropertyField(caretColorProperty);
                }

                EditorGUILayout.EndFadeGroup();

                EditorGUILayout.PropertyField(selectionColorProperty);
                EditorGUILayout.PropertyField(readOnlyProperty);
#if UNITY_2020_3_OR_NEWER
                EditorGUILayout.PropertyField(shouldActivateOnSelectProperty);
#endif

                EditorGUILayout.Space();
            }

            if (languageTypeProperty != null)
            {
                EditorGUILayout.PropertyField(languageTypeProperty);
            }

            textViewObject.ApplyModifiedProperties();
            inputFieldSubObject.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();

            DelegatableObjectEditor.DrawEditFields(
                "On Value Changed",
                inputField,
                inputField.OnValueChangedDelegates,
                typeof(AuthorizedAccessAttribute),
                "TextView_InputField.OnValueChanged");

            DelegatableObjectEditor.DrawEditFields(
                "On End Edit",
                inputField,
                inputField.OnEndEditDelegates,
                typeof(AuthorizedAccessAttribute),
                "TextView_InputField.OnEndEdit");

            EditorUtilsTools.RegisterUndo("Edit TextView_InputField", GUI.changed, target);
            EditorUtilsTools.RegisterUndo("Edit InputFieldSub", GUI.changed, inputField.InputField);

            EditorGUI.EndChangeCheck();
        }
    }
}
