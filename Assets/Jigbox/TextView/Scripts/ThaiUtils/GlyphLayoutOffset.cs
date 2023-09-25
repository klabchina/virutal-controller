/**
 * Additional Language Utility Library
 * Copyright(c) 2018 KLab, Inc. All Rights Reserved.
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

namespace ThaiUtils
{
    /// <summary>
    /// 文字のレイアウト用オフセット
    /// </summary>
    public class GlyphLayoutOffset
    {
        /// <summary>オフセット値0のデータ</summary>
        public static GlyphLayoutOffset Zero { get { return new GlyphLayoutOffset(0, 0); } }

        /// <summary>xオフセット値</summary>
        public readonly int x = 0;

        /// <summary>yオフセット値</summary>
        public readonly int y = 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="x">xオフセット値</param>
        /// <param name="y">yオフセット値</param>
        public GlyphLayoutOffset(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
