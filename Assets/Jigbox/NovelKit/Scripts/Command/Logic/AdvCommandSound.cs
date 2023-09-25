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
    using CommandComplementType = CommandComplementUtils.CommandComplementType;

    public class AdvCommandSound : AdvCommandBase, IAdvCommandSoundResource, IAdvCommandControlComplement
    {
#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 2; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 3; } }

        /// <summary>サウンド情報</summary>
        protected string Sound { get { return Param[1]; } }

        /// <summary>コマンドの補完の種類</summary>
        protected virtual string ComplementType { get { return Param[2]; } }

        /// <summary>コマンドの補完の種類が存在するかどうか</summary>
        protected virtual bool IsExistComplementType { get { return Param.Length == MaxParameterCount; } }

        /// <summary>コマンドで使用するサウンドのリソース</summary>
        public virtual string SoundResource { get { return Sound; } }

        /// <summary>コマンドの補完の種類</summary>
        protected CommandComplementType complementType;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandSound(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            if (!IsExistComplementType)
            {
                return;
            }

            string errorMessage = null;
            complementType = CommandComplementUtils.Parse(
                ComplementType,
                ref errorMessage,
                CommandComplementType.Default);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ErrorMessage = errorMessage;
                Type = CommandType.ErrorCommand;
            }
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            engine.SoundManager.Post(Sound);
            return true;
        }
        
        /// <summary>
        /// 補完のためにコマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public virtual bool ExecuteByComplement(AdvMainEngine engine)
        {
            CommandComplementType defautType = engine.Settings.EngineSetting.IsSkipSoundCommandComplement ?
                CommandComplementType.Skip : CommandComplementType.Execute;

            if (!CommandComplementUtils.IsExecuteByComplement(complementType, defautType))
            {
                return true;
            }

            return Execute(engine);
        }

#endregion
    }
}
