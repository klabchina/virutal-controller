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
    using CameraType = AdvCameraManager.CameraType;

    public class AdvLayerManager : MonoBehaviour, IAdvManagementComponent
    {
#region properties

        /// <summary>前面プレーンのcanvas</summary>
        [SerializeField]
        protected GameObject canvasFront;

        /// <summary>背面プレーンのcanvas</summary>
        [SerializeField]
        protected GameObject canvasBack;

        /// <summary>拡張用プレーンのGameObject</summary>
        [SerializeField]
        protected GameObject planeOptional;

        /// <summary>UIのcanvas</summary>
        [SerializeField]
        protected GameObject canvasUI;

#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        public virtual void Init(AdvMainEngine engine)
        {
            AdvLayerSetting setting = engine.Settings.LayerSetting;

            canvasFront.layer = LayerMask.NameToLayer(setting.PlaneFrontLayer);
            canvasBack.layer = LayerMask.NameToLayer(setting.PlaneBackLayer);
            planeOptional.layer = LayerMask.NameToLayer(setting.PlaneOptionalLayer);
            canvasUI.layer = LayerMask.NameToLayer(setting.UILayer);

            SetCamera(engine, canvasFront, CameraType.Front);
            SetCamera(engine, canvasBack, CameraType.Back);
            SetCamera(engine, planeOptional, CameraType.Optional);
            SetCamera(engine, canvasUI, CameraType.UI);
        }

        /// <summary>
        /// 初期化前の状態に戻します。
        /// </summary>
        public virtual void Uninit()
        {
        }

#endregion

#region protected methods

        /// <summary>
        /// CanvasにCameraを設定します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <param name="target">対象のGameObject</param>
        /// <param name="type">カメラの種類</param>
        protected void SetCamera(AdvMainEngine engine, GameObject target, CameraType type)
        {
            Canvas canvas = target.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.worldCamera = engine.CameraManager.GetCamera(type);
            }
        }

#endregion
    }
}
