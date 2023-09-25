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

using UnityEngine;

namespace Jigbox.NovelKit
{
    public class AdvUTF8CompressionUtils : MonoBehaviour
    {
#region constants
        
        /// <summary>符号化して圧縮するために必要なバイト数</summary>
        public static readonly int CompressionNeedByte = 3;

        /// <summary>圧縮した箇所を識別するための符号データ</summary>
        public static readonly byte CompressionCode = 0x80;

        /// <summary>UTF-8で1Byteで表現される文字の先頭Byteの最小値</summary>
        public static readonly byte Byte1Min = 0x00;

        /// <summary>UTF-8で1Byteで表現される文字の先頭Byteの最大値</summary>
        public static readonly byte Byte1Max = 0x7f;

        /// <summary>UTF-8で2Byteで表現される文字の先頭Byteの最小値</summary>
        public static readonly byte Byte2Min = 0xc2;

        /// <summary>UTF-8で2Byteで表現される文字の先頭Byteの最大値</summary>
        public static readonly byte Byte2Max = 0xdf;

        /// <summary>UTF-8で3Byteで表現される文字の先頭Byteの最小値</summary>
        public static readonly byte Byte3Min = 0xe0;

        /// <summary>UTF-8で3Byteで表現される文字の先頭Byteの最大値</summary>
        public static readonly byte Byte3Max = 0xef;

        /// <summary>UTF-8で4Byteで表現される文字の先頭Byteの最小値</summary>
        public static readonly byte Byte4Min = 0xf0;

        /// <summary>UTF-8で4Byteで表現される文字の先頭Byteの最大値</summary>
        public static readonly byte Byte4Max = 0xf7;

#endregion

#region public methods

        /// <summary>
        /// UTF-8で記録された文字のバイト数を取得します。
        /// </summary>
        /// <param name="firstByte">UTF8で記録された文字の先頭Byte</param>
        /// <returns>文字を表すバイト数を返します。</returns>
        public static int GetCharacterByteLength(byte firstByte)
        {
            // 1Byte文字
            if (firstByte <= Byte1Max)
            {
                return 1;
            }
            // 2Byte文字
            // 1Byte文字と2Byte文字の判別先頭は繋がった数値ではないため、
            // 2Byte文字の判定のみ、前後の範囲を判定する
            if (firstByte >= Byte2Min && firstByte <= Byte2Max)
            {
                return 2;
            }
            // 3Byte文字
            if (firstByte <= Byte3Max)
            {
                return 3;
            }
            // 4Byte文字
            if (firstByte <= Byte4Max)
            {
                return 4;
            }

            return 1;
        }

#endregion
    }
}
