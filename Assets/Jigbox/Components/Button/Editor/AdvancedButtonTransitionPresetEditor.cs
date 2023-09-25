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
    [CustomEditor(typeof(AdvancedButtonTransitionPreset))]
    public class AdvancedButtonTransitionPresetEditor : Editor
    {
        protected SerializedProperty isDefault;
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
            isDefault = serializedObject.FindProperty("isDefault");
        }

        /// <summary>
        /// 利用する SerializedProperty の設定値の初期化を行います
        /// </summary>
        protected virtual void InitializePropertiesValue()
        {
            serializedObject.Update();
            presetProvider.NormalizeDefaults(target);
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// プリセットに関連する Editor 表示処理
        /// </summary>
        protected virtual void DrawPreset()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                var editable = targets.Length == 1 && !isDefault.boolValue;
                EditorGUI.BeginDisabledGroup(!editable);
                {
                    var text = "この設定を既定のプリセットにする";
                    if (targets.Length > 1)
                    {
                        text = "既定のプリセットは一つのみ設定可能です";
                    }
                    else if (isDefault.boolValue)
                    {
                        text = "この設定が既定のプリセットです";
                    }
                    if (GUILayout.Button(text))
                    {
                        presetProvider.SetDefault(target);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// プリセット値に関連する Editor 表示処理
        /// </summary>
        protected virtual void DrawProperties()
        {
            properties.DrawScale();
            properties.DrawColor();
            properties.DrawEffect();
        }
    }
}
