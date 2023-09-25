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
    public class AdvCommandGoto : AdvCommandBase, IAdvCommandScriptResource
    {
#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 2; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 3; } }

        /// <summary>再生するシーン名</summary>
        protected virtual string SceneNameParam { get { return Param[1]; } }

        /// <summary>選択肢が選択された際に読み込むスクリプト</summary>
        protected virtual string ScriptFileParam { get { return Param.Length > MinParameterCount ? Param[2] : string.Empty; } }

        /// <summary>コマンドで使用するリソース</summary>
        public string ScriptResource { get { return ScriptFileParam; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandGoto(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            Type = CommandType.GoTo;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="str">元となる文字列</param>
        public AdvCommandGoto(string str) : base(str)
        {
            // こちらのコンストラクタでは自動で呼び出されないので、明示的に呼び出す
            IsValidParameterCount();

            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            Type = CommandType.GoTo;
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            ChangeScene(engine);
            return false;
        }

#endregion

#region protected methods

        /// <summary>
        /// 設定されているシーンを再生します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns></returns>
        protected virtual bool ChangeScene(AdvMainEngine engine)
        {
            if (!string.IsNullOrEmpty(ScriptFileParam))
            {
                if (string.IsNullOrEmpty(engine.LoadScenario(ScriptFileParam)))
                {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                    AdvLog.Error("選択肢で読み込もうとしたスクリプトが正しく読み込めませんでした。"
                    + "\nスクリプト : " + ScriptFileParam);
#endif
                    return false;
                }
            }

            bool result = engine.Player.StartScene(SceneNameParam);
#if UNITY_EDITOR || NOVELKIT_DEBUG
            if (!result)
            {
                AdvLog.Error("指定されたシーンが存在しないため、再生できません。"
                + "\nシーン名 : " + SceneNameParam);
            }
#endif
            return result;
        }

#endregion
    }
}
