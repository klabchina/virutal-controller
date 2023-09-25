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
    /// <summary>
    /// タイ語専用、グリフ(文字)の各種情報を保持するクラス
    /// </summary>
    public class ThaiFontGlyphSpec : FontGlyphSpec
    {
        /// <summary>表示位置のx軸オフセット</summary>
        public int OffsetX { get; private set; }

        /// <summary>表示位置のy軸オフセット</summary>
        public int OffsetY { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="character">文字</param>
        /// <param name="font">フォント</param>
        /// <param name="size">フォントサイズ</param>
        /// <param name="style">フォントスタイル</param>
        /// <param name="color">フォントカラー</param>
        /// <param name="glyphScaleX">フォントカラー</param>
        /// <param name="offsetX">表示位置のx軸オフセット</param>
        /// <param name="offsetY">表示位置のy軸オフセット</param>
        public ThaiFontGlyphSpec(char character, Font font, int size, FontStyle style, Color? color, float glyphScaleX, int offsetX, int offsetY)
            : base(character, font, size, style, color, glyphScaleX)
        {
            float rate = (float) size / ThaiUtils.Layout.ThaiLayoutGenerator.BaseLayoutSize;
            this.OffsetX = Mathf.RoundToInt(offsetX * rate * glyphScaleX);
            this.OffsetY = Mathf.RoundToInt(offsetY * rate);
        }
    }
}
