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
using System.Collections;
using Jigbox.Delegatable;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Droppable), true)]
    public class DroppableEditor : Editor
    {
#region constants

        /// <summary>開閉状態保存用キーの先頭語</summary>
        protected static readonly string KeyHeader = typeof(Droppable).ToString();

#endregion

#region properties

        /// <summary>ドロップ用コンポーネントの参照</summary>
        protected Droppable droppable;

        /// <summary>有効かどうか</summary>
        protected SerializedProperty isEnablePropertiy;

#endregion

#region protected methods

        /// <summary>
        /// プロパティの編集用の表示を行います。
        /// </summary>
        protected virtual void DrawProperties()
        {
            EditorGUILayout.PropertyField(isEnablePropertiy);
        }

#endregion

#region override unitye methods

        protected virtual void OnEnable()
        {
            droppable = target as Droppable;

            isEnablePropertiy = serializedObject.FindProperty("isEnable");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            DrawProperties();

            serializedObject.ApplyModifiedProperties();

            if (targets.Length == 1)
            {
                DelegatableObjectEditor.DrawEditFields(
                    "On Enter",
                    droppable,
                    droppable.OnEnterDelegates,
                    typeof(AuthorizedAccessAttribute),
                    KeyHeader + "_OnEnter");

                DelegatableObjectEditor.DrawEditFields(
                    "On Exit",
                    droppable,
                    droppable.OnExitDelegates,
                    typeof(AuthorizedAccessAttribute),
                    KeyHeader + "_OnExit");

                DelegatableObjectEditor.DrawEditFields(
                    "On Drop",
                    droppable,
                    droppable.OnDropDelegates,
                    typeof(AuthorizedAccessAttribute),
                    KeyHeader + "_OnDrop");
            }
            else
            {
                EditorGUILayout.HelpBox("複数同時選択時は、イベントの編集はできません。", MessageType.Warning);
            }

            EditorUtilsTools.RegisterUndo("Edit Droppable", GUI.changed, targets);
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
