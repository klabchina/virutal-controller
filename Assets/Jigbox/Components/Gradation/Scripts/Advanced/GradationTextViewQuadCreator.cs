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
    public class GradationTextViewQuadCreator : GradationQuadCreator
    {
        public override List<UIVertex> CreateTopQuad(GradationVerticesInfo info, float point)
        {
            if (Direction == GradationEffectDirection.Down || Direction == GradationEffectDirection.Up)
            {
                cachedQuadData.MinX = info.VertexUnitRangeMinX;
                cachedQuadData.MaxX = info.VertexUnitRangeMaxX;
                cachedQuadData.MinY = point;
                cachedQuadData.MaxY = info.VertexUnitRangeMaxY;

                cachedQuadData.Percentage = (point - info.VertexUnitRangeMaxY) / (info.VertexUnitRangeMaxY - info.VertexUnitRangeMinY);
                
                cachedQuadData.LeftTopUV = info.uv[1];
                cachedQuadData.RightTopUV = info.uv[2];
                cachedQuadData.LeftBottomUV = info.uv[1] + (info.uv[1] - info.uv[0]) * cachedQuadData.Percentage;
                cachedQuadData.RightBottomUV = info.uv[2] + (info.uv[2] - info.uv[3]) * cachedQuadData.Percentage;
            }

            if (Direction == GradationEffectDirection.Left || Direction == GradationEffectDirection.Right)
            {
                cachedQuadData.MinX = info.VertexUnitRangeMinX;
                cachedQuadData.MaxX = point;
                cachedQuadData.MinY = info.VertexUnitRangeMinY;
                cachedQuadData.MaxY = info.VertexUnitRangeMaxY;

                cachedQuadData.Percentage = (point - info.VertexUnitRangeMinX) / (info.VertexUnitRangeMinX - info.VertexUnitRangeMaxX);

                cachedQuadData.LeftTopUV = info.uv[1];
                cachedQuadData.RightTopUV = info.uv[1] + (info.uv[1] - info.uv[2]) * cachedQuadData.Percentage;
                cachedQuadData.LeftBottomUV = info.uv[0];
                cachedQuadData.RightBottomUV = info.uv[0] + (info.uv[0] - info.uv[3]) * cachedQuadData.Percentage;
            }

            return InternalCreateQuad(
                cachedQuadData.MinX,
                cachedQuadData.MaxX,
                cachedQuadData.MinY,
                cachedQuadData.MaxY,
                cachedQuadData.LeftTopUV,
                cachedQuadData.RightTopUV,
                cachedQuadData.LeftBottomUV,
                cachedQuadData.RightBottomUV);
        }

        public override List<UIVertex> CreateMarginQuad(GradationVerticesInfo info, float point, float beforePoint)
        {
            if (Direction == GradationEffectDirection.Down || Direction == GradationEffectDirection.Up)
            {
                cachedQuadData.MinX = info.VertexUnitRangeMinX;
                cachedQuadData.MaxX = info.VertexUnitRangeMaxX;
                cachedQuadData.MinY = point;
                cachedQuadData.MaxY = beforePoint;
                
                cachedQuadData.Percentage = (point - info.VertexUnitRangeMaxY) / (info.VertexUnitRangeMaxY - info.VertexUnitRangeMinY);
                cachedQuadData.BeforePercentage = (beforePoint - info.VertexUnitRangeMaxY) / (info.VertexUnitRangeMaxY - info.VertexUnitRangeMinY);
                
                cachedQuadData.LeftTopUV = info.uv[1] + (info.uv[1] - info.uv[0]) * cachedQuadData.BeforePercentage;
                cachedQuadData.RightTopUV = info.uv[2] + (info.uv[2] - info.uv[3]) * cachedQuadData.BeforePercentage;
                cachedQuadData.LeftBottomUV = info.uv[1] + (info.uv[1] - info.uv[0]) * cachedQuadData.Percentage;
                cachedQuadData.RightBottomUV = info.uv[2] + (info.uv[2] - info.uv[3]) * cachedQuadData.Percentage;
            }

            if (Direction == GradationEffectDirection.Left || Direction == GradationEffectDirection.Right)
            {
                cachedQuadData.MinX = beforePoint;
                cachedQuadData.MaxX = point;
                cachedQuadData.MinY = info.VertexUnitRangeMinY;
                cachedQuadData.MaxY = info.VertexUnitRangeMaxY;
                
                cachedQuadData.Percentage = (point - info.VertexUnitRangeMinX) / (info.VertexUnitRangeMinX - info.VertexUnitRangeMaxX);
                cachedQuadData.BeforePercentage = (beforePoint - info.VertexUnitRangeMinX) / (info.VertexUnitRangeMinX - info.VertexUnitRangeMaxX);

                cachedQuadData.LeftTopUV = info.uv[1] + (info.uv[1] - info.uv[2]) * cachedQuadData.BeforePercentage;
                cachedQuadData.RightTopUV = info.uv[1] + (info.uv[1] - info.uv[2]) * cachedQuadData.Percentage;
                cachedQuadData.LeftBottomUV = info.uv[0] + (info.uv[0] - info.uv[3]) * cachedQuadData.BeforePercentage;
                cachedQuadData.RightBottomUV = info.uv[0] + (info.uv[0] - info.uv[3]) * cachedQuadData.Percentage;
            }
            
            return InternalCreateQuad(
                cachedQuadData.MinX,
                cachedQuadData.MaxX,
                cachedQuadData.MinY,
                cachedQuadData.MaxY,
                cachedQuadData.LeftTopUV,
                cachedQuadData.RightTopUV,
                cachedQuadData.LeftBottomUV,
                cachedQuadData.RightBottomUV);
        }

        public override List<UIVertex> CreateBottomQuad(GradationVerticesInfo info, float point)
        {
            if (Direction == GradationEffectDirection.Down || Direction == GradationEffectDirection.Up)
            {
                cachedQuadData.MinX = info.VertexUnitRangeMinX;
                cachedQuadData.MaxX = info.VertexUnitRangeMaxX;
                cachedQuadData.MinY = info.VertexUnitRangeMinY;
                cachedQuadData.MaxY = point;

                cachedQuadData.Percentage = (point - info.VertexUnitRangeMaxY) / (info.VertexUnitRangeMaxY - info.VertexUnitRangeMinY);

                cachedQuadData.LeftTopUV = info.uv[1] + (info.uv[1] - info.uv[0]) * cachedQuadData.Percentage;
                cachedQuadData.RightTopUV = info.uv[2] + (info.uv[2] - info.uv[3]) * cachedQuadData.Percentage;
                cachedQuadData.LeftBottomUV = info.uv[0];
                cachedQuadData.RightBottomUV = info.uv[3];
            }

            if (Direction == GradationEffectDirection.Left || Direction == GradationEffectDirection.Right)
            {
                cachedQuadData.MinX = point;
                cachedQuadData.MaxX = info.VertexUnitRangeMaxX;
                cachedQuadData.MinY = info.VertexUnitRangeMinY;
                cachedQuadData.MaxY = info.VertexUnitRangeMaxY;

                cachedQuadData.Percentage = (point - info.VertexUnitRangeMinX) / (info.VertexUnitRangeMinX - info.VertexUnitRangeMaxX);

                cachedQuadData.LeftTopUV = info.uv[1] + (info.uv[1] - info.uv[2]) * cachedQuadData.Percentage;
                cachedQuadData.RightTopUV = info.uv[2];
                cachedQuadData.LeftBottomUV = info.uv[0] + (info.uv[0] - info.uv[3]) * cachedQuadData.Percentage;
                cachedQuadData.RightBottomUV = info.uv[3];
            }
            
            // 頂点生成
            return InternalCreateQuad(
                cachedQuadData.MinX,
                cachedQuadData.MaxX,
                cachedQuadData.MinY,
                cachedQuadData.MaxY,
                cachedQuadData.LeftTopUV,
                cachedQuadData.RightTopUV,
                cachedQuadData.LeftBottomUV,
                cachedQuadData.RightBottomUV);
        }

        protected override List<UIVertex> InternalCreateQuad(
            float minX, 
            float maxX, 
            float minY, 
            float maxY, 
            Vector2 leftTopUV,
            Vector2 rightTopUV,
            Vector2 leftBottomUV, 
            Vector2 rightBottomUV)
        {
            cachedQuads.Clear();

            UIVertex leftTop = UIVertex.simpleVert;
            leftTop.position = new Vector2(minX, maxY);
            leftTop.uv0 = leftTopUV;

            UIVertex rightTop = UIVertex.simpleVert;
            rightTop.position = new Vector2(maxX, maxY);
            rightTop.uv0 = rightTopUV;

            UIVertex leftBottom = UIVertex.simpleVert;
            leftBottom.position = new Vector2(minX, minY);
            leftBottom.uv0 = leftBottomUV;

            UIVertex rightBottom = UIVertex.simpleVert;
            rightBottom.position = new Vector2(maxX, minY);
            rightBottom.uv0 = rightBottomUV;

            cachedQuads.Add(leftBottom);
            cachedQuads.Add(leftTop);
            cachedQuads.Add(rightTop);
            cachedQuads.Add(rightTop);
            cachedQuads.Add(rightBottom);
            cachedQuads.Add(leftBottom);

            return cachedQuads;
        }
    }
}
