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
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    using FixedType = RectangleSizePreserver.FixedType;

    [CustomEditor(typeof(RectangleSizePreserver), true)]
    [CanEditMultipleObjects]
    public class RectangleSizePreserverEditor : Editor
    {
#region properties
        
        /// <summary>Updateで更新するかどうかのプロパティ</summary>
        SerializedProperty isUpdateProperty;

        /// <summary横幅の固定設定のプロパティ</summary>
        SerializedProperty fixWidthProperty;

        /// <summary>縦幅の固定設定のプロパティ</summary>
        SerializedProperty fixHeightProperty;

        /// <summary>固定する横幅のプロパティ</summary>
        SerializedProperty fixedSizeXProperty;

        /// <summary>固定する縦幅のプロパティ</summary>
        SerializedProperty fixedSizeYProperty;

        /// <summary>合成したGUI変更フラグ。</summary>
        bool compositedGUIChanged = false;

#endregion

#region override unity methods
        
        protected virtual void OnEnable()
        {
            isUpdateProperty = serializedObject.FindProperty("isUpdate");
            fixWidthProperty = serializedObject.FindProperty("fixWidth");
            fixHeightProperty = serializedObject.FindProperty("fixHeight");
            SerializedProperty property = serializedObject.FindProperty("fixedSize");
            fixedSizeXProperty = property.FindPropertyRelative("x");
            fixedSizeYProperty = property.FindPropertyRelative("y");
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            compositedGUIChanged = false;

            serializedObject.Update();

            EditorGUILayout.PropertyField(isUpdateProperty, new GUIContent("Is Update"));

            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;

            bool isFixWidth = fixWidthProperty.intValue != (int) FixedType.None;
            bool isFixHeight = fixHeightProperty.intValue != (int) FixedType.None;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(fixWidthProperty);
                if (isFixWidth)
                {
                    EditorGUILayout.PropertyField(fixedSizeXProperty, new GUIContent(""));
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(fixHeightProperty);
                if (isFixHeight)
                {
                    EditorGUILayout.PropertyField(fixedSizeYProperty, new GUIContent(""));
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUI.changed)
            {
                foreach (Object obj in serializedObject.targetObjects)
                {
                    RectangleSizePreserver preserver = obj as RectangleSizePreserver;
                    preserver.UpdateSize();
                }
                compositedGUIChanged |= GUI.changed;
                GUI.changed = false;
            }

            serializedObject.ApplyModifiedProperties();

            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo("Edit Size Presever", compositedGUIChanged, serializedObject.targetObjects);
            
            foreach (Object obj in serializedObject.targetObjects)
            {
                RectangleSizePreserver preserver = obj as RectangleSizePreserver;
                Vector2 anchorMin = preserver.RectTransform.anchorMin;
                Vector2 anchorMax = preserver.RectTransform.anchorMax;
                if ((isFixWidth && anchorMin.x != 0.0f && anchorMax.x != 1.0f)
                    || (isFixHeight && anchorMin.y != 0.0f && anchorMax.y != 1.0f))
                {
                    EditorGUILayout.HelpBox("Anchorが端に固定されていないため、正しく動作しない可能性があります。", MessageType.Warning);
                    break;
                }
            }

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
