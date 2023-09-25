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

    public class AdvCommandObjectMaterial : AdvObjectCommandBase, IAdvCommandGraphicResource
    {
#region constants

        /// <summary>デフォルトのマテリアルを設定する場合</summary>
        protected static readonly string MaterialDefault = "default";

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return isSub ? 5 : 4; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return MinParameterCount; } }

        /// <summary>マテリアルのリソースのパス</summary>
        protected string MaterialParam { get { return isSub ? Param[4] : Param[3]; } }

        /// <summary>コマンドで使用するリソース</summary>
        public virtual string GraphicResource { get { return MaterialParam != MaterialDefault ? MaterialParam : string.Empty; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <param name="isSub">差分オブジェクトかどうか</param>
        public AdvCommandObjectMaterial(AdvCommandBase command, ObjectType objectType, bool isSub = false)
            : base(command, objectType, isSub)
        {
            if (Type == CommandType.ErrorCommand)
            {
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
            if (MaterialParam != MaterialDefault)
            {
                Material material = engine.Loader.Load<Material>(MaterialParam);
                if (material != null)
                {
                    obj.Material = material;
                }
#if UNITY_EDITOR || NOVELKIT_DEBUG
                else
                {
                    AdvLog.Error("マテリアルが存在しないため、マテリアルを設定できませんでした。"
                    + "\nマテリアル : " + MaterialParam);
                }
#endif
            }
            else
            {
                obj.Material = null;
            }
        }

#endregion
    }
}
