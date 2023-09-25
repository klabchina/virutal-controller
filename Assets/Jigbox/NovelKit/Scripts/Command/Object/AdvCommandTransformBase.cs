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

    public class AdvCommandTransformBase : AdvObjectCommandBase
    {
#region properties

        /// <summary>変数が利用可能となるパラメータのインデックス</summary>
        protected override int VariableEnableIndex { get { return 3; } }
        
        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return isSub ? 5 : 4; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return isSub ? 7 : 6; } }

        /// <summary>x値</summary>
        protected virtual string XParam { get { return isSub ? Param[4] : Param[3]; } }

        /// <summary>y値</summary>
        protected virtual string YParam { get { return isSub ? Param[5] : Param[4]; } }

        /// <summary>z値</summary>
        protected virtual string ZParam { get { return isSub ? Param[6] : Param[5]; } }

        /// <summary>y値が存在するかどうか</summary>
        protected virtual bool IsExistY { get { return Param.Length > MinParameterCount; } }

        /// <summary>z値が存在するかどうか</summary>
        protected virtual bool IsExistZ { get { return Param.Length > MinParameterCount + 1; } }

        /// <summary>x値情報</summary>
        protected ValueInfoFloat x = new ValueInfoFloat();

        /// <summary>y値情報</summary>
        protected ValueInfoFloat y = new ValueInfoFloat();

        /// <summary>z値情報</summary>
        protected ValueInfoFloat z = new ValueInfoFloat();

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <param name="isSub">差分オブジェクトかどうか</param>
        public AdvCommandTransformBase(AdvCommandBase command, ObjectType objectType, bool isSub = false)
            : base(command, objectType, isSub)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            ParseTransformValues();
        }

#endregion

#region protected methods

        /// <summary>
        /// 現在の変数の値に合わせて、自身の状態を更新します。
        /// </summary>
        /// <returns></returns>
        protected override bool UpdateByVariable()
        {
            ParseTransformValues();
            return Type != CommandType.ErrorCommand;
        }
        
        /// <summary>
        /// コマンドによって発生する処理を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <param name="obj">対象となるオブジェクト</param>
        protected override void CommandProcess(AdvMainEngine engine, AdvObjectBase obj)
        {
        }

        /// <summary>
        /// Transformに設定する値をパースします。
        /// </summary>
        protected void ParseTransformValues()
        {
            if (!x.Parse(XParam))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "x値の指定が不正です。";
                return;
            }
            if (!IsExistY)
            {
                return;
            }
            if (!y.Parse(YParam))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "y値の指定が不正です。";
                return;
            }
            if (!IsExistZ)
            {
                return;
            }
            if (!z.Parse(ZParam))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "z値の指定が不正です。";
                return;
            }
        }

        /// <summary>
        /// コマンドで指定された情報から新しい値を計算します。
        /// </summary>
        /// <param name="value">元の値</param>
        /// <returns></returns>
        protected Vector3 CalculateValue(Vector3 value)
        {
            value.x = x.IsOffset ? value.x + x.Value : x.Value;
            if (IsExistY)
            {
                value.y = y.IsOffset ? value.y + y.Value : y.Value;
            }
            if (IsExistZ)
            {
                value.z = z.IsOffset ? value.z + z.Value : z.Value;
            }
            return value;
        }

#endregion
    }
}
