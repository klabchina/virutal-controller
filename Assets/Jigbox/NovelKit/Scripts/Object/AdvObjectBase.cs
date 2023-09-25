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

namespace Jigbox.NovelKit
{
    public abstract class AdvObjectBase : MonoBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>オブジェクトの種類</summary>
        public enum ObjectType
        {
            /// <summary>キャラクター</summary>
            Character,
            /// <summary>キャラクター以外の画像</summary>
            Sprite,
            /// <summary>背景</summary>
            Bg,
            /// <summary>CG(一枚絵)</summary>
            Cg,
            /// <summary>感情表現系エモーション</summary>
            Emotional,
            /// <summary>演出</summary>
            Effect,
            /// <summary>その他</summary>
            Other,
            /// <summary>画面効果</summary>
            ScreenEffection,
        }

#endregion
        
#region properties

        /// <summary>管理用ID(オブジェクトの種類毎)</summary>
        public int Id { get; protected set; }

        /// <summary>オブジェクトの種類</summary>
        public ObjectType Type { get; protected set; }
        
        /// <summary>表示座標</summary>
        public Vector3 Position
        {
            get
            {
                return LocalTransform.localPosition;
            }
            set
            {
                LocalTransform.localPosition = value;
            }
        }
        
        /// <summary>回転状態のキャッシュ</summary>
        protected Vector3 localRotation = Vector3.zero;

        /// <summary>回転状態</summary>
        public Vector3 Rotation
        {
            get
            {
                return localRotation;
            }
            set
            {
                localRotation = value;
                LocalTransform.localEulerAngles = localRotation;
            }
        }

        /// <summary>拡大縮小状態</summary>
        public Vector3 Scale
        {
            get
            {
                return LocalTransform.localScale;
            }
            set
            {
                LocalTransform.localScale = value;
            }
        }
        
        /// <summary>Transform</summary>
        protected Transform localTransform;

        /// <summary>Transform</summary>
        public Transform LocalTransform
        {
            get
            {
                if (localTransform == null)
                {
                    localTransform = transform;
                }
                return localTransform;
            }
        }
        
        /// <summary>自分が所属するプレーン(サブオブジェクトの場合、null)</summary>
        public AdvPlaneController Plane { get; set; }

        /// <summary>Material</summary>
        public abstract Material Material { get; set; }

        /// <summary>差分用オブジェクトの参照</summary>
        Dictionary<int, AdvObjectBase> subObjects;

        protected Dictionary<int, AdvObjectBase> SubObjects
        {
            get
            {
                if (subObjects == null)
                {
                    subObjects = new Dictionary<int, AdvObjectBase>();
                }
                return subObjects;
            }
        }

        /// <summary>外部のオブジェクトかどうか</summary>
        internal bool IsExternal { get; private set; }

#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="id">管理用ID(オブジェクトの種類毎)</param>
        /// <param name="type">オブジェクトの種類</param>
        /// <param name="setting">オブジェクトの基礎設定</param>
        public virtual void Init(int id, ObjectType type, AdvObjectSetting.ObjectSetting setting)
        {
            Id = id;
            Type = type;
        }

        /// <summary>
        /// オブジェクトを表示状態にします。
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// オブジェクトを非表示状態にします。
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        public virtual void Delete()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// リソースを読み込みます。
        /// </summary>
        /// <param name="loader">Loader</param>
        /// <param name="resourcePath">リソースのパス</param>
        public abstract void LoadResource(IAdvResourceLoader loader, string resourcePath);

        /// <summary>
        /// メッセージを受け取ります。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public virtual void ReceiveMessage(string message)
        {
        }

        /// <summary>
        /// 差分用オブジェクトを登録します。
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="obj">差分用オブジェクト</param>
        /// <returns></returns>
        public virtual bool RegisterSubObject(int id, AdvObjectBase obj)
        {
            Dictionary<int, AdvObjectBase> subObjects = SubObjects;
            if (!subObjects.ContainsKey(id))
            {
                subObjects.Add(id, obj);
                obj.transform.SetParent(transform, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 差分用オブジェクトの登録を解除します。
        /// </summary>
        /// <param name="id">ID</param>
        public virtual void UnregisterSubObject(int id)
        {
            Dictionary<int, AdvObjectBase> subObjects = SubObjects;
            if (subObjects.ContainsKey(id))
            {
                subObjects.Remove(id);
            }
        }

        /// <summary>
        /// 差分用オブジェクトを取得します。
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public virtual AdvObjectBase GetSubObject(int id)
        {
            Dictionary<int, AdvObjectBase> subObjects = SubObjects;
            if (subObjects.ContainsKey(id))
            {
                return subObjects[id];
            }
            return null;
        }

        /// <summary>
        /// 外部のオブジェクトとして扱います。
        /// </summary>
        internal void SetExternal()
        {
            // これに関しては完全にエンジン内部の制御的な話なので、
            // 外部に公開する予定がないのでinternalで定義している
            IsExternal = true;
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            Id = -1;
            Type = ObjectType.Other;
        }

#endregion
    }
}
