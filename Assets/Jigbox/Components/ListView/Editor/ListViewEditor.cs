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
using UnityEngine.UI;
using UnityEditor;
using Jigbox.EditorUtils;
using Jigbox.UIControl;
using Jigbox.Delegatable;

namespace Jigbox.Components
{
    [CustomEditor(typeof(ListViewBase), true)]
    public class ListViewEditor : Editor
    {
#region properties

        /// <summary>ListView</summary>
        protected ListViewBase listView;

        /// <summary>タイル外周の余白のプロパティ</summary>
        protected SerializedPadding paddingProperty;

        /// <summary>セルの面積のプロパティ</summary>
        protected SerializedProperty cellSizeProperty;

        /// <summary>セルのピボットのプロパティ</summary>
        protected SerializedVector2 cellPivotProperty;

        /// <summary>セルの感覚のプロパティ</summary>
        protected SerializedProperty spacingProperty;

        /// <summary>CellSizeのラベル</summary>
        protected virtual string CellSizeLabel { get { return "Cell Size"; } }

        /// <summary>ヘッダ・フッタの設定に使うプロパティの参照</summary>
        protected SerializedProperty headerFooterProperty;

#endregion

#region protected methods

        /// <summary>
        /// Viewport、ContentのAnchor、Pivotの状態をViewに合わせて固定します。
        /// </summary>
        protected virtual void LockAnchorAndPivot()
        {
            SerializedProperty listModelProperty = serializedObject.FindProperty("model");
            SerializedProperty viewPivotProperty = listModelProperty.FindPropertyRelative("viewPivot");

            // RequireComponentしているので絶対あるはず
            ScrollRect scrollRect = listView.GetComponent<ScrollRect>();

            RectTransform viewport = scrollRect.viewport;
            if (viewport != null)
            {
                RectTransformUtils.SetAnchor(viewport, RectTransformUtils.AnchorPoint.StretchFull);
                RectTransformUtils.SetPivot(viewport, viewPivotProperty.vector2Value);
            }

            RectTransform content = scrollRect.content;
            if (content != null)
            {
                RectTransformUtils.SetAnchor(content, RectTransformUtils.AnchorPoint.StretchFull);
                RectTransformUtils.SetPivot(content, viewPivotProperty.vector2Value);
            }
        }

        /// <summary>
        /// ListViewの拡張用コンポーネントを探索します。
        /// </summary>
        protected virtual void SearchExtendComponents()
        {
            // 複数同時選択時は参照をつけると自身のコンポーネントでないものが参照されてしまうので参照を設定しない
            if (serializedObject.targetObjects.Length > 1)
            {
                return;
            }

            if (headerFooterProperty.objectReferenceValue == null)
            {
                var layoutTarget = listView.GetComponent<VirtualCollectionHeaderFooter>();
                if (layoutTarget != null)
                {
                    headerFooterProperty.objectReferenceValue = listView.gameObject;
                }
            }
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            listView = target as ListViewBase;

            paddingProperty = new SerializedPadding(serializedObject.FindProperty("padding"));
            cellSizeProperty = serializedObject.FindProperty("cellSize");
            cellPivotProperty = new SerializedVector2(serializedObject.FindProperty("cellPivot"));
            spacingProperty = serializedObject.FindProperty("spacing");
            headerFooterProperty = serializedObject.FindProperty("headerFooter");

            cellPivotProperty.ResetValue = new Vector2(0.5f, 0.5f);

            LockAnchorAndPivot();
            SearchExtendComponents();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            serializedObject.Update();

            // Padding
            paddingProperty.EditProperty();

            // Cell Size
            EditorGUILayout.PropertyField(cellSizeProperty, new GUIContent(CellSizeLabel));

            // Cell Pivot
            cellPivotProperty.EditProperty("Cell Pivot", "P", 55.0f);

            // Spacing
            EditorGUILayout.PropertyField(spacingProperty);

            SearchExtendComponents();

            serializedObject.ApplyModifiedProperties();

            DelegatableObjectEditor.DrawEditFields(
                "On Update Cell", 
                listView,
                listView.OnUpdateCellDelegates,
                typeof(AuthorizedAccessAttribute),
                "ListView.OnUpdateCell");
            
            EditorUtilsTools.RegisterUndo("Edit ListView",GUI.changed , targets);
            
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
