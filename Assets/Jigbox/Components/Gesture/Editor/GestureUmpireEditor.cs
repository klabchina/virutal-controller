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
using System.Collections.Generic;
using Jigbox.Gesture;
using Jigbox.Delegatable;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    public abstract class GestureUmpireEditor<T, U, V> : Editor where T : IConvertible where U : IGestureEventData where V : GestureEventHandler<T>
    {
#region constants
        
        /// <summary>ジェスチャーの開閉状態の保存用キーの先頭文字列</summary>
        protected static readonly string KeyHeader = "Jigbox.Components.GestureUmpire.";

#endregion

#region properties

        /// <summary>ジェスチャーの判別が有効かどうかのプロパティ</summary>
        protected SerializedProperty isEnableProperty;

        /// <summary>計算する座標系をスクリーン座標系で行うかどうかのプロパティ</summary>
        protected SerializedProperty useScreenPositionProperty;
        
        /// <summary>ジェスチャーの判別用クラス</summary>
        protected GestureUmpire<T, U, V> umpire;

        /// <summary>未登録なジェスチャー</summary>
        protected string[] notExistGestures;

        /// <summary>ジェスチャーのデリゲート表示の開閉状態の保存用キー</summary>
        protected virtual string GestureHeaderKey { get { return umpire.GetType() + ".Gestures"; } }

        /// <summary>イベントハンドラ全体のグループヘッダーに表示するラベル名</summary>
        protected virtual string HandlerGroupHeaderLabel { get { return "Gestures"; } }

        /// <summary>イベントハンドラの追加用文言</summary>
        protected virtual string AddHandlerWord { get { return "Add Gesture"; } }

        /// <summary>Canvas情報</summary>
        protected Canvas canvas;

#endregion

#region protected methods
        
        /// <summary>
        /// プロパティの編集用の表示を行います。
        /// </summary>
        protected virtual void DrawProperties()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(isEnableProperty);
            EditorGUILayout.PropertyField(useScreenPositionProperty);

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// デリゲートの編集用の表示を行います。
        /// </summary>
        protected virtual void DrawDelegates()
        {
            List<T> removeGestures = new List<T>();

            foreach (V handler in umpire.GetHandlers())
            {
                if (EditDelegate(handler))
                {
                    removeGestures.Add(handler.Type);
                }
            }

            if (removeGestures.Count > 0)
            {
                foreach (T gesture in removeGestures)
                {
                    umpire.RemoveHandler(gesture);
                }

                UpdateNotExistGestures();
            }
        }

        /// <summary>
        /// デリゲートを編集します。
        /// </summary>
        /// <param name="handler">編集するデリゲートが記録されているイベントハンドラ</param>
        /// <returns></returns>
        protected bool EditDelegate(V handler)
        {
            string label = handler.Type.ToString();
            return DelegatableObjectEditor.DrawEditFields(
                label,
                target,
                handler.Delegates,
                typeof(AuthorizedAccessAttribute),
                KeyHeader + label,
                true);
        }

        /// <summary>
        /// 無効なデリゲート、イベントハンドラを整理します。
        /// </summary>
        protected void RefreshHandlers()
        {
            List<DelegatableObject> removeDelegate = new List<DelegatableObject>();
            List<T> removeGestures = new List<T>();
            
            foreach (V handler in umpire.GetHandlers())
            {
                DelegatableList delegates = handler.Delegates;

                for (int i = 0; i < delegates.Count; ++i)
                {
                    DelegatableObject delegatable = delegates.Get(i);
                    // イベントの発行対象が設定されていないものは削除する
                    if (!delegatable.IsValid && delegatable.Target == null)
                    {
                        removeDelegate.Add(delegatable);
                    }
                }

                foreach (DelegatableObject delegatable in removeDelegate)
                {
                    delegates.Remove(delegatable);
                }

                // デリゲートが空になっている場合はハンドラ自体も削除する
                if (delegates.Count == 0)
                {
                    removeGestures.Add(handler.Type);
                }

                removeDelegate.Clear();
            }

            foreach (T gesture in removeGestures)
            {
                umpire.RemoveHandler(gesture);
            }
        }

        /// <summary>
        /// 新しく追加するジェスチャーを選択して、ジェスチャーのイベントハンドラを追加します。
        /// </summary>
        protected void SelectNewGesture()
        {
            // 追加するものがなければ表示しない
            if (notExistGestures.Length == 1)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                // 右寄せするのに左側にスペースを入れる
                EditorGUILayout.Separator();

                int selectIndex = EditorGUILayout.Popup(0, notExistGestures, GUILayout.Width(160.0f));

                // 0は初期表示用の固定文言
                if (selectIndex > 0)
                {
                    T gesture = (T) Enum.Parse(typeof(T), notExistGestures[selectIndex]);
                    umpire.AddHandler(gesture);
                    UpdateNotExistGestures();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 未登録のジェスチャーを更新します。
        /// </summary>
        protected virtual void UpdateNotExistGestures()
        {
            List<string> gestures = new List<string>();
            // 先頭は必ず固定文言にしておく
            gestures.Add(AddHandlerWord);
            Array gestureTypes = Enum.GetValues(typeof(T));

            foreach (T gesture in gestureTypes)
            {
                if (umpire.GetHandler(gesture) != null)
                {
                    continue;
                }
                gestures.Add(gesture.ToString());
            }

            notExistGestures = gestures.ToArray();
        }

        /// <summary>
        /// GestureDetectorに自身を登録します。
        /// </summary>
        protected virtual void RegisterPropertyToDetector()
        {
            GestureDetector detector = umpire.GetComponent<GestureDetector>();
            if (detector != null)
            {
                canvas = detector.RaycastArea.canvas;

                SerializedObject detectorSerializedObject = new SerializedObject(detector);
                detectorSerializedObject.Update();
                SerializedProperty umpiresProperty = detectorSerializedObject.FindProperty("umpires");

                int arraySize = umpiresProperty.arraySize;
                bool isAlreadyExist = false;
                for (int i = 0; i < arraySize; ++i)
                {
                    SerializedProperty umpireProperty = umpiresProperty.GetArrayElementAtIndex(i);
                    if (umpireProperty.objectReferenceValue == umpire)
                    {
                        isAlreadyExist = true;
                    }
                }

                if (!isAlreadyExist)
                {
                    bool isExistNullProperty = false;
                    for (int i = 0; i < arraySize; ++i)
                    {
                        SerializedProperty umpireProperty = umpiresProperty.GetArrayElementAtIndex(i);
                        if (umpireProperty.objectReferenceValue == null)
                        {
                            umpireProperty.objectReferenceValue = umpire;
                            isExistNullProperty = true;
                            break;
                        }
                    }

                    if (!isExistNullProperty)
                    {
                        umpiresProperty.InsertArrayElementAtIndex(arraySize);
                        SerializedProperty umpireProperty = umpiresProperty.GetArrayElementAtIndex(arraySize);
                        umpireProperty.objectReferenceValue = umpire;
                    }

                    detectorSerializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// float値のSerializedPropertyの編集用表示を行います。
        /// </summary>
        /// <param name="lable">ラベル</param>
        /// <param name="property">SerializedeProperty</param>
        /// <param name="minValue">値の最低値</param>
        /// <param name="maxValue">値の最大値</param>
        protected void FloatField(string lable, SerializedProperty property, float minValue = 0.0f, float maxValue = float.MaxValue)
        {
            float value = EditorGUILayout.FloatField(lable, property.floatValue);
            if (value < minValue)
            {
                value = minValue;
            }
            else if (value > maxValue)
            {
                value = maxValue;
            }

            if (value != property.floatValue)
            {
                property.floatValue = value;
            }
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            umpire = target as GestureUmpire<T, U, V>;
            isEnableProperty = serializedObject.FindProperty("isEnable");
            useScreenPositionProperty = serializedObject.FindProperty("useScreenPosition");

            RegisterPropertyToDetector();
            UpdateNotExistGestures();
        }

        public override void OnInspectorGUI()
        {
            if (canvas == null)
            {
                EditorGUILayout.HelpBox("GestureDetectorと同じGameObjectにアタッチして下さい。", MessageType.Warning);
                return;
            }
            
            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            DrawProperties();

            if (isEnableProperty.boolValue)
            {
                if (!EditorUtilsTools.DrawGroupHeader(HandlerGroupHeaderLabel, GestureHeaderKey))
                {
                    return;
                }

                EditorUtilsTools.FitContentToHeader();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawDelegates();
                    SelectNewGesture();
                }
                EditorGUILayout.EndVertical();
            }

            EditorUtilsTools.RegisterUndo("Edit Gesture Umpire", GUI.changed, target);
            EditorGUI.EndChangeCheck();
        }

        protected virtual void OnDisable()
        {
            RefreshHandlers();
        }

#endregion
    }
}
