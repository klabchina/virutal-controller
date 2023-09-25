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

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ToggleSwitch), true)]
    public class ToggleSwitchEditor : BasicToggleEditor
    {
#region protected methods

        /// <summary>
        /// Inspectorの表示を行います。
        /// </summary>
        protected override void DrawEditFields()
        {
            base.DrawEditFields();

            if (basicToggle.GetComponent<DragBehaviour>() == null)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Enable Drag"))
                    {
                        basicToggle.gameObject.AddComponent<DragBehaviour>();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

#endregion

#region override unity methods

        public override void OnEnable()
        {
            base.OnEnable();

            foreach (Object obj in targets)
            {
                ToggleSwitch toggle = obj as ToggleSwitch;
                SerializedObject toggleObject = new SerializedObject(toggle);
                toggleObject.Update();
                SerializedProperty buttonProperty = toggleObject.FindProperty("button");
                buttonProperty.objectReferenceValue = toggle.GetComponent<BasicButton>();
                toggleObject.ApplyModifiedProperties();
            }
        }

#endregion
    }
}
