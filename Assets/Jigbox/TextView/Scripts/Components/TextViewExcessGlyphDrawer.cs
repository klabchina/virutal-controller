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
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasRenderer))]
    public class TextViewExcessGlyphDrawer : MonoBehaviour
    {
#region properties

        /// <summary>CanvasRenderer</summary>
        protected CanvasRenderer canvasRenderer;

        /// <summary>CanvasRenderer</summary>
        protected CanvasRenderer CanvasRenderer
        {
            get
            {
                if (canvasRenderer == null)
                {
                    canvasRenderer = gameObject.GetComponent<CanvasRenderer>();
                }
                return canvasRenderer;
            }
        }

        /// <summary>生成時に利用するメッシュ</summary>
        protected static Mesh workerMesh;

        /// <summary>生成時に利用するメッシュ</summary>
        protected static Mesh WorkerMesh
        {
            get
            {
                if (workerMesh == null)
                {
                    workerMesh = new Mesh();
                    workerMesh.name = "Shared TextView Mesh";
                    workerMesh.hideFlags = HideFlags.HideAndDontSave;
                }
                return workerMesh;
            }
        }

        /// <summary>VertexHelper</summary>
        protected static VertexHelper vertexHelper;

        /// <summary>VertexHelper</summary>
        protected static VertexHelper VertexHelper
        {
            get
            {
                if (vertexHelper == null)
                {
                    vertexHelper = new VertexHelper();
                }
                return vertexHelper;
            }
        }

#endregion

#region public methods

        /// <summary>
        /// TextViewで描画しきれない余剰文字の描画用オブジェクトを生成します。
        /// </summary>
        /// <param name="parent">親となるオブジェクト(TextView)</param>
        /// <returns></returns>
        public static TextViewExcessGlyphDrawer Create(RectTransform parent)
        {
            GameObject gameObject = new GameObject("TextViewExcessGlyphDrawer", typeof(RectTransform));
            TextViewExcessGlyphDrawer drawer = gameObject.AddComponent<TextViewExcessGlyphDrawer>();
            drawer.canvasRenderer = gameObject.GetComponent<CanvasRenderer>();
            drawer.transform.SetParent(parent, false);
            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;

#if JIGBOX_DEBUG
            gameObject.hideFlags = HideFlags.DontSave;
#else
            gameObject.hideFlags = HideFlags.HideAndDontSave;
#endif

            return drawer;
        }

        /// <summary>
        /// 頂点を追加します。
        /// </summary>
        /// <param name="vertices">追加する頂点情報</param>
        public void AddVertices(UIVertex[] vertices)
        {
            VertexHelper.AddUIVertexQuad(vertices);
        }


        /// <summary>
        /// マテリアルを更新します。
        /// </summary>
        /// <param name="material">TextViewのマテリアル</param>
        /// <param name="texture">TextViewのテクスチャ</param>
        public void UpdateMaterial(Material material, Texture texture)
        {
            CanvasRenderer.materialCount = 1;
            CanvasRenderer.SetMaterial(material, 0);
            CanvasRenderer.SetTexture(texture);
        }

        /// <summary>
        /// メッシュを生成して、CanvasRendererに適用します。
        /// </summary>
        /// <param name="modifiers">MeshModifier</param>
        /// <param name="material">TextViewのマテリアル</param>
        /// <param name="texture">TextViewのテクスチャ</param>
        public void GenerateMesh(List<IMeshModifier> modifiers, Material material, Texture texture)
        {
            if (VertexHelper.currentVertCount > 1)
            {
                gameObject.SetActive(true);
                // Pivotの影響で位置がズレる可能性があるため
                transform.localPosition = Vector3.zero;

                foreach (IMeshModifier modifier in modifiers)
                {
                    modifier.ModifyMesh(VertexHelper);
                }
                VertexHelper.FillMesh(WorkerMesh);

                UpdateMaterial(material, texture);

                CanvasRenderer.SetMesh(WorkerMesh);

                VertexHelper.Clear();
            }
            else
            {
                WorkerMesh.Clear();
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// カリングの状態を更新します。
        /// </summary>
        /// <param name="cull">カリングを行うかどうか</param>
        public virtual void UpdateCull(bool cull)
        {
            canvasRenderer.cull = cull;
        }

        /// <summary>
        /// Clipping領域の更新を行います
        /// </summary>
        /// <param name="clipRect">Clipping領域</param>
        /// <param name="validRect">Clippingを行うかどうか</param>
        public void SetClipRect(Rect clipRect, bool validRect)
        {
            if (validRect)
            {
                canvasRenderer.EnableRectClipping(clipRect);
            }
            else
            {
                canvasRenderer.DisableRectClipping();
            }
        }

#endregion

#region override unity methods

        protected virtual void OnDisable()
        {
            // 中身クリアしておかないとGameObjectが非表示になっても表示が残る
            if (canvasRenderer != null)
            {
                canvasRenderer.Clear();
            }
        }

#endregion
    }
}
