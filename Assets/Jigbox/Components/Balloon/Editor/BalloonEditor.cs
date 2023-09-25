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

using Jigbox.Delegatable;
using Jigbox.EditorUtils;
using UnityEditor;
using UnityEngine;

namespace Jigbox.Components
{
    [CustomEditor(typeof(Balloon), true)]
    [CanEditMultipleObjects]
    public class BalloonEditor : Editor
    {
#region property

        protected Balloon BalloonProperty;

        protected SerializedProperty BalloonLayoutProperty;

        protected SerializedProperty BalloonCalculatorProperty;

        protected SerializedProperty BalloonContentProperty;

        protected SerializedProperty AutoLayoutAreaProperty;

        protected SerializedProperty BalloonLayoutPositionRateProperty;

        protected SerializedProperty SpacingProperty;

        protected SerializedProperty OpenOnStartProperty;

        protected SerializedProperty DestroyOnCloseProperty;

        protected SerializedProperty BalloonTransitionProperty;

        protected SerializedProperty BalloonLayerExtensionProperty;

        protected SerializedProperty BasePositionProperty;

        protected SerializedProperty BasePositionRectTransformProperty;

#endregion

#region protected methods

        protected virtual void SearchExtensionComponents()
        {
            serializedObject.Update();

            if (BalloonCalculatorProperty.objectReferenceValue == null)
            {
                BalloonCalculatorProperty.objectReferenceValue = BalloonProperty.GetComponent<BalloonLayoutCalculator>();
            }

            if (BalloonTransitionProperty.objectReferenceValue == null)
            {
                BalloonTransitionProperty.objectReferenceValue = BalloonProperty.GetComponent<BalloonTransitionBase>();
            }

            if (BalloonLayerExtensionProperty.objectReferenceValue == null)
            {
                BalloonLayerExtensionProperty.objectReferenceValue = BalloonProperty.GetComponent<BalloonLayerExtension>();
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawGeneralSettings()
        {
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(BalloonContentProperty);

            EditorGUILayout.PropertyField(BalloonCalculatorProperty);

            EditorGUILayout.PropertyField(BalloonLayoutProperty);

            EditorGUILayout.PropertyField(SpacingProperty);

            EditorGUILayout.PropertyField(BalloonLayoutPositionRateProperty);

            EditorGUILayout.PropertyField(BasePositionRectTransformProperty);

            // RectTransformが登録されている場合はBasePositionを更新する
            if (BasePositionRectTransformProperty.objectReferenceValue != null)
            {
                var baseRect = (RectTransform)BasePositionRectTransformProperty.objectReferenceValue;
                var layout = (BalloonLayout)BalloonLayoutProperty.enumValueIndex;
                BasePositionProperty.vector2Value = BalloonBasePositionUtil.GetBasePositionByLayout(baseRect, layout);
            }
        }

        protected virtual void DrawOtherSettings()
        {
            EditorGUILayout.Space();

            if (EditorUtilsTools.DrawGroupHeader("Other Settings", "Balloon.Other"))
            {
                EditorUtilsTools.FitContentToHeader();

                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.PropertyField(AutoLayoutAreaProperty);
                EditorGUILayout.PropertyField(BalloonTransitionProperty);
                EditorGUILayout.PropertyField(BalloonLayerExtensionProperty);
                EditorGUILayout.PropertyField(DestroyOnCloseProperty);
                EditorGUILayout.PropertyField(OpenOnStartProperty);

                EditorGUILayout.EndVertical();
            }
        }

        protected virtual void DrawCallbackSettings()
        {
            EditorGUILayout.Space();

            if (EditorUtilsTools.DrawGroupHeader("Callback Settings", "Balloon.Callback"))
            {
                EditorUtilsTools.FitContentToHeader();

                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Begin Open",
                    BalloonProperty,
                    BalloonProperty.OnBeginOpenDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Balloon.OnBeginOpen");

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Complete Open",
                    BalloonProperty,
                    BalloonProperty.OnCompleteOpenDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Balloon.OnCompleteOpen");

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Begin Close",
                    BalloonProperty,
                    BalloonProperty.OnBeginCloseDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Balloon.OnBeginClose");

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Complete Close",
                    BalloonProperty,
                    BalloonProperty.OnCompleteCloseDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Balloon.OnCompleteClose");

                EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
            }
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            BalloonProperty = target as Balloon;
            SerializedProperty modelProperty = serializedObject.FindProperty("balloonModel");
            BalloonCalculatorProperty = modelProperty.FindPropertyRelative("calculator");
            BalloonContentProperty = modelProperty.FindPropertyRelative("balloonContent");
            AutoLayoutAreaProperty = modelProperty.FindPropertyRelative("autoLayoutArea");
            BalloonLayoutProperty = modelProperty.FindPropertyRelative("balloonLayout");
            BalloonLayoutPositionRateProperty = modelProperty.FindPropertyRelative("balloonLayoutPositionRate");
            SpacingProperty = modelProperty.FindPropertyRelative("spacing");
            OpenOnStartProperty = serializedObject.FindProperty("openOnStart");
            DestroyOnCloseProperty = serializedObject.FindProperty("destroyOnClose");
            BalloonTransitionProperty = serializedObject.FindProperty("balloonTransition");
            BalloonLayerExtensionProperty = serializedObject.FindProperty("balloonLayerExtension");
            BasePositionProperty = modelProperty.FindPropertyRelative("basePosition");
            BasePositionRectTransformProperty = modelProperty.FindPropertyRelative("basePositionRectTransform");

            SearchExtensionComponents();
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            DrawGeneralSettings();
            DrawOtherSettings();
            DrawCallbackSettings();

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Balloon", GUI.changed, target);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
