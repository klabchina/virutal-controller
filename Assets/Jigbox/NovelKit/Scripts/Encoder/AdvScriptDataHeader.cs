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

using System;

namespace Jigbox.NovelKit
{
    public class AdvScriptDataHeader
    {
#region constants

        /// <summary>ヘッダーを構成するバイト列の長さ</summary>
        public static readonly int HeaderLength = 27;

        /**
         * ヘッダーを構成するバイト列の構成
         * 1Byte : ヘッダー識別用(0x80)
         * 1Byte : データ情報(0xXX)
         * 4Byte : データ領域の開始バイト数
         * 4Byte : リソース領域の開始バイト数
         * 4Byte : データ領域の展開サイズ
         * 4Byte : リソース領域の展開サイズ
         * 1Byte : 改竄防止確認のための確認用バイト
         * 4Byte : 改竄防止用のデータ用トークン
         * 4Byte : 改竄防止用のキー用トークン
         */

        /// <summary>ヘッダー識別用データのバイト数</summary>
        protected static readonly int HeaderIdentifierByte = 0;

        /// <summary>データ情報のバイト数</summary>
        protected static readonly int DataInfomationByte = 1;

        /// <summary>データ領域の開始バイト数</summary>
        protected static readonly int DataBeginIndexBeginByte = 2;

        /// <summary>リソース領域の開始バイト数</summary>
        protected static readonly int ResourceBeginIndexBeginByte = 6;

        /// <summary>データ領域の展開サイズの開始バイト数</summary>
        protected static readonly int DataLengthBeginByte = 10;

        /// <summary>リソース領域の展開サイズの開始バイト数</summary>
        protected static readonly int ResourceLengthBeginByte = 14;

        /// <summary>改竄防止の確認用バイトの開始バイト数</summary>
        protected static readonly int CheckByteBeginByte = 18;

        /// <summary>改竄防止用のデータ用トークンの開始バイト数</summary>
        protected static readonly int NonceBeginByte = 19;

        /// <summary>改竄防止用のキー用トークンの開始バイト数</summary>
        protected static readonly int KeyNonceBeginByte = 23;

        /// <summary>ヘッダー識別用データの値</summary>
        protected static readonly byte HeaderIdentifier = 0x80;

        /// <summary>圧縮されているかどうかのマスク</summary>
        protected static readonly byte CompressedMask = 0x80;

        /// <summary>リソース情報を含むかどうかのマスク</summary>
        protected static readonly byte HasResourceMask = 0x40;

#endregion

#region properties

        /// <summary>ヘッダーが有効かどうか</summary>
        public bool IsEnable { get { return bytes[HeaderIdentifierByte] == HeaderIdentifier; } }

        /// <summary>ヘッダーのバイト列</summary>
        protected byte[] bytes = new byte[HeaderLength];

        /// <summary>ヘッダーのバイト列</summary>
        public byte[] Bytes
        {
            get
            {
                byte[] temp = new byte[bytes.Length];
                Array.Copy(bytes, temp, bytes.Length);
                return temp;
            }
        }

        /// <summary>データが圧縮されているかどうか</summary>
        public bool IsCompressed { get { return (bytes[DataInfomationByte] & CompressedMask) > 0; } }

        /// <summary>リソース情報が記録されているかどうか</summary>
        public bool HasResource { get { return (bytes[DataInfomationByte] & HasResourceMask) > 0; } }

        /// <summary>リソース領域の開始バイト数</summary>
        public int ResourceBeginIndex { get { return BitConverter.ToInt32(bytes, ResourceBeginIndexBeginByte); } }

        /// <summary>データ領域の開始バイト数</summary>
        public int DataBeginIndex { get { return BitConverter.ToInt32(bytes, DataBeginIndexBeginByte); } }

        /// <summary>データ領域の展開サイズ</summary>
        public int DataLength { get { return BitConverter.ToInt32(bytes, DataLengthBeginByte); } }

        /// <summary>リソース領域の展開サイズ</summary>
        public int ResourceLength { get { return BitConverter.ToInt32(bytes, ResourceLengthBeginByte); } }

        /// <summary>改竄防止の確認用バイト</summary>
        public byte CheckByte { get { return bytes[CheckByteBeginByte]; } }

        /// <summary>改竄防止用のデータ用トークン</summary>
        public int Nonce { get { return BitConverter.ToInt32(bytes, NonceBeginByte); } }

        /// <summary>改竄防止用のキー用トークン</summary>
        public int KeyNonce { get { return BitConverter.ToInt32(bytes, KeyNonceBeginByte); } }

#endregion
    }
}
