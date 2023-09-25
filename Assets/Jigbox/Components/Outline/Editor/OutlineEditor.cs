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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Outline), true)]
    public class OutlineEditor : Editor
    {
#region constants

        /// <summary>描画回数の最小値</summary>
        protected static readonly int MinDrawCount = Outline.DrawCountLow;

        /// <summary>描画回数の最大値</summary>
        protected static readonly int MaxDrawCount = Outline.MaxDrawCount;

        /// <summary>描画回数を直接編集可能な品質設定のインデックス(QualityLevelがCustomの場合)</summary>
        protected static readonly int DrawCountEditEnableIndex = 4;

#endregion

#region properties

        /// <summary>アウトラインの色</summary>
        protected SerializedProperty effectColorProperty;

        /// <summary>アウトラインの描画位置オフセット</summary>
        protected SerializedVector2 effectDistanceProperty;

        /// <summary>元の頂点情報のアルファ値を加味するかどうか</summary>
        protected SerializedProperty useGraphicAlphaProperty;

        /// <summary>品質</summary>
        protected SerializedProperty qualityLevelProperty;

        /// <summary>アウトラインの描画回数</summary>
        protected SerializedProperty drawCountProperty;

        /// <summary>合成したGUI変更フラグ。</summary>
        protected bool compositedGUIChanged = false;

        /// <summary>選択しているOutlineコンポーネント</summary>
        protected readonly List<Outline> outlines = new List<Outline>();

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            effectColorProperty = serializedObject.FindProperty("m_EffectColor");
            effectDistanceProperty = new SerializedVector2(serializedObject.FindProperty("m_EffectDistance"));
            useGraphicAlphaProperty = serializedObject.FindProperty("m_UseGraphicAlpha");
            qualityLevelProperty = serializedObject.FindProperty("qualityLevel");
            drawCountProperty = serializedObject.FindProperty("drawCount");

            effectDistanceProperty.ResetValue = Vector2.one;

            foreach (var t in targets)
            {
                outlines.Add((Outline) t);
            }
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            compositedGUIChanged = false;

            serializedObject.Update();

            EditorGUILayout.PropertyField(effectColorProperty, new GUIContent("Color"));
            effectDistanceProperty.EditProperty("Distance", "D", 60.0f);
            EditorGUILayout.PropertyField(useGraphicAlphaProperty, new GUIContent("Use Graphic Alpha"));
            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;
            EditorGUI.showMixedValue = outlines.Select(o => o.QualityLevelInEditor).Distinct().Count() >= 2;
            var qualityLevel = (Outline.QualityLevel) EditorGUILayout.EnumPopup("Quality Level", outlines[0].QualityLevelInEditor);
            if (GUI.changed)
            {
                compositedGUIChanged |= GUI.changed;
                GUI.changed = false;
                foreach (var o in outlines)
                {
                    o.QualityLevelInEditor = qualityLevel;
                }
            }

            EditorGUI.BeginDisabledGroup(qualityLevelProperty.enumValueIndex != DrawCountEditEnableIndex);
            {
                EditorGUI.showMixedValue = outlines.Select(o => o.DrawCountInEditor).Distinct().Count() >= 2;
                var drawCount = EditorGUILayout.IntField("Draw Count", outlines[0].DrawCountInEditor);
                if (GUI.changed)
                {
                    foreach (var o in outlines)
                    {
                        o.DrawCountInEditor = drawCount;
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();

            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo("Edit Outline", compositedGUIChanged, targets);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
