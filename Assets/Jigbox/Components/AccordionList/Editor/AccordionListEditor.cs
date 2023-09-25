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
    [CustomEditor(typeof(AccordionListEditor), true)]
    public abstract class AccordionListEditor : Editor
    {
#region properties

        /// <summary>AccordionList</summary>
        protected AccordionListBase accordionList;

        /// <summary>Content内側の余白のプロパティ</summary>
        protected SerializedPadding paddingProperty;

        /// <summary>シングルモード</summary>
        protected SerializedProperty isSingleModeProperty;

        /// <summary>子ノード領域セル</summary>
        protected SerializedProperty childAreaContentProperty;

        /// <summary>クリッピング領域</summary>
        protected SerializedProperty clippingAreaProperty;

        /// <summary>クリッピングなし領域</summary>
        protected SerializedProperty notClippingAreaProperty;
        
        /// <summary>合成したGUI変更フラグ。</summary>
        protected bool compositedGUIChanged;

#endregion

#region abstract

        protected abstract Vector2 ViewPivot { get; }

        protected abstract Vector2 ContentAnchorMin { get; }

        protected abstract Vector2 ContentAnchorMax { get; }

#endregion

#region protected methods

        /// <summary>
        /// Viewの情報を設定します
        /// </summary>
        protected virtual void SetViewSettings()
        {
            ScrollRect scrollRect = accordionList.GetComponent<ScrollRect>();

            RectTransform viewport = scrollRect.viewport;
            if (viewport != null)
            {
                RectTransformUtils.SetAnchor(viewport, RectTransformUtils.AnchorPoint.StretchFull);
                RectTransformUtils.SetPivot(viewport, ViewPivot);
            }

            RectTransform content = scrollRect.content;
            if (content != null)
            {
                RectTransformUtils.SetAnchor(content, ContentAnchorMin, ContentAnchorMax);
                RectTransformUtils.SetPivot(content, ViewPivot);
            }

            SearchRaycastValidator();
        }

        /// <summary>
        /// RaycastValidatorコンポーネントの参照をつける
        /// RaycastValidatorはUIControl以下のコンポーネントのため、AccordionListからつける必要がある
        /// </summary>
        protected virtual void SearchRaycastValidator()
        {
            var raycastValidator = accordionList.GetComponent<RaycastValidator>();

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

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            accordionList = target as AccordionListBase;

            isSingleModeProperty = serializedObject.FindProperty("isSingleMode");

            paddingProperty = new SerializedPadding(serializedObject.FindProperty("padding"));

            childAreaContentProperty = serializedObject.FindProperty("childAreaContent");

            clippingAreaProperty = serializedObject.FindProperty("clippingArea");

            notClippingAreaProperty = serializedObject.FindProperty("notClippingArea");

            SetViewSettings();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            compositedGUIChanged = false;
            
            serializedObject.Update();

            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;

            EditorGUILayout.PropertyField(isSingleModeProperty);

            paddingProperty.EditProperty();

            if (GUI.changed && EditorApplication.isPlaying)
            {
                accordionList.IsSingleMode = isSingleModeProperty.boolValue;
                accordionList.RefreshCells();
            }

            EditorGUILayout.PropertyField(childAreaContentProperty);
            EditorGUILayout.PropertyField(clippingAreaProperty);
            EditorGUILayout.PropertyField(notClippingAreaProperty);

            serializedObject.ApplyModifiedProperties();

            DelegatableObjectEditor.DrawEditFields(
                "On Update Cell",
                accordionList,
                accordionList.OnUpdateCellDelegates,
                typeof(AuthorizedAccessAttribute),
                "AccordionList.OnUpdateCell");

            DelegatableObjectEditor.DrawEditFields(
                "On Update Cell Size",
                accordionList,
                accordionList.OnUpdateCellSizeDelegates,
                typeof(AuthorizedAccessAttribute),
                "AccordionList.OnUpdateCellSize");

            DelegatableObjectEditor.DrawEditFields(
                "On Start Expand",
                accordionList,
                accordionList.OnStartExpandDelegates,
                typeof(AuthorizedAccessAttribute),
                "AccordionList.OnStartExpand");

            DelegatableObjectEditor.DrawEditFields(
                "On Complete Expand",
                accordionList,
                accordionList.OnCompleteExpandDelegates,
                typeof(AuthorizedAccessAttribute),
                "AccordionList.OnCompleteExpand");

            DelegatableObjectEditor.DrawEditFields(
                "On Start Collapse",
                accordionList,
                accordionList.OnStartCollapseDelegates,
                typeof(AuthorizedAccessAttribute),
                "AccordionList.OnStartCollapse");

            DelegatableObjectEditor.DrawEditFields(
                "On Complete Collapse",
                accordionList,
                accordionList.OnCompleteCollapseDelegates,
                typeof(AuthorizedAccessAttribute),
                "AccordionList.OnCompleteCollapse");
            
            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo("Edit AccordionList", compositedGUIChanged, targets);

            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
