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
    public class AdvCommandDebugPrint : AdvCommandBase
    {
#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 3; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 3; } }

        /// <summary>タグ</summary>
        protected string Tag { get { return Param[1]; } }

        /// <summary>出力内容</summary>
        protected string Data { get { return Param[2]; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandDebugPrint(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            Type = CommandType.Debug;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となる文字列</param>
        public AdvCommandDebugPrint(string str) : base(str)
        {
            // こちらのコンストラクタでは自動で呼び出されないので、明示的に呼び出す
            IsValidParameterCount();
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            Type = CommandType.Debug;
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            AdvDebugStatusManager debugStatusManager = engine.DebugStatusManager;
            if (debugStatusManager != null)
            {
                debugStatusManager.DebugStatusView.Set(Tag, Data);
            }
            return true;
        }

#endregion

#region protected methods

        /// <summary>
        /// 文字列からパラメータを展開します。
        /// </summary>
        /// <param name="str"></param>
        protected override void Parse(string str)
        {
            ParseWithPacking(str);
        }

#endregion
    }
}
