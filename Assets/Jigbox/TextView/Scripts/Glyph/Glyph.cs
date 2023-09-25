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

using System;
using ArabicUtils;
using UnityEngine;

namespace Jigbox.TextView
{
    public class Glyph : IGlyph
    {
#region constants

        /// <summary>頂点数</summary>
        public const int VertexCount = 4;

#endregion

#region properties

        /// <summary>
        /// 表示するUnicode文字.
        /// </summary>
        /// <value>The character.</value>
        public char Character { get; private set; }
        
        /// <summary>文字の色</summary>
        protected Color? color;

        /// <summary>GlyphのX軸表示倍率</summary>
        protected float scaleX;

        /// <summary>フォントスタイル</summary>
        protected FontStyle style;

        /// <summary>文字の左下の座標</summary>
        protected Vector3 positionMin;

        /// <summary>文字の左上の座標</summary>
        protected Vector3 positionMax;

        /// <summary>UV座標</summary>
        protected Vector2[] uv = new Vector2[4];

#region IGlyph

        /// <summary>文字の幅(次の文字の描画開始位置までのオフセット値)</summary>
        public float Width { get; private set; }

        /// <summary>文字の幅(元サイズ)</summary>
        public float RawWidth { get; private set; }

        /// <summary>文字の高さ(基本的に文字サイズと同値)</summary>
        public float Height { get; private set; }

        /// <summary>文字を表示するための頂点位置のx軸方向の最小値(画面左側)</summary>
        public float xMin { get { return positionMin.x; } }

        /// <summary>文字を表示するための頂点位置のy軸方向の最小値(画面下側)</summary>
        public float yMin { get { return positionMin.y; } }

        /// <summary>文字を表示するための頂点位置のx軸方向の最大値(画面右側)</summary>
        public float xMax { get { return positionMax.x; } }

        /// <summary>文字を表示するための頂点位置のy軸方向の最大値(画面上側)</summary>
        public float yMax { get { return positionMax.y; } }

        /// <summary>空白、または制御文字かどうか</summary>
        public bool IsWhiteSpaceOrControl { get { return Char.IsControl(this.Character) || Char.IsWhiteSpace(this.Character); } }

        /// <summary>フォントの左側のベアリングと右側のベアリングの差分。アラビア語用</summary>
        public float RightBearingDiff { get; private set; }

#endregion

#endregion

        public Glyph(FontGlyphSpec spec, CharacterInfo info)
        {
            Character = spec.Character;
            color = spec.Color;
            // あとからGlyphを更新する際にDictionaryから情報を引く際に必要になる
            style = spec.Style;

            positionMin = new Vector3(info.minX, info.minY);
            positionMax = new Vector3(info.maxX, info.maxY);

            // Bottom Left
            uv[0] = info.uvBottomLeft;
            // Top Left
            uv[1] = info.uvTopLeft;
            // Top Right
            uv[2] = info.uvTopRight;
            // Bottom Right
            uv[3] = info.uvBottomRight;

            scaleX = spec.GlyphScaleX;
            RawWidth = info.advance;
            Width = info.advance * scaleX;
            Height = spec.Size;
            positionMin.x *= scaleX;
            positionMax.x *= scaleX;
            if (ArabicLetter.IsArabic(Character))
            {
                RightBearingDiff = (info.bearing + info.glyphWidth - info.advance + info.bearing) * scaleX;
            }
        }

        /// <summary>
        /// GlyphからFontGlyphSpecを逆算して取得します。
        /// </summary>
        /// <param name="font">フォント</param>
        /// <returns></returns>
        public FontGlyphSpec GetSpec(Font font)
        {
            return new FontGlyphSpec(Character, font, (int) Height, style, color, scaleX);
        }

        /// <summary>
        /// グリフの表示しようとしている文字を返します。
        /// </summary>
        /// <returns>文字</returns>
        public char GetVisibleCharacter()
        {
            return Character;
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
            var posMin = positionMin;
            var posMax = positionMax;

            if (halfType == PunctuationHalfType.BeginOfLine)
            {
                var half = Width / 2.0f;
                posMin = new Vector3(positionMin.x - half, positionMin.y);
                posMax = new Vector3(positionMax.x - half, positionMax.y);
            }

            // Bottom Left
            vertices[0] = UIVertex.simpleVert;
            vertices[0].position.x = posMin.x;
            vertices[0].position.y = posMin.y;
            vertices[0].color = this.color != null ? this.color.Value : color;
            // Top Left
            vertices[1] = UIVertex.simpleVert;
            vertices[1].position.x = posMin.x;
            vertices[1].position.y = posMax.y;
            // Top Right
            vertices[2] = UIVertex.simpleVert;
            vertices[2].position.x = posMax.x;
            vertices[2].position.y = posMax.y;
            // Bottom Right
            vertices[3] = UIVertex.simpleVert;
            vertices[3].position.x = posMax.x;
            vertices[3].position.y = posMin.y;

            for (int i = 0; i < VertexCount; ++i)
            {
                vertices[i].position.x += offsetX;
                vertices[i].position.y += offsetY;
                vertices[i].uv0 = uv[i];
                vertices[i].color = vertices[0].color;
            }
        }

        /// <summary>
        /// アラビア語用に修正された頂点情報を取得します
        /// 文字列全体を鏡表示にしてさらに文字ごとにy軸を反転させた表示になる頂点情報を取得する
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="color"></param>
        /// <param name="mirrorOffsetX"></param>
        public void GetMirrorVertices(ref UIVertex[] vertices, float offsetX, float offsetY, Color color, float mirrorOffsetX)
        {
            float left = -(positionMin.x + offsetX - RightBearingDiff) + mirrorOffsetX;
            float right = -(positionMax.x + offsetX - RightBearingDiff) + mirrorOffsetX;

            // Bottom Left
            vertices[0] = UIVertex.simpleVert;
            vertices[0].position.x = right;
            vertices[0].position.y = positionMin.y;
            vertices[0].color = this.color != null ? this.color.Value : color;
            // Top Left
            vertices[1] = UIVertex.simpleVert;
            vertices[1].position.x = right;
            vertices[1].position.y = positionMax.y;
            // Top Right
            vertices[2] = UIVertex.simpleVert;
            vertices[2].position.x = left;
            vertices[2].position.y = positionMax.y;
            // Bottom Right
            vertices[3] = UIVertex.simpleVert;
            vertices[3].position.x = left;
            vertices[3].position.y = positionMin.y;

            for (int i = 0; i < VertexCount; ++i)
            {
                vertices[i].position.y += offsetY;
                vertices[i].uv0 = uv[i];
                vertices[i].color = vertices[0].color;
            }
        }

        public override string ToString()
        {
            return "Glyph: " + Character;
        }
    }
}
