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

using System;
using Jigbox.Tween;

namespace Jigbox.NovelKit
{
    using ObjectType = AdvObjectBase.ObjectType;

    public class AdvCommandObjectTween : AdvCommandTransformBase
    {
#region constants

        /// <summary>指定なしの場合の文字列</summary>
        public static readonly string NoneSpecofy = "-";

#endregion

#region properties

        /// <summary>変数が利用可能となるパラメータのインデックス</summary>
        protected override int VariableEnableIndex { get { return 6; } }
        
        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return isSub ? 8 : 7; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return isSub ? 10 : 9; } }

        /// <summary>モーションタイプ</summary>
        protected string MotionParam { get { return isSub ? Param[4] : Param[3]; } }

        /// <summary>イージング</summary>
        protected string EasingParam { get { return isSub ? Param[5] : Param[4]; } }

        /// <summary>トランジションの時間</summary>
        protected string TimeParam { get { return isSub ? Param[6] : Param[5]; } }

        /// <summary>x値</summary>
        protected override string XParam { get { return isSub ? Param[7] : Param[6]; } }

        /// <summary>y値</summary>
        protected override string YParam { get { return isSub ? Param[8] : Param[7]; } }

        /// <summary>z値</summary>
        protected override string ZParam { get { return isSub ? Param[9] : Param[8]; } }
        
        /// <summary>モーションタイプ</summary>
        protected MotionType motion = MotionType.Linear;

        /// <summary>イージング</summary>
        protected EasingType easing = EasingType.Custom;

        /// <summary>トランジションの時間</summary>
        protected float time = 0.0f;

#endregion

#region public methods

#pragma warning disable 168

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <param name="isSub">差分オブジェクトかどうか</param>
        public AdvCommandObjectTween(AdvCommandBase command, ObjectType objectType, bool isSub = false)
            : base(command, objectType, isSub)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            if (MotionParam != NoneSpecofy)
            {
                try
                {
                    motion = (MotionType)Enum.Parse(typeof(MotionType), MotionParam);
                }
                catch (ArgumentException e)
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "モーションタイプの指定が不正です。";
                    return;
                }
            }

            if (EasingParam != NoneSpecofy)
            {
                try
                {
                    easing = (EasingType)Enum.Parse(typeof(EasingType), EasingParam);
                }
                catch (ArgumentException e)
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "イージングの指定が不正です。";
                    return;
                }
            }

            if (!float.TryParse(TimeParam, out time))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "時間の指定が不正です。";
                return;
            }
            else if (time <= 0.0f)
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "時間に0以下の値を設定することはできません。";
                return;
            }
        }

#pragma warning restore 168

#endregion
    }
}
