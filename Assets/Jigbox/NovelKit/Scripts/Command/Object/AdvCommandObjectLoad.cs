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

    public class AdvCommandObjectLoad : AdvObjectCommandBase, IAdvCommandGraphicResource
    {
#region constants

        /// <summary>リソースの読み込みを行わない場合の指定</summary>
        protected static readonly string ResourceEmpth = "empty";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return isSub ? 5 : 4; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return isSub ? 6 : 5; } }

        /// <summary>リソースのパス(第4パラメータ)</summary>
        protected string ResourcePathParam { get { return isSub ? Param[4] : Param[3]; } }

        /// <summary>
        /// <para>プレーンのレベル(第5パラメータ)</para>
        /// <para>差分オブジェクトの場合は、空文字</para>
        /// </summary>
        protected string PlaneLevelParam { get { return IsSetPlaneLevel ? Param[4] : string.Empty; } }

        /// <summary>
        /// <para>プレーンのレベルが指定されているかどうか</para>
        /// <para>差分オブジェクトの場合は、プレーンのレベルの指定は無効</para>
        /// </summary>
        protected bool IsSetPlaneLevel { get { return !isSub && Param.Length > MinParameterCount; } }

        /// <summary>プレーンのレベル</summary>
        int level = 0;

        /// <summary>コマンドで使用するリソース</summary>
        public virtual string GraphicResource { get { return ResourcePathParam != ResourceEmpth ? ResourcePathParam : string.Empty; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <param name="isSub">差分オブジェクトかどうか</param>
        public AdvCommandObjectLoad(AdvCommandBase command, ObjectType objectType, bool isSub = false)
            : base(command, objectType, isSub)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            if (IsSetPlaneLevel && !int.TryParse(PlaneLevelParam, out level))
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "プレーンのレベルの指定が不正です。";
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
            if (ResourcePathParam != ResourceEmpth)
            {
                obj.LoadResource(engine.Loader, ResourcePathParam);
            }
        }

        /// <summary>
        /// 対象となるオブジェクトが存在しなかった場合の処理を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        protected override void NotExistObjectProcess(AdvMainEngine engine)
        {
            AdvObjectSetting.ObjectSetting setting = engine.Settings.ObjectSetting.GetSetting(objectType, isSub);
            AdvObjectBase parent = null;

            // 通常のオブジェクトの場合、不正なプレーンを指定していないか確認
            if (!isSub)
            {
                if (!IsSetPlaneLevel)
                {
                    level = engine.Settings.PlaneSetting.GetPlaneLevel(objectType);
                }

                if (!engine.PlaneManager.IsValidLevel(level))
                {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                    AdvLog.Error("無効なプレーンのレベルが指定されました。"
                    + "\nレベル : " + level);
#endif
                    return;
                }
            }
            // 差分用オブジェクトの場合、親となるオブジェクトが存在するか確認
            else
            {
                parent = engine.ObjectManager.GetById(objectType, id);
                if (parent == null)
                {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                    AdvLog.Error("指定されたIDのオブジェクトが存在しないため、差分用オブジェクトを生成できません。"
                    + "\nオブジェクトの種類 : " + objectType.ToString()
                    + "\nID : " + id);
#endif
                    return;
                }
            }

            // 基礎オブジェクトを設定を元に生成
            Object resource = engine.Loader.Load<Object>(setting.ResourcePath);
            if (resource == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("オブジェクトの元となるリソースが存在しません。"
                + "\nオブジェクトの種類 : " + objectType.ToString()
                + "\nパス : " + setting.ResourcePath);
#endif
                return;
            }

            GameObject obj = Object.Instantiate(resource) as GameObject;
            AdvObjectBase advObject = obj.GetComponent<AdvObjectBase>();
            if (advObject == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("対象リソースにシナリオ用オブジェクトのコンポーネントが存在しません。"
                + "\nパス : " + setting.ResourcePath);
#endif
                return;
            }
            
            // 通常オブジェクトはPlane以下に設定
            if (!isSub)
            {
                advObject.Init(id, objectType, setting);
                AdvPlaneController plane = engine.PlaneManager.GetPlane(level);
                plane.SetObject(advObject);
                engine.ObjectManager.RegisterObject(advObject);
            }
            // 差分オブジェクトは対象となるオブジェクト以下に設定
            else
            {
                advObject.Init(subId, objectType, setting);
                parent.RegisterSubObject(subId, advObject);
            }

            CommandProcess(engine, advObject);
        }

#endregion
    }
}
