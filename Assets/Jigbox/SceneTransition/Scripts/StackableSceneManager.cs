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

namespace Jigbox.SceneTransition
{
    public class StackableSceneManager : SceneManager
    {
#region inner classes, enum, and structs
        
        public class StackInfo
        {
            /// <summary>シーン名</summary>
            public string SceneName { get; protected set; }

            /// <summary>データ</summary>
            public object Data { get; set; }            

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="sceneName">シーン名</param>
            /// <param name="data">データ</param>
            public StackInfo(string sceneName, object data)
            {
                SceneName = sceneName;
                Data = data;
            }
        }

#endregion

#region constants

        /// <summary>シーンのスタック情報が見つからなかった場合のインデックス</summary>
        protected static readonly int StackNotFound = -1;

#endregion

#region properties
        
        /// <summary>インスタンス</summary>
        public new static StackableSceneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StackableSceneManager();
                }
                return instance as StackableSceneManager;
            }
        }

        /// <summary>シーン情報のスタック数</summary>
        public int StackCount { get { return sceneStack.Count; } }

        /// <summary>シーンの情報スタック</summary>
        protected List<StackInfo> sceneStack = new List<StackInfo>();
        
        /// <summary>シーン遷移時に必要なスタックの回数</summary>
        protected int needStackCount = 0;

#endregion

#region public methods

        /// <summary>
        /// 現在のモーダルシーンを破棄し、モーダルシーンを開く前にあったシーンをアクティブ化します。
        /// </summary>
        /// <param name="transitionResourcePath">シーン遷移時のトランジション用オブジェクトのプレハブのパス</param>
        /// <returns>シーンの遷移を開始できた場合、trueを返します。</returns>
        public override bool CloseModalScene(string transitionResourcePath = null)
        {
            bool result = base.CloseModalScene(transitionResourcePath);
            if (result)
            {
                ModalStatus modal = modalSceneStack.Peek();
                StackableBaseSceneController controller = modal.SceneController as StackableBaseSceneController;
                if (controller != null)
                {
                    controller.AutoRemoveStack = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 全てのモーダルシーンを破棄し、モーダルシーンを開く前にあったシーンをアクティブ化します。
        /// </summary>
        /// <param name="transitionResourcePath">シーン遷移時のトランジション用オブジェクトのプレハブのパス</param>
        /// <returns>シーンの遷移を開始できた場合、trueを返します。</returns>
        public override bool CloseModalSceneAll(string transitionResourcePath = null)
        {
            bool result = base.CloseModalSceneAll(transitionResourcePath);
            if (result)
            {
                foreach (ModalStatus modal in modalSceneStack)
                {
                    StackableBaseSceneController controller = modal.SceneController as StackableBaseSceneController;
                    if (controller != null)
                    {
                        controller.AutoRemoveStack = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// シーンをトランジションを行わずに読み込みます。
        /// </summary>
        /// <param name="sceneName">読み込むシーン名</param>
        /// <param name="isAsync">非同期で読み込むかどうか</param>
        /// <returns></returns>
        public override AsyncOperation NoneTransitionLoadScene(string sceneName, bool isAsync = false)
        {
            CheckSceneStack(sceneName);
            return base.NoneTransitionLoadScene(sceneName, isAsync);
        }

        /// <summary>
        /// シーンを戻ります。
        /// </summary>
        /// <param name="sceneName">戻り先のシーン名</param>
        /// <param name="data">戻り先のシーンの受け渡し用データを変更する場合に設定します</param>
        /// <param name="transitionResourcePath">シーン遷移時のトランジション用オブジェクトのプレハブのパス</param>
        /// <returns>シーンの遷移を開始できた場合、trueを返します。</returns>
        public virtual bool BackScene(string sceneName = null, object data = null, string transitionResourcePath = null)
        {
#if UNITY_EDITOR
            if (modalSceneStack.Count > 0)
            {
                Debug.LogWarning("StackableSceneManager.BackScene : Opened modal!");
            }
#endif
            return BackScene(sceneName, data, transitionResourcePath, true);
        }

        /// <summary>
        /// <para>シーンを戻ります。</para>
        /// <para>※戻り先のシーンは、最古のシーンとなります。</para>
        /// </summary>
        /// <param name="sceneName">戻り先のシーン名</param>
        /// <param name="data">戻り先のシーンの受け渡し用データを変更する場合に設定します</param>
        /// <param name="transitionResourcePath">シーン遷移時のトランジション用オブジェクトのプレハブのパス</param>
        /// <returns>シーンの遷移を開始できた場合、trueを返します。</returns>
        public virtual bool BackLastScene(string sceneName, object data = null, string transitionResourcePath = null)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
#if UNITY_EDITOR
                Debug.LogError("StackableSceneManager.BackLastScene : Scene name is null or empty");
#endif
                return false;
            }
#if UNITY_EDITOR
            if (modalSceneStack.Count > 0)
            {
                Debug.LogWarning("StackableSceneManager.BackScene : Opened modal!");
            }
#endif
            return BackScene(sceneName, data, transitionResourcePath, false);
        }

        /// <summary>
        /// シーン間の受け渡し用データを取得します。
        /// </summary>
        /// <param name="sceneName">設定されているシーン名</param>
        /// <returns></returns>
        public override object GetPassingData(string sceneName)
        {
            int index = IndexOfSceneStack(sceneName);
            if (index != StackNotFound)
            {
                return sceneStack[index].Data;
            }
            else
            {
                return base.GetPassingData(sceneName);
            }
        }

        /// <summary>
        /// <para>シーン情報を設定します。</para>
        /// <para>すでに同一のシーン名が設定されている場合でも追加されます。</para>
        /// </summary>
        /// <param name="sceneName">受け渡し先のシーン名</param>
        /// <param name="data">受け渡すデータ</param>
        public void PushSceneStack(string sceneName, object data)
        {
            --needStackCount;
            sceneStack.Add(new StackInfo(sceneName, data));
        }

        /// <summary>
        /// <para>スタックされているシーン情報の中で、
        /// 対象シーン名でスタックされている最新のデータより新しいスタックを破棄します。</para>
        /// <para>※指定したシーンは破棄の対象に含まれません。</para>
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        public void RemoveSceneStack(string sceneName)
        {
            int index = IndexOfSceneStack(sceneName);
            if (index != StackNotFound)
            {
                RemoveSceneStack(sceneStack.Count - index - 1);
            }
        }

        /// <summary>
        /// <para>スタックされているシーン情報の中で、
        /// 対象シーン名でスタックされている最古のデータより新しいスタックを破棄します。</para>
        /// <para>※指定したシーンは破棄の対象に含まれません。</para>
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        public void RemoveLastSceneStack(string sceneName)
        {
            int index = LastIndexOfSceneStack(sceneName);
            if (index != StackNotFound)
            {
                RemoveSceneStack(sceneStack.Count - index - 1);
            }
        }

        /// <summary>
        /// <para>スタックされているシーン情報を最新から指定数分破棄します。</para>
        /// <para>※デフォルト状態で最新から1つ分(最新のスタック)を破棄します。</para>
        /// </summary>
        /// <param name="removeCount"></param>
        public void RemoveSceneStack(int removeCount = 1)
        {
            if (removeCount <= 0)
            {
                return;
            }
            if (sceneStack.Count < removeCount)
            {
#if UNITY_EDITOR
                Debug.LogWarning("StackableSceneManager.RemoveSceneStack : stack count zero!");
#endif
                sceneStack.Clear();
                return;
            }
            int index = sceneStack.Count - removeCount;
            sceneStack.RemoveRange(index, removeCount);
        }

        /// <summary>
        /// スタックされているシーン情報を全て破棄します。
        /// </summary>
        public void ClearSceneStack()
        {
            sceneStack.Clear();
        }

        /// <summary>
        /// スタックされている最新のシーン情報のシーン名を取得します。
        /// </summary>
        /// <returns></returns>
        public string GetSceneNameOfStackTop()
        {
            if (sceneStack.Count > 0)
            {
                return sceneStack[sceneStack.Count - 1].SceneName;
            }
            return string.Empty;
        }

        /// <summary>
        /// 対象シーン名でスタックされているシーン情報の中で最も新しいデータを上書きします。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="data">上書きするデータ</param>
        /// <returns>上書きに成功した場合、trueを返します。</returns>
        public bool OverrideSceneStack(string sceneName, object data)
        {
            int index = IndexOfSceneStack(sceneName);
            if (index != StackNotFound)
            {
                sceneStack[index].Data = data;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 現在のシーンのスタックを破棄します。
        /// </summary>
        /// <param name="sceneName">受け渡し先のシーン名</param>
        public void AutoRemoveSceneStack(string sceneName)
        {
            if (sceneStack.Count <= 1)
            {
#if UNITY_EDITOR
                Debug.LogWarning("StackableSceneController.AutoRemoveSceneStack : Not enough to stack!");
#endif
                return;
            }

            // 先頭には次のシーン情報が積まれているので、先頭は最初から飛ばす
            for (int i = sceneStack.Count - 2; i >= 0; --i)
            {
                if (sceneStack[i].SceneName == sceneName)
                {
                    sceneStack.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// スタックのリストを取得します。
        /// </summary>
        /// <returns></returns>
        public List<StackInfo> GetStackList()
        {
            return new List<StackInfo>(sceneStack);
        }

#endregion

#region protected methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected StackableSceneManager()
        {
        }

        /// <summary>
        /// シーンを戻ります。
        /// </summary>
        /// <param name="sceneName">戻り先のシーン名</param>
        /// <param name="data">戻り先のシーンの受け渡し用データを変更する場合に設定します</param>
        /// <param name="transitionResourcePath">シーン遷移時のトランジション用オブジェクトのプレハブのパス</param>
        /// <param name="isTop">先頭からかどうか</param>
        /// <returns>シーンの遷移を開始できた場合、trueを返します。</returns>
        protected virtual bool BackScene(string sceneName, object data, string transitionResourcePath, bool isTop)
        {
            // スタックが2以上ないと戻れない
            if (sceneStack.Count <= 1)
            {
                return false;
            }

            if (SceneTransitionManager.Instance.IsTransition)
            {
#if UNITY_EDITOR
                Debug.Log("StackableSceneManager.BackScene : Can't load the scene, because already began to transitioning!");
#endif
                return false;
            }

            // シーン名指定がない場合、一つ前のスタックのシーンに戻るように設定
            if (string.IsNullOrEmpty(sceneName))
            {
                RemoveSceneStack();
                sceneName = GetSceneNameOfStackTop();
            }
            else
            {
                if (IndexOfSceneStack(sceneName) == StackNotFound)
                {
#if UNITY_EDITOR
                    Debug.Log("StackableSceneManager.BackScene : Not stacked the scene!");
#endif
                    return false;
                }
                if (isTop)
                {
                    RemoveSceneStack(sceneName);
                }
                else
                {
                    RemoveLastSceneStack(sceneName);
                }
            }

            if (data != null)
            {
                OverrideSceneStack(sceneName, data);
            }

            BaseSceneTransitionController transitionController = SceneTransitionManager.Instance.CreateTransition(transitionResourcePath);
            transitionController.SetLoadAction(sceneName, () => base.LoadDefault(sceneName));

            int handlerCount = 0;
            foreach (BaseSceneController controller in loadedSceneControllers.Values)
            {
                StackableBaseSceneController stackable = controller as StackableBaseSceneController;
                if (stackable != null)
                {
                    // 現在のシーンがどういう設定であれ、すでに戻り先のシーンまでスタックは戻し終わっているので
                    // 余計な操作を挟まれないようにフラグを切る
                    stackable.AutoRemoveStack = false;
                }

                if (controller is ISceneUnloadHandler)
                {
                    ISceneUnloadHandler handler = controller as ISceneUnloadHandler;
                    handler.OnWillUnload(new SceneTransitionTransientController(transitionController, true));
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
        /// シーンの読み込み処理を行います。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <returns></returns>
        protected override List<AsyncOperation> LoadDefault(string sceneName)
        {
            CheckSceneStack(sceneName);
            return base.LoadDefault(sceneName);
        }

        /// <summary>
        /// モーダルシーンの読み込み処理を行います。
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        protected override List<AsyncOperation> LoadModal(string sceneName)
        {
            CheckSceneStack(sceneName);
            return base.LoadModal(sceneName);
        }

        /// <summary>
        /// 現在のモーダルシーンを破棄する処理を行います。
        /// </summary>
        /// <returns></returns>
        protected override List<AsyncOperation> UnloadModal()
        {
            RemoveSceneStack();
            return base.UnloadModal();
        }

        /// <summary>
        /// 全てのモーダルシーンを破棄する処理を行います。
        /// </summary>
        /// <returns></returns>
        protected override List<AsyncOperation> UnloadModalAll()
        {
            RemoveSceneStack(modalSceneStack.Count);
            return base.UnloadModalAll();
        }

        /// <summary>
        /// シーン情報スタックから対象のシーンの最新のインデックスを取得します。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <returns></returns>
        protected int IndexOfSceneStack(string sceneName)
        {
            for (int i = sceneStack.Count - 1; i >= 0; --i)
            {
                if (sceneStack[i].SceneName == sceneName)
                {
                    return i;
                }
            }

            return StackNotFound;
        }

        /// <summary>
        /// シーン情報スタックから対象のシーンの最古のインデックスを取得します。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <returns></returns>
        protected int LastIndexOfSceneStack(string sceneName)
        {
            for (int i = 0; i < sceneStack.Count; ++i)
            {
                if (sceneStack[i].SceneName == sceneName)
                {
                    return i;
                }
            }

            return StackNotFound;
        }

        /// <summary>
        /// スタックされているシーン情報を確認し、必要であれば空のスタックを追加します。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        protected void CheckSceneStack(string sceneName)
        {
            // 遷移するためにはスタックが詰まれる必要があるため、カウントアップする。
            // このメソッドが呼ばれる前に、PushSceneStackが呼び出されていた場合、
            // needStackCountはこの時点で-1以下となっているため、0より大きくはならない。
            // PushSceneStackが呼び出されていなかった場合、必ず1となる。
            ++needStackCount;
            bool isNeedStack = true;

            // スタックがない場合は要追加
            if (sceneStack.Count > 0)
            {
                // スタック必要数が0より大きい場合は要追加
                if (needStackCount <= 0)
                {
                    StackInfo stack = sceneStack[sceneStack.Count - 1] as StackInfo;
                    // 先の条件に加え、最新のスタックの情報が
                    // 遷移予定のシーンのものであれば追加不要
                    if (stack.SceneName == sceneName)
                    {
                        isNeedStack = false;
                    }
                }
            }
            
            if (isNeedStack)
            {
                PushSceneStack(sceneName, null);
            }

            needStackCount = 0;
        }

#endregion
    }
}
