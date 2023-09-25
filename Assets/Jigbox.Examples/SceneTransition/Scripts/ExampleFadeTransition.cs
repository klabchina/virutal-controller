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
using Jigbox.SceneTransition;

namespace Jigbox.Examples
{
    public class ExampleFadeTransition : BaseSceneTransitionController
    {
#region constants

        /// <summary>デフォルトのフェードアウト時間</summary>
        public const float DefaultFadeOutTime = 0.5f;

        /// <summary>デフォルトのフェードイン時間</summary>
        public const float DefaultFadeInTime = 0.5f;

        /// <summary>画面サイズ</summary>
        protected readonly Rect ScreenRect = new Rect(0, 0, Screen.width, Screen.height);

        /// <summary>非同期読み込みの終了基準</summary>
        protected static float AsyncLoadStateBorder = 0.9f;

#endregion

#region properties

        /// <summary>フェードアウトする時間</summary>
        protected float fadeOutTime;

        /// <summary>フェードアウト後待機する時間</summary>
        protected float fadeInTime;

        /// <summary>フェードインする時間</summary>
        protected float holdTime;

        /// <summary>フェードの色</summary>
        protected Color fadeColor;

        /// <summary>フェード用画像</summary>
        [SerializeField]
        protected Image image;

#endregion

#region public methods

        /// <summary>
        /// フェード時間の設定を行います。
        /// </summary>
        /// <param name="fadeOutTime">フェードアウト時間</param>
        /// <param name="fadeInTime">フェードイン時間</param>
        /// <param name="holdTime">フェードアウト後の待機時間</param>
        public void SetFadeTime(
            float fadeOutTime = DefaultFadeOutTime,
            float fadeInTime = DefaultFadeInTime,
            float holdTime = 0.0f)
        {
            this.fadeOutTime = fadeOutTime;
            this.fadeInTime = fadeInTime;
            this.holdTime = holdTime;
        }

        /// <summary>
        /// フェードの色を設定します
        /// </summary>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        public void SetColor(float r, float g, float b)
        {
            fadeColor.r = r;
            fadeColor.g = g;
            fadeColor.b = b;
        }

#endregion

#region protected methods

        protected override void OnAwake()
        {
            base.OnAwake();

            SetColor(0.0f, 0.0f, 0.0f);
            SetFadeTime();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            UpdateImage();
        }

        /// <summary>
        /// フェードアウトを行います。
        /// </summary>
        protected override void SceneOutTransition()
        {
            deltatime += Time.deltaTime;
            fadeColor.a = deltatime / fadeOutTime;
            if (deltatime >= fadeOutTime)
            {
                // SceneControllerにフェードアウト完了を通知
                NoticeMessageFinishOutTransition();

                fadeColor.a = 1.0f;
                
                // フェードアウトの保持時間が設定されていない、かつ、読み込み可能な場合、シーンを読み込み
                if (holdTime <= 0.0f && SceneTransitionManager.Instance.IsReadyLoadScene)
                {
                    LoadScene();
                }
                else
                {
                    state = State.Hold;
                    deltatime = 0.0f;
                }
            }
        }

        /// <summary>
        /// フェードアウトが終了した状態を維持します。
        /// </summary>
        protected override void Hold()
        {
            deltatime += Time.deltaTime;
            if (deltatime >= holdTime && SceneTransitionManager.Instance.IsReadyLoadScene)
            {
                LoadScene();
            }
        }

        /// <summary>
        /// フェードインの開始待ちを行います。
        /// </summary>
        protected override void WaitInTransition()
        {
            if (SceneTransitionManager.Instance.IsReadyInTransition)
            {
                state = State.SceneInTransition;
            }
        }

        /// <summary>
        /// フェードインを行います。
        /// </summary>
        protected override void SceneInTransition()
        {
            deltatime += Time.deltaTime;
            fadeColor.a = 1.0f - deltatime / fadeInTime;
            if (deltatime >= fadeInTime)
            {
                fadeColor.a = 0.0f;
                state = State.None;
                deltatime = 0.0f;
                NoticeMessageFinishInTransition();
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// シーンの読み込みが完了した際に呼び出されます。
        /// </summary>
        protected override void OnFinishSceneLoad()
        {
            if (SceneTransitionManager.Instance.IsReadyInTransition)
            {
                state = State.SceneInTransition;
            }
            else
            {
                state = State.WaitInTransition;
            }
            deltatime = 0.0f;
        }

        /// <summary>
        /// 画像の状態を更新します。
        /// </summary>
        protected virtual void UpdateImage()
        {
            if (image.color.a != fadeColor.a)
            {
                image.color = fadeColor;
            }
        }

#endregion
    }
}
