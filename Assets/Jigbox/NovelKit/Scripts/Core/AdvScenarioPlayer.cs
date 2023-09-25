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
    using CommandType = AdvCommandBase.CommandType;
    using PlayState = AdvPlayStateManager.PlayState;

    public class AdvScenarioPlayer : MonoBehaviour, IAdvManagementComponent
    {
#region properties

        /// <summary>シナリオ制御統合コンポーネント</summary>
        protected AdvMainEngine engine;

        /// <summary>シナリオの状態管理コンポーネント</summary>
        protected AdvScenarioStatusManager statusManager;
        
        /// <summary>コマンドの遅延実行クラス</summary>
        public AdvCommandDelayManager DelayManager { get; protected set; }

        /// <summary>現在読み込まれているスクリプトに含まれるラベル毎のコマンドデータ</summary>
        protected Dictionary<string, List<AdvCommandBase>> scenarioCommands = new Dictionary<string, List<AdvCommandBase>>();

        /// <summary>現在読み込まれているシナリオのシーン名</summary>
        public string CurrentScene { get; protected set; }

        /// <summary>現在実行されているコマンドのインデックス</summary>
        public int CommandIndex { get; protected set; }

        /// <summary>終了するかどうか</summary>
        protected bool isEnd = false;

        /// <summary>コマンドを遅延実行するかどうか</summary>
        protected bool isDelay = false;

#endregion

#region public methods
        
        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="engine">シナリオ制御統合コンポーネント</param>
        public virtual void Init(AdvMainEngine engine)
        {
            this.engine = engine;
            statusManager = engine.StatusManager;
            DelayManager = new AdvCommandDelayManager();
            DelayManager.Init(engine);
        }

        /// <summary>
        /// 初期化前の状態に戻します。
        /// </summary>
        public virtual void Uninit()
        {
            statusManager = null;
            DelayManager = null;
            scenarioCommands.Clear();
            CurrentScene = string.Empty;
            CommandIndex = 0;
            isEnd = false;
            isDelay = false;
        }

        /// <summary>
        /// シーンを追加します。
        /// </summary>
        /// <param name="sceneName">追加するシナリオのシーン名</param>
        /// <param name="commands">コマンド</param>
        /// <returns></returns>
        public virtual bool AddScene(string sceneName, List<AdvCommandBase> commands)
        {
            if (scenarioCommands.ContainsKey(sceneName))
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("AdvScenarioPlayer.AddCommands : The label is already loaded!"
                    + "\n Scenario Label : " + sceneName);
#endif
                return false;
            }

            scenarioCommands.Add(sceneName, commands);
            return true;
        }

        /// <summary>
        /// シーンを破棄します。
        /// </summary>
        /// <param name="sceneName">破棄するシナリオのシーン名</param>
        /// <returns></returns>
        public virtual bool RemoveScene(string sceneName)
        {
            if (!scenarioCommands.ContainsKey(sceneName))
            {
                return false;
            }
            scenarioCommands.Remove(sceneName);
            return true;
        }

        /// <summary>
        /// 指定されたシーンからシナリオを再生します。
        /// </summary>
        /// <param name="sceneName">シナリオのシーン名</param>
        /// <returns></returns>
        public virtual bool StartScene(string sceneName)
        {
            if (!scenarioCommands.ContainsKey(sceneName))
            {
                return false;
            }

            CurrentScene = sceneName;
            CommandIndex = 0;
            isEnd = false;
            return true;
        }

        /// <summary>
        /// コマンドの遅延実行を有効にするかどうかを設定します。
        /// </summary>
        /// <param name="isEnable">遅延実行を有効にするかどうか</param>
        /// <returns></returns>
        public virtual void SetEnableDelay(bool isEnable)
        {
            isDelay = isEnable;
        }

        /// <summary>
        /// クリック待ちになるまでコマンドを実行します。
        /// </summary>
        public virtual void ExecuteToWaitClick()
        {
            // 既にコマンドが全て実行されているなら処理しない
            if (isEnd)
            {
                return;
            }

            bool isWaitClick = statusManager.State.IsWait(PlayState.WaitClick);
            while (!isWaitClick)
            {
                ExecuteNextCommand(true);
                // 全てのコマンドが実行されるとクリック待ち状態は発生しないので明示的に抜ける
                if (isEnd)
                {
                    return;
                }
                isWaitClick = statusManager.State.IsWait(PlayState.WaitClick);
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="isComplement">補完によるものかどうか</param>
        protected virtual void ExecuteNextCommand(bool isComplement = false)
        {
            // コマンドの実行前にそれ以前に実行されていたTweenが残っている場合、
            // 次のコマンドとバッティングして挙動が狂わないように、全て終了させる
            engine.MovementManager.EndAll();
            DelayManager.EndAll();

            if (isEnd)
            {
                engine.EndScenario();
                return;
            }

            List<AdvCommandBase> commands = scenarioCommands[CurrentScene];
            int commandCount = commands.Count;
            bool isContinual = true;
            bool isExecute = true;

            while (isContinual)
            {
                AdvCommandBase command = commands[CommandIndex];
                ++CommandIndex;

                // 変数ありのコマンドの場合は、変数の状態に合わせてコマンドを更新する
                if (command.HasVariables)
                {
                    if (!command.ApplyVariables(engine.VariableParamManager))
                    {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                        AdvLog.Error("不明な変数が指定されたことにより、正しく実行出来ませんでした。");
#endif
                        engine.EndScenario();
                        return;
                    }
                }

                switch (command.Type)
                {
                    case CommandType.If:
                        isExecute = command.Execute(engine);
                        break;
                    case CommandType.Else:
                        isExecute = !isExecute;
                        break;
                    case CommandType.EndIf:
                        isExecute = true;
                        break;
                    default:
                        if (isExecute)
                        {
                            bool isWaitReserved = statusManager.State.IsWait(PlayState.Select, true);
                            // 選択肢待ち状態が予約になっている状態で選択肢以外のコマンドを実行しようとした場合、
                            // 選択肢待ち状態を確定させて、処理を中断
                            if (isWaitReserved && command.Type != CommandType.Select)
                            {
                                statusManager.State.ConfirmReservation(PlayState.Select);
                                isContinual = false;
                                break;
                            }

                            // 遅延状態になっている間は、すぐにコマンドを実行せずにコマンドをスタックする
                            // ただし、遅延実行の終了コマンドの場合は、遅延状態を解除するために実行する
                            if (!isDelay || command.Type == CommandType.EndDelay)
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
                                    isContinual = command.Execute(engine);
                                }
                            }
                            else
                            {
                                DelayManager.AddCommand(command);
                            }
                        }
                        break;
                }

                // 該当ラベルにおける全てのコマンドが終了
                if (CommandIndex >= commandCount)
                {
                    // 選択肢待ち状態の予約があれば確定
                    // 選択肢待ち状態になっている場合は、終了扱いにはしない
                    statusManager.State.ConfirmReservation(PlayState.Select);
                    if (statusManager.State.IsWait(PlayState.Select))
                    {
                        break;
                    }

                    // コマンドが連続実行されようとしている場合、即時終了
                    if (isContinual)
                    {
                        engine.EndScenario();
                    }
                    else
                    {
                        isEnd = true;
                    }
                    break;
                }
            }
        }

#endregion

#region override unity methods

        void Update()
        {
            if (!engine.IsPlaying)
            {
                return;
            }

            DelayManager.Update();

            if (statusManager.IsExecute)
            {
                ExecuteNextCommand();
            }
        }

#endregion
    }
}
