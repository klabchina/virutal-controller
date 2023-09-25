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

using Jigbox.EditorUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GradationAdvanced), true)]
    public class GradationAdvancedEditor : Editor
    {
#region properties

        protected GradationAdvanced gradationAdvanced;
        
        protected SerializedProperty directionProperty;

        protected SerializedProperty targetTypeProperty;

        protected SerializedProperty gradientProperty;

        protected SerializedProperty rangeByVerticesProperty;

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            gradationAdvanced = target as GradationAdvanced;
            directionProperty = serializedObject.FindProperty("direction");
            gradientProperty = serializedObject.FindProperty("gradient");
            targetTypeProperty = serializedObject.FindProperty("targetType");
            rangeByVerticesProperty = serializedObject.FindProperty("rangeByVertices");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            EditorGUILayout.PropertyField(targetTypeProperty);
            
            if (gradationAdvanced.TargetType == GradationTargetType.UGUI)
            {
                var imageComponent = gradationAdvanced.GetComponent<Image>();
                if (imageComponent != null && imageComponent.type != Image.Type.Simple)
                {
                    EditorGUILayout.HelpBox("ImageTypeはSimpleにのみ対応しており、Sliced,Tiled,Filledでの動作は保証されていません。", MessageType.Warning);
                }
            }
            
            EditorGUILayout.PropertyField(directionProperty);
            EditorGUILayout.PropertyField(gradientProperty);

            if (gradationAdvanced.Gradient.mode == GradientMode.Fixed)
            {
                EditorGUILayout.HelpBox("GradientではBlendModeのみが利用可能です。", MessageType.Warning);
            }
            
            EditorGUILayout.HelpBox("Alphaを設定した場合、カラーが変更されますので設定しないようにしてください。", MessageType.Info);

            EditorGUILayout.PropertyField(rangeByVerticesProperty);

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Gradation Advanced", GUI.changed, targets);

            EditorGUI.EndChangeCheck();

            if (GUI.changed)
            {
                foreach (var t in targets)
                {
                    var gradation = t as GradationAdvanced;
                    gradation.RefreshQuadCreator();
                }
            }
        }

#endregion
    }
}
