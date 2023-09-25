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

namespace Jigbox.SceneTransition
{
    public abstract class BaseSceneController : MonoBehaviour
    {
#region constains

        /// <summary>他のシーンへの遷移時に、遷移時のトランジション(フェードアウト等)が完了した際に呼び出されるコールバック関数名</summary>
        public static readonly string CallbackFinishSceneOutTransition = "OnFinishSceneOutTransition";

        /// <summary>他のシーンからの遷移時に、遷移時のトランジション(フェードイン等)が完了した際に呼び出されるコールバック関数名</summary>
        public static readonly string CallbackFinishSceneInTransition = "OnFinishSceneInTransition";

        /// <summary>モーダルシーンから戻って、元のシーンがアクティブになった際に呼び出されるコールバック関数名</summary>
        public static readonly string CallbackResumeModal = "OnResumeModal";

        /// <summary>モーダルシーンを表示した後、モーダルシーンから戻る際のトランジションが終了した際に呼び出されるコールバック関数名</summary>
        public static readonly string CallbackFinishResumeTransitionModal = "OnFinishResumeTransitionModal";

        /// <summary>エスケープキー(Androidのバックキー)が押された際に呼び出されるコールバックの関数名</summary>
        public static readonly string CallbackEscape = "OnEscape";

        /// <summary>モーダルシーン上でエスケープキー(Androidのバックキー)が押された際に呼び出されるコールバックの関数名</summary>
        public static readonly string CallBackEscapeOnModal = "OnEscapeModal";

#endregion

#region properties

        /// <summary>シーン名</summary>
        [HideInInspector]
        [SerializeField]
        string sceneName = string.Empty;

        /// <summary>シーン名</summary>
        public string SceneName { get { return sceneName; } }

        /// <summary>シーンをアクティブ化しないようにするかどうか</summary>
        [HideInInspector]
        [SerializeField]
        bool isNotActivate = false;

        /// <summary>一時停止状態かどうか</summary>
        protected bool IsPause { get; private set; }

        /// <summary>シーンの起動後、トランジションを開始していいかどうか</summary>
        bool isReadyInTransition = true;

        /// <summary>
        /// <para>シーンの起動後、トランジションを開始していいかどうか</para>
        /// <para>トランジションの開始を待つ場合は、OnStartが終了する前にこのフラグをfalseにしてください。</para>
        /// <para>OnStart終了時点でfalseが設定されていた場合、このフラグをtrueにするまでトランジションは開始されません。</para>
        /// <para>モーダルシーンから戻る際は、OnStartのタイミングではなく、
        /// OnResumeModal終了のタイミングでトランジションの開始を判断します。</para>
        /// </summary>
        protected bool IsReadyInTransition
        {
            get { return isReadyInTransition; }
            set
            {
                isReadyInTransition = value;
                if (SceneTransitionManager.Instance.IsTransition)
                {
                    SceneTransitionManager.Instance.IsReadyInTransition = value;
                }
            }
        }

        /// <summary>
        /// <para>シーン遷移時のトランジション終了後、次のシーンを読み込んでもいいかどうか</para>
        /// <para>OnFinishSceneOutTransitionをオーバーライドしていない場合、自動で切り替わります。</para>
        /// <para>トランジション終了時に次のシーンで必要なデータの取得(通信)等を行う場合は、
        /// 処理が終了した時点でこのフラグをtrueにして下さい。</para>
        /// </summary>
        protected bool IsReadyLoadScene
        {
            get { return SceneTransitionManager.Instance.IsReadyLoadScene; }
            set
            {
                if (SceneTransitionManager.Instance.IsTransition)
                {
                    SceneTransitionManager.Instance.IsReadyLoadScene = value;
                }
            }
        }
        
#endregion

#region protected methods

        /// <summary>
        /// シーンが生成されたタイミングで呼び出されます。
        /// </summary>
        /// <param name="passingData">シーンの受け渡し用データ、存在しない場合はnullとなります。</param>
        protected virtual void OnAwake(object passingData)
        {
        }

        /// <summary>
        /// シーンの開始時に呼び出されます。
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// シーンの更新時に呼び出されます。
        /// </summary>
        protected virtual void OnUpdate()
        {
        }

        /// <summary>
        /// シーンが有効になった際に呼び出されます。
        /// </summary>
        protected virtual void OnEnabled()
        {
        }

        /// <summary>
        /// シーンが無効になった際に呼び出されます。
        /// </summary>
        protected virtual void OnDisabled()
        {
        }

        /// <summary>
        /// シーンが破棄される際に呼び出されます。
        /// </summary>
        protected virtual void OnDestroyed()
        {
        }

        /// <summary>
        /// アプリが一時停止した際に呼び出されます。
        /// </summary>
        protected virtual void OnSuspend()
        {
        }

        /// <summary>
        /// アプリが再開した際に呼び出されます。
        /// </summary>
        protected virtual void OnResume()
        {
        }

        /// <summary>
        /// <para>他のシーンへの遷移時に、遷移時のトランジション(フェードアウト等)が完了した際に呼び出されます。</para>
        /// <para>次のシーンに必要なデータの取得(通信)等が必要な場合はこのメソッドをオーバーライドして実行して下さい。</para>
        /// <para>モーダルシーンを閉じる際に呼び出される場合、nextSceneNameは空文字列が入ります。</para>
        /// </summary>
        /// <param name="nextSceneName">次に遷移するシーン名</param>
        protected virtual void OnFinishSceneOutTransition(string nextSceneName)
        {
            IsReadyLoadScene = true;
        }

        /// <summary>
        /// 他のシーンからの遷移時に、遷移時のトランジション(フェードイン等)が終了した際に呼び出されます。
        /// </summary>
        protected virtual void OnFinishSceneInTransition()
        {
        }

        /// <summary>
        /// <para>モーダルビューから戻って、元のシーンがアクティブ化された際に呼び出されます。</para>
        /// <para>このメソッドをオーバーライドした場合、IsReadyInTransitionをtrueにするか、
        /// 継承元の本メソッドを呼び出さない限り、トランジションが開始されません。</para>
        /// </summary>
        protected virtual void OnResumeModal()
        {
            IsReadyInTransition = true;
        }

        /// <summary>
        /// モーダルビューから戻る遷移のトランジションが終了した際に呼び出されます。
        /// </summary>
        protected virtual void OnFinishResumeTransitionModal()
        {
        }

        /// <summary>
        /// <para>エスケープキー(Androidのバックキー)が押された際に呼び出されます。</para>
        /// <para>モーダルシーンとして開いている場合、このメソッドは呼び出されません。</para>
        /// </summary>
        protected virtual void OnEscape()
        {
        }

        /// <summary>
        /// <para>エスケープキー(Androidのバックキー)が押された際に呼び出されます。</para>
        /// <para>モーダルシーンとして開いている場合のみ呼びだされます。</para>
        /// </summary>
        protected virtual void OnEscapeModal()
        {
            SceneManager.Instance.CloseModalScene();
        }

#endregion

#region override unity methods

        void Awake()
        {
            SceneManager.Instance.RegisterScene(sceneName, this);

#pragma warning disable 219
            // インスタンスが必ず生成されるように参照を行う
            BackKeyManager instance = BackKeyManager.Instance;
#pragma warning restore 219
            
            OnAwake(SceneManager.Instance.GetPassingData(sceneName));
        }

        void Start()
        {
            if (!isNotActivate)
            {
                SceneManager.Instance.ActivateScene(sceneName);
            }

            OnStart();
            
            if (isReadyInTransition)
            {
                IsReadyInTransition = true;
            }
        }

        void OnEnable()
        {
            OnEnabled();
        }

        void Update()
        {
            OnUpdate();
        }

        void OnDisable()
        {
            OnDisabled();
        }

        void OnDestroy()
        {
            OnDestroyed();

            SceneManager.Instance.UnregisterScene(sceneName);
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                if (IsPause)
                {
                    return;
                }
                IsPause = true;
                OnSuspend();
            }
            else
            {
                if (!IsPause)
                {
                    return;
                }
                IsPause = false;
                OnResume();
            }
        }

#endregion
    }
}
