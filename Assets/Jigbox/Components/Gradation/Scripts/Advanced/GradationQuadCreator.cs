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

using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.Components
{
    public abstract class GradationQuadCreator
    {
        GradationEffectDirection direction;
        public GradationEffectDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        /// <summary>頂点情報を利用したメッシュのデータキャッシュ</summary>
        protected GradationQuadData cachedQuadData = new GradationQuadData();

        /// <summary>Quad生成時のリスト、6つの頂点を入れ込むためCapacityは6にして不要なアロケーションを回避</summary>
        protected List<UIVertex> cachedQuads = new List<UIVertex>(6);

        /// <summary>
        /// 上部のQuadを生成します
        /// </summary>
        /// <param name="info"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract List<UIVertex> CreateTopQuad(GradationVerticesInfo info, float point);
        
        /// <summary>
        /// 中間のQuadを生成します
        /// </summary>
        /// <param name="info"></param>
        /// <param name="point"></param>
        /// <param name="beforePoint"></param>
        /// <returns></returns>
        public abstract List<UIVertex> CreateMarginQuad(GradationVerticesInfo info, float point, float beforePoint);
        
        /// <summary>
        /// 下部のQuadを生成します
        /// </summary>
        /// <param name="info"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract List<UIVertex> CreateBottomQuad(GradationVerticesInfo info, float point);

        /// <summary>
        /// Quadを生成します
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <param name="leftTopUV"></param>
        /// <param name="rightTopUV"></param>
        /// <param name="leftBottomUV"></param>
        /// <param name="rightBottomUV"></param>
        /// <returns></returns>
        protected abstract List<UIVertex> InternalCreateQuad(
            float minX,
            float maxX,
            float minY,
            float maxY,
            Vector2 leftTopUV,
            Vector2 rightTopUV,
            Vector2 leftBottomUV,
            Vector2 rightBottomUV);
    }
}
