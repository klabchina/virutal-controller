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
    public class AdvCommandMessage : AdvCommandBase
    {
#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 3; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return MinParameterCount; } }

        /// <summary>同種のオブジェクトの全てが対象かどうか</summary>
        protected bool IsAll { get { return Param[1] == AdvObjectCommandParser.IndexAll; } }

        /// <summary>対象名のパラメータ</summary>
        protected string TargetNameParam { get { return Param[1]; } }

        /// <summary>メッセージ内容のパラメータ</summary>
        protected string MessageParam { get { return Param[2]; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandMessage(AdvCommandBase command)
            : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
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
            if (IsAll)
            {
                foreach (IAdvReferencedObject target in engine.ObjectManager.GetAllReferencedObject())
                {
                    target.ReceiveMessage(MessageParam);
                }
            }
            else
            {
                IAdvReferencedObject target = engine.ObjectManager.GetReferencedObject(TargetNameParam);
                if (target != null)
                {
                    target.ReceiveMessage(MessageParam);
                }
#if UNITY_EDITOR || NOVELKIT_DEBUG
                else
                {
                    AdvLog.Error("対象のオブジェクト(" + TargetNameParam + ")が見つかりませんでした。");
                }
#endif
            }
            return true;
        }

#endregion
    }
}
