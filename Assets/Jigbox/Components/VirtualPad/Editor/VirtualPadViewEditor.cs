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
    [CustomEditor(typeof(VirtualPadView), true)]
    public class VirtualPadViewEditor : Editor
    {
#region properties

        /// <summary>バーチャルパッドの見た目を構成するクラス</summary>
        protected VirtualPadView virtualPadView;

        /// <summary>操作用のハンドルオブジェクトのRectTransformのプロパティ</summary>
        protected SerializedProperty handleProperty;

#endregion

#region protected methods

        /// <summary>
        /// SerializedPropertyを表示します。
        /// </summary>
        protected virtual void DrawSerializedProperties()
        {
            EditorGUILayout.PropertyField(handleProperty);
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            virtualPadView = target as VirtualPadView;
            handleProperty = serializedObject.FindProperty("handle");
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            serializedObject.Update();
            DrawSerializedProperties();
            serializedObject.ApplyModifiedProperties();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.FloatField("Range of Motion", virtualPadView.RangeOfMotion);
            EditorGUI.EndDisabledGroup();

            EditorUtilsTools.RegisterUndo("Edit VirtualPadView", GUI.changed, target);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
