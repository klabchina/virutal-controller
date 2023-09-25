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
using System.Reflection;
using System.Text;
using Jigbox.EditorUtils;

namespace Jigbox.Delegatable
{
    public static class DelegatableObjectEditor
    {
#region inner classes, enum, and structs

        public class ReflectionEntry
        {
            /// <summary>対象となるコンポーネント</summary>
            public Component Target { get; protected set; }

            /// <summary>名前</summary>
            public string Name { get; protected set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="target">対象となるコンポーネント</param>
            /// <param name="name">名前</param>
            public ReflectionEntry(Component target, string name)
            {
                Target = target;
                Name = name;
            }
        }

        public class MethodEntry : ReflectionEntry
        {
            /// <summary>メソッド情報</summary>
            public MethodInfo Method { get; protected set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="target">対象となるコンポーネント</param>
            /// <param name="method">メソッド情報</param>
            public MethodEntry(Component target, MethodInfo method) : base(target, method.Name)
            {
                Method = method;
            }
        }

#endregion

#region constants

        /// <summary>破棄ボタンの色</summary>
        static readonly Color RemoveButtonColor = new Color(0.8f, 0.4f, 0.4f);

        /// <summary>メソッドやフィールド情報を取得する際のBingingFlag</summary>
        static readonly BindingFlags BindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

#endregion

#region public methods

        /// <summary>
        /// 指定したGameObjectに付随する全てのコンポーネントから指定した属性を持つメソッドを取得して返します。
        /// </summary>
        /// <param name="target">対象となるGameObject</param>
        /// <param name="attributeType">取得したいメソッドの属性</param>
        /// <returns></returns>
        public static List<ReflectionEntry> GetMethods(GameObject target, System.Type attributeType)
        {
            List<ReflectionEntry> methods = new List<ReflectionEntry>();

            // GameObjectに付随する全てのコンポーネントを取得
            MonoBehaviour[] components = target.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour component in components)
            {
                MethodInfo[] methodsInfo = component.GetType().GetMethods(BindingFlag);

                foreach (MethodInfo methodInfo in methodsInfo)
                {
                    if (methodInfo.ReturnType != typeof(void))
                    {
                        continue;
                    }
                    // 特定の属性が指定されたメソッドのみを抽出
                    if (System.Attribute.GetCustomAttribute(methodInfo, attributeType) != null)
                    {
                        methods.Add(new MethodEntry(component, methodInfo));
                    }
                }
            }

            return methods;
        }

        /// <summary>
        /// <para>指定したGameObjectに付随する全てのコンポーネントから指定した型で</para>
        /// <para>指定した属性を持つフィールド、プロパティを取得して返します。</para>
        /// </summary>
        /// <param name="target">対象となるGameObject</param>
        /// <param name="type">取得したい型</param>
        /// <param name="attributeType">取得したいフィールド、プロパティの属性</param>
        /// <returns></returns>
        public static List<ReflectionEntry> GetProperties(GameObject target, System.Type type, System.Type attributeType)
        {
            List<ReflectionEntry> properties = new List<ReflectionEntry>();

            // GameObjectに付随する全てのコンポーネントを取得
            MonoBehaviour[] components = target.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour component in components)
            {
                System.Type componentType = component.GetType();
                PropertyInfo[] propertiesInfo = componentType.GetProperties(BindingFlag);

                foreach (PropertyInfo propertyInfo in propertiesInfo)
                {
                    if (propertyInfo.PropertyType != type)
                    {
                        continue;
                    }
                    // 特定の属性が指定されたプロパティのみ抽出
                    if (System.Attribute.GetCustomAttribute(propertyInfo, attributeType) != null)
                    {
                        properties.Add(new ReflectionEntry(component, propertyInfo.Name));
                    }
                }

                FieldInfo[] fieldsInfo = componentType.GetFields(BindingFlag);

                foreach (FieldInfo fieldInfo in fieldsInfo)
                {
                    if (fieldInfo.FieldType != type)
                    {
                        continue;
                    }
                    // 特定の属性が指定されたフィールドのみ抽出
                    if (System.Attribute.GetCustomAttribute(fieldInfo, attributeType) != null)
                    {
                        properties.Add(new ReflectionEntry(component, fieldInfo.Name));
                    }
                }
            }

            return properties;
        }

        /// <summary>
        /// デリゲート用オブジェクトの編集フィールドを表示マス。
        /// </summary>
        /// <param name="undoObject">Undoを記録するオブジェクト</param>
        /// <param name="delegatable">デリゲート用オブジェクト</param>
        /// <param name="attributeType">デリゲートとして扱えるインスペクタから扱える属性</param>
        public static void DrawEditField(Object undoObject, DelegatableObject delegatable, System.Type attributeType)
        {
            if (undoObject == null || delegatable == null)
            {
                return;
            }

            bool prevChanged = GUI.changed;

            DrawEditEventField(undoObject, delegatable, attributeType);
            DrawEditArgumentsField(undoObject, delegatable, attributeType);

            // デリゲートの編集が行われていなければ、Inspector上での変更状態を元の状態に設定し直しておく
            // 編集が行われていれば、trueのまま
            if (!GUI.changed)
            {
                GUI.changed = prevChanged;
            }
            return;
        }

        /// <summary>
        /// デリゲート用オブジェクトのリストの編集フィールドを表示します。
        /// </summary>
        /// <param name="label"></param>
        /// <param name="undoObject">Undoを記録するオブジェクト</param>
        /// <param name="delegatableList">デリゲート用オブジェクトのリスト</param>
        /// <param name="attributeType">デリゲートとして扱えるインスペクタから扱える属性</param>
        /// <param name="key">開閉情報の保存キー</param>
        /// <param name="isShowDelete">削除ボタンを表示するかどうか</param>
        /// <returns></returns>
        public static bool DrawEditFields(
            string label,
            Object undoObject,
            DelegatableList delegatableList,
            System.Type attributeType,
            string key = "",
            bool isShowDelete = false)
        {
            bool isDelete = false;

            // 非表示の場合はデリゲートの編集を表示しない
            if (!EditorUtilsTools.DrawGroupHeader(label, key))
            {
                return isDelete;
            }

            EditorUtilsTools.FitContentToHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                List<DelegatableObject> deleteList = new List<DelegatableObject>();

                for (int i = 0; i < delegatableList.Count; ++i)
                {
                    DelegatableObject delegatable = delegatableList.Get(i);

                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        DrawEditField(undoObject, delegatable, attributeType);
                    }
                    EditorGUILayout.EndVertical();

                    // 対象が選択されていない要素は破棄する
                    if (!delegatable.IsValid && delegatable.Target == null)
                    {
                        deleteList.Add(delegatable);
                    }
                }

                foreach (DelegatableObject deleteObject in deleteList)
                {
                    delegatableList.Remove(deleteObject);
                }

                // 追加用に実要素より1つ分多く編集領域を作成
                DelegatableObject delegatableObject = new DelegatableObject();
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawEditField(undoObject, delegatableObject, attributeType);
                }
                EditorGUILayout.EndVertical();

                if (delegatableObject.Target != null || !string.IsNullOrEmpty(delegatableObject.MethodName))
                {
                    delegatableList.Add(delegatableObject);
                }

                if (isShowDelete)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Separator();
                        GUI.backgroundColor = RemoveButtonColor;
                        isDelete = GUILayout.Button("Delete Events", GUILayout.Width(100.0f));
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

            return isDelete;
        }

        /// <summary>
        /// DelegatableListにデリゲートを追加します。
        /// </summary>
        /// <param name="delegatableList">DelegatableList</param>
        /// <param name="target">対象となるMonoBehaviour</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="attributeType">メソッドの属性</param>
        /// <returns></returns>
        public static bool AddDelegate(
            DelegatableList delegatableList,
            MonoBehaviour target,
            string methodName,
            System.Type attributeType)
        {
            MethodInfo methodInfo = GetAddDelegateMethodInfo(target, methodName, attributeType);
            if (methodInfo == null)
            {
                return false;
            }

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.Target = target;
            delegatable.MethodName = methodName;
            delegatable.CreateParameters(methodInfo);

            delegatableList.Add(delegatable);

            return true;
        }

        /// <summary>
        /// DelegatableObjectにデリゲートを設定します。
        /// </summary>
        /// <param name="delegatable">DelegatableObject</param>
        /// <param name="target">対象となるMonoBehaviour</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="attributeType">メソッドの属性</param>
        /// <returns></returns>
        public static bool SetDelegate(
            DelegatableObject delegatable,
            MonoBehaviour target,
            string methodName,
            System.Type attributeType)
        {
            MethodInfo methodInfo = GetAddDelegateMethodInfo(target, methodName, attributeType);
            if (methodInfo == null)
            {
                return false;
            }

            delegatable.Target = target;
            delegatable.MethodName = methodName;
            delegatable.CreateParameters(methodInfo);

            return true;
        }

        /// <summary>
        /// <para>UnityEventからDelegatableListにデリゲートを複製して追加します。</para>
        /// <para>※引数情報は複製されません。</para>
        /// </summary>
        /// <param name="delegatable">DelegatableList</param>
        /// <param name="unityEvent">対象となるUnityEventのプロパティ</param>
        public static void CopyDelegateFromUnityEvent(DelegatableList delegatable, SerializedProperty unityEvent)
        {
            SerializedProperty persistentCalls = unityEvent.FindPropertyRelative("m_PersistentCalls");
            SerializedProperty calls = persistentCalls.FindPropertyRelative("m_Calls");

            int delegateCount = calls.arraySize;
            for (int i = 0; i < delegateCount; ++i)
            {
                SerializedProperty call = calls.GetArrayElementAtIndex(i);
                SerializedProperty target = call.FindPropertyRelative("m_Target");
                SerializedProperty methodName = call.FindPropertyRelative("m_MethodName");
                AddDelegate(delegatable,
                    target.objectReferenceValue as MonoBehaviour,
                    methodName.stringValue,
                    typeof(AuthorizedAccessAttribute));
            }
        }

        /// <summary>
        /// DelegatableListの中身をSerializedPropertyとして渡されるDelegatableListにコピーします。
        /// </summary>
        /// <param name="delegatableList">コピー元のDelegatableList</param>
        /// <param name="delegatableListProperty">コピー先のDelegatableListのSerializedProperty</param>
        public static void CopyToDelegatableListProperty(DelegatableList delegatableList, SerializedProperty delegatableListProperty)
        {
            var property = delegatableListProperty.FindPropertyRelative("delegatableList");
            property.ClearArray();
            if (delegatableList.Count > 0)
            {
                for (int i = 0; i < delegatableList.Count; ++i)
                {
                    property.InsertArrayElementAtIndex(i);
                    DelegatableObject delegatableObject = delegatableList.Get(i);
                    SerializedProperty delegatable = property.GetArrayElementAtIndex(i);
                    CopyToDelegatableObjectProperty(delegatableObject, delegatable);
                }
            }
            else
            {
                // 複数選択時にListが空の場合にコピーが行われない問題の対策コード
                // 一度値を入れ、再度Clearにかけることで変更が起きたとUnityに認識させる
                property.InsertArrayElementAtIndex(0);
                property.ClearArray();
            }
        }

        /// <summary>
        /// DelegatableObjectの中身をSerializedPropertyとして渡されるDelegatableObjectにコピーします
        /// </summary>
        /// <param name="delegatableObject">コピー元のDelegatableList</param>
        /// <param name="delegatableObjectProperty">コピー先のDelegatableListのSerializedProperty</param>
        public static void CopyToDelegatableObjectProperty(DelegatableObject delegatableObject, SerializedProperty delegatableObjectProperty)
        {
            SerializedProperty target = delegatableObjectProperty.FindPropertyRelative("target");
            SerializedProperty methodName = delegatableObjectProperty.FindPropertyRelative("methodName");
            target.objectReferenceValue = delegatableObject.Target;
            methodName.stringValue = delegatableObject.MethodName;

            if (delegatableObject.Parameters == null)
            {
                return;
            }
            SerializedProperty parameters = delegatableObjectProperty.FindPropertyRelative("parameters");
            parameters.ClearArray();
            for (int j = 0; j < delegatableObject.Parameters.Length; ++j)
            {
                parameters.InsertArrayElementAtIndex(j);
                SerializedProperty parameter = parameters.GetArrayElementAtIndex(j);
                SerializedProperty referencedObject = parameter.FindPropertyRelative("referencedObject");
                SerializedProperty fieldName = parameter.FindPropertyRelative("fieldName");
                referencedObject.objectReferenceValue = delegatableObject.Parameters[j].ReferencedObject;
                fieldName.stringValue = delegatableObject.Parameters[j].FieldName;
            }
        }

#endregion

#region private methods

        /// <summary>
        /// デリゲート用オブジェクトのイベントの編集フィールドを表示します。
        /// </summary>
        /// <param name="undoObject">Undoを記録するオブジェクト</param>
        /// <param name="delegatable">デリゲート用オブジェクト</param>
        /// <param name="attributeType">デリゲートとして扱えるインスペクタから扱える属性</param>
        static void DrawEditEventField(Object undoObject, DelegatableObject delegatable, System.Type attributeType)
        {
            GUI.changed = false;
            MonoBehaviour target = delegatable.Target as MonoBehaviour;

            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            EditorGUILayout.BeginVertical(GUI.skin.button);
            {
                GUI.color = Color.white;
                // 対象となるオブジェクトの選択
                bool isRemove = DrawEditEventTarget(delegatable, ref target);

                if (isRemove || delegatable.Target != target)
                {
                    EditorUtilsTools.RegisterUndo("Delegate Target Change", undoObject);
                    delegatable.Target = target;
                    delegatable.MethodName = string.Empty;
                }

                // メソッドの選択
                if (delegatable.Target != null && target != null)
                {
                    ReflectionEntry entry = DrawEditEventCallback(delegatable, target, attributeType);

                    if (entry != null)
                    {
                        EditorUtilsTools.RegisterUndo("Delegate Method Change", undoObject);
                        delegatable.Target = entry.Target as MonoBehaviour;
                        delegatable.MethodName = entry.Name;
                        delegatable.CreateParameters((entry as MethodEntry).Method);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// デリゲートの対象となるオブジェクトの編集フィールドを表示します。
        /// </summary>
        /// <param name="delegatable">DelegatableObject</param>
        /// <param name="target">対象となるMonoBehaviour</param>
        /// <returns>対象オブジェクトが破棄された場合に<c>true</c>を返します。</returns>
        static bool DrawEditEventTarget(DelegatableObject delegatable, ref MonoBehaviour target)
        {
            bool isRemove = false;

            if (target == null)
            {
                if (delegatable.IsValid)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(delegatable.ToString());
                        GUI.backgroundColor = RemoveButtonColor;
                        if (GUILayout.Button("X", GUILayout.Width(20.0f)))
                        {
                            isRemove = true;
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    target = EditorGUILayout.ObjectField("Notify", target, typeof(MonoBehaviour), true) as MonoBehaviour;
                }

                return isRemove;
            }

            if (target == null && delegatable.IsValid)
            {
                EditorGUILayout.LabelField("Notify", delegatable.ToString());
            }
            else
            {
                GUILayout.BeginHorizontal();
                {
                    target = EditorGUILayout.ObjectField("Notify", target, typeof(MonoBehaviour), true) as MonoBehaviour;

                    GUI.backgroundColor = RemoveButtonColor;
                    if (GUILayout.Button("X", GUILayout.Width(20.0f)))
                    {
                        isRemove = true;
                        target = null;
                    }
                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();
            }

            return isRemove;
        }

        /// <summary>
        /// デリゲートの対象となるメソッドの編集フィールドを表示します。
        /// </summary>
        /// <param name="delegatable">DelegatableObject</param>
        /// <param name="target">対象となるMonoBehaviour</param>
        /// <param name="attributeType">取得したいメソッドの属性</param>
        /// <returns></returns>
        static ReflectionEntry DrawEditEventCallback(
            DelegatableObject delegatable, 
            MonoBehaviour target, 
            System.Type attributeType)
        {
            GameObject gameObject = target.gameObject;
            List<ReflectionEntry> methods = GetMethods(gameObject, attributeType);

            int index = 0;
            int selectIndex = 0;
            // 抽出したメソッドをInspector上でプルダウンに表示するための文字列の配列データに変換し、
            // すでにコールバック用のメソッドが登録されている場合は、配列上でのインデックスを取得する
            string[] displayedOptions = GetPopupDisplayedOptions(methods, delegatable.ToString(), out index);

            EditorGUILayout.BeginHorizontal();
            {
                selectIndex = EditorGUILayout.Popup("Method", index, displayedOptions);
            }
            EditorGUILayout.EndHorizontal();

            // selectIndexが0の場合はそもそも操作を行っていないか、
            // 元々と同じものを選択した場合なので、何も返さない
            return (selectIndex > 0 && index != selectIndex) ? methods[selectIndex - 1] : null;
        }

        /// <summary>
        /// デリゲート用オブジェクトのイベントに渡す引数の編集フィールドを表示します。
        /// </summary>
        /// <param name="undoObject">Undoを記録するオブジェクト</param>
        /// <param name="delegatable">デリゲート用オブジェクト</param>
        /// <param name="attributeType">デリゲートとして扱えるインスペクタから扱える属性</param>
        static void DrawEditArgumentsField(Object undoObject, DelegatableObject delegatable, System.Type attributeType)
        {
            ArgumentParameter[] parameters = delegatable.Parameters;
            ParameterInfo[] parametersInfo = delegatable.ParametersInfo;

            if (parameters == null || parametersInfo == null)
            {
                return;
            }

            // パラメータの選択
            for (int i = 0; i < parameters.Length; ++i)
            {
                ArgumentParameter parameter = parameters[i];
                ParameterInfo parameterInfo = parametersInfo[i];

                GUILayout.Space(2.5f);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    Object obj = DrawEditArgumentTarget(parameter, parameterInfo, i);

                    if (obj != parameter.ReferencedObject)
                    {
                        parameter.ReferencedObject = obj;
                        EditorUtility.SetDirty(undoObject);
                    }

                    if (obj == null)
                    {
                        // 途中で抜ける形になるのでレイアウトの終了メソッドを呼び出しておかないと
                        // 自動レイアウト上の不整合がでる
                        EditorGUILayout.EndVertical();
                        continue;
                    }

                    // 選択しているオブジェクトからGameObjectが参照できるか確認
                    // 参照できなければ情報を取得できないので変数は選択できない
                    GameObject gameObject = null;
                    if (obj is GameObject)
                    {
                        gameObject = obj as GameObject;
                    }
                    else if (obj is Component)
                    {
                        gameObject = (obj as Component).gameObject;
                    }

                    if (gameObject != null)
                    {
                        ReflectionEntry entry = DrawEditArgument(gameObject, parameter, parameterInfo);

                        if (entry != null)
                        {
                            EditorUtilsTools.RegisterUndo("Argment Property Change", undoObject);
                            parameter.ReferencedObject = entry.Target;
                            parameter.FieldName = entry.Name;
                        }
                    }
                    else
                    {
                        parameter.FieldName = null;
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// イベントに渡す引数を取得する対象のオブジェクトの編集フィールドを表示します。
        /// </summary>
        /// <param name="parameter">イベントに渡す引数</param>
        /// <param name="parameterInfo">イベントに渡す引数の情報</param>
        /// <param name="argumentIndex">引数のインデックス</param>
        /// <returns>オブジェクトの参照を返します。</returns>
        static Object DrawEditArgumentTarget(
            ArgumentParameter parameter,
            ParameterInfo parameterInfo,
            int argumentIndex)
        {
            StringBuilder builder = new StringBuilder();
            DelegatableObject.ToStringTrimNameSpace(builder, parameterInfo.ParameterType);
            builder.Insert(0, ':').Insert(0, argumentIndex + 1).Insert(0, "Arg");
            // パラメータの取得対象となるオブジェクトの選択
            Object obj = EditorGUILayout.ObjectField(
                builder.ToString(),
                parameter.ReferencedObject,
                typeof(Object),
                true);
            builder = null;

            return obj;
        }

        /// <summary>
        /// イベントに渡す引数の編集フィールドを表示します。
        /// </summary>
        /// <param name="targetObject">引数を取得する対象のGameObject</param>
        /// <param name="parameter">イベントに渡す引数</param>
        /// <param name="parameterInfo">イベントに渡す引数の情報</param>
        /// <returns></returns>
        static ReflectionEntry DrawEditArgument(
            GameObject targetObject,
            ArgumentParameter parameter,
            ParameterInfo parameterInfo)
        {
            List<ReflectionEntry> properties = GetProperties(targetObject, parameterInfo.ParameterType, typeof(AuthorizedAccessAttribute));

            int index = 0;
            int selectIndex = 0;
            string[] displayedOptions = GetPopupDisplayedOptions(properties, parameter.ToString(), out index);

            if (displayedOptions.Length > 1)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    selectIndex = EditorGUILayout.Popup(" ", index, displayedOptions);
                }
                EditorGUILayout.EndHorizontal();
            }
            // 選択可能な要素が実質存在しない場合
            else
            {
                EditorGUILayout.HelpBox("引数として適切なプロパティが存在しません。", MessageType.Warning);
            }

            // selectIndexが0の場合はそもそも操作を行っていないか、
            // 元々と同じものを選択した場合なので、何も返さない
            return (selectIndex > 0 && index != selectIndex) ? properties[selectIndex - 1] : null;
        }

        /// <summary>
        /// リフレクション用のエントリーからInspectorのプルダウンに表示するための文字列の配列を取得します。
        /// </summary>
        /// <param name="entries">エントリーのリスト</param>
        /// <param name="current">現在の文字列</param>
        /// <param name="selectIndex">現在の文字列が戻り値の配列に含まれていた場合、配列におけるインデックス</param>
        /// <returns></returns>
        static string[] GetPopupDisplayedOptions(List<ReflectionEntry> entries, string current, out int selectIndex)
        {
            // 先頭に現在設定されている要素を追加するので
            // リストの要素は実際に利用できる数+1となる
            string[] names = new string[entries.Count + 1];
            names[0] = current;
            selectIndex = 0;

            int index = 1;

            StringBuilder builder = new StringBuilder();

            foreach (ReflectionEntry entry in entries)
            {
                DelegatableObject.ToStringTrimNameSpace(builder, entry.Target.GetType());
                if (entry is MethodEntry)
                {
                    MethodEntry methodEntry = entry as MethodEntry;
                    builder.AppendFormat("/{0}", entry.Name);
                    DelegatableObject.AddMethodParameter(builder, methodEntry.Method.GetParameters());
                    names[index] = builder.ToString();
                }
                else
                {
                    names[index] = builder.AppendFormat("/{0}", entry.Name).ToString();
                }
                if (current == names[index])
                {
                    selectIndex = index;
                }
                ++index;
            }

            return names;
        }

        /// <summary>
        /// デリゲートを追加、設定するためのMethodInfoを取得します。
        /// </summary>
        /// <param name="target">対象となるMonoBehaviour</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="attributeType">メソッドの属性</param>
        /// <returns></returns>
        static MethodInfo GetAddDelegateMethodInfo(MonoBehaviour target, string methodName, System.Type attributeType)
        {
            MethodInfo[] methods = target.GetType().GetMethods(BindingFlag);

            foreach (MethodInfo method in methods)
            {
                if (method.ReturnType != typeof(void))
                {
                    continue;
                }
                if (method.Name != methodName)
                {
                    continue;
                }
                // 特定の属性が指定されたメソッドのみを抽出
                if (System.Attribute.GetCustomAttribute(method, attributeType) != null)
                {
                    return method;
                }
            }

            return null;
        }

#endregion
    }
}
