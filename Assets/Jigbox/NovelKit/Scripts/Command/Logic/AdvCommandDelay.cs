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
    public class AdvCommandDelay : AdvCommandBase
    {
#region constants

        /// <summary>サウンド内マーカーでの遅延実行指定文字列</summary>
        protected static readonly string MarkerString = "marker";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 2; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 2; } }

        /// <summary>コマンドの遅延時間</summary>
        protected string DelayTimeParam { get { return Param[1]; } }

        /// <summary>コマンドの遅延時間</summary>
        protected float delayTime = 0.0f;

        /// <summary>サウンド内マーカーによる遅延実行かどうか</summary>
        protected bool isMarkerDelay = false;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandDelay(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            Type = CommandType.Delay;

            if (DelayTimeParam == MarkerString)
            {
                isMarkerDelay = true;
                return;
            }

            if (!float.TryParse(DelayTimeParam, out delayTime))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "時間の指定が不正です。";
                return;
            }
            if (delayTime <= 0.0f)
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "時間に0以下の値を設定することはできません。";
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
            if (!isMarkerDelay)
            {
                engine.Player.DelayManager.AddDelay(GetScaledTime(engine, delayTime));
            }
            else
            {
                engine.Player.DelayManager.AddMarkerDelay();
            }
            engine.Player.SetEnableDelay(true);
            return true;
        }

#endregion
    }
}
