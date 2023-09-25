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
    public class AdvCommandLetterImage : AdvCommandLetterBox
    {
#region properties

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 6; } }

        /// <summary>使用する画像のパス</summary>
        protected string ImagePathParam { get { return Param[5]; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandLetterImage(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            if (isShow && Param.Length < MaxParameterCount)
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "画像のパスが指定されていません。";
            }
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            AdvLetterBoxManager manager = engine.LetterBoxManager;
            if (manager.IsShow == isShow)
            {
                return true;
            }

            base.Execute(engine);

            if (isShow)
            {
                manager.SetBlindImage(engine.Loader.Load<Sprite>(ImagePathParam));                
            }

            return true;
        }

#endregion
    }
}
