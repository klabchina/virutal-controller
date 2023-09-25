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

using System.Text;

namespace Jigbox.NovelKit
{
    public sealed class AdvUTF8Decompressor : IAdvScriptDataDecompressor
    {
#region public methods

        /// <summary>
        /// 渡されたバイト列を解凍して文字列情報に変換して返します。
        /// </summary>
        /// <param name="src">圧縮されたバイト列</param>
        /// <param name="index">シーク開始位置</param>
        /// <param name="dataLength">展開後のバイト列の長さ</param>
        /// <returns>解凍された文字列情報を返します。</returns>
        public string Decompression(byte[] src, int index, int dataLength)
        {
            byte[] dst = new byte[dataLength];
            int dstIndex = 0;

            int length = src.Length;

            for (int i = index; i < length;)
            {
                // 圧縮されているデータを展開する
                if (src[i] == AdvUTF8CompressionUtils.CompressionCode)
                {
                    // 長さを表す部分は256までの整数となるが1Byteの表現範囲は0～255なので
                    // -1して格納してあるので+1して取り出す

                    // 2Byte目が辞書の参照位置
                    int offset = src[i + 1] + 1;
                    // 3Byte目が辞書から取り出すバイト列の長さ
                    int byteLength = src[i + 2] + 1;

                    // 解凍時はすでに展開済みのデータが辞書の代わりになるので、
                    // 展開済みのデータのインデックスから参照位置を補正
                    offset = dstIndex - offset;

                    for (int j = 0; j < byteLength; ++j)
                    {
                        dst[dstIndex] = dst[offset + j];
                        ++dstIndex;
                    }

                    // 符号分だけ進める
                    i += AdvUTF8CompressionUtils.CompressionNeedByte;
                }
                // 圧縮されていないのでそのまま展開
                else
                {
                    // 文字単位で展開する
                    int byteLength = AdvUTF8CompressionUtils.GetCharacterByteLength(src[i]);
                    for (int j = 0; j < byteLength; ++j)
                    {
                        dst[dstIndex] = src[i + j];
                        ++dstIndex;
                    }

                    i += byteLength;
                }
            }

            // 正しく展開できていない場合、バイト列の長さと参照位置がズレる
            if (dstIndex != dataLength)
            {
                throw new System.NotSupportedException("Trying to decompression for data in unsupported format!");
            }

            return Encoding.UTF8.GetString(dst);
        }

#endregion
    }
}
