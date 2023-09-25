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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Jigbox.Components
{
    public class GradationAdvanced : BaseMeshEffect
    {
#region properties

        /// <summary>グラデーションさせる方向</summary>
        [SerializeField] protected GradationEffectDirection direction = GradationEffectDirection.Down;

        /// <summary>グラデーションさせる方向</summary>
        public virtual GradationEffectDirection Direction
        {
            get { return direction; }
            set
            {
                if (direction != value)
                {
                    direction = value;
                    RefreshQuadCreator();
                    SetGraphicDirty();
                }
            }
        }

        [SerializeField] protected GradationTargetType targetType = GradationTargetType.UGUI;

        public virtual GradationTargetType TargetType
        {
            get { return targetType; }
            set
            {
                if (targetType != value)
                {
                    targetType = value;
                    RefreshQuadCreator();
                    SetGraphicDirty();
                }
            }
        }

        /// <summary>グラデーションの色の割合を求めるための範囲を頂点から求めるかどうか</summary>
        [SerializeField] protected bool rangeByVertices = false;

        /// <summary>グラデーションの色の割合を求めるための範囲を頂点から求めるかどうか</summary>
        public virtual bool RangeByVertices
        {
            get { return rangeByVertices; }
            set
            {
                if (rangeByVertices != value)
                {
                    rangeByVertices = value;
                    SetGraphicDirty();
                }
            }
        }

        /// <summary>グラデーションの設定</summary>
        [SerializeField] Gradient gradient = new Gradient();

        /// <summary>グラデーションの設定</summary>
        public virtual Gradient Gradient
        {
            get { return gradient; }
            set { gradient = value; }
        }

        /// <summary>Quadを作成するロジッククラス</summary>
        protected GradationQuadCreator QuadCreator;

        /// <summary>頂点情報の操作用領域</summary>
        protected static List<UIVertex> cachedVerticesStream = new List<UIVertex>();

        /// <summary>頂点色を計算する際に利用する座標値の最小値</summary>
        protected static float rangeMin = 0.0f;

        /// <summary>頂点色を計算する際に利用する座標値の最大値</summary>
        protected static float rangeMax = 0.0f;
        
        /// <summary>グラデーション座標のキャッシュ</summary>
        protected List<float> gradientPoints = new List<float>();
        
        /// <summary>頂点分割処理で生成されるポリゴン情報のキャッシュ</summary>
        protected List<UIVertex> cachedQuads = new List<UIVertex>();
        
        /// <summary>頂点情報のキャッシュ</summary>
        protected GradationVerticesInfo cachedVerticesInfo = new GradationVerticesInfo();

        /// <summary>頂点分割を行うかどうか</summary>
        protected virtual bool IsNeededAddVertices
        {
            get
            {
                //カラーキーが2以上ある場合は分割する
                if (gradient.colorKeys.Length > 2)
                {
                    return true;
                }

                //カラーキーが1以下の場合は分割しない
                if (gradient.colorKeys.Length <= 1)
                {
                    return false;
                }

                //カラーキーが2の場合、キーの座標が始点と終点の場合は分割しない
                if (Mathf.Approximately(gradient.colorKeys[0].time, 0.0f) &&
                    Mathf.Approximately(gradient.colorKeys[gradient.colorKeys.Length - 1].time, 1.0f))
                {
                    return false;
                }

                return true;
            }
        }

#endregion

#region public methods

        /// <summary>
        /// 頂点生成クラスを更新します
        /// </summary>
        public virtual void RefreshQuadCreator()
        {
            if (TargetType == GradationTargetType.TextView)
            {
                QuadCreator = new GradationTextViewQuadCreator();
            }

            if (TargetType == GradationTargetType.UGUI)
            {
                QuadCreator = new GradationUGUIQuadCreator();
            }

            QuadCreator.Direction = Direction;
        }

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

            if (QuadCreator == null)
            {
                RefreshQuadCreator();
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
            if (IsNeededAddVertices)
            {
                AddVertices();
            }

            for (int i = 0; i < cachedVerticesStream.Count; ++i)
            {
                UIVertex vertex = cachedVerticesStream[i];
                var rate = GetRate(vertex.position);
                vertex.color = gradient.Evaluate(rate);
                cachedVerticesStream[i] = vertex;
            }
        }

        /// <summary>
        /// 頂点情報を追加します
        /// </summary>
        protected virtual void AddVertices()
        {
            // 6頂点ごとにリストを分割(1描画分)し、頂点を追加する
            List<UIVertex> modifiedVertices = new List<UIVertex>();
            CalculateGradientPoints();

            for (int i = 0; i < cachedVerticesStream.Count / 6; i++)
            {
                modifiedVertices.AddRange(ModifyVertexQuad(cachedVerticesStream.GetRange(i * 6, 6)));
            }

            cachedVerticesStream = modifiedVertices;
        }

        /// <summary>
        /// グラデーションの座標を計算し返します
        /// </summary>
        /// <returns></returns>
        protected virtual void CalculateGradientPoints()
        {
            // gradientからキーのRateを全て算出する
            var gradientKeys = gradient.colorKeys;

            //DirectionがUpをLeftの場合、グラデーション方向が反転するのでカラー情報も反転させる
            if (direction == GradationEffectDirection.Up || direction == GradationEffectDirection.Left)
            {
                //GradientColorKeysの構造体配列を作成
                GradientColorKey[] reverseKeys = new GradientColorKey[gradientKeys.Length];

                //キーの配列をコピーし、timeを反転させる
                for (int i = 0; i < gradientKeys.Length; i++)
                {
                    reverseKeys[i] = new GradientColorKey(gradientKeys[i].color, 1 - gradientKeys[i].time);
                }

                //gradientKeysにtimeを反転させた配列をReverseをかけ、要素を逆順にして詰める
                Array.Reverse(reverseKeys);
                gradientKeys = reverseKeys;
            }

            // キーのRateを元に、Keyの座標を算出
            gradientPoints.Clear();
            
            //分割の際に始点と終点が0.0,1.0の場合頂点を追加する必要がないのでチェックする
            var gradientStartIndex = 0;
            var gradientEndIndex = gradientKeys.Length;

            if (Mathf.Approximately(gradientKeys[0].time, 0.0f))
            {
                gradientStartIndex = 1;
            }
            if (Mathf.Approximately(gradientKeys[gradientKeys.Length - 1].time, 1.0f))
            {
                gradientEndIndex = gradientKeys.Length - 1;
            }

            for (int i = gradientStartIndex; i < gradientEndIndex; i++)
            {
                //グラデーション座標を方向に応じて処理する
                if (direction == GradationEffectDirection.Up || direction == GradationEffectDirection.Down)
                {
                    gradientPoints.Add(gradientKeys[i].time * (rangeMin - rangeMax) + rangeMax);
                }
                else
                {
                    gradientPoints.Add(gradientKeys[i].time * (rangeMax - rangeMin) + rangeMin);
                }
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
        /// Quadを生成して返します
        /// </summary>
        /// <param name="vertexUnit"></param>
        /// <returns></returns>
        protected virtual List<UIVertex> ModifyVertexQuad(List<UIVertex> vertexUnit)
        {
            cachedVerticesInfo.Clear();
            cachedVerticesInfo.CalculateInfo(vertexUnit);
            
            cachedQuads.Clear();
            float? beforePoint = null;

            // Key座標がポリゴンのMaxとMinの間に当てはまるものを洗い出す
            for (int i = 0; i < gradientPoints.Count; i++)
            {
                var currentPoint = gradientPoints[i];
                var isBottomQuad = i == gradientPoints.Count - 1;

                if (direction == GradationEffectDirection.Up || direction == GradationEffectDirection.Down)
                {
                    // キーの位置が範囲外の場合はスキップする
                    if (currentPoint > cachedVerticesInfo.VertexUnitRangeMaxY || currentPoint < cachedVerticesInfo.VertexUnitRangeMinY)
                    {
                        if (isBottomQuad && cachedQuads.Count != 0)
                        {
                            cachedQuads.AddRange(QuadCreator.CreateBottomQuad(cachedVerticesInfo, beforePoint.Value));
                        }

                        continue;
                    }
                }

                if (direction == GradationEffectDirection.Right || direction == GradationEffectDirection.Left)
                {
                    // キーの位置が範囲外の場合はスキップする
                    if (currentPoint > cachedVerticesInfo.VertexUnitRangeMaxX || currentPoint < cachedVerticesInfo.VertexUnitRangeMinX)
                    {
                        if (isBottomQuad && cachedQuads.Count != 0)
                        {
                            cachedQuads.AddRange(QuadCreator.CreateBottomQuad(cachedVerticesInfo, beforePoint.Value));
                        }

                        continue;
                    }
                }

                // Key座標の数に応じて、全ての頂点を作成していく
                // メッシュの上部を生成する
                if (beforePoint == null)
                {
                    cachedQuads.AddRange(QuadCreator.CreateTopQuad(cachedVerticesInfo, currentPoint));
                }

                // メッシュの中間を生成
                if (beforePoint != null)
                {
                    cachedQuads.AddRange(QuadCreator.CreateMarginQuad(cachedVerticesInfo, currentPoint, beforePoint.Value));
                }

                // このループで全ての頂点追加が終了する場合、底辺部分のメッシュも追加で作成する
                if (isBottomQuad)
                {
                    cachedQuads.AddRange(QuadCreator.CreateBottomQuad(cachedVerticesInfo, currentPoint));
                }

                beforePoint = currentPoint;
            }

            // キーの追加がなかったのでそのまま返す
            if (cachedQuads.Count == 0)
            {
                return vertexUnit;
            }

            return cachedQuads;
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
