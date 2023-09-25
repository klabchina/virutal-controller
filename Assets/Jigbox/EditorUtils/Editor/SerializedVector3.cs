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

namespace Jigbox.EditorUtils
{
    public class SerializedVector3 : SerializedVector2
    {
#region properties

        /// <summary>zプロパティ</summary>
        protected SerializedProperty zProperty;

        /// <summary>zプロパティ</summary>
        public SerializedProperty Z { get { return zProperty; } }

        /// <summary>リセット時の値</summary>
        public new Vector3 ResetValue { get; set; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property">SerializedProperty</param>
        public SerializedVector3(SerializedProperty property) : base(property)
        {
            zProperty = property.FindPropertyRelative("z");
        }

        /// <summary>
        /// 値を編集します。
        /// </summary>
        /// <param name="label">ラベル</param>
        /// <param name="resetLabel">リセットボタンのラベル</param>
        public override void EditProperty(string label, string resetLabel = "")
        {
            float labelWidth = EditorGUIUtility.labelWidth;

            EditorGUILayout.BeginHorizontal();
            {
                if (!string.IsNullOrEmpty(label))
                {
                    EditorUtilsTools.SetLabelWidth(LabelWidth);
                    EditorGUILayout.LabelField(label);
                }

                EditorUtilsTools.SetLabelWidth(PropertyLabelWidth);

                bool isReset = false;
                if (!string.IsNullOrEmpty(resetLabel))
                {
                    isReset = GUILayout.Button(resetLabel, GUILayout.Width(ButtonWidth));
                }

                EditorGUILayout.PropertyField(xProperty);
                EditorGUILayout.PropertyField(yProperty);
                EditorGUILayout.PropertyField(zProperty);

                if (isReset)
                {
                    xProperty.floatValue = ResetValue.x;
                    yProperty.floatValue = ResetValue.y;
                    zProperty.floatValue = ResetValue.z;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorUtilsTools.SetLabelWidth(labelWidth);
        }

#endregion
    }
}
