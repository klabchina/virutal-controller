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
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    [CustomEditor(typeof(ButtonSoundBase), true)]
    [CanEditMultipleObjects]
    public class ButtonSoundBaseEditor : Editor
    {
#region properties

        /// <summary>サウンド再生用コンポーネント</summary>
        protected ButtonSoundBase buttonSound;

        /// <summary>サウンドのエントリ情報のプロパティ</summary>
        protected SerializedProperty entries;

        /// <summary>サウンドイベントの定義フィールド</summary>
        protected string[] fields;

        /// <summary>追加予定のエベントの種類</summary>
        protected InputEventType reservationType = InputEventType.OnClick;

        /// <summary>追加用のエディタを表示するか</summary>
        protected bool isAdd = true;

        /// <summary>StringBuilder</summary>
        protected StringBuilder builder = new StringBuilder();

#endregion

#region protected methods

        /// <summary>
        /// <para>クラス内に存在するフィールドをリストに追加し、</para>
        /// <para>クラス内に別なクラスが存在する場合は再帰的にフィールドを探索します。</para>
        /// </summary>
        /// <param name="type">探索するクラスの型</param>
        /// <param name="list">フィールドを追加するリスト</param>
        protected void FindEvents(Type type, List<string> list)
        {
            string typeName = type.ToString();

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            if (fields != null)
            {
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType == typeof(string))
                    {
                        list.Add(typeName + "." + field.Name);
                    }
                }
            }
            
            Type[] types = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
            if (types != null)
            {
                foreach (Type t in types)
                {
                    FindEvents(t, list);
                }
            }
        }

        /// <summary>
        /// リスト内のフィールドからエディタ上でPopupを扱うための形式に変換します。
        /// </summary>
        /// <param name="list"></param>
        protected void CreateFieldsArray(List<string> list)
        {
            fields = new string[list.Count];

            for (int i = 0; i < fields.Length; ++i)
            {
                builder.Length = 0;
                // 最後の'.'はプロパティとの区切り
                int index = list[i].LastIndexOf('.');
                if (index < 0)
                {
                    fields[i] = list[i];
                    continue;
                }

                int replaceIndex = 0;
                replaceIndex = index;

                // '.'が先頭にはない前提
                --index;
                // 最後から2番目の'.'はnamespaceとの区切り
                index = list[i].LastIndexOf('.', index, index);
                if (index >= 0)
                {
                    replaceIndex = index;
                }

                builder.Append(list[i]);
                // namespace以下の'.'をエディタで扱うために変換
                builder.Replace('.', '/', replaceIndex, list[i].Length - replaceIndex);
                fields[i] = builder.ToString();
            }
        }

        /// <summary>
        /// サウンドのエントリ情報を編集します。
        /// </summary>
        /// <param name="elementIndex">インデックス</param>
        protected bool EditEntry(int elementIndex, bool enableRemove)
        {
            bool isRemove = false;

            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                SerializedProperty entry = entries.GetArrayElementAtIndex(elementIndex);
                SerializedProperty type = entry.FindPropertyRelative("type");
                SerializedProperty fieldPath = entry.FindPropertyRelative("fieldPath");

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(type, new GUIContent("Event"));

                    EditorGUI.BeginDisabledGroup(!enableRemove);
                    {
                        GUI.backgroundColor = new Color(0.8f, 0.4f, 0.4f);
                        if (GUILayout.Button("x", GUILayout.Width(25.0f)))
                        {
                            isRemove = true;
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                bool isEqual = true;

                if (serializedObject.targetObjects.Length > 1)
                {
                    string origin = fieldPath.stringValue;

                    foreach (UnityEngine.Object obj in serializedObject.targetObjects)
                    {
                        ButtonSoundBase selected = obj as ButtonSoundBase;
                        if (selected.Entries[elementIndex].FieldPath != origin)
                        {
                            isEqual = false;
                            break;
                        }
                    }
                }

                int index = isEqual ? GetIndex(fieldPath.stringValue) : 0;
                index = index == -1 ? 0 : index;
                index = EditorGUILayout.Popup("Sound", index, fields);
                if (index != 0)
                {
                    fieldPath.stringValue = fields[index];
                }
            }
            EditorGUILayout.EndVertical();

            if (isRemove)
            {
                EditorUtilsTools.RegisterUndo("Remove Sound Event", GUI.changed, serializedObject.targetObjects);
                entries.DeleteArrayElementAtIndex(elementIndex);
            }

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();

            return isRemove;
        }

        /// <summary>
        /// 追加予定のサウンドのエントリ情報を編集します。
        /// </summary>
        protected void EditReservationEntry()
        {
            EditorGUILayout.Space();
            int index = 0;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                reservationType = (InputEventType) EditorGUILayout.EnumPopup("Event", reservationType);
                index = EditorGUILayout.Popup("Sound", index, fields);
            }
            EditorGUILayout.EndVertical();

            if (index != 0)
            {
                entries.InsertArrayElementAtIndex(entries.arraySize);
                SerializedProperty entry = entries.GetArrayElementAtIndex(entries.arraySize - 1);
                SerializedProperty type = entry.FindPropertyRelative("type");
                SerializedProperty fieldPath = entry.FindPropertyRelative("fieldPath");
                type.intValue = (int) reservationType;
                fieldPath.stringValue = fields[index];

                reservationType = InputEventType.OnClick;
                isAdd = false;
            }
        }

        /// <summary>
        /// Popupに表示しているフィールド内でのインデックスを取得します。
        /// </summary>
        /// <param name="field">対象のフィールド</param>
        /// <returns></returns>
        protected int GetIndex(string field)
        {
            int index = 0;
            foreach (string f in fields)
            {
                if (field == f)
                {
                    return index;
                }
                ++index;
            }
            return -1;
        }

        /// <summary>
        /// 互換性のためにシリアライズされているデータを整形します。
        /// </summary>
        protected void CheckEntries()
        {
            serializedObject.Update();

            for (int i = 0; i < entries.arraySize; ++i)
            {
                SerializedProperty entry = entries.GetArrayElementAtIndex(i);
                SerializedProperty fieldPath = entry.FindPropertyRelative("fieldPath");

                builder.Length = 0;
                int index = fieldPath.stringValue.LastIndexOf('/');
                if (index < 0)
                {
                    continue;
                }

                int replaceIndex = 0;
                replaceIndex = index;
                
                --index;
                index = fieldPath.stringValue.LastIndexOf('/', index, index);
                if (index >= 0)
                {
                    replaceIndex = index;
                }

                builder.Append(fieldPath.stringValue);
                builder.Replace('/', '.', 0, replaceIndex - 1);
                fieldPath.stringValue = builder.ToString();
            }

            serializedObject.ApplyModifiedProperties();
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            buttonSound = target as ButtonSoundBase;

            entries = serializedObject.FindProperty("entries");
            isAdd = entries.arraySize == 0;
            // 互換性保持のためのシリアライズ情報修正用
            if (!isAdd)
            {
                CheckEntries();
            }
                        
            Type soundDefinitionClass = buttonSound.SoundDefinitionClass;

            List<string> fieldList = new List<string>();
            // 未選択用に先頭に空要素を追加
            fieldList.Add("");

            if (soundDefinitionClass != null)
            {
                FindEvents(soundDefinitionClass, fieldList);
            }
            CreateFieldsArray(fieldList);
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            EditorUtilsTools.SetLabelWidth(80.0f);

            serializedObject.Update();

            int editCount = entries.arraySize;
            bool isCantEditEntry = false;
            bool enableRemove = true;
            if (serializedObject.targetObjects.Length > 1)
            {
                // 同時編集は最低要素数分のみ(要素数の異なるものを同時編集しようとするとメモリの不正アクセスになる)
                foreach (UnityEngine.Object obj in serializedObject.targetObjects)
                {
                    ButtonSoundBase selected = obj as ButtonSoundBase;
                    if (selected.Entries.Count < editCount)
                    {
                        editCount = selected.Entries.Count;
                        isCantEditEntry = true;
                    }
                }

                // 要素数が同じで要素の不一致が合った場合、削除ボタンを無効化する
                if (!isCantEditEntry)
                {
                    ButtonSoundBase origin = serializedObject.targetObject as ButtonSoundBase;
                    for (int i = 0; i < origin.Entries.Count; ++i)
                    {
                        InputEventType originType = origin.Entries[i].Type;
                        string originPath = origin.Entries[i].FieldPath;
                        for (int j = 0; j < serializedObject.targetObjects.Length; ++j)
                        {
                            ButtonSoundBase compare = serializedObject.targetObjects[j] as ButtonSoundBase;
                            if (originType != compare.Entries[i].Type
                                || originPath != compare.Entries[i].FieldPath)
                            {
                                enableRemove = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    enableRemove = false;
                }
            }

            for (int i = 0; i < editCount; ++i)
            {
                if (EditEntry(i, enableRemove))
                {
                    break;
                }
            }
            if (isCantEditEntry)
            {
                EditorGUILayout.HelpBox("エントリ情報の数が異なるため、同時に編集できない要素があります。", MessageType.Warning);
            }

            if (isAdd)
            {
                if (!isCantEditEntry)
                {
                    EditReservationEntry();
                }
                else
                {
                    EditorGUILayout.HelpBox("エントリ情報の数が異なる場合、エントリを追加できません", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    
                    if (GUILayout.Button("Add"))
                    {
                        isAdd = true;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Button Sound", GUI.changed, serializedObject.targetObjects);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
