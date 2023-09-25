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
using System.Collections;
using System.Collections.Generic;

namespace Jigbox.SceneTransition
{
    public class BaseSceneTransitionController : MonoBehaviour
    {
#region inner classes, enum, and stracts

        /// <summary>フェード状態</summary>
        protected enum State
        {
            None,
            /// <summary>他のシーンへ遷移するためのトランジション</summary>
            SceneOutTransition,
            /// <summary>他のシーンへ遷移するためのトランジションが終了した後の待機状態</summary>
            Hold,
            /// <summary>シーンが読み込まれるまでの待機状態</summary>
            WaitSceneLoad,
            /// <summary>シーンが読み込まれた後、トランジションを開始するまでの待機状態</summary>
            WaitInTransition,
            /// <summary>他のシーンから遷移するためのトランジション</summary>
            SceneInTransition,
        }

#endregion

#region properties

        /// <summary>フェードの状態</summary>
        protected State state;

        /// <summary>次に遷移するシーン名</summary>
        protected string nextSceneName;

        /// <summary>シーンの遷移処理</summary>
        protected Func<List<AsyncOperation>> loadAction;

        /// <summary>経過時間</summary>
        protected float deltatime;

        /// <summary>シーンの読込中かどうか</summary>
        public bool IsLoadingScene { get { return state == State.WaitSceneLoad; } }

        /// <summary>シーンの読み込みが行われたか</summary>
        public bool IsLoadedScene { get; protected set; }

        /// <summary>モーダルシーンから戻る際の遷移かどうか</summary>
        public bool IsBackFromModal { get; set; }

        /// <summary>シーンの読み込みが完了した際のコールバック</summary>
        protected Action onFinishLoad = null;

        /// <summary>シーンの読み込みが完了した際のコールバック</summary>
        public Action OnFinishLoad { get { return onFinishLoad; } set { onFinishLoad = value; } }

#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// <para>シーンの遷移処理を設定します。</para>
        /// <para>同期処理で遷移を行う場合、loadActionはnullを返して下さい。</para>
        /// </summary>
        /// <param name="nextSceneName">次に遷移するシーン名</param>
        /// <param name="loadAction">シーンの読み込み処理</param>
        public void SetLoadAction(string nextSceneName, Func<List<AsyncOperation>> loadAction)
        {
            this.nextSceneName = nextSceneName;
            this.loadAction = loadAction;
        }

        /// <summary>
        /// 遷移を開始します。
        /// </summary>
        public virtual void Begin()
        {
            state = State.SceneOutTransition;
        }

        /// <summary>
        /// 遷移処理を削除します。
        /// </summary>
        public void DeleteLoadAction()
        {
            loadAction = null;
        }

        /// <summary>
        /// 遷移処理を上書きします。
        /// </summary>
        /// <param name="sceneName">遷移するシーン名</param>
        /// <param name="loadAction">遷移処理</param>
        public void OverrideLoadAction(string sceneName, Func<List<AsyncOperation>> loadAction)
        {
            // すでにシーンの読み込みが実行されている場合は上書き不可
            if (!IsLoadedScene)
            {
                nextSceneName = sceneName;
                this.loadAction = loadAction;
            }
        }

#endregion

#region protected methods
        
        protected virtual void OnAwake()
        {
            state = State.None;
            loadAction = null;
            deltatime = 0.0f;
            IsLoadedScene = false;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnUpdate()
        {
            switch (state)
            {
                case State.SceneOutTransition:
                    SceneOutTransition();
                    break;
                case State.Hold:
                    Hold();
                    break;
                case State.WaitInTransition:
                    WaitInTransition();
                    break;
                case State.SceneInTransition:
                    SceneInTransition();
                    break;
                default:
                    break;
            }
        }

        protected virtual void OnDisabled()
        {
            SceneTransitionManager.Instance.DeleteTransition();
        }

        /// <summary>
        /// シーンの読み込みが完了した際に呼び出されます。
        /// </summary>
        protected virtual void OnFinishSceneLoad()
        {
            state = State.WaitInTransition;
        }

        /// <summary>
        /// 他のシーンへ遷移するためのトランジションを行います。
        /// </summary>
        protected virtual void SceneOutTransition()
        {
            NoticeMessageFinishOutTransition();
            state = State.Hold;
        }

        /// <summary>
        /// 他のシーンへ遷移するためのトランジションが終了したを維持します。
        /// </summary>
        protected virtual void Hold()
        {
            if (SceneTransitionManager.Instance.IsReadyLoadScene)
            {
                LoadScene();
            }
        }

        /// <summary>
        /// 他のシーンから遷移するためのトランジションの開始待ちを行います。
        /// </summary>
        protected virtual void WaitInTransition()
        {
            state = State.SceneInTransition;
        }

        /// <summary>
        /// 他のシーンから遷移するためのトランジションを行います。
        /// </summary>
        protected virtual void SceneInTransition()
        {
            if (SceneTransitionManager.Instance.IsReadyInTransition)
            {
                NoticeMessageFinishInTransition();
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// シーンを読み込みます。
        /// </summary>
        protected virtual void LoadScene()
        {
            state = State.WaitSceneLoad;

            if (loadAction != null)
            {
                List<AsyncOperation> asyncOperations = loadAction();
                // 非同期読み込みの場合は読み込みが完了するまで待つ
                if (asyncOperations != null)
                {
                    int priority = 1;

                    // 読み込まれる順番を制御するために優先度を設定する
                    // 読み込み順が狂うとSceneManager側での管理が狂う可能性があるため
                    foreach (AsyncOperation operation in asyncOperations)
                    {
                        operation.priority = priority;
                        ++priority;
                    }

                    StartCoroutine(WaitingSceneLoad(asyncOperations));
                }
                else
                {
                    InvokeFinishSceneLoadCallback();
                    OnFinishSceneLoad();
                }
                loadAction = null;
                IsLoadedScene = true;
            }
            else
            {
                InvokeFinishSceneLoadCallback();
                OnFinishSceneLoad();
                IsLoadedScene = true;
            }
        }

        /// <summary>
        /// シーンの読み込みが完了した際のコールバックを発火させます。
        /// </summary>
        protected virtual void InvokeFinishSceneLoadCallback()
        {
            if (onFinishLoad != null)
            {
                onFinishLoad();
                onFinishLoad = null;
            }
        }

        /// <summary>
        /// 他のシーンへ遷移するためのトランジションが終了したことを全てのSceneControllerに通知します。
        /// </summary>
        protected virtual void NoticeMessageFinishOutTransition()
        {
            // アクティブ状態のシーンの制御コンポーネントに対して、
            // 他のシーンへ遷移するためのトランジションが終了したメッセージを送る
            List<BaseSceneController> controllers = SceneManager.Instance.GetSceneControllers();
            foreach (BaseSceneController controller in controllers)
            {
                if (controller.gameObject.activeSelf)
                {
                    controller.SendMessage(BaseSceneController.CallbackFinishSceneOutTransition, nextSceneName);
                }
            }
        }

        /// <summary>
        /// 他のシーンから遷移するためのトランジションが終了したことを全てのSceneControllerに通知します。
        /// </summary>
        protected virtual void NoticeMessageFinishInTransition()
        {
            // アクティブ状態のシーンの制御コンポーネントに対して、
            // 他のシーンから遷移するためのトランジションが終了したメッセージを送る
            List<BaseSceneController> controllers = SceneManager.Instance.GetSceneControllers();

            // モーダルシーンから戻る場合のみ、メッセージを変更
            string message = IsBackFromModal
                ? BaseSceneController.CallbackFinishResumeTransitionModal
                : BaseSceneController.CallbackFinishSceneInTransition;

            foreach (BaseSceneController controller in controllers)
            {
                if (controller.gameObject.activeSelf)
                {
                    controller.SendMessage(message);
                }
            }
        }

        /// <summary>
        /// シーンが読み込まれるのを待ちます。
        /// </summary>
        /// <param name="asyncOperations">シーンの非同期読み込み状況</param>
        /// <returns></returns>
        protected virtual IEnumerator WaitingSceneLoad(List<AsyncOperation> asyncOperations)
        {
            bool isWaitLoad = true;

            while (isWaitLoad)
            {
                // 読み込まれる全てのシーンの読み込みが終了した段階で読み込み完了とする
                bool isComplete = true;
                foreach (AsyncOperation operation in asyncOperations)
                {
                    isComplete &= operation.isDone;
                }

                if (isComplete)
                {
                    isWaitLoad = false;
                }
                else
                {
                    yield return null;
                }
            }

            InvokeFinishSceneLoadCallback();
            OnFinishSceneLoad();
        }

#endregion

#region override unity methods

        void Awake()
        {
            OnAwake();
        }

        void Start()
        {
            OnStart();
        }

        void Update()
        {
            OnUpdate();
        }

        void OnDisable()
        {
            OnDisabled();
        }

#endregion
    }
}
