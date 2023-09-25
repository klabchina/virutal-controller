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
    [CustomEditor(typeof(Draggable), true)]
    public class DraggableEditor : Editor
    {
#region constants

        /// <summary>開閉状態保存用キーの先頭語</summary>
        protected static readonly string KeyHeader = typeof(Draggable).ToString();

#endregion

#region properties

        /// <summary>ドラッグ用コンポーネントの参照</summary>
        protected Draggable draggable;

        /// <summary>有効かどうか</summary>
        protected SerializedProperty isEnablePropertiy;

        /// <summary>ドラッグ中にオブジェクトを移動させるためのTransform空間</summary>
        protected SerializedProperty dragAreaProperty;

        /// <summary>ドラッグ終了時の位置の戻し方</summary>
        protected SerializedProperty restoreTypeDropProperty;

        /// <summary>ドラッグが開始されるまでに誤差として扱われる移動量</summary>
        protected SerializedProperty thresholdProperty;

        /// <summary>ドラッグ開始時にドラッグ用コンポーネントのドラッグとして許容されるドラッグの方向</summary>
        protected SerializedProperty permissiveDirectionProperty;

        /// <summary>ドラッグ開始時のコールバック</summary>
        protected SerializedProperty onBeginDragProperty;

        /// <summary>ドラッグ中にポインタが移動した際のコールバック</summary>
        protected SerializedProperty onDragProperty;

        /// <summary>ドラッグ終了時のコールバック</summary>
        protected SerializedProperty onEndDragProperty;

#endregion

#region protected methods

        /// <summary>
        /// プロパティの編集用の表示を行います。
        /// </summary>
        protected virtual void DrawProperties()
        {
            EditorGUILayout.PropertyField(isEnablePropertiy);
            EditorGUILayout.PropertyField(dragAreaProperty);
            EditorGUILayout.PropertyField(restoreTypeDropProperty);
            EditorGUILayout.PropertyField(thresholdProperty);
            EditorGUILayout.PropertyField(permissiveDirectionProperty);
        }

        /// <summary>
        /// 複数選択時にDelegatableの内容をコピーする機能の表示を行います
        /// </summary>
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
                    DelegatableObjectEditor.CopyToDelegatableListProperty(draggable.OnBeginDragDelegates, onBeginDragProperty);
                    DelegatableObjectEditor.CopyToDelegatableListProperty(draggable.OnDragDelegates, onDragProperty);
                    DelegatableObjectEditor.CopyToDelegatableListProperty(draggable.OnEndDragDelegates, onEndDragProperty);
                }
            }
        }

#endregion

#region override unitye methods

        protected virtual void OnEnable()
        {
            if (serializedObject.targetObjects.Length == 1)
            {
                draggable = target as Draggable;
            }
            else
            {
                if (Selection.activeGameObject != null)
                {
                    draggable = Selection.activeGameObject.GetComponent<Draggable>();
                }
            }

            isEnablePropertiy = serializedObject.FindProperty("isEnable");
            dragAreaProperty = serializedObject.FindProperty("dragArea");
            restoreTypeDropProperty = serializedObject.FindProperty("restoreType");
            thresholdProperty = serializedObject.FindProperty("threshold");
            permissiveDirectionProperty = serializedObject.FindProperty("permissiveDirection");
            onBeginDragProperty = serializedObject.FindProperty("onBeginDrag");
            onDragProperty = serializedObject.FindProperty("onDrag");
            onEndDragProperty = serializedObject.FindProperty("onEndDrag");

            foreach (Object obj in targets)
            {
                SerializedObject serialized = new SerializedObject(obj);
                SerializedProperty buttonProperty = serialized.FindProperty("button");
                serialized.Update();
                buttonProperty.objectReferenceValue = (obj as Component).GetComponent<ButtonBase>();
                serialized.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            DrawProperties();

            DrawCopyDelegateButton();

            serializedObject.ApplyModifiedProperties();

            DelegatableObjectEditor.DrawEditFields(
                "On Begin Drag",
                draggable,
                draggable.OnBeginDragDelegates,
                typeof(AuthorizedAccessAttribute),
                KeyHeader + "_OnBeginDrag");

            DelegatableObjectEditor.DrawEditFields(
                "On Drag",
                draggable,
                draggable.OnDragDelegates,
                typeof(AuthorizedAccessAttribute),
                KeyHeader + "_OnDrag");

            DelegatableObjectEditor.DrawEditFields(
                "On End Drag",
                draggable,
                draggable.OnEndDragDelegates,
                typeof(AuthorizedAccessAttribute),
                KeyHeader + "_OnEndDrag");
            EditorUtilsTools.RegisterUndo("Edit Draggable", GUI.changed, targets);
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
