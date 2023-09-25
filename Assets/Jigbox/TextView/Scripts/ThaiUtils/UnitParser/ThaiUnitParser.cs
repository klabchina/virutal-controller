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
    /// 最小構成単位に分割するパーサー
    /// </summary>
    public static class ThaiUnitParser
    {
        /// <summary>文字の種類</summary>
        static List<CharacterType> characterTypes = new List<CharacterType>();

        /// <summary>文字の種類</summary>
        public static List<CharacterType> CharacterTypes { get { return characterTypes; } }

        /// <summary>最小構成単位の文字情報</summary>
        static List<ThaiUnit> units = new List<ThaiUnit>();

        /// <summary>最小構成単位の文字情報</summary>
        public static List<ThaiUnit> Units { get { return units; } }

        /// <summary>
        /// パースします。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="startIndex">開始インデックス</param>
        /// <param name="length">長さ</param>
        public static void Parse(string text, int startIndex, int length)
        {
            characterTypes.Clear();
            units.Clear();
            ThaiUnitFactory.Refresh();

            int endIndex = startIndex + length;
            for (int i = startIndex; i < endIndex; ++i)
            {
                characterTypes.Add(ThaiTable.GetType(text[i]));
            }

            int unitLength = 0;

            for (int i = 0; i < characterTypes.Count; ++i)
            {
                bool isLast = i + 1 == characterTypes.Count;
                if (ThaiUnitFactory.Push(text[startIndex + i],
                    characterTypes[i],
                    isLast ? '\0' : text[startIndex + i + 1],
                    isLast ? CharacterType.Other : characterTypes[i + 1]))
                {
                    continue;
                }

                ThaiUnit unit = ThaiUnitFactory.Get();
                units.Add(unit);
                unitLength += unit.length;

                // 1以上の場合は、今回入れようとした文字が判定上、構成として成立しなかったパターンなので、インデックスを戻して再処理する
                if (ThaiUnitFactory.Length >= 1)
                {
                    --i;
                }

                ThaiUnitFactory.Refresh();
            }

            int lastUnitLength = length - unitLength;
            if (lastUnitLength > 0)
            {
                units.Add(ThaiUnitFactory.Get());
                ThaiUnitFactory.Refresh();
            }

            return;
        }
    }
}
