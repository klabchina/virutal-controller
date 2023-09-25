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

using UnityEditor;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// AdvancedButtonTransition のプリセットパラメータ群の Editor 処理クラス
    /// </summary>
    public class AdvancedButtonTransitionPropertyEditor
    {
        protected SerializedProperty scaleEnabled;
        protected SerializedProperty scalePressValue;
        protected SerializedProperty scalePressDuration;
        protected SerializedProperty scalePressEasingType;
        protected SerializedProperty scalePressMotionType;
        protected SerializedProperty scaleReleaseValue;
        protected SerializedProperty scaleReleaseDuration;
        protected SerializedProperty scaleReleaseEasingType;
        protected SerializedProperty scaleReleaseMotionType;
        protected SerializedProperty colorEnabled;
        protected SerializedProperty colorPressValue;
        protected SerializedProperty colorPressDuration;
        protected SerializedProperty colorPressEasingType;
        protected SerializedProperty colorPressMotionType;
        protected SerializedProperty colorReleaseValue;
        protected SerializedProperty colorReleaseDuration;
        protected SerializedProperty colorReleaseEasingType;
        protected SerializedProperty colorReleaseMotionType;
        protected SerializedProperty effectEnabled;
        protected SerializedProperty effectTemplate;

        protected readonly SerializedObject serializedObject;
        protected readonly GUIContent temporaryLabel = new GUIContent();

        /// <summary>スケールに関する詳細設定の表示が有効かどうか</summary>
        protected static bool scaleAdvancedSetting;

        /// <summary>カラーに関する詳細設定の表示が有効かどうか</summary>
        protected static bool colorAdvancedSetting;

        public AdvancedButtonTransitionPropertyEditor(SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;
        }

        /// <summary>
        /// 利用する SerializedProperty への参照取得を行います 
        /// </summary>
        public virtual void InitializeProperties()
        {
            scaleEnabled = serializedObject.FindProperty("scaleEnabled");
            scalePressDuration = serializedObject.FindProperty("scalePressDuration");
            scalePressEasingType = serializedObject.FindProperty("scalePressEasingType");
            scalePressMotionType = serializedObject.FindProperty("scalePressMotionType");
            scalePressValue = serializedObject.FindProperty("scalePressValue");
            scaleReleaseDuration = serializedObject.FindProperty("scaleReleaseDuration");
            scaleReleaseEasingType = serializedObject.FindProperty("scaleReleaseEasingType");
            scaleReleaseMotionType = serializedObject.FindProperty("scaleReleaseMotionType");
            scaleReleaseValue = serializedObject.FindProperty("scaleReleaseValue");

            colorEnabled = serializedObject.FindProperty("colorEnabled");
            colorPressDuration = serializedObject.FindProperty("colorPressDuration");
            colorPressEasingType = serializedObject.FindProperty("colorPressEasingType");
            colorPressMotionType = serializedObject.FindProperty("colorPressMotionType");
            colorPressValue = serializedObject.FindProperty("colorPressValue");
            colorReleaseDuration = serializedObject.FindProperty("colorReleaseDuration");
            colorReleaseEasingType = serializedObject.FindProperty("colorReleaseEasingType");
            colorReleaseMotionType = serializedObject.FindProperty("colorReleaseMotionType");
            colorReleaseValue = serializedObject.FindProperty("colorReleaseValue");

            effectEnabled = serializedObject.FindProperty("effectEnabled");
            effectTemplate = serializedObject.FindProperty("effectTemplate");
        }

        /// <summary>
        /// 引数のプリセット値を適用する
        /// </summary>
        public virtual void SetProperties(AdvancedButtonTransitionPropertyEditor source)
        {
            serializedObject.Update();
            scaleEnabled.boolValue = source.scaleEnabled.boolValue;
            scalePressValue.vector2Value = source.scalePressValue.vector2Value;
            scalePressDuration.floatValue = source.scalePressDuration.floatValue;
            scalePressEasingType.enumValueIndex = source.scalePressEasingType.enumValueIndex;
            scalePressMotionType.enumValueIndex = source.scalePressMotionType.enumValueIndex;
            scaleReleaseValue.vector2Value = source.scaleReleaseValue.vector2Value;
            scaleReleaseDuration.floatValue = source.scaleReleaseDuration.floatValue;
            scaleReleaseEasingType.enumValueIndex = source.scaleReleaseEasingType.enumValueIndex;
            scaleReleaseMotionType.enumValueIndex = source.scaleReleaseMotionType.enumValueIndex;
            colorEnabled.boolValue = source.colorEnabled.boolValue;
            colorPressValue.colorValue = source.colorPressValue.colorValue;
            colorPressDuration.floatValue = source.colorPressDuration.floatValue;
            colorPressEasingType.enumValueIndex = source.colorPressEasingType.enumValueIndex;
            colorPressMotionType.enumValueIndex = source.colorPressMotionType.enumValueIndex;
            colorReleaseValue.colorValue = source.colorReleaseValue.colorValue;
            colorReleaseDuration.floatValue = source.colorReleaseDuration.floatValue;
            colorReleaseEasingType.enumValueIndex = source.colorReleaseEasingType.enumValueIndex;
            colorReleaseMotionType.enumValueIndex = source.colorReleaseMotionType.enumValueIndex;
            effectEnabled.boolValue = source.effectEnabled.boolValue;
            effectTemplate.objectReferenceValue = source.effectTemplate.objectReferenceValue;
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Scale に関する Editor 表示処理
        /// </summary>
        public virtual void DrawScale()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                ToggleLeft(scaleEnabled, " Scale", EditorStyles.boldLabel);
                if (scaleEnabled.boolValue && !scaleEnabled.hasMultipleDifferentValues)
                {
                    DrawScaleParts("Press", scalePressValue, scalePressDuration, scalePressEasingType, scalePressMotionType);
                    DrawScaleParts("Release", scaleReleaseValue, scaleReleaseDuration, scaleReleaseEasingType, scaleReleaseMotionType);
                    scaleAdvancedSetting = GUILayout.Toggle(scaleAdvancedSetting, "Advanced Setting", GUI.skin.button);
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Color に関する Editor 表示処理
        /// </summary>
        public virtual void DrawColor()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                ToggleLeft(colorEnabled, " Color", EditorStyles.boldLabel);
                if (colorEnabled.boolValue && !colorEnabled.hasMultipleDifferentValues)
                {
                    DrawColorParts("Press", colorPressValue, colorPressDuration, colorPressEasingType, colorPressMotionType);
                    DrawColorParts("Release", colorReleaseValue, colorReleaseDuration, colorReleaseEasingType, colorReleaseMotionType);
                    colorAdvancedSetting = GUILayout.Toggle(colorAdvancedSetting, "Advanced Setting", GUI.skin.button);
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Effect に関する Editor 表示処理
        /// </summary>
        public virtual void DrawEffect()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            {
                ToggleLeft(effectEnabled, " Effect", EditorStyles.boldLabel);
                if (effectEnabled.boolValue && !effectEnabled.hasMultipleDifferentValues)
                {
                    EditorGUILayout.PropertyField(effectTemplate, TempContent("Duration"));
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Scale に関する TweenBase 値の Editor 表示処理
        /// </summary>
        protected virtual void DrawScaleParts(string title, SerializedProperty value, SerializedProperty duration, SerializedProperty easing, SerializedProperty motion)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                if (!scaleAdvancedSetting)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = value.hasMultipleDifferentValues || value.vector2Value.x != value.vector2Value.y;
                    var scaleValue = EditorGUILayout.FloatField("Scale", (value.vector2Value.x + value.vector2Value.y) / 2);
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        value.vector2Value = new Vector2(scaleValue, scaleValue);
                    }
                    EditorGUILayout.PropertyField(duration, TempContent("Duration"));
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = value.hasMultipleDifferentValues;
                    var vector2 = new Vector2(
                        EditorGUILayout.FloatField("ScaleX", value.vector2Value.x),
                        EditorGUILayout.FloatField("ScaleY", value.vector2Value.y)
                    );
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        value.vector2Value = vector2;
                    }
                    EditorGUILayout.PropertyField(duration, TempContent("Duration"));
                    EditorGUILayout.PropertyField(easing, TempContent("Easing"));
                    EditorGUILayout.PropertyField(motion, TempContent("Motion"));
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Color に関する TweenBase 値の Editor 表示処理
        /// </summary>
        protected virtual void DrawColorParts(string title, SerializedProperty value, SerializedProperty duration, SerializedProperty easing, SerializedProperty motion)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(value, TempContent("Color"));
                EditorGUILayout.PropertyField(duration, TempContent("Duration"));
                if (colorAdvancedSetting)
                {
                    EditorGUILayout.PropertyField(easing, TempContent("Easing"));
                    EditorGUILayout.PropertyField(motion, TempContent("Motion"));
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// EditorGUIUtility.TrTempContent の自前実装
        /// </summary>
        /// <remarks>自前実装している理由はローカライズ処理を噛ませたくないから</remarks>
        protected GUIContent TempContent(string text)
        {
            temporaryLabel.image = null;
            temporaryLabel.text = text;
            temporaryLabel.tooltip = null;
            return temporaryLabel;
        }

        /// <summary>
        /// EditorGUILayout.ToggleLeft の複数選択時の表示対応版
        /// </summary>
        static void ToggleLeft(SerializedProperty property, string label, GUIStyle labelStyle = null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            var value = EditorGUILayout.ToggleLeft(label, property.boolValue, labelStyle ?? EditorStyles.label);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = value;
            }
        }
    }
}
