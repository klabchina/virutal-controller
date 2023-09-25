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

namespace Jigbox.TextView
{
    public static class LetterCaseUtils
    {
        /// <summary>小文字の始まりの文字コード</summary>
        public static readonly int LowerBegin = 0x0061;

        /// <summary>小文字の終わりの文字コード</summary>
        public static readonly int LowerEnd = 0x007a;

        /// <summary>大文字の始まりの文字コード</summary>
        public static readonly int UpperBegin = 0x0041;

        /// <summary>大文字の終わりの文字コード</summary>
        public static readonly int UpperEnd = 0x005a;

        /// <summary>小文字大文字の変換用</summary>
        public static readonly int LowerToUpper = 0x0020;
        
        /// <summary>囲み文字の小文字の始まりの文字コード</summary>
        public static readonly int EnclosureLowerBegin = 0x24d0;

        /// <summary>囲み文字の小文字の終わりの文字コード</summary>
        public static readonly int EnclosureLowerEnd = 0x24e9;

        /// <summary>囲み文字の大文字の始まりの文字コード</summary>
        public static readonly int EnclosureUpperBegin = 0x24b6;

        /// <summary>囲み文字の大文字の終わりの文字コード</summary>
        public static readonly int EnclosureUpperEnd = 0x24cf;
        
        /// <summary>囲み文字の小文字大文字の変換用</summary>
        public static readonly int EnclosureLowerToUpper = 0x001A;

        /// <summary>＿の文字コード</summary>
        public static readonly int UnderBarCode = 0x005f;

        /// <summary>:の文字コード</summary>
        public static readonly int ColonCode = 0x003a;

        /// <summary>
        /// SmallCaps用の乗算係数
        /// 0.8はTMPを参考に設定しています。
        /// </summary>
        public static readonly float SmallCapsMultiplier = 0.8f;

        /// <summary>
        /// 文字が大文字かを判定
        /// </summary>
        /// <param name="character">対象の文字</param>
        /// <returns>大文字が含まれていれば、<c>true</c>を返します。</returns>
        public static bool IsUpper(char character)
        {
            return (character >= UpperBegin && character <= UpperEnd)
                || (character >= EnclosureUpperBegin && character <= EnclosureUpperEnd);
        }

        /// <summary>
        /// 文字が小文字かを判定
        /// </summary>
        /// <param name="character">対象の文字</param>
        /// <returns>小文字が含まれていれば、<c>true</c>を返します。</returns>
        public static bool IsLower(char character)
        {
            return (character >= LowerBegin && character <= LowerEnd)
                || (character >= EnclosureLowerBegin && character <= EnclosureLowerEnd);
        }

        /// <summary>
        /// 文字を大文字に変換
        /// </summary>
        /// <param name="character">対象の文字</param>
        /// <returns>変換した文字</returns>
        public static char ToUpper(char character)
        {
            if (character >= LowerBegin && character <= LowerEnd)
            {
                return (char) (character - LowerToUpper);
            }
            
            if (character >= EnclosureLowerBegin && character <= EnclosureLowerEnd)
            {
                return (char) (character - EnclosureLowerToUpper);
            }

            return character;
        }

        /// <summary>
        /// 文字を小文字に変換
        /// </summary>
        /// <param name="character">対象の文字</param>
        /// <returns>変換した文字</returns>
        public static char ToLower(char character)
        {
            if (character >= UpperBegin && character <= UpperEnd)
            {
                return (char) (character + LowerToUpper);
            }
            
            if (character >= EnclosureUpperBegin && character <= EnclosureUpperEnd)
            {
                return (char) (character + EnclosureLowerToUpper);
            }

            return character;
        }
    }
}
