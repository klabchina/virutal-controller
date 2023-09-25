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
    [System.Serializable]
    public class AdvSettingDataManager
    {
#region properties

        /// <summary>エンジンの設定データ</summary>
        [SerializeField]
        protected AdvEngineSetting engineSetting;

        /// <summary>エンジンの設定データ</summary>
        public AdvEngineSetting EngineSetting { get { return engineSetting; } }

        /// <summary>プレーンの設定データ</summary>
        [SerializeField]
        protected AdvPlaneSetting planeSetting;

        /// <summary>プレーンの設定データ</summary>
        public AdvPlaneSetting PlaneSetting { get { return planeSetting; } }

        /// <summary>レイヤーの設定データ</summary>
        [SerializeField]
        protected AdvLayerSetting layerSetting;

        /// <summary>レイヤーの設定データ</summary>
        public AdvLayerSetting LayerSetting { get { return layerSetting; } }

        /// <summary>オブジェクトの設定データ</summary>
        [SerializeField]
        protected AdvObjectSetting objectSetting;

        /// <summary>オブジェクトの設定データ</summary>
        public AdvObjectSetting ObjectSetting {  get { return objectSetting; } }


#endregion
    }
}
