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

namespace ThaiUtils
{
    /// <summary>
    /// キャレット参照単位でのテキスト情報
    /// </summary>
    public sealed class CaretUnit
    {
        /// <summary>
        /// テキスト情報
        /// </summary>
        public sealed class TextInfo
        {
            /// <summary>インデックス</summary>
            public readonly int index;

            /// <summary>文字列の長さ</summary>
            public readonly int length;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="index">インデックス</param>
            /// <param name="length">文字列の長さ</param>
            public TextInfo(int index, int length)
            {
                this.index = index;
                this.length = length;
            }
        }

        /// <summary>入力文字列の情報</summary>
        public readonly TextInfo source;

        /// <summary>変換後の文字列の情報</summary>
        public readonly TextInfo destination;

        /// <summary>右から左にレイアウトするかどうか(タイ語では使用しない)</summary>
        public readonly bool isRightToLeft = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="srcIndex">入力文字列のインデックス</param>
        /// <param name="srcLength">入力文字列内での文字列の長さ</param>
        /// <param name="dstIndex">変換後の文字列のインデックス</param>
        /// <param name="dstLength">変換後の文字列内での文字列の長さ</param>
        public CaretUnit(int srcIndex, int srcLength, int dstIndex, int dstLength)
        {
            source = new TextInfo(srcIndex, srcLength);
            destination = new TextInfo(dstIndex, dstLength);
        }
    }
}
