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
    public class GradationVerticesInfo
    {
        public float VertexUnitRangeMinX = float.MaxValue;
        public float VertexUnitRangeMaxX = float.MinValue;
        public float VertexUnitRangeMinY = float.MaxValue;
        public float VertexUnitRangeMaxY = float.MinValue;
        public float VertexUVRangeMinX = float.MaxValue;
        public float VertexUVRangeMaxX = float.MinValue;
        public float VertexUVRangeMinY = float.MaxValue;
        public float VertexUVRangeMaxY = float.MinValue;
        public float PolySizeX = 0.0f;
        public float PolySizeY = 0.0f;
        public Vector2[] position = new Vector2[4];
        public Vector2[] uv = new Vector2[4];

        public void Clear()
        {
            VertexUnitRangeMinX = float.MaxValue;
            VertexUnitRangeMaxX = float.MinValue;
            VertexUnitRangeMinY = float.MaxValue;
            VertexUnitRangeMaxY = float.MinValue;
            VertexUVRangeMinX = float.MaxValue;
            VertexUVRangeMaxX = float.MinValue;
            VertexUVRangeMinY = float.MaxValue;
            VertexUVRangeMaxY = float.MinValue;
            PolySizeX = 0.0f;
            PolySizeY = 0.0f;
        }

        /// <summary>
        /// 頂点のリストからデータを算出する
        /// </summary>
        /// <param name="vertexUnit"></param>
        public void CalculateInfo(List<UIVertex> vertexUnit)
        {
            foreach (var vertex in vertexUnit)
            {
                if (VertexUnitRangeMinX > vertex.position.x)
                {
                    VertexUnitRangeMinX = vertex.position.x;
                }

                if (VertexUnitRangeMaxX < vertex.position.x)
                {
                    VertexUnitRangeMaxX = vertex.position.x;
                }

                if (VertexUnitRangeMinY > vertex.position.y)
                {
                    VertexUnitRangeMinY = vertex.position.y;
                }

                if (VertexUnitRangeMaxY < vertex.position.y)
                {
                    VertexUnitRangeMaxY = vertex.position.y;
                }

                if (VertexUVRangeMinX > vertex.uv0.x)
                {
                    VertexUVRangeMinX = vertex.uv0.x;
                }

                if (VertexUVRangeMaxX < vertex.uv0.x)
                {
                    VertexUVRangeMaxX = vertex.uv0.x;
                }

                if (VertexUVRangeMinY > vertex.uv0.y)
                {
                    VertexUVRangeMinY = vertex.uv0.y;
                }

                if (VertexUVRangeMaxY < vertex.uv0.y)
                {
                    VertexUVRangeMaxY = vertex.uv0.y;
                }
            }

            position[0] = vertexUnit[0].position;
            position[1] = vertexUnit[1].position;
            position[2] = vertexUnit[2].position;
            position[3] = vertexUnit[4].position;

            uv[0] = vertexUnit[0].uv0;
            uv[1] = vertexUnit[1].uv0;
            uv[2] = vertexUnit[2].uv0;
            uv[3] = vertexUnit[4].uv0;

            PolySizeX = VertexUnitRangeMaxX - VertexUnitRangeMinX;
            PolySizeY = VertexUnitRangeMinY - VertexUnitRangeMaxY;
        }
    }
}
