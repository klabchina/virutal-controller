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
using Jigbox.EditorUtils;

namespace Jigbox.Examples
{
    [CustomEditor(typeof(LicenseInfo))]
    public class LicenseInfoEditor : Editor
    {
        private LicenseInfo licenseInfo;
        private SerializedProperty titleProperty;
        private SerializedProperty bodyProperty;

        void OnEnable()
        {
            this.licenseInfo = this.target as LicenseInfo;

            this.titleProperty = this.serializedObject.FindProperty("Title");
            this.bodyProperty = this.serializedObject.FindProperty("Body");
        }

        public override void OnInspectorGUI()
        {
            if (this.licenseInfo == null)
            {
                return;
            }

            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            this.serializedObject.Update();

            EditorGUILayout.LabelField("Title");
            this.licenseInfo.Title = EditorGUILayout.TextField(this.titleProperty.stringValue);

            EditorGUILayout.LabelField("Body");
            this.licenseInfo.Body = EditorGUILayout.TextArea(this.bodyProperty.stringValue);

            this.serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit License Info", GUI.changed, this.targets);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }
    }
}
