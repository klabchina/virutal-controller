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

namespace Jigbox.Components
{
    public class SoftnessRectMask : BlendMask
    {
#region constants

        /// <summary>ソフトクリッピングに使用するパラメータの名前</summary>
        protected static readonly string SoftnessPropertyName = "_Softness";

        /// <summary>デフォルトのクリッピング範囲</summary>
        protected static readonly Vector2 DefaultSoftness = new Vector2(4.0f, 4.0f);

        /// <summary>
        /// <para>ソフトクリッピングを無効化する場合の値</para>
        /// <para>※擬似的に無効化するだけで正確には無効化されているわけではない</para>
        /// </summary>
        protected static readonly float SoftnessInvalidation = 10000.0f;

#endregion

#region properties

        /// <summary>デフォルトで使用するマスキング用のマテリアル</summary>
        protected override string DefaultTargetMaterial { get { return "SoftnessMask"; } }

        /// <summary>ソフトクリッピングによるクリッピング範囲</summary>
        [HideInInspector]
        [SerializeField]
        protected Vector2 softness = DefaultSoftness;

        /// <summary>ソフトクリッピングによるクリッピング範囲</summary>
        public Vector2 Softness
        {
            get
            {
                return softness;
            }
            set
            {
                if (softness != value)
                {
                    softness = value;
                    if (isEnable)
                    {
                        UpdateMaskMaterial();
                    }
                }
            }
        }

#endregion

#region public methods

        /// <summary>
        /// マスキングに使用するマテリアルを更新します。
        /// </summary>
        public override void UpdateMaskMaterial()
        {
            // 計算は中心を基準とするので値は半分にする
            Vector2 size = rectTransform.rect.size * 0.5f;
            Vector2 softness = Vector2.one;
            softness.x = this.softness.x > 0 ? size.x / this.softness.x : SoftnessInvalidation;
            softness.y = this.softness.y > 0 ? size.y / this.softness.y : SoftnessInvalidation;

            MaskMaterial.SetVector(SoftnessPropertyName, softness);

            foreach (Material material in UpdateTogetherMaterials)
            {
                if (material == null)
                {
                    continue;
                }

                material.SetVector(SoftnessPropertyName, softness);
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// マスキングに使用するクリッピング情報を更新します。
        /// </summary>
        protected override void UpdateClipRange()
        {
            base.UpdateClipRange();
            // ソフトクリッピングはRectTransformの情報を基準にクリッピングをするので、
            // クリップ情報の更新をした際に一緒にソフトクリッピング用の情報も更新する
            UpdateMaskMaterial();
        }

#endregion
    }
}
