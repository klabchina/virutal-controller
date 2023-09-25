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
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Jigbox.Delegatable
{
    [System.Serializable]
    public class DelegatableObject
    {
#region inner classes, enum, and structs

        /// <summary>
        /// デフォルトのデリゲート型
        /// </summary>
        public delegate void Callback();

#endregion

#region constants

        /// <summary>メソッドのバインドフラグ</summary>
        public static readonly BindingFlags ReflectionBindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

#endregion

#region properties

        /// <summary>イベントの発行対象</summary>
        [SerializeField]
        Object target;

        /// <summary>イベントの発行対象</summary>
        public Object Target
        {
            get
            {
                return target;
            }
            set
            {
                if (target != value)
                {
                    Clear();
                    target = value;
                }
            }
        }

        /// <summary>イベントを発行した際に呼び出されるメソッド名</summary>
        [SerializeField]
        string methodName;

        /// <summary>イベントを発行した際に呼び出されるメソッド名</summary>
        public string MethodName
        {
            get
            {
                return methodName;
            }
            set
            {
                if (methodName != value)
                {
                    // Targetまではクリアしないので、Clear()は使わない
                    methodName = value;
                    callback = null;
                    eventDelegate = null;
                    isCached = false;
                    methodInfo = null;
                    parameters = null;
                    parametersInfo = null;
                    args = null;

#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        isCached = true;
                        FindCallback();
                    }
#endif
                }
            }
        }

        /// <summary>イベントを発行した際に呼び出されるメソッドの引数</summary>
        [SerializeField]
        ArgumentParameter[] parameters;

        /// <summary>イベントを発行した際に呼び出されるメソッドの引数</summary>
        public ArgumentParameter[] Parameters
        {
            get
            {
#if UNITY_EDITOR
                if (!isCached || !Application.isPlaying)
#else
                if (!isCached)
#endif
                {
                    isCached = true;
                    FindCallback();
                }
                return parameters;
            }
        }

        /// <summary>メソッドがキャッシュ済みかどうか(毎回探さないため)</summary>
        [System.NonSerialized]
        bool isCached = false;

        /// <summary>無引数のイベント</summary>
        [System.NonSerialized]
        Callback callback;

        [System.NonSerialized]
        IEventDelegate eventDelegate;

        /// <summary>イベントを発行した際に呼び出されるメソッドの情報</summary>
        [System.NonSerialized]
        MethodInfo methodInfo;

        /// <summary>イベントを発行した際に呼び出されるメソッドの引数情報</summary>
        [System.NonSerialized]
        ParameterInfo[] parametersInfo;

        /// <summary>イベントを発行した際に呼び出されるメソッドの引数情報</summary>
        public ParameterInfo[] ParametersInfo
        {
            get
            {
#if UNITY_EDITOR
                if (!isCached || !Application.isPlaying)
#else
                if (!isCached)
#endif
                {
                    isCached = true;
                    FindCallback();
                }
                return parametersInfo;
            }
        }

        /// <summary>イベントを発行した際に呼び出されるメソッドの引数</summary>
        [System.NonSerialized]
        object[] args;

        /// <summary>デリゲートが有効かどうか</summary>
        public bool IsValid
        {
            get
            {
#if UNITY_EDITOR
                if (!isCached || !Application.isPlaying)
#else
                if (!isCached)
#endif
                {
                    isCached = true;
                    FindCallback();
                }

                return (callback != null) || (eventDelegate != null) || (target != null && !string.IsNullOrEmpty(methodName));
            }
        }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DelegatableObject()
        {
        }

        /// <summary>
        /// デリゲートを実行します。
        /// </summary>
        /// <returns></returns>
        public bool Execute()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("DelegatableObject.Execute : Can't call! The method is enable when playing!");
                return false;
            }
#endif
            if (!isCached)
            {
                isCached = true;
                FindCallback();
            }

            if (callback != null)
            {
                callback();
                return true;
            }

            if (methodInfo == null)
            {
                return false;
            }

            int argsLength = parameters != null ? parameters.Length : 0;

            // 引数なしの場合
            if (argsLength == 0)
            {
                if (eventDelegate != null)
                {
                    eventDelegate.Invoke(null);
                }
                else
                {
                    methodInfo.Invoke(target, null);
                }
                return true;
            }

            // 引数ありの場合
            if (args == null)
            {
                args = new object[argsLength];
            }

            for (int i = 0; i < argsLength; ++i)
            {
                args[i] = parameters[i].Value;
            }

            try
            {
                if (eventDelegate != null)
                {
                    eventDelegate.Invoke(args);
                }
                else
                {
                    methodInfo.Invoke(target, args);
                }

                for (int i = 0; i < argsLength; ++i)
                {
                    // ref、outが引数の属性として設定されていた場合は値を更新する
                    if (parametersInfo[i].IsIn || parametersInfo[i].IsOut)
                    {
                        parameters[i].Value = args[i];
                    }
                }

                return true;
            }
            catch (System.ArgumentException e)
            {
                string errorMessage = e.ToString() + "\n";
                errorMessage += "DelegatableObject.Execute : Can't calling method!\n"
                    + "Target : " + target.name
                    + "Method : " + methodInfo.Name + "\n";

                for (int i = 0; i < argsLength; ++i)
                {
                    errorMessage += "\narg" + (i + 1) + " : ";

                    Object obj = parameters[i].ReferencedObject;
                    string fieldName = parameters[i].FieldName;
                    if (obj != null && !string.IsNullOrEmpty(fieldName))
                    {
                        errorMessage += obj.GetType() + "." + parameters[i].FieldName;
                    }
                    else if (args[i] == null)
                    {
                        errorMessage += "null";
                        continue;
                    }

                    System.Type argType = args[i].GetType();
                    if (argType != parametersInfo[i].ParameterType &&
                        !argType.IsSubclassOf(parametersInfo[i].ParameterType))
                    {
                        errorMessage += " - error argument type : " + argType;
                    }
                }

                Debug.LogError(errorMessage);
            }

            return false;
        }

        /// <summary>
        /// <para>デリゲートを実行します。</para>
        /// <para>実行するデリゲートの引数に特定の型が含まれる場合、対象の引数を指定したデータで置き換えます。</para>
        /// </summary>
        /// <param name="data">特定の型の引数が存在した場合のデータ</param>
        /// <returns></returns>
        public bool Execute<T>(T data)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("DelegatableObject.Execute : Can't call! The method is enable when playing!");
                return false;
            }
#endif
            if (!isCached)
            {
                isCached = true;
                FindCallback();
            }

            if (callback != null)
            {
                callback();
                return true;
            }

            if (methodInfo == null)
            {
                return false;
            }

            int argsLength = parameters != null ? parameters.Length : 0;

            // 引数なしの場合
            if (argsLength == 0)
            {
                if (eventDelegate != null)
                {
                    eventDelegate.Invoke(null);
                }
                else
                {
                    methodInfo.Invoke(target, null);
                }
                return true;
            }

            // 引数ありの場合
            if (args == null)
            {
                args = new object[argsLength];
            }

            System.Type dataType = data.GetType();

            // 引数として渡すデータを設定する際に、
            // 指定されたデータと同じ型を持つ引数は
            // そのデータを引数として使用するように上書きする
            for (int i = 0; i < argsLength; ++i)
            {
                if (parametersInfo[i].ParameterType != dataType &&
                    !dataType.IsSubclassOf(parametersInfo[i].ParameterType))
                {
                    args[i] = parameters[i].Value;
                }
                else
                {
                    args[i] = data;
                }
            }

            try
            {
                if (eventDelegate != null)
                {
                    eventDelegate.Invoke(args);
                }
                else
                {
                    methodInfo.Invoke(target, args);
                }

                for (int i = 0; i < argsLength; ++i)
                {
                    // ref、outが引数の属性として設定されていた場合は値を更新する
                    if (parametersInfo[i].IsIn || parametersInfo[i].IsOut)
                    {
                        parameters[i].Value = args[i];
                    }
                }

                return true;
            }
            catch (System.ArgumentException e)
            {
                string errorMessage = e.ToString() + "\n";
                errorMessage += "DelegatableObject.Execute : Can't calling method!\n"
                    + "Target : " + target.name
                    + "Method : " + methodInfo.Name + "\n";

                for (int i = 0; i < argsLength; ++i)
                {
                    errorMessage += "\narg" + (i + 1) + " : ";

                    if (parametersInfo[i].ParameterType == dataType ||
                        dataType.IsSubclassOf(parametersInfo[i].ParameterType))
                    {
                        errorMessage += data.GetType().ToString();
                        continue;
                    }

                    Object obj = parameters[i].ReferencedObject;
                    string fieldName = parameters[i].FieldName;
                    if (obj != null && !string.IsNullOrEmpty(fieldName))
                    {
                        errorMessage += obj.GetType().ToString() + "." + parameters[i].FieldName;
                    }
                    else if (args[i] == null)
                    {
                        errorMessage += "null";
                        continue;
                    }

                    System.Type argType = args[i].GetType();
                    if (argType != parametersInfo[i].ParameterType)
                    {
                        errorMessage += " - error argument type : " + argType;
                    }
                }

                Debug.LogError(errorMessage);
            }

            return false;
        }

        /// <summary>
        /// <para>デリゲートを実行します。</para>
        /// <para>実行するデリゲートの引数として、引数リストを使用します。</para>
        /// </summary>
        /// <param name="data">特定の型の引数が存在した場合のデータ</param>
        /// <returns></returns>
        public bool Execute(params object[] args)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("DelegatableObject.Execute : Can't call! The method is enable when playing!");
                return false;
            }
#endif
            if (!isCached)
            {
                isCached = true;
                FindCallback();
            }

            if (callback != null)
            {
                callback();
#if UNITY_EDITOR || JIGBOX_DEBUG
                Debug.LogWarning("DelegatableObject.Execute : The method not has arguments!");
#endif
                return true;
            }

            if (methodInfo == null)
            {
                return false;
            }

            int argsLength = parameters != null ? parameters.Length : 0;

            if (argsLength != args.Length)
            {
#if UNITY_EDITOR || JIGBOX_DEBUG
                Debug.LogWarning("DelegatableObject.Execute : The method's parameter count not equal args count!");
#endif
                // 引数とパラメータ数が合っていない場合は指定された引数は使えないので、通常方法で実行する
                return Execute();
            }

            // 引数なしの場合
            if (argsLength == 0)
            {
                if (eventDelegate != null)
                {
                    eventDelegate.Invoke(null);
                }
                else
                {
                    methodInfo.Invoke(target, null);
                }
                return true;
            }

            // 引数の型が合っていないと実行しても正しい結果が得られないので、
            // 型チェックをして、合っていない場合は、通常方法で実行する
            for (int i = 0; i < argsLength; ++i)
            {
                var parameterType = parametersInfo[i].ParameterType;
                var argsType = args[i].GetType();

                if (parameterType != argsType && !argsType.IsSubclassOf(parameterType))
                {
#if UNITY_EDITOR || JIGBOX_DEBUG
                    Debug.LogWarning("DelegatableObject.Execute : The method's parameter type not equal argument type!");
#endif
                    return Execute();
                }
            }

            // 引数ありの場合
            try
            {
                if (eventDelegate != null)
                {
                    eventDelegate.Invoke(args);
                }
                else
                {
                    methodInfo.Invoke(target, args);
                }
                return true;
            }
            catch (System.ArgumentException e)
            {
                string errorMessage = e.ToString() + "\n";
                errorMessage += "DelegatableObject.Execute : Can't calling method!\n"
                    + "Target : " + target.name
                    + "Method : " + methodInfo.Name + "\n";

                for (int i = 0; i < argsLength; ++i)
                {
                    errorMessage += "\narg" + (i + 1) + " : ";

                    Object obj = parameters[i].ReferencedObject;
                    string fieldName = parameters[i].FieldName;
                    if (obj != null && !string.IsNullOrEmpty(fieldName))
                    {
                        errorMessage += obj.GetType() + "." + parameters[i].FieldName;
                    }
                    else if (args[i] == null)
                    {
                        errorMessage += "null";
                        continue;
                    }

                    System.Type argType = args[i].GetType();
                    System.Type paramType = parametersInfo[i].ParameterType;

                    if (argType != paramType && !argType.IsSubclassOf(paramType))
                    {
                        errorMessage += " - error argument type : " + argType;
                    }
                }

                Debug.LogError(errorMessage);
            }

            return false;
        }

        /// <summary>
        /// <para>イベントが発行された際のメソッドを設定します。</para>
        /// <para>System.Actionは使用できません。</para>
        /// </summary>
        /// <param name="callback">デリゲート</param>
        public void SetEvent(Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("DelegatableObejct.SetEvent : Can't call! The method is enable when playing!");
                return;
            }
#endif

            Clear();

            isCached = true;
            this.callback = callback;
            target = callback.Target as MonoBehaviour;
            methodName = callback.Method.Name;
        }

        /// <summary>
        /// <para>イベントが発行された際のメソッドを設定します。</para>
        /// <para>指定された引数とメソッドの引数の数が不一致の場合、イベントの設定に失敗します。</para>
        /// </summary>
        /// <param name="target">イベントの発行対象</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="args">引数</param>
        public void SetEvent(Object target, string methodName, params object[] args)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("DelegatableObejct.SetEvent : Can't call! The method is enable when playing!");
                return;
            }
#endif

            Clear();
            this.target = target;
            this.methodName = methodName;
            parameters = new ArgumentParameter[args.Length];
            for (int i = 0; i < args.Length; ++i)
            {
                parameters[i] = new ArgumentParameter();
                parameters[i].Value = args[i];
            }

            if (args != null && args.Length > 0)
            {
                isCached = true;
                FindCallback();

                if (parameters == null || parameters.Length == 0)
                {
                    return;
                }
                // パラメータ数と引数の数が不一致の場合
                if (parameters.Length != args.Length)
                {
                    for (int i = 0; i < args.Length; ++i)
                    {
                        parameters[i].Value = args[i];
                    }
                }
            }
        }

        /// <summary>
        /// イベントが発行された際のメソッドを設定します。
        /// </summary>
        /// <param name="eventDelegate">デリゲート</param>
        public void SetEvent(IEventDelegate eventDelegate)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("DelegatableObejct.SetEvent : Can't call! The method is enable when playing!");
                return;
            }
#endif

            Clear();
            target = eventDelegate.Target;
            methodInfo = eventDelegate.Method;
            methodName = methodInfo.Name;
            parametersInfo = methodInfo.GetParameters();
            parameters = new ArgumentParameter[parametersInfo.Length];
            for (int i = 0; i < parametersInfo.Length; ++i)
            {
                parameters[i] = new ArgumentParameter();
                parameters[i].Type = parametersInfo[i].ParameterType;
            }
            this.eventDelegate = eventDelegate;

            // 直接設定設定が可能な形式で来ている前提なので、
            // 無駄に再検索をしないようにキャッシュ済み扱いとする
            isCached = true;
        }

#if UNITY_EDITOR

        /// <summary>
        /// パラメータを生成します。
        /// </summary>
        /// <param name="method"></param>
        public void CreateParameters(MethodInfo method)
        {
            parametersInfo = method.GetParameters();
            int parameterLength = parametersInfo.Length;

            parameters = new ArgumentParameter[parameterLength];
            for (int i = 0; i < parameterLength; ++i)
            {
                parameters[i] = new ArgumentParameter();
                parameters[i].Type = parametersInfo[i].ParameterType;
            }
            if (parameterLength > 0)
            {
                callback = null;
            }
        }

#endif

#region static methods

        /// <summary>
        /// 全てのデリゲートを実行します。
        /// </summary>
        /// <param name="list">実行するデリゲートのリスト</param>
        public static void Executes(List<DelegatableObject> list)
        {
            foreach (DelegatableObject delegatable in list)
            {
                if (delegatable == null)
                {
                    continue;
                }
                delegatable.Execute();
            }
        }

        /// <summary>
        /// 全てのデリゲートを実行します。
        /// </summary>
        /// <param name="list">実行するデリゲートのリスト</param>
        /// <param name="parameter">特定の型の引数が存在した場合のパラメータ</param>
        public static void Executes<T>(List<DelegatableObject> list, T parameter)
        {
            foreach (DelegatableObject delegatable in list)
            {
                if (delegatable == null)
                {
                    continue;
                }
                delegatable.Execute<T>(parameter);
            }
        }

        /// <summary>
        /// 全てのデリゲートを実行します。
        /// </summary>
        /// <param name="list">実行するデリゲートのリスト</param>
        /// <param name="args">引数リスト</param>
        public static void Executes(List<DelegatableObject> list, params object[] args)
        {
            foreach (DelegatableObject delegatable in list)
            {
                if (delegatable == null)
                {
                    continue;
                }
                delegatable.Execute(args);
            }
        }

        /// <summary>
        /// 型からnamespaceを除外した文字列を返します。
        /// </summary>
        /// <param name="builder">StringBuilder</param>
        /// <param name="type">型</param>
        public static void ToStringTrimNameSpace(StringBuilder builder, System.Type type)
        {
            string typeString = type.ToString();
            builder.Length = 0;
            builder.Append(typeString);
            int dotIndex = typeString.LastIndexOf(".");
            if (dotIndex != -1)
            {
                builder.Remove(0, dotIndex + 1);
            }
        }

        /// <summary>
        /// StringBuilderに設定された文字列にイベントの引数の型情報を追加します。
        /// </summary>
        /// <param name="builder">StringBuilder</param>
        /// <param name="paramters">引数情報</param>
        public static void AddMethodParameter(StringBuilder builder, ParameterInfo[] paramters)
        {
            if (paramters == null || paramters.Length == 0)
            {
                return;
            }
            builder.Append('(');
            StringBuilder typeBuilder = new StringBuilder();
            for (int i = 0; i < paramters.Length; ++i)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                ToStringTrimNameSpace(typeBuilder, paramters[i].ParameterType);
                builder.Append(typeBuilder.ToString());
            }
            builder.Append(')');
        }

#endregion

#region override methods

        /// <summary>
        /// Equalsのオーバーライドメソッドです。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Callback)
            {
                return callback == obj as Callback;
            }
            else if (obj is DelegatableObject)
            {
                DelegatableObject del = obj as DelegatableObject;
                Object target = del.target;
                string methodName = del.methodName;
                if (this.target != target || this.methodName != methodName)
                {
                    return false;
                }
                // 両方nullならどちらも引数なし
                if (parameters == null && del.parameters == null)
                {
                    return true;
                }
                // 両方nullではなく、片方がnullなら片方は引数なし
                if (parameters == null || del.parameters == null)
                {
                    return false;
                }
                if (parameters.Length != del.parameters.Length)
                {
                    return false;
                }
                for (int i = 0; i < del.parameters.Length; ++i)
                {
                    if (parameters[i].Type != del.parameters[i].Type &&
                        !parameters[i].Type.IsSubclassOf(del.parameters[i].Type) &&
                        !del.parameters[i].Type.IsSubclassOf(parameters[i].Type))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (obj is IEventDelegate)
            {
                IEventDelegate eventDelegate = obj as IEventDelegate;
                if (this.eventDelegate != null)
                {
                    return this.eventDelegate.Equals(eventDelegate);
                }
                Object target = eventDelegate.Target;
                // targetがnullの場合はラムダで登録された場合で、
                // 上の条件で判定できていない場合は、これ以降の条件では
                // 正しく判定できないのでfalseとする
                if (target == null)
                {
                    return false;
                }
                string methodName = eventDelegate.Method.Name;
                if (this.target != target || this.methodName != methodName)
                {
                    return false;
                }
                ParameterInfo[] paramtersInfo = methodInfo.GetParameters();
                // 同対象、同名、引数情報なし
                if (parameters == null && parametersInfo.Length == 0)
                {
                    return true;
                }
                if (parameters.Length != paramtersInfo.Length)
                {
                    return false;
                }
                for (int i = 0; i < parameters.Length; ++i)
                {
                    if (parameters[i].Type != parametersInfo[i].ParameterType)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// GetHashCodeのオーバライドメソッドです。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= target != null ? target.GetHashCode() : 0;
            hash ^= !string.IsNullOrEmpty(methodName) ? methodName.GetHashCode() : 0;
            return hash;
        }

        /// <summary>
        /// ToStringのオーバーライドメソッドです。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (target != null)
            {
                StringBuilder builder = new StringBuilder();
                ToStringTrimNameSpace(builder, target.GetType());

                if (!string.IsNullOrEmpty(methodName))
                {
                    builder.AppendFormat("/{0}", methodName);
                    AddMethodParameter(builder, ParametersInfo);
                    return builder.ToString();
                }
                else
                {
                    return builder.Append("/[delegate]").ToString();
                }
            }
            else if (callback != null)
            {
                return "Lambda -> " + callback.Method.DeclaringType;
            }
            else if (eventDelegate != null)
            {
                return "Lambda -> " + eventDelegate.Method.DeclaringType;
            }
            return "[delegate]";
        }

#endregion

#endregion

#region protected methods

        /// <summary>
        /// イベントの発行対象とメソッド名から対象メソッドを検索し、情報を設定します。
        /// </summary>
        protected void FindCallback()
        {
#if UNITY_EDITOR
            // Func()とFunc(int)など、引数なしと引数ありのオーバーロードメソッドでロールバックをした際に
            // 情報が更新されずにズレる場合があるので引数と引数情報に乖離があれば
            // コールバックをクリアする
            if (parameters != null
                && parametersInfo != null
                && parameters.Length != parametersInfo.Length)
            {
                callback = null;
            }
#endif

            if (callback == null
                || callback.Target as MonoBehaviour != target
                || callback.Method.Name != methodName)
            {
                if (target != null && !string.IsNullOrEmpty(methodName))
                {
                    System.Type targetType = target.GetType();

                    // メソッドがオーバーロードされて定義されている場合は、
                    // 引数の型情報も含めていないと正しくメソッドが取得できないので
                    // 引数情報がシリアライズされている場合は、型情報を生成する
                    bool typeGetFailed = false;
                    System.Type[] argumentTypes = new System.Type[parameters != null ? parameters.Length : 0];
                    System.Type emptyType = typeof(void);
                    for (int i = 0; i < argumentTypes.Length; ++i)
                    {
                        System.Type type = parameters[i].Type;
                        if (type != emptyType)
                        {
                            argumentTypes[i] = parameters[i].Type;
                        }
                        else
                        {
                            typeGetFailed = true;
                            break;
                        }
                    }

                    // 型がきちんと取得できていれば、型指定込みでメソッドを特定
                    if (typeGetFailed)
                    {
                        try
                        {
                            methodInfo = targetType.GetMethod(methodName, ReflectionBindingFlag);
                        }
                        // メソッドだけ変更してシリアライズされた情報を更新していない場合等に発生する
                        catch (AmbiguousMatchException e)
                        {
                            Debug.LogError(e);
#if UNITY_EDITOR
                            Debug.LogError("Delegate(" + targetType.ToString() + "." + methodName + ")を再設定して下さい");
#endif
                        }
                    }
                    else
                    {
                        methodInfo = targetType.GetMethod(methodName, ReflectionBindingFlag, null, argumentTypes, null);
                    }
                }
            }
            else
            {
                return;
            }

            if (methodInfo == null)
            {
                return;
            }
            if (methodInfo.ReturnType != typeof(void))
            {
                return;
            }

            parametersInfo = methodInfo.GetParameters();
            int parameterLength = parametersInfo.Length;

            // 引数なし
            if (parameterLength == 0)
            {
                callback = (Callback) System.Delegate.CreateDelegate(typeof(Callback), target, methodName);
                methodInfo = null;
                parameters = null;
                args = null;
            }
            else
            {
                callback = null;
            }

            if (parameters == null || parameters.Length != parameterLength)
            {
                parameters = new ArgumentParameter[parameterLength];
                args = null;

                for (int i = 0; i < parameterLength; ++i)
                {
                    parameters[i] = new ArgumentParameter();
                    parameters[i].Type = parametersInfo[i].ParameterType;
                }
            }
            else
            {
                for (int i = 0; i < parameterLength; ++i)
                {
                    parameters[i].Type = parametersInfo[i].ParameterType;
                }
            }
        }

        /// <summary>
        /// 記録されている情報をクリアします。
        /// </summary>
        protected void Clear()
        {
            target = null;
            methodName = null;
            isCached = false;
            callback = null;
            eventDelegate = null;
            parameters = null;
            methodInfo = null;
            parametersInfo = null;
            args = null;
        }

#endregion
    }
}
