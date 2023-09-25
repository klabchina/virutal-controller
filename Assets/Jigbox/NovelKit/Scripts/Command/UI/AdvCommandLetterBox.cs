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
    public class AdvCommandLetterBox : AdvCommandBase
    {
#region constants

        /// <summary>表示する場合の文字列</summary>
        protected const string Show = "show";

        /// <summary>非表示する場合の文字列</summary>
        protected const string Hide = "hide";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 3; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 5; } }

        /// <summary>有効にするかどうか</summary>
        protected string ShowParam { get { return Param[1]; } }

        /// <summary>時間</summary>
        protected string TimeParam { get { return Param[2]; } }

        /// <summary>サイズ指定</summary>
        protected string SizeParam { get { return Param[3]; } }

        /// <summary>プレーンのレベル</summary>
        protected string PlaneLevelParam { get { return Param[4]; } }

        /// <summary>表示するどうか</summary>
        protected bool isShow = true;

        /// <summary>時間</summary>
        protected float transitionTime = 0.0f;

        /// <summary>サイズ指定がピクセルかどうか</summary>
        protected bool isPixel = true;

        /// <summary>サイズ</summary>
        protected float size = 0.0f;

        /// <summary>プレーンのレベル</summary>
        protected int level = 1;

        /// <summary>ブラインドの種類</summary>
        protected AdvLetterBoxController.BlindType blindType = AdvLetterBoxController.BlindType.Letter;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandLetterBox(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            switch (ShowParam)
            {
                case Show:
                    isShow = true;
                    break;
                case Hide:
                    isShow = false;
                    break;
                default:
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "表示状態の指定が不正です。";
                    return;
            }

            if (!float.TryParse(TimeParam, out transitionTime))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "時間の指定が不正です。";
                return;
            }
            else if (transitionTime < 0.0f)
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "時間に負数は設定できません。";
                return;
            }

            if (!isShow)
            {
                return;
            }

            // 終端が%の場合、割合指定
            isPixel = SizeParam[SizeParam.Length - 1] != '%';
            
            if (isPixel)
            {
                if (!float.TryParse(SizeParam, out size))
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "サイズの指定が不正です。";
                    return;
                }
            }
            else
            {
                if (!float.TryParse(SizeParam.Substring(0, SizeParam.Length - 1), out size))
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "サイズの指定が不正です。";
                    return;
                }

                // %指定から、実際に扱う0~1の数値にキャスト
                size /= 100.0f;
            }

            if (size <= 0.0f)
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "サイズに0以下の数値は指定できません。";
            }

            if (!int.TryParse(PlaneLevelParam, out level))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "プレーンのレベルが不正です。";
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
            AdvLetterBoxManager manager = engine.LetterBoxManager;
            if (manager.IsShow == isShow)
            {
                return true;
            }

            if (isShow)
            {
                if (!engine.PlaneManager.IsValidLevel(level))
                {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                    AdvLog.Error("無効なレベルが指定されているため、プレーンを設定できません。");
#endif
                    return true;
                }

                AdvPlaneController plane = engine.PlaneManager.GetPlane(level);
                if (plane.RectTransform == null)
                {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                    AdvLog.Error("Canvasを持たないプレーンでは使用できません。");
#endif
                    return true;
                }

                manager.Show(plane.LocalTransform.gameObject, blindType, GetScaledTime(engine, transitionTime), size, isPixel);
            }
            else
            {
                manager.Hide(GetScaledTime(engine, transitionTime));
            }
            return true;
        }

#endregion
    }
}
