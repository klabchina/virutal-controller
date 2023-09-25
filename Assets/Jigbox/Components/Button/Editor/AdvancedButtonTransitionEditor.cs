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
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AdvancedButtonTransition), true)]
    public class AdvancedButtonTransitionEditor : Editor
    {
        protected SerializedProperty preset;
        protected AdvancedButtonTransitionPropertyEditor properties;
        protected AdvancedButtonTransitionPresetProviderEditor presetProvider;

        protected virtual void OnEnable()
        {
            properties = CreatePropertyEditor();
            presetProvider = CreatePresetProvider();
            presetProvider.Initialize();
            InitializeProperties();
            InitializePropertiesValue();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPreset();
            DrawProperties();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// プリセットパラメータ群の処理を担うクラスを生成します
        /// </summary>
        protected virtual AdvancedButtonTransitionPropertyEditor CreatePropertyEditor(SerializedObject serializedObject = null)
        {
            return new AdvancedButtonTransitionPropertyEditor(serializedObject ?? this.serializedObject);
        }

        /// <summary>
        /// プリセットに関する操作や処理を担うクラスを生成します
        /// </summary>
        protected virtual AdvancedButtonTransitionPresetProviderEditor CreatePresetProvider()
        {
            return new AdvancedButtonTransitionPresetProviderEditor(typeof(AdvancedButtonTransitionPreset));
        }

        /// <summary>
        /// 利用する SerializedProperty への参照取得を行います 
        /// </summary>
        protected virtual void InitializeProperties()
        {
            properties.InitializeProperties();
            preset = serializedObject.FindProperty("preset");
        }

        /// <summary>
        /// 利用する SerializedProperty の設定値の初期化を行います
        /// </summary>
        protected virtual void InitializePropertiesValue()
        {
            var defaultPreset = presetProvider.GetDefault();
            foreach (var obj in targets)
            {
                var serialized = new SerializedObject(obj);
                var buttonProperty = serialized.FindProperty("button");
                var presetProperty = serialized.FindProperty("preset");
                if (buttonProperty.objectReferenceValue == null)
                {
                    buttonProperty.objectReferenceValue = (obj as Component).GetComponent<ButtonBase>();
                    if (presetProperty.objectReferenceValue == null)
                    {
                        presetProperty.objectReferenceValue = defaultPreset;
                    }
                    serialized.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// プリセットに関連する Editor 表示処理
        /// </summary>
        protected virtual void DrawPreset()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                GUILayout.Label("Preset", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(preset, EditorGUIUtility.TrTempContent("ScriptableObject"));
                if (preset.objectReferenceValue == null && GUILayout.Button("New Preset"))
                {
                    var path = EditorUtility.SaveFilePanelInProject("Save Preset",
                        "ButtonTransitionPreset.asset",
                        "asset",
                        "プリセットの保存先を指定してください");
                    if (!string.IsNullOrEmpty(path))
                    {
                        var createdPreset = presetProvider.Create(path);
                        var createdPresetEditor = CreatePropertyEditor(new SerializedObject(createdPreset));
                        createdPresetEditor.InitializeProperties();
                        createdPresetEditor.SetProperties(properties);
                        if (preset.objectReferenceValue == null)
                        {
                            preset.objectReferenceValue = createdPreset;
                        }
                    }
                }
                if (presetProvider.GetDefault() == null)
                {
                    EditorGUILayout.HelpBox("プリセットを1つ以上作成することを推奨します", MessageType.Warning);
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// プリセット値に関連する Editor 表示処理
        /// </summary>
        protected virtual void DrawProperties()
        {
            if (preset.objectReferenceValue == null)
            {
                properties.DrawScale();
                properties.DrawColor();
                properties.DrawEffect();
            }
        }
    }
}
