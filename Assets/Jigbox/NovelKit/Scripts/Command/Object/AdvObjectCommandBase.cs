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

    public class AdvObjectCommandBase : AdvCommandBase
    {
#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return isSub ? 4 : 3; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return isSub ? 4 : 3; } }

        /// <summary>同種のオブジェクトの全てが対象かどうか</summary>
        protected bool IsAll { get { return Param[1] == AdvObjectCommandParser.IndexAll; } }

        /// <summary>差分用オプションのID指定(sub-*形式での'*'部分)</summary>
        protected string SubIdParam { get { return Param[3].Length > 4 ? Param[3].Substring(4) : Param[3]; } }

        /// <summary>オブジェクトの管理番号</summary>
        protected int id;

        /// <summary>差分用オブジェクトの管理番号</summary>
        protected int subId;

        /// <summary>オブジェクトの種類</summary>
        protected ObjectType objectType;

        /// <summary>差分用オブジェクトかどうか</summary>
        protected bool isSub;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <param name="isSub">差分用オブジェクトかどうか</param>
        public AdvObjectCommandBase(AdvCommandBase command, ObjectType objectType, bool isSub) : base(command.BaseParam)
        {
            // コマンドの上書きする場合、エラー扱いのままだと処理できなくなるので、
            // ErrorCommandの場合は一旦Custom扱いとして処理を進める
            Type = command.Type == CommandType.ErrorCommand ? CommandType.Custom : command.Type;
            this.isSub = isSub;
            IsValidParameterCount();

            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            Type = CommandType.ObjectCommand;
            this.objectType = objectType;

            if (!IsAll && !int.TryParse(Param[1], out id))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "無効なIDが設定されています。";
            }
            if (isSub)
            {
                if (!int.TryParse(SubIdParam, out subId))
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "差分オブジェクトとして無効なIDが設定されています。";
                }
            }
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            if (IsAll)
            {
                foreach (AdvObjectBase obj in engine.ObjectManager.GetByType(objectType))
                {
                    CommandProcess(engine, obj);
                }
            }
            else
            {
                AdvObjectBase obj = engine.ObjectManager.GetById(objectType, id);
                if (objectType == ObjectType.ScreenEffection && !engine.ScreenEffectionManager.Enable)
                {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                    AdvLog.Error("画面効果の設定が無効になっているため、画面効果用オブジェクトへの操作は無効です。");
#endif
                    return true;
                }
                if (obj != null)
                {
                    if (!isSub)
                    {
                        CommandProcess(engine, obj);
                    }
                    else
                    {
                        AdvObjectBase subObj = obj.GetSubObject(subId);
                        if (subObj == null)
                        {
                            NotExistObjectProcess(engine);
                        }
                        else
                        {
                            CommandProcess(engine, subObj);
                        }
                    }
                }
                else
                {
                    NotExistObjectProcess(engine);
                }
            }
            return true;
        }

#endregion

#region protected methods

        /// <summary>
        /// コマンドによって発生する処理を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <param name="obj">対象となるオブジェクト</param>
        protected virtual void CommandProcess(AdvMainEngine engine, AdvObjectBase obj)
        {
        }

        /// <summary>
        /// 対象となるオブジェクトが存在しなかった場合の処理を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        protected virtual void NotExistObjectProcess(AdvMainEngine engine)
        {
#if UNITY_EDITOR || NOVELKIT_DEBUG
            AdvLog.Error("ID:" + Param[1] + "のオブジェクトが存在しないため、コマンドを実行できませんでした。");
#endif
        }

#endregion
    }
}
