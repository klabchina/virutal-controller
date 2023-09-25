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
    using ObjectType = AdvObjectBase.ObjectType;

    public class AdvCommandObjectSize : AdvObjectCommandBase
    {
#region properties

        /// <summary>変数が利用可能となるパラメータのインデックス</summary>
        protected override int VariableEnableIndex { get { return 3; } }

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return isSub ? 6 : 5; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return MinParameterCount; } }

        /// <summary>横幅</summary>
        protected string WidthParam { get { return isSub ? Param[4] : Param[3]; } }

        /// <summary>縦幅</summary>
        protected string HeightParam { get { return isSub ? Param[5] : Param[4]; } }

        /// <summary>横幅</summary>
        protected ValueInfoFloat width = new ValueInfoFloat();

        /// <summary>縦幅</summary>
        protected ValueInfoFloat height = new ValueInfoFloat();

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <param name="isSub">差分オブジェクトかどうか</param>
        public AdvCommandObjectSize(AdvCommandBase command, ObjectType objectType, bool isSub = false)
            : base(command, objectType, isSub)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            ParseSize();
        }

#endregion

#region protected methods

        /// <summary>
        /// 現在の変数の値に合わせて、自身の状態を更新します。
        /// </summary>
        /// <returns></returns>
        protected override bool UpdateByVariable()
        {
            ParseSize();
            return Type != CommandType.ErrorCommand;
        }

        /// <summary>
        /// コマンドによって発生する処理を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <param name="obj">対象となるオブジェクト</param>
        protected override void CommandProcess(AdvMainEngine engine, AdvObjectBase obj)
        {
            AdvObject2DBase object2D = obj as AdvObject2DBase;
            if (object2D == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("2D以外のオブジェクトのサイズは変更できません。");
#endif
                return;
            }
            Vector3 size = object2D.Size;
            size.x = width.IsOffset ? size.x + width.Value : width.Value;
            size.y = height.IsOffset ? size.y + height.Value : height.Value;
            object2D.Size = size;
        }

        /// <summary>
        /// サイズをパースします。
        /// </summary>
        protected void ParseSize()
        {
            if (!width.Parse(WidthParam))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "横幅の指定が不正です。";
                return;
            }
            if (!height.Parse(HeightParam))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "縦幅の指定が不正です。";
                return;
            }
        }

#endregion
    }
}
