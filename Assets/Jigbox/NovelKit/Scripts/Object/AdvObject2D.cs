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

using Jigbox.Tween;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.NovelKit
{
    public class AdvObject2D : AdvObject2DBase
    {
#region properties
        
        /// <summary>Canvas Group</summary>
        [SerializeField]
        protected CanvasGroup group;

        /// <summary>表示順を決めるためのDepth値</summary>
        public override int Depth
        {
            get
            {
                return depth;
            }
            set
            {
                if (depth != value)
                {
                    // 最前面
                    if (value <= 0)
                    {
                        LocalTransform.SetAsFirstSibling();
                    }
                    // 最背面
                    else if (value >= Plane.DepthMax)
                    {
                        LocalTransform.SetAsLastSibling();
                    }
                    else
                    {
                        LocalTransform.SetSiblingIndex(value);
                    }
                    UpdateDepth();
                }
            }
        }

        /// <summary>アルファ値</summary>
        protected override float Alpha { get { return group.alpha; } set { group.alpha = value; } }

#endregion

#region protected methods

        /// <summary>
        /// アルファの状態が更新された際に呼び出されます。
        /// </summary>
        /// <param name="tween"></param>
        protected override void OnUpdateAlpha(ITween<float> tween)
        {
            group.alpha = tween.Value;
        }

#endregion
    }
}
