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
    public class AdvCommandThumbnail : AdvCommandBase
    {
#region inner classes, enum, and structs

        protected enum Status
        {
            /// <summary>表示</summary>
            Show,
            /// <summary>非表示</summary>
            Hide,
            /// <summary>読み込み</summary>
            Load,
        }

#endregion

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

        /// <summary>表示パラメータ</summary>
        protected string ShowParam { get { return Param[1]; } }

        /// <summary>コマンドによる操作状態</summary>
        protected Status status = Status.Show;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandThumbnail(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            switch (ShowParam)
            {
                case Show:
                    status = Status.Show;
                    break;
                case Hide:
                    status = Status.Hide;
                    break;
                default:
                    status = Status.Load;
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
            AdvThumbnailViewController thumbnail = engine.UIManager.WindowManager.ActiveWindow.ThumbnailViewController;
            if (thumbnail == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("サムネイルの参照がないため、サムネイルの状態を変更できません。");
#endif
                return true;
            }

            switch (status)
            {
                case Status.Show:
                    thumbnail.Show();
                    break;
                case Status.Hide:
                    thumbnail.Hide();
                    break;
                case Status.Load:
                    thumbnail.LoadResource(engine.Loader, ShowParam);
                    thumbnail.Show();
                    break;
            }
            return true;
        }

#endregion
    }
}
