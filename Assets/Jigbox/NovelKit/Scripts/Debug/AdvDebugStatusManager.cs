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
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public class AdvDebugStatusManager : MonoBehaviour
    {
#region properties

        /// <summary>シナリオ制御統合コンポーネント</summary>
        protected AdvMainEngine engine;

        /// <summary>デバッグ情報の表示用コンポーネント</summary>
        protected AdvDebugStatusView debugStatusView;

        /// <summary>デバッグ情報の表示用コンポーネント</summary>
        public AdvDebugStatusView DebugStatusView { get { return debugStatusView; } }

        /// <summary>デバッグ情報の監視モジュール</summary>
        protected HashSet<IAdvDebugStatusMonitor> monitors = new HashSet<IAdvDebugStatusMonitor>();

#endregion

#region public methods

        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="engine">シナリオ制御統合コンポーネント</param>
        public virtual void Init(AdvMainEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// デバッグ情報の表示用の構成を作成します。
        /// </summary>
        /// <param name="uiCanvas">UIを表示しているCanvasの参照</param>
        /// <param name="order">デバッグ情報の表示要求</param>
        public virtual void CreateView(Transform uiCanvas, AdvDebugStatusViewOrder order)
        {
            if (debugStatusView != null)
            {
                GameObject.Destroy(debugStatusView.gameObject);
                debugStatusView = null;
            }

            debugStatusView = order.Generate(uiCanvas);
            debugStatusView.gameObject.SetActive(false);
        }

        /// <summary>
        /// デバッグを開始します。
        /// </summary>
        public virtual void StartDebug()
        {
            debugStatusView.StartDebug();

            if (debugStatusView.IsShowFps)
            {
                AddMonitor(new AdvDebugFpsMonitor());
            }
            AdvDebugTimeMonitor timeMonitor = new AdvDebugTimeMonitor();
            AddMonitor(timeMonitor);
            timeMonitor.ShowTimecode(debugStatusView.IsShowTimecode);

            foreach (IAdvDebugStatusMonitor monitor in monitors)
            {
                monitor.Start();
            }
        }

        /// <summary>
        /// デバッグを終了します。
        /// </summary>
        public virtual void EndDebug()
        {
            debugStatusView.EndDebug();
            monitors.Clear();
        }

        /// <summary>
        /// 監視用モジュールを追加します。
        /// </summary>
        /// <param name="monitor">監視用モジュール</param>
        public void AddMonitor(IAdvDebugStatusMonitor monitor)
        {
            if (!monitors.Contains(monitor))
            {
                monitor.Init(debugStatusView);
                monitors.Add(monitor);
                if (engine.IsPlaying)
                {
                    monitor.Start();
                }
            }
#if UNITY_EDITOR || NOVELKIT_DEBUG
            else
            {
                AdvLog.Error("AdvDebugStatusManager.AddMonitor : Already added!");
            }
#endif
        }

        /// <summary>
        /// 監視用モジュールを取得します。
        /// </summary>
        /// <typeparam name="T">取得する監視用モジュールの型</typeparam>
        /// <returns></returns>
        public T GetMonitor<T>() where T : class, IAdvDebugStatusMonitor
        {
            foreach (IAdvDebugStatusMonitor monitor in monitors)
            {
                if (monitor is T)
                {
                    return monitor as T;
                }
            }

            return null;
        }

#endregion

#region override unity methods

        protected virtual void Update()
        {
            if (!engine.IsPlaying)
            {
                return;
            }

            foreach (IAdvDebugStatusMonitor monitor in monitors)
            {
                monitor.Update();
            }
        }

#endregion
    }
}
