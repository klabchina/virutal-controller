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
    [CustomEditor(typeof(AdvObjectSetting))]
    public sealed class AdvObjectSettingEditor : Editor
    {
#region inner classes, enum, and structs

        class SerializedObjectSetting
        {
            /// <summary>オブジェクトを生成する際のリソースのパス</summary>
            public SerializedProperty ResourcePath { get; private set; }

            /// <summary>表示切替時のトランジションの時間</summary>
            public SerializedProperty ShowTransitionTime { get; private set; }
            
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="property">プロパティ</param>
            public SerializedObjectSetting(SerializedProperty property)
            {
                ResourcePath = property.FindPropertyRelative("resourcePath");
                ShowTransitionTime = property.FindPropertyRelative("showTransitionTime");
            }

            public void Draw()
            {
                EditorGUILayout.PropertyField(ResourcePath, new GUIContent("プレハブのパス"));
                if (string.IsNullOrEmpty(ResourcePath.stringValue))
                {
                    EditorGUILayout.HelpBox("オブジェクトを使用するには、使用するプレハブのパスを指定する必要があります。", MessageType.Error);
                }
                EditorGUILayout.PropertyField(ShowTransitionTime, new GUIContent("表示非表示時の切り替え時間"));
                if (ShowTransitionTime.floatValue < 0.0f)
                {
                    ShowTransitionTime.floatValue = 0.0f;
                }
            }
        }

        class SerializedObjectSettings
        {
            /// <summary>キャラクター</summary>
            public SerializedObjectSetting Character { get; private set; }

            /// <summary>キャラクター以外の画像</summary>
            public SerializedObjectSetting Sprite { get; private set; }

            /// <summary>背景</summary>
            public SerializedObjectSetting Bg { get; private set; }

            /// <summary>CG(一枚絵)</summary>
            public SerializedObjectSetting Cg { get; private set; }

            /// <summary>感情表現系エモーション</summary>
            public SerializedObjectSetting Emotional { get; private set; }

            /// <summary>演出</summary>
            public SerializedObjectSetting Effect { get; private set; }

            /// <summary>その他</summary>
            public SerializedObjectSetting Other { get; private set; }

            /// <summary>開閉状態保存用キーに付加する文字列</summary>
            string keyString = string.Empty;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="property">プロパティ</param>
            /// <param name="KeyString">キーに付加する文字列</param>
            public SerializedObjectSettings(SerializedProperty property, string KeyString)
            {
                Character = new SerializedObjectSetting(property.FindPropertyRelative("character"));
                Sprite = new SerializedObjectSetting(property.FindPropertyRelative("sprite"));
                Bg = new SerializedObjectSetting(property.FindPropertyRelative("bg"));
                Cg = new SerializedObjectSetting(property.FindPropertyRelative("cg"));
                Emotional = new SerializedObjectSetting(property.FindPropertyRelative("emotional"));
                Effect = new SerializedObjectSetting(property.FindPropertyRelative("effect"));
                Other = new SerializedObjectSetting(property.FindPropertyRelative("other"));
                
                keyString = KeyString;
            }

            public void Draw()
            {
                DrawProperty(Character, "ch「キャラクター」", keyString + "CHARACTER", true);
                DrawProperty(Sprite, "sp「キャラクター以外の画像」", keyString + "SPRITE", true);
                DrawProperty(Bg, "bg「背景」", keyString + "BG", true);
                DrawProperty(Cg, "cg「一枚絵、CG」", keyString + "CG", true);
                DrawProperty(Emotional, "em「感情エモーション」", keyString + "EMOTIONAL", true);
                DrawProperty(Effect, "ef「演出、エフェクト」", keyString + "EFFECT", true);
                DrawProperty(Other, "ot「その他」", keyString + "OTHER", true);
            }
        }

#endregion

#region constants

        /// <summary>開閉状態保存用キーの接頭語</summary>
        static readonly string KeyHeader = "ADV_OBJECT_SETTING_";

#endregion

#region properties

        SerializedObjectSettings mainSetting;

        SerializedObjectSettings subSetting;

        SerializedObjectSetting screenEffection;

#endregion

#region private methods
        
        /// <summary>
        /// プロパティの編集用表示を行います。
        /// </summary>
        /// <param name="property">編集するプロパティ</param>
        /// <param name="label">ラベル</param>
        /// <param name="keySuffix">開閉状態保存用キーの接尾語</param>
        static void DrawProperty(SerializedObjectSetting property, string label, string keySuffix, bool defaultState)
        {
            if (!EditorUtilsTools.DrawGroupHeader(label, KeyHeader + keySuffix, defaultState))
            {
                return;
            }

            EditorUtilsTools.FitContentToHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                property.Draw();
            }
            EditorGUILayout.EndVertical();
        }

#endregion

#region override unity methods


        void OnEnable()
        {
            mainSetting = new SerializedObjectSettings(serializedObject.FindProperty("main"), "MAIN");
            subSetting = new SerializedObjectSettings(serializedObject.FindProperty("sub"), "SUB");
            screenEffection = new SerializedObjectSetting(serializedObject.FindProperty("screenEffection"));
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            // オブジェクト本体の設定
            if (EditorUtilsTools.DrawGroupHeader("Object Settings", KeyHeader + "MAIN", true))
            {
                EditorUtilsTools.FitContentToHeader();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    mainSetting.Draw();
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            // サブオブジェクトの設定
            if (EditorUtilsTools.DrawGroupHeader("SubObject Settings", KeyHeader + "SUB", false))
            {
                EditorUtilsTools.FitContentToHeader();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    subSetting.Draw();
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            // 画面効果
            DrawProperty(screenEffection, "Screen Effection Setting - se「画面効果」", "SCREEN_EFFECTION", false);

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Object Setting", GUI.changed, target);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
