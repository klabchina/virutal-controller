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

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public abstract class ButtonSoundBase : MonoBehaviour, IButtonExtendComponent
    {
#region inner classes, enum, and structs

        [System.Serializable]
        public class SoundEntry
        {
            /// <summary>イベントの種類</summary>
            [SerializeField]
            InputEventType type;

            /// <summary>イベントの種類</summary>
            public InputEventType Type { get { return type; } }

            /// <summary>定義フィールドのパス</summary>
            [SerializeField]
            protected string fieldPath;
            
            /// <summary>定義フィールドのパス</summary>
            public string FieldPath { get { return fieldPath; } }

            /// <summary>イベント名</summary>
            [System.NonSerialized]
            protected string eventName;

            /// <summary>イベント名</summary>
            public string EventName
            {
                get
                {
                    if (!string.IsNullOrEmpty(eventName))
                    {
                        return eventName;
                    }

                    if (isCached)
                    {
                        return string.Empty;
                    }

                    if (!string.IsNullOrEmpty(fieldPath))
                    {
                        isCached = true;
                        int index = fieldPath.LastIndexOf("/");
                        if (index == -1)
                        {
                            return string.Empty;
                        }

                        // クラスとフィールドを分離
                        // クラス用の文字列はエディタ編集用から実行用へ変換
                        string targetClass = fieldPath.Substring(0, index).Replace('/', '.');
                        string fieldName = fieldPath.Substring(index + 1);
                        System.Type type = Assembly.GetType(targetClass);
                        if (type != null)
                        {
                            FieldInfo info = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
                            if (info != null && info.FieldType == typeof(string))
                            {
                                eventName = info.GetValue(null) as string;
                                return eventName;
                            }
                        }
                    }

                    return string.Empty;
                }
            }

            /// <summary>イベント名がキャッシュされているかどうか</summary>
            [System.NonSerialized]
            protected bool isCached = false;

            /// <summary>アセンブリ</summary>
            public Assembly Assembly { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="type">イベントの種類</param>
            public SoundEntry(InputEventType type)
            {
                this.type = type;
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="type">イベントの種類</param>
            /// <param name="fieldPath">クラスのパス</param>
            public SoundEntry(InputEventType type, string fieldPath)
            {
                this.type = type;
                this.fieldPath = fieldPath;
            }
        }

#endregion

#region properties

        /// <summary>サウンドの定義クラス</summary>
        public abstract System.Type SoundDefinitionClass { get; }
        
        /// <summary>サウンドの定義アセンブリ</summary>
        public virtual Assembly SoundDefinitionAssembly
        {
            get
            {
                return SoundDefinitionClass.Assembly;
            }
        }

        /// <summary>サウンドのエントリ情報</summary>
        [HideInInspector]
        [SerializeField]
        protected List<SoundEntry> entries = new List<SoundEntry>();

        /// <summary>サウンドのエントリ情報</summary>
        public List<SoundEntry> Entries { get { return entries; } }

#endregion

#region public methods
        
        /// <summary>
        /// ボタンのイベントの通知を受け取ります。
        /// </summary>
        /// <param name="type">発火したイベントの種類</param>
        public void NoticeEvent(InputEventType type)
        {
            foreach (SoundEntry entry in entries)
            {
                if (entry.Type == type)
                {
                    PlaySound(entry.EventName);
                }
            }
        }

        /// <summary>
        /// サウンドを再生します。
        /// </summary>
        /// <param name="eventName">イベント名</param>
        protected abstract void PlaySound(string eventName);

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            var assembly = SoundDefinitionAssembly;
            // 開始時点で全てのイベント名をキャッシュ
            foreach (SoundEntry entry in entries)
            {
                entry.Assembly = assembly;
#pragma warning disable 219
                string eventName = entry.EventName;
#pragma warning restore 219
            }
        }

#endregion
    }
}
