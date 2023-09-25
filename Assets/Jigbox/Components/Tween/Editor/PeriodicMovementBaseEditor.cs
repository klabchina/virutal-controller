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
using Jigbox.Delegatable;
using Jigbox.Tween;

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TweenBase), true)]
    public class PeriodicMovementBaseEditor<TPeriodicMovement>
        : Editor
        where TPeriodicMovement : IPeriodicMovement
    {
#region constatns

        /// <summary>エディタ表示用キー</summary>
        protected static readonly string HeaderStatusKey = "TWEEN_COMPONENT_PARAMS_KEY";

        /// <summary>完了コールバックのエディタ表示用キー</summary>
        protected static readonly string OnCompleteCallbackKey = "TWEEN_COMPONENT_ON_COMPLETE_KEY";

#endregion

#region properties

        /// <summary>Tweenコンポーネントのベースクラス</summary>
        protected PeriodicMovementBase<TPeriodicMovement> tweenBase;

        /// <summary>色の変更対象の種類</summary>
        protected SerializedProperty playOnStart;

        /// <summary>TweenのSerializedObject</summary>
        protected SerializedProperty periodicMovement;

        /// <summary>動きの抑揚</summary>
        protected SerializedProperty motionType;

        /// <summary>動きの緩急</summary>
        protected SerializedProperty easingType;

        /// <summary>ユーザー定義な動き</summary>
        protected SerializedProperty customMotion;

        /// <summary>Tweenの開始値</summary>
        protected SerializedProperty begin;

        /// <summary>Tweenの終了値</summary>
        protected SerializedProperty final;

        /// <summary>Tweenを開始するまでの遅延時間</summary>
        protected SerializedProperty delay;

        /// <summary>Tweenの時間</summary>
        protected SerializedProperty duration;

        /// <summary>ループする際の間隔時間</summary>
        protected SerializedProperty interval;

        /// <summary>タイムスケールを有効にするか</summary>
        protected SerializedProperty followTimeScale;

        /// <summary>ループの種類</summary>
        protected SerializedProperty loopMode;

        /// <summary>ループ回数</summary>
        protected SerializedProperty loopCount;

        /// <summary>Tweenが完了した際のコールバック</summary>
        protected SerializedProperty onCompleteCallback;

#endregion

#region protected methods

        /// <summary>
        /// SerializedPropertyのデフォルト設定を行います。
        /// </summary>
        protected virtual void InitSerializedProperties()
        {
        }

        /// <summary>
        /// 通常のプロパティのデフォルト設定を行います。
        /// </summary>
        protected virtual void InitProperties()
        {
        }

        /// <summary>
        /// SerializedPropertyの編集用フィールドを表示します。
        /// </summary>
        protected virtual void DrawSerializedProperties()
        {
            EditorGUILayout.PropertyField(playOnStart, new GUIContent("Play On Start"));
        }

        /// <summary>
        /// Tweenの編集用フィールドを表示します。
        /// </summary>
        protected virtual void DrawTween()
        {
            EditorGUILayout.Space();

            if (!EditorUtilsTools.DrawGroupHeader("Tween Parameters", HeaderStatusKey))
            {
                return;
            }

            EditorUtilsTools.FitContentToHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(motionType);
                if (motionType.enumValueIndex != (int) MotionType.Custom)
                {
                    EditorGUILayout.PropertyField(easingType);
                }
                else
                {
                    EditorGUILayout.PropertyField(customMotion);
                }

                EditTweenValueField();

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
                bool isLooped = loopMode.enumValueIndex != (int) LoopMode.NoLoop;
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
                EditorGUILayout.PropertyField(loopMode);
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

        protected virtual void DrawCopyDelegateButton()
        {
            if (serializedObject.targetObjects.Length > 1)
            {
                bool isCopy;
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.HelpBox("イベントの設定は、現在選択しているオブジェクトにのみ適用されます。", MessageType.Info);
                    isCopy = GUILayout.Button("選択中の\nイベントをコピー", GUILayout.Width(100.0f));
                }
                EditorGUILayout.EndHorizontal();

                // DelegatableList の中身のコピーを行う
                if (isCopy)
                {
                    DelegatableObjectEditor.CopyToDelegatableListProperty(tweenBase.OnComplete, onCompleteCallback);
                }
            }
        }

        /// <summary>
        /// Tweenの開始値、終了値の編集用フィールドを表示します。
        /// </summary>
        protected virtual void EditTweenValueField()
        {
            EditorGUILayout.PropertyField(begin);
            ValidateBeginParameter();
            EditorGUILayout.PropertyField(final);
            ValidateFinalParameter();
        }

        /// <summary>
        /// Tweenの開始値のバリデーションを行います。
        /// </summary>
        protected virtual void ValidateBeginParameter()
        {
        }

        /// <summary>
        /// Tweenの終了値のバリデーションを行います。
        /// </summary>
        protected virtual void ValidateFinalParameter()
        {
        }

        /// <summary>
        /// 通常のプロパティの編集用フィールドを表示します。
        /// </summary>
        protected virtual void DrawProperties()
        {
        }

        /// <summary>
        /// コールバックの編集用フィールドを表示します。
        /// </summary>
        protected virtual void DrawCallback()
        {
            bool isDisable = loopMode.enumValueIndex != (int) LoopMode.NoLoop && loopCount.intValue == 0;
            
            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(isDisable);
            {
                DelegatableObjectEditor.DrawEditFields(
                    "On Complete",
                    tweenBase,
                    tweenBase.OnComplete,
                    typeof(AuthorizedAccessAttribute),
                    OnCompleteCallbackKey);
            }
            EditorGUI.EndDisabledGroup();

            if (isDisable)
            {
                EditorGUILayout.HelpBox("LoopModeがNoLoopでない場合、LoopCountが設定されていない場合は、このコールバックは呼び出されません。", MessageType.Warning);
            }
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            if (serializedObject.targetObjects.Length == 1)
            {
                tweenBase = target as PeriodicMovementBase<TPeriodicMovement>;
            }
            else
            {
                if (Selection.activeGameObject != null)
                {
                    tweenBase = Selection.activeGameObject.GetComponent<PeriodicMovementBase<TPeriodicMovement>>();
                }
            }

            motionType = periodicMovement.FindPropertyRelative("motionType");
            easingType = periodicMovement.FindPropertyRelative("easingType");
            customMotion = periodicMovement.FindPropertyRelative("customMotion");
            begin = periodicMovement.FindPropertyRelative("begin");
            final = periodicMovement.FindPropertyRelative("final");
            delay = periodicMovement.FindPropertyRelative("delay");
            duration = periodicMovement.FindPropertyRelative("duration");
            interval = periodicMovement.FindPropertyRelative("interval");
            followTimeScale = periodicMovement.FindPropertyRelative("followTimeScale");
            loopMode = periodicMovement.FindPropertyRelative("loopMode");
            loopCount = periodicMovement.FindPropertyRelative("loopCount");
            onCompleteCallback = serializedObject.FindProperty("onComplete");

            SerializedProperty hasBeenInitialized = serializedObject.FindProperty("hasBeenInitialized");
            playOnStart = serializedObject.FindProperty("playOnStart");

            if (hasBeenInitialized != null && !hasBeenInitialized.boolValue)
            {
                serializedObject.Update();

                hasBeenInitialized.boolValue = true;
                playOnStart.boolValue = true;

                InitSerializedProperties();

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
            DrawTween();
            DrawCopyDelegateButton();

            serializedObject.ApplyModifiedProperties();

            DrawProperties();

            DrawCallback();

            EditorUtilsTools.RegisterUndo("Edit Tween", GUI.changed, targets);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
