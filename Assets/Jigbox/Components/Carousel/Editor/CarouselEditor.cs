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
    /// <summary>
    /// CarouselのEditorScript
    /// </summary>
    [CustomEditor(typeof(Carousel), true)]
    public class CarouselEditor : Editor
    {
#region constants

        /// <summary>
        /// StartCornerに設定する値
        /// </summary>
        protected static readonly GridLayoutGroup.Corner FixedStartCorner = GridLayoutGroup.Corner.UpperLeft;

        /// <summary>
        /// ChildAlignmentに設定する値
        /// </summary>
        protected static readonly TextAnchor FixedChildAlignment = TextAnchor.MiddleCenter;

        /// <summary>
        /// AxisがHorizontalの場合Constraintに設定する値
        /// </summary>
        protected static readonly GridLayoutGroup.Constraint FixedHorizontalConstraint = GridLayoutGroup.Constraint.FixedRowCount;

        /// <summary>
        /// AxisがVerticalの場合Constraintに設定する値
        /// </summary>
        protected static readonly GridLayoutGroup.Constraint FixedVerticalConstraint = GridLayoutGroup.Constraint.FixedColumnCount;

        /// <summary>
        /// ConstraintCountに設定する値
        /// </summary>
        protected static readonly int FixedConstraintCount = 1;

        /// <summary>
        /// GridLayoutGroupとその親のPivotに設定する値
        /// </summary>
        protected static readonly Vector2 FixedPivot = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// 開閉状態保存用キーの先頭語
        /// </summary>
        protected static readonly string KeyHeader = typeof(Carousel).ToString();

#endregion

#region properties

        /// <summary>
        /// Carousel
        /// </summary>
        protected Carousel carousel;

        /// <summary>
        /// GridLayoutGroup(Content)のRectTransform
        /// </summary>
        protected RectTransform layoutRectTransform;

        /// <summary>
        /// GridLayoutGroupのparent(Viewport)のRectTransform
        /// </summary>
        protected RectTransform layoutParentRectTransform;

        /// <summary>
        /// Cell用のGridLayoutGroupのSerializedProperty
        /// </summary>
        protected SerializedProperty layoutProperty;

        /// <summary>
        /// BulletController用のGridLayoutGroupのSerializedProperty
        /// </summary>
        protected SerializedProperty bulletControllerProperty;

        /// <summary>
        /// LoopTypeのSerializedProperty
        /// </summary>
        protected SerializedProperty loopTypeProperty;

        /// <summary>
        /// AutoScrollIntervalのSerializedProperty
        /// </summary>
        protected SerializedProperty autoScrollIntervalProperty;

        /// <summary>
        /// GridLayoutGroupのSerializedObject
        /// </summary>
        protected SerializedObject gridLayoutObject;

        /// <summary>
        /// StartCornerのSerializedProperty
        /// </summary>
        protected SerializedProperty startCornerProperty;

        /// <summary>
        /// StartAxisのSerializedProperty
        /// </summary>
        protected SerializedProperty startAxisProperty;

        /// <summary>
        /// ChildAlignmentのSerializedProperty
        /// </summary>
        protected SerializedProperty childAlignmentProperty;

        /// <summary>
        /// ConstraintのSerializedProperty
        /// </summary>
        protected SerializedProperty constraintProperty;

        /// <summary>
        /// constraintCountのSerializedProperty
        /// </summary>
        protected SerializedProperty constraintCountProperty;

        /// <summary>
        /// 現在選択されているAxis
        /// </summary>
        protected GridLayoutGroup.Axis selectedAxis = GridLayoutGroup.Axis.Horizontal;
        
        /// <summary>
        /// 合成したGUI変更フラグ。
        /// </summary>
        protected bool compositedGUIChanged;

        /// <summary>
        /// フリック操作を行うかどうか
        /// </summary>
        protected SerializedProperty isControllableFlickProperty;
        
        /// <summary>
        /// フリックが有効となるピクセル単位の距離の閾値
        /// </summary>
        protected SerializedProperty flickMovementThresholdProperty;

        /// <summary>
        /// フリックが有効となる加速度の閾値
        /// </summary>
        protected SerializedProperty flickAccelerationThresholdProperty;

        /// <summary>
        /// フリックが有効となる時間の閾値
        /// </summary>
        protected SerializedProperty flickTimeThresholdProperty;

#endregion

#region protected methods

        /// <summary>
        /// Axis情報の描画、更新を行います
        /// </summary>
        protected virtual void DrawAxis()
        {
            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            selectedAxis = (GridLayoutGroup.Axis) EditorGUILayout.EnumPopup("Axis", (GridLayoutGroup.Axis) startAxisProperty.enumValueIndex);
            EditorGUI.EndDisabledGroup();

            // CarouselEditor上で変更されたとき
            if (GUI.changed)
            {
                ValidateCellLayoutGroupByAxis();
            }
        }

        /// <summary>
        /// CellLayoutGroupの参照を探します
        /// </summary>
        protected virtual void FindCellLayoutGroup()
        {
            // 自身の配下にGridLayoutGroupがついてるかどうかチェックをする
            var component = carousel.GetComponentInChildren<GridLayoutGroup>();
            if (component != null)
            {
                // あれば参照を持たせる
                serializedObject.Update();
                layoutProperty.objectReferenceValue = component;
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                // なければWarningを出力する
                Debug.LogWarning("Reference to the GridLayoutGroup is mandatory.", carousel.gameObject);
            }
        }

        /// <summary>
        /// CellLayoutGroupにまつわる情報をキャッシュする
        /// </summary>
        protected virtual void CacheCellLayoutGroupDatas()
        {
            var layoutObj = layoutProperty.objectReferenceValue as GridLayoutGroup;
            if (layoutObj != null)
            {
                gridLayoutObject = new SerializedObject(layoutObj);
                startCornerProperty = gridLayoutObject.FindProperty("m_StartCorner");
                startAxisProperty = gridLayoutObject.FindProperty("m_StartAxis");
                childAlignmentProperty = gridLayoutObject.FindProperty("m_ChildAlignment");
                constraintProperty = gridLayoutObject.FindProperty("m_Constraint");
                constraintCountProperty = gridLayoutObject.FindProperty("m_ConstraintCount");
                selectedAxis = (GridLayoutGroup.Axis) startAxisProperty.enumValueIndex;
                layoutRectTransform = layoutObj.GetComponent<RectTransform>();
                layoutParentRectTransform = layoutObj.transform.parent.GetComponent<RectTransform>();
            }
            else
            {
                AllCacheClearCellLayoutGroupDatas();
            }
        }

        /// <summary>
        /// CellLayoutGroupにまつわるキャッシュをクリアする
        /// </summary>
        protected virtual void AllCacheClearCellLayoutGroupDatas()
        {
            startCornerProperty = null;
            startAxisProperty = null;
            childAlignmentProperty = null;
            constraintProperty = null;
            constraintCountProperty = null;
            layoutParentRectTransform = null;
            layoutRectTransform = null;
            selectedAxis = GridLayoutGroup.Axis.Horizontal;
        }

        /// <summary>
        /// Axisの情報を基にして各種値をValidateします
        /// </summary>
        protected virtual void ValidateCellLayoutGroupByAxis()
        {
            // 参照がない場合は即終了
            if (layoutProperty.objectReferenceValue == null)
            {
                return;
            }

            var isChanged = startCornerProperty.enumValueIndex != (int) FixedStartCorner
                || startAxisProperty.enumValueIndex != (int) selectedAxis
                || childAlignmentProperty.enumValueIndex != (int) FixedChildAlignment
                || constraintCountProperty.intValue != FixedConstraintCount;

            // 変更がかかっていれば更新する
            if (isChanged)
            {
                gridLayoutObject.Update();

                startCornerProperty.enumValueIndex = (int) FixedStartCorner;
                startAxisProperty.enumValueIndex = (int) selectedAxis;
                childAlignmentProperty.enumValueIndex = (int) FixedChildAlignment;
                constraintProperty.enumValueIndex = (int) (selectedAxis == GridLayoutGroup.Axis.Horizontal ? FixedHorizontalConstraint : FixedVerticalConstraint);
                constraintCountProperty.intValue = FixedConstraintCount;

                gridLayoutObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// layoutParentRectTransformとlayoutRectTransformに対してValidateを行います
        /// </summary>
        protected virtual void ValidatePivot()
        {
            // GridLayoutGroupのparentのPivotチェック
            if (layoutParentRectTransform != null && layoutParentRectTransform.pivot != FixedPivot)
            {
                RectTransformUtils.SetPivot(layoutParentRectTransform, FixedPivot);
            }
            // GridLayoutGroupのPivotチェック
            if (layoutRectTransform != null && layoutRectTransform.pivot != FixedPivot)
            {
                RectTransformUtils.SetPivot(layoutRectTransform, FixedPivot);
            }
        }

        /// <summary>
        /// AutoScrollIntervalに対してValidateを行います
        /// </summary>
        protected virtual void ValidateAutoScrollInterval()
        {
            var rowValue = autoScrollIntervalProperty.floatValue;
            var clampValue = Mathf.Clamp(rowValue, 0f, rowValue);
            if (rowValue != clampValue)
            {
                // 0以下の値が設定されていた場合は0を適用する
                autoScrollIntervalProperty.floatValue = clampValue;
            }
        }

        /// <summary>
        /// 各SerializedPropertyなどの値をキャッシュします
        /// </summary>
        protected virtual void CacheData()
        {
            // CellLayoutGroup
            layoutProperty = serializedObject.FindProperty("cellLayoutGroup");
            if (layoutProperty.objectReferenceValue == null)
            {
                // 参照がなければ探す
                FindCellLayoutGroup();
            }
            CacheCellLayoutGroupDatas();

            // LoopType
            loopTypeProperty = serializedObject.FindProperty("loopType");

            // AutoScrollInterval
            autoScrollIntervalProperty = serializedObject.FindProperty("autoScrollInterval");

            // BulletController
            bulletControllerProperty = serializedObject.FindProperty("bulletController");

            // フリック関連プロパティ
            isControllableFlickProperty = serializedObject.FindProperty("IsControllableFlick");
            flickMovementThresholdProperty = serializedObject.FindProperty("FlickMovementThreshold");
            flickAccelerationThresholdProperty = serializedObject.FindProperty("FlickAccelerationThreshold");
            flickTimeThresholdProperty = serializedObject.FindProperty("FlickTimeThreshold");
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            carousel = target as Carousel;

            // 各種情報をキャッシュする
            CacheData();

            // CellLayoutGroup
            ValidateCellLayoutGroupByAxis();

            // CellLayoutGroupとCellLayoutGroupのparentのPivot
            ValidatePivot();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            compositedGUIChanged = false;
            
            // CellLayoutGroup
            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;
            serializedObject.Update();
            EditorGUILayout.PropertyField(layoutProperty);
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                // layoutPropertyに変更がかかった場合はキャッシュし直す
                CacheCellLayoutGroupDatas();
                ValidateCellLayoutGroupByAxis();
                ValidatePivot();
            }

            // Axis
            if (layoutProperty.objectReferenceValue != null)
            {
                DrawAxis();
            }

            serializedObject.Update();

            // LoopType
            EditorGUILayout.PropertyField(loopTypeProperty);

            // AutoScrollInterval
            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;
            EditorGUILayout.PropertyField(autoScrollIntervalProperty);
            if (GUI.changed)
            {
                ValidateAutoScrollInterval();
            }

            // BulletController
            EditorGUILayout.PropertyField(bulletControllerProperty);

            // フリック関連プロパティ
            if (EditorUtilsTools.DrawGroupHeader("Flick Property", "Carousel Flick Edit"))
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.PropertyField(isControllableFlickProperty);
                    if (isControllableFlickProperty.boolValue)
                    {
                        EditorGUILayout.PropertyField(flickMovementThresholdProperty);
                        EditorGUILayout.PropertyField(flickAccelerationThresholdProperty);
                        EditorGUILayout.PropertyField(flickTimeThresholdProperty);
                    }
                }
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();

            // OnCenterのdelegate
            DelegatableObjectEditor.DrawEditFields(
                "On Complete Transition",
                target,
                carousel.OnCompleteTransitionDelegates,
                typeof(AuthorizedAccessAttribute),
                KeyHeader + "_OnCompleteTransition");

            // OnChangeIndexのdelegate
            DelegatableObjectEditor.DrawEditFields(
                "On Change Index",
                target,
                carousel.OnChangeIndexDelegates,
                typeof(AuthorizedAccessAttribute),
                KeyHeader + "_OnChangeIndex");

            // Undo情報の登録
            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo("Edit Carousel", compositedGUIChanged, target);

            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
