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
    public class AdvCommandFileLoad : AdvCommandBase, IAdvCommandDefinitionResource
    {
#region inner classes, enum, and structs

        /// <summary>
        /// ファイルの種類
        /// </summary>
        public enum LoadFileType
        {
            None,
            /// <summary>マクロファイル</summary>
            Macro,
            /// <summary>定数ファイル</summary>
            ConstantValue,
        }

#endregion

#region constants

        /// <summary>マクロファイルを読み込む際の指定文字列</summary>
        protected const string FileTypeMacro = "macro";

        /// <summary>定数ファイルを読み込む際の指定文字列</summary>
        protected const string FileTypeConstantValue = "const";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 2; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 2; } }

        /// <summary>ファイルのパス</summary>
        public string FilePath { get { return Param[1]; } }

        /// <summary>読み込むファイルの種類</summary>
        public LoadFileType FileType = LoadFileType.None;

        /// <summary>コマンドで使用するリソース</summary>
        public string DefinitionResource { get { return FilePath; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandFileLoad(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            switch (Param[0].Substring(1))
            {
                case FileTypeMacro:
                    FileType = LoadFileType.Macro;
                    break;
                case FileTypeConstantValue:
                    FileType = LoadFileType.ConstantValue;
                    break;
            }

            if (FileType == LoadFileType.None)
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "読み込むファイルの指定が不正です。";
            }
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            // 初回実行時は、基本的にはこの時点ではすでに読み込みが終わっている前提
            // スクリプトを読み込んだままシーン変更等で複数回読む際に、マクロ等を破棄していた場合の対策
            switch (FileType)
            {
                case LoadFileType.Macro:
                    break;
                case LoadFileType.ConstantValue:
                    engine.LoadConstantValues(FilePath);
                    break;
            }
            return true;
        }

#endregion
    }
}
