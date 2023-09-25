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
    public interface IGlyph
    {
        /// <summary>
        /// グリフの幅を取得する.
        /// </summary>
        /// <value>The width.</value>
        float Width { get; }

        /// <summary>
        /// グリフの高さを取得する.
        /// </summary>
        /// <value>The height.</value>
        float Height { get; }

        /// <summary>
        /// グリフのX座標の最小値
        /// </summary>
        float xMin { get; }

        /// <summary>
        /// グリフのY座標の最小値
        /// </summary>
        float yMin { get; }

        /// <summary>
        /// グリフのX座標の最大値
        /// </summary>
        float xMax { get; }

        /// <summary>
        /// グリフのY座標の最大値
        /// </summary>
        float yMax { get; }

        /// <summary>
        /// 指定したUnicode文字が制御文字かどうか、または空白文字かどうかを返します。
        /// </summary>
        /// <value><c>true</c> if this instance is white space or control; otherwise, <c>false</c>.</value>
        bool IsWhiteSpaceOrControl { get; }

        /// <summary>
        /// グリフの表示しようとしている文字を返します。
        /// </summary>
        /// <returns>文字</returns>
        char GetVisibleCharacter();

        /// <summary>
        /// 頂点情報を取得します。
        /// </summary>
        /// <param name="vertices">頂点情報を格納する領域</param>
        /// <param name="offsetX">x方向のオフセット値</param>
        /// <param name="offsetY">y方向のオフセット値</param>
        /// <param name="color">色</param>
        /// <param name="halfType">約物半角描画の種類</param>
        void GetVertices(ref UIVertex[] vertices, float offsetX, float offsetY, Color color, PunctuationHalfType halfType);
    }
}
