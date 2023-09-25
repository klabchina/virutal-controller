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
    public class AdvCommandWindow : AdvCommandBase
    {
#region constants

        /// <summary>表示する場合の文字列</summary>
        protected const string Show = "show";

        /// <summary>非表示する場合の文字列</summary>
        protected const string Hide = "hide";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 2; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 2; } }

        /// <summary>有効にするかどうか</summary>
        protected string ShowParam { get { return Param[1]; } }

        /// <summary>ウィンドウを表示するどうか</summary>
        protected bool isShow = true;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandWindow(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            switch (ShowParam)
            {
                case Show:
                    isShow = true;
                    break;
                case Hide:
                    isShow = false;
                    break;
                default:
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "表示状態の指定が不正です。";
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
            if (isShow)
            {
                engine.UIManager.ShowWindow();
            }
            else
            {
                engine.UIManager.HideWindow();
            }
            return true;
        }

#endregion
    }
}
