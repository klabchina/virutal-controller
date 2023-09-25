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

namespace Jigbox.NovelKit
{
    public class AdvScreenEffectionManager : MonoBehaviour, IAdvManagementComponent
    {
#region constants

        /// <summary>画面効果用オブジェクトのID</summary>
        public static readonly int ScreenEffectionObjectId = 1;

#endregion

#region properties

        /// <summary>画面効果を表示する際に親となるオブジェクト</summary>
        [SerializeField]
        protected GameObject effectionRoot;

        /// <summary>カメラのクリアフラグを自動設定するかどうか</summary>
        [SerializeField]
        protected bool autoChangeClearFlag = false;

        /// <summary>画面効果を表示するためのオブジェクト</summary>
        protected AdvObjectScreenEffection screenEffectionObject;
        
        /// <summary>オフスクリーンレンダリング用テクスチャ</summary>
        protected RenderTexture texture;

        /// <summary>現在オフスクリーンレンダリングで使用しているカメラ</summary>
        protected Camera offscreenTargetCamera;

        /// <summary>現在オフスクリーンレンダリングで使用しているカメラの本来のクリアカラー</summary>
        protected Color targetCameraClearColor;

        /// <summary>現在オフスクリーンレンダリングで使用しているカメラの本来のクリアフラグ</summary>
        protected CameraClearFlags targetCamerClearFlag;

        /// <summary>画面効果使用時に自動で表示を切り替えるか</summary>
        protected bool autoSwitchScreenEffection;

        /// <summary>画面効果が有効かどうか</summary>
        protected bool enable = false;

        /// <summary>画面効果が有効かどうか</summary>
        public bool Enable
        {
            get
            {
                return enable;
            }
            set
            {
                if (enable != value)
                {
                    enable = value;
                    if (autoSwitchScreenEffection)
                    {
                        if (enable && !screenEffectionObject.gameObject.activeSelf)
                        {
                            screenEffectionObject.Show(0.0f);
                        }
                        else if (!enable && screenEffectionObject.gameObject.activeSelf)
                        {
                            screenEffectionObject.Hide(0.0f);
                        }
                    }
                    
                    if (!enable)
                    {
                        ReleaseOffscreenTarget();
                    }
                }
            }
        }

#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="engine">シナリオ制御統合コンポーネント</param>
        public virtual void Init(AdvMainEngine engine)
        {
            if (!engine.Settings.EngineSetting.UseScreenEffection)
            {
                return;
            }

            autoSwitchScreenEffection = engine.Settings.EngineSetting.AutoSwitchScreenEffection;

            // 画面効果を表示するためのオブジェクトを生成
            AdvObjectSetting.ObjectSetting setting = engine.Settings.ObjectSetting.ScreenEffection;

            Object resource = engine.Loader.Load<Object>(setting.ResourcePath);
            if (resource == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("画面効果に使用するリソースが存在しません。"
                    + "\nパス : " + setting.ResourcePath);
#endif
                return;
            }

            GameObject obj = Instantiate(resource) as GameObject;
            screenEffectionObject = obj.GetComponent<AdvObjectScreenEffection>();
            if (screenEffectionObject == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("対象リソースに画面効果用のコンポーネントが存在しません。"
                    + "\nパス : " + setting.ResourcePath);
#endif
                return;
            }

            screenEffectionObject.Init(ScreenEffectionObjectId, AdvObjectBase.ObjectType.ScreenEffection, setting);
            screenEffectionObject.transform.SetParent(effectionRoot.transform, false);
            engine.ObjectManager.RegisterObject(screenEffectionObject);

            // レンダーテクスチャの生成、設定
            texture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            texture.enableRandomWrite = false;
            texture.autoGenerateMips = false;
            screenEffectionObject.RawImage.texture = texture;
            screenEffectionObject.Hide(0.0f);
        }

        /// <summary>
        /// 初期化前の状態に戻します。
        /// </summary>
        public virtual void Uninit()
        {
            enable = false;
            ReleaseRenderTexture();
        }

        /// <summary>
        /// オフスクリーンレンダリングの対象カメラを設定します。
        /// </summary>
        /// <param name="target">オフスクリーンレンダリングの対象となるカメラ</param>
        public void SetOffscreenTarget(Camera target)
        {
            offscreenTargetCamera = target;
            targetCameraClearColor = offscreenTargetCamera.backgroundColor;
            if (autoChangeClearFlag)
            {
                targetCamerClearFlag = offscreenTargetCamera.clearFlags;
                offscreenTargetCamera.clearFlags = CameraClearFlags.SolidColor;
            }
            offscreenTargetCamera.backgroundColor = Color.clear;
            // Camera.targetTextureに直接RenderTextureを設定した際、
            // 最初の1フレームだけレンダリングが遅れて一瞬だけ正しく表示されないため、
            // Camera.SetTargetBuffers(...)を使用
            offscreenTargetCamera.SetTargetBuffers(texture.colorBuffer, texture.depthBuffer);
        }

        /// <summary>
        /// オフスクリーンレンダリングの対象カメラを解除します。
        /// </summary>
        public void ReleaseOffscreenTarget()
        {
            if (offscreenTargetCamera != null)
            {
                // Camera.SetTargetBuffers(...)で突っ込んだバッファを直接クリアするメソッドはないが
                // targetTextureを設定することで内容を上書きできる
                offscreenTargetCamera.targetTexture = null;
                if (autoChangeClearFlag)
                {
                    offscreenTargetCamera.clearFlags = targetCamerClearFlag;
                }
                offscreenTargetCamera.backgroundColor = targetCameraClearColor;
                offscreenTargetCamera = null;
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// レンダーテクスチャを破棄します。
        /// </summary>
        protected void ReleaseRenderTexture()
        {
            if (texture != null)
            {
                texture.Release();
                texture = null;
            }
        }

#endregion

#region override unity methods

        void OnDestroy()
        {
            ReleaseOffscreenTarget();
            ReleaseRenderTexture();
        }

#endregion
    }
}
