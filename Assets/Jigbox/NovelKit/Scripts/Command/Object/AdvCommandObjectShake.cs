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

    public class AdvCommandObjectShake : AdvObjectCommandBase
    {
#region constants

        /// <summary>x軸</summary>
        public const string AxesX = "x";

        /// <summary>y軸</summary>
        public const string AxesY = "y";

        /// <summary>z軸</summary>
        public const string AxesZ = "z";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return isSub ? 7 : 6; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return isSub ? 8 : 7; } }

        /// <summary>振動させる軸のパラメータ</summary>
        protected string AxesParam { get { return isSub ? Param[4] : Param[3]; } }

        /// <summary>振動数のパラメータ</summary>
        protected string FrequencyParam { get { return isSub ? Param[5] : Param[4]; } }

        /// <summary>振幅のパラメータ</summary>
        protected string AmplitudeParam { get { return isSub ? Param[6] : Param[5]; } }

        /// <summary>モーションタイプ</summary>
        protected string MotionParam { get { return Param.Length != MaxParameterCount ? string.Empty : isSub ? Param[7] : Param[6]; } }

        /// <summary>1振動あたりの時間</summary>
        protected float duration = 1.0f;

        /// <summary>振幅</summary>
        protected float amplitude = 1.0f;

        /// <summary>モーションタイプ</summary>
        protected MotionType motion = MotionType.Linear;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <param name="isSub">差分オブジェクトかどうか</param>
        public AdvCommandObjectShake(AdvCommandBase command, ObjectType objectType, bool isSub = false)
            : base(command, objectType, isSub)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            switch (AxesParam)
            {
                case AxesX:
                case AxesY:
                case AxesZ:
                    break;
                default:
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "軸の指定が不正です。";
                    return;
            }

            float f;
            if (!float.TryParse(FrequencyParam, out f))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "振動数の指定が不正です。";
                return;
            }
            else
            {
                if (f <= 0.0f)
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "振動数に0以下の値を設定することはできません。";
                    return;
                }
                else
                {
                    duration = 1.0f / f;
                }
            }

            if (!float.TryParse(AmplitudeParam, out amplitude))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "振幅の指定が不正です。";
                return;
            }

            if (!string.IsNullOrEmpty(MotionParam))
            {
                try
                {
                    motion = (MotionType) Enum.Parse(typeof(MotionType), MotionParam);
                }
#pragma warning disable 168
                catch (ArgumentException e)
#pragma warning restore 168
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "モーションタイプの指定が不正です。";
                    return;
                }
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// コマンドによって発生する処理を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <param name="obj">対象となるオブジェクト</param>
        protected override void CommandProcess(AdvMainEngine engine, AdvObjectBase obj)
        {
            AdvObjectShaker shaker = obj.GetComponent<AdvObjectShaker>();
            if (shaker == null)
            {
                shaker = obj.gameObject.AddComponent<AdvObjectShaker>();
            }

            duration = GetScaledTime(engine, duration);

            bool isExecute = shaker.Shake(AxesParam, duration, amplitude, motion);
#if UNITY_EDITOR || NOVELKIT_DEBUG
            if (!isExecute)
            {
                AdvLog.Error("同軸の振動がすでに設定されているため、新たに振動を設定できませんでした。");
            }
#endif
        }

#endregion
    }
}
