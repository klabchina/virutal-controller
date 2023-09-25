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
    public class AdvCommandSelect : AdvCommandGoto
    {
#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 3; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 4; } }

        /// <summary>選択肢に表示する文言</summary>
        protected string TextParam { get { return Param[1]; } set { Param[1] = value; } }

        /// <summary>選択肢が選択された際に再生するシーン名</summary>
        protected override string SceneNameParam { get { return Param[2]; } }

        /// <summary>選択肢が選択された際に読み込むスクリプト</summary>
        protected override string ScriptFileParam { get { return Param.Length > MinParameterCount ? Param[3] : string.Empty; } }

        /// <summary>シナリオの統合管理コンポーネント</summary>
        protected AdvMainEngine engine;

        /// <summary>選択肢が連続しているかどうか</summary>
        protected bool isContinualSelect = false;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandSelect(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            Type = CommandType.Select;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandSelect(string str) : base(str)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            Type = CommandType.Select;
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            this.engine = engine;

            Object resource = engine.Loader.Load<Object>(engine.Settings.EngineSetting.SelectResourcePath);
            if (resource == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("選択肢の元となるリソースが存在しません。"
                + "\nパス : " + engine.Settings.EngineSetting.SelectResourcePath);
#endif
                return isContinualSelect;
            }

            GameObject obj = Object.Instantiate(resource) as GameObject;
            AdvSelectItemController item = obj.GetComponent<AdvSelectItemController>();
            if (item == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("生成したオブジェクトに制御用コンポーネントがアタッチされていません。");
#endif
                return isContinualSelect;
            }

            item.Init(engine.Localizer.Resolve(TextParam), this);
            engine.UIManager.SelectManager.AddSelect(item);
            // 選択肢が連続して続いている場合は選択肢待ちの予約のみ
            // 選択肢の最後で待機状態を確定させる
            if (isContinualSelect)
            {
                engine.StatusManager.State.Wait(AdvPlayStateManager.PlayState.Select, isContinualSelect);
            }
            else
            {
                engine.StatusManager.State.Wait(AdvPlayStateManager.PlayState.Select, isContinualSelect);
                engine.StatusManager.State.ConfirmReservation(AdvPlayStateManager.PlayState.Select);
            }

            return isContinualSelect;
        }

        /// <summary>
        /// 選択肢が連続していることを設定します。
        /// </summary>
        public void EnableContinual()
        {
            isContinualSelect = true;
        }

        /// <summary>
        /// 設定されているシーンを再生します。
        /// </summary>
        public void ChangeScene()
        {
            if (engine != null)
            {
                ChangeScene(engine);
                engine.StatusManager.State.WaitRelease(AdvPlayStateManager.PlayState.Select);
            }
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
