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

namespace Jigbox.TextView
{
    /// <summary>
    /// TextLineを生成するためのBuilder
    /// </summary>
    public class TextLineBuilder
    {
#region inner classes

        /// <summary>
        /// 与えられたLineRenderingElementsPointerを操作するためのクラスです。
        /// Pointerを使い捨てずに使いまわします。
        /// </summary>
        class MutableLineRenderingElementsPointer : LineRenderingElementsPointer
        {
            public MutableLineRenderingElementsPointer(LineRenderingElementsPointer origin) :
                base(origin.Target, origin.ZoneIndex, origin.ElementIndex)
            {
            }

            public bool MoveNext()
            {
                if (Target.ZonesCount == zoneIndex)
                {
                    return false;
                }

                if (Target.ElementAt(zoneIndex).ElementsCount == elementIndex + 1)
                {
                    zoneIndex += 1;
                    elementIndex = 0;
                }
                else
                {
                    elementIndex += 1;
                }

                return true;
            }
        }

#endregion

#region fields & properties

        /// <summary>
        /// 開始位置を示すポインタ
        /// </summary>
        public LineRenderingElementsPointer From { get; private set; }

        /// <summary>
        /// 終了位置を示すポインタ
        /// </summary>
        public LineRenderingElementsPointer To { get; private set; }

        /// <summary>
        /// スキップ済みの本文文字数。GlyphPlacementを作成する際にGlyphIndexから減算する
        /// </summary>
        public int TotalSkippedCount { get; private set; }
        
        /// <summary>
        /// 約物で半角化するインデックスのキャッシュ
        /// </summary>
        protected readonly HashSet<int> punctuationIndexes;

        /// <summary>
        /// Fromが指す本文のインデックスのキャッシュ
        /// </summary>
        protected readonly int? fromGlyphIndexCache;

        /// <summary>
        /// Widthプロパティのキャッシュ時の状態
        /// キャッシュ更新時のToプロパティの値を保存している
        /// </summary>
        protected LineRenderingElementsPointer widthCacheState;

        /// <summary>
        /// WidthWプロパティのキャッシュ済みの値
        /// </summary>
        protected float widthCacheValue;

        /// <summary>
        /// Widthプロパティを算出した際の最後の要素の値
        /// </summary>
        /// <remarks>
        /// 最後の要素はWidth()の引数に渡す値が変わるため、戻り値が変わる可能性があるのでキャッシュは使えない
        ///
        /// 例:
        /// {
        ///     [0] Width(MainRenderingElementPosition.LineHeadTail)    // New Element
        /// }
        ///
        /// ↓ this.MoveNextMainRenderingElement
        ///
        /// {
        ///     [0] Width(MainRenderingElementPosition.LineHead),       // Cached Last Element(LineHeadTail -> LineHead)
        ///     [1] Width(MainRenderingElementPosition.LineTail),       // New Element
        /// }
        ///
        /// ↓ this.MoveNextMainRenderingElement
        ///
        /// {
        ///     [0] Width(MainRenderingElementPosition.LineHead),       // Same
        ///     [1] Width(MainRenderingElementPosition.LineMiddle),     // Cached Last Element(LineTail -> LineMiddle)
        ///     [2] Width(MainRenderingElementPosition.LineTail),       // New Element
        /// }
        ///
        /// ↓ this.MoveNextMainRenderingElement
        ///
        /// {
        ///     [0] Width(MainRenderingElementPosition.LineHead),       // Same
        ///     [1] Width(MainRenderingElementPosition.LineMiddle),     // Same
        ///     [2] Width(MainRenderingElementPosition.LineMiddle),     // Cached Last Element(LineTail -> LineMiddle)
        ///     [3] Width(MainRenderingElementPosition.LineTail),       // New Element
        /// }
        ///
        /// </remarks>
        protected float widthLastElementCacheValue;

        /// <summary>
        /// 本文の文字数をカウントして返します
        /// </summary>
        public int MainRenderingElementsCount { get; protected set; }

        /// <summary>
        /// 行末文字のスペース文字の数
        /// </summary>
        public int TailSpacingElementCount { get; set; }

        /// <summary>
        /// 行末の右マージンを含まない幅を返します
        /// </summary>
        public float Width
        {
            get
            {
                var mainRenderingElementsCount = this.MainRenderingElementsCount;
                if (mainRenderingElementsCount == 0)
                {
                    return 0;
                }

                TailSpacingElementCount = 0;

                int toCompareResult;

                // キャッシュがない場合は、一から作る
                if (this.widthCacheState == null)
                {
                    toCompareResult = 1;
                }
                // キャッシュがある場合は、更新 or 一から作る
                else
                {
                    toCompareResult = this.widthCacheState.CompareTo(this.To);

                    // キャッシュがそのまま使えるので値を返して終了
                    if (toCompareResult == 0)
                    {
                        return this.widthCacheValue;
                    }
                }

                MutableLineRenderingElementsPointer current;
                float sum;

                // 前回キャッシュ作成時よりToが後ろに移動している場合、最後の要素と新しく範囲に含まれた要素のみ計算し、キャッシュを更新する
                if (toCompareResult < 0)
                {
                    current = new MutableLineRenderingElementsPointer(
                        this.widthCacheState.PreviousMainRenderingElement());
                    sum = this.widthCacheValue - this.widthLastElementCacheValue;
                }
                // 前回キャッシュ作成時よりToが前に移動している場合、キャッシュを一から作る
                else
                {
                    if (mainRenderingElementsCount == 0)
                    {
                        this.widthCacheState = this.To;
                        this.widthCacheValue = 0;
                        this.widthLastElementCacheValue = 0;

                        return this.widthCacheValue;
                    }

                    current = new MutableLineRenderingElementsPointer(this.From);
                    sum = 0.0f;
                }

                var lastElementWidth = 0.0f;
                // 幅を算出する
                while (current.CompareTo(this.To) < 0)
                {
                    var currentElement = current.Element();
                    var glyphIndex = currentElement.GlyphIndex - this.TotalSkippedCount;

                    var position = MainRenderingElementPosition.LineHeadTail.PositionAtIndex(
                        glyphIndex - fromGlyphIndexCache.Value, mainRenderingElementsCount
                    );
                    
                    if (currentElement is MainRenderingFontElement)
                    {
                        if (current.Zone() is SplitDenyZone)
                        {
                            var fontElement = currentElement as MainRenderingFontElement;
                            var isHalfElement = fontElement.IsHalfElement(position);
                            if (isHalfElement && !punctuationIndexes.Contains(fontElement.GlyphIndex))
                            {
                                punctuationIndexes.Add(fontElement.GlyphIndex);
                            }
                        }
                    }

                    lastElementWidth = currentElement.Width(position);
                    sum += lastElementWidth;

                    current.MoveNext();
                }

                // キャッシュする
                this.widthCacheState = this.To;
                this.widthCacheValue = sum;
                this.widthLastElementCacheValue = lastElementWidth;

                return this.widthCacheValue;
            }
        }

        /// <summary>
        /// 最後に描画されるべきMainRenderingElementを返します
        /// </summary>
        public MainRenderingElement LastElement
        {
            get
            {
                var lastElementPointer = this.To.PreviousMainRenderingElement();
                return (lastElementPointer == null) ? null : lastElementPointer.Element();
            }
        }

        /// <summary>
        /// Fromで示した位置からToで示した位置までの情報をTextLineとして生成して返します
        /// </summary>
        public TextLine Result
        {
            get
            {
                if (this.MainRenderingElementsCount == 0)
                {
                    return new TextLine { PlacedGlyphs = new GlyphPlacement[] { } };
                }

                // ルビが存在する場合、MainRenderingElementsCount + RubyのCountになるためリサイズが発生する
                var placedGlyphs = new List<GlyphPlacement>(this.MainRenderingElementsCount);
                var offsetX = 0.0f;
                var current = new MutableLineRenderingElementsPointer(this.From);

                while (current.CompareTo(this.To) < 0)
                {
                    var currentElement = current.Element();
                    var isZero = System.Math.Abs(current.CompareTo(To)) <= TailSpacingElementCount;

                    var glyphIndex = currentElement.GlyphIndex - this.TotalSkippedCount;

                    var position = MainRenderingElementPosition.LineHeadTail.PositionAtIndex(
                        glyphIndex - this.fromGlyphIndexCache.Value, this.MainRenderingElementsCount
                    );

                    if (currentElement is MainRenderingFontElement)
                    {
                        var fontElement = currentElement as MainRenderingFontElement;
                        // 行頭約物半角の処理
                        var isLineHead = fontElement.IsHalfElement(position) ? PunctuationHalfType.BeginOfLine : PunctuationHalfType.None;

                        if (fontElement.SubsCount > 0)
                        {
                            var rubyPlacements = new List<GlyphPlacement>(fontElement.SubsCount);

                            // ルビのGlyphPlacementを生成
                            foreach (var subElement in fontElement.Subs(position))
                            {
                                var rubyGlyphPlacement = new GlyphPlacement(
                                    subElement.Glyph,
                                    offsetX + subElement.OffsetX,
                                    subElement.OffsetY,
                                    glyphIndex,
                                    false,
                                    null,
                                    false,
                                    PunctuationHalfType.None);

                                rubyPlacements.Add(rubyGlyphPlacement);
                            }

                            // 本文のGlyphPlacementを生成
                            placedGlyphs.Add(new GlyphPlacement(
                                fontElement.Glyph,
                                offsetX + fontElement.RubySpacingLeft + fontElement.OffsetX,
                                fontElement.OffsetY,
                                glyphIndex,
                                true,
                                rubyPlacements,
                                false,
                                isLineHead));

                            // ルビのGlyphPlacementを追加
                            placedGlyphs.AddRange(rubyPlacements);
                        }
                        else
                        {
                            // 本文のGlyphPlacementを生成
                            placedGlyphs.Add(new GlyphPlacement(
                                fontElement.Glyph,
                                offsetX + fontElement.RubySpacingLeft + fontElement.OffsetX,
                                fontElement.OffsetY,
                                glyphIndex,
                                true,
                                null,
                                isZero,
                                isLineHead));
                        }
                    }
                    else if (currentElement is MainRenderingImageElement)
                    {
                        var imageElement = currentElement as MainRenderingImageElement;

                        // 本文のGlyphPlacementを生成
                        placedGlyphs.Add(new GlyphPlacement(
                            imageElement.Glyph,
                            offsetX + imageElement.MarginLeft + imageElement.RubySpacingLeft,
                            imageElement.OffsetY,
                            glyphIndex,
                            true,
                            null,
                            false,
                            PunctuationHalfType.None));
                    }

                    if (!isZero)
                    {
                        offsetX += currentElement.Width(position);
                    }

                    current.MoveNext();
                }

                return new TextLine { PlacedGlyphs = placedGlyphs.ToArray() };
            }
        }

#endregion

#region constructors

        public TextLineBuilder(LineRenderingElementsPointer FromPointer, int totalSkippedCount, HashSet<int> punctuationIndexes = null)
        {
            this.From = FromPointer;
            this.To = this.From;
            this.TotalSkippedCount = totalSkippedCount;
            this.punctuationIndexes = punctuationIndexes;

            if (this.From.IsEndOfElements())
            {
                this.fromGlyphIndexCache = null;
            }
            else
            {
                this.fromGlyphIndexCache = this.From.Element().GlyphIndex - this.TotalSkippedCount;
            }

            this.MainRenderingElementsCount = 0;
        }

#endregion

#region methods

        /// <summary>
        /// 次のMainRenderingElementへToを移動させます
        /// </summary>
        /// <returns>正常に移動できればtrueを返します</returns>
        public bool MoveNextMainRenderingElement()
        {
            var newTo = this.To.NextMainRenderingElement();
            if (newTo == null)
            {
                return false;
            }

            this.To = newTo;
            this.MainRenderingElementsCount++;
            return true;
        }

        /// <summary>
        /// 前のMainRenderingElementへToを移動させます
        /// </summary>
        /// <returns>正常に移動できればtrueを返します</returns>
        public bool MovePreviousMainRenderingElement()
        {
            // Fromより前には戻れない
            if (this.From.CompareTo(this.To) == 0)
            {
                return false;
            }

            this.To = this.To.PreviousMainRenderingElement();
            this.MainRenderingElementsCount--;
            return true;
        }

        /// <summary>
        /// 次のSplitDenyZoneへToを移動させます
        /// </summary>
        /// <returns>正常に移動できればtrueを返します</returns>
        public bool MoveNextSplitDenyZone()
        {
            var newTo = this.To.NextSplitDenyZone();
            if (newTo == null)
            {
                return false;
            }

            this.To = newTo;
            this.UpdateMainRenderingElementsCount();
            return true;
        }

        /// <summary>
        /// 前のSplitDenyZoneへToを移動させます
        /// </summary>
        /// <returns>正常に移動できればtrueを返します</returns>
        public bool MovePreviousSplitDenyZone()
        {
            // Fromより前には戻れない
            if (this.To.CompareTo(this.From) == 0)
            {
                return false;
            }

            var newTo = this.To.PreviousSplitDenyZone();
            // Fromより前に戻ってしまっていたらFromまで進める
            if (newTo.CompareTo(this.From) < 0)
            {
                this.To = this.From;
            }
            else
            {
                this.To = newTo;
            }

            this.UpdateMainRenderingElementsCount();
            return true;
        }

        protected void UpdateMainRenderingElementsCount()
        {
            this.MainRenderingElementsCount = (this.From.CompareTo(this.To) == 0) ? 0 : this.To.PreviousMainRenderingElement().Element().GlyphIndex - this.fromGlyphIndexCache.Value + 1;
        }

#endregion
    }
}
