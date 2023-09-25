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

namespace Jigbox.Components
{
    /// <summary>
    /// 偽装入力の対象となるオブジェクトの管理モジュール
    /// </summary>
    public sealed class ImitateInputTargetManager : MonoBehaviour
    {
#region properties

        /// <summary>インスタンス</summary>
        static ImitateInputTargetManager instance = null;

        /// <summary>インスタンス</summary>
        public static ImitateInputTargetManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("ImitateInputTargetManager");
                    instance = gameObject.AddComponent<ImitateInputTargetManager>();
                    DontDestroyOnLoad(gameObject);
#if JIGBOX_DEBUG
                    gameObject.hideFlags = HideFlags.DontSave;
#else
                    gameObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
#endif
                }
                return instance;
            }
        }

        /// <summary>
        /// <para>偽装入力による入力対象となるオブジェクト</para>
        /// <para>※シリアライズは確認のため</para>
        /// </summary>
        [SerializeField]
        Dictionary<GameObject, ImitateInputTarget> targets = new Dictionary<GameObject, ImitateInputTarget>();

#endregion

#region public methods

        /// <summary>
        /// ImitateInputTargetの参照を記録します。
        /// </summary>
        /// <param name="target">ImitateInputTarget</param>
        public static void Register(ImitateInputTarget target)
        {
            if (!Instance.targets.ContainsKey(target.gameObject))
            {
                Instance.targets.Add(target.gameObject, target);
            }
        }

        /// <summary>
        /// 記録されているImitateInputTargetの参照を解除します。
        /// </summary>
        /// <param name="target">ImitateInputTarget</param>
        public static void Unregister(ImitateInputTarget target)
        {
            if (instance == null)
            {
                return;
            }

            if (Instance.targets.ContainsKey(target.gameObject))
            {
                Instance.targets.Remove(target.gameObject);
            }
        }

        /// <summary>
        /// 入力のバリデーションの状態を切り替えます。
        /// </summary>
        /// <param name="throughValidate">バリデーションをスルーするかどうか</param>
        public void ChangeValidate(bool throughValidate)
        {
            foreach (var target in targets)
            {
                target.Value.ThroughValidateFromInputModule = throughValidate;
            }
        }

        /// <summary>
        /// 対象GameObjectにImitateInputTargetがアタッチされているかを返します。
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public bool IsValidTarget(GameObject gameObject)
        {
            return targets.ContainsKey(gameObject);
        }

#endregion

#region override unity methods

        void OnDestroy()
        {
            instance = null;
        }

#endregion
    }
}
