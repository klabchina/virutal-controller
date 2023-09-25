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
using Jigbox.Delegatable;

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BasicToggle), true)]
    public class BasicToggleEditor : Editor
    {
#region properties

        /// <summary>トグル</summary>
        protected BasicToggle basicToggle;

        /// <summary>トグルのオン状態・オフ状態のプロパティを参照</summary>
        protected SerializedProperty isOnProperty;

        /// <summary>値更新バリデータのプロパティを参照</summary>
        protected SerializedProperty valueChangeValidationTargetProperty;

        /// <summary>デリゲートのプロパティ</summary>
        protected SerializedProperty onValueChangeDelegatesProperty;

#endregion

#region protected methods

        /// <summary>
        /// Inspectorの表示を行います。
        /// </summary>
        protected virtual void DrawEditFields()
        {
            isOnProperty.boolValue = EditorGUILayout.ToggleLeft(" Is On", isOnProperty.boolValue);

            GUILayout.Space(2.5f);

            DrawCopyDelegateButton();

            DelegatableObjectEditor.DrawEditFields("On Value Changed", basicToggle,
                basicToggle.OnValueChangedDelegates, typeof(AuthorizedAccessAttribute),
                "BasicToggle.OnValueChanged");

            GUILayout.Space(2.5f);
            
            var validationTarget = EditorGUILayout.ObjectField(
                                       "Validator",
                                       valueChangeValidationTargetProperty.objectReferenceValue as MonoBehaviour,
                                       typeof(MonoBehaviour),
                                       true
                                   ) as MonoBehaviour;
            if (validationTarget != valueChangeValidationTargetProperty.objectReferenceValue)
            {
                if (validationTarget == null)
                {
                    valueChangeValidationTargetProperty.objectReferenceValue = null;
                }
                else
                {
                    IToggleValueChangeValidator validator = validationTarget.GetComponent<IToggleValueChangeValidator>();
                    valueChangeValidationTargetProperty.objectReferenceValue = validator as MonoBehaviour;
                    if (validator == null)
                    {
                        Debug.LogWarning("Toggle's value change validation target need to has IToggleValueChangeValidator!");
                    }
                }
            }
        }

        protected virtual void DrawCopyDelegateButton()
        {
            if (serializedObject.targetObjects.Length > 1)
            {
                bool isCopy = false;
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.HelpBox("イベントの設定は、現在選択しているオブジェクトにのみ適用されます。", MessageType.Info);
                    isCopy = GUILayout.Button("選択中の\nイベントをコピー", GUILayout.Width(100.0f));
                }
                EditorGUILayout.EndHorizontal();

                // DelegatableList の中身のコピーを行う
                if (isCopy)
                {
                    DelegatableObjectEditor.CopyToDelegatableListProperty(basicToggle.OnValueChangedDelegates, onValueChangeDelegatesProperty);
                }
            }
        }

#endregion

#region override unity methods

        public virtual void OnEnable()
        {
            if (serializedObject.targetObjects.Length == 1)
            {
                basicToggle = target as BasicToggle;
            }
            else
            {
                if (Selection.activeGameObject != null)
                {
                    basicToggle = Selection.activeGameObject.GetComponent<BasicToggle>();
                }
            }
            isOnProperty = serializedObject.FindProperty("isOn");
            valueChangeValidationTargetProperty = serializedObject.FindProperty("valueChangeValidationTarget");
            onValueChangeDelegatesProperty = serializedObject.FindProperty("onValueChangedDelegates");
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            DrawEditFields();

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Toggle", GUI.changed, basicToggle);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
