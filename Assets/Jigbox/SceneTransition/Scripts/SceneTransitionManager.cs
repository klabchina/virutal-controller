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
using System;
using System.Collections.Generic;

namespace Jigbox.SceneTransition
{
    public class SceneTransitionManager
    {
#region properties

        /// <summary>インスタンス</summary>
        static SceneTransitionManager instance;

        /// <summary>インスタンス</summary>
        public static SceneTransitionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SceneTransitionManager();
                }
                return instance;
            }
        }

        /// <summary>フェード用オブジェクトのインスタンス</summary>
        protected BaseSceneTransitionController transitionController;
        
        /// <summary>シーンの読み込みを行ってもいいかどうか</summary>
        public bool IsReadyLoadScene { get; set; }

        /// <summary>シーンの読み込み後、トランジションを行ってもいいかどうか</summary>
        public bool IsReadyInTransition { get; set; }

        /// <summary>シーン遷移時のトランジションが行われているかどうか</summary>
        public bool IsTransition
        {
            get
            {
                return (transitionController != null);
            }
        }

        /// <summary>シーンの読込中かどうか</summary>
        public bool IsLoadingScene
        {
            get
            {
                if (transitionController == null)
                {
                    return false;
                }
                else
                {
                    return transitionController.IsLoadingScene;
                }
            }
        }

        /// <summary>すでにシーンの切り替えが行われたかどうか</summary>
        public bool IsAlreadyLoadScene
        {
            get
            {
                if (transitionController == null)
                {
                    return false;
                }
                else
                {
                    return transitionController.IsLoadedScene;
                }
            }
        }

        /// <summary>デフォルトで使用するシーン遷移時のトランジションのプレハブのパス</summary>
        public string DefaultTransitionPrefabPath { get; set; }

#endregion

#region public methods

        /// <summary>
        /// シーン遷移時のトランジションを開始します。
        /// </summary>
        /// <param name="controller">シーン遷移時のトランジションの制御コンポーネント</param>
        public void Begin(BaseSceneTransitionController controller)
        {
            if (controller == null)
            {
                return;
            }

            // すでにトランジションが行われている(シーンの読込中である)
            if (transitionController != null)
            {
                return;
            }

            IsReadyLoadScene = false;
            IsReadyInTransition = false;

            transitionController = controller;
            transitionController.Begin();
        }

        /// <summary>
        /// <para>シーン遷移時のトランジション用オブジェクトを生成します。</para>
        /// <para>resourcePathを指定しなかった場合、設定されているデフォルトのパスが適用されます。</para>
        /// </summary>
        /// <param name="prefabPath">トランジション用オブジェクトのプレハブのパス</param>
        /// <returns></returns>
        public BaseSceneTransitionController CreateTransition(string prefabPath = null)
        {
            BaseSceneTransitionController transition = null;
            // 空文字列の場合はデフォルト設定されているトランジションのプレハブのパスを設定
            if (string.IsNullOrEmpty(prefabPath))
            {
                prefabPath = DefaultTransitionPrefabPath;
            }

            // デフォルトが設定されていない可能性もあるため、再度チェック
            if (!string.IsNullOrEmpty(prefabPath))
            {
                UnityEngine.Object resource = Resources.Load(prefabPath);
                if (resource != null)
                {
                    GameObject obj = UnityEngine.Object.Instantiate(resource) as GameObject;
                    transition = obj.GetComponent<BaseSceneTransitionController>();
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogError("SceneTransitionManager.CreateTransition : Can't load transition resource!");
                }
#endif

            }
            
            if (transition == null)
            {
                GameObject obj = new GameObject("DefaultTransitionObject");
                return obj.AddComponent<BaseSceneTransitionController>();
            }

            transition.Init();
            return transition;
        }

        /// <summary>
        /// <para>トランジションを状態に関わらず参照を破棄します。</para>
        /// <para>基本的にBaseSceneTransitionControllerから自動で呼び出されます。</para>
        /// </summary>
        public void DeleteTransition()
        {
            if (transitionController != null)
            {
                transitionController = null;
            }
        }

        /// <summary>
        /// トランジションに登録されたシーンの読み込み処理を削除します。
        /// </summary>
        public void DeleteLoadAction()
        {
            if (transitionController != null)
            {
                // すでにシーンの読み込みが実行されていた場合は無効
                if (transitionController.IsLoadedScene)
                {
#if UNITY_EDITOR
                    Debug.LogError("FadeManager.DeleteLoadAction : Failed delete! Scene load is already executed!");
#endif
                    return;
                }
                transitionController.DeleteLoadAction();
            }
        }

        /// <summary>
        /// シーンの読み込み処理を上書きします。
        /// </summary>
        /// <param name="sceneName">遷移するシーン名</param>
        public void OverrideLoadAction(string sceneName)
        {
            List<AsyncOperation> operations = new List<AsyncOperation>();
            operations.Add(SceneManager.Instance.NoneTransitionLoadScene(sceneName, true));
            OverrideLoadAction(sceneName, () => operations);
        }

        /// <summary>
        /// <para>シーンの読み込み処理を上書きします。</para>
        /// <para>同期処理で遷移を行う場合、loadActionはnullを返して下さい。</para>
        /// </summary>
        /// <param name="sceneName">遷移するシーン名</param>
        /// <param name="loadAction">シーンの読み込み処理</param>
        public void OverrideLoadAction(string sceneName, Func<List<AsyncOperation>> loadAction)
        {
            if (transitionController != null)
            {
                // すでにシーンの読み込みが実行されていた場合は無効
                if (transitionController.IsLoadedScene)
                {
#if UNITY_EDITOR
                    Debug.LogError("FadeManager.OverwriteLoadAction : Failed over write! Scene load is already executed!");
#endif
                    return;
                }
                transitionController.OverrideLoadAction(sceneName, loadAction);
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected SceneTransitionManager()
        {
        }

#endregion
    }
}
