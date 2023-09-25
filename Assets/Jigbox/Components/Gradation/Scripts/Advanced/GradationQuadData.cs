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

namespace Jigbox.Components
{
    /// <summary>
    /// Quad生成時のキャッシュデータ
    /// </summary>
    public class GradationQuadData
    {
        public float MinX;
        public float MaxX;
        public float MinY;
        public float MaxY;
        public Vector2 LeftTopUV;
        public Vector2 RightTopUV;
        public Vector2 LeftBottomUV;
        public Vector2 RightBottomUV;
        public float CalculateUV;
        public float Percentage;
        
        /// <summary>
        /// 中間地点で使用されるUV
        /// </summary>
        public float BeforeCalculateUV;
        
        /// <summary>
        /// 中間地点で利用される位置のパーセンテージ
        /// </summary>
        public float BeforePercentage = 0.0f;
    }
}
