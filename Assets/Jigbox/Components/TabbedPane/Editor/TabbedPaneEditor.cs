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
    [CustomEditor(typeof(TabbedPane), true)]
    public class TabbedPaneEditor : Editor
    {
#region properties

        /// <summary>タブドペイン</summary>
        protected TabbedPane tabbedPane;

        /// <summary>タブのプロパティを参照</summary>
        protected SerializedProperty tabsProperty;

        /// <summary>コンテンツのプロパティを参照</summary>
        protected SerializedProperty contentsProperty;

        /// <summary>ロックするCanvasGroupのプロパティを参照</summary>
        protected SerializedProperty lockCanvasGroupProperty;

#endregion

#region protected methods

        /// <summary>
        /// Inspectorの表示を行います。
        /// </summary>
        protected virtual void DrawEditFields()
        {
            EditorGUILayout.PropertyField(tabsProperty);

            var contents = EditorGUILayout.ObjectField(
                               "Contents",
                               contentsProperty.objectReferenceValue as MonoBehaviour,
                               typeof(MonoBehaviour),
                               true
                           ) as MonoBehaviour;
            if (contents != contentsProperty.objectReferenceValue)
            {
                if (contents == null)
                {
                    contentsProperty.objectReferenceValue = null;
                }
                else
                {
                    ITabbedPaneContents contentsComponent = contents.GetComponent<ITabbedPaneContents>();
                    contentsProperty.objectReferenceValue = contentsComponent as MonoBehaviour;
                    if (contentsComponent == null)
                    {
                        Debug.LogWarning("Tabbed pane contents need to has ITabbedPaneContents!");
                    }
                }
            }

            EditorGUILayout.PropertyField(lockCanvasGroupProperty);
        }

        /// <summary>
        /// 自身のゲームオブジェクト以下に存在するToggleGroupを取得し、タブとして設定します。
        /// </summary>
        protected void FindTabs()
        {
            var toggleGroupComponent = tabbedPane.GetComponentInChildren<ToggleGroup>();

            if (toggleGroupComponent == null)
            {
                return;
            }

            serializedObject.Update();

            tabsProperty.objectReferenceValue = toggleGroupComponent;

            serializedObject.ApplyModifiedProperties();
        }

#endregion

#region override unity methods

        public virtual void OnEnable()
        {
            tabbedPane = target as TabbedPane;

            tabsProperty = serializedObject.FindProperty("tabs");
            contentsProperty = serializedObject.FindProperty("contents");
            lockCanvasGroupProperty = serializedObject.FindProperty("lockCanvasGroup");

            if (tabsProperty.objectReferenceValue == null)
            {
                FindTabs();
            }
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();
            
            serializedObject.Update();

            DrawEditFields();

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Tabbed Pane", GUI.changed, tabbedPane);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
