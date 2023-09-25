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
using Jigbox.Shake;
using UnityEditor;
using UnityEngine;

namespace Jigbox.Components
{
    [CustomEditor(typeof(WaveShakePosition), true)]
    [CanEditMultipleObjects]
    public class WaveShakePositionEditor : Editor
    {
#region constants

        protected static readonly string XHeaderStatusKey = "SHAKE_COMPONENT_PARAMS_KEY_X";
        protected static readonly string YHeaderStatusKey = "SHAKE_COMPONENT_PARAMS_KEY_Y";
        protected static readonly string ZHeaderStatusKey = "SHAKE_COMPONENT_PARAMS_KEY_Z";

#endregion

#region properties

        protected WaveShakePosition shake;

        protected SerializedProperty playOnStart;

        protected SerializedProperty xAxisEnabled;
        protected SerializedProperty xShakeAmplitude;
        protected SerializedProperty xShakeFrequency;
        protected SerializedProperty xShakeDelay;
        protected SerializedProperty xShakeFollowTimeScale;

        protected SerializedProperty yAxisEnabled;
        protected SerializedProperty yShakeAmplitude;
        protected SerializedProperty yShakeFrequency;
        protected SerializedProperty yShakeDelay;
        protected SerializedProperty yShakeFollowTimeScale;

        protected SerializedProperty zAxisEnabled;
        protected SerializedProperty zShakeAmplitude;
        protected SerializedProperty zShakeFrequency;
        protected SerializedProperty zShakeDelay;
        protected SerializedProperty zShakeFollowTimeScale;

        protected SerializedVector3 origin;

#endregion

#region protected methods

        protected virtual void DrawSerializedProperties()
        {
            EditorGUILayout.PropertyField(playOnStart, new GUIContent("Play On Start"));

            var prevFollowTimeScale = xShakeFollowTimeScale.boolValue;
            EditorGUILayout.PropertyField(xShakeFollowTimeScale);
            if (prevFollowTimeScale != xShakeFollowTimeScale.boolValue)
            {
                yShakeFollowTimeScale.boolValue = xShakeFollowTimeScale.boolValue;
                zShakeFollowTimeScale.boolValue = xShakeFollowTimeScale.boolValue;
            }

            origin.EditProperty("Origin", "O");
        }

        protected virtual void DrawShake()
        {
            EditorGUILayout.Space();

            DrawAxis(
                "X Axis",
                XHeaderStatusKey,
                xAxisEnabled,
                xShakeAmplitude,
                xShakeFrequency,
                xShakeDelay
            );

            EditorGUILayout.Space();

            DrawAxis(
                "Y Axis",
                YHeaderStatusKey,
                yAxisEnabled,
                yShakeAmplitude,
                yShakeFrequency,
                yShakeDelay
            );

            EditorGUILayout.Space();

            DrawAxis(
                "Z Axis",
                ZHeaderStatusKey,
                zAxisEnabled,
                zShakeAmplitude,
                zShakeFrequency,
                zShakeDelay
            );
        }

        static void DrawAxis(
            string title,
            string headerStatusKey,
            SerializedProperty axisEnabled,
            SerializedProperty shakeAmplitude,
            SerializedProperty shakeFrequency,
            SerializedProperty shakeDelay)
        {
            if (!EditorUtilsTools.DrawGroupHeader(title, headerStatusKey))
            {
                return;
            }
            EditorUtilsTools.FitContentToHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(axisEnabled, new GUIContent("Enabled"));

                EditorGUI.BeginDisabledGroup(!axisEnabled.boolValue);
                {
                    EditorGUILayout.PropertyField(shakeAmplitude);
                    EditorGUILayout.PropertyField(shakeFrequency);
                    EditorGUILayout.PropertyField(shakeDelay);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 通常のプロパティのデフォルト設定を行います。
        /// </summary>
        protected virtual void InitProperties()
        {
            shake.XAxisEnabled = true;
            shake.YAxisEnabled = true;
            shake.ZAxisEnabled = true;
            shake.Origin = shake.transform.localPosition;
        }

#endregion

#region Unity methods

        protected virtual void OnEnable()
        {
            shake = target as WaveShakePosition;

            playOnStart = serializedObject.FindProperty("playOnStart");

            xAxisEnabled = serializedObject.FindProperty("xAxisEnabled");
            xShakeAmplitude = serializedObject.FindProperty("xShake.wave.amplitude");
            xShakeFrequency = serializedObject.FindProperty("xShake.wave.frequency");
            xShakeDelay = serializedObject.FindProperty("xShake.delay");
            xShakeFollowTimeScale = serializedObject.FindProperty("xShake.followTimeScale");

            yAxisEnabled = serializedObject.FindProperty("yAxisEnabled");
            yShakeAmplitude = serializedObject.FindProperty("yShake.wave.amplitude");
            yShakeFrequency = serializedObject.FindProperty("yShake.wave.frequency");
            yShakeDelay = serializedObject.FindProperty("yShake.delay");
            yShakeFollowTimeScale = serializedObject.FindProperty("yShake.followTimeScale");

            zAxisEnabled = serializedObject.FindProperty("zAxisEnabled");
            zShakeAmplitude = serializedObject.FindProperty("zShake.wave.amplitude");
            zShakeFrequency = serializedObject.FindProperty("zShake.wave.frequency");
            zShakeDelay = serializedObject.FindProperty("zShake.delay");
            zShakeFollowTimeScale = serializedObject.FindProperty("zShake.followTimeScale");

            origin = new SerializedVector3(serializedObject.FindProperty("origin"));

            var hasBeenInitialized = serializedObject.FindProperty("hasBeenInitialized");

            if (hasBeenInitialized != null && !hasBeenInitialized.boolValue)
            {
                serializedObject.Update();

                hasBeenInitialized.boolValue = true;
                playOnStart.boolValue = true;

                serializedObject.ApplyModifiedProperties();

                InitProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            DrawSerializedProperties();
            DrawShake();

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Shake", GUI.changed, targets);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
