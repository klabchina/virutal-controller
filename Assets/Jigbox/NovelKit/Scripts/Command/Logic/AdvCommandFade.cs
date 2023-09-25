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
    public class AdvCommandFade : AdvCommandBase
    {
#region constants

        /// <summary>フェードインさせる際のパラメータ</summary>
        protected const string FadeInParameter = "in";
        
        /// <summary>フェードアウトさせる際のパラメータ</summary>
        protected const string FadeOutParameter = "out";
        
        /// <summary>UIより前面でフェードさせる際のパラメータ</summary>
        protected const string FrontPositionParameter = "front";

        /// <summary>UIより背面でフェードさせる際のパラメータ</summary>
        protected const string BackPositionParameter = "back";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 4; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 6; } }

        /// <summary>フェードの種類</summary>
        protected string FadeTypeParam { get { return Param[1]; } }

        /// <summary>フェードさせる位置</summary>
        protected string FadePositionParam { get { return Param[2]; } }

        /// <summary>時間のパラメータ</summary>
        protected string TimeParam { get { return Param[3]; } }

        /// <summary>色情報のパラメータ(</summary>
        protected string ColorParam { get { return Param.Length > 4 ? Param[4] : string.Empty; } }

        /// <summary>コマンドを連続処理するかどうかのパラメータ</summary>
        protected string ContinualParam { get { return Param.Length > 5 ? Param[5] : string.Empty; } }

        /// <summary>フェードインかどうか</summary>
        protected bool isFadeIn = true;

        /// <summary>UIより前面に表示するかどうか</summary>
        protected bool isPositionFront = true;
        
        /// <summary>フェードの時間</summary>
        protected float time = 0.0f;

        /// <summary>フェードの色</summary>
        protected Color color = Color.black;

        /// <summary>コマンドを連続処理するかどうか</summary>
        protected bool isContinual = false;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandFade(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            Type = CommandType.Fade;

            switch (FadeTypeParam)
            {
                case FadeInParameter:
                    isFadeIn = true;
                    break;
                case FadeOutParameter:
                    isFadeIn = false;
                    break;
                default:
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "フェードの種類の指定が不正です。";
                    return;
            }

            switch (FadePositionParam)
            {
                case FrontPositionParameter:
                    isPositionFront = true;
                    break;
                case BackPositionParameter:
                    isPositionFront = false;
                    break;
                default:
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "フェードさせる位置の指定が不正です。";
                    return;
            }

            if (float.TryParse(TimeParam, out time))
            {
                if (time < 0.0f)
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "時間の指定が不正です。";
                    return;
                }
            }

            if (string.IsNullOrEmpty(ColorParam))
            {
                return;
            }
            if (!ColorUtility.TryParseHtmlString(ColorParam, out color))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "色の指定が不正です。";
                return;
            }
            
            if (string.IsNullOrEmpty(ContinualParam))
            {
                return;
            }

            isContinual = ContinualParam == ContinualCommandParameter;
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            engine.FadeManager.BeginFade(GetScaledTime(engine, time), color, isFadeIn, isPositionFront);
            if (!isContinual && time > 0.0f)
            {
                engine.StatusManager.State.WaitTime(GetScaledTime(engine, time));
            }

            return isContinual;
        }

#endregion
    }
}
