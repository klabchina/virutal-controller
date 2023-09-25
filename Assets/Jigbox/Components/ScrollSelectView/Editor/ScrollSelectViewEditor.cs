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

using Jigbox.Delegatable;
using Jigbox.EditorUtils;
using Jigbox.UIControl;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    [CustomEditor(typeof(ScrollSelectViewBase), true)]
    public class ScrollSelectViewEditor : Editor
    {
#region properties

        /// <summary>ScrollSelectView</summary>
        protected ScrollSelectViewBase ScrollSelectView;

        /// <summary>LoopTypeのプロパティ</summary>
        protected SerializedProperty loopTypeProperty;

        /// <summary>selectedCellPositionRateのプロパティ</summary>
        protected SerializedProperty selectedCellPositionRateProperty;
        
        /// <summary>タイル外周の余白のプロパティ</summary>
        protected SerializedPadding paddingProperty;

        /// <summary>セルの面積のプロパティ</summary>
        protected SerializedProperty cellSizeProperty;

        /// <summary>セルの間隔のプロパティ</summary>
        protected SerializedProperty spacingProperty;

        /// <summary>選択セルのHead側の間隔プロパティ</summary>
        protected SerializedProperty headSpacingProperty;

        /// <summary>選択セルのTail側の間隔プロパティ</summary>
        protected SerializedProperty tailSpacingProperty;
        
        /// <summary>合成したGUI変更フラグ。</summary>
        protected bool compositedGUIChanged;

        /// <summary>CellSizeのラベル</summary>
        protected virtual string CellSizeLabel { get { return "Cell Size"; } }

#endregion

#region protected methods

        /// <summary>
        /// Viewport、ContentのAnchor、Pivotの状態をViewに合わせて固定します。
        /// </summary>
        protected virtual void LockAnchorAndPivot(SerializedProperty modelProperty)
        {
            SerializedProperty viewPivotProperty = modelProperty.FindPropertyRelative("viewPivot");

            // RequireComponentしているので絶対あるはず
            ScrollRect scrollRect = ScrollSelectView.GetComponent<ScrollRect>();

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

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            ScrollSelectView = target as ScrollSelectViewBase;
            SerializedProperty modelProperty = serializedObject.FindProperty("model");
            loopTypeProperty = modelProperty.FindPropertyRelative("loopType");
            selectedCellPositionRateProperty = modelProperty.FindPropertyRelative("selectedCellPositionRate");
            // paddingはVirtualCeollctionView側がもっているためmodelの値を直接いじれない
            paddingProperty = new SerializedPadding(serializedObject.FindProperty("padding"));
            cellSizeProperty = modelProperty.FindPropertyRelative("cellSize");
            spacingProperty = modelProperty.FindPropertyRelative("spacing");
            headSpacingProperty = modelProperty.FindPropertyRelative("selectedCellHeadSpacing");
            tailSpacingProperty = modelProperty.FindPropertyRelative("selectedCellTailSpacing");
            SearchRaycastValidator();

            LockAnchorAndPivot(modelProperty);
        }

        /// <summary>
        /// RaycastValidatorコンポーネントの参照をつける
        /// RaycastValidatorはUIControl以下のコンポーネントのため、ScrollSelectView側から付ける必要がある
        /// </summary>
        protected virtual void SearchRaycastValidator()
        {
            var raycastValidator = ScrollSelectView.GetComponent<RaycastValidator>();
            if (raycastValidator != null)
            {
                var raycastValidatorProperty = serializedObject.FindProperty("raycastValidator");
                if (raycastValidatorProperty.objectReferenceValue != raycastValidator)
                {
                    serializedObject.Update();
                    raycastValidatorProperty.objectReferenceValue = raycastValidator;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            compositedGUIChanged = false;
            
            serializedObject.Update();

            // LoopType
            // ランタイム中はインスペクター上からいじらせない
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.PropertyField(loopTypeProperty);
            EditorGUI.EndDisabledGroup();

            // selectedCellPositionRate
            // Sldierを使って0 ~ 1の範囲で指定されるように
            float positionRateValue = selectedCellPositionRateProperty.floatValue;
            positionRateValue = EditorGUILayout.Slider("Selected Cell Position Rate", positionRateValue, 0.0f, 1.0f);
            selectedCellPositionRateProperty.floatValue = positionRateValue;

            // Padding
            paddingProperty.EditProperty();

            // 値に制限をいれたいものをまとめて制限をいれていく
            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;

            // Cell Size
            EditorGUILayout.PropertyField(cellSizeProperty, new GUIContent(CellSizeLabel));

            // Spacing
            EditorGUILayout.PropertyField(spacingProperty);

            // Head Spacing
            EditorGUILayout.PropertyField(headSpacingProperty);

            // Tail Spacing
            EditorGUILayout.PropertyField(tailSpacingProperty);

            if (GUI.changed)
            {
                // CellSizeは1以上になるようにする
                cellSizeProperty.floatValue = Mathf.Max(1, cellSizeProperty.floatValue);
                // Spacingは0以上になるようにする
                spacingProperty.floatValue = Mathf.Max(0, spacingProperty.floatValue);
                headSpacingProperty.floatValue = Mathf.Max(0, headSpacingProperty.floatValue);
                tailSpacingProperty.floatValue = Mathf.Max(0, tailSpacingProperty.floatValue);
            }

            serializedObject.ApplyModifiedProperties();

            DelegatableObjectEditor.DrawEditFields(
                "On Update Cell",
                ScrollSelectView,
                ScrollSelectView.OnUpdateCellDelegates,
                typeof(AuthorizedAccessAttribute),
                "ScrollSelect.OnUpdateCell");

            DelegatableObjectEditor.DrawEditFields(
                "On Select Cell",
                ScrollSelectView,
                ScrollSelectView.OnSelect,
                typeof(AuthorizedAccessAttribute),
                "ScrollSelect.OnSelect");

            DelegatableObjectEditor.DrawEditFields(
                "On Deselect Cell",
                ScrollSelectView,
                ScrollSelectView.OnDeselect,
                typeof(AuthorizedAccessAttribute),
                "ScrollSelect.OnDeselect");

            DelegatableObjectEditor.DrawEditFields(
                "On Adjust Complete Cell",
                ScrollSelectView,
                ScrollSelectView.OnAdjustComplete,
                typeof(AuthorizedAccessAttribute),
                "ScrollSelect.OnAdjustComplete");
                
            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo("Edit ScrollSelectView", compositedGUIChanged, targets);
            
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
