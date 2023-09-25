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
    public class InlineImageGlyph : IGlyph
    {
#region implement IGlyph

        public float Width { get; private set; }

        public float Height { get; private set; }

        public float xMin { get { return 0.0f; } }

        public float yMin { get { return 0.0f; } }

        public float xMax { get { return Width; } }

        public float yMax { get { return Height; } }

        public bool IsWhiteSpaceOrControl { get { return false; } }

#endregion

        public string Source { get; private set; }

        public string Name { get; private set; }

        /// <summary>
        /// ベースラインを基準とした縦方向位置調整用のOffset値(ピクセル単位)
        /// </summary>
        public int OffsetY { get; private set; }

        /// <summary>
        /// インライン画像では表示しようとしている
        /// 文字はないため代わりの文字を代用する.
        /// </summary>
        private const char inlineImagePlaceholder = '□';

        public InlineImageGlyph(float width, float height, string source, string name, int offsetY)
        {
            this.Width = width;
            this.Height = height;
            this.Source = source;
            this.Name = name;
            this.OffsetY = offsetY;
        }

        /// <summary>
        /// グリフの表示しようとしている文字を返します。
        /// </summary>
        /// <returns>文字</returns>
        public char GetVisibleCharacter()
        {
            return inlineImagePlaceholder;
        }

        /// <summary>
        /// 頂点情報を取得します。
        /// </summary>
        /// <param name="vertices">頂点情報を格納する領域</param>
        /// <param name="offsetX">x方向のオフセット値</param>
        /// <param name="offsetY">y方向のオフセット値</param>
        /// <param name="color">色</param>
        /// <param name="halfType">約物半角描画の種類</param>
        public void GetVertices(ref UIVertex[] vertices, float offsetX, float offsetY, Color color, PunctuationHalfType halfType)
        {
            // Bottom Left
            vertices[0].position.x = xMin;
            vertices[0].position.y = yMin;
            // Bottom Right
            vertices[1].position.x = xMax;
            vertices[1].position.y = yMin;
            // Top Right
            vertices[2].position.x = xMax;
            vertices[2].position.y = yMax;
            // Top Left
            vertices[3].position.x = xMin;
            vertices[3].position.y = yMax;

            for (int i = 0; i < Glyph.VertexCount; ++i)
            {
                vertices[i].position.x += offsetX;
                vertices[i].position.y += offsetY;
                vertices[i].color = color;
            }
        }
    }
}
