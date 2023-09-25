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
    [CustomEditor(typeof(GestureInputEmulator))]
    public class GestureInputEmulatorEditor : Editor
    {
#region properties

        /// <summary>擬似入力の入力IDのプロパティ</summary>
        protected SerializedProperty dummyIdProperty;

        /// <summary>擬似入力を発生させるキーのプロパティ</summary>
        protected SerializedProperty keyProperty;

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            serializedObject.Update();

            SerializedProperty detectorProperty = serializedObject.FindProperty("detector");

            GestureDetector detector = (target as GestureInputEmulator).GetComponent<GestureDetector>();
            if (detectorProperty.objectReferenceValue != detector)
            {
                detectorProperty.objectReferenceValue = detector;
                serializedObject.ApplyModifiedProperties();
            }

            dummyIdProperty = serializedObject.FindProperty("dummyId");
            keyProperty = serializedObject.FindProperty("key");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            serializedObject.Update();

            int id = EditorGUILayout.IntField("Dummy Input Id", dummyIdProperty.intValue);
            if (id < 1)
            {
                id = 1;
            }
            if (dummyIdProperty.intValue != id)
            {
                dummyIdProperty.intValue = id;
            }
            EditorGUILayout.PropertyField(keyProperty);

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Gesture Input Emulator", GUI.changed, target);
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
