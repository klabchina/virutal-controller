/**
 * Additional Language Utility Library
 * Copyright(c) 2018 KLab, Inc. All Rights Reserved.
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
using System.Text;
using ThaiUtils.UnitParser;

namespace ThaiUtils.Layout
{
    /// <summary>
    /// タイ語のレイアウト生成モジュール
    /// </summary>
    public static class ThaiLayoutGenerator
    {
        /// <summary>レイアウトの数値を算出する際に基準となるフォントサイズ</summary>
        public static int BaseLayoutSize = 36;

        /// <summary>テキストの構成</summary>
        static List<TextUnit> layoutedUnits = new List<TextUnit>();

        /// <summary>テキストの構成</summary>
        public static List<TextUnit> LayoutedUnits { get { return layoutedUnits; } }

        /// <summary>キャレット参照単位の情報</summary>
        static List<CaretUnit> caretUnits = new List<CaretUnit>();

        /// <summary>キャレット参照単位の情報</summary>
        public static List<CaretUnit> CaretUnits { get { return caretUnits; } }

        /// <summary>StringBuilder</summary>
        static StringBuilder builder = new StringBuilder();

        /// <summary>構築中のレイアウト情報のキャッシュ</summary>
        static List<GlyphLayoutOffset> workspace = new List<GlyphLayoutOffset>();

        /// <summary>拡張用のレイアウト作成モジュール</summary>
        public static IThaiLayoutBuilder layoutBuilder = null;

        /// <summary>
        /// レイアウト情報込みの構成を生成します。
        /// </summary>
        /// <param name="text">変換前のテキスト</param>
        /// <param name="startIndex">変換開始位置のインデックス</param>
        /// <param name="characterTypes">文字の種類</param>
        /// <param name="units">最小構成単位の文字情報</param>
        /// <param name="convertedCharacterLength">変換処理が済んだ変換後のテキストの文字数</param>
        public static void Generate(string text,
            int startIndex,
            List<CharacterType> characterTypes,
            List<ThaiUnit> units,
            int convertedCharacterLength = 0)
        {
            layoutedUnits.Clear();
            caretUnits.Clear();

            bool isCreateCaretUnit = ThaiTextConverter.IsCreateCaretUnit;

            int layoutedLength = 0;
            for (int i = 0; i < units.Count; ++i)
            {
                TextUnit unit = null;
                if (units[i].length == 1)
                {
                    switch (characterTypes[layoutedLength])
                    {
                        // 上下にレイアウトする文字だけの場合は、全角スペースを追加してから処理する
                        case CharacterType.SaraAm:
                        case CharacterType.TopVowels:
                        case CharacterType.BottomVowels:
                        case CharacterType.ToneMarks:
                            unit = new TextUnit(new char[] { '　', text[startIndex + layoutedLength] },
                                new GlyphLayoutOffset[] { GlyphLayoutOffset.Zero, GlyphLayoutOffset.Zero });
                            if (isCreateCaretUnit)
                            {
                                caretUnits.Add(new CaretUnit(startIndex + layoutedLength, 1, convertedCharacterLength, 2));
                            }
                            break;
                        default:
                            unit = new TextUnit(new char[] { text[startIndex + layoutedLength] },
                                new GlyphLayoutOffset[] { GlyphLayoutOffset.Zero });
                            if (isCreateCaretUnit)
                            {
                                caretUnits.Add(new CaretUnit(startIndex + layoutedLength, 1, convertedCharacterLength, 1));
                            }
                            break;
                    }
                }
                else
                {
                    if (layoutBuilder == null)
                    {
                        unit = Build(text,
                            startIndex + layoutedLength,
                            units[i],
                            characterTypes,
                            layoutedLength,
                            isCreateCaretUnit,
                            caretUnits,
                            convertedCharacterLength);
                    }
                    else
                    {
                        unit = layoutBuilder.Build(text,
                            startIndex + layoutedLength,
                            units[i],
                            characterTypes,
                            layoutedLength,
                            isCreateCaretUnit,
                            caretUnits,
                            convertedCharacterLength);
                    }
                }

                if (i > 0 && !units[i - 1].onlyConsonant && units[i].onlyConsonant)
                {
                    // 続くデータが末子音と思われる場合、フラグを変更してグループ化する
                    layoutedUnits[layoutedUnits.Count - 1].isEndSpan = false;
                    unit.isStartSpan = false;
                }

                layoutedUnits.Add(unit);
                layoutedLength += units[i].length;
                convertedCharacterLength += unit.data.Length;
            }
        }

        /// <summary>
        /// レイアウト情報込みの構成を作成します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="textIndex">テキストの開始インデックス</param>
        /// <param name="unit">タイ語の最小構成情報</param>
        /// <param name="characterTypes">文字の種類</param>
        /// <param name="characterTypeIndex">文字の種類の開始インデックス</param>
        /// <param name="isCreateCaretUnit">キャレット参照単位の情報を生成するかどうか</param>
        /// <param name="caretUnits">キャレット参照単位の情報</param>
        /// <param name="convertedCharacterLength">変換処理が済んだ変換後のテキストの文字数</param>
        /// <returns>作成した構成情報を返します。</returns>
        static TextUnit Build(string text,
            int textIndex,
            ThaiUnit unit,
            List<CharacterType> characterTypes,
            int characterTypeIndex,
            bool isCreateCaretUnit,
            List<CaretUnit> caretUnits,
            int convertedCharacterLength)
        {
            builder.Length = 0;
            workspace.Clear();

            char consonants = (char) 0;
            bool isSecondLayer = false;
            bool isForthLayer = false;
            int caretIndex = textIndex;
            int lastCount = 0;

            for (int i = 0; i < unit.length; ++i)
            {
                int x = 0;
                int y = 0;
                char character = text[textIndex + i];
                CharacterType type = characterTypes[characterTypeIndex + i];
                switch (type)
                {
                    case CharacterType.Consonants:
                        // 文字の一部が下母音に置き換わる文字は、置き換わる部分を削除した専用文字に置き換える
                        if (unit.hasBottom)
                        {
                            if (character == (int) ThaiLetter.Consonants.YoYing)
                            {
                                character = (char) ThaiLetter.ExConstants.YoYing;
                            }
                            else if (character == (int) ThaiLetter.Consonants.ThoThan)
                            {
                                character = (char) ThaiLetter.ExConstants.ThoThan;
                            }
                        }

                        // 第三層からはみ出すレイアウトの文字のフラグを処理する
                        switch (character)
                        {
                            // 第二層(二重子音あり)
                            case (char) ThaiLetter.Consonants.PoPla:
                                // 二重子音の場合、第二層にはみ出すレイアウトの文字は、1文字目にしか来ないので、
                                // 母音、声調記号のレイアウトには影響しない
                                // ex) เปลี่ย
                                isSecondLayer = !unit.isDoubleConsonant;
                                break;
                            // 第二層
                            case (char) ThaiLetter.Consonants.FoFa:
                            case (char) ThaiLetter.Consonants.FoFan:
                                isSecondLayer = true;
                                break;
                            // 第四層
                            case (char) ThaiLetter.Consonants.DoChada:
                            case (char) ThaiLetter.Consonants.ToPatak:
                            case (char) ThaiLetter.Consonants.Ru:
                            case (char) ThaiLetter.Consonants.Lu:
                                isForthLayer = true;
                                break;
                            default:
                                break;
                        }

                        consonants = character;

                        if (isCreateCaretUnit)
                        {
                            if (lastCount != i)
                            {
                                int characterLength = i - lastCount;
                                caretUnits.Add(new CaretUnit(caretIndex, characterLength, convertedCharacterLength, characterLength));
                                caretIndex += characterLength;
                                convertedCharacterLength += characterLength;
                                lastCount = i;
                            }
                        }
                        break;
                    case CharacterType.VagueConsonants:
                        // レイアウト的には何もしなくていい

                        if (isCreateCaretUnit)
                        {
                            if (lastCount != i)
                            {
                                int characterLength = i - lastCount;
                                caretUnits.Add(new CaretUnit(caretIndex, characterLength, convertedCharacterLength, characterLength));
                                caretIndex += characterLength;
                                convertedCharacterLength += characterLength;
                                lastCount = i;
                            }
                        }
                        break;
                    case CharacterType.LeftVowels:
                        // レイアウト的には何もしなくていい

                        if (isCreateCaretUnit)
                        {
                            // 原則的にタイ語の音としての構成単位で左母音より前に入力される文字はない前提
                            caretUnits.Add(new CaretUnit(caretIndex, 1, convertedCharacterLength, 1));
                            ++caretIndex;
                            ++convertedCharacterLength;
                            lastCount = 1;
                        }
                        break;
                    case CharacterType.RightVowels:
                        bool isConvertLakkhangyao = character == (int) ThaiLetter.RightVowels.SaraAa
                            && (consonants == (int) ThaiLetter.Consonants.Ru || consonants == (int) ThaiLetter.Consonants.Lu);
                        // 特定の組み合わせの場合、文字の形状が変化する
                        if (isConvertLakkhangyao)
                        {
                            character = (char) ThaiLetter.RightVowels.Lakkhangyao;
                            // 上下のレイアウト調整は、オフセットでできるが、字幅の調整は専用のメタ情報で処理する必要があるため、
                            // ๅを使う場合に、フォント自体の字幅が足りてないので半角スペース入れて増やす
                            builder.Append(' ');
                            workspace.Add(GlyphLayoutOffset.Zero);
                        }

                        if (isCreateCaretUnit)
                        {
                            if (lastCount != i)
                            {
                                int characterLength = i - lastCount;
                                caretUnits.Add(new CaretUnit(caretIndex, characterLength, convertedCharacterLength, characterLength));
                                caretIndex += characterLength;
                                convertedCharacterLength += characterLength;
                            }

                            int convertedLength = isConvertLakkhangyao ? 2 : 1;
                            caretUnits.Add(new CaretUnit(caretIndex, 1, convertedCharacterLength, convertedLength));
                            caretIndex += convertedLength;
                            convertedCharacterLength += convertedLength;
                            lastCount = i;
                        }
                        break;
                    // 子音次第で分割が必要
                    case CharacterType.SaraAm:
                        // 第二層にはみ出している子音に付く場合、文字を分割する必要がある
                        if (isSecondLayer)
                        {
                            builder.Append((char) ThaiLetter.TopVowels.Nikhahit);
                            workspace.Add(new GlyphLayoutOffset(-7, 0));
                            character = (char) ThaiLetter.RightVowels.SaraAa;
                        }

                        if (isCreateCaretUnit)
                        {
                            // Unit化されていない部分＋SaraAm
                            int characterLength = i - lastCount + 1;
                            int convertedLength = isSecondLayer ? characterLength + 1 : characterLength;
                            caretUnits.Add(new CaretUnit(caretIndex, characterLength, convertedCharacterLength, convertedLength));
                            caretIndex += characterLength;
                            convertedCharacterLength += convertedLength;
                            lastCount = i;
                        }
                        break;
                    case CharacterType.TopVowels:
                        // 第二層にはみ出している子音に付く場合、文字を横にずらす必要がある
                        if (isSecondLayer)
                        {
                            x -= 7;
                        }
                        break;
                    case CharacterType.BottomVowels:
                        // 第四層にはみ出している子音に付く場合のみ、位置を下げる必要がある
                        if (isForthLayer)
                        {
                            y += 11;
                        }
                        break;
                    case CharacterType.ToneMarks:
                        // 第二層にはみ出している子音に付く場合、文字を横にずらす必要がある
                        if (isSecondLayer)
                        {
                            x -= 7;
                        }
                        // 上にレイアウトする文字が２つ付いている場合は、更に上にずらす必要がある
                        if (unit.hasTopDouble)
                        {
                            y -= 13;
                        }
                        break;
                }

                builder.Append(character);
                workspace.Add(new GlyphLayoutOffset(x, y));
            }

            if (isCreateCaretUnit)
            {
                if (caretIndex != textIndex + unit.length)
                {
                    int characterLength = unit.length - lastCount;
                    caretUnits.Add(new CaretUnit(caretIndex, characterLength, convertedCharacterLength, characterLength));
                }
            }

            return new TextUnit(builder.ToString().ToCharArray(), workspace.ToArray());
        }
    }
}
