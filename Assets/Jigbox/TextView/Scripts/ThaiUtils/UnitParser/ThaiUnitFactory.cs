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

namespace ThaiUtils.UnitParser
{
    /// <summary>
    /// 最小構成を生成するためのモジュール
    /// </summary>
    public static class ThaiUnitFactory
    {
        /// <summary>左母音の数</summary>
        static int leftVowelCount = 0;

        /// <summary>子音の数</summary>
        static int consonantCount = 0;

        /// <summary>SaraAmの数</summary>
        static int saraAmCount = 0;

        /// <summary>上母音の数</summary>
        static int topVowelCount = 0;

        /// <summary>声調記号の数</summary>
        static int toneMarkCount = 0;

        /// <summary>右母音の数</summary>
        static int rightVowelCount = 0;

        /// <summary>下母音の数</summary>
        static int bottomVowelCount = 0;

        /// <summary>文字列の長さ</summary>
        static int length = 0;

        /// <summary>文字列の長さ</summary>
        public static int Length { get { return length; } }

        /// <summary>二重子音かどうか</summary>
        static bool isDoubleConsonant = false;

        /// <summary>直前の文字</summary>
        static char lastCharacter = '\0';

        /// <summary>直前の文字の種類</summary>
        static CharacterType lastType = CharacterType.Other;

        /// <summary>文字の入力ルール</summary>
        static Dictionary<CharacterType, HashSet<CharacterType>> unitRules = new Dictionary<CharacterType, HashSet<CharacterType>> {
            { CharacterType.Consonants, new HashSet<CharacterType>() { CharacterType.VagueConsonants, CharacterType.RightVowels, CharacterType.SaraAm, CharacterType.TopVowels, CharacterType.BottomVowels, CharacterType.ToneMarks } },
            { CharacterType.LeftVowels, new HashSet<CharacterType>() { CharacterType.Consonants, CharacterType.VagueConsonants } },
            { CharacterType.RightVowels, new HashSet<CharacterType>() { CharacterType.VagueConsonants, CharacterType.RightVowels } },
            { CharacterType.SaraAm, new HashSet<CharacterType>() { } },
            { CharacterType.TopVowels, new HashSet<CharacterType>() { CharacterType.VagueConsonants, CharacterType.RightVowels, CharacterType.BottomVowels, CharacterType.ToneMarks } },
            { CharacterType.BottomVowels, new HashSet<CharacterType>() { CharacterType.VagueConsonants, CharacterType.RightVowels, CharacterType.SaraAm, CharacterType.TopVowels, CharacterType.ToneMarks } },
            { CharacterType.ToneMarks, new HashSet<CharacterType>() { CharacterType.VagueConsonants, CharacterType.RightVowels, CharacterType.SaraAm, CharacterType.TopVowels, CharacterType.BottomVowels } },
            { CharacterType.Other, new HashSet<CharacterType>() { CharacterType.Consonants, CharacterType.VagueConsonants, CharacterType.LeftVowels } },
        };

        /// <summary>
        /// 内部でキャッシュしている値をクリアして、初期化します。
        /// </summary>
        public static void Refresh()
        {
            leftVowelCount = 0;
            consonantCount = 0;
            saraAmCount = 0;
            topVowelCount = 0;
            toneMarkCount = 0;
            rightVowelCount = 0;
            bottomVowelCount = 0;
            length = 0;
            isDoubleConsonant = false;
            lastCharacter = '\0';
            lastType = CharacterType.Other;
        }

        /// <summary>
        /// 文字を追加します。
        /// </summary>
        /// <param name="character">追加する文字</param>
        /// <param name="type">追加する文字の種類</param>
        /// <param name="nextCharacter">次に追加する文字</param>
        /// <param name="nextType">追加する文字の次の文字の種類</param>
        /// <returns>追加できた場合、<c>true</c>を返します。</returns>
        public static bool Push(char character, CharacterType type, char nextCharacter, CharacterType nextType)
        {
            bool isDoubleConsonant = false;
            // 子音が連続する場合
            if (lastType == CharacterType.Consonants
                && (type == CharacterType.Consonants || type == CharacterType.VagueConsonants))
            {
                isDoubleConsonant = ThaiTable.IsDoubleConsonants(lastCharacter, character);
            }

            if (isDoubleConsonant)
            {
                // 二重子音の場合は、強制的に子音文字扱いして処理する
                type = CharacterType.Consonants;
                ThaiUnitFactory.isDoubleConsonant = true;
            }
            else if (!unitRules[lastType].Contains(type))
            {
                return false;
            }

            // 文字数をカウントアップ
            switch (type)
            {
                case CharacterType.Consonants:
                    ++consonantCount;
                    ++length;
                    lastType = CharacterType.Consonants;
                    break;
                case CharacterType.VagueConsonants:
                    if (consonantCount == 0)
                    {
                        ++consonantCount;
                        ++length;
                        lastType = CharacterType.Consonants;
                    }
                    else
                    {
                        // すでに右に付ける母音が2つ付いてる場合は、結合できない
                        if (rightVowelCount == 2)
                        {
                            return false;
                        }

                        // 次に来る文字が母音、声調記号の場合は、そちらと合わせるようにする
                        switch (nextType)
                        {
                            case CharacterType.RightVowels:
                            case CharacterType.SaraAm:
                            case CharacterType.TopVowels:
                            case CharacterType.BottomVowels:
                            case CharacterType.ToneMarks:
                                return false;
                            case CharacterType.VagueConsonants:
                                // 判定がおかしくなりやすい例外パターン
                                // 母音にも子音にもなる文字の内、ย、วは、2文字目の右母音になるパターンがあるが、อは1文字目にしかこない
                                if (rightVowelCount > 0 && nextCharacter == (char) ThaiLetter.VagueConsonants.OAng)
                                {
                                    return false;
                                }
                                break;
                        }

                        ++rightVowelCount;
                        ++length;
                        lastType = CharacterType.RightVowels;
                    }
                    break;
                case CharacterType.LeftVowels:
                    ++leftVowelCount;
                    ++length;
                    lastType = CharacterType.LeftVowels;
                    break;
                case CharacterType.RightVowels:
                    // すでに右に付ける母音が2つ付いてる場合は、結合できない
                    if (rightVowelCount == 2)
                    {
                        return false;
                    }
                    ++rightVowelCount;
                    ++length;
                    lastType = CharacterType.RightVowels;
                    break;
                case CharacterType.SaraAm:
                    // すでに上につく母音が存在する場合は、結合できない
                    if (topVowelCount > 0)
                    {
                        return false;
                    }
                    ++saraAmCount;
                    ++rightVowelCount;
                    ++topVowelCount;
                    ++length;
                    lastType = CharacterType.SaraAm;
                    break;
                case CharacterType.TopVowels:
                    // すでに右につく母音が存在する場合は、結合できない
                    if (rightVowelCount > 0)
                    {
                        return false;
                    }
                    ++topVowelCount;
                    ++length;
                    lastType = CharacterType.TopVowels;
                    break;
                case CharacterType.BottomVowels:
                    // すでに右につく母音が存在する場合は、結合できない
                    if (rightVowelCount > 0)
                    {
                        return false;
                    }
                    ++bottomVowelCount;
                    ++length;
                    lastType = CharacterType.BottomVowels;
                    break;
                case CharacterType.ToneMarks:
                    // すでに右につく母音が存在する場合は、結合できない
                    if (rightVowelCount > 0)
                    {
                        return false;
                    }
                    ++toneMarkCount;
                    ++length;
                    lastType = CharacterType.ToneMarks;
                    break;
                case CharacterType.Other:
                    // ここに来ることはないはず
                    break;
            }

            // 右に付けられる母音は2文字まで
            // 上に付けられる母音は1文字まで
            // 下に付けられる母音は1文字まで
            // 声調記号は1文字まで
            if (rightVowelCount > 2
                || topVowelCount > 1
                || bottomVowelCount > 1
                || toneMarkCount > 1)
            {
                --length;
                return false;
            }

            lastCharacter = character;

            return true;
        }

        /// <summary>
        /// 最小構成の情報を取得します。
        /// </summary>
        /// <returns>現在設定されている情報から最小構成の情報を生成して返します。</returns>
        public static ThaiUnit Get()
        {
            return length > 0
                ? new ThaiUnit(length,
                (leftVowelCount + topVowelCount + bottomVowelCount + rightVowelCount) == 0 && consonantCount == 1,
                (topVowelCount + toneMarkCount) == 2,
                bottomVowelCount > 0,
                saraAmCount > 0,
                isDoubleConsonant)
                : new ThaiUnit();
        }
    }
}
