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

namespace Jigbox.NovelKit
{
    public class AdvObjectMix2DSub : AdvObject2DSub
    {
#region properties
        
        /// <summary>Image</summary>
        public Image Image { get { return image; } }

        /// <summary>合成を行うオブジェクト</summary>
        protected AdvObjectMix2D mixedTarget;

#endregion

#region public methods

        /// <summary>
        /// 合成を行うオブジェクトを設定します。
        /// </summary>
        /// <param name="target"></param>
        public virtual void SetMixedTarget(AdvObjectMix2D target)
        {
            mixedTarget = target;
            image.enabled = false;
        }

        /// <summary>
        /// 合成を行うオブジェクトを外します。
        /// </summary>
        public virtual void RemoveMixedTarget()
        {
            mixedTarget = null;
            image.enabled = true;
        }

        /// <summary>
        /// 合成に使用する情報を取得します。
        /// </summary>
        /// <returns></returns>
        public virtual Vector4 GetMixRange()
        {
            // TODO : マスクが取り込まれた後にユーティリティ化して置き換える
            Vector4 mixRange = transform.InverseTransformPoint(image.canvas.transform.position);

            // Canvasを除いたScale値を計算
            Vector2 scale = new Vector2(transform.lossyScale.x / image.canvas.transform.lossyScale.x,
                transform.lossyScale.y / image.canvas.transform.lossyScale.y);

            // 位置の計算をCanvas以下相当にした際に、拡大縮小によってズレる分を補正する
            mixRange.x += mixRange.x * (scale.x - 1.0f);
            mixRange.y += mixRange.y * (scale.y - 1.0f);

            mixRange.x += (rectTransform.pivot.x - 0.5f) * rectTransform.rect.width * scale.x;
            mixRange.y += (rectTransform.pivot.y - 0.5f) * rectTransform.rect.height * scale.y;

            Vector2 size = GetRenderSize();

            mixRange.z = size.x != 0.0f ? 1.0f / (size.x * 0.5f) : 0.0f;
            mixRange.w = size.y != 0.0f ? 1.0f / (size.y * 0.5f) : 0.0f;

            if (mixRange.z != 0.0f)
            {
                mixRange.x = mixRange.x * mixRange.z;
            }
            if (mixRange.w != 0.0f)
            {
                mixRange.y = mixRange.y * mixRange.w;
            }

            return mixRange;
        }

        /// <summary>
        /// リソースを読み込みます。
        /// </summary>
        /// <param name="loader">Loader</param>
        /// <param name="resourcePath">リソースのパス</param>
        public override void LoadResource(IAdvResourceLoader loader, string resourcePath)
        {
            base.LoadResource(loader, resourcePath);

            if (mixedTarget != null)
            {
                mixedTarget.UpdateMaterial();
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// レンダリングする矩形領域のサイズを取得します。
        /// </summary>
        /// <returns></returns>
        protected Vector2 GetRenderSize()
        {
            // CanvasのScale値は見た目が正しくなるように自動的に計算されているため、
            // Canvasを除いた状態の拡縮値を求めてサイズ計算に適用する
            Vector2 scale = new Vector2(transform.lossyScale.x / image.canvas.transform.lossyScale.x,
                    transform.lossyScale.y / image.canvas.transform.lossyScale.y);

            Vector2 size = rectTransform.rect.size;
            size.x = size.x * scale.x;
            size.y = size.y * scale.y;

            return size;
        }
#endregion
    }
}
