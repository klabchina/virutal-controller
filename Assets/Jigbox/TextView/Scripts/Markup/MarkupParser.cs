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
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Jigbox.TextView.Markup
{
    public class MarkupParser : MarkupParserBase
    {
        readonly static Dictionary<string, Color> presetColors =
            new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase) {
                { "red",     Color.red },
                { "green",   Color.green },
                { "blue",    Color.blue },
                { "white",   Color.white },
                { "black",   Color.black },
                { "yellow",  Color.yellow },
                { "cyan",    Color.cyan },
                { "magenta", Color.magenta },
                { "gray",    Color.gray },
                { "grey",    Color.grey },
                { "clear",   Color.clear },
            };

        readonly static Regex pixelValueRegex = new Regex("^-?[0-9]+$");
        readonly static string pixelSuffix = "px";

        protected override TextRun[] VisitMarkupElement(Element element, string tagNameUpper)
        {
            switch (tagNameUpper)
            {
                case "SPAN":
                    return SpanElement(element, tagNameUpper);
                case "RUBY":
                    return RubyElement(element, tagNameUpper);
                case "COLOR":
                    return ColoredElement(element, tagNameUpper);
                case "SIZE":
                    return SizedElement(element, tagNameUpper);
                case "I":
                    return ItalicElement(element, tagNameUpper);
                case "B":
                    return BoldElement(element, tagNameUpper);
                case "IMG":
                    return ImageElement(element, tagNameUpper);
                case "ALIGN":
                    return AlignElement(element, tagNameUpper);
                case "BDO":
                    return BiDirectionOverrideElement(element, tagNameUpper);
                case "UPPERCASE":
                    return UpperCaseElement(element, tagNameUpper);
                case "LOWERCASE":
                    return LowerCaseElement(element, tagNameUpper);
                case "CAPITALIZE":
                    return CapitalizeElement(element, tagNameUpper);
                case "SMALLCAPS":
                    return SmallCapsElement(element, tagNameUpper);
                default:
                    return base.VisitMarkupElement(element, tagNameUpper);
            }
        }

#region Parsing Basic Data Type Attribute Semantics

        static string TryGetStringAttribute(Element element, string attributeName)
        {
            if (element.AttributesCount == 0)
            {
                return null;
            }

            string data = null;
            if (element.Attributes.ContainsKey(attributeName))
            {
                data = element.GetAttribute(attributeName);
            }
            return data;
        }

        static int? TryGetIntAttribute(Element element, string attributeName)
        {
            if (element.AttributesCount == 0)
            {
                return null;
            }

            int? data = null;
            if (element.Attributes.ContainsKey(attributeName))
            {
                int tempData;
                if (Int32.TryParse(element.GetAttribute(attributeName), out tempData))
                {
                    data = tempData;
                }
            }
            return data;
        }

        static float? TryGetFloatAttribute(Element element, string attributeName)
        {
            if (element.AttributesCount == 0)
            {
                return null;
            }

            float? data = null;
            if (element.Attributes.ContainsKey(attributeName))
            {
                float tempData;
                if (Single.TryParse(element.GetAttribute(attributeName), out tempData))
                {
                    data = tempData;
                }
            }
            return data;
        }

        static int? TryGetIntAttributeByPixelSuffix(Element element, string attributeName)
        {
            if (element.AttributesCount == 0)
            {
                return null;
            }

            int? data = null;
            if (element.Attributes.ContainsKey(attributeName))
            {
                // トリミングした文字列
                var attribute = element.GetAttribute(attributeName).Trim();
                // 文字数がSuffixよりも上、かつ末尾がSuffixと一致するか(文字の大小は区別しない)
                if (attribute.Length > pixelSuffix.Length && string.Compare(attribute.Substring(attribute.Length - pixelSuffix.Length), pixelSuffix, true) == 0)
                {
                    // 数値部を切り出してTrimする
                    var numericStr = attribute.Substring(0, attribute.Length - pixelSuffix.Length).Trim();
                    // 正規表現に一致するか
                    if (pixelValueRegex.IsMatch(numericStr))
                    {
                        data = int.Parse(numericStr);
                    }
                }
            }
            return data;
        }

#endregion

#region Parsing Attribute Semantics

        static float? TryGetSpacingAttribute(Element element, string attributeName = "Spacing")
        {
            return TryGetFloatAttribute(element, attributeName);
        }

        static Color? TryGetColorAttribute(Element element, string attributeName = "Color")
        {
            Color color;
            string value = element.GetAttribute(attributeName);
            // 後方互換性維持(一般的な色の定義とUnityの色の定義がズレている)のため、
            // 直接色名を指定するパターンは元の定義を使って返すようにしている
            if (presetColors.ContainsKey(value))
            {
                color = presetColors[value];
            }
            else if (!ColorUtility.TryParseHtmlString(value, out color))
            {
                return null;
            }
            return color;
        }

        static int? TryGetFontSizeAttribute(Element element, string attributeName = "Size")
        {
            return TryGetIntAttribute(element, attributeName);
        }

        static float? TryGetFontScaleAttribute(Element element, string attributeName = "Scale")
        {
            return TryGetFloatAttribute(element, attributeName);
        }

        static float? TryGetRubyOffsetAttribute(Element element, string attributeName = "Offset")
        {
            return TryGetFloatAttribute(element, attributeName);
        }

        static string TryGetSourceAttribute(Element element, string attributeName = "Src")
        {
            return TryGetStringAttribute(element, attributeName);
        }
        static string TryGetNameAttribute(Element element, string attributeName = "Name")
        {
            return TryGetStringAttribute(element, attributeName);
        }

        static float? TryGetWidthAttribute(Element element, string attributeName = "Width")
        {
            return TryGetFloatAttribute(element, attributeName);
        }

        static float? TryGetHeightAttribute(Element element, string attributeName = "Height")
        {
            return TryGetFloatAttribute(element, attributeName);
        }

        static InlineImageSize TryGetWidthPixelAttribute(Element element, string attributeName = "Width")
        {
            var data = TryGetIntAttributeByPixelSuffix(element, attributeName);
            return data.HasValue ? new InlineImageSize(data.Value) : null;
        }

        static InlineImageSize TryGetHeightPixelAttribute(Element element, string attributeName = "Height")
        {
            var data = TryGetIntAttributeByPixelSuffix(element, attributeName);
            return data.HasValue ? new InlineImageSize(data.Value) : null;
        }

        static int? TryGetMarginLeftPixelAttribute(Element element, string attributeName = "Margin-Left")
        {
            return TryGetIntAttributeByPixelSuffix(element, attributeName);
        }

        static int? TryGetMarginRightPixelAttribute(Element element, string attributeName = "Margin-Right")
        {
            return TryGetIntAttributeByPixelSuffix(element, attributeName);
        }

        static int? TryGetOffsetYPixelAttribute(Element element, string attributeName = "OffsetY")
        {
            return TryGetIntAttributeByPixelSuffix(element, attributeName);
        }

        static bool CheckWidthUsePixelSuffix(Element element, string attributeName = "Width")
        {
            var attributes = element.Attributes;
            return attributes.ContainsKey(attributeName) && attributes[attributeName].ToUpper().Contains(pixelSuffix.ToUpper());
        }

        static bool CheckHeightUsePixelSuffix(Element element, string attributeName = "Height")
        {
            var attributes = element.Attributes;
            return attributes.ContainsKey(attributeName) && attributes[attributeName].ToUpper().Contains(pixelSuffix.ToUpper());
        }

        static InlineImageSize CreateWidthAttribute(Element element)
        {
            if (CheckWidthUsePixelSuffix(element))
            {
                var imageSize = TryGetWidthPixelAttribute(element);
                if (imageSize != null)
                {
                    return imageSize;
                }
            }
            else
            {
                var attr = TryGetWidthAttribute(element);
                if (attr.HasValue)
                {
                    return new InlineImageSize(attr.Value);
                }
            }

            return InlineImageSize.CreateByDefaultMagnificationValue();
        }

        static InlineImageSize CreateHeightAttribute(Element element)
        {
            if (CheckHeightUsePixelSuffix(element))
            {
                var imageSize = TryGetHeightPixelAttribute(element);
                if (imageSize != null)
                {
                    return imageSize;
                }
            }
            else
            {
                var attr = TryGetHeightAttribute(element);
                if (attr.HasValue)
                {
                    return new InlineImageSize(attr.Value);
                }
            }

            return InlineImageSize.CreateByDefaultMagnificationValue();
        }

#endregion

#region Semantic Analyze

        protected virtual TextRun[] SpanElement(Element element, string tagNameUpper)
        {
            var modifier = new TextCharactersSpanGroup() {
                Color = TryGetColorAttribute(element),
                FontSize = TryGetFontSizeAttribute(element),
                Spacing = TryGetSpacingAttribute(element),
            };
            return Packing(modifier, element, tagNameUpper);
        }

        protected virtual TextRun[] RubyElement(Element element, string tagNameUpper)
        {
            var modifier = new TextModifier() {
                RubyColor = TryGetColorAttribute(element),
                RubyFontScale = TryGetFontScaleAttribute(element),
                RubyOffset = TryGetRubyOffsetAttribute(element),
                FontSize = TryGetFontSizeAttribute(element),
                Spacing = TryGetSpacingAttribute(element),
            };
            return Packing(modifier, element, tagNameUpper);
        }

        protected virtual TextRun[] ColoredElement(Element element, string tagNameUpper)
        {
            var modifier = new TextModifier() {
                Color = TryGetColorAttribute(element, "Value"),
                FontSize = TryGetFontSizeAttribute(element),
                Spacing = TryGetSpacingAttribute(element),
            };
            return Packing(modifier, element, tagNameUpper);
        }

        protected virtual TextRun[] SizedElement(Element element, string tagNameUpper)
        {
            var modifier = new TextModifier() {
                FontSize = TryGetFontSizeAttribute(element, "Value"),
                Color = TryGetColorAttribute(element),
                Spacing = TryGetSpacingAttribute(element),
            };
            return Packing(modifier, element, tagNameUpper);
        }

        protected virtual TextRun[] BoldElement(Element element, string tagNameUpper)
        {
            var modifier = new TextModifier() {
                FontStyle = FontStyle.Bold,
                Color = TryGetColorAttribute(element),
                FontSize = TryGetFontSizeAttribute(element),
                Spacing = TryGetSpacingAttribute(element),
            };
            return Packing(modifier, element, tagNameUpper);
        }

        protected virtual TextRun[] ItalicElement(Element element, string tagNameUpper)
        {
            var modifier = new TextModifier() {
                FontStyle = FontStyle.Italic,
                Color = TryGetColorAttribute(element),
                FontSize = TryGetFontSizeAttribute(element),
                Spacing = TryGetSpacingAttribute(element),
            };
            return Packing(modifier, element, tagNameUpper);
        }

        protected virtual TextRun[] ImageElement(Element element, string tagNameUpper)
        {
            return new TextRun[] {
                new InlineImage() {
                    Source = TryGetSourceAttribute(element),
                    Name = TryGetNameAttribute(element),
                    Width = CreateWidthAttribute(element),
                    Height = CreateHeightAttribute(element),
                    MarginLeftWithPixel = TryGetMarginLeftPixelAttribute(element),
                    MarginRightWithPixel = TryGetMarginRightPixelAttribute(element),
                    OffsetYWithPixel = TryGetOffsetYPixelAttribute(element)
                }
            };
        }

        protected virtual TextRun[] AlignElement(Element element, string tagNameUpper)
        {
            return new TextRun[]
            {
                new TextAlignModifier(element.GetAttribute("value"))
            };
        }

        protected virtual TextRun[] BiDirectionOverrideElement(Element element, string tagNameUpper)
        {
            return new TextRun[]
            {
                new TextBiDirectionOverride(element.GetAttribute("dir", "auto")),
            };
        }
        
        protected virtual TextRun[] UpperCaseElement(Element element, string tagNameUpper)
        {
            var modifier = new TextModifier() {
                LetterCase = LetterCase.UpperCase
            };
            return Packing(modifier, element, tagNameUpper);
        }
        
        protected virtual TextRun[] LowerCaseElement(Element element, string tagNameUpper)
        {
            var modifier = new TextModifier() {
                LetterCase = LetterCase.LowerCase
            };
            return Packing(modifier, element, tagNameUpper);
        }
        
        protected virtual TextRun[] CapitalizeElement(Element element, string tagNameUpper)
        {
            var modifier = new TextModifier() {
                LetterCase = LetterCase.Capitalize
            };
            return Packing(modifier, element, tagNameUpper);
        }
        
        protected virtual TextRun[] SmallCapsElement(Element element, string tagNameUpper)
        {
            var modifier = new TextModifier() {
                LetterCase = LetterCase.SmallCaps
            };
            return Packing(modifier, element, tagNameUpper);
        }

#endregion

        TextRun[] Packing(TextRun head, Element element, string tagNameUpper)
        {
            TextRun[] children = base.VisitMarkupElement(element, tagNameUpper);
            TextRun[] result = new TextRun[children.Length + 2];

            // 先頭タグのModifier、末尾はTextEndOfSegment固定
            result[0] = head;
            result[result.Length - 1] = new TextEndOfSegment();

            for (int i = 1; i - 1 < children.Length; ++i)
            {
                result[i] = children[i - 1];
            }

            return result;
        }
    }
}
