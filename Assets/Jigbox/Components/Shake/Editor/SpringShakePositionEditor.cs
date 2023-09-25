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
using Jigbox.EditorUtils;
using Jigbox.Shake;
using Jigbox.Tween;

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpringShakePosition))]
    public class SpringShakePositionEditor : PeriodicMovementBaseEditor<SpringShakeVector3>
    {
#region properties
        protected SerializedProperty amplitude;
        protected SerializedProperty frequency;
        protected SerializedProperty xAxis;
        protected SerializedProperty yAxis;
        protected SerializedProperty zAxis;
        protected SerializedProperty angleRandomness;
        protected SerializedVector3 origin;
        protected SerializedProperty envelopeCurve;

        SpringShakePosition shakeTarget;

#endregion

#region protected methods

        protected override void OnEnable()
        {
            shakeTarget = target as SpringShakePosition;
            periodicMovement = serializedObject.FindProperty("shake");

            base.OnEnable();

            var envelopedWave = periodicMovement.FindPropertyRelative("wave");
            amplitude = envelopedWave.FindPropertyRelative("amplitude");
            frequency = envelopedWave.FindPropertyRelative("frequency");
            envelopeCurve = envelopedWave.FindPropertyRelative("envelopeCurve");
            xAxis = periodicMovement.FindPropertyRelative("xAxis");
            yAxis = periodicMovement.FindPropertyRelative("yAxis");
            zAxis = periodicMovement.FindPropertyRelative("zAxis");
            origin = new SerializedVector3(periodicMovement.FindPropertyRelative("origin"));
            angleRandomness = periodicMovement.FindPropertyRelative("angleRandomness");
        }

        protected override void InitProperties()
        {
            var springShakeVector3 = shakeTarget.PeriodicMovement;
            springShakeVector3.Origin = shakeTarget.transform.localPosition;
            springShakeVector3.Wave.EnvelopeCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        }

        protected override void DrawTween()
        {
            EditorGUILayout.Space();

            if (!EditorUtilsTools.DrawGroupHeader("Shake Parameters", HeaderStatusKey))
            {
                return;
            }

            EditorUtilsTools.FitContentToHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                // Shake パラメーター
                EditorGUILayout.PropertyField(amplitude);
                EditorGUILayout.PropertyField(frequency);
                EditorGUILayout.PropertyField(envelopeCurve);

                EditorGUILayout.BeginHorizontal();
                {
                    float labelWidthBackup = EditorGUIUtility.labelWidth;

                    EditorGUIUtility.labelWidth = 50;
                    EditorGUILayout.LabelField("Axises");

                    EditorGUIUtility.labelWidth = 15;
                    EditorGUILayout.PropertyField(xAxis, new GUIContent("X"));
                    EditorGUILayout.PropertyField(yAxis, new GUIContent("Y"));
                    EditorGUILayout.PropertyField(zAxis, new GUIContent("Z"));

                    EditorGUIUtility.labelWidth = labelWidthBackup;
                }
                EditorGUILayout.EndHorizontal();

                origin.EditProperty("Origin", "O");

                EditorGUILayout.PropertyField(angleRandomness);

                // Tween パラメーター
                EditorGUILayout.PropertyField(duration);
                if (duration.floatValue < 0.001f)
                {
                    duration.floatValue = 0.001f;
                }
                EditorGUILayout.PropertyField(delay);
                if (delay.floatValue < 0.0f)
                {
                    delay.floatValue = 0.0f;
                }
                bool isLooped = loopMode.enumValueIndex != (int) Tween.LoopMode.NoLoop;
                // ループ状態でない場合、ループ間隔の設定は無意味
                EditorGUI.BeginDisabledGroup(!isLooped);
                {
                    EditorGUILayout.PropertyField(interval);
                    if (interval.floatValue < 0.0f)
                    {
                        interval.floatValue = 0.0f;
                    }
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.PropertyField(followTimeScale);
                var prevLoopModeIndex = loopMode.enumValueIndex;
                EditorGUILayout.PropertyField(loopMode);
                // サポートしていないモードのときは直前に選択していたものに戻す
                switch (loopMode.enumValueIndex)
                {
                    case (int) Tween.LoopMode.PingPong:
                    case (int) Tween.LoopMode.Yoyo:
                        Debug.LogWarningFormat(
                            "ループモード ”{0}” はサポートしていません",
                            loopMode.enumDisplayNames[loopMode.enumValueIndex]
                        );
                        loopMode.enumValueIndex = prevLoopModeIndex;
                        break;
                }
                // ループ状態でない場合、ループ回数の設定は無意味
                EditorGUI.BeginDisabledGroup(!isLooped);
                {
                    EditorGUILayout.PropertyField(loopCount);
                    if (loopCount.intValue < 0)
                    {
                        loopCount.intValue = 0;
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
        }

#endregion
    }
}
