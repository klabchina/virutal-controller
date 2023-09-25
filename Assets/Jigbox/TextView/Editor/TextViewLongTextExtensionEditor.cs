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
    [CustomEditor(typeof(TextViewLongTextExtension))]
    public class TextViewLongTextExtensionEditor : Editor
    {
#region properties

        /// <summary>分割を行う文字数</summary>
        protected SerializedProperty divideLengthProperty;

        /// <summary>Outlineなど頂点を増やすModifierが一緒にアタッチされているかどうか</summary>
        protected bool withIncreaseVerticesModifier = false;

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            divideLengthProperty = serializedObject.FindProperty("divideLength");
            TextViewLongTextExtension longTextExtension = target as TextViewLongTextExtension;
            withIncreaseVerticesModifier = longTextExtension.GetComponent<UnityEngine.UI.Shadow>() != null
                || longTextExtension.GetComponent<Outline>() != null;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            int length = EditorGUILayout.IntSlider("Divide Length", divideLengthProperty.intValue, 1, TextViewLongTextExtension.DivideLengthLimit);
            if (length != divideLengthProperty.intValue)
            {
                divideLengthProperty.intValue = length;
            }

            serializedObject.ApplyModifiedProperties();

            if (withIncreaseVerticesModifier)
            {
                EditorGUILayout.HelpBox("ShadowやOutlineを利用している場合、1文字のレンダリングに必要な頂点数が増えるため、Divide Lengthの設定に注意して下さい。", MessageType.Info);
            }

            EditorUtilsTools.RegisterUndo("Edit TextViewLongTextExtension", GUI.changed, targets);
            
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
