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

using System;

namespace Jigbox.NovelKit
{
    using TextMode = AdvWindowManager.TextMode;

    public class AdvCommandTextMode : AdvCommandBase
    {
#region protected methods

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 2; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 2; } }

        /// <summary>テキストの表示モードのパラメータ</summary>
        protected string ModeParam { get { return Param[1]; } }

        /// <summary>テキストの表示モード</summary>
        protected TextMode mode = TextMode.Normal;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandTextMode(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            try
            {
                mode = (TextMode) Enum.Parse(typeof(TextMode), ModeParam);
            }
#pragma warning disable 168
            catch (ArgumentException e)
#pragma warning restore 168
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "テキストの表示モードの指定が不正です。";
                return;
            }
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            engine.UIManager.WindowManager.SetTextMode(mode);
            return false;
        }

#endregion
    }
}
