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
    public sealed class AdvLayerSetting : ScriptableObject
    {
#region properties

        /// <summary>前面プレーンのレイヤー</summary>
        [SerializeField]
        string planeFrontLayer = "Default";

        /// <summary>前面プレーンのレイヤー</summary>
        public string PlaneFrontLayer { get { return planeFrontLayer; } }

        /// <summary>前面プレーンを写すカメラのレイヤーカリング</summary>
        [SerializeField]
        int frontCameraCulilngMask = 0;

        /// <summary>前面プレーンを写すカメラのレイヤーカリング</summary>
        public int FrontCameraCulilngMask { get { return frontCameraCulilngMask; } }

        /// <summary>背面プレーンのレイヤー</summary>
        [SerializeField]
        string planeBackLayer = "Default";

        /// <summary>背面プレーンのレイヤー</summary>
        public string PlaneBackLayer { get { return planeBackLayer; } }

        /// <summary>背面プレーンを写すカメラのレイヤーカリング</summary>
        [SerializeField]
        int backCameraCullingMask = 0;

        /// <summary>背面プレーンを写すカメラのレイヤーカリング</summary>
        public int BackCameraCullingMask { get { return backCameraCullingMask; } }

        /// <summary>拡張用プレーンのレイヤー</summary>
        [SerializeField]
        string planeOptionalLayer = "Default";

        /// <summary>拡張用プレーンのレイヤー</summary>
        public string PlaneOptionalLayer { get { return planeOptionalLayer; } }

        /// <summary>拡張用プレーンを写すカメラのレイヤーカリング</summary>
        [SerializeField]
        int optionalCameraCullingMask = 0;

        /// <summary>拡張用プレーンを写すカメラのレイヤーカリング</summary>
        public int OptionalCameraCullingMask { get { return optionalCameraCullingMask; } }

        /// <summary>UIのレイヤー</summary>
        [SerializeField]
        string uiLayer = "UI";

        /// <summary>UIのレイヤー</summary>
        public string UILayer { get { return uiLayer; } }

        /// <summary>UIを写すカメラのレイヤーカリング</summary>
        [SerializeField]
        int uiCameraCullingMask = 0;

        /// <summary>UIを写すカメラのレイヤーカリング</summary>
        public int UICameraCullingMask { get { return uiCameraCullingMask; } }

        /// <summary>拡張用補助カメラのレイヤー</summary>
        [SerializeField]
        string subCameraLayer = "Default";

        /// <summary>拡張用補助カメラのレイヤー</summary>
        public string SubCameraLayer { get { return subCameraLayer; } }

        /// <summary>拡張用補助カメラのレイヤーカリング</summary>
        [SerializeField]
        int subCameraCullingMask = 0;

        /// <summary>拡張用補助カメラのレイヤーカリング</summary>
        public int SubCameraCullingMask { get { return subCameraCullingMask; } }

#endregion
    }
}
