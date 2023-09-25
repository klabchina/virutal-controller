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
    public class AdvDebugFpsMonitor : IAdvDebugStatusMonitor
    {
#region constants

        /// <summary>FPS表示用のタグ名</summary>
        protected static readonly string TagName = "DEBUG_FPS";

        /// <summary>デバッグ表示のフォーマット</summary>
        protected static readonly string OutputFormat = "FPS : {0}";

        /// <summary>表示の更新間隔</summary>
        protected static readonly float UpdateIntervalTime = 0.5f;

#endregion

#region properties

        /// <summary>デバッグ情報のハンドラ</summary>
        protected IAdvDebugStatusHandler handler;

        /// <summary>経過時間</summary>
        protected float deltaTime = 0.0f;

        /// <summary>表示が更新されるまでの間にUpdateメソッドが呼び出された回数</summary>
        protected int updateCount = 0;

        /// <summary>現在のFPS</summary>
        public int CurrentFps { get; protected set; }

#endregion

#region public methods

        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="handler">デバッグ情報のハンドラ</param>
        public virtual void Init(IAdvDebugStatusHandler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// FPSの監視を開始します。
        /// </summary>
        public virtual void Start()
        {
            CurrentFps = 0;
            deltaTime = 0.0f;
            NotifyDebugStatus();
        }

        /// <summary>
        /// 状態を更新します。
        /// </summary>
        public virtual void Update()
        {
            deltaTime += Time.deltaTime;
            ++updateCount;
            if (deltaTime >= UpdateIntervalTime)
            {
                CurrentFps = Mathf.RoundToInt(1.0f / (deltaTime / updateCount));
                NotifyDebugStatus();
                deltaTime = 0.0f;
                updateCount = 0;
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// デバッグ状態を通知します。
        /// </summary>
        protected virtual void NotifyDebugStatus()
        {
            handler.Set(TagName, string.Format(OutputFormat, CurrentFps));
        }

#endregion
    }
}
