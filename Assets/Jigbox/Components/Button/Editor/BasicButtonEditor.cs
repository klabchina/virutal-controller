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
    [CustomEditor(typeof(BasicButton), true)]
    [CanEditMultipleObjects]
    public class BasicButtonEditor : ButtonBaseEditor
    {
#region properties

        /// <summary>ボタンの動作を制御するコンポーネントのプロパティの参照</summary>
        protected SerializedProperty transitionComponent;

        /// <summary>イベント毎にサウンドを再生させるコンポーネントのプロパティの参照</summary>
        protected SerializedProperty soundComponent;

#endregion

#region protected methods
        
        /// <summary>
        /// プロパティの初期化を行います。
        /// </summary>
        protected override void InitProperties()
        {
            base.InitProperties();

            transitionComponent = serializedObject.FindProperty("transitionComponent");
            soundComponent = serializedObject.FindProperty("soundComponent");

            SearchButtonExtendComponents();
        }

        /// <summary>
        /// 現在登録、もしくは追加予定となっていないイベント名のリストを更新します。
        /// </summary>
        protected override void UpdateNotExistEventNames()
        {
            UpdateNotExistEventNames(true);
        }

        /// <summary>
        /// Inspectorの表示を行います。
        /// </summary>
        protected override void DrawEditFields()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditSubmitColleague();
                EditColor();
                AttachDefaultExtendComponents();
            }
            EditorGUILayout.EndVertical();

            if (!button.Clickable)
            {
                return;
            }

            GUILayout.Space(2.5f);

            EditEvents();

            GUILayout.Space(2.5f);

            SelectNewEvent();
        }

        /// <summary>
        /// ボタンの動作拡張用コンポーネントを探索します。
        /// 複数選択されている場合にも対応します。
        /// </summary>
        protected virtual void SearchButtonExtendComponents()
        {
            foreach (var obj in targets)
            {
                ButtonBase buttonBase = obj as BasicButton;
                SerializedObject buttonSerializedObject = new SerializedObject(buttonBase);
                buttonSerializedObject.Update();
                SerializedProperty transitionProperty = buttonSerializedObject.FindProperty("transitionComponent");
                SerializedProperty soundProperty = buttonSerializedObject.FindProperty("soundComponent");
                transitionProperty.objectReferenceValue = buttonBase.GetComponent<ButtonTransitionBase>();
                soundProperty.objectReferenceValue = buttonBase.GetComponent<ButtonSoundBase>();
                buttonSerializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 色を編集します。
        /// </summary>
        protected virtual void EditColor()
        {
            if (!this.button.Clickable)
            {
                return;
            }

            BasicButton button = this.button as BasicButton;

            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f);

            EditorGUILayout.BeginVertical(GUI.skin.button);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    bool isEnableColorChange = button.IsEnableColorChange;
                    isEnableColorChange =
                        EditorGUILayout.ToggleLeft("Color", button.IsEnableColorChange, GUILayout.Width(120.0f));
                    if (isEnableColorChange != button.IsEnableColorChange)
                    {
                        button.IsEnableColorChange = isEnableColorChange;
                        // 複数選択時は全てのオブジェクトに展開
                        if (serializedObject.targetObjects.Length > 1)
                        {
                            for (int i = 0; i < serializedObject.targetObjects.Length; ++i)
                            {
                                BasicButton b = serializedObject.targetObjects[i] as BasicButton;
                                b.IsEnableColorChange = isEnableColorChange;
                            }
                        }
                    }

                    if (button.IsEnableColorChange)
                    {
                        bool isSyncColor = button.IsSyncColor;
                        isSyncColor = EditorGUILayout.ToggleLeft("Sync color", button.IsSyncColor);
                        if (isSyncColor != button.IsSyncColor)
                        {
                            button.IsSyncColor = isSyncColor;
                            // 複数選択時は全てのオブジェクトに展開
                            if (serializedObject.targetObjects.Length > 1)
                            {
                                for (int i = 0; i < serializedObject.targetObjects.Length; ++i)
                                {
                                    BasicButton b = serializedObject.targetObjects[i] as BasicButton;
                                    b.IsSyncColor = isSyncColor;
                                }
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (button.IsEnableColorChange)
                {
                    Color disableColor = button.DisableColor;
                    disableColor = EditorGUILayout.ColorField("DisableColor", button.DisableColor);
                    if (disableColor != button.DisableColor)
                    {
                        button.DisableColor = disableColor;
                        // 複数選択時は全てのオブジェクトに展開
                        if (serializedObject.targetObjects.Length > 1)
                        {
                            for (int i = 0; i < serializedObject.targetObjects.Length; ++i)
                            {
                                BasicButton b = serializedObject.targetObjects[i] as BasicButton;
                                b.DisableColor = disableColor;
                            }
                        }
                    }
                }                
            }
            EditorGUILayout.EndVertical();
            
            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// デフォルトの拡張コンポーネントの追加ボタンを表示ます。
        /// </summary>
        protected virtual void AttachDefaultExtendComponents()
        {
            BasicButton button = this.button as BasicButton;

            // 既にコンポーネントが設定されているか、設定するコンポーネントが指定されていない場合は表示しない
            if ((transitionComponent.objectReferenceValue != null || button.DefaultTransitionClass == null)
                && (soundComponent.objectReferenceValue != null || button.DefaultSoundClass == null))
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Separator();

                if (transitionComponent.objectReferenceValue == null && button.DefaultTransitionClass != null)
                {
                    if (GUILayout.Button("Attach Transition", GUILayout.Width(120.0f)))
                    {
                        if (serializedObject.targetObjects.Length == 1)
                        {
                            transitionComponent.objectReferenceValue = button.gameObject.AddComponent(button.DefaultTransitionClass);
                        }
                        // 複数同時選択時は参照をつけると自身のコンポーネントでないものが参照されてしまうので参照を設定しない
                        else
                        {
                            for (int i = 0; i < serializedObject.targetObjects.Length; ++i)
                            {
                                BasicButton b = serializedObject.targetObjects[i] as BasicButton;
                                if (b.gameObject.GetComponent(button.DefaultTransitionClass) == null)
                                {
                                    b.gameObject.AddComponent(button.DefaultTransitionClass);
                                }
                            }
                        }
                    }
                }
                if (soundComponent.objectReferenceValue == null && button.DefaultSoundClass != null)
                {
                    if (GUILayout.Button("Attach Sound", GUILayout.Width(120.0f)))
                    {
                        if (serializedObject.targetObjects.Length == 1)
                        {
                            soundComponent.objectReferenceValue = button.gameObject.AddComponent(button.DefaultSoundClass);
                        }
                        // 複数同時選択時は参照をつけると自身のコンポーネントでないものが参照されてしまうので参照を設定しない
                        else
                        {
                            for (int i = 0; i < serializedObject.targetObjects.Length; ++i)
                            {
                                BasicButton b = serializedObject.targetObjects[i] as BasicButton;
                                if (b.gameObject.GetComponent(button.DefaultSoundClass) == null)
                                {
                                    b.gameObject.AddComponent(button.DefaultSoundClass);
                                }
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();            
        }

#endregion

#region override unity methods

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            EditorUtilsTools.RegisterUndo("Edit Button", GUI.changed, serializedObject.targetObjects);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}

