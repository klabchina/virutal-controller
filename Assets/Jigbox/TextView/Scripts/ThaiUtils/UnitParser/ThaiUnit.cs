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

namespace ThaiUtils.UnitParser
{
    /// <summary>
    /// タイ語の最小構成
    /// </summary>
    public class ThaiUnit
    {
        /// <summary>長さ</summary>
        public readonly int length = 1;

        /// <summary>子音のみかどうか</summary>
        public readonly bool onlyConsonant = false;

        /// <summary>上にレイアウトする文字を2文字含むかどうか</summary>
        public readonly bool hasTopDouble = false;

        /// <summary>下にレイアウトする文字を含むかどうか</summary>
        public readonly bool hasBottom = false;

        /// <summary>SaraAmを含むかどうか</summary>
        public readonly bool hasSaraAm = false;

        /// <summary>二重子音かどうか</summary>
        public readonly bool isDoubleConsonant = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ThaiUnit()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="length">長さ</param>
        /// <param name="onlyConsonant">子音のみかどうか</param>
        /// <param name="hasTopDouble">上にレイアウトする文字を2文字含むかどうか</param>
        /// <param name="hasBottom">下にレイアウトする文字を含むかどうか</param>
        /// <param name="hasSaraAm">SaraAmを含むかどうか</param>
        /// <param name="isDoubleConsonant">二重子音かどうか</param>
        public ThaiUnit(int length,
            bool onlyConsonant,
            bool hasTopDouble,
            bool hasBottom,
            bool hasSaraAm,
            bool isDoubleConsonant)
        {
            this.length = length;
            this.onlyConsonant = onlyConsonant;
            this.hasTopDouble = hasTopDouble;
            this.hasBottom = hasBottom;
            this.hasSaraAm = hasSaraAm;
            this.isDoubleConsonant = isDoubleConsonant;
        }
    }
}
