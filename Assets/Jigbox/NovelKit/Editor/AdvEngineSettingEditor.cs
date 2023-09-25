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
    using TextMode = AdvWindowManager.TextMode;

    [CustomEditor(typeof(AdvEngineSetting))]
    public class AdvEngineSettingEditor : Editor
    {
#region constants

        /// <summary>ローカル保存が不可能なエンジンの設定のヘッダーの開閉状態保存キー</summary>
        protected static readonly string EngineSettingKey = "ADV_ENGINE_SETTING";

        /// <summary>ローカル保存が可能なエンジンの設定のヘッダーの開閉状態保存キー</summary>
        protected static readonly string SavedSettingKey = "ADV_SAVED_ENGINE_SETTING";

#endregion

#region properties

#region unsaved settings

        /// <summary>独自拡張したLoaderを使用するか</summary>
        protected SerializedProperty useCustomLoader;

        /// <summary>独自拡張したLocalizerを使用するか</summary>
        protected SerializedProperty useCustomLocalizer;

        /// <summary>画面効果を使用するか</summary>
        protected SerializedProperty useScreenEffection;

        /// <summary>画面効果使用時に自動で表示を切り替えるか</summary>
        protected SerializedProperty autoSwitchScreenEffection;

        /// <summary>コマンドの補完時にサウンドコマンドをスキップするかどうか</summary>
        protected SerializedProperty isSkipSoundCommandComplement;

        /// <summary>バックログの長さ</summary>
        protected SerializedProperty backlogLength;

        /// <summary>バックログのリソースのパス</summary>
        protected SerializedProperty backlogResourcePath;

        /// <summary>選択肢のリソースのパス</summary>
        protected SerializedProperty selectResourcePath;

        /// <summary>テキストの表示モード</summary>
        protected SerializedProperty defaultTextMode;

        /// <summary>TextModeをAdditiveNewLineに指定した際に自動挿入される改行タグ</summary>
        protected SerializedProperty autoAdditiveNewLineTag;

        /// <summary>スキップ状態での時間の短縮率</summary>
        protected SerializedProperty skipTimeScale;

#endregion

#region saved settings

        /// <summary>テキストの文字送り速度</summary>
        protected SerializedProperty textCaptionMargin;

        /// <summary>自動再生時の次のテキストへの待ち時間</summary>
        protected SerializedProperty autoPlayWait;

        /// <summary>自動再生時のテキストの文字送り速度</summary>
        protected SerializedProperty textCaptionMarginAuto;

        /// <summary>クリックで音声を停止させるかどうか</summary>
        protected SerializedProperty stopVoiceWithClick;

        /// <summary>自動再生時、ボイスの終了を待つかどうか</summary>
        protected SerializedProperty waitVoiceEndWhenAuto;

        /// <summary>選択肢で自動再生を解除するかどうか</summary>
        protected SerializedProperty isReleaseAutoWhenSelect;

        /// <summary>選択肢でスキップを解除するかどうか</summary>
        protected SerializedProperty isReleaseSkipWhenSelect;

#endregion

#endregion

#region protected methods

        /// <summary>
        /// ローカル保存が不可能な設定の編集用表示を行います。
        /// </summary>
        protected virtual void DrawUnsavedSettings()
        {
            EditorGUILayout.PropertyField(useCustomLoader, new GUIContent("独自のLoaderを使用"));
            EditorGUILayout.PropertyField(useCustomLocalizer, new GUIContent("独自のローカライザを使用"));
            EditorGUILayout.PropertyField(useScreenEffection, new GUIContent("画面効果を使用"));
            EditorGUI.BeginDisabledGroup(!useScreenEffection.boolValue);
            {
                EditorGUILayout.PropertyField(autoSwitchScreenEffection, new GUIContent("画面効果使用時、自動で表示を切り替える"));
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(isSkipSoundCommandComplement, new GUIContent("コマンドの補完時にサウンドをスキップする"));

            EditorGUILayout.PropertyField(backlogLength, new GUIContent("バックログの長さ"));
            if (backlogLength.intValue < 1)
            {
                backlogLength.intValue = 1;
            }

            EditorGUILayout.PropertyField(backlogResourcePath, new GUIContent("バックログのプレハブ"));
            if (string.IsNullOrEmpty(backlogResourcePath.stringValue))
            {
                EditorGUILayout.HelpBox("リソースを指定して下さい。", MessageType.Error);
            }

            EditorGUILayout.PropertyField(selectResourcePath, new GUIContent("選択肢のプレハブ"));
            if (string.IsNullOrEmpty(selectResourcePath.stringValue))
            {
                EditorGUILayout.HelpBox("リソースを指定して下さい。", MessageType.Error);
            }

            EditorGUILayout.PropertyField(defaultTextMode, new GUIContent("テキスト表示モード"));
            switch (defaultTextMode.enumValueIndex)
            {
                case (int) TextMode.Normal:
                    EditorGUILayout.HelpBox("クリック時に自動でテキストをクリアします。", MessageType.Info);
                    break;
                case (int) TextMode.Additive:
                    EditorGUILayout.HelpBox("クリック時にテキストをクリアしません。", MessageType.Info);
                    break;
                case (int) TextMode.AdditiveNewLine:
                    EditorGUILayout.HelpBox("クリック時にテキストをクリアせず、自動改行を追加します。", MessageType.Info);
                    break;
            }

            if (defaultTextMode.enumValueIndex != (int) TextMode.AdditiveNewLine)
            {
                GUI.color = Color.gray;
            }
            EditorGUILayout.PropertyField(autoAdditiveNewLineTag, new GUIContent("自動改行タグ"));
            GUI.color = Color.white;

            float timeScale = EditorGUILayout.FloatField("スキップ時の倍率", 1.0f / skipTimeScale.floatValue);
            timeScale = timeScale < 1.0f ? 1.0f : timeScale;
            timeScale = 1.0f / timeScale;
            if (timeScale != skipTimeScale.floatValue)
            {
                skipTimeScale.floatValue = timeScale;
            }
        }

        /// <summary>
        /// ローカル保存が可能な設定の編集用表示を行います。
        /// </summary>
        protected virtual void DrawSavedSettings()
        {
            EditorGUILayout.PropertyField(textCaptionMargin, new GUIContent("テキストの字送り間隔"));
            if (textCaptionMargin.floatValue < 0.0f)
            {
                textCaptionMargin.floatValue = 0.0f;
            }

            EditorGUILayout.PropertyField(autoPlayWait, new GUIContent("自動再生時の進行待ち時間"));
            if (autoPlayWait.floatValue < 0.0f)
            {
                autoPlayWait.floatValue = 0.0f;
            }

            EditorGUILayout.PropertyField(textCaptionMarginAuto, new GUIContent("自動再生時の字送り間隔"));
            if (textCaptionMarginAuto.floatValue < 0.0f)
            {
                textCaptionMarginAuto.floatValue = 0.0f;
            }

            EditorGUILayout.PropertyField(stopVoiceWithClick, new GUIContent("クリックでボイスを停止"));
            EditorGUILayout.PropertyField(waitVoiceEndWhenAuto, new GUIContent("自動再生時、ボイスの終了を待つ"));
            EditorGUILayout.PropertyField(isReleaseAutoWhenSelect, new GUIContent("選択肢で自動再生を解除"));
            EditorGUILayout.PropertyField(isReleaseSkipWhenSelect, new GUIContent("選択肢でスキップを解除"));
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            useCustomLoader = serializedObject.FindProperty("useCustomLoader");
            useCustomLocalizer = serializedObject.FindProperty("useCustomLocalizer");
            useScreenEffection = serializedObject.FindProperty("useScreenEffection");
            autoSwitchScreenEffection = serializedObject.FindProperty("autoSwitchScreenEffection");
            isSkipSoundCommandComplement = serializedObject.FindProperty("isSkipSoundCommandComplement");
            backlogLength = serializedObject.FindProperty("backlogLength");
            backlogResourcePath = serializedObject.FindProperty("backlogResourcePath");
            selectResourcePath = serializedObject.FindProperty("selectResourcePath");
            defaultTextMode = serializedObject.FindProperty("defaultTextMode");
            autoAdditiveNewLineTag = serializedObject.FindProperty("autoAdditiveNewLineTag");
            skipTimeScale = serializedObject.FindProperty("skipTimeScale");

            SerializedProperty savedSetting = serializedObject.FindProperty("savedSetting");
            textCaptionMargin = savedSetting.FindPropertyRelative("TextCaptionMargin");
            autoPlayWait = savedSetting.FindPropertyRelative("AutoPlayWait");
            textCaptionMarginAuto = savedSetting.FindPropertyRelative("TextCaptionMarginAuto");
            stopVoiceWithClick = savedSetting.FindPropertyRelative("StopVoiceWithClick");
            waitVoiceEndWhenAuto = savedSetting.FindPropertyRelative("WaitVoiceEndWhenAuto");
            isReleaseAutoWhenSelect = savedSetting.FindPropertyRelative("IsReleaseAutoWhenSelect");
            isReleaseSkipWhenSelect = savedSetting.FindPropertyRelative("IsReleaseSkipWhenSelect");
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            if (EditorUtilsTools.DrawGroupHeader("Engine Settings", EngineSettingKey, true))
            {
                EditorUtilsTools.FitContentToHeader();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawUnsavedSettings();
                EditorGUILayout.EndVertical();
            }

            if (EditorUtilsTools.DrawGroupHeader("Local Saved Settings", SavedSettingKey, true))
            {
                EditorUtilsTools.FitContentToHeader();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSavedSettings();
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Engine Setting", GUI.changed, target);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
