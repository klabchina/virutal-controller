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
    public class SerializedVector2
    {
#region constants

        /// <summary>ラベルの幅</summary>
        protected static float LabelWidth = 120.0f;

        /// <summary>プロパティのラベルの幅</summary>
        protected static float PropertyLabelWidth = 15.0f;

        /// <summary>リセットボタン幅</summary>
        protected static float ButtonWidth = 25.0f;

#endregion

#region properties

        /// <summary>xプロパティ</summary>
        protected SerializedProperty xProperty;

        /// <summary>xプロパティ</summary>
        public SerializedProperty X { get { return xProperty; } }

        /// <summary>yプロパティ</summary>
        protected SerializedProperty yProperty;

        /// <summary>yプロパティ</summary>
        public SerializedProperty Y { get { return yProperty; } }

        /// <summary>リセット時の値</summary>
        public Vector2 ResetValue { get; set; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property">SerializedProperty</param>
        public SerializedVector2(SerializedProperty property)
        {
            xProperty = property.FindPropertyRelative("x");
            yProperty = property.FindPropertyRelative("y");
        }

        /// <summary>
        /// 値を編集します。
        /// </summary>
        /// <param name="label">ラベル</param>
        /// <param name="resetLabel">リセットボタンのラベル</param>
        public virtual void EditProperty(string label, string resetLabel = "")
        {
            float labelWidth = EditorGUIUtility.labelWidth;

            EditorGUILayout.BeginHorizontal();
            {
                if (!string.IsNullOrEmpty(label))
                {
                    EditorGUILayout.LabelField(label, GUILayout.Width(LabelWidth));
                }

                EditorUtilsTools.SetLabelWidth(PropertyLabelWidth);

                bool isReset = false;
                if (!string.IsNullOrEmpty(resetLabel))
                {
                    isReset = GUILayout.Button(resetLabel, GUILayout.Width(ButtonWidth));
                }

                EditorGUILayout.PropertyField(xProperty);
                EditorGUILayout.PropertyField(yProperty);

                if (isReset)
                {
                    xProperty.floatValue = ResetValue.x;
                    yProperty.floatValue = ResetValue.y;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorUtilsTools.SetLabelWidth(labelWidth);
        }
        
        /// <summary>
        /// 値を編集します。ラベルの横幅を上書きし、Inspectorでの見え方を調整する際に使用します。
        /// SerializedVector3の継承メソッドに影響が出ないよう、新規にメソッドを作成しています。
        /// </summary>
        /// <param name="label">ラベル</param>
        /// <param name="resetLabel">リセットボタンのラベル</param>
        /// <param name="overrideLabelWidth">ラベルのサイズの上書き値</param>>
        public virtual void EditProperty(string label, string resetLabel = "", float overrideLabelWidth = 120.0f)
        {
            float labelWidth = EditorGUIUtility.labelWidth;

            EditorGUILayout.BeginHorizontal();
            {
                if (!string.IsNullOrEmpty(label))
                {
                    EditorGUILayout.LabelField(label, GUILayout.Width(overrideLabelWidth));
                }

                EditorUtilsTools.SetLabelWidth(PropertyLabelWidth);

                bool isReset = false;
                if (!string.IsNullOrEmpty(resetLabel))
                {
                    isReset = GUILayout.Button(resetLabel, GUILayout.Width(ButtonWidth));
                }

                EditorGUILayout.PropertyField(xProperty);
                EditorGUILayout.PropertyField(yProperty);

                if (isReset)
                {
                    xProperty.floatValue = ResetValue.x;
                    yProperty.floatValue = ResetValue.y;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorUtilsTools.SetLabelWidth(labelWidth);
        }

#endregion
    }
}
