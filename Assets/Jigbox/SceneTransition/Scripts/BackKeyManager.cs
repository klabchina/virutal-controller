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

#if !UNITY_IOS || UNITY_EDITOR
#define ENABLE_ESCAPE
#endif

using UnityEngine;
using System.Collections.Generic;
using Jigbox.UIControl;

namespace Jigbox.SceneTransition
{
    public class BackKeyManager : MonoBehaviour
    {
#region properties

        /// <summary>インスタンス</summary>
        static BackKeyManager instance;

        /// <summary>インスタンス</summary>
        public static BackKeyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("BackKeyManager");
                    instance = obj.AddComponent<BackKeyManager>();
                    obj.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                    DontDestroyOnLoad(obj);
                }
                return instance;
            }
        }

#if ENABLE_ESCAPE

        /// <summary>エスケープキー押下の通知対象</summary>
        List<IBackKeyNoticeTarget> noticeTarget = new List<IBackKeyNoticeTarget>();

#endif

#endregion

#region public methods

        /// <summary>
        /// <para>エスケープキー(Androidのバックキー)押下の通知対象を記録します。</para>
        /// <para>通知対象が設定されている間、シーンへの通知は行われなくなります。</para>
        /// </summary>
        /// <param name="target">通知対象となるコンポーネント</param>
        public void RegisterNoticeTarget(IBackKeyNoticeTarget target)
        {
#if ENABLE_ESCAPE
            if (!noticeTarget.Contains(target))
            {
                noticeTarget.Add(target);
            }
#endif
        }

        /// <summary>
        /// エスケープキー(Androidのバックキー)押下の通知対象の記録を解除します。
        /// </summary>
        /// <param name="target">記録していたコンポーネント</param>
        public void UnregisterNoticeTarget(IBackKeyNoticeTarget target)
        {
#if ENABLE_ESCAPE
            if (noticeTarget.Contains(target))
            {
                noticeTarget.Remove(target);
            }
#endif
        }

        /// <summary>
        /// エスケープキー(Androidのバックキー)押下の通知対象をクリアします。
        /// </summary>
        public void ClearNoticeTarget()
        {
#if ENABLE_ESCAPE
            if (noticeTarget.Count > 0)
            {
                noticeTarget.Clear();
            }
#endif
        }

        /// <summary>
        /// 現在記録されているエスケープキー(Androidのバックキー)の通知対象の優先度の最大値を返します。
        /// </summary>
        /// <param name="enableOnly">通知が有効な対象のみを返すかどうか</param>
        /// <returns>通知対象が存在しない場合、<c>int.MinValue</c>を返します。</returns>
        public int GetPriorityMax(bool enableOnly = true)
        {
            
#if ENABLE_ESCAPE
            int priorityMax = int.MinValue;
            foreach (IBackKeyNoticeTarget target in noticeTarget)
            {
                if ((!enableOnly || target.Enable) && target.Priority > priorityMax)
                {
                    priorityMax = target.Priority;
                }
            }

            return priorityMax;
#else
            return int.MinValue;
#endif
        }
        
#endregion

#region protected methods

#if ENABLE_ESCAPE

        /// <summary>
        /// エスケープキー(Androidのバックキー)が押された事による処理を実行します。
        /// </summary>
        protected void InvokeEscape()
        {
            // 通知対象が設定されている状態ではシーンへは通知しない
            if (noticeTarget.Count > 0)
            {
                int priorityMax = GetPriorityMax();

                // 通知対象を選別
                List<IBackKeyNoticeTarget> targets = new List<IBackKeyNoticeTarget>();
                foreach (IBackKeyNoticeTarget target in noticeTarget)
                {
                    if (target.Enable && target.Priority == priorityMax)
                    {
                        targets.Add(target);
                    }
                }

                if (targets.Count > 0)
                {
                    foreach (IBackKeyNoticeTarget target in targets)
                    {
                        target.OnEscape();
                    }
                    return;
                }
            }
            // シーンの遷移処理中はシーンへの通知は行わない
            if (SceneTransitionManager.Instance.IsTransition)
            {
                return;
            }

            NoticeMessageToScenes();
        }

        /// <summary>
        /// シーンに対して、エスケープキー(Androidのバックキー)が押された事を通知します。
        /// </summary>
        protected void NoticeMessageToScenes()
        {
            string modalSceneName = SceneManager.Instance.CurrentModalSceneName;
            // モーダルシーンが存在しない場合はアクティブなシーンに通知
            if (string.IsNullOrEmpty(modalSceneName))
            {
                foreach (BaseSceneController sceneController in SceneManager.Instance.GetSceneControllers())
                {
                    if (sceneController.gameObject.activeInHierarchy)
                    {
                        sceneController.SendMessage(BaseSceneController.CallbackEscape);
                    }
                }
            }
            // モーダルシーンが存在する場合はモーダルシーンに対して通知
            else
            {
                BaseSceneController sceneController = SceneManager.Instance.GetSceneController(modalSceneName);
                sceneController.SendMessage(BaseSceneController.CallBackEscapeOnModal);
            }
        }

#endif

#endregion

#region override unity methods

        void Update()
        {
#if ENABLE_ESCAPE
            // 他の入力によるロックが発生している間はエスケープキーを実行しない
            if (SubmitMediator.Instance.LockCount > 0)
            {
                return;
            }
            
            if (InputWrapper.GetKeyDown(KeyCode.Escape))
            {
                InvokeEscape();
            }
#endif
        }

#endregion
    }
}
