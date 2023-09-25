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
    public class AdvCommandSingleText : AdvCommandBase
    {
#region constants

        /// <summary>有効化する場合の文字列</summary>
        protected const string Enable = "enable";

        /// <summary>無効化する場合の文字列</summary>
        protected const string Disable = "disable";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 2; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 2; } }

        /// <summary>有効にするかどうか</summary>
        protected string EnableParam { get { return Param[1]; } }

        /// <summary>テキストコマンドを単一のテキストとして認識するかどうか</summary>
        protected bool isEnable = true;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandSingleText(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            switch (EnableParam)
            {
                case Enable:
                    isEnable = true;
                    break;
                case Disable:
                    isEnable = false;
                    break;
                default:
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "有効、無効の指定が不正です。";
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
            engine.UIManager.SetSingleTextMode(isEnable);
            return true;
        }

#endregion
    }
}
