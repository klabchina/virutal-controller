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
    [CustomEditor(typeof(TileViewBase), true)]
    public class TileViewEditor : Editor
    {
#region constants

        /// <summary>開閉状態保存用キーの先頭語</summary>
        protected static string KeyHeader = typeof(TileViewBase).ToString();

#endregion

#region properties

        /// <summary>TileView</summary>
        protected TileViewBase tileView;

        /// <summary>タイル外周の余白のプロパティ</summary>
        protected SerializedPadding paddingProperty;

        /// <summary>セルの面積のプロパティ</summary>
        protected SerializedVector2 cellSizeProperty;

        /// <summary>セルのピボットのプロパティ</summary>
        protected SerializedVector2 cellPivotProperty;

        /// <summary>セルの感覚のプロパティ</summary>
        protected SerializedVector2 spacingProperty;
        
        /// <summary>ヘッダ・フッタの設定に使うプロパティの参照</summary>
        protected SerializedProperty headerFooterProperty;

#endregion
        
#region protected methods

        /// <summary>
        /// Viewport、ContentのAnchor、Pivotの状態をViewに合わせて固定します。
        /// </summary>
        protected virtual void LockAnchorAndPivot()
        {
            SerializedProperty tileModelProperty = serializedObject.FindProperty("tile");
            SerializedProperty viewPivotProperty = tileModelProperty.FindPropertyRelative("viewPivot");

            // RequireComponentしているので絶対あるはず
            ScrollRect scrollRect = tileView.GetComponent<ScrollRect>();

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
        /// TileViewの拡張用コンポーネントを探索します。
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
                var layoutTarget = tileView.GetComponent<VirtualCollectionHeaderFooter>();
                if (layoutTarget != null)
                {
                    headerFooterProperty.objectReferenceValue = tileView.gameObject;
                }
            }
        }

#endregion
        
#region override unity methods

        protected virtual void OnEnable()
        {
            tileView = target as TileViewBase;

            paddingProperty = new SerializedPadding(serializedObject.FindProperty("padding"));
            cellSizeProperty = new SerializedVector2(serializedObject.FindProperty("cellSize"));
            cellPivotProperty = new SerializedVector2(serializedObject.FindProperty("cellPivot"));
            spacingProperty = new SerializedVector2(serializedObject.FindProperty("spacing"));
            headerFooterProperty = serializedObject.FindProperty("headerFooter");

            cellSizeProperty.ResetValue = Vector2.one;
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
            cellSizeProperty.EditProperty("Cell Size", "S", 60.0f);

            // Cell Pivot
            cellPivotProperty.EditProperty("Cell Pivot", "P", 60.0f);

            // Spacing
            spacingProperty.EditProperty("Spacing", "S", 60.0f);

            SearchExtendComponents();

            serializedObject.ApplyModifiedProperties();

            // Delegate
            DelegatableObjectEditor.DrawEditFields(
                "On Update Cell",
                target,
                tileView.OnUpdateCellDelegates,
                typeof(AuthorizedAccessAttribute),
                KeyHeader + "_OnUpdateCell");

            EditorUtilsTools.RegisterUndo("Edit TileView", GUI.changed, target);
            
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
