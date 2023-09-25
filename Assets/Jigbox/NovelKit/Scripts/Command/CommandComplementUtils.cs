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
    public static class CommandComplementUtils
    {
#region inner classes, enum, and structs

        /// <summary>
        /// コマンドの補完の種類
        /// </summary>
        public enum CommandComplementType
        {
            /// <summary>デフォルト設定に従う</summary>
            Default,
            /// <summary>実行する</summary>
            Execute,
            /// <summary>実行せず、スキップする</summary>
            Skip
        }

#endregion

#region constants

        /// <summary>補完の設定をデフォルト設定にする場合の文字列</summary>
        public const string ComplementDefault = "default";

        /// <summary>補完の設定をExecuteにする場合の文字列</summary>
        public const string ComplementExecute = "execute";

        /// <summary>補完の設定をSkipにする場合の文字列</summary>
        public const string ComplementSkip = "skip";

#endregion

#region public methods

        /// <summary>
        /// パラメータをパースして補完の種類を返します。
        /// </summary>
        /// <param name="param">コマンドのパラメータ</param>
        /// <param name="errorMessage">パースに失敗した場合のメッセージ</param>
        /// <param name="defaultType">補完の種類のデフォルト設定</param>
        /// <returns></returns>
        public static CommandComplementType Parse(
            string param,
            ref string errorMessage,
            CommandComplementType defaultType = CommandComplementType.Execute)
        {
            switch (param)
            {
                case ComplementDefault:
                    return defaultType;
                case ComplementExecute:
                    return CommandComplementType.Execute;
                case ComplementSkip:
                    return CommandComplementType.Skip;
                default:
                    errorMessage = "補完の指定が不正です。";
                    return CommandComplementType.Default;
            }
        }

        /// <summary>
        /// 補完時に実行するかどうかを返します。
        /// </summary>
        /// <param name="type">補完の種類</param>
        /// <param name="defaultType">補完の種類のデフォルト設定</param>
        /// <returns></returns>
        public static bool IsExecuteByComplement(
            CommandComplementType type,
            CommandComplementType defaultType = CommandComplementType.Execute)
        {
            if (type == CommandComplementType.Default)
            {
                UnityEngine.Assertions.Assert.AreNotEqual(CommandComplementType.Default, defaultType);
                type = defaultType;
            }

            return type == CommandComplementType.Execute;
        }

#endregion
    }
}
