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
using System.Collections.Generic;
using Jigbox.UIControl;
using Jigbox.EditorUtils;
using Jigbox.Delegatable;

namespace Jigbox.Components
{
    using EventEntry = ButtonBase.EventEntry;

    [CustomEditor(typeof(ButtonBase), true)]
    [CanEditMultipleObjects]
    public class ButtonBaseEditor : Editor
    {
#region constants

        /// <summary>デフォルトのロック解除後の再ロック可能になるまでの待機時間</summary>
        protected static readonly float DefaultCoolTime = 0.2f;

        /// <summary>ドラッグ可能かのチェック間隔</summary>
        static readonly float DraggableCheckIntervalTime = 3.0f;

#endregion

#region properties

        /// <summary>ボタン</summary>
        protected ButtonBase button;

        /// <summary>colleagueのプロパティの参照</summary>
        protected SerializedProperty colleagueProperty;
        
        /// <summary>現在登録されていないイベント名のリスト</summary>
        protected string[] notExistEvents;

        /// <summary>追加予定のイベントのリスト</summary>
        protected List<EventEntry> reservationEntries = new List<EventEntry>();

        /// <summary>ドラッグ関連のコールバックが使用できるかどうか</summary>
        protected bool isDraggable = false;

        /// <summary>最後のチェック時間</summary>
        double lastCheckTime = 0.0f;

#endregion

#region public methods

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="button">対象のボタンコンポーネント</param>
        /// <param name="type">入力イベントの種類</param>
        /// <param name="target">デリゲートの対象となるMonoBehaviour</param>
        /// <param name="methodName">メソッド名</param>
        public static void AddEvent(ButtonBase button, InputEventType type, MonoBehaviour target, string methodName)
        {
            EventEntry entry = null;
            foreach (EventEntry e in button.Entries)
            {
                if (e.Type == type)
                {
                    entry = e;
                }
            }

            if (entry == null)
            {
                entry = new EventEntry(type);
                button.Entries.Add(entry);
            }

            DelegatableObjectEditor.AddDelegate(entry.Delegates, target, methodName, typeof(AuthorizedAccessAttribute));
        }

#endregion

#region protected methods

        /// <summary>
        /// プロパティの初期化を行います。
        /// </summary>
        protected virtual void InitProperties()
        {
            colleagueProperty = serializedObject.FindProperty("colleague");

            // 複数選択時は値が使い手が意図しない形になる可能性があるので設定しない
            if (serializedObject.targetObjects.Length == 1)
            {
                SerializedProperty raycastAreaProperty = serializedObject.FindProperty("raycastArea");
                if (raycastAreaProperty.objectReferenceValue == null)
                {
                    RaycastArea raycastArea = button.GetComponent<RaycastArea>();
                    raycastArea.color = Color.clear;
                    raycastAreaProperty.objectReferenceValue = raycastArea;
                }

                SerializedProperty graphicGroupProperty = serializedObject.FindProperty("graphicGroup");
                if (graphicGroupProperty.objectReferenceValue == null)
                {
                    graphicGroupProperty.objectReferenceValue = button.GetComponent<GraphicGroup>();
                }

                // SubmitColleagueはnull非許容型であるため、初期化されているかどうかはnullで判断できないため
                // 内部のIDが設定されているかどうかを初期化条件として扱う
                // objectRefelenceValueはnull許容型であるが、今回は実体がnull非許容型であるため、
                // nullチェックが行われるような記述を行うとエラーメッセージが出力される
                SerializedProperty idProperty = colleagueProperty.FindPropertyRelative("id");
                if (idProperty.intValue == 0)
                {
                    SerializedProperty coolTimeProperty = colleagueProperty.FindPropertyRelative("coolTime");
                    idProperty.intValue = button.GetInstanceID();
                    coolTimeProperty.floatValue = DefaultCoolTime;
                }
            }
        }

        /// <summary>
        /// 編集によって参照が剥がれているデリゲートをクリアします。
        /// </summary>
        protected void RefleshEntry()
        {
            List<DelegatableObject> removeDelegate = new List<DelegatableObject>();
            List<EventEntry> removeEntries = new List<EventEntry>();

            foreach (EventEntry entry in button.Entries)
            {
                for (int i = 0; i < entry.Delegates.Count; ++i)
                {
                    DelegatableObject del = entry.Delegates.Get(i);
                    if (!del.IsValid && del.Target == null)
                    {
                        removeDelegate.Add(del);
                    }
                }

                foreach (DelegatableObject del in removeDelegate)
                {
                    entry.Delegates.Remove(del);
                }

                // デリゲートがなくなったものはエントリ情報自体を破棄する
                if (entry.Delegates.Count == 0)
                {
                    removeEntries.Add(entry);
                }

                removeDelegate.Clear();
            }

            foreach (EventEntry entry in removeEntries)
            {
                button.Entries.Remove(entry);
            }
        }

        /// <summary>
        /// 新しく追加予定のイベントを選択します。
        /// </summary>
        protected void SelectNewEvent()
        {
            // イベント名に空白しかなければ表示しない
            if (notExistEvents.Length == 1)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Separator();
                int selectIndex = EditorGUILayout.Popup(0, notExistEvents, GUILayout.Width(160.0f));

                if (selectIndex > 0)
                {
                    InputEventType type = (InputEventType)System.Enum.Parse(typeof(InputEventType), notExistEvents[selectIndex]);
                    reservationEntries.Add(new EventEntry(type));
                    UpdateNotExistEventNames();
                }
            }
            EditorGUILayout.EndHorizontal();            
        }
        
        /// <summary>
        /// 現在登録、もしくは追加予定となっていないイベント名のリストを更新します。
        /// </summary>
        /// <param name="existExtensionEventTypes">Unityの標準イベント以外のボタンイベントが存在するかどうか</param>
        protected void UpdateNotExistEventNames(bool existExtensionEventTypes)
        {
            List<string> events = new List<string>();
            // 先頭は必ず固定文言にしておく
            events.Add("Add Event");
            System.Array eventTypes = System.Enum.GetValues(typeof(InputEventType));

            foreach (InputEventType type in eventTypes)
            {
                if (!existExtensionEventTypes)
                {
                    switch (type)
                    {
                        // 拡張クラス側でなければ、以下のイベントは登録しても動作しないため除外
                        case InputEventType.OnLongPress:
                        case InputEventType.OnKeyRepeat:
                        case InputEventType.OnUpdateSelected:
                            continue;
                    }
                }

                if (!isDraggable)
                {
                    switch (type)
                    {
                        // ドラッグ用コンポーネントがアタッチされていない場合は、
                        // 以下のイベントは登録しても動作しないため除外
                        case InputEventType.OnInitDrag:
                        case InputEventType.OnBeginDrag:
                        case InputEventType.OnDrag:
                        case InputEventType.OnEndDrag:
                            continue;
                    }
                }

                bool isExist = false;
                // すでに登録済み、もしくは追加予定のリストにあるイベント名は除外
                foreach (EventEntry entry in button.Entries)
                {
                    if (entry.Type == type)
                    {
                        isExist = true;
                        break;
                    }
                }
                foreach (EventEntry entry in reservationEntries)
                {
                    if (entry.Type == type)
                    {
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    events.Add(type.ToString());
                }
            }

            notExistEvents = events.ToArray();
        }


        /// <summary>
        /// 現在登録、もしくは追加予定となっていないイベント名のリストを更新します。
        /// </summary>
        protected virtual void UpdateNotExistEventNames()
        {
            UpdateNotExistEventNames(false);
        }

        /// <summary>
        /// イベントを編集します。
        /// </summary>
        /// <param name="entry">イベントのエントリ情報</param>
        /// <returns></returns>
        protected bool EditEvent(EventEntry entry)
        {
            return DelegatableObjectEditor.DrawEditFields(
                entry.Type.ToString(), button, entry.Delegates, typeof(AuthorizedAccessAttribute), GetShowEventDelegateKey(entry.Type), true);
        }

        /// <summary>
        /// イベントの編集用エディタ表示の開閉状態を保存するキーを取得します。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string GetShowEventDelegateKey(InputEventType type)
        {
            return button.GetType().ToString() + "." + type.ToString();
        }

        /// <summary>
        /// イベントの編集用エディタ表示の開閉状態を保存するキーを取得します。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string GetShowEventDelegateKey(System.Type type)
        {
            return button.GetType().ToString() + "." + type.ToString();
        }

        /// <summary>
        /// 入力の排他制御用情報を編集します。
        /// </summary>
        protected void EditSubmitColleague()
        {
            button.Clickable = EditorGUILayout.ToggleLeft(" Enable", button.Clickable);

            if (!button.Clickable)
            {
                return;
            }

            // 入力の排他制御情報を編集
            SubmitColleagueEditor.DrawEdit(colleagueProperty);
        }

        /// <summary>
        /// ボタンに記録するイベントを編集します。
        /// </summary>
        protected void EditEvents()
        {
            if (serializedObject.targetObjects.Length > 1)
            {
                bool isCopy = false;
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.HelpBox("イベントの設定は、現在選択しているオブジェクトにのみ適用されます。", MessageType.Info);
                    isCopy = GUILayout.Button("選択中の\nイベントをコピー", GUILayout.Width(100.0f));
                }
                EditorGUILayout.EndHorizontal();

                // シリアライズされているデータを一旦クリアして、全て展開し直す
                if (isCopy)
                {
                    SerializedProperty entriesProperty = serializedObject.FindProperty("entries");
                    entriesProperty.ClearArray();
                    
                    for (int i = 0; i < button.Entries.Count; ++i)
                    {
                        EventEntry entry = button.Entries[i];
                        entriesProperty.InsertArrayElementAtIndex(i);
                        SerializedProperty element = entriesProperty.GetArrayElementAtIndex(i);
                        SerializedProperty type = element.FindPropertyRelative("type");
                        SerializedProperty delegatableList = element.FindPropertyRelative("delegates");
                        type.intValue = (int)entry.Type;
                        // entry が持つ DelegatableList の中身を SerializedProperty へコピーする
                        DelegatableObjectEditor.CopyToDelegatableListProperty(entry.Delegates, delegatableList);
                    }

                    if (button.Entries.Count == 0)
                    {
                        // 複数選択時にListが空の場合にコピーが行われない問題の対策コード
                        // 一度値を入れ、再度Clearにかけることで変更が起きたとUnityに認識させる
                        entriesProperty.InsertArrayElementAtIndex(0);
                        entriesProperty.ClearArray();
                    }

                    return;
                }
            }

            bool isReflesh = false;
            List<EventEntry> removeList = new List<EventEntry>();

            // 登録済みイベントの編集
            foreach (EventEntry entry in button.Entries)
            {
                if (!isDraggable)
                {
                    switch (entry.Type)
                    {
                        // ドラッグ用コンポーネントがアタッチされていない場合は、
                        // 以下のイベントは登録しても動作しないため、色を変更して無効状態のように見せる
                        case InputEventType.OnInitDrag:
                        case InputEventType.OnBeginDrag:
                        case InputEventType.OnDrag:
                        case InputEventType.OnEndDrag:
                            GUI.contentColor = Color.gray;
                            break;
                    }
                }
                if (EditEvent(entry) || entry.Delegates.Count == 0)
                {
                    removeList.Add(entry);
                }
                GUI.contentColor = Color.white;
            }

            if (removeList.Count > 0)
            {
                foreach (EventEntry removeEntry in removeList)
                {
                    button.Entries.Remove(removeEntry);
                }

                removeList.Clear();
                isReflesh = true;
            }

            // 追加予定のイベントを編集
            foreach (EventEntry entry in reservationEntries)
            {
                if (EditEvent(entry))
                {
                    removeList.Add(entry);
                }
                // 1つでもイベントが登録された時点でボタン側へエントリを移動
                else if (entry.Delegates.Count > 0)
                {
                    button.Entries.Add(entry);
                    removeList.Add(entry);
                }
            }

            if (removeList.Count > 0)
            {
                foreach (EventEntry removeEntry in removeList)
                {
                    reservationEntries.Remove(removeEntry);
                }

                removeList.Clear();
                isReflesh = true;
            }

            if (isReflesh)
            {
                UpdateNotExistEventNames();
            }
        }

        /// <summary>
        /// Inspectorの表示を行います。
        /// </summary>
        protected virtual void DrawEditFields()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditSubmitColleague();
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
        
#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            if (serializedObject.targetObjects.Length == 1)
            {
                button = target as ButtonBase;
            }
            else
            {
                if (Selection.activeGameObject != null)
                {
                    button = Selection.activeGameObject.GetComponent<ButtonBase>();
                }

                // Button以外のコンポーネントを選択してから複数のButtonを選択した場合にnullになる
                // 異なるコンポーネントが選択された場合はそもそもInspector上で表示されないのでOnEnableの処理もしなくて良い
                if (button == null)
                {
                    return;
                }
            }

            RefleshEntry();

            serializedObject.Update();

            InitProperties();

            serializedObject.ApplyModifiedProperties();

            // イベント情報がなければ、とりあえずOnClickを追加予定リストに追加
            if (button.Entries.Count == 0 && reservationEntries.Count == 0)
            {
                reservationEntries.Add(new EventEntry(InputEventType.OnClick));
            }

            UpdateNotExistEventNames();
        }

        public override void OnInspectorGUI()
        {
            if (button == null)
            {
                return;
            }

            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            double time = EditorApplication.timeSinceStartup;
            if (time - lastCheckTime >= DraggableCheckIntervalTime)
            {
                bool lastDraggable = isDraggable;
                isDraggable = button.GetComponent<DragBehaviour>() != null;
                if (lastDraggable != isDraggable)
                {
                    UpdateNotExistEventNames();
                }
                lastCheckTime = time;
            }

            serializedObject.Update();

            DrawEditFields();

            serializedObject.ApplyModifiedProperties();
            
            EditorUtilsTools.RegisterUndo("Edit Button", GUI.changed, button);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }
        
#endregion
    }
}
