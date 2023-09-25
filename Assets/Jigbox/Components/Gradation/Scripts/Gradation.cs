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
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections.Generic;
using Jigbox.UIControl;

namespace Jigbox.Components
{
    public class Gradation : BaseMeshEffect, IGradation
    {
#region properties

        /// <summary>グラデーションさせる方向</summary>
        [SerializeField]
        protected GradationEffectDirection direction = GradationEffectDirection.Down;

        /// <summary>グラデーションさせる方向</summary>
        public GradationEffectDirection Direction
        {
            get
            {
                return direction;
            }
            set
            {
                if (direction != value)
                {
                    direction = value;
                    SetGraphicDirty();
                }
            }
        }

        /// <summary>グラデーションさせる際の色の合成方法</summary>
        [SerializeField]
        protected ColorEffectType type;

        /// <summary>グラデーションさせる際の色の合成方法</summary>
        public ColorEffectType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type != value)
                {
                    type = value;
                    SetGraphicDirty();
                }
            }
        }

        /// <summary>グラデーションの色の割合を求めるための範囲を頂点から求めるかどうか</summary>
        [SerializeField]
        protected bool rangeByVertices = false;

        /// <summary>グラデーションの色の割合を求めるための範囲を頂点から求めるかどうか</summary>
        public bool RangeByVertices
        {
            get
            {
                return rangeByVertices;
            }
            set
            {
                if (rangeByVertices != value)
                {
                    rangeByVertices = value;
                    SetGraphicDirty();
                }
            }
        }

        /// <summary>グラデーションの開始点の色</summary>
        [SerializeField]
        protected Color startColor = Color.white;

        /// <summary>グラデーションの開始点の色</summary>
        public Color StartColor
        {
            get
            {
                return startColor;
            }
            set
            {
                if (startColor != value)
                {
                    startColor = value;
                    SetGraphicDirty();
                }
            }
        }

        /// <summary>グラデーションの終了点の色</summary>
        [SerializeField]
        protected Color endColor = Color.white;

        /// <summary>グラデーションの終了点の色</summary>
        public Color EndColor
        {
            get
            {
                return endColor;
            }
            set
            {
                if (endColor != value)
                {
                    endColor = value;
                    SetGraphicDirty();
                }
            }
        }

        /// <summary>頂点情報の操作用領域</summary>
        protected static List<UIVertex> cachedVerticesStream = new List<UIVertex>();

        /// <summary>頂点色を計算する際に利用する座標値の最小値</summary>
        protected static float rangeMin = 0.0f;

        /// <summary>頂点色を計算する際に利用する座標値の最大値</summary>
        protected static float rangeMax = 0.0f;

#endregion

#region public methods

        /// <summary>
        /// 頂点情報を編集します。
        /// </summary>
        /// <param name="vertexHelper">VertexHelper</param>
        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (!IsActive())
            {
                return;
            }

            vertexHelper.GetUIVertexStream(cachedVerticesStream);

            CalculateRange();
            ModifyVertices();

            vertexHelper.Clear();
            vertexHelper.AddUIVertexTriangleStream(cachedVerticesStream);
            cachedVerticesStream.Clear();
        }

#endregion

#region protected methods

        /// <summary>
        /// 頂点情報を編集します。
        /// </summary>
        protected virtual void ModifyVertices()
        {
            for (int i = 0; i < cachedVerticesStream.Count; ++i)
            {
                UIVertex vertex = cachedVerticesStream[i];
                float rate = GetRate(vertex.position);
                vertex.color = GetColor(vertex.color, Color.Lerp(startColor, endColor, rate));
                cachedVerticesStream[i] = vertex;
            }
        }

        /// <summary>
        /// 頂点色を計算する際の座標範囲を計算します。
        /// </summary>
        protected virtual void CalculateRange()
        {
            if (rangeByVertices)
            {
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;

                foreach (UIVertex vertex in cachedVerticesStream)
                {
                    Vector3 position = vertex.position;
                    minX = Mathf.Min(minX, position.x);
                    minY = Mathf.Min(minY, position.y);
                    maxX = Mathf.Max(maxX, position.x);
                    maxY = Mathf.Max(maxY, position.y);
                }

                if (direction == GradationEffectDirection.Up
                    || direction == GradationEffectDirection.Down)
                {
                    rangeMin = minY;
                    rangeMax = maxY;
                }
                else
                {
                    rangeMin = minX;
                    rangeMax = maxX;
                }
            }
            else
            {
                Rect rect = graphic.rectTransform.rect;

                if (direction == GradationEffectDirection.Up
                    || direction == GradationEffectDirection.Down)
                {
                    rangeMin = rect.yMin;
                    rangeMax = rect.yMax;
                }
                else
                {
                    rangeMin = rect.xMin;
                    rangeMax = rect.xMax;
                }
            }
        }

        /// <summary>
        /// 座標の範囲内における割合を返します。
        /// </summary>
        /// <param name="position">座標</param>
        /// <returns></returns>
        protected virtual float GetRate(Vector3 position)
        {
            float range = rangeMax - rangeMin;
            switch (direction)
            {
                case GradationEffectDirection.Up:
                    return (position.y - rangeMin) / range;
                case GradationEffectDirection.Down:
                    return 1.0f - (position.y - rangeMin) / range;
                case GradationEffectDirection.Left:
                    return 1.0f - (position.x - rangeMin) / range;
                case GradationEffectDirection.Right:
                    return (position.x - rangeMin) / range;
                default:
                    Assert.IsTrue(false, "If you support all GradationDirection, you will not reach here.");
                    return 0.0f;
            }
        }

        /// <summary>
        /// 元の頂点色、グラデーションで利用する色から合成後の色を返します。
        /// </summary>
        /// <param name="vertexColor">元の色</param>
        /// <param name="compositionColor">グラデーションで利用する色</param>
        /// <returns></returns>
        protected virtual Color GetColor(Color vertexColor, Color compositionColor)
        {
            switch (type)
            {
                case ColorEffectType.None:
                    return compositionColor;
                case ColorEffectType.Additive:
                    return GraphicComponentInfo.ColorAdditive(vertexColor, GraphicComponentInfo.ConvertAdditiveColor(compositionColor));
                case ColorEffectType.Multiple:
                    return GraphicComponentInfo.ColorMultiply(vertexColor, GraphicComponentInfo.ConvertMultiplyColor(compositionColor));
                default:
                    Assert.IsTrue(false, "If you support all GradationEffectType, you will not reach here.");
                    return compositionColor;
            }
        }

        /// <summary>
        /// Graphicコンポーネントに対して、Dirtyを設定します。
        /// </summary>
        protected virtual void SetGraphicDirty()
        {
            if (graphic != null)
            {
                graphic.SetVerticesDirty();
            }
        }

#endregion
    }
}
