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

namespace Jigbox.UIControl
{
    public static class SubmitColleagueEditor
    {
#region public methods

        /// <summary>
        /// SubmitColleagueの編集用のエディタ表示を行います。
        /// </summary>
        /// <param name="colleague"></param>
        public static void DrawEdit(SerializedProperty colleague)
        {
            SerializedProperty group = colleague.FindPropertyRelative("group");
            SerializedProperty coolTime = colleague.FindPropertyRelative("coolTime");

            float tempWidth = EditorGUIUtility.labelWidth;

            EditorUtilsTools.SetLabelWidth(80.0f);
            EditorGUILayout.PropertyField(group, new GUIContent("Input Group"));

            EditorUtilsTools.SetLabelWidth(160.0f);
            float time = EditorGUILayout.FloatField("Reusable time after unlock", coolTime.floatValue);
            time = Mathf.Clamp(time, 0.0f, 10.0f);
            if (time != coolTime.floatValue)
            {
                coolTime.floatValue = time;
            }

            EditorUtilsTools.SetLabelWidth(tempWidth);
        }

#endregion
    }
}
