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

using System;
using System.Collections.Generic;

namespace ThaiUtils
{
    /// <summary>
    /// タイ語のテーブルデータ
    /// </summary>
    public static class ThaiTable
    {
#region properties

        /// <summary>レイアウトが必要なタイ語のテーブル</summary>
        static HashSet<int> thaiCharacters;

        /// <summary>子音のテーブル</summary>
        static HashSet<int> consonants;

        /// <summary>母音になる可能性のある子音のテーブル</summary>
        static HashSet<int> vagueConsonants;

        /// <summary>子音の左に付く母音のテーブル</summary>
        static HashSet<int> leftVowels;

        /// <summary>子音の右に付く母音のテーブル</summary>
        static HashSet<int> rightVowels;

        /// <summary>子音の右に付く母音で上に付く母音を含む特殊なもののテーブル</summary>
        static HashSet<int> saraAm;

        /// <summary>子音の上に付く母音のテーブル</summary>
        static HashSet<int> topVowels;

        /// <summary>子音の下に付く母音のテーブル</summary>
        static HashSet<int> bottomVowels;

        /// <summary>声調記号のテーブル</summary>
        static HashSet<int> toneMarks;

        /// <summary>二重子音対応テーブル</summary>
        static Dictionary<int, HashSet<int>> doubleConsonants;

#endregion

#region public methods

        /// <summary>
        /// レイアウトが必要かどうかを返します。
        /// </summary>
        /// <param name="character">文字</param>
        /// <returns>レイアウトが必要な文字であれば、<c>true</c>を返します。</returns>
        public static bool IsLayoutNeeded(char character)
        {
            return thaiCharacters.Contains(character);
        }

        /// <summary>
        /// タイ語としてレイアウトが必要かどうかを返します。
        /// </summary>
        /// <param name="str">文字文字列</param>
        /// <returns>レイアウトが必要な文字が含まれていれば、<c>true</c>を返します。</returns>
        public static bool IsLayoutNeeded(string str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                if (IsLayoutNeeded(str[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 文字の種類を取得します。
        /// </summary>
        /// <param name="character">文字</param>
        /// <returns>文字の種類を返します。</returns>
        public static CharacterType GetType(char character)
        {
            if (consonants.Contains(character))
            {
                return CharacterType.Consonants;
            }
            if (vagueConsonants.Contains(character))
            {
                return CharacterType.VagueConsonants;
            }
            if (leftVowels.Contains(character))
            {
                return CharacterType.LeftVowels;
            }
            if (rightVowels.Contains(character))
            {
                return CharacterType.RightVowels;
            }
            if (saraAm.Contains(character))
            {
                return CharacterType.SaraAm;
            }
            if (topVowels.Contains(character))
            {
                return CharacterType.TopVowels;
            }
            if (bottomVowels.Contains(character))
            {
                return CharacterType.BottomVowels;
            }
            if (toneMarks.Contains(character))
            {
                return CharacterType.ToneMarks;
            }

            return CharacterType.Other;
        }

        /// <summary>
        /// 二重子音となる組み合わせかどうかを返します。
        /// </summary>
        /// <param name="first">第一子音文字</param>
        /// <param name="second">第二子音文字</param>
        /// <returns>二重子音であれば、<c>true</c>を返します。</returns>
        public static bool IsDoubleConsonants(char first, char second)
        {
            HashSet<int> secondTable = null;
            if (doubleConsonants.TryGetValue(first, out secondTable))
            {
                return secondTable.Contains(second);
            }
            else
            {
                return false;
            }
        }

#endregion

#region private methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static ThaiTable()
        {
            thaiCharacters = new HashSet<int>();
            consonants = new HashSet<int>();
            vagueConsonants = new HashSet<int>();
            leftVowels = new HashSet<int>();
            rightVowels = new HashSet<int>();
            saraAm = new HashSet<int>();
            topVowels = new HashSet<int>();
            bottomVowels = new HashSet<int>();
            toneMarks = new HashSet<int>();
            doubleConsonants = new Dictionary<int, HashSet<int>>();

            CreateTable();

            if ((consonants.Count
                + vagueConsonants.Count
                + leftVowels.Count
                + rightVowels.Count
                + saraAm.Count
                + topVowels.Count
                + bottomVowels.Count
                + toneMarks.Count) != thaiCharacters.Count)
            {
                UnityEngine.Debug.Log("error");
            }
        }

        /// <summary>
        /// レイアウトが必要なタイ語のテーブルを作成します。
        /// </summary>
        static void CreateTable()
        {
            foreach (int code in Enum.GetValues(typeof(ThaiLetter.Consonants)))
            {
                consonants.Add(code);
                thaiCharacters.Add(code);
            }

            foreach (int code in Enum.GetValues(typeof(ThaiLetter.VagueConsonants)))
            {
                vagueConsonants.Add(code);
                thaiCharacters.Add(code);
            }

            foreach (int code in Enum.GetValues(typeof(ThaiLetter.LeftVowels)))
            {
                leftVowels.Add(code);
                thaiCharacters.Add(code);
            }

            foreach (int code in Enum.GetValues(typeof(ThaiLetter.RightVowels)))
            {
                rightVowels.Add(code);
                thaiCharacters.Add(code);
            }

            foreach (int code in Enum.GetValues(typeof(ThaiLetter.SaraAm)))
            {
                saraAm.Add(code);
                thaiCharacters.Add(code);
            }

            foreach (int code in Enum.GetValues(typeof(ThaiLetter.TopVowels)))
            {
                topVowels.Add(code);
                thaiCharacters.Add(code);
            }

            foreach (int code in Enum.GetValues(typeof(ThaiLetter.BottomVowels)))
            {
                bottomVowels.Add(code);
                thaiCharacters.Add(code);
            }

            foreach (int code in Enum.GetValues(typeof(ThaiLetter.ToneMarks)))
            {
                toneMarks.Add(code);
                thaiCharacters.Add(code);
            }

            // 二重子音対応
            // 参考 : http://www.tlin.jp/thaimoji13.php
            HashSet<int> doubleConsonantPattern1 = new HashSet<int>() { (int) ThaiLetter.Consonants.RoRua, (int) ThaiLetter.Consonants.LoLing, (int) ThaiLetter.VagueConsonants.WoWaen };
            HashSet<int> doubleConsonantPattern2 = new HashSet<int>() { (int) ThaiLetter.Consonants.RoRua, (int) ThaiLetter.Consonants.LoLing };
            HashSet<int> doubleConsonantPattern3 = new HashSet<int>() { (int) ThaiLetter.Consonants.RoRua };
            doubleConsonants.Add((int) ThaiLetter.Consonants.KoKai, doubleConsonantPattern1);
            doubleConsonants.Add((int) ThaiLetter.Consonants.KhoKhai, doubleConsonantPattern1);
            doubleConsonants.Add((int) ThaiLetter.Consonants.KhoKhwai, doubleConsonantPattern1);
            doubleConsonants.Add((int) ThaiLetter.Consonants.PoPla, doubleConsonantPattern2);
            doubleConsonants.Add((int) ThaiLetter.Consonants.PhoPhung, doubleConsonantPattern2);
            doubleConsonants.Add((int) ThaiLetter.Consonants.PhoPhan, doubleConsonantPattern2);
            doubleConsonants.Add((int) ThaiLetter.Consonants.ToTao, doubleConsonantPattern3);
        }

#endregion
    }
}
