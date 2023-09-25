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

namespace Jigbox
{
    public class SerializedPadding
    {
        /// <summary>ラベルの幅</summary>
        protected static float LabelWidth = 120.0f;
        
        /// <summary>プロパティのラベルの幅</summary>
        protected static float PropertyLabelWidth = 45.0f;

        /// <summary>リセットボタン幅</summary>
        protected static float ButtonWidth = 25.0f;

        /// <summary>leftのプロパティ</summary>
        protected SerializedProperty leftProperty;

        /// <summary>leftのプロパティ</summary>
        public SerializedProperty LeftProperty
        {
            get { return leftProperty; }
        }

        /// <summary>rightのプロパティ</summary>
        protected SerializedProperty rightProperty;

        /// <summary>rightのプロパティ</summary>
        public SerializedProperty RightProperty
        {
            get { return rightProperty; }
        }

        /// <summary>topのプロパティ</summary>
        protected SerializedProperty topProperty;

        /// <summary>topのプロパティ</summary>
        public SerializedProperty TopProperty
        {
            get { return topProperty; }
        }

        /// <summary>bottomのプロパティ</summary>
        protected SerializedProperty bottomProperty;

        /// <summary>bottomのプロパティ</summary>
        public SerializedProperty BottomProperty
        {
            get { return bottomProperty; }
        }

        /// <summary>横方向の値(Left,Right)を表示するか</summary>
        public bool ShowHorizontal { get; set; }

        /// <summary>縦方向の値(Top,Bottom)を表示するか</summary>
        public bool ShowVertical { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property">SerializedProperty</param>
        public SerializedPadding(SerializedProperty property)
        {
            topProperty = property.FindPropertyRelative("top");
            bottomProperty = property.FindPropertyRelative("bottom");
            leftProperty = property.FindPropertyRelative("left");
            rightProperty = property.FindPropertyRelative("right");
            ShowHorizontal = true;
            ShowVertical = true;
        }

        /// <summary>
        /// 値を編集します。
        /// </summary>
        public void EditProperty(string labelName = "Padding")
        {
            float labelWidth = EditorGUIUtility.labelWidth;

            EditorGUILayout.LabelField(labelName, GUILayout.Width(LabelWidth));

            EditorGUILayout.BeginVertical();
            {
                EditorUtilsTools.SetLabelWidth(PropertyLabelWidth);
                
                if (ShowHorizontal)
                {
                    // 横
                    EditorGUILayout.BeginHorizontal();
                    {
                        bool isReset = GUILayout.Button("H", GUILayout.Width(ButtonWidth));
                        
                        EditorGUILayout.PropertyField(leftProperty);
                        EditorGUILayout.PropertyField(rightProperty);

                        if (isReset)
                        {
                            leftProperty.intValue = 0;
                            rightProperty.intValue = 0;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (ShowVertical)
                {
                    // 縦
                    EditorGUILayout.BeginHorizontal();
                    {
                        bool isReset = GUILayout.Button("V", GUILayout.Width(ButtonWidth));

                        EditorGUILayout.PropertyField(topProperty);
                        EditorGUILayout.PropertyField(bottomProperty);

                        if (isReset)
                        {
                            topProperty.intValue = 0;
                            bottomProperty.intValue = 0;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            
            EditorUtilsTools.SetLabelWidth(labelWidth);
        }
    }
}
