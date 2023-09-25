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

#if UNITY_EDITOR || NOVELKIT_EDITOR
using System.Collections.Generic;
using System.Text;

namespace Jigbox.NovelKit
{
    public sealed class AdvUTF8Compressor : IAdvScriptDataCompressor
    {
#region constants

        /// <summary>辞書の最大サイズ</summary>
        public static readonly int DictLengthMax = 256;

        /// <summary>改行コード</summary>
        public static readonly byte LineFeedCode = (byte) '\n';

#endregion

#region properties

        /// <summary>元となるバイト列</summary>
        List<byte> src = new List<byte>();

        /// <summary>圧縮後のバイト列</summary>
        List<byte> dst = new List<byte>();

        /// <summary>圧縮のための辞書データ</summary>
        List<byte> dict = new List<byte>();

#endregion

#region public methods

        /// <summary>
        /// 渡された文字列情報を圧縮したバイト列に変換して返します。
        /// </summary>
        /// <param name="stream">文字列情報</param>
        /// <param name="srcLength">元のバイト列の長さ</param>
        /// <returns>圧縮されたバイト列を返します。</returns>
        public byte[] Compression(List<string> stream, ref int srcLength)
        {
            ClearBuffer();
            GetBytesFromStream(stream);

            int dictLength = 0;
            int matchLength = 0;
            int matchedIndex = 0;
            srcLength = src.Count;

            for (int i = 0; i < srcLength;)
            {
                dictLength = dict.Count;
                matchLength = 0;
                matchedIndex = 0;
                
                // 辞書内に適合するバイト列が存在するか確認する
                for (int j = 0; j < dictLength;)
                {
                    if (dict[j] == src[i])
                    {
                        matchLength = GetMatchLength(i, j);
                        // 符号化に必要なバイト数が3Byteなので、
                        // 重複が3Byte以下では圧縮にならないため、圧縮を行わない
                        if (matchLength > AdvUTF8CompressionUtils.CompressionNeedByte)
                        {
                            matchedIndex = j;
                            break;
                        }
                    }

                    // 文字の先頭がマッチしていない = 文字がマッチしていない
                    // ということなので1文字分のバイト数を飛ばす
                    j += AdvUTF8CompressionUtils.GetCharacterByteLength(dict[j]);
                }

                // 適合するバイト列が存在した場合
                if (matchLength > AdvUTF8CompressionUtils.CompressionNeedByte)
                {
                    // 符号を挿入
                    dst.Add(AdvUTF8CompressionUtils.CompressionCode);

                    // 長さを表す部分は256までの整数となるが1Byteの表現範囲は0～255なので-1して格納する

                    // 辞書の参照位置を挿入
                    // 解凍時に処理しやすいように、復号位置から逆算した辞書の位置にする
                    // 符号化に必要なバイト数が3なので、matchedIndexはdictLenght-3以下にはならない
                    dst.Add((byte) (dictLength - matchedIndex - 1));
                    dst.Add((byte) (matchLength - 1));

                    for (int j = 0; j < matchLength; ++j)
                    {
                        dict.Add(src[i + j]);
                    }

                    i += matchLength;
                }
                // 適合するバイト列が存在しなかった場合、
                // 1文字分をそのまま出力用データに追加する
                else
                {
                    int byteLength = AdvUTF8CompressionUtils.GetCharacterByteLength(src[i]);

                    for (int j = 0; j < byteLength; ++j)
                    {
                        dst.Add(src[i + j]);
                        dict.Add(src[i + j]);
                    }

                    i += byteLength;
                }

                // 辞書サイズが溢れた場合、古くなった先頭のデータから
                // 溢れた分のデータを削除する
                if (dict.Count > DictLengthMax)
                {
                    dict.RemoveRange(0, dict.Count - DictLengthMax);
                }
            }

            return dst.ToArray();
        }

#endregion

#region private methods

        /// <summary>
        /// 元のバイト列と辞書を比較して、適合するバイト列の長さを返します。
        /// </summary>
        /// <param name="srcIndex">元のバイト列の参照位置</param>
        /// <param name="dictIndex">辞書の参照位置</param>
        /// <returns>適合したバイト列の長さを返します。</returns>
        int GetMatchLength(int srcIndex, int dictIndex)
        {
            int length = 0;
            int dictLength = dict.Count - dictIndex;
            int srcLength = src.Count;

            // 元のバイト列の終端になるまで確認する
            // (実際はほとんど途中で辞書データからあぶれるか適合しなくなって中断される)
            while (srcIndex + length < srcLength)
            {
                int byteLength = AdvUTF8CompressionUtils.GetCharacterByteLength(src[srcIndex + length]);
                
                // 判定後のバイト列が辞書からあぶれる、または、適合しなかったら判定終了
                if (length + byteLength > dictLength
                    || !CompereBytes(srcIndex + length, dictIndex + length, byteLength))
                {
                    break;
                }

                length += byteLength;
            }

            // 辞書の終端が全て適合している場合は、同一パターンが連続して
            // 出現しているかどうかを確認する
            if (length == dictLength)
            {
                // 元のバイト列の終端になるか、
                // パターンが辞書の最大サイズを超えるまで確認
                // 辞書の最大サイズ以上の長さのデータは一度に圧縮できない
                while (srcIndex + length < srcLength && length + dictLength <= DictLengthMax)
                {
                    if (!CompereBytes(srcIndex + length, dictIndex, dictLength))
                    {
                        break;
                    }

                    length += dictLength;
                }
            }

            return length;
        }

        /// <summary>
        /// バイト列を比較して、一致しているかどうかを返します。
        /// </summary>
        /// <param name="srcIndex">元のバイト列の参照位置</param>
        /// <param name="dictIndex">辞書の参照位置</param>
        /// <param name="length">比較するバイト列の長さ</param>
        /// <returns>比較して一致していれば<c>true</c>、一致していなければ<c>false</c>を返します。</returns>
        bool CompereBytes(int srcIndex, int dictIndex, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                if (src[srcIndex + i] != dict[dictIndex + i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 処理のために保持しているバッファをクリアします。
        /// </summary>
        void ClearBuffer()
        {
            src.Clear();
            dst.Clear();
            dict.Clear();
        }

        /// <summary>
        /// 文字列情報からUTF-8状態のバイト列を取得します。
        /// </summary>
        /// <param name="stream">文字列情報</param>
        void GetBytesFromStream(List<string> stream)
        {
            foreach (string str in stream)
            {
                src.AddRange(Encoding.UTF8.GetBytes(str));
                src.Add(LineFeedCode);
            }
        }

#endregion
    }
}
#endif