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
    public class AdvCommandTextSpeed : AdvCommandBase
    {
#region constants

        /// <summary>デフォルトの文字送り速度の指定文字列</summary>
        protected static readonly string CaptionSpeedDefault = "default";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 2; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 3; } }

        /// <summary>文字送り速度</summary>
        protected string CaptionMarginParam { get { return Param[1]; } }

        /// <summary>オートモードの表示速度を変更するか</summary>
        protected bool ChangeCaptionMarginAuto { get { return Param.Length == MaxParameterCount &&
                                             Param[MaxParameterCount - 1] == "auto"; } }

        /// <summary>文字送り速度</summary>
        protected float captionMargin = 0.0f;

        /// <summary>デフォルトの速度を指定するかどうか</summary>
        protected bool isDefault = false;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandTextSpeed(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            if (CaptionMarginParam == CaptionSpeedDefault)
            {
                isDefault = true;
                return;
            }
            
            if (!float.TryParse(CaptionMarginParam, out captionMargin))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "文字送り速度の指定が不正です。";
                return;
            }
            if (captionMargin <= 0.0f)
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "文字送り速度に0以下の値を設定することはできません。";
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
            if (isDefault)
            {
                captionMargin = !engine.StatusManager.Mode.IsAuto ?
                    engine.Settings.EngineSetting.TextCaptionMargin : engine.Settings.EngineSetting.TextCaptionMarginAuto;
            }

            var playMode = !ChangeCaptionMarginAuto ?
                AdvPlayModeManager.PlayMode.Normal : AdvPlayModeManager.PlayMode.Auto;

            engine.UIManager.SetTextCaptionSpeed(playMode, captionMargin);

            if (engine.StatusManager.Mode.CurrentMode == playMode)
            {
                engine.UIManager.ApplyTextCaptionspeed(playMode);
            }
            return false;
        }

#endregion
    }
}
