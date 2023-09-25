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

using UnityEditor;
using UnityEditorInternal;
using Jigbox.VirtualPad;

namespace Jigbox.Components
{
    [CustomEditor(typeof(VirtualPadController), true)]
    public class VirtualPadControllerEditor : GestureUmpireEditor<VirtualPadEventType, VirtualPadData, VirtualPadEventHandler>
    {
#region properties

        /// <summary>バーチャルパッドの表示を構成するクラスのプロパティ</summary>
        protected SerializedProperty viewProperty;

        /// <summary>バーチャルパッドの中心からの移動率が0扱いになる範囲のプロパティ</summary>
        protected SerializedProperty deadZoneProperty;

        /// <summary>横方向に動かないようにするかどうかのプロパティ</summary>
        protected SerializedProperty freezeHorizontalProperty;

        /// <summary>縦方向に動かないようにするかどうかのプロパティ</summary>
        protected SerializedProperty freezeVerticalProperty;

        /// <summary>バーチャルパッドの値の変更を受け取るコンポーネントのプロパティ</summary>
        protected SerializedProperty receiversProperty;

        /// <summary>バーチャルパッドの値の変更を受け取るコンポーネントのプロパティのエディタ表示用リスト</summary>
        protected ReorderableList receiverList;

        /// <summary>イベントハンドラ全体のグループヘッダーに表示するラベル名</summary>
        protected override string HandlerGroupHeaderLabel { get { return "Events"; } }

        /// <summary>イベントハンドラの追加用文言</summary>
        protected override string AddHandlerWord { get { return "Add Event"; } }

#endregion

#region protected methods

        /// <summary>
        /// プロパティの編集用の表示を行います。
        /// </summary>
        protected override void DrawProperties()
        {
            base.DrawProperties();

            serializedObject.Update();

            EditorGUILayout.PropertyField(viewProperty);
            FloatField("Dead Zone", deadZoneProperty, 0.0f, 1.0f);
            EditorGUILayout.PropertyField(freezeHorizontalProperty);
            EditorGUILayout.PropertyField(freezeVerticalProperty);

            receiverList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

#endregion

#region override unity methods

        protected override void OnEnable()
        {
            base.OnEnable();

            viewProperty = serializedObject.FindProperty("view");
            deadZoneProperty = serializedObject.FindProperty("deadZone");
            freezeHorizontalProperty = serializedObject.FindProperty("freezeHorizontal");
            freezeVerticalProperty = serializedObject.FindProperty("freezeVertical");
            receiversProperty = serializedObject.FindProperty("receivers");

            receiverList = new ReorderableList(serializedObject, receiversProperty);
            receiverList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Receiver Components");
            receiverList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty material = receiversProperty.GetArrayElementAtIndex(index);
                UnityEngine.Object edited = EditorGUI.ObjectField(rect, "Receiver " + (index + 1), material.objectReferenceValue, typeof(VirtualPadUpdateReceiver), true);
                if (material.objectReferenceValue != edited)
                {
                    material.objectReferenceValue = edited;
                }
            };
        }

#endregion
    }
}
