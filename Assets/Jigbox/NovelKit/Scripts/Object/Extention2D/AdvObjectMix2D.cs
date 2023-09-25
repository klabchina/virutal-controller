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
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public class AdvObjectMix2D : AdvObject2D
    {
#region constants

        /// <summary>クリッピングに使用するテクスチャのパラメータの名前</summary>
        protected static readonly string MixTexturePropertyName = "_MixTex";

        /// <summary>クリッピング時の範囲を計算するためのパラメータの名前</summary>
        protected static readonly string MixRangePropertyName = "_MixRange";

#endregion

#region properties

        /// <summary>Material</summary>
        public override Material Material
        {
            get
            {
                return base.Material;
            }
            set
            {
                if (!IsMixed)
                {
                    base.Material = value;
                }
#if UNITY_EDITOR || NOVELKIT_DEBUG
                else
                {
                    AdvLog.Warning("合成中はマテリアルの変更はできません。");
                }
#endif
            }
        }

        /// <summary>合成中かどうか</summary>
        public bool IsMixed { get; protected set; }

        /// <summary>合成対象のサブオブジェクト</summary>
        protected AdvObjectMix2DSub mixTarget;

        /// <summary>非合成状態でのマテリアル</summary>
        protected Material defaultMaterial;

        /// <summary>合成に使用するデフォルトのマテリアルのリソース</summary>
        protected virtual string DefaultTargetMaterial { get { return "Default/Materials/ImageMix"; } }

        /// <summary>合成に使用するマテリアルのデフォルト</summary>
        protected Material defaultMixMaterial;

        /// <summary>合成に使用するマテリアルのデフォルト</summary>
        public virtual Material DefaultMixMaterial
        {
            get
            {
                if (defaultMixMaterial == null)
                {
                    defaultMixMaterial = Instantiate(Resources.Load(DefaultTargetMaterial)) as Material;
                }
                return defaultMixMaterial;
            }
        }

        /// <summary>合成に使用するマテリアル</summary>
        [SerializeField]
        protected Material mixMaterial;

        /// <summary>合成に使用するマテリアル</summary>
        public virtual Material MixMaterial
        {
            get
            {
                return mixMaterial != null ? mixMaterial : DefaultMixMaterial;
            }
            set
            {
                if (mixMaterial != value)
                {
                    mixMaterial = value;
                    if (IsMixed)
                    {
                        UpdateTexture();
                    }
                }
            }
        }

#endregion

#region public methods

        /// <summary>
        /// 合成を行います。
        /// </summary>
        /// <param name="id">合成対象となるサブオブジェクトのID</param>
        /// <returns>合成できる場合、<c>true</c>を返します。</returns>
        public virtual bool Mix(int id)
        {
            AdvObjectMix2DSub next = null;

            Dictionary<int, AdvObjectBase> subObjects = SubObjects;
            if (subObjects.ContainsKey(id))
            {
                next = subObjects[id] as AdvObjectMix2DSub;
            }

            if (next != null)
            {
                SetMixMaterial();

                if (mixTarget != null)
                {
                    mixTarget.RemoveMixedTarget();
                }
                mixTarget = next;
                mixTarget.SetMixedTarget(this);

                UpdateMaterial();

                IsMixed = true;
            }

            return next != null;
        }

        /// <summary>
        /// 合成しているオブジェクトを分離します。
        /// </summary>
        public virtual void Divide()
        {
            if (mixTarget != null)
            {
                mixTarget.RemoveMixedTarget();
                mixTarget = null;
                IsMixed = false;
                image.material = defaultMaterial;
            }
        }

        /// <summary>
        /// 差分用オブジェクトの登録を解除します。
        /// </summary>
        /// <param name="id">ID</param>
        public override void UnregisterSubObject(int id)
        {
            Dictionary<int, AdvObjectBase> subObjects = SubObjects;
            if (subObjects.ContainsKey(id))
            {
                if (subObjects[id] == mixTarget)
                {
                    Divide();
                }
                subObjects.Remove(id);
            }
        }

        /// <summary>
        /// マテリアルの状態を更新します。
        /// </summary>
        public virtual void UpdateMaterial()
        {
            UpdateTexture();
            UpdateMixRange();
        }

#endregion

#region protected methods

        /// <summary>
        /// マテリアルのテクスチャを更新します。
        /// </summary>
        protected virtual void UpdateTexture()
        {
            MixMaterial.SetTexture(MixTexturePropertyName, mixTarget.Image.sprite.texture);
        }

        /// <summary>
        /// マテリアルの合成情報を更新します。
        /// </summary>
        protected virtual void UpdateMixRange()
        {
            MixMaterial.SetVector(MixRangePropertyName, mixTarget.GetMixRange());
        }

        /// <summary>
        /// 合成用のマテリアルを設定します。
        /// </summary>
        protected virtual void SetMixMaterial()
        {
            Material material = image.material;
            if (material != MixMaterial)
            {
                if (material != image.defaultMaterial)
                {
                    defaultMaterial = null;
                }
                else
                {
                    defaultMaterial = null;
                }

                image.material = MixMaterial;
            }
        }

#endregion

#region override unity methods

        protected virtual void LateUpdate()
        {
            if (!IsMixed)
            {
                return;
            }

            UpdateMixRange();
        }

#endregion
    }
}
