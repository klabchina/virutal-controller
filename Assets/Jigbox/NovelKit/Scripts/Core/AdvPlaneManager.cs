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
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public class AdvPlaneManager : MonoBehaviour
    {
#region constants

        /// <summary>プレーンの最小レベル</summary>
        public static readonly int PlaneLevelLow = 1;

        /// <summary>拡張用プレーンのレベル</summary>
        public static readonly int OptionalPlaneLevel = 999;

#endregion

#region properties

        /// <summary>通常のシナリオオブジェクトを配置するプレーン</summary>
        [SerializeField]
        protected List<AdvPlaneController> planes = new List<AdvPlaneController>();

        /// <summary>UI用プレーン</summary>
        [SerializeField]
        protected AdvPlaneController ui;

        /// <summary>通常プレーン以外で使用する拡張用プレーン</summary>
        [SerializeField]
        protected AdvPlaneController optionalPlane;

#endregion

#region public methods

        /// <summary>
        /// レベルを指定してプレーンを取得します。
        /// </summary>
        /// <param name="level">取得するプレーンのレベル</param>
        /// <returns>指定されたレベルのプレーン。レベルが無効な場合は、現在選択状態にあるプレーンを返します。</returns>
        public virtual AdvPlaneController GetPlane(int level)
        {
            if (IsValidLevel(level))
            {
                return level == OptionalPlaneLevel ? optionalPlane : planes[level - 1];
            }

#if UNITY_EDITOR || NOVELKIT_DEBUG
            AdvLog.Error("AdvPlaneManager.GetPlane : Set invalid level! - level " + level);
#endif
            return null;
        }
        
        /// <summary>
        /// 拡張用プレーンを設定します。
        /// </summary>
        /// <param name="plane">プレーン</param>
        public void SetOptionalPlane(AdvPlaneController plane)
        {
            optionalPlane = plane;
        }

        /// <summary>
        /// プレーンのレベルが有効かどうかを返します。
        /// </summary>
        /// <param name="level">レベル</param>
        /// <returns></returns>
        public virtual bool IsValidLevel(int level)
        {
            return (level >= PlaneLevelLow && level <= planes.Count) || (level == OptionalPlaneLevel);
        }

#endregion
        
#region override unity methos

        protected virtual void Awake()
        {
            for (int i = 0; i < planes.Count; ++i)
            {
                planes[i].Level = i;
            }
        }

#endregion
    }
}
