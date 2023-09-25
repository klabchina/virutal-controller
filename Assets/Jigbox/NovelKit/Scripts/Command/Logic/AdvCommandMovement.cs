﻿/**
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

using Jigbox.Tween;

namespace Jigbox.NovelKit
{
    public class AdvCommandMovement : AdvCommandBase
    {
#region constants

        /// <summary>表示する場合の文字列</summary>
        protected const string Keep = "keep";

        /// <summary>非表示する場合の文字列</summary>
        protected const string End = "end";

        /// <summary>再スタートする場合の文字列</summary>
        protected const string Restart = "loop";

        /// <summary>跳ね返るように戻す場合の文字列</summary>
        protected const string PingPong = "bound";

        /// <summary>巻き戻るように戻す場合の文字列</summary>
        protected const string Yoyo = "rewind";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 3; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 4; } }

        /// <summary>継続するかどうか</summary>
        protected string KeepParam { get { return Param[1]; } }

        /// <summary>継続する動作の識別名</summary>
        protected string MovementNameParam { get { return Param[2]; } }

        /// <summary>継続する動作のループ設定</summary>
        protected string LoopModeParam { get { return Param[3]; } }

        /// <summary>継続するかどうか</summary>
        protected bool isKeep = true;

        /// <summary>ループ設定</summary>
        protected LoopMode loopMode = LoopMode.NoLoop;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandMovement(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            switch (KeepParam)
            {
                case Keep:
                    isKeep = true;
                    break;
                case End:
                    isKeep = false;
                    break;
                default:
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "状態の指定が不正です。";
                    return;
            }

            if (Param.Length < MaxParameterCount)
            {
                return;
            }

            switch (LoopModeParam)
            {
                case Restart:
                    loopMode = LoopMode.Restart;
                    break;
                case PingPong:
                    loopMode = LoopMode.PingPong;
                    break;
                case Yoyo:
                    loopMode = LoopMode.Yoyo;
                    break;
                default:
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "ループ設定の指定が不正です。";
                    break;
            }
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            if (isKeep)
            {
                engine.MovementManager.SetKeepMovementName(MovementNameParam, loopMode);
            }
            else
            {
                engine.MovementManager.EndKeepMovement(MovementNameParam);
            }
            return true;
        }

#endregion
    }
}