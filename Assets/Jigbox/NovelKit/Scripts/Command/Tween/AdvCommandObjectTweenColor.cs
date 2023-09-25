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
using Jigbox.Tween;

namespace Jigbox.NovelKit
{
    using ObjectType = AdvObjectBase.ObjectType;

    public class AdvCommandObjectTweenColor : AdvObjectCommandBase
    {
#region properties

        /// <summary>変数が利用可能となるパラメータのインデックス</summary>
        protected override int VariableEnableIndex { get { return int.MaxValue; } }

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return isSub ? 8 : 7; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return MinParameterCount; } }

        /// <summary>モーションタイプ</summary>
        protected string MotionParam { get { return isSub ? Param[4] : Param[3]; } }

        /// <summary>イージング</summary>
        protected string EasingParam { get { return isSub ? Param[5] : Param[4]; } }

        /// <summary>トランジションの時間</summary>
        protected string TimeParam { get { return isSub ? Param[6] : Param[5]; } }

        /// <summary>色情報</summary>
        protected string ColorParam { get { return isSub ? Param[7] : Param[6]; } }

        /// <summary>モーションタイプ</summary>
        protected MotionType motion = MotionType.Linear;

        /// <summary>イージング</summary>
        protected EasingType easing = EasingType.EaseInOut;

        /// <summary>トランジションの時間</summary>
        protected float time = 0.0f;
        
        /// <summary>色</summary>
        protected Color color = Color.white;

#endregion

#region public methods

#pragma warning disable 168

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <param name="isSub">差分オブジェクトかどうか</param>
        public AdvCommandObjectTweenColor(AdvCommandBase command, ObjectType objectType, bool isSub = false)
            : base(command, objectType, isSub)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }
            
            if (MotionParam != AdvCommandObjectTween.NoneSpecofy)
            {
                try
                {
                    motion = (MotionType) Enum.Parse(typeof(MotionType), MotionParam);
                }
                catch (ArgumentException e)
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "モーションタイプの指定が不正です。";
                    return;
                }
            }

            if (EasingParam != AdvCommandObjectTween.NoneSpecofy)
            {
                try
                {
                    easing = (EasingType) Enum.Parse(typeof(EasingType), EasingParam);
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

            if (!ColorUtility.TryParseHtmlString(ColorParam, out color))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "色の指定が不正です。";
                return;
            }
        }

#pragma warning restore 168

#endregion

#region protected methods

        /// <summary>
        /// コマンドによって発生する処理を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <param name="obj">対象となるオブジェクト</param>
        protected override void CommandProcess(AdvMainEngine engine, AdvObjectBase obj)
        {
            IAdvObjectColor coloredObject = obj as IAdvObjectColor;
            if (coloredObject == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("色変更用インタフェースを持つオブジェクト以外の色は変更できません。");
#endif
                return;
            }

            TweenColor tween = new TweenColor();

            Color from = coloredObject.Color;
            Color to = color;

            if (easing != EasingType.Custom)
            {
                tween.EasingType = easing;
            }
            else
            {
                tween.EasingWith(t => t);
            }
            tween.MotionType = motion;
            tween.Begin = from;
            tween.Final = to;
            tween.OnUpdate(t => coloredObject.Color = t.Value);
            tween.Duration = GetScaledTime(engine, time);
            tween.Start();

            engine.MovementManager.Register(tween);
        }

#endregion
    }
}
