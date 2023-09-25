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
    [CustomEditor(typeof(VariableListBase), true)]
    public abstract class VariableListEditor : Editor
    {
#region properties

        /// <summary>ListView</summary>
        protected VariableListBase variableList;

        /// <summary>タイル外周の余白のプロパティ</summary>
        protected SerializedPadding paddingProperty;
        
        /// <summary>合成したGUI変更フラグ。</summary>
        protected bool compositedGUIChanged;

#endregion

#region protected methods

        /// <summary>
        /// Viewの情報を設定します
        /// </summary>
        protected virtual void SetViewSettings()
        {
            ScrollRect scrollRect = variableList.GetComponent<ScrollRect>();

            RectTransform viewport = scrollRect.viewport;
            if (viewport != null)
            {
                RectTransformUtils.SetAnchor(viewport, RectTransformUtils.AnchorPoint.StretchFull);
                RectTransformUtils.SetPivot(viewport, ViewPivot);
            }

            RectTransform content = scrollRect.content;
            if (content != null)
            {
                RectTransformUtils.SetAnchor(content, RectTransformUtils.AnchorPoint.StretchFull);
                RectTransformUtils.SetPivot(content, ViewPivot);
            }
        }

#endregion

#region abstract

        protected abstract Vector2 ViewPivot { get; }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            variableList = target as VariableListBase;

            paddingProperty = new SerializedPadding(serializedObject.FindProperty("padding"));

            SetViewSettings();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            compositedGUIChanged = false;
            
            serializedObject.Update();

            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;
            paddingProperty.EditProperty();
            if (GUI.changed)
            {
                variableList.RefreshCells();
            }

            serializedObject.ApplyModifiedProperties();

            DelegatableObjectEditor.DrawEditFields(
                "On Update Cell",
                variableList,
                variableList.OnUpdateCellDelegates,
                typeof(AuthorizedAccessAttribute),
                "VariableList.OnUpdateCell");

            DelegatableObjectEditor.DrawEditFields(
                "On Update Cell Size",
                variableList,
                variableList.OnUpdateCellSizeDelegates,
                typeof(AuthorizedAccessAttribute),
                "VariableList.OnUpdateCellSize");

            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo("Edit VariableList", compositedGUIChanged, targets);
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
