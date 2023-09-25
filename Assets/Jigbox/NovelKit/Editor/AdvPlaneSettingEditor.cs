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

namespace Jigbox.NovelKit
{
    [CustomEditor(typeof(AdvPlaneSetting))]
    public sealed class AdvPlaneSettingEditor : Editor
    {
#region properties

        /// <summary>キャラクター</summary>
        SerializedProperty character;
        
        /// <summary>キャラクター以外の画像</summary>
        SerializedProperty sprite;
        
        /// <summary>背景</summary>
        SerializedProperty bg;
        
        /// <summary>CG(一枚絵)</summary>
        SerializedProperty cg;
        
        /// <summary>感情表現系エモーション</summary>
        SerializedProperty emotional;

        /// <summary>演出</summary>
        SerializedProperty effect;

        /// <summary>その他</summary>
        SerializedProperty other;

#endregion

#region private methods

        /// <summary>
        /// プロパティの編集用表示を行います。
        /// </summary>
        /// <param name="property">編集するプロパティ</param>
        /// <param name="label">ラベル</param>
        void DrawProperty(SerializedProperty property, string label)
        {
            EditorGUILayout.PropertyField(property, new GUIContent(label));
            if (property.intValue < 1)
            {
                property.intValue = 1;
            }
            else if (property.intValue > 999)
            {
                property.intValue = 999;
            }
        }

#endregion

#region override unity methods

        void OnEnable()
        {
            character = serializedObject.FindProperty("character");
            sprite = serializedObject.FindProperty("sprite");
            bg = serializedObject.FindProperty("bg");
            cg = serializedObject.FindProperty("cg");
            emotional = serializedObject.FindProperty("emotional");
            effect = serializedObject.FindProperty("effect");
            other = serializedObject.FindProperty("other");
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            DrawProperty(character, "ch「キャラクター」");
            DrawProperty(sprite, "sp「キャラクター以外の画像」");
            DrawProperty(bg, "bg「背景」");
            DrawProperty(cg, "cg「一枚絵、CG」");
            DrawProperty(emotional, "em「感情エモーション」");
            DrawProperty(effect, "ef「演出、エフェクト」");
            DrawProperty(other, "ot「その他」");

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Plane Setting", GUI.changed, target);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
