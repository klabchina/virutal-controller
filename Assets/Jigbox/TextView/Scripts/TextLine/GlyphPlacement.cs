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

using System.Collections.Generic;
using System.Linq;

namespace Jigbox.TextView
{
    public class GlyphPlacement
    {
        /// <summary>
        /// メモリ削減のための空のサブ文字配列
        /// </summary>
        protected static readonly IEnumerable<GlyphPlacement> EmptySubGlyphPlacements = Enumerable.Empty<GlyphPlacement>();

        /// <summary>
        /// 配置するグリフ情報.
        /// </summary>
        /// <value>The glyph.</value>
        public IGlyph Glyph { get; set; }

        /// <summary>
        /// グリフを配置するX軸.
        /// </summary>
        /// <value>The x.</value>
        public float X { get; set; }

        /// <summary>
        /// グリフを配置するY軸. ルビやタイ語、インライン画像のOffsetYが符号反転した数値になる。
        /// 正数だと"下方"に移動、負数だと"上方"に移動という形でこのY軸を利用してオフセット座標が計算されるが
        /// 見かけ上、ユーザーがoffsetYに設定する値が正数だと"上方"、負数だと"下方"に移動させたいので符号反転された状態でY軸の数値が反映される
        /// </summary>
        /// <value>The y.</value>
        public float Y { get; set; }

        /// <summary>
        /// グリフの幅
        /// 条件によりサイズを0にする
        /// InlineImageGlyphが0になる事はないのでこのプロパティを使用しない
        /// InputFieldで横幅0のスペースを残しておく必要があった為追加
        /// </summary>
        public float Width
        {
            get
            {
                if (IsWidthZero)
                {
                    return 0f;
                }

                // 行頭半角であれば文字サイズを半分として計算する
                if (PunctuationHalfType == PunctuationHalfType.BeginOfLine)
                {
                    return Glyph.Width / 2.0f;
                }

                return Glyph.Width;
            }
        }

        /// <summary>
        /// テキスト中のグリフのIndex.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; set; }

        /// <summary>
        /// 文字横幅を0にするかのフラグ
        /// InputFieldで横幅0のスペースを残しておく必要があった為追加
        /// </summary>
        public bool IsWidthZero { get; set; }

        /// <summary>
        /// 本文かどうか
        /// </summary>
        public readonly bool IsMainGlyph;

        /// <summary>
        /// この文字に付随するサブ文字(ルビなど)
        /// </summary>
        protected IEnumerable<GlyphPlacement> subGlyphPlacements;

        /// <summary>
        /// この文字に付随するサブ文字のアクセサ
        /// サブ文字が存在しない場合は空の配列を返す
        /// </summary>
        public IEnumerable<GlyphPlacement> SubGlyphPlacements
        {
            get { return subGlyphPlacements != null ? subGlyphPlacements : EmptySubGlyphPlacements; }
        }

        /// <summary>
        /// アラインメント設定でTextAlign.JustifyもしくはTextAlign.JustifyAllを設定している時に発生するオフセット位置
        /// </summary>
        public float JustifyShiftOffsetX { get; set; }

        /// <summary>約物の処理方法の種類</summary>
        public PunctuationHalfType PunctuationHalfType { get; set; } 

        public GlyphPlacement(
            IGlyph glyph, 
            float x, 
            float y, 
            int index, 
            bool isMainGlyph,
            IEnumerable<GlyphPlacement> subGlyphPlacements,
            bool isWidthZero,
            PunctuationHalfType punctuationHalfType
        )
        {
            this.Glyph = glyph;
            this.X = x;
            this.Y = y;
            this.Index = index;
            this.IsMainGlyph = isMainGlyph;
            this.subGlyphPlacements = subGlyphPlacements;
            this.IsWidthZero = isWidthZero;
            this.PunctuationHalfType = punctuationHalfType;
        }

        public override string ToString()
        {
            return string.Format(
                "GlyphPlacement(Glyph: {0}, X: {1}, Y: {2}, Index: {3}, Width: {4})",
                this.Glyph, this.X, this.Y, this.Index, Width
            );
        }
    }
}
