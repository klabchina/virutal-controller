﻿/**
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
    using ObjectType = AdvObjectBase.ObjectType;

    public class AdvCommandObjectMix : AdvObjectCommandBase
    {
#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 4; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return MinParameterCount; } }

        /// <summary>合成対象のID</summary>
        protected string TargetIdParam { get { return Param[3]; } }

        /// <summary>合成対象のID</summary>
        protected int targetId = 1;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <param name="isSub">差分オブジェクトかどうか</param>
        public AdvCommandObjectMix(AdvCommandBase command, ObjectType objectType, bool isSub = false)
            : base(command, objectType, isSub)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            if (!int.TryParse(TargetIdParam, out targetId))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "IDの指定が不正です。";
                return;
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
            if (obj is AdvObjectMix2D)
            {
                AdvObjectMix2D mixObject = obj as AdvObjectMix2D;
#if UNITY_EDITOR || NOVELKIT_DEBUG
                if (!mixObject.Mix(targetId))
                {
                    AdvLog.Error("指定されたIDのサブオブジェクトが存在しません。");
                }
#else
                mixObject.Mix(targetId);
#endif
            }
#if UNITY_EDITOR || NOVELKIT_DEBUG
            else
            {
                AdvLog.Error("合成が可能なオブジェクトではありません。");
            }
#endif
        }

#endregion
    }
}