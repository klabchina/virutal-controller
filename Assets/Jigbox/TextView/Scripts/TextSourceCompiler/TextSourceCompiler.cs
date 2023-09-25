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
using ThaiUtils;
using UnityEngine;

namespace Jigbox.TextView
{
    /// <summary>
    /// 論理行を生成するクラス
    /// </summary>
    public class TextSourceCompiler
    {
#region properties & fields

        /// <summary>TextFormatterで扱うプロパティの参照クラス</summary>
        protected readonly TextSourceCompilerProperty compilerProperty;

        /// <summary>TextFormatterで扱うプロパティの参照クラス</summary>
        protected TextSourceCompilerProperty CompilerProperty { get { return compilerProperty; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="compilerProperty">TextViewのプロパティ参照クラス</param>
        public TextSourceCompiler(TextSourceCompilerProperty compilerProperty)
        {
            this.compilerProperty = compilerProperty;
        }

        /// <summary>
        /// 論理行を生成します
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual List<List<SplitDenyGlyphSpecs>> Compile(TextSource source)
        {
            var ptr = new TextRunPointer(source);

            if (ptr.EndOfSource)
            {
                return new List<List<SplitDenyGlyphSpecs>>();
            }

            var builder = new SplitDenyGlyphSpecsBuilder();
            var glyphIndex = 0;
            var textModifierScope = this.CreateTextModifierScope(null, null);

            while (!ptr.EndOfSource)
            {
                this.CompileSingleTextRun(ptr, builder, ref glyphIndex, ref textModifierScope);
            }

            return builder.Build();
        }

#endregion

#region protected methods

        /// <summary>
        /// 1つのTextRunをコンパイルします
        /// </summary>
        /// <param name="ptr">次にコンパイルすべきTextRunの位置を示すポインタ</param>
        /// <param name="builder">コンパイル結果を格納したビルダー</param>
        /// <param name="glyphIndex">コンパイル済みの本文文字数</param>
        /// <param name="textModifierScope">現在のテキスト修飾状態</param>
        protected virtual void CompileSingleTextRun(
            TextRunPointer ptr,
            SplitDenyGlyphSpecsBuilder builder,
            ref int glyphIndex,
            ref TextModifierScope textModifierScope
        )
        {
            if (ptr.Current is LineBreak)
            {
                builder.AddLineBreak();

                ptr.Next();
            }
            else if (ptr.Current is TextModifier)
            {
                var textModifier = (TextModifier) ptr.Current;
                textModifierScope = this.CreateTextModifierScope(textModifierScope, textModifier);
                builder.PushTextModifier(textModifier);

                ptr.Next();
            }
            else if (ptr.Current is TextEndOfSegment)
            {
                if (textModifierScope != null)
                {
                    textModifierScope = textModifierScope.Parent;
                }
                builder.PopTextModifier();

                ptr.Next();
            }
            else if (ptr.Current is TextAlignModifier)
            {
                compilerProperty.ModifyAlign = (ptr.Current as TextAlignModifier).Align;
                ptr.Next();
            }
            else if (ptr.Current is TextBiDirectionOverride)
            {
                ptr.Next();
            }
            else if (ptr.Current is TextCharacters)
            {
                var hasRuby = ptr.Current is TextCharactersRubyGroup;

                if (hasRuby)
                {
                    var rubyGroup = (TextCharactersRubyGroup) ptr.Current;

                    var rubyScope = textModifierScope as IRubyTextModifierScope;
                    var scope = rubyScope != null ? rubyScope.RubyScope : textModifierScope;

                    var rubySize = scope.FontSize.Value;
                    var rubyStyle = scope.FontStyle.Value;
                    var rubyColor = scope.ModifiedColor;

                    builder.StartRuby(scope);

                    foreach (var rubyCharacter in rubyGroup.RawRubyCharacters)
                    {
                        builder.AddRubyFontRequestSpec(
                            new FontGlyphSpec(
                                rubyCharacter,
                                compilerProperty.Font,
                                rubySize,
                                rubyStyle,
                                rubyColor,
                                compilerProperty.GlyphScaleX
                            )
                        );
                    }
                }

                var baseSpacing = textModifierScope.Spacing.Value;
                var letterCase = textModifierScope.LetterCase.Value;

                var characters = ((TextCharacters) ptr.Current).RawCharacters.ToCharArray();

                if (letterCase == LetterCase.Capitalize)
                {
                    characters = ToCapitalize(characters, ptr);
                }

                var size = textModifierScope.FontSize.Value;
                var smallCapsSize = letterCase == LetterCase.SmallCaps ? (int) (size * LetterCaseUtils.SmallCapsMultiplier) : 0;
                var style = textModifierScope.FontStyle.Value;
                var color = textModifierScope.ModifiedColor;

                while (true)
                {
                    if (ptr.Next())
                    {
                        break;
                    }

                    var localSize = size;
                    GetLetterCaseText(letterCase, smallCapsSize, ref characters[ptr.Position], ref localSize);

                    builder.AddMainGlyphPlacementSpec(
                        new MainGlyphPlacementSpec(
                            new FontGlyphSpec(characters[ptr.Position], compilerProperty.Font, localSize, style, color, compilerProperty.GlyphScaleX),
                            0,
                            0,
                            0,
                            glyphIndex++,
                            baseSpacing
                        )
                    );
                }

                if (hasRuby)
                {
                    builder.EndRuby();
                }
            }
            else if (ptr.Current is InlineImage)
            {
                var inlineImage = (InlineImage) ptr.Current;

                var marginLeft = Math.Max(IntParseNullableFloatValue(inlineImage.MarginLeftWithPixel), 0);
                var marginRight = Math.Max(IntParseNullableFloatValue(inlineImage.MarginRightWithPixel), 0);

                // offsetYの値は最終的にGlyphPlacement.Yに反映されるが、GlyphPlacement.Yは正数だと下方に移動、負数だと上方に移動という形で使用されている。
                // offsetYに入ってくる値の場合は正数だと上方、負数だと下方に移動させたいためここで符号反転を行っている
                var offsetY = -IntParseNullableFloatValue(inlineImage.OffsetYWithPixel);

                var size = textModifierScope.FontSize.Value;
                var style = textModifierScope.FontStyle.Value;
                var color = textModifierScope.ModifiedColor;

                var sizeBaseGlyphSpec = new FontGlyphSpec('M', compilerProperty.Font, size, style, color, compilerProperty.GlyphScaleX);

                builder.AddMainGlyphPlacementSpec(
                    new MainGlyphPlacementSpec(
                        new ImageGlyphSpec(inlineImage, sizeBaseGlyphSpec),
                        marginLeft,
                        marginRight,
                        offsetY,
                        glyphIndex++,
                        textModifierScope.Spacing.Value
                    )
                );

                ptr.NextChunk();
            } else if (ptr.Current is ThaiTextCharacters)
            {
                ThaiTextCharacters thaiCharacters = ptr.Current as ThaiTextCharacters;

                float baseSpacing = textModifierScope.Spacing.Value;

                int size = textModifierScope.FontSize.Value;
                FontStyle style = textModifierScope.FontStyle.Value;
                Color? color = textModifierScope.ModifiedColor;

                ptr.Next();

                foreach (TextUnit text in thaiCharacters.Texts)
                {
                    char[] characters = text.data;
                    if (text.isLayouted)
                    {
                        // 母音が途中で切れたりしないように、Spanを入れる
                        if (text.isStartSpan)
                        {
                            TextModifier spanGroup = new TextCharactersSpanGroup();
                            textModifierScope = CreateTextModifierScope(textModifierScope, spanGroup);
                            builder.PushTextModifier(spanGroup);
                        }

                        for (int i = 0; i < characters.Length; ++i)
                        {
                            builder.AddMainGlyphPlacementSpec(
                                new MainGlyphPlacementSpec(
                                    new ThaiFontGlyphSpec(characters[i], compilerProperty.Font, size, style, color, compilerProperty.GlyphScaleX,
                                        text.offsets[i].x, text.offsets[i].y),
                                    0,
                                    0,
                                    0,
                                    glyphIndex++,
                                    baseSpacing));
                            ptr.Next();
                        }

                        // Spanを外す
                        if (text.isEndSpan)
                        {
                            if (textModifierScope != null)
                            {
                                textModifierScope = textModifierScope.Parent;
                            }
                            builder.PopTextModifier();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < characters.Length; ++i)
                        {
                            builder.AddMainGlyphPlacementSpec(
                                new MainGlyphPlacementSpec(
                                    new FontGlyphSpec(characters[i], compilerProperty.Font, size, style, color, compilerProperty.GlyphScaleX),
                                    0,
                                    0,
                                    0,
                                    glyphIndex++,
                                    baseSpacing));
                            ptr.Next();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// null許容int型をint型に変換するヘルパーメソッドです
        /// </summary>
        /// <param name="nullableValue">null許容int型</param>
        /// <returns>int型に変換した値。nullの場合は0になります</returns>
        protected static int IntParseNullableFloatValue(float? nullableValue)
        {
            return nullableValue.HasValue ? (int) Math.Round(nullableValue.Value) : 0;
        }

        /// <summary>
        /// 文字を修飾するデフォルト値で初期化された TextModifier を生成します
        /// </summary>
        /// <returns>The default text modifier.</returns>
        protected virtual TextModifier CreateDefaultTextModifier()
        {
            return new TextModifier {
                FontSize = compilerProperty.FontSize,
                FontStyle = compilerProperty.DefaultFontStyle,
                Spacing = compilerProperty.CharacterSpacing,
                RubyFontScale = 0.5f,
                Color = compilerProperty.Color,
                LetterCase = compilerProperty.LetterCase
            };
        }

        /// <summary>
        /// 文字に施される修飾の範囲の親と修飾する内容を元に、 TextModiferScope を生成します
        /// </summary>
        /// <returns>The text modifier scope.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="modifier">Modifier.</param>
        protected virtual TextModifierScope CreateTextModifierScope(TextModifierScope parent, TextModifier modifier)
        {
            return new TextModifierScope(parent, (modifier ?? this.CreateDefaultTextModifier()));
        }

        /// <summary>
        /// レターケースに対応した文字を返す
        /// </summary>
        /// <returns>変換後の文字</returns>
        /// <param name="letterCase">レターケース.</param>
        /// <param name="smallCapsSize">スモールキャップス用サイズ.</param>
        /// <param name="character">元の文字.</param>
        /// <param name="size">サイズ.</param>
        protected void GetLetterCaseText(LetterCase letterCase, int smallCapsSize, ref char character, ref int size)
        {
            switch (letterCase)
            {
                case LetterCase.UpperCase:
                    character = LetterCaseUtils.ToUpper(character);
                    break;
                case LetterCase.LowerCase:
                    character = LetterCaseUtils.ToLower(character);
                    break;
                case LetterCase.SmallCaps:
                    if (LetterCaseUtils.IsLower(character))
                    {
                        size = smallCapsSize;
                        character = LetterCaseUtils.ToUpper(character);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 頭文字のみ大文字にした文字列を返す
        /// </summary>
        /// <returns>変換後の文字列</returns>
        /// <param name="characterList">元の文字列.</param>
        /// <param name="ptr">現在のテキスト.</param>
        protected char[] ToCapitalize(char[] characterList, TextRunPointer ptr)
        {
            string prevText = ptr.PreviousTextCharacters(1);
            char prevCharacter = ' ';
            char prev2Character = ' ';
            if (!string.IsNullOrEmpty(prevText))
            {
                prevCharacter = prevText[prevText.Length - 1];
                if (prevText.Length > 1)
                {
                    prev2Character = prevText[prevText.Length - 2];
                }
                else
                {
                    string prev2Text = ptr.PreviousTextCharacters(2);
                    if (!string.IsNullOrEmpty(prev2Text))
                    {
                        prev2Character = prev2Text[prev2Text.Length - 1];
                    }
                }
            }

            bool first = true;
            for (int i = 0; i < characterList.Length; i++)
            {
                if (LetterCaseUtils.IsLower(characterList[i]))
                {
                    // 大文字に変換する条件
                    // 条件1: 1つ前の文字が大文字、小文字、＿、:のいずれかでない
                    // 条件2: 1つ前の文字が:かつ2つ目前の文字が大文字か小文字でないなら
                    // 条件3: 現在の文字列の最初の文字かつ一つ前のTextRunが改行かインラインイメージ
                    if (!(LetterCaseUtils.IsUpper(prevCharacter) || LetterCaseUtils.IsLower(prevCharacter) || prevCharacter == LetterCaseUtils.UnderBarCode || prevCharacter == LetterCaseUtils.ColonCode)
                        || prevCharacter == LetterCaseUtils.ColonCode && !(LetterCaseUtils.IsUpper(prev2Character) || LetterCaseUtils.IsLower(prev2Character))
                        || (first && (ptr.Previous is LineBreak || ptr.Previous is InlineImage)))
                    {
                        characterList[i] = LetterCaseUtils.ToUpper(characterList[i]);
                    }
                }

                prev2Character = prevCharacter;
                prevCharacter = characterList[i];
                first = false;
            }
        
            return characterList;
        }

#endregion
    }
}
