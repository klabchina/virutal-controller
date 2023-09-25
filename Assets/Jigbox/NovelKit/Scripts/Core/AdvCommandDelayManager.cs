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
    public class AdvCommandDelayManager : IAdvManagementComponent
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 遅延処理情報基底クラス
        /// </summary>
        protected abstract class DelayInfo
        {
            public abstract bool IsExecuted { get; }

            /// <summary>コマンド</summary>
            protected List<AdvCommandBase> commands = new List<AdvCommandBase>();
            
            /// <summary>
            /// 更新を行います。
            /// </summary>
            public virtual void Update()
            {
            }
            
            /// <summary>
            /// コマンドを実行します。
            /// </summary>
            /// <param name="engine">シナリオの統合管理コンポーネント</param>
            public virtual void Execute(AdvMainEngine engine, bool isComplement = false)
            {
                foreach (AdvCommandBase command in commands)
                {
                    // 補完ためにコマンドを実行する際に、コマンドが補完を制御するインタフェースを
                    // 実装している場合は、補完用の実行を行う
                    if (isComplement && command is IAdvCommandControlComplement)
                    {
                        IAdvCommandControlComplement complementCommand = command as IAdvCommandControlComplement;
                        complementCommand.ExecuteByComplement(engine);
                    }
                    else
                    {
                        command.Execute(engine);
                    }
                }
            }

            /// <summary>
            /// コマンドを追加します。
            /// </summary>
            /// <param name="command">コマンド</param>
            public virtual void Add(AdvCommandBase command)
            {
                commands.Add(command);
            }
        }

        /// <summary>
        /// 遅延処理するコマンド情報
        /// </summary>
        protected class DelayCommandInfo : DelayInfo
        {
            /// <summary>実行済みかどうか</summary>
            public override bool IsExecuted { get { return deltaTime >= delayTime; } }

            /// <summary>遅延時間</summary>
            protected float delayTime = 0.0f;

            /// <summary>経過時間</summary>
            protected float deltaTime = 0.0f;
            
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="delayTime">遅延時間</param>
            public DelayCommandInfo(float delayTime)
            {
                this.delayTime = delayTime;
            }

            /// <summary>
            /// 更新を行います。
            /// </summary>
            public override void Update()
            {
                if (IsExecuted)
                {
                    return;
                }

                deltaTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// サウンドからのマーカーによって遅延処理するコマンド情報
        /// </summary>
        protected class MarkerDelayCommandInfo : DelayInfo
        {
            public override bool IsExecuted { get { return false; } }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public MarkerDelayCommandInfo()
            {
            }
        }

#endregion

#region properties

        /// <summary>シナリオ制御統合コンポーネント</summary>
        protected AdvMainEngine engine;

        /// <summary>遅延処理するコマンドが存在するかどうか</summary>
        public bool IsExistDelayCommands { get { return delayInfo.Count > 0; } }
        
        /// <summary>遅延処理情報</summary>
        protected List<DelayInfo> delayInfo = new List<DelayInfo>();

#endregion

#region public methods
        
        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="engine">シナリオ制御統合コンポーネント</param>
        public virtual void Init(AdvMainEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// 初期化前の状態に戻します。
        /// </summary>
        public virtual void Uninit()
        {
        }

        /// <summary>
        /// 遅延処理を追加します。
        /// </summary>
        /// <param name="delayTime">遅延時間</param>
        public void AddDelay(float delayTime)
        {
            delayInfo.Add(new DelayCommandInfo(delayTime));
        }

        /// <summary>
        /// マーカーによる遅延処理を追加します。
        /// </summary>
        public void AddMarkerDelay()
        {
            delayInfo.Add(new MarkerDelayCommandInfo());
        }

        /// <summary>
        /// 遅延させるコマンドを追加します。
        /// </summary>
        /// <param name="command">コマンド</param>
        public virtual void AddCommand(AdvCommandBase command)
        {
            if (delayInfo.Count == 0)
            {
                return;
            }
            delayInfo[delayInfo.Count - 1].Add(command);
        }

        /// <summary>
        /// サウンド内のマーカーに達した際に呼び出されます。
        /// </summary>
        public void OnMarkedSound()
        {
            DelayInfo info = delayInfo[0];
            if (!(info is MarkerDelayCommandInfo))
            {
                return;
            }
            info.Execute(engine);
            delayInfo.RemoveAt(0);
        }

        /// <summary>
        /// 全ての遅延処理を終了させます。
        /// </summary>
        public virtual void EndAll()
        {
            foreach (DelayCommandInfo info in delayInfo)
            {
                info.Execute(engine, true);
                engine.MovementManager.EndAll();
            }

            delayInfo.Clear();
        }

        /// <summary>
        /// 更新を行います。
        /// </summary>
        public virtual void Update()
        {
            if (delayInfo.Count == 0)
            {
                return;
            }

            // 時間による遅延実行
            DelayInfo info = delayInfo[0];
            info.Update();
            if (info.IsExecuted)
            {
                info.Execute(engine);
                delayInfo.RemoveAt(0);
            }
        }

#endregion
    }
}
