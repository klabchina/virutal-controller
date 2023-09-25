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
using System;
using System.Text;

namespace Jigbox.TextView
{
    /// <summary>
    /// 行毎の情報を取り扱うクラス
    /// </summary>
    public class TextLine
    {
#region fields

        /// <summary>
        /// 行の高さがキャッシュされているかどうか
        /// </summary>
        bool isCachedTextLineHeightFromFont = false;

        /// <summary>
        /// LineHeightFixedが適用された行の高さがキャッシュされているかどうか
        /// </summary>
        bool isCachedTextLineFixedHeightFromFont = false;

#endregion
        
#region properties

        /// <summary>
        /// 行内に配置するGlyphの配列
        /// </summary>
        public GlyphPlacement[] PlacedGlyphs { get; set; }

        /// <summary>
        /// この行のY軸の基準位置を設定、取得します
        /// </summary>
        public float LineY { get; set; }

        /// <summary>
        /// この行のX軸の基準位置を設定、取得します
        /// </summary>
        public float LineX { get; set; }

        /// <summary>
        /// このTextLineが最後のTextLineかどうかを返します
        /// </summary>
        public bool IsLastLine { get; set; }

        /// <summary>
        /// 自動改行された行かどうかを返します
        /// </summary>
        public bool IsAutoLineBreak { get; set; }
        
        /// <summary>
        /// ベースラインより上の高さ
        /// </summary>
        public float UpperHeightFromFont { get; protected set; }
        
        /// <summary>
        /// ベースラインより下の高さ
        /// </summary>
        public float UnderHeightFromFont { get; protected set; }
        
        /// <summary>
        /// 行の高さ
        /// </summary>
        public float HeightFromFont
        {
            get { return UpperHeightFromFont + UnderHeightFromFont; }
        }

#endregion

#region public methods

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("TextLine: \"");
            foreach (var g in this.PlacedGlyphs)
            {
                if (g.Y >= 0)
                {
                    str.Append(g.Glyph.GetVisibleCharacter());
                }
            }
            str.Append('"');
            return str.ToString();
        }

        /// <summary>
        /// Lowest the Glyph index.
        /// </summary>
        /// <returns>The index.</returns>
        /// <remarks>グリフが一つも存在しない場合、<c>-1</c>を返します</remarks>
        public int MinIndex()
        {
            if (this.PlacedGlyphs.Length < 1)
            {
                return -1;
            }
            int min = int.MaxValue;
            for (int i = 0; i < this.PlacedGlyphs.Length; i++)
            {
                var p = this.PlacedGlyphs[i];
                if (min > p.Index)
                {
                    min = p.Index;
                }
            }
            return min;
        }

        /// <summary>
        /// Highest the Glyph index.
        /// </summary>
        /// <returns>The index.</returns>
        /// <remarks>グリフが一つも存在しない場合、<c>-1</c>を返します</remarks>
        public int MaxIndex()
        {
            if (this.PlacedGlyphs.Length < 1)
            {
                return -1;
            }
            int max = int.MinValue;
            for (int i = 0; i < this.PlacedGlyphs.Length; i++)
            {
                var p = this.PlacedGlyphs[i];
                if (max < p.Index)
                {
                    max = p.Index;
                }
            }
            return max;
        }

        /// <summary>
        /// テキスト１行の表示に必要な幅を計算します
        /// </summary>
        /// <returns>The width.</returns>
        public float LineWidth()
        {
            var max = 0.0f;
            for (int i = 0, l = this.PlacedGlyphs.Length; i < l; i++)
            {
                var p = this.PlacedGlyphs[i];
                var x = p.X + p.Width;
                if (max < x)
                {
                    max = x;
                }
            }
            return max;
        }

        /// <summary>
        /// テキスト１行の表示に必要な高さを計算します
        /// </summary>
        /// <returns>The width.</returns>
        public float LineHeight()
        {
            var max = 0.0f;
            for (int i = 0, l = this.PlacedGlyphs.Length; i < l; i++)
            {
                var p = this.PlacedGlyphs[i];
                var y = p.Y + p.Glyph.Height;
                if (max < y)
                {
                    max = y;
                }
            }
            return max;
        }

        /// <summary>
        /// ベースラインより上の幅を計算して返します。
        /// </summary>
        /// <param name="fontSize">フォントサイズ</param>
        /// <param name="isLineHeightFixed">1行の高さを固定するかどうか</param>
        /// <param name="visibleLength">表示する文字数</param>
        /// <returns></returns>
        public float CalculateHeightUpperBaseLine(
            int fontSize,
            bool isLineHeightFixed,
            int visibleLength = Components.TextView.UnlimitedVisibleLength)
        {
            float height = fontSize;

            if (isLineHeightFixed || this.PlacedGlyphs.Length == 0)
            {
                return height;
            }

            int maxIndex = visibleLength == Components.TextView.UnlimitedVisibleLength ? MaxIndex() : visibleLength - 1;

            for (int i = 0; i < this.PlacedGlyphs.Length; ++i)
            {
                GlyphPlacement glyph = this.PlacedGlyphs[i];
                if (glyph.Index > maxIndex)
                {
                    break;
                }
                height = Mathf.Max(height, glyph.Glyph.Height - glyph.Y);
            }

            return height;
        }

        /// <summary>
        /// ベースラインより下の幅を計算して返します。
        /// </summary>
        /// <param name="font">フォント</param>
        /// <param name="heightUpperBaseLine">ベースラインより上の幅</param>
        /// <param name="isLineHeightFixed">1行の高さを固定するかどうか</param>
        /// <param name="visibleLength">表示する文字数</param>
        /// <returns></returns>
        public float CalculateHeightUnderBaseLine(
            Font font,
            float heightUpperBaseLine,
            bool isLineHeightFixed,
            int visibleLength = Components.TextView.UnlimitedVisibleLength)
        {
            float rate = CalculateHeightUnderBaseLineRate(font);
            float height = heightUpperBaseLine * rate;
            if (isLineHeightFixed || this.PlacedGlyphs.Length == 0)
            {
                return height;
            }

            int maxIndex = visibleLength == Components.TextView.UnlimitedVisibleLength ? MaxIndex() : visibleLength - 1;

            for (int i = 0; i < this.PlacedGlyphs.Length; ++i)
            {
                GlyphPlacement glyph = this.PlacedGlyphs[i];
                if (glyph.Index > maxIndex)
                {
                    break;
                }
                height = Mathf.Max(height, glyph.Y);
            }

            return height;
        }

        /// <summary>
        /// 1行の高さをFont情報から計算して算出します
        /// </summary>
        /// <param name="fontSize">フォントのサイズ</param>
        /// <param name="pointSize">フォントデータ上のサイズ</param>
        /// <param name="fontScale">フォントのスケール</param>
        /// <param name="ascentLine">フォントの上部ライン</param>
        /// <param name="descentLine">フォントの基底ライン</param>
        /// <param name="isLineHeightFixed">フォントサイズで計算を行うかどうか</param>
        /// <returns></returns>
        public float CalculateHeightFromFontInfo(
            float fontSize,
            float pointSize,
            float fontScale,
            float ascentLine,
            float descentLine,
            bool isLineHeightFixed)
        {
            // LineHeightFixedの適用がされているかどうかを確認し、キャッシュされていたらそのまま返す
            if ((this.isCachedTextLineHeightFromFont && !isLineHeightFixed) ||
                this.isCachedTextLineFixedHeightFromFont && isLineHeightFixed)
            {
                return HeightFromFont;
            }

            var mainSize = 0.0f;
            var subSize = 0.0f;
            var isPlacedSubGlyph = false;
            var isPlacedInlineGlyph = false;
            var subUpperOffset = 0.0f;
            var subUnderOffset = 0.0f;
            var inlineSize = 0.0f;
            var inlineUpperOffset = 0.0f;
            var inlineUnderOffset = 0.0f;
            var targetSize = 0.0f;

            // 計算に必要なデータをGlyphの配置から洗い出す
            foreach (var glyph in PlacedGlyphs)
            {
                // インライン画像の場合
                if (glyph.Glyph is InlineImageGlyph)
                {
                    var inlineImage = glyph.Glyph as InlineImageGlyph;
                    inlineSize = Mathf.Max(inlineSize, glyph.Glyph.Height);
                    inlineUpperOffset = Mathf.Max(inlineUpperOffset, inlineSize + -inlineImage.OffsetY);
                    inlineUnderOffset = Mathf.Max(inlineUnderOffset, inlineImage.OffsetY);
                    isPlacedInlineGlyph = true;
                }
                // メイン文字の場合
                else if (glyph.IsMainGlyph)
                {
                    mainSize = Mathf.Max(mainSize, glyph.Glyph.Height);
                }
                // サブ文字の場合
                else
                {
                    subSize = Mathf.Max(subSize, glyph.Glyph.Height);
                    subUpperOffset = Mathf.Max(subUpperOffset, glyph.Glyph.Height - glyph.Y);
                    subUnderOffset = Mathf.Max(subUnderOffset, glyph.Y);
                    isPlacedSubGlyph = true;
                }
            }

            targetSize = mainSize;

            // 文字列全体のサイズがフォントサイズより小さい場合、フォントサイズで計算を行うため文字列全体のサイズをフォントサイズとする
            // これはTextViewの配置計算がフォントサイズで行われているため、オフセットやフォントサイズのタグ指定により高さのズレを防ぐため
            // <br>タグで文字なしの改行のみの場合にもこの処理が適用される
            // LineHeightFixedがtrueの場合もフォントサイズ基準で処理をする
            if (fontSize > targetSize || isLineHeightFixed)
            {
                targetSize = fontSize;
            }
            
            // 実際の配置計算を行う、ここまででtargetSizeに全体の文字列の高さが入っている
            // ベースラインからアセントラインまでの高さ
            UpperHeightFromFont = targetSize / (pointSize * fontScale) * ascentLine;
            // ベースラインからディセントラインまでの高さ
            UnderHeightFromFont = Mathf.Abs(targetSize / (pointSize * fontScale) * descentLine);

            // 行の高さが固定でない場合の処理
            if (!isLineHeightFixed)
            {
                // インライン画像が存在する場合、Heightの指定によりUpperHeightを超えて大きく表示されているケースがあるため
                // その場合はインライン画像のサイズでUpperHeightを上書きする
                if (isPlacedInlineGlyph)
                {
                    UpperHeightFromFont = Mathf.Max(UpperHeightFromFont, inlineUpperOffset);
                }
                // ルビタグが存在する場合はルビタグの高さを最終結果に加算する
                if (isPlacedSubGlyph)
                {
                    UpperHeightFromFont += subUpperOffset - mainSize;
                }
                
                // オフセットにより文字列の下部に干渉するものがある場合はそちらを基準にしてdescentの計算をする
                // InlineImageのオフセット数値が大きい場合はdescent部分をオフセット数値にする
                UnderHeightFromFont = Mathf.Max(UnderHeightFromFont, inlineUnderOffset);
                // サブ文字のオフセット数値が大きい場合はdescent部分をオフセット数値にする
                UnderHeightFromFont = Mathf.Max(UnderHeightFromFont, subUnderOffset);
            }

            // キャッシュ済みフラグを建てる
            this.isCachedTextLineFixedHeightFromFont = isLineHeightFixed;
            this.isCachedTextLineHeightFromFont = !isLineHeightFixed;

            return HeightFromFont;
        }

        /// <summary>
        /// VisibleLengthで指定したindexまでのGlyphPlacement数を返します
        /// </summary>
        /// <param name="visibleLength">visibleLength</param>
        /// <returns></returns>
        public int GetPlacedGlyphCountAtVisibleIndex(int visibleLength)
        {
            if (visibleLength < 0)
            {
                return 0;
            }

            var count = 0;
            foreach (var p in this.PlacedGlyphs)
            {
                // visibleLength で指定している表示文字数制限を超えてる場合は終了する
                if (p.Index >= visibleLength)
                {
                    break;
                }

                count++;
            }

            return count;
        }

        /// <summary>
        /// GlyphPlacementを追加します
        /// </summary>
        /// <param name="glyphPlacement"></param>
        public void AddGlyphPlacement(GlyphPlacement glyphPlacement)
        {
            var dest = new GlyphPlacement[PlacedGlyphs.Length + 1];
            Array.Copy(PlacedGlyphs, dest, PlacedGlyphs.Length);
            dest[PlacedGlyphs.Length] = glyphPlacement;
            PlacedGlyphs = dest;
        }

#endregion

#region protected methods

        /// <summary>
        /// フォントデータからベースラインより下の幅を計算する際の基準値を計算します。
        /// </summary>
        /// <param name="font">フォント</param>
        /// <returns></returns>
        protected float CalculateHeightUnderBaseLineRate(Font font)
        {
            if (font == null)
            {
#if UNITY_EDITOR
                Debug.LogError("TextLine.CalculateDescentRate : Font is null!");
#endif
                return 0;
            }
            return 0.5f * (font.lineHeight - font.fontSize) / font.fontSize;
        }

#endregion
        
    }
}
