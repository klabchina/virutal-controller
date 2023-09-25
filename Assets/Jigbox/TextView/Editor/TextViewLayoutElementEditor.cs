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
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextViewLayoutElement), true)]
    public class TextViewLayoutElementEditor : Editor
    {
#region properties

        /// <summary>レイアウトを無効化するかどうか</summary>
        protected SerializedProperty isIgnoreProperty;

        /// <summary>TextView</summary>
        protected SerializedProperty textViewProperty;

        /// <summary>必要な幅を求める方法</summary>
        protected SerializedProperty preferredWidthTypeProperty;

        /// <summary>必要な高さを求める方法</summary>
        protected SerializedProperty preferredHeightTypeProperty;
        
        /// <summary>合成したGUI変更フラグ。</summary>
        protected bool compositedGUIChanged;

#endregion

#region protected methods

        /// <summary>
        /// プロパティの編集用フィールドを表示します。
        /// </summary>
        protected virtual void DrawProperties()
        {
            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;

            EditorGUILayout.PropertyField(isIgnoreProperty);
            EditorGUILayout.PropertyField(preferredWidthTypeProperty);
            EditorGUILayout.PropertyField(preferredHeightTypeProperty);

            // プロパティが変更された場合、レイアウトの方法が変更になるので、
            // TextView自体に変更がなくてもSetLayoutDirtyを行う
            if (GUI.changed && textViewProperty.objectReferenceValue != null)
            {
                TextView textView = textViewProperty.objectReferenceValue as TextView;
                textView.SetLayoutDirty();
            }
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            isIgnoreProperty = serializedObject.FindProperty("isIgnore");
            this.textViewProperty = serializedObject.FindProperty("textView");
            preferredWidthTypeProperty = serializedObject.FindProperty("preferredWidthType");
            preferredHeightTypeProperty = serializedObject.FindProperty("preferredHeightType");

            // 元のserializeObjectは、targets全てを参照するものなので、
            // 個別に参照関係を設定しなければいけないパラメータは一つずつ設定する
            foreach (Object obj in targets)
            {
                TextView textView = (obj as Component).GetComponent<TextView>();

                // 自身にTextViewの参照を設定
                {
                    SerializedObject serialized = new SerializedObject(obj);
                    SerializedProperty textViewProperty = serialized.FindProperty("textView");
                    serialized.Update();
                    textViewProperty.objectReferenceValue = textView;
                    serialized.ApplyModifiedProperties();
                }

                // TextViewに自身の参照を設定
                if (textView != null)
                {
                    SerializedObject serialized = new SerializedObject(textView);
                    SerializedProperty layoutElementProperty = serialized.FindProperty("layoutElement");
                    serialized.Update();
                    layoutElementProperty.objectReferenceValue = obj;
                    serialized.ApplyModifiedProperties();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            compositedGUIChanged = false;
            
            DrawDefaultInspector();

            serializedObject.Update();

            DrawProperties();

            serializedObject.ApplyModifiedProperties();

            if (targets.Length == 1 && textViewProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("TextViewと同じGameObjectにアタッチして下さい。", MessageType.Warning);
            }

            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo("Edit TextView LayoutElement", compositedGUIChanged, targets);

            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
