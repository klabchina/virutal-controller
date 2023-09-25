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
    public class MaskedGraphic
    {
#region properties

        /// <summary>マスキング対象となるGraphicコンポーネント</summary>
        protected Graphic graphic;

        /// <summary>マスキング対象となるGraphicコンポーネント</summary>
        public Graphic Graphic { get { return graphic; } }

        /// <summary>元々設定されていたマテリアル</summary>
        protected Material defaultMaterial;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="graphic">マスキング対象となるGraphicコンポーネント</param>
        public MaskedGraphic(Graphic graphic)
        {
            this.graphic = graphic;
            Material material = graphic.material;
            // デフォルト以外のマテリアルなら記録
            if (material != graphic.defaultMaterial)
            {
                defaultMaterial = material;
            }
        }

        /// <summary>
        /// Graphicコンポーネントにマテリアルを設定します。
        /// </summary>
        /// <param name="material">マテリアル</param>
        public void SetMaterial(Material material)
        {
            if (graphic == null)
            {
                return;
            }

            // 参照をキャッシュしているため、参照を解除する前に
            // オブジェクトが破棄された場合などにMissingReferenceExceptionが
            // 発生するので握りつぶす
            try
            {
                graphic.material = material;
            }
            catch (MissingReferenceException)
            {
                graphic = null;
            }
        }

        /// <summary>
        /// Graphicコンポーネントのマテリアルを元に戻します。
        /// </summary>
        public void SetDefault()
        {
            if (graphic == null)
            {
                return;
            }

            // 参照をキャッシュしているため、参照を解除する前に
            // オブジェクトが破棄された場合などにMissingReferenceExceptionが
            // 発生するので握りつぶす
            try
            {
                graphic.material = defaultMaterial;
            }
            catch (MissingReferenceException)
            {
                graphic = null;
            }
        }

#endregion
    }
}
