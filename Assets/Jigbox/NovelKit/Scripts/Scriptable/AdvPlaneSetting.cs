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

    public sealed class AdvPlaneSetting : ScriptableObject
    {
#region properties
        
        /// <summary>キャラクター</summary>
        [SerializeField]
        int character = 1;

        /// <summary>キャラクター以外の画像</summary>
        [SerializeField]
        int sprite = 1;

        /// <summary>背景</summary>
        [SerializeField]
        int bg = 1;

        /// <summary>CG(一枚絵)</summary>
        [SerializeField]
        int cg = 1;

        /// <summary>感情表現系エモーション</summary>
        [SerializeField]
        int emotional = 1;

        /// <summary>演出</summary>
        [SerializeField]
        int effect = 1;

        /// <summary>その他</summary>
        [SerializeField]
        int other = 1;

#endregion

#region public methods

        /// <summary>
        /// オブジェクトの種類に応じたプレーンレベルを取得します。
        /// </summary>
        /// <param name="type">オブジェクトの種類</param>
        /// <returns></returns>
        public int GetPlaneLevel(ObjectType type)
        {
            int result = 1;

            switch (type)
            {
                case ObjectType.Character:
                    result = character;
                    break;
                case ObjectType.Sprite:
                    result = sprite;
                    break;
                case ObjectType.Bg:
                    result = bg;
                    break;
                case ObjectType.Cg:
                    result = cg;
                    break;
                case ObjectType.Emotional:
                    result = emotional;
                    break;
                case ObjectType.Effect:
                    result = effect;
                    break;
                case ObjectType.Other:
                    result = other;
                    break;
            }

            return result > 0 ? result : 1;
        }

#endregion
    }
}
