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

namespace Jigbox.Components
{
    [CustomEditor(typeof(VirtualPadDynamicView), true)]
    public class VirtualPadDynamicViewEditor : VirtualPadViewEditor
    {
#region properties

        /// <summary>有効範囲を表すRectTransformのプロパティ</summary>
        protected SerializedProperty validRectProperty;

#endregion

#region protected methods

        /// <summary>
        /// SerializedPropertyを表示します。
        /// </summary>
        protected override void DrawSerializedProperties()
        {
            base.DrawSerializedProperties();
            EditorGUILayout.PropertyField(validRectProperty);
        }

#endregion

#region override unity methods

        protected override void OnEnable()
        {
            base.OnEnable();
            validRectProperty = serializedObject.FindProperty("validRect");
        }

#endregion
    }
}
