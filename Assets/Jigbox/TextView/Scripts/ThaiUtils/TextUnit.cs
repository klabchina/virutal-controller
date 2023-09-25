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
    /// テキストの構成
    /// </summary>
    public sealed class TextUnit
    {
        /// <summary>文字列</summary>
        public readonly char[] data = null;

        /// <summary>タイ語としてレイアウトする文字列かどうか</summary>
        public readonly bool isLayouted = false;

        /// <summary>オフセット情報</summary>
        public readonly GlyphLayoutOffset[] offsets = null;

        /// <summary>グループ化する最初の要素かどうか</summary>
        public bool isStartSpan = true;

        /// <summary>グループ化する最後の要素かどうか</summary>
        public bool isEndSpan = true;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data">文字列</param>
        public TextUnit(char[] data)
        {
            this.data = data;
            isLayouted = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data">文字列</param>
        /// <param name="offsets">オフセット情報</param>
        public TextUnit(char[] data, GlyphLayoutOffset[] offsets)
        {
            this.data = data;
            this.offsets = offsets;
            isLayouted = true;
        }
    }
}
