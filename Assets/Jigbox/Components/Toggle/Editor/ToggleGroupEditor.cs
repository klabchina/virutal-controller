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
using UnityEditorInternal;
using Jigbox.EditorUtils;
using Jigbox.Delegatable;

namespace Jigbox.Components
{
    [CustomEditor(typeof(ToggleGroup), true)]
    public class ToggleGroupEditor : Editor
    {
#region properties

        /// <summary>トグルグループ</summary>
        protected ToggleGroup toggleGroup;

        /// <summary>現在アクティブとなっているトグルコンポーネント</summary>
        protected SerializedProperty activeToggle;

        /// <summary>グループ対象のトグルコンポーネント</summary>
        protected SerializedProperty toggles;

        /// <summary>トグルグループの表示、編集用リスト</summary>
        protected ReorderableList toggleList;

        /// <summary>合成したGUI変更フラグ。</summary>
        bool compositedGUIChanged;

#endregion

#region protected methods

        /// <summary>
        /// Inspectorの表示を行います。
        /// </summary>
        protected virtual void DrawEditFields()
        {
            DrawDefaultInspector();

            if (EditorUtilsTools.DrawGroupHeader("Toggle Info", "TOGGLE_GROUP_INFO_KEY"))
            {
                EditorUtilsTools.FitContentToHeader();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    bool isChange = GUI.changed;
                    compositedGUIChanged |= GUI.changed;
                    GUI.changed = false;
                    toggleList.DoLayoutList();
                    if (GUI.changed)
                    {
                        SetValidator();
                    }
                    compositedGUIChanged |= GUI.changed;
                    GUI.changed = isChange;

                    var toggle = EditorGUILayout.ObjectField(
                                     "Active Toggle",
                                     activeToggle.objectReferenceValue,
                                     typeof(BasicToggle),
                                     true
                                 ) as BasicToggle;
                    if (toggle != activeToggle.objectReferenceValue)
                    {
                        activeToggle.objectReferenceValue = null;
                        // グループに含まれているトグルだった場合のみシリアライズする
                        for (int i = 0; i < toggles.arraySize; ++i)
                        {
                            SerializedProperty t = toggles.GetArrayElementAtIndex(i);
                            if (toggle == t.objectReferenceValue)
                            {
                                activeToggle.objectReferenceValue = toggle;
                                break;
                            }
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }
            
            GUILayout.Space(2.5f);

            DelegatableObjectEditor.DrawEditFields("On Active Toggle Changed", toggleGroup,
                toggleGroup.OnActiveToggleChangedDelegates, typeof(AuthorizedAccessAttribute),
                "ToggleGroup.OnActiveToggleChanged");
        }

        /// <summary>
        /// 自身のゲームオブジェクト以下に存在するBasicToggleを取得し、グループとして設定します。
        /// </summary>
        protected void FindToggles()
        {
            var toggleComponents = toggleGroup.GetComponentsInChildren<BasicToggle>();

            serializedObject.Update();

            for (int i = 0; i < toggleComponents.Length; ++i)
            {
                toggles.InsertArrayElementAtIndex(i);
                SerializedProperty element = toggles.GetArrayElementAtIndex(i);
                element.objectReferenceValue = toggleComponents[i];
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 参照が設定されているToggleのバリデータに自身を設定します。
        /// </summary>
        protected void SetValidator()
        {
            for (int i = 0; i < toggles.arraySize; ++i)
            {
                var element = toggles.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == null)
                {
                    continue;
                }
                var toggleObject = new SerializedObject(element.objectReferenceValue);
                toggleObject.Update();
                var validationTarget = toggleObject.FindProperty("valueChangeValidationTarget");
                validationTarget.objectReferenceValue = target;
                toggleObject.ApplyModifiedProperties();
            }
        }

#endregion

#region override unity methods

        public virtual void OnEnable()
        {
            toggleGroup = target as ToggleGroup;

            activeToggle = serializedObject.FindProperty("activeToggle");
            toggles = serializedObject.FindProperty("toggles");
            if (toggles.arraySize == 0)
            {
                FindToggles();
            }

            toggleList = new ReorderableList(serializedObject, toggles);
            toggleList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Toggles");
            toggleList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty toggle = toggles.GetArrayElementAtIndex(index);
                toggle.objectReferenceValue = EditorGUI.ObjectField(
                    rect,
                    "Toggle " + (index + 1),
                    toggle.objectReferenceValue,
                    typeof(BasicToggle),
                    true
                );
            };

            serializedObject.Update();
            SetValidator();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            compositedGUIChanged = false;

            serializedObject.Update();

            DrawEditFields();

            serializedObject.ApplyModifiedProperties();

            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo("Edit Toggle Group", compositedGUIChanged, toggleGroup);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
