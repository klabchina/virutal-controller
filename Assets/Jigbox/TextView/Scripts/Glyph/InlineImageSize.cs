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

namespace Jigbox.TextView
{
    public class InlineImageSize
    {
        int? pixelValue;
        float? magnificationValue;

        /// <summary>
        /// pixel指定の値を返します
        /// </summary>
        public int? PixelValue
        {
            get
            {
                return this.pixelValue;
            }
        }

        /// <summary>
        /// 倍率指定の値を返します
        /// </summary>
        public float? MagnificationValue
        {
            get
            {
                return this.magnificationValue;
            }
        }

        /// <summary>
        /// int型を受け取るコンストラクタ
        /// </summary>
        /// <param name="value"></param>
        public InlineImageSize(int value)
        {
            this.pixelValue = value;
            this.magnificationValue = null;
        }

        /// <summary>
        /// float型を受け取るコンストラクタ
        /// </summary>
        /// <param name="value"></param>
        public InlineImageSize(float value)
        {
            this.pixelValue = null;
            this.magnificationValue = value;
        }

        /// <summary>
        /// 倍率のデフォルト値で生成したInlineImageSizeを返します
        /// </summary>
        /// <returns></returns>
        public static InlineImageSize CreateByDefaultMagnificationValue()
        {
            return new InlineImageSize(1.0f);
        }
    }

    public static class InlineImageSizeExtension
    {
        public static int CalculatePixel(this InlineImageSize self, int basicImageSize)
        {
            if (self.PixelValue.HasValue)
            {
                return self.PixelValue.Value;
            }
            return Mathf.RoundToInt(basicImageSize * (self.MagnificationValue ?? 1.0f));
        }
    }
}
