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
using Jigbox.NovelKit;

namespace Jigbox.Examples
{
    public class ScenarioExampleController : ExampleSceneBase
    {
#region properties

        [SerializeField]
        AdvMainEngine engine = null;

        [SerializeField]
        Examples.AdvDummyPreloader preloader = null;

        [SerializeField]
        string script = "";

        [SerializeField]
        bool isShowDebugStatus = true;

        /// <summary>
        /// StreamingAssets以下にスクリプトを配置して、このフラグをtrueにすると、StreamingAssetsからデータを読み込みます。
        /// </summary>
        bool useStreamingAssets = false;

#endregion

#region override unity methods

        void Start()
        {
            if (useStreamingAssets)
            {
                engine.SetCustomScriptLoader(new AdvScriptStreamingAssetsLoader());
            }
            engine.SetPreloader(preloader);
            if (isShowDebugStatus)
            {
                engine.ShowDebugStatus();
            }
            engine.StartScenario(engine.LoadScenario(script));
            engine.SetEndHandler(() => { Debug.Log("end"); });
        }

#endregion
    }
}
