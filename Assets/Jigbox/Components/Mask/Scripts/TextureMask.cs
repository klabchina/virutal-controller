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

namespace Jigbox.Components
{
    public class TextureMask : BlendMask
    {
#region constants
        
        /// <summary>クリッピングに使用するテクスチャのパラメータの名前</summary>
        protected static readonly string ClipTexturePropertyName = "_ClipTex";

#endregion

#region properties

        /// <summary>デフォルトで使用するマスキング用のマテリアル</summary>
        protected override string DefaultTargetMaterial { get { return "TextureMask"; } }
        
        /// <summary>マスクとして使用するテクスチャ</summary>
        [HideInInspector]
        [SerializeField]
        protected Texture texture = null;

        /// <summary>マスクとして使用するテクスチャ</summary>
        public virtual Texture Texture
        {
            get
            {
                if (texture == null)
                {
                    // 参照がない場合は白の無地テクスチャ
                    return s_WhiteTexture;
                }
                return texture;
            }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    UpdateMaskMaterial();
                }
            }
        }

        /// <summary>レンダリング対象となるテクスチャ(マスクで実際に描画は行いません)</summary>
        public sealed override Texture mainTexture { get { return Texture; } }

#endregion

#region public methods

        /// <summary>
        /// マスキングに使用するマテリアルを更新します。
        /// </summary>
        public override void UpdateMaskMaterial()
        {
            MaskMaterial.SetTexture(ClipTexturePropertyName, texture);

            foreach (Material material in UpdateTogetherMaterials)
            {
                if (material == null)
                {
                    continue;
                }

                material.SetTexture(ClipTexturePropertyName, texture);
            }
        }

#endregion

#region override unity methods

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

#if UNITY_EDITOR
            // エディタでのみ、マスク用のテクスチャのプレビュー表示ができる
            if (!showMask)
            {
                return;
            }

            if (mainTexture != null)
            {
                Rect rect = GetPixelAdjustedRect();
                Vector4 vertexRect = new Vector4(rect.x, rect.y, rect.x + rect.width, rect.y + rect.height);

                Color color = Color.white;
                vh.AddVert(new Vector3(vertexRect.x, vertexRect.y), color, Vector2.zero);
                vh.AddVert(new Vector3(vertexRect.x, vertexRect.w), color, Vector2.up);
                vh.AddVert(new Vector3(vertexRect.z, vertexRect.w), color, Vector2.one);
                vh.AddVert(new Vector3(vertexRect.z, vertexRect.y), color, new Vector2(1.0f, 0.0f));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
            }
#endif
        }

#endregion
    }
}
