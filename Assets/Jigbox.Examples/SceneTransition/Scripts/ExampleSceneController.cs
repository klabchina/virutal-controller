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
using UnityEngine.UI;
using Jigbox.UIControl;
using Jigbox.SceneTransition;

namespace Jigbox.Examples
{
    public class ExampleSceneController : BaseSceneController
    {
#region properties

        [SerializeField]
        Text sceneNameLabel = null;

#endregion

#region protected methods

        /// <summary>
        /// シーンが生成されたタイミングで呼び出されます。
        /// </summary>
        /// <param name="passingData">シーンの受け渡し用データ、存在しない場合はnullとなります。</param>
        protected override void OnAwake(object passingData)
        {
            sceneNameLabel.text = SceneName;

            Debug.Log(SceneName + " : " + "OnAwake");
            SceneTransitionManager.Instance.DefaultTransitionPrefabPath = "ExsampleFadeTransition";

            if (passingData != null)
            {
                Debug.Log(SceneName + ":" + passingData.ToString());
            }
        }

        /// <summary>
        /// シーンの開始時に呼び出されます。
        /// </summary>
        protected override void OnStart()
        {
            Debug.Log(SceneName + " : " + "OnStart");

            // 即座にトランジションが明けても問題ない場合はそのまま処理を行い、
            // ビューの初期化に時間が掛かるなど、トランジションの開始を待たなくてはいけない場合は、
            // このメソッドが終わる前にIsReadyInTransitoinをfalseにして、
            // 準備が整ったタイミングでtrueにしてトランジションを開始させる想定
        }

        /// <summary>
        /// シーンの更新時に呼び出されます。
        /// </summary>
        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// シーンが有効になった際に呼び出されます。
        /// </summary>
        protected override void OnEnabled()
        {
            Debug.Log(SceneName + " : " + "OnEnabled");
        }

        /// <summary>
        /// シーンが無効になった際に呼び出されます。
        /// </summary>
        protected override void OnDisabled()
        {
            Debug.Log(SceneName + " : " + "OnDisabled");
        }

        /// <summary>
        /// シーンが破棄される際に呼び出されます。
        /// </summary>
        protected override void OnDestroyed()
        {
            Debug.Log(SceneName + " : " + "OnDestroyed");
        }

        /// <summary>
        /// アプリが一時停止した際に呼び出されます。
        /// </summary>
        protected override void OnSuspend()
        {
            Debug.Log(SceneName + " : " + "OnSuspend");
        }

        /// <summary>
        /// アプリが再開した際に呼び出されます。
        /// </summary>
        protected override void OnResume()
        {
            Debug.Log(SceneName + " : " + "OnResume");
        }

        /// <summary>
        /// <para>他のシーンへの遷移時に、遷移時のトランジション(フェードアウト等)が完了した際に呼び出されます。</para>
        /// <para>次のシーンに必要なデータの取得(通信)等が必要な場合はこのメソッドをオーバーライドして実行して下さい。</para>
        /// <para>モーダルシーンを閉じる際に呼び出される場合、nextSceneNameは空文字列が入ります。</para>
        /// </summary>
        /// <param name="nextSceneName">次に遷移するシーン名</param>
        protected override void OnFinishSceneOutTransition(string nextSceneName)
        {
            Debug.Log(SceneName + " : " + "OnFinishSceneOutTransition");

            // シーンの遷移に必要な処理がある場合(通信等)は、このタイミングで次のシーン名から
            // 必要な処理を取得し実行し、なければそのまま遷移
            // というような実装を各案件ごとに行ってもらう想定

            base.OnFinishSceneOutTransition(nextSceneName);
        }

        /// <summary>
        /// 他のシーンからの遷移時に、遷移時のトランジション(フェードイン等)が終了した際に呼び出されます。
        /// </summary>
        protected override void OnFinishSceneInTransition()
        {
            Debug.Log(SceneName + " : " + "OnFinishSceneInTransition");
        }

        /// <summary>
        /// <para>モーダルビューから戻って、元のシーンがアクティブ化された際に呼び出されます。</para>
        /// <para>このメソッドをオーバーライドした場合、IsReadyInTransitionをtrueにするか、
        /// 継承元の本メソッドを呼び出さない限り、トランジションが開始されません。</para>
        /// </summary>
        protected override void OnResumeModal()
        {
            Debug.Log(SceneName + " : " + "OnResumeModal");
            base.OnResumeModal();
        }

        /// <summary>
        /// モーダルビューから戻る遷移のトランジションが終了した際に呼び出されます。
        /// </summary>
        protected override void OnFinishResumeTransitionModal()
        {
            Debug.Log(SceneName + " : " + "OnFinishResumeTransitionModal");
        }

        /// <summary>
        /// <para>エスケープキー(Androidのバックキー)が押された際に呼び出されます。</para>
        /// <para>モーダルシーンとして開いている場合、このメソッドは呼び出されません。</para>
        /// </summary>
        protected override void OnEscape()
        {
            Debug.Log(SceneName + " : " + "OnEscape");
        }

        /// <summary>
        /// <para>エスケープキー(Androidのバックキー)が押された際に呼び出されます。</para>
        /// <para>モーダルシーンとして開いている場合のみ呼びだされます。</para>
        /// </summary>
        protected override void OnEscapeModal()
        {
            Debug.Log(SceneName + " : " + "OnEscapeModal");
            base.OnEscapeModal();
        }

        [AuthorizedAccess]
        protected void OnClickTest1()
        {
            SceneManager.Instance.LoadScene("SceneTransition1");
        }

        [AuthorizedAccess]
        protected void OnClickTest2()
        {
            SceneManager.Instance.LoadScene("SceneTransition2");
        }

        [AuthorizedAccess]
        protected void OnClickTest3()
        {
            SceneManager.Instance.LoadScene("SceneTransition3");
        }

        [AuthorizedAccess]
        protected void OnClickTest4()
        {
            SceneManager.Instance.LoadScene("SceneTransition4");
        }

        [AuthorizedAccess]
        protected void OnClickModal3()
        {
            if (!SceneManager.Instance.GetSceneNames().Contains("SceneTransition3"))
            {
                SceneManager.Instance.OpenModalScene("SceneTransition3");
            }
        }

        [AuthorizedAccess]
        protected void OnClickModal4()
        {
            if (!SceneManager.Instance.GetSceneNames().Contains("SceneTransition4"))
            {
                SceneManager.Instance.OpenModalScene("SceneTransition4");
            }
        }

        [AuthorizedAccess]
        protected void OnClickClose()
        {
            SceneManager.Instance.CloseModalScene();
        }

        [AuthorizedAccess]
        protected void OnClickCloseAll()
        {
            SceneManager.Instance.CloseModalSceneAll();
        }

#endregion
    }
}
