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
    public class AdvPreloadHandler
    {
#region properties

        /// <summary>再生状態の管理モジュール</summary>
        protected AdvPlayStateManager stateManager;

        /// <summary>読み込み中の処理の数</summary>
        protected int loadingCount = 0;

        /// <summary>読み込み中かどうか</summary>
        public virtual bool HasLoading { get { return loadingCount > 0; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="stateManager">再生状態の管理モジュール</param>
        public AdvPreloadHandler(AdvPlayStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        /// <summary>
        /// 読み込みを開始した際に呼び出されます。
        /// </summary>
        public void OnStartLoad()
        {
            ++loadingCount;
        }

        /// <summary>
        /// 読み込みが完了した際に呼び出されます。
        /// </summary>
        public void OnCompleteLoad()
        {
            --loadingCount;
            if (loadingCount == 0 && stateManager.IsWait(AdvPlayStateManager.PlayState.WaitPreload))
            {
                stateManager.WaitRelease(AdvPlayStateManager.PlayState.WaitPreload);
            }

            if (loadingCount < 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Complete count does not match to Start count!");
#endif
                loadingCount = 0;
            }
        }

#endregion
    }
}
