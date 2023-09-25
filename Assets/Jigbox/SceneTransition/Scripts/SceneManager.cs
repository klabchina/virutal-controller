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
using System.Linq;

namespace Jigbox.SceneTransition
{
    public class SceneManager
    {
#region inner classes, enum, and structs

        protected class ModalStatus
        {
            /// <summary>シーン名</summary>
            public string SceneName { get; private set; }

            /// <summary>遷移前にアクティブだったシーン名</summary>
            public string LastActiveSceneName { get; private set; }

            /// <summary>モーダルシーンの制御コンポーネント</summary>
            public BaseSceneController SceneController { get; private set; }

            /// <summary>モーダルシーンに遷移する前のシーン</summary>
            public List<BaseSceneController> BeforeScenes { get; private set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="sceneName">シーン名</param>
            /// <param name="lastActiveSceneName">遷移前にアクティブだったシーン名</param>
            /// <param name="sceneController">シーンの制御コンポーネント</param>
            /// <param name="beforeScenes">遷移前のシーン</param>
            public ModalStatus(string sceneName, string lastActiveSceneName, BaseSceneController sceneController, List<BaseSceneController> beforeScenes)
            {
                SceneName = sceneName;
                LastActiveSceneName = lastActiveSceneName;
                SceneController = sceneController;
                BeforeScenes = beforeScenes;
            }
        }

#endregion

#region properties

        /// <summary>インスタンス</summary>
        protected static SceneManager instance;

        /// <summary>インスタンス</summary>
        public static SceneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SceneManager();
                }
                return instance;
            }
        }

        /// <summary>読み込まれているシーンの制御コンポーネント</summary>
        protected Dictionary<string, BaseSceneController> loadedSceneControllers = new Dictionary<string, BaseSceneController>();

        /// <summary>Modalシーンのスタック</summary>
        protected Stack<ModalStatus> modalSceneStack = new Stack<ModalStatus>();

        /// <summary>最後に破棄されたModalシーンの情報</summary>
        protected ModalStatus lastUnloadModalScene = null;

        /// <summary>シーン間での受け渡し用データ</summary>
        protected Dictionary<string, object> passingData = new Dictionary<string, object>();

        /// <summary>読み込まれる予定のモーダルシーン名のリスト</summary>
        protected List<string> willLoadModal = new List<string>();

        /// <summary>現在のシーン名</summary>
        protected string currentSceneName = string.Empty;

        /// <summary>現在のシーン名</summary>
        public string CurrentSceneName
        {
            get
            {
                return currentSceneName;
            }
        }
        
        /// <summary>現在開いているモーダルシーン名</summary>
        public string CurrentModalSceneName
        {
            get
            {
                return modalSceneStack.Count > 0 ? modalSceneStack.Peek().SceneName : string.Empty;
            }
        }

#endregion

#region public methods

        /// <summary>
        /// シーンを読み込みます。
        /// </summary>
        /// <param name="sceneName">読み込むシーン名</param>
        /// <param name="transitionResourcePath">シーン遷移時のトランジション用オブジェクトのプレハブのパス</param>
        /// <returns>シーンの遷移を開始できた場合、trueを返します。</returns>
        public virtual bool LoadScene(string sceneName, string transitionResourcePath = null)
        {
            if (SceneTransitionManager.Instance.IsTransition)
            {
#if UNITY_EDITOR
                Debug.Log("SceneManager.LoadScene : Can't load the scene, because already began to transitioning!");
#endif
                return false;
            }

            BaseSceneTransitionController transitionController = SceneTransitionManager.Instance.CreateTransition(transitionResourcePath);

            transitionController.SetLoadAction(sceneName, () => LoadDefault(sceneName));

            int handlerCount = 0;
            foreach (BaseSceneController sceneController in loadedSceneControllers.Values)
            {
                if (sceneController is ISceneUnloadHandler)
                {
                    ISceneUnloadHandler handler = sceneController as ISceneUnloadHandler;
                    handler.OnWillUnload(new SceneTransitionTransientController(transitionController));
                    ++handlerCount;
                }
            }

            // アンロード処理が必要なコントローラーが存在していなければ遷移を開始
            if (handlerCount == 0)
            {
                SceneTransitionManager.Instance.Begin(transitionController);
            }

#if UNITY_EDITOR
            // 2個以上アンロード処理が必要なコントローラーがいる場合、
            // 整合性がおかしくなる可能性があるので警告を表示
            if (handlerCount > 1)
            {
                Debug.LogWarning("Some controllers has unload handler!");
            }
#endif

            return true;
        }

        /// <summary>
        /// <para>現在のシーンの上にシーンを読み込みます。</para>
        /// <para>シーンが読み込まれた時点で現在アクティブ状態なシーンは全て非アクティブとなります。</para>
        /// </summary>
        /// <param name="sceneName">読み込むシーン名</param>
        /// <param name="transitionResourcePath">シーン遷移時のトランジション用オブジェクトのプレハブのパス</param>
        /// <returns>シーンの遷移を開始できた場合、trueを返します。</returns>
        public virtual bool OpenModalScene(string sceneName, string transitionResourcePath = null)
        {
            if (SceneTransitionManager.Instance.IsTransition)
            {
#if UNITY_EDITOR
                Debug.Log("SceneManager.OpenModalScene : Can't open the scene, because already began to transitioning!");
#endif
                return false;
            }

            BaseSceneTransitionController transitionController = SceneTransitionManager.Instance.CreateTransition(transitionResourcePath);

            transitionController.SetLoadAction(sceneName, () => LoadModal(sceneName));

            willLoadModal.Add(sceneName);
            SceneTransitionManager.Instance.Begin(transitionController);

            return true;
        }

        /// <summary>
        /// 現在のモーダルシーンを破棄し、モーダルシーンを開く前にあったシーンをアクティブ化します。
        /// </summary>
        /// <param name="transitionResourcePath">シーン遷移時のトランジション用オブジェクトのプレハブのパス</param>
        /// <returns>シーンの遷移を開始できた場合、trueを返します。</returns>
        public virtual bool CloseModalScene(string transitionResourcePath = null)
        {
            if (SceneTransitionManager.Instance.IsTransition)
            {
#if UNITY_EDITOR
                Debug.Log("SceneManager.CloseModalScene : Can't close the scene, because already began to transitioning!");
#endif
                return false;
            }
            if (modalSceneStack.Count == 0)
            {
                return false;
            }

            BaseSceneTransitionController transitionController = SceneTransitionManager.Instance.CreateTransition(transitionResourcePath);

            transitionController.SetLoadAction(string.Empty, UnloadModal);
            transitionController.OnFinishLoad = ActivateSceneBeforeModal;
            transitionController.IsBackFromModal = true;

            // アンロード処理が必要なコントローラーであれば処理を委譲し、
            // そうでなければ、そのまま遷移を開始する
            ModalStatus modal = modalSceneStack.Peek();
            if (modal.SceneController is ISceneUnloadHandler)
            {
                ISceneUnloadHandler handler = modal.SceneController as ISceneUnloadHandler;
                handler.OnWillUnload(new SceneTransitionTransientController(transitionController));
            }
            else
            {
                SceneTransitionManager.Instance.Begin(transitionController);
            }


            return true;
        }

        /// <summary>
        /// 全てのモーダルシーンを破棄し、モーダルシーンを開く前にあったシーンをアクティブ化します。
        /// </summary>
        /// <param name="transitionResourcePath">シーン遷移時のトランジション用オブジェクトのプレハブのパス</param>
        /// <returns>シーンの遷移を開始できた場合、trueを返します。</returns>
        public virtual bool CloseModalSceneAll(string transitionResourcePath = null)
        {
            if (SceneTransitionManager.Instance.IsTransition)
            {
#if UNITY_EDITOR
                Debug.Log("SceneManager.CloseModalScene : Can't close modal scenes, because already began to transitioning!");
#endif
                return false;
            }
            if (modalSceneStack.Count == 0)
            {
                return false;
            }

            BaseSceneTransitionController transitionController = SceneTransitionManager.Instance.CreateTransition(transitionResourcePath);

            transitionController.SetLoadAction(string.Empty, UnloadModalAll);
            transitionController.OnFinishLoad = ActivateSceneBeforeModal;
            transitionController.IsBackFromModal = true;

            int handlerCount = 0;
            foreach (ModalStatus modal in modalSceneStack)
            {
                if (modal.SceneController is ISceneUnloadHandler)
                {
                    ISceneUnloadHandler handler = modal.SceneController as ISceneUnloadHandler;
                    handler.OnWillUnload(new SceneTransitionTransientController(transitionController));
                    ++handlerCount;
                }
            }

            // アンロード処理が必要なコントローラーが存在していなければ遷移を開始
            if (handlerCount == 0)
            {
                SceneTransitionManager.Instance.Begin(transitionController);
            }

#if UNITY_EDITOR
            // 2個以上アンロード処理が必要なコントローラーがいる場合、
            // 整合性がおかしくなる可能性があるので警告を表示
            if (handlerCount > 1)
            {
                Debug.LogWarning("Some controllers has unload handler!");
            }
#endif

            return true;
        }

        /// <summary>
        /// <para>シーンの読み込みを中止します。</para>
        /// <para>すでにシーンの読み込みが行われていた場合、中止できません。</para>
        /// </summary>
        public static void LoadCancel()
        {
            SceneTransitionManager.Instance.DeleteLoadAction();
        }

        /// <summary>
        /// シーンをトランジションを行わずに読み込みます。
        /// </summary>
        /// <param name="sceneName">読み込むシーン名</param>
        /// <param name="isAsync">非同期で読み込むかどうか</param>
        /// <returns></returns>
        public virtual AsyncOperation NoneTransitionLoadScene(string sceneName, bool isAsync = false)
        {
            return LoadScene(sceneName, isAsync);
        }

        /// <summary>
        /// シーンを追加で読み込みます。
        /// </summary>
        /// <param name="sceneName">読み込むシーン名</param>
        /// <param name="isAsync">非同期で読み込むかどうか</param>
        /// <returns></returns>
        public virtual AsyncOperation AddScene(string sceneName, bool isAsync = false)
        {
            return AddLoadScene(sceneName, isAsync);
        }

        /// <summary>
        /// 読み込まれているシーンを破棄します。
        /// </summary>
        /// <param name="sceneName">破棄するシーン名</param>
        /// <returns>Unity5.4以下のバージョンでは<c>null</c>、Uniyt5.5以降のバージョンでは非同期処理の状態を返します。</returns>
        public virtual AsyncOperation UnloadScene(string sceneName)
        {
            if (loadedSceneControllers.ContainsKey(sceneName))
            {
                return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            }

            return null;
        }

        /// <summary>
        /// <para>シーンの制御コンポーネントを登録します。</para>
        /// <para>基本的にBaseSceneControllerから自動的に呼び出されます。</para>
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="sceneController">シーンの制御コンポーネント</param>
        public void RegisterScene(string sceneName, BaseSceneController sceneController)
        {
            // モーダルシーンとして開いた場合のみ、情報をスタックする必要が有るため、追加で情報を登録する
            if (willLoadModal.Contains(sceneName))
            {
                // 現存するシーンのうち、アクティブなシーンの制御コンポーネントを
                // モーダルシーンを表示する前のシーンの表示状態として記録
                List<BaseSceneController> beforeScenes = new List<BaseSceneController>();
                foreach (BaseSceneController controller in loadedSceneControllers.Values)
                {
                    if (controller.gameObject.activeSelf)
                    {
                        beforeScenes.Add(controller);
                    }
                }

                UnityEngine.SceneManagement.Scene currentActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                modalSceneStack.Push(new ModalStatus(sceneName, currentActiveScene.name, sceneController, beforeScenes));
                HideSceneExceptModal();
                willLoadModal.Remove(sceneName);
            }

            loadedSceneControllers.Add(sceneName, sceneController);
        }
        
        /// <summary>
        /// シーンをUnity側の管理上でアクティブ化します。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        public void ActivateScene(string sceneName)
        {
            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.name != sceneName)
            {
                UnityEngine.SceneManagement.Scene activateScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(activateScene);
            }
        }

        /// <summary>
        /// <para>シーンの制御コンポーネントの登録を解除します。</para>
        /// <para>基本的にBaseSceneControllerから自動的に呼び出されます。</para>
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        public void UnregisterScene(string sceneName)
        {
            if (loadedSceneControllers.ContainsKey(sceneName))
            {
                loadedSceneControllers.Remove(sceneName);
            }
        }

        /// <summary>
        /// <para>シーン間の受け渡し用データを設定します。</para>
        /// <para>一度設定したデータは、RemovePassingData、ClearPassingDataで破棄するか、同一のシーン名で上書きしない限り有効となります。</para>
        /// </summary>
        /// <param name="sceneName">受け渡し先のシーン名</param>
        /// <param name="data">受け渡すデータ</param>
        public virtual void SetPassingData(string sceneName, object data)
        {
            if (passingData.ContainsKey(sceneName))
            {
                passingData[sceneName] = data;
            }
            else
            {
                passingData.Add(sceneName, data);
            }
        }

        /// <summary>
        /// シーン間の受け渡し用データを取得します。
        /// </summary>
        /// <param name="sceneName">設定されているシーン名</param>
        /// <returns></returns>
        public virtual object GetPassingData(string sceneName)
        {
            if (passingData.ContainsKey(sceneName))
            {
                return passingData[sceneName];
            }
            return null;
        }

        /// <summary>
        /// シーン間の受け渡し用データを破棄します。
        /// </summary>
        /// <param name="sceneName">設定しているシーン名</param>
        public virtual void RemovePassingData(string sceneName)
        {
            if (passingData.ContainsKey(sceneName))
            {
                passingData.Remove(sceneName);
            }
        }

        /// <summary>
        /// シーン間の受け渡し用データを全てクリアします。
        /// </summary>
        public virtual void ClearPassingData()
        {
            passingData.Clear();
        }

        /// <summary>
        /// シーンのアクティブ、非アクティブを切り替えます。
        /// </summary>
        /// <param name="sceneName">対象シーンのシーン名</param>
        /// <param name="active">シーンのアクティブ状態の指定</param>
        public void SetActive(string sceneName, bool active)
        {
            if (loadedSceneControllers.ContainsKey(sceneName))
            {
                GameObject sceneController = loadedSceneControllers[sceneName].gameObject;
                sceneController.SetActive(active);
            }
        }

        /// <summary>
        /// 現在読み込まれているシーンの制御コンポーネントを取得します。
        /// </summary>
        /// <returns></returns>
        public List<BaseSceneController> GetSceneControllers()
        {
            return loadedSceneControllers.Values.ToList();
        }

        /// <summary>
        /// シーン名から該当するシーンの制御コンポーネントを取得します。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <returns></returns>
        public BaseSceneController GetSceneController(string sceneName)
        {
            if (loadedSceneControllers.ContainsKey(sceneName))
            {
                return loadedSceneControllers[sceneName];
            }
            return null;
        }

        /// <summary>
        /// 現在読み込まれているシーンのシーン名を取得します。
        /// </summary>
        /// <returns></returns>
        public List<string> GetSceneNames()
        {
            return loadedSceneControllers.Keys.ToList();
        }

#endregion

#region protected methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected SceneManager()
        {
        }

        /// <summary>
        /// <para>シーンを読み込みます。</para>
        /// <para>isAsyncをtrueにした場合、非同期で読み込みを行います。</para>
        /// </summary>
        /// <param name="sceneName">読み込むシーン名</param>
        /// <param name="isAsync">非同期で読み込むかどうか</param>
        /// <returns></returns>
        protected AsyncOperation LoadScene(string sceneName, bool isAsync = true)
        {
            // 別なシーンに遷移した場合、モーダルシーンの情報を破棄する
            modalSceneStack.Clear();

            currentSceneName = sceneName;

            if (isAsync)
            {
                return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                return null;
            }
        }

        /// <summary>
        /// <para>シーンを追加で読み込みます。</para>
        /// <para>isAsyncをtrueにした場合、非同期で読み込みを行います。</para>
        /// </summary>
        /// <param name="sceneName">追加で読み込むシーン名</param>
        /// <param name="isAsync">非同期で読み込むかどうか</param>
        /// <returns></returns>
        protected AsyncOperation AddLoadScene(string sceneName, bool isAsync = true)
        {
            if (isAsync)
            {
                return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName,
                    UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName,
                    UnityEngine.SceneManagement.LoadSceneMode.Additive);
                return null;
            }
        }
        
        /// <summary>
        /// シーンの読み込み処理を行います。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <returns></returns>
        protected virtual List<AsyncOperation> LoadDefault(string sceneName)
        {
            List<AsyncOperation> operations = new List<AsyncOperation>();
            operations.Add(LoadScene(sceneName, true));
            return operations;
        }

        /// <summary>
        /// モーダルシーンの読み込み処理を行います。
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        protected virtual List<AsyncOperation> LoadModal(string sceneName)
        {
            List<AsyncOperation> operations = new List<AsyncOperation>();
            operations.Add(AddLoadScene(sceneName, true));
            return operations;
        }

        /// <summary>
        /// 現在のモーダルシーンを破棄する処理を行います。
        /// </summary>
        /// <returns></returns>
        protected virtual List<AsyncOperation> UnloadModal()
        {
            List<AsyncOperation> operations = new List<AsyncOperation>();

            lastUnloadModalScene = modalSceneStack.Pop();
            operations.Add(UnloadScene(lastUnloadModalScene.SceneName));

            return operations;
        }

        /// <summary>
        /// 全てのモーダルシーンを破棄する処理を行います。
        /// </summary>
        /// <returns></returns>
        protected virtual List<AsyncOperation> UnloadModalAll()
        {
            List<AsyncOperation> operations = new List<AsyncOperation>();

            ModalStatus modal = null;
            while (modalSceneStack.Count > 0)
            {
                modal = modalSceneStack.Pop();

                // 一番最初のモーダルシーンのみアクティブ状態を復元するために保持
                if (modalSceneStack.Count == 0)
                {
                    lastUnloadModalScene = modal;
                }

                operations.Add(UnloadScene(modal.SceneName));
            }

            return operations;
        }

        /// <summary>
        /// 最後に破棄されたModalシーンの前にアクティブだったシーンをUnity側の管理上でアクティブ化します。
        /// </summary>
        protected virtual void ActivateSceneBeforeModal()
        {
            if (lastUnloadModalScene == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("SceneManager.ActivateSceneBeforeModal : Not exist unloaded modal scene!");
#endif
                return;
            }

            ActivateScene(lastUnloadModalScene.LastActiveSceneName);
            foreach (BaseSceneController controller in lastUnloadModalScene.BeforeScenes)
            {
                controller.gameObject.SetActive(true);
                controller.SendMessage(BaseSceneController.CallbackResumeModal);
            }

            lastUnloadModalScene = null;
        }

        /// <summary>
        /// モーダルシーン以外の現在アクティブな全てのシーンを非アクティブ化します。
        /// </summary>
        protected void HideSceneExceptModal()
        {
            if (modalSceneStack.Count > 0)
            {
                List<BaseSceneController> beforeScenes = modalSceneStack.Peek().BeforeScenes;
                foreach (BaseSceneController controller in beforeScenes)
                {
                    controller.gameObject.SetActive(false);
                }
            }
        }

#endregion
    }
}
