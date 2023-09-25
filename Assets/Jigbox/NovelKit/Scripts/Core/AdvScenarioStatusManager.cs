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
    public class AdvScenarioStatusManager : MonoBehaviour, IAdvManagementComponent
    {
#region properties

        /// <summary>コマンドの遅延管理クラス</summary>
        protected AdvCommandDelayManager delayManager;
        
        /// <summary>再生状態</summary>
        public AdvPlayStateManager State { get; protected set; }

        /// <summary>再生モード</summary>
        public AdvPlayModeManager Mode { get; protected set; }
        
        /// <summary>コマンドが実行可能かどうか</summary>
        public bool IsExecute
        {
            get
            {
                if (Mode.IsAuto)
                {
                    return State.IsExecute && Mode.IsExecute && !delayManager.IsExistDelayCommands;
                }
                else
                {
                    return State.IsExecute;
                }
            }
        }

#endregion

#region public methods
        
        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        public virtual void Init(AdvMainEngine engine)
        {
            delayManager = engine.Player.DelayManager;
            State = new AdvPlayStateManager(this);
            Mode = new AdvPlayModeManager(this);
            Mode.SetAutoPlayWait(engine.Settings.EngineSetting.AutoPlayWait);
        }

        /// <summary>
        /// 初期化前の状態に戻します。
        /// </summary>
        public virtual void Uninit()
        {
            delayManager = null;
            State = null;
            Mode = null;
        }

#endregion

#region override unity methods

        protected virtual void Update()
        {
            if (State != null)
            {
                State.Update();
            }
            if (Mode != null)
            {
                Mode.Update();
            }
        }

#endregion
    }
}
