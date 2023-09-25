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

namespace Jigbox.NovelKit
{
    public static class AdvDefaultCommandParser
    {
#region constants

#region commands
        
        /// <summary>シーンの移動</summary>
        public const string CommandGoto = "goto";

        /// <summary>選択肢</summary>
        public const string CommandSelect = "select";

        /// <summary>時間指定待機</summary>
        public const string CommandWait = "wait";

        /// <summary>クリック待ち</summary>
        public const string CommandWaitClick = "waitclick";

        /// <summary>テキスト表示待ち</summary>
        public const string CommandWaitText = "waittext";

        /// <summary>リソースの事前読み込み待ち</summary>
        public const string CommandWaitLoad = "waitload";

        /// <summary>中断状態コマンド</summary>
        public const string CommandSuspend = "suspend";

        /// <summary>中断状態解除コマンド</summary>
        public const string CommandResume = "resume";

        /// <summary>コマンドの遅延実行を開始</summary>
        public const string CommandDelay = "delay";

        /// <summary>コマンドの遅延実行を終了</summary>
        public const string CommandEndDelay = "enddelay";

        /// <summary>条件分岐コマンド:条件一致</summary>
        public const string CommandIf = "if";

        /// <summary>条件分岐コマンド:条件不一致</summary>
        public const string CommandElse = "else";

        /// <summary>条件分岐コマンド：終了</summary>
        public const string CommandEndIf = "endif";

        /// <summary>サウンド命令</summary>
        public const string CommandSound = "sound";

        /// <summary>フェードコマンド</summary>
        public const string CommandFade = "fade";

        /// <summary>ウィンドウ切り替えコマンド</summary>
        public const string CommandChangeWindow = "changewindow";

        /// <summary>ウィンドウ表示切り替えコマンド</summary>
        public const string CommandWindow = "window";

        /// <summary>サムネイル状態変更コマンド</summary>
        public const string CommandThumbnail = "thumbnail";
        
        /// <summary>テキストのクリアコマンド</summary>
        public const string CommandClearText = "clear";

        /// <summary>テキストの表示モード変更コマンド</summary>
        public const string CommandTextMode = "textmode";

        /// <summary>テキストコマンドの単一認識化コマンド</summary>
        public const string CommandSingleText = "singletext";

        /// <summary>テキストの文字送り速度指定コマンド</summary>
        public const string CommandTextSpeed = "textspeed";

        /// <summary>入力制御コマンド</summary>
        public const string CommandClick = "click";

        /// <summary>リソースの事前読み込みコマンド</summary>
        public const string CommandPreload = "preload";

        /// <summary>動作の継続、停止コマンド</summary>
        public const string CommandMovement = "movement";

        /// <summary>LetterBoxの表示切り替えコマンド</summary>
        public const string CommandLetterBox = "letterbox";

        /// <summary>画像付きLetterBoxの表示切り替えコマンド</summary>
        public const string CommandLetterImage = "letterimage";

        /// <summary>PillerBoxの表示切替コマンド</summary>
        public const string CommandPillerBox = "pillerbox";

        /// <summary>画像付きPillerBoxの表示切替コマンド</summary>
        public const string CommandPillerImage = "pillerimage";

        /// <summary>管理外オブジェクトへのメッセージコマンド</summary>
        public const string CommandMessage = "message";

        /// <summary>画面効果切り替えコマンド</summary>
        public const string CommandEffection = "effection";

        /// <summary>ラップタイムの記録コマンド</summary>
        public const string CommandLap = "lap";

        /// <summary>デバッグ表示用のコマンド</summary>
        public const string CommandDebugPrint = "debugprint";

#endregion

#endregion

#region public methods

        /// <summary>
        /// オブジェクト用以外の基礎定義コマンドとしてパースを行います。
        /// </summary>
        /// <param name="commandBase">パース元のコマンド</param>
        /// <returns>パースに成功した場合、正規のコマンドが返り、不正コマンドの場合はエラーが返ります。</returns>
        public static AdvCommandBase Parse(AdvCommandBase commandBase)
        {
            AdvCommandBase command = null;
            switch (commandBase.Param[0])
            {
                case CommandGoto:
                    command = new AdvCommandGoto(commandBase);
                    break;
                case CommandSelect:
                    command = new AdvCommandSelect(AdvScriptParams.Join(commandBase.BaseParam));
                    break;
                case CommandWait:
                    command = new AdvCommandWait(commandBase);
                    break;
                case CommandWaitClick:
                    command = new AdvCommandWaitClick(commandBase);
                    break;
                case CommandWaitText:
                    command = new AdvCommandWaitText(commandBase);
                    break;
                case CommandWaitLoad:
                    command = new AdvCommandWaitLoad(commandBase);
                    break;
                case CommandSuspend:
                    command = new AdvCommandSuspend(commandBase);
                    break;
                case CommandResume:
                    command = new AdvCommandResume(commandBase);
                    break;
                case CommandDelay:
                    command = new AdvCommandDelay(commandBase);
                    break;
                case CommandEndDelay:
                    command = new AdvCommandEndDelay(commandBase);
                    break;
                case CommandIf:
                    command = new AdvCommandIf(commandBase);
                    break;
                case CommandElse:
                    command = new AdvCommandElse(commandBase);
                    break;
                case CommandEndIf:
                    command = new AdvCommandEndIf(commandBase);
                    break;
                case CommandSound:
                    command = new AdvCommandSound(commandBase);
                    break;
                case CommandFade:
                    command = new AdvCommandFade(commandBase);
                    break;
                case CommandChangeWindow:
                    command = new AdvCommandChangeWindow(commandBase);
                    break;
                case CommandWindow:
                    command = new AdvCommandWindow(commandBase);
                    break;
                case CommandThumbnail:
                    command = new AdvCommandThumbnail(commandBase);
                    break;
                case CommandClearText:
                    command = new AdvCommandClearText(commandBase);
                    break;
                case CommandTextMode:
                    command = new AdvCommandTextMode(commandBase);
                    break;
                case CommandSingleText:
                    command = new AdvCommandSingleText(commandBase);
                    break;
                case CommandTextSpeed:
                    command = new AdvCommandTextSpeed(commandBase);
                    break;
                case CommandClick:
                    command = new AdvCommandClick(commandBase);
                    break;
                case CommandPreload:
                    command = new AdvCommandPreload(commandBase);
                    break;
                case CommandMovement:
                    command = new AdvCommandMovement(commandBase);
                    break;
                case CommandLetterBox:
                    command = new AdvCommandLetterBox(commandBase);
                    break;
                case CommandLetterImage:
                    command = new AdvCommandLetterImage(commandBase);
                    break;
                case CommandPillerBox:
                    command = new AdvCommandPillerBox(commandBase);
                    break;
                case CommandPillerImage:
                    command = new AdvCommandPillerImage(commandBase);
                    break;
                case CommandMessage:
                    command = new AdvCommandMessage(commandBase);
                    break;
                case CommandEffection:
                    command = new AdvCommandEffection(commandBase);
                    break;
                case CommandLap:
                    command = new AdvCommandLap(commandBase);
                    break;
                case CommandDebugPrint:
                    command = new AdvCommandDebugPrint(AdvScriptParams.Join(commandBase.BaseParam));
                    break;
            }
            
            if (command == null)
            {
                command = new AdvCommandText(AdvScriptParams.Join(commandBase.BaseParam));
            }

            return command;
        }

#endregion
    }
}
