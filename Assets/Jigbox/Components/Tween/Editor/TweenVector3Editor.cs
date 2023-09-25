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
    public class TweenVector3Editor : TweenBaseEditor
    {
#region constants

        /// <summary>開始値、終了値のラベル幅</summary>
        protected static readonly float LabelWidth = 50.0f;

        /// <summary>開始値、終了値の編集用フィールドのラベル幅</summary>
        protected static readonly float PropertyLabelWidth = 15.0f;

        /// <summary>リセットボタンの横幅</summary>
        protected static readonly float ButtonWidth = 25.0f;

#endregion

#region properties

        /// <summary>開始値のx値</summary>
        protected SerializedProperty beginX;

        /// <summary>開始値のy値</summary>
        protected SerializedProperty beginY;

        /// <summary>開始値のz値</summary>
        protected SerializedProperty beginZ;

        /// <summary>終了値のx値</summary>
        protected SerializedProperty finalX;

        /// <summary>終了値のy値</summary>
        protected SerializedProperty finalY;

        /// <summary>終了値のz値</summary>
        protected SerializedProperty finalZ;

        /// <summary>リセットボタンに表示する文言</summary>
        protected virtual string ResetButtonLabel { get { return string.Empty; } }

        /// <summary>リセット時の値</summary>
        protected virtual Vector3 ResetValue { get { return Vector3.zero; } }

#endregion

#region protected methods

        /// <summary>
        /// Tweenの開始、終了値の編集用フィールドを表示します。
        /// </summary>
        protected override void EditTweenValueField()
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorUtilsTools.SetLabelWidth(LabelWidth);
                EditorGUILayout.LabelField("Begin");
                
                bool isReset = GUILayout.Button(ResetButtonLabel, GUILayout.Width(25));

                EditorUtilsTools.SetLabelWidth(PropertyLabelWidth);
                EditorGUILayout.PropertyField(beginX);
                EditorGUILayout.PropertyField(beginY);
                EditorGUILayout.PropertyField(beginZ);

                if (isReset)
                {
                    begin.vector3Value = ResetValue;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorUtilsTools.SetLabelWidth(LabelWidth);
                EditorGUILayout.LabelField("Final");

                bool isReset = GUILayout.Button(ResetButtonLabel, GUILayout.Width(25));

                EditorUtilsTools.SetLabelWidth(PropertyLabelWidth);
                EditorGUILayout.PropertyField(finalX);
                EditorGUILayout.PropertyField(finalY);
                EditorGUILayout.PropertyField(finalZ);

                if (isReset)
                {
                    final.vector3Value = ResetValue;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorUtilsTools.SetLabelWidth(labelWidth);
        }

#endregion

#region override unity methods

        protected override void OnEnable()
        {
            base.OnEnable();

            beginX = begin.FindPropertyRelative("x");
            beginY = begin.FindPropertyRelative("y");
            beginZ = begin.FindPropertyRelative("z");

            finalX = final.FindPropertyRelative("x");
            finalY = final.FindPropertyRelative("y");
            finalZ = final.FindPropertyRelative("z");
        }

#endregion
    }
}
