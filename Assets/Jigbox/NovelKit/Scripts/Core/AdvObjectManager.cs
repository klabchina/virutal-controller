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
    using ObjectType = AdvObjectBase.ObjectType;

    public class AdvObjectManager
    {
#region properties

        /// <summary>スクリプトから生成されたオブジェクト</summary>
        protected Dictionary<ObjectType, List<AdvObjectBase>> objects = new Dictionary<ObjectType, List<AdvObjectBase>>();

        /// <summary>エンジンの管理外のオブジェクト</summary>
        protected Dictionary<string, IAdvReferencedObject> referencedObjects = new Dictionary<string, IAdvReferencedObject>();

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AdvObjectManager()
        {
            objects.Add(ObjectType.Character, new List<AdvObjectBase>());
            objects.Add(ObjectType.Sprite, new List<AdvObjectBase>());
            objects.Add(ObjectType.Bg, new List<AdvObjectBase>());
            objects.Add(ObjectType.Cg, new List<AdvObjectBase>());
            objects.Add(ObjectType.Emotional, new List<AdvObjectBase>());
            objects.Add(ObjectType.Effect, new List<AdvObjectBase>());
            objects.Add(ObjectType.Other, new List<AdvObjectBase>());
            objects.Add(ObjectType.ScreenEffection, new List<AdvObjectBase>());
        }

        /// <summary>
        /// 全ての参照をクリアし、記録されたGameObjectを破棄します。
        /// </summary>
        public virtual void ReleaseAll()
        {
            foreach (List<AdvObjectBase> list in objects.Values)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (!list[i].IsExternal)
                    {
                        Object.Destroy(list[i].gameObject);
                    }
                }
                list.Clear();
            }
            referencedObjects.Clear();
        }

        /// <summary>
        /// 記録されているオブジェクトから該当IDのオブジェクトを返します。
        /// </summary>
        /// <param name="type">取得したいオブジェクトの種類</param>
        /// <param name="id">取得したいオブジェクトのID</param>
        /// <returns>該当するオブジェクトが存在しなかった場合、nullを返します。</returns>
        public virtual AdvObjectBase GetById(ObjectType type, int id)
        {
            List<AdvObjectBase> list = objects[type];

            foreach (AdvObjectBase obj in list)
            {
                if (obj.Id == id)
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// 指定されたレベルのプレーンに存在する特定の種類のオブジェクトを全て取得します。
        /// </summary>
        /// <param name="type">取得したいオブジェクトの種類</param>
        /// <param name="level"></param>
        /// <returns></returns>
        public virtual List<AdvObjectBase> GetByPlaneLevel(ObjectType type, int level)
        {
            List<AdvObjectBase> list = objects[type];
            List<AdvObjectBase> targets = new List<AdvObjectBase>();
            
            foreach (AdvObjectBase obj in list)
            {
                if (obj.Plane != null && obj.Plane.Level == level)
                {
                    targets.Add(obj);
                }
            }

            return targets;
        }

        /// <summary>
        /// 指定されたレベルのプレーンに存在するオブジェクトを全て取得します。
        /// </summary>
        /// <param name="level">プレーンのレベル</param>
        /// <returns></returns>
        public virtual List<AdvObjectBase> GetByPlaneLevel(int level)
        {
            List<AdvObjectBase> targets = new List<AdvObjectBase>();

            foreach (List<AdvObjectBase> list in objects.Values)
            {
                foreach (AdvObjectBase obj in list)
                {
                    if (obj.Plane != null && obj.Plane.Level == level)
                    {
                        targets.Add(obj);
                    }
                }
            }

            return targets;
        }

        /// <summary>
        /// 特定の種類のオブジェクトを全て取得します。
        /// </summary>
        /// <param name="type">取得したいオブジェクトの種類</param>
        /// <returns></returns>
        public virtual List<AdvObjectBase> GetByType(ObjectType type)
        {
            return new List<AdvObjectBase>(objects[type]);
        }

        /// <summary>
        /// 特定の種類のオブジェクトの数を取得します。
        /// </summary>
        /// <param name="type">取得したいオブジェクトの種類</param>
        /// <returns></returns>
        public int GetCount(ObjectType type)
        {
            return objects[type].Count;
        }

        /// <summary>
        /// <para>オブジェクトを登録します。</para>
        /// <para>登録するオブジェクトは、すでに登録されているオブジェクトとIDが重複していないことが前提となります。</para>
        /// <para>登録したオブジェクトをClearAllで破棄しないようにする場合は、isExternalをtrueにしてください。</para>
        /// </summary>
        /// <param name="obj">登録するオブジェクト</param>
        /// <param name="isExternal">外部のオブジェクトかどうか</param>
        public virtual void RegisterObject(AdvObjectBase obj, bool isExternal = false)
        {
            if (isExternal)
            {
                obj.SetExternal();
            }

            objects[obj.Type].Add(obj);
        }

        /// <summary>
        /// オブジェクトの登録を解除します。
        /// </summary>
        /// <param name="obj">登録を解除したいオブジェクト</param>
        public virtual void UnregisterObject(AdvObjectBase obj)
        {
            if (obj == null)
            {
                return;
            }

            objects[obj.Type].Remove(obj);
        }

        /// <summary>
        /// オブジェクトの登録を解除します。
        /// </summary>
        /// <param name="type">オブジェクトの種類</param>
        /// <param name="id">登録を解除したいオブジェクト</param>
        public virtual void UnregisterObject(ObjectType type, int id)
        {
            UnregisterObject(GetById(type, id));
        }

        /// <summary>
        /// エンジンの管理外のオブジェクトを登録します。
        /// </summary>
        /// <param name="name">登録するオブジェクト名</param>
        /// <param name="target">対象の参照</param>
        public void RegisterReferencedObject(string name, IAdvReferencedObject target)
        {
            if (referencedObjects.ContainsKey(name))
            {
                referencedObjects[name] = target;
            }
            else
            {
                referencedObjects.Add(name, target);
            }
        }

        /// <summary>
        /// エンジンの管理外のオブジェクトの登録を解除します。
        /// </summary>
        /// <param name="name">登録してあるオブジェクト名</param>
        public void UnregisterReferencedObject(string name)
        {
            if (referencedObjects.ContainsKey(name))
            {
                referencedObjects.Remove(name);
            }
        }

        /// <summary>
        /// 登録されているエンジンの管理外オブジェクトを取得します。
        /// </summary>
        /// <param name="name">オブジェクト名</param>
        /// <returns></returns>
        public IAdvReferencedObject GetReferencedObject(string name)
        {
            if (referencedObjects.ContainsKey(name))
            {
                return referencedObjects[name];
            }
            return null;
        }

        /// <summary>
        /// 登録されているエンジンの管理外オブジェクトを全て取得します。
        /// </summary>
        /// <returns></returns>
        public List<IAdvReferencedObject> GetAllReferencedObject()
        {
            return new List<IAdvReferencedObject>(referencedObjects.Values);
        }

#endregion
    }
}
