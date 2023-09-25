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

namespace Jigbox.Components
{
    [CustomEditor(typeof(TextureMask), true)]
    public class TextureMaskEditor : BlendMaskEditor
    {
#region properties

        /// <summary>マスクとして使用するテクスチャ</summary>
        protected SerializedProperty textureProperty;

#endregion

#region protected methods

        /// <summary>
        /// 各プロパティを表示します。
        /// </summary>
        protected override void DrawSerializedProperties()
        {
            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;
            EditorGUILayout.PropertyField(textureProperty);
            needUpdateMaterial = Application.isPlaying && GUI.changed;

            base.DrawSerializedProperties();
        }

#endregion

#region override unity methods

        protected override void OnEnable()
        {
            base.OnEnable();

            textureProperty = serializedObject.FindProperty("texture");
        }

#endregion
    }
}
