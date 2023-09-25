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
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Jigbox.TextView
{
    /// <summary>
    /// 論理行から実数を持った各行の情報へとアセンブルするクラス
    /// </summary>
    public class LineRenderingElementsAssembly
    {
#region inner class

        public class AssembleOption : MainRenderingElementOption
        {
            /// <summary>TextView</summary>
            protected Components.TextView textView;

            /// <summary>スペーシング単位</summary>
            public virtual SpacingUnit SpacingUnit { get { return textView.SpacingUnit; } }

            /// <summary>フォントサイズ</summary>
            public virtual int FontSize { get { return textView.FontSize; } }

            /// <summary></summary>
            public virtual float GlyphScaleX { get { return textView.GlyphScaleX; } }

            /// <summary>行末のスペースを取り除くかどうか</summary>
            public virtual bool TrimLineTailSpacing { get { return textView.TrimLineTailSpacing; } }

            /// <summary>行頭約物半角フラグ</summary>
            public virtual bool BeginningOfLineHalf { get { return textView.IsHalfPunctuationOfLineHead; } }

            public AssembleOption(Components.TextView textView)
            {
                this.textView = textView;
            }
        }

#endregion

#region temporary data type

        protected class TemporaryMainRenderingElement
        {
            public int glyphIndex;
            public float width;
            // Note: characterSpacingLeft は常に0なのでプロパティ化しない
            public float characterSpacingRight;
            public float rubySpacingLeft;
            public float rubySpacingRight;

            public MainRenderingElementOption option;

            TemporaryElementOption elementOption;

            public TemporaryElementOption ElementOption
            {
                get { return elementOption; }
                set { elementOption = value; }
            }

            public float TotalWidth
            {
                get { return elementOption.TotalWidth(this); }
            }

            public MainRenderingElement CreateImmutable(IEnumerable<SubRenderingFontElement> pairedRubies = null)
            {
                return elementOption.CreateImmutable(this, pairedRubies);
            }
        }

        /// <summary>
        /// TemporaryMainRenderingElementsにて、必要データが文字と画像で可変するものを切り分けたOptionクラス
        /// </summary>
        protected abstract class TemporaryElementOption
        {
            public abstract float TotalWidth(TemporaryMainRenderingElement element);

            public abstract MainRenderingElement CreateImmutable(
                TemporaryMainRenderingElement element,
                IEnumerable<SubRenderingFontElement> pairedRubies = null);
        }

        protected class TemporaryFontElementOption : TemporaryElementOption
        {
            public Glyph glyph;

            /// <summary>タイ語用拡張</summary>
            public int offsetX;

            /// <summary>タイ語用拡張</summary>
            public int offsetY;

            public override float TotalWidth(TemporaryMainRenderingElement element)
            {
                return element.width 
                   + element.rubySpacingLeft 
                   + element.rubySpacingRight 
                   + element.characterSpacingRight;
            }

            public override MainRenderingElement CreateImmutable(
                TemporaryMainRenderingElement element,
                IEnumerable<SubRenderingFontElement> pairedRubies = null)
            {
                return new MainRenderingFontElement(
                    this.glyph,
                    element.characterSpacingRight,
                    element.rubySpacingLeft,
                    element.rubySpacingRight,
                    pairedRubies,
                    element.glyphIndex,
                    element.option
                )
                {
                    OffsetX = offsetX,
                    OffsetY = offsetY
                };
            }
        }

        protected class TemporaryImageElementOption : TemporaryElementOption
        {
            public float height;
            public string source;
            public string name;
            public int marginLeft;
            public int marginRight;
            public int offsetY;

            public override float TotalWidth(TemporaryMainRenderingElement element)
            {
                return element.width 
                   + element.rubySpacingLeft 
                   + element.rubySpacingRight 
                   + this.marginLeft 
                   + this.marginRight 
                   + element.characterSpacingRight;
            }

            public override MainRenderingElement CreateImmutable(
                TemporaryMainRenderingElement element,
                IEnumerable<SubRenderingFontElement> pairedRubies = null)
            {
                return new MainRenderingImageElement(
                    element.width,
                    this.height,
                    this.source,
                    this.name,
                    element.characterSpacingRight,
                    element.rubySpacingLeft,
                    element.rubySpacingRight,
                    this.marginLeft,
                    this.marginRight,
                    this.offsetY,
                    pairedRubies,
                    element.glyphIndex,
                    element.option
                );
            }
        }

        protected class TemporarySubRenderingFontElement
        {
            public Glyph glyph;
            public float width;
            public float glyphWidth;
            public float offsetX;
            public float offsetY;
        }

        protected class Spacing
        {
            public float rubySpacing;
            public float rubyParentSpacing;
        }

#endregion

#region properties

        protected static List<TemporaryMainRenderingElement> cachedTemporaryMainElements = new List<TemporaryMainRenderingElement>();

        protected static List<TemporarySubRenderingFontElement> cachedTemporaryRubyElements = new List<TemporarySubRenderingFontElement>();

#endregion

#region public method

        /// <summary>
        /// ゾーンを再度アセンブルします
        /// </summary>
        /// <param name="splitDenyZone"></param>
        /// <param name="halfIndexes"></param>
        public static void ReAssemble(ISplitDenyZone splitDenyZone, int[] halfIndexes)
        {
            var catalog = splitDenyZone.Catalog;
            var option = splitDenyZone.Option;
            var specs = splitDenyZone.Specs;
            var mainRenderingElements = new MainRenderingElement[specs.MainsCount];
            var mainsIndex = 0;

            foreach (var group in specs.Groups)
            {
                if (group.RubiesCount == 0)
                {
                    var spacing = new Spacing();

                    CreateTemporaryMainRenderingElement(group, catalog, option, spacing);
                    // ルビが存在しない場合はRenderingElementsに含まれないよう前回のキャッシュを消しておく
                    cachedTemporaryRubyElements.Clear();
                }
                else
                {
                    var spacing = CalculateSpacing(group, catalog, option, halfIndexes);
                    CreateTemporaryMainRenderingElement(group, catalog, option, spacing, halfIndexes);
                    CreateTemporarySubRenderingFontElement(group, catalog, spacing);
                }

                mainsIndex += CreateMainRenderingElementFromTemporary(ref mainRenderingElements, mainsIndex, group.MainsCount);
            }

            if (splitDenyZone is MinimumSplitDenyZone)
            {
                ((MinimumSplitDenyZone) splitDenyZone).Update(mainRenderingElements[0]);
            }
            else if (splitDenyZone is SplitDenyZone)
            {
                ((SplitDenyZone) splitDenyZone).Update(mainRenderingElements);
            }
        }

        /// <summary>
        /// 引数から渡されたデータを使用して、具体的なピクセル数や幅や高さ・スペーシングなどを持ったLineRenderingElementsを作成します
        /// </summary>
        /// <param name="data">論理行</param>
        /// <param name="catalog">グリフの辞書</param>
        /// <param name="option">作成ルールを調整するオプション</param>
        /// <returns></returns>
        public static LineRenderingElements Assemble(List<SplitDenyGlyphSpecs> data, GlyphCatalog catalog, AssembleOption option)
        {
            var dataCount = data.Count;
            var zones = new ISplitDenyZone[dataCount];

            for (var i = 0; i < dataCount; ++i)
            {
                var specs = data[i];
                var mainRenderingElements = new MainRenderingElement[specs.MainsCount];
                var mainsIndex = 0;

                foreach (var group in specs.Groups)
                {
                    if (group.RubiesCount == 0)
                    {
                        var spacing = new Spacing();

                        CreateTemporaryMainRenderingElement(group, catalog, option, spacing);
                        // ルビが存在しない場合はRenderingElementsに含まれないよう前回のキャッシュを消しておく
                        cachedTemporaryRubyElements.Clear();
                    }
                    else
                    {
                        var spacing = CalculateSpacing(group, catalog, option, null);
                        CreateTemporaryMainRenderingElement(group, catalog, option, spacing);
                        CreateTemporarySubRenderingFontElement(group, catalog, spacing);
                    }

                    mainsIndex += CreateMainRenderingElementFromTemporary(ref mainRenderingElements, mainsIndex, group.MainsCount);
                }

                if (mainRenderingElements.Length == 1)
                {
                    zones[i] = new MinimumSplitDenyZone(mainRenderingElements[0], specs, catalog, option);
                }
                else
                {
                    zones[i] = new SplitDenyZone(mainRenderingElements, specs, catalog, option);
                }
            }

            return new LineRenderingElements(zones);
        }

#endregion

#region protected method

        protected static float GetMainTextWidth(IGlyphSpecGroup group, GlyphCatalog catalog, AssembleOption option, int[] halfIndexes)
        {
            var baseTextWidth = 0.0f;

            var index = 0;
            foreach (var glyphPlacementSpec in group.Mains)
            {
                var width = 0.0f;
                var glyphSpec = glyphPlacementSpec.GlyphSpec as FontGlyphSpec;
                var inlineImageGlyphSpec = glyphPlacementSpec.GlyphSpec as ImageGlyphSpec;
                if (glyphSpec != null)
                {
                    width = catalog.Get(glyphSpec).Width;
                }
                else if (inlineImageGlyphSpec != null)
                {
                    var largeMGlyph = catalog.Get(inlineImageGlyphSpec.SizeBaseGlyphSpec);
                    var basicImageSize = (int) Math.Round(largeMGlyph.RawWidth * 0.9f);
                    width = inlineImageGlyphSpec.Width.CalculatePixel(basicImageSize);
                }

                if (halfIndexes != null)
                {
                    foreach (var half in halfIndexes)
                    {
                        if (glyphPlacementSpec.Index == half)
                        {
                            width *= 0.5f;
                        }
                    }
                }

                // 最後の文字の場合、spacingを考慮しない
                if (index == (group.MainsCount - 1))
                {
                    baseTextWidth += width;
                }
                else
                {
                    var glyphSpacing = 0.0f;
                    switch (option.SpacingUnit)
                    {
                        case SpacingUnit.BeforeCharacterWidth:
                            // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                            glyphSpacing = (int) (glyphPlacementSpec.BaseSpacing * width);
                            break;
                        case SpacingUnit.FontSize:
                            // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                            glyphSpacing = (int) (glyphPlacementSpec.BaseSpacing * option.FontSize) * option.GlyphScaleX;
                            break;
                        default:
                            Assert.IsTrue(false, "If you support all SpacingUnit, you will not reach here");
                            break;
                    }

                    baseTextWidth += width + glyphSpacing;
                }

                index++;
            }

            return baseTextWidth;
        }

        protected static float GetRubyTextWidth(IGlyphSpecGroup group, GlyphCatalog catalog)
        {
            var rubyTextWidth = 0.0f;

            foreach (var ruby in group.Rubies)
            {
                rubyTextWidth += catalog.Get(ruby).Width;
            }

            return rubyTextWidth;
        }

        protected static Spacing CalculateSpacing(IGlyphSpecGroup group, GlyphCatalog catalog, AssembleOption option, int[] halfIndexes)
        {
            var result = new Spacing();

            var baseTextWidth = GetMainTextWidth(group, catalog, option, halfIndexes);
            var rubyTextWidth = GetRubyTextWidth(group, catalog);

            if (baseTextWidth > rubyTextWidth)
            {
                var spaces = baseTextWidth - rubyTextWidth;
                result.rubySpacing = spaces / (group.RubiesCount * 2.0f);
            }
            else
            {
                var spaces = rubyTextWidth - baseTextWidth;
                result.rubyParentSpacing = spaces / (group.MainsCount * 2.0f);
            }

            return result;
        }

        protected static void CreateTemporaryMainRenderingElement(
            IGlyphSpecGroup group,
            GlyphCatalog catalog,
            AssembleOption option,
            Spacing spacing,
            int[] halfIndexes = null
        )
        {
            // 前回のRenderingElementsより要素数が多い場合、リストのリサイズが走る可能性があるため先にキャパシティを設定する
            if (cachedTemporaryMainElements.Count < group.MainsCount)
            {
                cachedTemporaryMainElements.Capacity = group.MainsCount;
                for (int i = cachedTemporaryMainElements.Count; i < group.MainsCount; ++i)
                {
                    cachedTemporaryMainElements.Add(new TemporaryMainRenderingElement());
                }
            }

            var index = 0;

            foreach (var bodyGlyph in group.Mains)
            {
                var glyphSpec = bodyGlyph.GlyphSpec;
                TemporaryMainRenderingElement element = cachedTemporaryMainElements[index];

                if (glyphSpec is FontGlyphSpec)
                {
                    var fontGlyphSpec = bodyGlyph.GlyphSpec as FontGlyphSpec;

                    // キャッシュされていない or ImageのOptionがキャッシュされている場合は新規でnewしてキャッシュする
                    TemporaryFontElementOption elementOption = element.ElementOption as TemporaryFontElementOption;
                    if (elementOption == null)
                    {
                        elementOption = new TemporaryFontElementOption();
                    }

                    var glyph = catalog.Get(fontGlyphSpec);
                    elementOption.glyph = glyph;
                    element.width = glyph.Width;
                    if (halfIndexes != null)
                    {
                        foreach (var half in halfIndexes)
                        {
                            if (bodyGlyph.Index == half)
                            {
                                element.width *= 0.5f;
                                break;
                            }
                        }
                    }

                    switch (option.SpacingUnit)
                    {
                        case SpacingUnit.BeforeCharacterWidth:
                            // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                            element.characterSpacingRight = (int) (bodyGlyph.BaseSpacing * element.width);
                            break;
                        case SpacingUnit.FontSize:
                            // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                            element.characterSpacingRight = (int) (bodyGlyph.BaseSpacing * option.FontSize) * option.GlyphScaleX;
                            break;
                        default:
                            Assert.IsTrue(false, "If you support all SpacingUnit, you will not reach here");
                            element.characterSpacingRight = 0;
                            break;
                    }

                    // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                    var rubySpacing = (int) (spacing.rubyParentSpacing * 2.0f);
                    element.rubySpacingLeft = (int) (rubySpacing / 2.0f);
                    element.rubySpacingRight = rubySpacing - element.rubySpacingLeft;
                    element.glyphIndex = bodyGlyph.Index;
                    element.option = option;

                    ThaiFontGlyphSpec thaiFontGlyphSpec = glyphSpec as ThaiFontGlyphSpec;
                    if (thaiFontGlyphSpec != null)
                    {
                        elementOption.offsetX = thaiFontGlyphSpec.OffsetX;
                        elementOption.offsetY = thaiFontGlyphSpec.OffsetY;
                    }
                    else
                    {
                        elementOption.offsetX = 0;
                        elementOption.offsetY = 0;
                    }

                    element.ElementOption = elementOption;
                }
                else if (glyphSpec is ImageGlyphSpec)
                {
                    var inlineImageGlyphSpec = bodyGlyph.GlyphSpec as ImageGlyphSpec;

                    // キャッシュされていない or FontのOptionがキャッシュされている場合は新規でnewしてキャッシュする
                    TemporaryImageElementOption elementOption = element.ElementOption as TemporaryImageElementOption;
                    if (elementOption == null)
                    {
                        elementOption = new TemporaryImageElementOption();
                    }

                    var sizeBaseGlyph = catalog.Get(inlineImageGlyphSpec.SizeBaseGlyphSpec);
                    var basicImageSize = (int) Math.Round(sizeBaseGlyph.RawWidth * 0.9f);
                    element.width = inlineImageGlyphSpec.Width.CalculatePixel(basicImageSize) * option.GlyphScaleX;
                    elementOption.height = inlineImageGlyphSpec.Height.CalculatePixel(basicImageSize);
                    elementOption.source = inlineImageGlyphSpec.Source;
                    elementOption.name = inlineImageGlyphSpec.Name;
                    switch (option.SpacingUnit)
                    {
                        case SpacingUnit.BeforeCharacterWidth:
                            // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                            element.characterSpacingRight = (int) (bodyGlyph.BaseSpacing * element.width);
                            break;
                        case SpacingUnit.FontSize:
                            // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                            element.characterSpacingRight = (int) (bodyGlyph.BaseSpacing * option.FontSize) * option.GlyphScaleX;
                            break;
                        default:
                            Assert.IsTrue(false, "If you support all SpacingUnit, you will not reach here");
                            element.characterSpacingRight = 0;
                            break;
                    }

                    // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                    var rubySpacing = (int) (spacing.rubyParentSpacing * 2.0f);
                    element.rubySpacingLeft = (int) (rubySpacing / 2.0f);
                    element.rubySpacingRight = rubySpacing - element.rubySpacingLeft;
                    elementOption.marginLeft = bodyGlyph.MarginLeft;
                    elementOption.marginRight = bodyGlyph.MarginRight;
                    elementOption.offsetY = bodyGlyph.OffsetY;
                    element.glyphIndex = bodyGlyph.Index;
                    element.option = option;

                    element.ElementOption = elementOption;
                }

                cachedTemporaryMainElements[index] = element;
                ++index;
            }
        }

        protected static void CreateTemporarySubRenderingFontElement(
            IGlyphSpecGroup group,
            GlyphCatalog catalog,
            Spacing spacing
        )
        {
            // 前回のGroupで使用したrubyが残っているのでClearする
            cachedTemporaryRubyElements.Clear();

            var baseTextHeight = 0.0f;
            foreach (var x in group.Mains)
            {
                var textHeight = 0.0f;
                if (x.GlyphSpec is FontGlyphSpec)
                {
                    textHeight = catalog.Get(x.GlyphSpec as FontGlyphSpec).Height;
                }
                else if (x.GlyphSpec is ImageGlyphSpec)
                {
                    var tempQualifier = x.GlyphSpec as ImageGlyphSpec;
                    var largeMGlyph = catalog.Get(tempQualifier.SizeBaseGlyphSpec);
                    var basicImageSize = (int) Math.Round(largeMGlyph.Width * 0.9f);
                    textHeight = tempQualifier.Height.CalculatePixel(basicImageSize);
                }

                if (textHeight > baseTextHeight)
                {
                    baseTextHeight = textHeight;
                }
            }

            // NOTE ルビのオフセット値は、横組でも縦組でも「(親文字に対するルビ文字の比率) * オフセット値」として計算されます
            var rubyOffsetY = baseTextHeight;
            if (group.RubyOffset != null && group.RubyScale != null)
            {
                rubyOffsetY = (int) Math.Floor(baseTextHeight * (1.0f + group.RubyOffset.Value * group.RubyScale.Value));
            }

            var rubyOffsetX = spacing.rubySpacing;
            var index = 0;
            var comsumedRubySpacing = 0.0f;
            foreach (var rubyGlyphSpec in group.Rubies)
            {
                // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                var totalRubySpacing = (int) (spacing.rubySpacing * 2 * (index + 1));
                var rubySpacing = totalRubySpacing - comsumedRubySpacing;

                var newRuby = new TemporarySubRenderingFontElement();
                newRuby.glyph = catalog.Get(rubyGlyphSpec);
                newRuby.width = newRuby.glyph.Width + rubySpacing;
                newRuby.glyphWidth = newRuby.glyph.Width;
                // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                newRuby.offsetX = (int) rubyOffsetX;
                newRuby.offsetY = -rubyOffsetY;

                cachedTemporaryRubyElements.Add(newRuby);

                rubyOffsetX += newRuby.width;
                comsumedRubySpacing = totalRubySpacing;
                ++index;
            }
        }

        protected static int CreateMainRenderingElementFromTemporary(
            ref MainRenderingElement[] mainRenderingElements,
            int index,
            int mainsLength)
        {
            var rubiesLength = cachedTemporaryRubyElements.Count;

            if (rubiesLength == 0)
            {
                for (var i = 0; i < mainsLength; i++)
                {
                    mainRenderingElements[index + i] = cachedTemporaryMainElements[i].CreateImmutable();
                }
            }
            else
            {
                List<SubRenderingFontElement> pairedRubies = new List<SubRenderingFontElement>();
                float bodyOffsetX = 0.0f;
                int placedRubyCount = 0;

                for (var i = 0; i < mainsLength; i++)
                {
                    float mainWidth = cachedTemporaryMainElements[i].TotalWidth;
                    pairedRubies.Clear();

                    if (i == (mainsLength - 1))
                    {
                        for (var j = placedRubyCount; j < rubiesLength; ++j)
                        {
                            var ruby = cachedTemporaryRubyElements[j];
                            pairedRubies.Add(new SubRenderingFontElement(
                                ruby.glyph,
                                ruby.width,
                                ruby.offsetX - bodyOffsetX,
                                ruby.offsetY));
                            ++placedRubyCount;
                        }
                    }
                    else
                    {
                        for (var j = placedRubyCount; j < rubiesLength; ++j)
                        {
                            var ruby = cachedTemporaryRubyElements[j];
                            if ((bodyOffsetX + mainWidth) >= (ruby.offsetX + ruby.glyphWidth))
                            {
                                pairedRubies.Add(new SubRenderingFontElement(
                                    ruby.glyph,
                                    ruby.width,
                                    ruby.offsetX - bodyOffsetX,
                                    ruby.offsetY));
                                ++placedRubyCount;
                            }
                        }
                    }

                    mainRenderingElements[index + i] = cachedTemporaryMainElements[i].CreateImmutable(pairedRubies);
                    bodyOffsetX += mainWidth;
                }
            }

            return mainsLength;
        }

#endregion
    }
}
