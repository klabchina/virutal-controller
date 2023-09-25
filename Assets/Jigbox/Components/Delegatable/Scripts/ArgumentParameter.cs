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
using System.Reflection;
using System.Text;

namespace Jigbox.Delegatable
{
    [System.Serializable]
    public class ArgumentParameter
    {
#region properties

        /// <summary>参照されるオブジェクト</summary>
        [SerializeField]
        Object referencedObject;

        /// <summary>参照されるオブジェクト</summary>
        public Object ReferencedObject
        {
            get
            {
                return referencedObject;
            }
            set
            {
                if (referencedObject != value)
                {
                    referencedObject = value;
                }
            }
        }

        /// <summary>フィールド名</summary>
        [SerializeField]
        string fieldName;

        /// <summary>フィールド名</summary>
        public string FieldName
        {
            get
            {
                return fieldName;
            }
            set
            {
                if (fieldName != value)
                {
                    fieldName = value;
                    type = null;
                    typeName = string.Empty;
                }
            }
        }

        /// <summary>型名</summary>
        [SerializeField]
        string typeName;

        /// <summary>フィールドの値</summary>
        [System.NonSerialized]
        object value;

        /// <summary>フィールドの値</summary>
        public object Value
        {
            get
            {
                // 値が直接設定されている場合は、それを使用する
                if (value != null)
                {
                    return value;
                }

                // 検索を毎度行うと処理コストが増えるので、最初だけ情報を取得する
                if (!isCached)
                {
                    isCached = true;
                    propertyInfo = null;
                    fieldInfo = null;

                    if (referencedObject != null && !string.IsNullOrEmpty(fieldName))
                    {
                        System.Type objectType = referencedObject.GetType();

                        // 対象がプロパティになるか、フィールドになるかは区別していないので、
                        // 取得できた方を利用する
                        propertyInfo = objectType.GetProperty(fieldName, DelegatableObject.ReflectionBindingFlag);
                        if (propertyInfo == null)
                        {
                            fieldInfo = objectType.GetField(fieldName, DelegatableObject.ReflectionBindingFlag);
                        }
                    }
                }
                if (propertyInfo != null)
                {
                    return propertyInfo.GetValue(referencedObject, null);
                }
                if (fieldInfo != null)
                {
                    return fieldInfo.GetValue(referencedObject);
                }
                if (referencedObject != null)
                {
                    return referencedObject;
                }

                return null;
            }
            set
            {
                this.value = value;
            }
        }

        [System.NonSerialized]
        System.Type type;

        /// <summary>型</summary>
        public System.Type Type
        {
            get
            {
                if (type != null)
                {
                    return type;
                }

                // 型自体をシリアライズすることはできないので、
                // シリアライズされた型名から型を取得する
                if (!string.IsNullOrEmpty(typeName))
                {
                    type = System.Type.GetType(typeName);
                    if (type != null)
                    {
                        return type;
                    }
                }

                if (value != null)
                {
                    type = value.GetType();
                }
                else
                {
                    object tempValue = Value;
                    if (tempValue != null)
                    {
                        type = tempValue.GetType();
                    }
                }

                // 型名がシリアライズされていない状態で
                // 値から型を取得できた場合は、その型名をシリアライズする
                if (type != null)
                {
                    typeName = type.ToString();
                    return type;
                }

                return typeof(void);
            }
            set
            {
                if (type != value)
                {
                    type = value;
                    typeName = type.ToString();
                }
            }
        }

        /// <summary>参照されるオブジェクトの型のフィールド情報</summary>
        [System.NonSerialized]
        FieldInfo fieldInfo;

        /// <summary>参照されるオブジェクトの型のプロパティ情報</summary>
        [System.NonSerialized]
        PropertyInfo propertyInfo;

        /// <summary>フィールドの値がキャッシュ済みかどうか(毎回探さないため)</summary>
        [System.NonSerialized]
        bool isCached = false;

#endregion

#region public methods

        public override string ToString()
        {
            if (referencedObject != null)
            {
                StringBuilder builder = new StringBuilder();
                DelegatableObject.ToStringTrimNameSpace(builder, referencedObject.GetType());

                if (!string.IsNullOrEmpty(fieldName))
                {
                    return builder.AppendFormat("/{0}", fieldName).ToString();
                }
                else
                {
                    return builder.Append("/[field]").ToString();
                }
            }
            return "[field]";
        }

#endregion
    }
}
