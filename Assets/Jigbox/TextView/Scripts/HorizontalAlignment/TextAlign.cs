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

namespace Jigbox.TextView
{
    /// <summary>
    /// TextView の表示するテキストの行揃えを指定する列挙型です。
    /// </summary>
    /// <remarks>
    /// 現時点では <c>Justify</c> を指定しても、均等割り付けは実装されていません
    /// </remarks>
    public enum TextAlign
    {
        /// <summary>
        /// テキストを左寄せで揃えます
        /// </summary>
        Left,

        /// <summary>
        /// テキストを中央寄せで揃えます
        /// </summary>
        Center,

        /// <summary>
        /// テキストを右寄せで揃えます
        /// </summary>
        Right,

        /// <summary>
        /// テキストを均等割付で両端を揃えます。最終行は両端揃えになりません
        /// </summary>
        Justify,

        /// <summary>
        /// テキストを均等割付で両端を揃えます。最終行も両端揃えになります
        /// </summary>
        JustifyAll

    }
}
