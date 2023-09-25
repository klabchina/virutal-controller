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
    public class RoundOutline : Outline
    {
#region protected methods

        /// <summary>
        /// アウトラインとして描画する頂点を生成します。
        /// </summary>
        protected override void CreateOutlineVertices()
        {
            int startIndex = 0;
            int length = cachedVerticesStream.Count;
            float angle = Mathf.PI * 2.0f / drawCount;

            for (int i = 0; i < drawCount; ++i)
            {
                ApplyShadowZeroAlloc(
                    cachedVerticesStream,
                    effectColor,
                    startIndex,
                    startIndex + length,
                    effectDistance.x * Mathf.Cos(angle * i),
                    effectDistance.y * Mathf.Sin(angle * i));
                startIndex += length;
            }
        }

#endregion
    }
}
