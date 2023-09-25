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
using UnityEngine;

namespace Jigbox
{
    [CustomPropertyDrawer(typeof(PaddingCustomAttribute))]
    public class PaddingPropertyDrawer : PropertyDrawer
    {
        /// <summary>ラベルの幅</summary>
        protected static float LabelWidth = 120.0f;

        /// <summary>プロパティのラベルの幅</summary>
        protected static float PropertyLabelWidth = 45.0f;

        /// <summary>リセットボタン幅</summary>
        protected static float ButtonWidth = 25.0f;

        /// <summary>プロパティの高さ</summary>
        protected static float Height = 16.0f;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.labelWidth = PropertyLabelWidth;

            var paddingAttribute = attribute as PaddingCustomAttribute;
            if (paddingAttribute.Orientation == PaddingCustomAttribute.ScrollOrientation.Vertical)
            {
                var left = property.FindPropertyRelative("left");
                var right = property.FindPropertyRelative("right");
                DrawOrientationPadding(position, "Margin", "H", left, right);
            }
            else if (paddingAttribute.Orientation == PaddingCustomAttribute.ScrollOrientation.Horizontal)
            {
                var top = property.FindPropertyRelative("top");
                var bottom = property.FindPropertyRelative("bottom");
                DrawOrientationPadding(position, "Margin", "V", top, bottom);
            }
        }

        protected virtual void DrawOrientationPadding(Rect position, string label, string buttonText, SerializedProperty property1, SerializedProperty property2)
        {
            EditorGUI.LabelField(new Rect(position.x, position.y, LabelWidth, Height), new GUIContent(label));
            if (GUI.Button(new Rect(position.x + LabelWidth, position.y, ButtonWidth, Height), buttonText))
            {
                property1.intValue = 0;
                property2.intValue = 0;
            }

            var x = position.x + LabelWidth + ButtonWidth;
            var width = (position.x + position.width - x) / 2;
            EditorGUI.PropertyField(new Rect(x, position.y, width, Height), property1);
            EditorGUI.PropertyField(new Rect(x + width, position.y, width, Height), property2);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Height;
        }
    }
}
