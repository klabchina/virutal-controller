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
using System.Collections.Generic;

namespace Jigbox.Components
{
    public class Outline : Shadow
    {
#region inner classes, enum, and structs

        /// <summary>
        /// アウトラインの品質
        /// </summary>
        public enum QualityLevel
        {
            /// <summary>低品質</summary>
            Low,
            /// <summary>中品質</summary>
            Middle,
            /// <summary>高品質</summary>
            High,
            /// <summary>最高品質</summary>
            Excellent,
            /// <summary>カスタム設定</summary>
            Custom,
        }

#endregion

#region constants

        /// <summary>低品質でのアウトラインの描画回数</summary>
        public static readonly int DrawCountLow = 4;

        /// <summary>中品質でのアウトラインの描画回数</summary>
        public static readonly int DrawCountMiddle = 8;

        /// <summary>高品質でのアウトラインの描画回数</summary>
        public static readonly int DrawCountHigh = 16;

        /// <summary>最高品質でのアウトラインの描画回数</summary>
        public static readonly int DrawCountExcellent = 32;

        /// <summary>描画回数の最大値</summary>
        public static readonly int MaxDrawCount = 256;

        /// <summary>アウトラインを配置し始める角度</summary>
        protected static readonly float DrawStartAngle = Mathf.PI / 4.0f;

        /// <summary>45度で交わるベクトルの内積値</summary>
        protected static readonly float DotAngle45 = 0.707106769f;

#endregion

#region properties

        /// <summary>頂点情報の操作用領域</summary>
        protected static List<UIVertex> cachedVerticesStream = new List<UIVertex>();

        /// <summary>品質</summary>
        [HideInInspector]
        [SerializeField]
        protected QualityLevel qualityLevel = QualityLevel.Low;

        /// <summary>アウトラインの描画回数</summary>
        [HideInInspector]
        [SerializeField]
        protected int drawCount = DrawCountLow;

#if UNITY_EDITOR
        /// <summary>品質のプロパティ(Editor Only)</summary>
        public QualityLevel QualityLevelInEditor
        {
            get { return qualityLevel; }
            set
            {
                if (qualityLevel != value)
                {
                    graphic.SetVerticesDirty();
                }

                qualityLevel = value;
                switch (qualityLevel)
                {
                    case QualityLevel.Low:
                        drawCount = DrawCountLow;
                        break;
                    case QualityLevel.Middle:
                        drawCount = DrawCountMiddle;
                        break;
                    case QualityLevel.High:
                        drawCount = DrawCountHigh;
                        break;
                    case QualityLevel.Excellent:
                        drawCount = DrawCountExcellent;
                        break;
                    default:
                        return;
                }

                Debug.LogFormat("QualityLevelに合わせてDrawCountを{0}に変更しました。", drawCount);
            }
        }

        /// <summary>アウトラインの描画回数プロパティ(Editor Only)</summary>
        public int DrawCountInEditor
        {
            get { return drawCount; }
            set
            {
                if (drawCount != value)
                {
                    graphic.SetVerticesDirty();
                }

                if (qualityLevel != QualityLevel.Custom)
                {
                    qualityLevel = QualityLevel.Custom;
                    Debug.LogFormat("QualityLevelをCustomに変更しました。");
                }

                drawCount = Mathf.Clamp(value, DrawCountLow, MaxDrawCount);
            }
        }
#endif

#endregion

#region protected methods

        /// <summary>
        /// アウトラインとして描画する頂点を生成します。
        /// </summary>
        protected virtual void CreateOutlineVertices()
        {
            int startIndex = 0;
            int length = cachedVerticesStream.Count;
            float angle = Mathf.PI * 2.0f / drawCount;
            Vector3 direction = Vector3.zero;

            for (int i = 0; i < drawCount; ++i)
            {
                direction.x = Mathf.Cos(DrawStartAngle + angle * i);
                direction.y = Mathf.Sin(DrawStartAngle + angle * i);

                // 360度をdrawCountで分割した角度のベクトルが、
                // effectDistanceが作る矩形範囲と交差する位置まで表示位置をずらすよう計算する

                float verticalDot = Vector3.Dot(direction, Vector3.up);
                float horizontalDot = Vector3.Dot(direction, Vector3.right);

                direction.y = Mathf.Clamp(verticalDot / DotAngle45, -1.0f, 1.0f);
                direction.x = Mathf.Clamp(horizontalDot / DotAngle45, -1.0f, 1.0f);

                ApplyShadowZeroAlloc(
                    cachedVerticesStream,
                    effectColor,
                    startIndex,
                    startIndex + length,
                    effectDistance.x * direction.x,
                    effectDistance.y * direction.y);
                startIndex += length;
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
            if (!IsActive())
            {
                return;
            }

            vertexHelper.GetUIVertexStream(cachedVerticesStream);
            // 必要頂点数は、元の頂点数 + (元の頂点数 * 描画回数)
            int neededCapacity = cachedVerticesStream.Count * (drawCount + 1);
            if (cachedVerticesStream.Capacity < neededCapacity)
            {
                cachedVerticesStream.Capacity = neededCapacity;
            }

            CreateOutlineVertices();

            vertexHelper.Clear();
            vertexHelper.AddUIVertexTriangleStream(cachedVerticesStream);
            cachedVerticesStream.Clear();
        }

#endregion
    }
}
