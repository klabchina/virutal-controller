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

using System.Reflection;
using Jigbox.Delegatable;
using Jigbox.EditorUtils;
using UnityEditor;
using UnityEngine;

namespace Jigbox.Components
{
    [CustomEditor(typeof(Marquee), true)]
    [CanEditMultipleObjects]
    public class MarqueeEditor : Editor
    {
#region properties

        protected Marquee marquee;

        protected SerializedProperty layoutGroupProperty;

        protected SerializedProperty transitionProperty;

        protected SerializedProperty canvasGroupProperty;

        protected SerializedProperty playOnStartProperty;

        protected SerializedProperty viewportProperty;

        protected SerializedProperty scrollTypeProperty;
        
        protected SerializedProperty scrollDirectionTypeProperty;
        
        protected SerializedProperty entranceAnimationProperty;
        
        protected SerializedProperty entranceEnableProperty;
        
        protected SerializedProperty exitAnimationProperty;
        
        protected SerializedProperty exitEnableProperty;

        protected SerializedProperty speedProperty;

        protected SerializedProperty durationDelayProperty;

        protected SerializedProperty intervalProperty;

        protected SerializedProperty loopDurationDelayProperty;

        protected SerializedProperty startPositionRateProperty;

        protected SerializedProperty endPositionRateProperty;

        protected SerializedProperty isLoopProperty;
        
        /// <summary>合成したGUI変更フラグ。</summary>
        protected bool compositedGUIChanged;

#endregion

#region protected methods

        protected virtual void SearchExtensionComponents()
        {
            serializedObject.Update();

            if (transitionProperty.objectReferenceValue == null)
            {
                MarqueeTransitionBase transitionBase = marquee.GetComponent<MarqueeTransitionBase>();
                transitionProperty.objectReferenceValue = transitionBase;
                if (transitionBase == null)
                {
                    Debug.LogErrorFormat("{0} : Can't GetComponent MarqueeTransitoinBase!", marquee.name);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawMarqueeSettings()
        {
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(layoutGroupProperty);

            EditorGUILayout.PropertyField(transitionProperty);

            EditorGUILayout.PropertyField(canvasGroupProperty);

            EditorGUILayout.PropertyField(playOnStartProperty);
        }

        protected virtual void DrawTransitionModelSettings()
        {
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(viewportProperty);

            EditorGUILayout.PropertyField(scrollTypeProperty);

            bool updatePosition = false;
            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;
            EditorGUILayout.PropertyField(scrollDirectionTypeProperty);
            updatePosition |= GUI.changed;

            EditorGUILayout.PropertyField(speedProperty);

            EditorGUILayout.PropertyField(durationDelayProperty);

            EditorGUILayout.PropertyField(intervalProperty);

            EditorGUILayout.PropertyField(loopDurationDelayProperty);
            
            EditorGUILayout.PropertyField(isLoopProperty);

            DrawScrollAnimationSetting(entranceAnimationProperty, "Entrance Animation");
            DrawScrollAnimationSetting(exitAnimationProperty, "Exit Animation");

            if (EditorUtilsTools.DrawGroupHeader("PositionRate", "PositionRate" + "key"))
            {
                EditorUtilsTools.FitContentToHeader();
                EditorGUILayout.BeginVertical(GUI.skin.box);
                if (entranceEnableProperty.boolValue)
                {
                    EditorGUILayout.HelpBox("Entrance AnimationがONの場合設定できません。", MessageType.Info);
                    EditorGUI.BeginDisabledGroup(true);
                }

                // StartPositionの変更をEditrモードでも適用されるようにする
                compositedGUIChanged |= GUI.changed;
                GUI.changed = false;
                EditorGUILayout.PropertyField(startPositionRateProperty);
                updatePosition |= GUI.changed;

                if (entranceEnableProperty.boolValue)
                {
                    EditorGUI.EndDisabledGroup();
                }

                if (updatePosition)
                {
                    // 変更があった内容を適用して再度serializedObjectのアップデートをかけて更新をおこなう
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                    marquee.InitPosition();
                }

                if (exitEnableProperty.boolValue)
                {
                    EditorGUILayout.HelpBox("Exit AnimationがONの場合設定できません。", MessageType.Info);
                    EditorGUI.BeginDisabledGroup(true);
                }

                EditorGUILayout.PropertyField(endPositionRateProperty);

                if (exitEnableProperty.boolValue)
                {
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.EndVertical();
            }
        }

        protected virtual void DrawScrollAnimationSetting(SerializedProperty animationProperty, string name)
        {
            if (EditorUtilsTools.DrawGroupHeader(name, name+"key"))
            {
                EditorUtilsTools.FitContentToHeader();
                EditorGUILayout.BeginVertical(GUI.skin.box);
                var enableProperty = animationProperty.FindPropertyRelative("enable");
                EditorGUILayout.PropertyField(enableProperty);

                if (enableProperty.boolValue)
                {
                    EditorGUILayout.PropertyField(animationProperty.FindPropertyRelative("motionType"));
                    EditorGUILayout.PropertyField(animationProperty.FindPropertyRelative("easingType"));
                    EditorGUILayout.PropertyField(animationProperty.FindPropertyRelative("duration"));
                    EditorGUILayout.PropertyField(animationProperty.FindPropertyRelative("delay"));
                }
                
                EditorGUILayout.EndVertical();
            }
        }

        protected virtual void DrawCallbackSettings()
        {
            EditorGUILayout.Space();

            if (EditorUtilsTools.DrawGroupHeader("Callback Settings", "Marquee.Callback"))
            {
                EditorUtilsTools.FitContentToHeader();

                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Update Content",
                    marquee,
                    marquee.OnUpdateContentDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Marquee.OnUpdateContent");

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Start Transition",
                    marquee,
                    marquee.OnStartTransitionDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Marquee.OnStartTransition");

                if (entranceEnableProperty.boolValue)
                {
                    EditorGUILayout.Space();
                    
                    DelegatableObjectEditor.DrawEditFields(
                        "On Complete Entrance Delay",
                        marquee,
                        marquee.OnCompleteEntranceDelayDelegates,
                        typeof(AuthorizedAccessAttribute),
                        "Marquee.OnCompleteEntranceDelayDelegates");

                    EditorGUILayout.Space();

                    DelegatableObjectEditor.DrawEditFields(
                        "On Complete Entrance Duration",
                        marquee,
                        marquee.OnCompleteEntranceDurationDelegates,
                        typeof(AuthorizedAccessAttribute),
                        "Marquee.OnCompleteEntranceDurationDelegates");
                }

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Complete Duration Delay",
                    marquee,
                    marquee.OnCompleteDurationDelayDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Marquee.OnCompleteDurationDelay");

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Complete Duration",
                    marquee,
                    marquee.OnCompleteDurationDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Marquee.OnCompleteDuration");

                if (exitEnableProperty.boolValue)
                {
                    EditorGUILayout.Space();
                    
                    DelegatableObjectEditor.DrawEditFields(
                        "On Complete Exit Delay",
                        marquee,
                        marquee.OnCompleteExitDelayDelegates,
                        typeof(AuthorizedAccessAttribute),
                        "Marquee.OnCompleteExitDelayDelegates");

                    EditorGUILayout.Space();

                    DelegatableObjectEditor.DrawEditFields(
                        "On Complete Exit Duration",
                        marquee,
                        marquee.OnCompleteExitDurationDelegates,
                        typeof(AuthorizedAccessAttribute),
                        "Marquee.OnCompleteExitDurationDelegates");
                }

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Complete Interval",
                    marquee,
                    marquee.OnCompleteIntervalDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Marquee.OnCompleteInterval");

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Complete Loop Duration Delay",
                    marquee,
                    marquee.OnCompleteLoopDurationDelayDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Marquee.OnCompleteLoopDelay");

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Complete Layout Content",
                    marquee,
                    marquee.OnCompleteLayoutContentDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Marquee.OnCompleteLayoutContent");

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Pause",
                    marquee,
                    marquee.OnPauseDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Marquee.OnPause");

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Resume",
                    marquee,
                    marquee.OnResumeDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Marquee.OnResume");

                EditorGUILayout.Space();

                DelegatableObjectEditor.DrawEditFields(
                    "On Kill Transition",
                    marquee,
                    marquee.OnKillTransitionDelegates,
                    typeof(AuthorizedAccessAttribute),
                    "Marquee.OnKill");

                EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
            }
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            marquee = target as Marquee;
            layoutGroupProperty = serializedObject.FindProperty("layoutGroup");
            transitionProperty = serializedObject.FindProperty("transition");
            canvasGroupProperty = serializedObject.FindProperty("canvasGroup");
            playOnStartProperty = serializedObject.FindProperty("playOnStart");
            SerializedProperty transitionPropertyProperty = serializedObject.FindProperty("transitionProperty");
            viewportProperty = transitionPropertyProperty.FindPropertyRelative("viewport");
            scrollTypeProperty = transitionPropertyProperty.FindPropertyRelative("scrollType");
            scrollDirectionTypeProperty = transitionPropertyProperty.FindPropertyRelative("scrollDirectionType");
            entranceAnimationProperty = transitionPropertyProperty.FindPropertyRelative("entranceAnimationProperty");
            entranceEnableProperty = entranceAnimationProperty.FindPropertyRelative("enable");
            exitAnimationProperty = transitionPropertyProperty.FindPropertyRelative("exitAnimationProperty");
            exitEnableProperty = exitAnimationProperty.FindPropertyRelative("enable");
            speedProperty = transitionPropertyProperty.FindPropertyRelative("speed");
            durationDelayProperty = transitionPropertyProperty.FindPropertyRelative("durationDelay");
            intervalProperty = transitionPropertyProperty.FindPropertyRelative("interval");
            loopDurationDelayProperty = transitionPropertyProperty.FindPropertyRelative("loopDurationDelay");
            startPositionRateProperty = transitionPropertyProperty.FindPropertyRelative("startPositionRate");
            endPositionRateProperty = transitionPropertyProperty.FindPropertyRelative("endPositionRate");
            isLoopProperty = transitionPropertyProperty.FindPropertyRelative("isLoop");

            // EditモードInspectorの変更から設定が反映されるように、marqueeのInitをリフレクションで実行して必要な情報を渡す
            if (!Application.isPlaying)
            {
                marquee.GetType().GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(marquee, null);
            }

            SearchExtensionComponents();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            compositedGUIChanged = false;
            
            serializedObject.Update();

            DrawMarqueeSettings();
            DrawTransitionModelSettings();
            DrawCallbackSettings();

            serializedObject.ApplyModifiedProperties();
            
            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo("Edit Marquee", compositedGUIChanged, target);

            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
