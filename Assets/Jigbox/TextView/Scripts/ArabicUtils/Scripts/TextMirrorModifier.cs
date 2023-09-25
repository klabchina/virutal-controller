/**
 * Additional Language Utility Library
 * Copyright(c) 2018 KLab, Inc. All Rights Reserved.
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
using UnityEngine.UI;
using System.Collections.Generic;

namespace ArabicUtils
{
    public class TextMirrorModifier : BaseMeshEffect
    {
#region properties

        /// <summary>頂点情報の操作用領域</summary>
        protected static List<UIVertex> cachedVerticesStream = new List<UIVertex>();

        /// <summary>反転を有効にするかどうか</summary>
        [SerializeField]
        protected bool isEnable = true;

        /// <summary>反転を有効にするかどうか</summary>
        public bool IsEnable
        {
            get
            {
                return isEnable; 
            }
            set
            {
                if (isEnable != value)
                {
                    if (graphic != null && !CanvasUpdateRegistry.IsRebuildingLayout())
                    {
                        graphic.SetVerticesDirty();
                    }
                    isEnable = value;
                }
            }
        }

        /// <summary>RectTransform</summary>
        protected RectTransform rectTransform;

        /// <summary>RectTransform</summary>
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = transform as RectTransform;
                }
                return rectTransform;
            }
        }

#endregion

#region public methods

        /// <summary>
        /// 頂点情報を編集します。
        /// </summary>
        /// <param name="vertexHelper">VertexHelper</param>
        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (!IsActive() || !isEnable)
            {
                return;
            }

            vertexHelper.GetUIVertexStream(cachedVerticesStream);

            float offset = RectTransform.rect.width * (0.5f - RectTransform.pivot.x) * 2.0f;

            UIVertex vertex;

            for (int i = 0; i < cachedVerticesStream.Count; i += 6)
            {
                float left = -cachedVerticesStream[i].position.x + offset;
                float right = -cachedVerticesStream[i + 1].position.x + offset;

                // left bottom
                vertex = cachedVerticesStream[i];
                vertex.position.x = right;
                cachedVerticesStream[i] = vertex;
                // right bottom
                vertex = cachedVerticesStream[i + 1];
                vertex.position.x = left;
                cachedVerticesStream[i + 1] = vertex;
                // right top
                vertex = cachedVerticesStream[i + 2];
                vertex.position.x = left;
                cachedVerticesStream[i + 2] = vertex;
                // right top
                vertex = cachedVerticesStream[i + 3];
                vertex.position.x = left;
                cachedVerticesStream[i + 3] = vertex;
                // left top
                vertex = cachedVerticesStream[i + 4];
                vertex.position.x = right;
                cachedVerticesStream[i + 4] = vertex;
                // left bottom
                vertex = cachedVerticesStream[i + 5];
                vertex.position.x = right;
                cachedVerticesStream[i + 5] = vertex;
            }

            vertexHelper.Clear();
            vertexHelper.AddUIVertexTriangleStream(cachedVerticesStream);
            cachedVerticesStream.Clear();
        }

#endregion
    }
}