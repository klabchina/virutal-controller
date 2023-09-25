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
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Gradation), true)]
    public class GradationEditor : Editor
    {
#region properties

        /// <summary>グラデーションコンポーネント</summary>
        protected Gradation gradation;

        /// <summary>グラデーションさせる方向</summary>
        protected SerializedProperty directionProperty;

        /// <summary>グラデーションさせる際の色の合成方法</summary>
        protected SerializedProperty typeProperty;

        /// <summary>グラデーションの色の割合を求めるための範囲を頂点から求めるかどうか</summary>
        protected SerializedProperty rangeByVerticesProperty;

        /// <summary>グラデーションの開始点の色</summary>
        protected SerializedProperty startColorProperty;

        /// <summary>グラデーションの終了点の色</summary>
        protected SerializedProperty endColorProperty;

        /// <summary>グラデーションの方向が同じかどうか</summary>
        protected bool IsSameDirection
        {
            get
            {
                // 複数選択されていない場合は常にtrue
                if (targets.Length <= 1)
                {
                    return true;
                }

                GradationEffectDirection direction = this.gradation.Direction;

                foreach (Object obj in targets)
                {
                    Gradation gradation = obj as Gradation;
                    if (gradation.Direction != direction)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>グラデーションの色が同じかどうか</summary>
        protected bool IsSameColor
        {
            get
            {
                // 複数選択されていない場合は常にtrue
                if (targets.Length <= 1)
                {
                    return true;
                }

                Color startColor = this.gradation.StartColor;
                Color endColor = this.gradation.EndColor;

                foreach (Object obj in targets)
                {
                    Gradation gradation = obj as Gradation;
                    if (gradation.StartColor != startColor
                        || gradation.EndColor != endColor)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// グラデーションの開始点の色のラベルに指定する文字列を取得します。
        /// </summary>
        /// <returns></returns>
        protected GUIContent GetStartColorLabel()
        {
            string label;

            switch (gradation.Direction)
            {
                case GradationEffectDirection.Up:
                    label = "Start Color (Bottom)";
                    break;
                case GradationEffectDirection.Down:
                    label = "Start Color (Top)";
                    break;
                case GradationEffectDirection.Left:
                    label = "Start Color (Right)";
                    break;
                case GradationEffectDirection.Right:
                    label = "Start Color (Left)";
                    break;
                default:
                    label = "Start Color";
                    break;
            }

            return new GUIContent(label);
        }

        /// <summary>
        /// グラデーションの終了点の色のラベルに指定する文字列を取得します。
        /// </summary>
        /// <returns></returns>
        protected GUIContent GetEndColorLabel()
        {
            string label;

            switch (gradation.Direction)
            {
                case GradationEffectDirection.Up:
                    label = "End Color (Top)";
                    break;
                case GradationEffectDirection.Down:
                    label = "End Color (Bottom)";
                    break;
                case GradationEffectDirection.Left:
                    label = "End Color (Left)";
                    break;
                case GradationEffectDirection.Right:
                    label = "End Color (Right)";
                    break;
                default:
                    label = "End Color";
                    break;
            }

            return new GUIContent(label);
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            gradation = target as Gradation;

            directionProperty = serializedObject.FindProperty("direction");
            typeProperty = serializedObject.FindProperty("type");
            rangeByVerticesProperty = serializedObject.FindProperty("rangeByVertices");
            startColorProperty = serializedObject.FindProperty("startColor");
            endColorProperty = serializedObject.FindProperty("endColor");
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            EditorGUILayout.PropertyField(directionProperty);
            EditorGUILayout.PropertyField(typeProperty);
            EditorGUILayout.PropertyField(rangeByVerticesProperty);

            if (IsSameDirection)
            {
                EditorGUILayout.PropertyField(startColorProperty, GetStartColorLabel());
                EditorGUILayout.PropertyField(endColorProperty, GetEndColorLabel());
            }
            else
            {
                EditorGUILayout.PropertyField(startColorProperty);
                EditorGUILayout.PropertyField(endColorProperty);
            }

            EditorGUI.BeginDisabledGroup(!IsSameColor);
            {
                if (GUILayout.Button("Swap Color"))
                {
                    startColorProperty.colorValue = gradation.EndColor;
                    endColorProperty.colorValue = gradation.StartColor;
                }
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Gradation", GUI.changed, targets);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
