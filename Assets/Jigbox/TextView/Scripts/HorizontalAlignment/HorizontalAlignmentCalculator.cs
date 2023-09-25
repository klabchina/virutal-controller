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
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Jigbox.TextView
{
	public class HorizontalAlignmentCalculator
	{
		/// <summary>
		/// <para>
		/// 	<paramref name="textLines"/>の各行に対して
		/// 	<paramref name="align"/>と<paramref name="justify"/>で指定された横方向アライメントを
		/// 	<paramref name="width"/>に収まる形で実現するために必要なX座標のオフセットを計算します。
		/// </para>
		/// <para>
		/// 	計算結果は<paramref name="textLines"/>の各行の<c>LineX</c>および<c>PlacedGlyphs.JustifyShiftOffsetX</c>に格納されます。
		/// 	前者は行内で共通のオフセットが、後者は各文字のオフセットが格納されます。
		/// </para>
		/// </summary>
		/// <param name="textLines"></param>
		/// <param name="align"></param>
		/// <param name="justify"></param>
		/// <param name="width"></param>
		public static void Apply(List<TextLine> textLines, TextAlign align, TextJustify justify, float width)
		{
			var lineCalculator = SelectLineCalculator(align);
			var glyphCalculator = SelectGlyphCalculator(align, justify);

			foreach (var textLine in textLines)
			{
				var remainingWidth = Mathf.Max(width - textLine.LineWidth(), 0.0f);
				
				lineCalculator(textLine, remainingWidth);
				glyphCalculator(textLine, remainingWidth);
			}
		}

#region Select Calculator

		protected static Action<TextLine, float> SelectLineCalculator(TextAlign align)
		{
			switch (align)
			{
				case TextAlign.Left:
				case TextAlign.Justify:
				case TextAlign.JustifyAll:
					return (textLine, remainingWidth) => textLine.LineX = 0.0f;
				case TextAlign.Center:
					return (textLine, remainingWidth) => textLine.LineX = remainingWidth / 2.0f;
				case TextAlign.Right:
					return (textLine, remainingWidth) => textLine.LineX = remainingWidth;
				default:
					Assert.IsTrue(false, "If you support all TextAlign, you will not reach here");
					return (textLine, remainingWidth) => textLine.LineX = 0.0f;
			}
		}

		protected static Action<TextLine, float> SelectGlyphCalculator(TextAlign align ,TextJustify justify)
		{
			switch (align)
			{
				case TextAlign.Left:
				case TextAlign.Center:
				case TextAlign.Right:
					return ZeroFillGlyphCalculator;
				case TextAlign.Justify:
				case TextAlign.JustifyAll:
					return JustifyGlyphCalculatorGenerator(align == TextAlign.JustifyAll, justify);
				default:
					Assert.IsTrue(false, "If you support all TextAlign, you will not reach here");
					return ZeroFillGlyphCalculator;
			}
		}
		
#endregion
		
		
#region GlyphCalculator Implementation

		protected static void ZeroFillGlyphCalculator(TextLine input, float width)
		{
			foreach (var glyph in input.PlacedGlyphs)
			{
				glyph.JustifyShiftOffsetX = 0.0f;
			}
		}
		
		protected static Action<TextLine, float> JustifyGlyphCalculatorGenerator(bool IsJusfifyAll, TextJustify justify)
		{
			Func<IEnumerable<GlyphPlacement>, IList<IList<GlyphPlacement>>> splitter;

			switch (justify)
			{
				case TextJustify.Auto:
					splitter = AutoSplitter;
					break;
				case TextJustify.InterWord:
					splitter = InterWordSplitter;
					break;
				case TextJustify.InterCharacter:
					splitter = InterCharacterSplitter;
					break;
				default:
					Assert.IsTrue(false, "If you support all TextJustify, you will not reach here");
					splitter = AutoSplitter;
					break;
			}

			return (input, width) =>
			{
				if (IsJusfifyAll == false && input.IsLastLine)
				{
					ZeroFillGlyphCalculator(input, width);
					return;
				}
				
				var tokens = splitter(input.PlacedGlyphs);
				if (tokens.Count <= 1)
				{
					ZeroFillGlyphCalculator(input, width);
					return;
				}
				
				var offsetPerToken = width / (tokens.Count - 1);
				for (var i = 0; i < tokens.Count; i++)
				{
					var justifyOffsetX = offsetPerToken * i;
						
					foreach (var glyph in tokens[i])
					{
						glyph.JustifyShiftOffsetX = justifyOffsetX;
							
						foreach (var subGlyphPlacement in glyph.SubGlyphPlacements)
						{
							subGlyphPlacement.JustifyShiftOffsetX = justifyOffsetX;
						}
					}
				}
			};
		}

#endregion

#region Justify Splitter

		protected enum GlyphType
		{
			Space,
			AlphaNum,
			Other
		}

		protected static GlyphType GetGlyphType(IGlyph glyph)
		{
			if (glyph.IsWhiteSpaceOrControl)
			{
				return GlyphType.Space;
			}

			if (glyph is Glyph)
			{
				switch (CharUnicodeInfo.GetUnicodeCategory(((Glyph) glyph).Character))
				{
					case UnicodeCategory.DecimalDigitNumber:
					case UnicodeCategory.LowercaseLetter:
					case UnicodeCategory.UppercaseLetter:
						return GlyphType.AlphaNum;
					default:
						return GlyphType.Other;
				}
			}

			if (glyph is InlineImageGlyph)
			{
				return GlyphType.Other;
			}
			
			Assert.IsTrue(false, "If you support all IGlyph delivered classes, you will not reach here");
			return GlyphType.Other;
		}
		
		protected static IList<IList<GlyphPlacement>> AutoSplitter(IEnumerable<GlyphPlacement> glyphPlacements)
		{
			var result = new List<IList<GlyphPlacement>>();
			
			var unit = new List<GlyphPlacement>();
			var canAcceptHeadingSpace = true;
			var canAcceptAlphaNum = true;
			var canAcceptOther = true;

			foreach (var glyphPlacement in glyphPlacements)
			{
				if (glyphPlacement.IsMainGlyph == false)
				{
					continue;
				}

				var type = GetGlyphType(glyphPlacement.Glyph);

				switch (type)
				{
					case GlyphType.Space:
						if (canAcceptHeadingSpace)
						{							
							unit.Add(glyphPlacement);
							
							canAcceptHeadingSpace = true;
							canAcceptAlphaNum = true;
							canAcceptOther = true;
						}
						else
						{
							unit.Add(glyphPlacement);
							
							canAcceptHeadingSpace = false;
							canAcceptAlphaNum = false;
							canAcceptOther = false;
						}
						
						break;
						
					case GlyphType.AlphaNum:
						if (!canAcceptAlphaNum)
						{
							result.Add(unit);
							unit = new List<GlyphPlacement>();
						}
						
						unit.Add(glyphPlacement);

						canAcceptHeadingSpace = false;
						canAcceptAlphaNum = true;
						canAcceptOther = false;

						break;
												
					case GlyphType.Other:
						if (!canAcceptOther)
						{
							result.Add(unit);
							unit = new List<GlyphPlacement>();
						}
						
						unit.Add(glyphPlacement);

						canAcceptHeadingSpace = false;
						canAcceptAlphaNum = false;
						canAcceptOther = false;
						
						break;
				}
			}

			// 最後の要素を結果に含める
			result.Add(unit);

			return result;
		}
		
		protected static IList<IList<GlyphPlacement>> InterWordSplitter(IEnumerable<GlyphPlacement> glyphPlacements)
		{
			var result = new List<IList<GlyphPlacement>>();
			
			var unit = new List<GlyphPlacement>();
			var canAcceptHeadingSpace = true;
			var canAcceptNonSpace = true;

			foreach (var glyphPlacement in glyphPlacements)
			{
				if (glyphPlacement.IsMainGlyph == false)
				{
					continue;
				}
				
				var type = GetGlyphType(glyphPlacement.Glyph);

				switch (type)
				{
					case GlyphType.Space:
						if (canAcceptHeadingSpace)
						{							
							unit.Add(glyphPlacement);
							
							canAcceptHeadingSpace = true;
							canAcceptNonSpace = true;
						}
						else
						{
							unit.Add(glyphPlacement);
							
							canAcceptHeadingSpace = false;
							canAcceptNonSpace = false;
						}
						
						break;
						
					case GlyphType.AlphaNum:
					case GlyphType.Other:
						if (!canAcceptNonSpace)
						{
							result.Add(unit);
							unit = new List<GlyphPlacement>();
						}
						
						unit.Add(glyphPlacement);

						canAcceptHeadingSpace = false;
						canAcceptNonSpace = true;

						break;
				}
			}

			// 最後の要素を結果に含める
			result.Add(unit);

			return result;
		}
		
		protected static IList<IList<GlyphPlacement>> InterCharacterSplitter(IEnumerable<GlyphPlacement> glyphPlacements)
		{
			var result = new List<IList<GlyphPlacement>>();
			
			foreach (var glyphPlacement in glyphPlacements)
			{
				if (glyphPlacement.IsMainGlyph == false)
				{
					continue;
				}
				
				result.Add(new List<GlyphPlacement> { glyphPlacement });
			}

			return result;			
		}
	
#endregion

	}
}
