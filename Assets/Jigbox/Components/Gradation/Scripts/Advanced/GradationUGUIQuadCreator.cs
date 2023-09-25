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
    public class GradationUGUIQuadCreator : GradationQuadCreator
    {
        public override List<UIVertex> CreateTopQuad(GradationVerticesInfo info, float point)
        {
            if (Direction == GradationEffectDirection.Down || Direction == GradationEffectDirection.Up)
            {
                cachedQuadData.MinX = info.VertexUnitRangeMinX;
                cachedQuadData.MaxX = info.VertexUnitRangeMaxX;
                cachedQuadData.MinY = point;
                cachedQuadData.MaxY = info.VertexUnitRangeMaxY;

                cachedQuadData.CalculateUV = info.VertexUVRangeMaxY - (point - info.VertexUnitRangeMaxY) / 
                    info.PolySizeY * (info.VertexUVRangeMaxY - info.VertexUVRangeMinY);
                
                cachedQuadData.LeftTopUV = new Vector2(info.VertexUVRangeMinX, info.VertexUVRangeMaxY);
                cachedQuadData.RightTopUV = new Vector2(info.VertexUVRangeMaxX, info.VertexUVRangeMaxY);
                cachedQuadData.LeftBottomUV = new Vector2(info.VertexUVRangeMinX, cachedQuadData.CalculateUV);
                cachedQuadData.RightBottomUV = new Vector2(info.VertexUVRangeMaxX, cachedQuadData.CalculateUV);
            }

            if (Direction == GradationEffectDirection.Right || Direction == GradationEffectDirection.Left)
            {
                cachedQuadData.MinX = info.VertexUnitRangeMinX;
                cachedQuadData.MaxX = point;
                cachedQuadData.MinY = info.VertexUnitRangeMinY;
                cachedQuadData.MaxY = info.VertexUnitRangeMaxY;
                
                cachedQuadData.CalculateUV = info.VertexUVRangeMinX - (point - info.VertexUnitRangeMinX) /
                    info.PolySizeX * (info.VertexUVRangeMinX - info.VertexUVRangeMaxX);

                cachedQuadData.LeftTopUV = new Vector2(info.VertexUVRangeMinX, info.VertexUVRangeMaxY);
                cachedQuadData.RightTopUV = new Vector2(cachedQuadData.CalculateUV, info.VertexUVRangeMaxY);
                cachedQuadData.LeftBottomUV = new Vector2(info.VertexUVRangeMinX, info.VertexUVRangeMinY);
                cachedQuadData.RightBottomUV = new Vector2(cachedQuadData.CalculateUV, info.VertexUVRangeMinY);
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
                
                cachedQuadData.CalculateUV = info.VertexUVRangeMaxY - (point - info.VertexUnitRangeMaxY) /
                    info.PolySizeY * (info.VertexUVRangeMaxY - info.VertexUVRangeMinY);
                cachedQuadData.BeforeCalculateUV = info.VertexUVRangeMaxY - (beforePoint - info.VertexUnitRangeMaxY) /
                    info.PolySizeY * (info.VertexUVRangeMaxY - info.VertexUVRangeMinY);
                    
                cachedQuadData.LeftTopUV = new Vector2(info.VertexUVRangeMinX, cachedQuadData.BeforeCalculateUV);
                cachedQuadData.RightTopUV = new Vector2(info.VertexUVRangeMaxX, cachedQuadData.BeforeCalculateUV);
                cachedQuadData.LeftBottomUV = new Vector2(info.VertexUVRangeMinX, cachedQuadData.CalculateUV);
                cachedQuadData.RightBottomUV = new Vector2(info.VertexUVRangeMaxX, cachedQuadData.CalculateUV);
            }

            if (Direction == GradationEffectDirection.Right || Direction == GradationEffectDirection.Left)
            {
                cachedQuadData.MinX = beforePoint;
                cachedQuadData.MaxX = point;
                cachedQuadData.MinY = info.VertexUnitRangeMinY;
                cachedQuadData.MaxY = info.VertexUnitRangeMaxY;
                
                cachedQuadData.CalculateUV = info.VertexUVRangeMinX - (point - info.VertexUnitRangeMinX) /
                    info.PolySizeX * (info.VertexUVRangeMinX - info.VertexUVRangeMaxX);
                cachedQuadData.BeforeCalculateUV = info.VertexUVRangeMinX - (beforePoint - info.VertexUnitRangeMinX) /
                    info.PolySizeX * (info.VertexUVRangeMinX - info.VertexUVRangeMaxX);

                cachedQuadData.LeftTopUV = new Vector2(cachedQuadData.BeforeCalculateUV, info.VertexUVRangeMaxY);
                cachedQuadData.RightTopUV = new Vector2(cachedQuadData.CalculateUV, info.VertexUVRangeMaxY);
                cachedQuadData.LeftBottomUV = new Vector2(cachedQuadData.BeforeCalculateUV, info.VertexUVRangeMinY);
                cachedQuadData.RightBottomUV = new Vector2(cachedQuadData.CalculateUV, info.VertexUVRangeMinY);
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
                
                cachedQuadData.CalculateUV = info.VertexUVRangeMaxY - (point - info.VertexUnitRangeMaxY) / 
                    info.PolySizeY * (info.VertexUVRangeMaxY - info.VertexUVRangeMinY);
                
                cachedQuadData.LeftTopUV = new Vector2(info.VertexUVRangeMinX, cachedQuadData.CalculateUV);
                cachedQuadData.RightTopUV = new Vector2(info.VertexUVRangeMaxX, cachedQuadData.CalculateUV);
                cachedQuadData.LeftBottomUV = new Vector2(info.VertexUVRangeMinX, info.VertexUVRangeMinY);
                cachedQuadData.RightBottomUV = new Vector2(info.VertexUVRangeMaxX, info.VertexUVRangeMinY);
            }

            if (Direction == GradationEffectDirection.Right || Direction == GradationEffectDirection.Left)
            {
                cachedQuadData.MinX = point;
                cachedQuadData.MaxX = info.VertexUnitRangeMaxX;
                cachedQuadData.MinY = info.VertexUnitRangeMinY;
                cachedQuadData.MaxY = info.VertexUnitRangeMaxY;

                cachedQuadData.CalculateUV = info.VertexUVRangeMinX - (point - info.VertexUnitRangeMinX) /
                    info.PolySizeX * (info.VertexUVRangeMinX - info.VertexUVRangeMaxX);

                cachedQuadData.LeftTopUV = new Vector2(cachedQuadData.CalculateUV, info.VertexUVRangeMaxY);
                cachedQuadData.RightTopUV = new Vector2(info.VertexUVRangeMaxX, info.VertexUVRangeMaxY);
                cachedQuadData.LeftBottomUV = new Vector2(cachedQuadData.CalculateUV, info.VertexUVRangeMinY);
                cachedQuadData.RightBottomUV = new Vector2(info.VertexUVRangeMaxX, info.VertexUVRangeMinY);
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
