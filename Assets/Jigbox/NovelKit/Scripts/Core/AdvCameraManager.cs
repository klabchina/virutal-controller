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
    public class AdvCameraManager : MonoBehaviour, IAdvManagementComponent
    {
#region inner classes, enum, and structs

        /// <summary>
        /// カメラの種類
        /// </summary>
        public enum CameraType
        {
            /// <summary>UI</summary>
            UI,
            /// <summary>前面</summary>
            Front,
            /// <summary>背面</summary>
            Back,
            /// <summary>拡張用</summary>
            Optional,
            /// <summary>拡張用補助</summary>
            Sub,
        }

#endregion

#region properties

        /// <summary>シナリオの統合管理コンポーネント</summary>
        protected AdvMainEngine engine;

        /// <summary>前面プレーンのカメラ</summary>
        [SerializeField]
        protected Camera frontCamera;

        /// <summary>背面プレーンのカメラ</summary>
        [SerializeField]
        protected Camera backCamera;

        /// <summary>拡張用カメラ</summary>
        [SerializeField]
        protected Camera optionalCamera;

        /// <summary>拡張用補助カメラ</summary>
        [SerializeField]
        protected Camera subCamera;

        /// <summary>UI用カメラ</summary>
        [SerializeField]
        protected Camera uiCamera;
        
#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        public virtual void Init(AdvMainEngine engine)
        {
            this.engine = engine;
            AdvLayerSetting setting = engine.Settings.LayerSetting;

            frontCamera.cullingMask = 0;
            backCamera.cullingMask = 0;
            optionalCamera.cullingMask = 0;
            uiCamera.cullingMask = 0;

            frontCamera.cullingMask |= LayerMask.GetMask(setting.PlaneFrontLayer);
            frontCamera.cullingMask |= setting.FrontCameraCulilngMask;

            backCamera.cullingMask |= LayerMask.GetMask(setting.PlaneBackLayer);
            backCamera.cullingMask |= setting.BackCameraCullingMask;

            optionalCamera.cullingMask |= LayerMask.GetMask(setting.PlaneOptionalLayer);
            optionalCamera.cullingMask |= setting.OptionalCameraCullingMask;

            uiCamera.cullingMask |= LayerMask.GetMask(setting.UILayer);
            uiCamera.cullingMask |= setting.UICameraCullingMask;

            if (subCamera != null)
            {
                subCamera.cullingMask |= LayerMask.GetMask(setting.SubCameraLayer);
                subCamera.cullingMask |= setting.SubCameraCullingMask;
            }
        }

        /// <summary>
        /// 初期化前の状態に戻します。
        /// </summary>
        public virtual void Uninit()
        {
            engine = null;
        }

        /// <summary>
        /// オフスクリーンレンダリングの対象となるカメラを設定します。
        /// </summary>
        /// <param name="type">カメラの種類</param>
        public void SetOffscreenTarget(CameraType type)
        {
            engine.ScreenEffectionManager.ReleaseOffscreenTarget();

            Camera target = null;
            
            switch (type)
            {
                case CameraType.Front:
                    target = frontCamera;
                    break;
                case CameraType.Back:
                    target = backCamera;
                    break;
                case CameraType.Optional:
                    target = optionalCamera;
                    break;
                case CameraType.Sub:
                    target = subCamera;
                    break;
                default:
#if UNITY_EDITOR || NOVELKIT_DEBUG
                    AdvLog.Error("AdvCameraManager.SetOffscreenTarget : CameraType is invalid!");
#endif
                    return;
            }

            engine.ScreenEffectionManager.SetOffscreenTarget(target);
        }

        /// <summary>
        /// カメラを取得します。
        /// </summary>
        /// <param name="type">カメラの種類</param>
        /// <returns></returns>
        public Camera GetCamera(CameraType type)
        {
            switch (type)
            {
                case CameraType.UI:
                    return uiCamera;
                case CameraType.Front:
                    return frontCamera;
                case CameraType.Back:
                    return backCamera;
                case CameraType.Optional:
                    return optionalCamera;
                case CameraType.Sub:
                    return subCamera;
            }
            return null;
        }

#endregion
    }
}
