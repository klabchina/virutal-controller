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
using System;

namespace Jigbox.NovelKit
{
    using CameraType = AdvCameraManager.CameraType;

    public class AdvCommandEffection : AdvCommandBase
    {
#region constants

        /// <summary>有効化する場合の文字列</summary>
        protected const string Enable = "enable";

        /// <summary>無効化する場合の文字列</summary>
        protected const string Disable = "disable";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 2; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 3; } }

        /// <summary>有効にするかどうか</summary>
        protected string EnableParam { get { return Param[1]; } }

        /// <summary>画面効果として使用するカメラの種類</summary>
        protected string CameraTypeParam { get { return Param[2]; } }

        /// <summary>画面効果として使用するカメラの種類</summary>
        protected CameraType cameraType;

        /// <summary>画面効果を有効化するかどうか</summary>
        protected bool isEnable = true;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandEffection(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            switch (EnableParam)
            {
                case Enable:
                    isEnable = true;
                    break;
                case Disable:
                    isEnable = false;
                    break;
                default:
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "有効、無効の指定が不正です。";
                    break;
            }

            if (isEnable)
            {
                if (Param.Length == MinParameterCount)
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "画面効果を有効化する場合、対象カメラの指定は必須です。";
                    return;
                }
            }
            else
            {
                if (Param.Length == MaxParameterCount)
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "画面効果を無効化する場合、対象カメラの指定は不要です。";
                }
                return;
            }

            try
            {
                cameraType = (CameraType) Enum.Parse(typeof(CameraType), CameraTypeParam);
                if (cameraType == CameraType.UI)
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "UI用カメラは指定できません。";
                    return;
                }
            }
#pragma warning disable 168
            catch (ArgumentException e)
#pragma warning restore 168
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "対象カメラの指定が不正です。";
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
            if (!engine.Settings.EngineSetting.UseScreenEffection)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("画面効果の設定が無効になっているため、画面効果に関するコマンドは無効です。");
#endif
                return true;
            }

            engine.ScreenEffectionManager.Enable = isEnable;
            if (isEnable)
            {
                engine.CameraManager.SetOffscreenTarget(cameraType);
            }
            return true;
        }

#endregion
    }
}
