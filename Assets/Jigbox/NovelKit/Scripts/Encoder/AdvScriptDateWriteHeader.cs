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
using System;

namespace Jigbox.NovelKit
{
    public class AdvScriptDateWriteHeader : AdvScriptDataHeader
    {
#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AdvScriptDateWriteHeader()
        {
            // 識別用データを設定
            bytes[HeaderIdentifierByte] = HeaderIdentifier;

            // デフォルトのデータ領域の開始バイト数を設定
            SetInt(HeaderLength, DataBeginIndexBeginByte);
        }

        /// <summary>
        /// 圧縮されていることを設定します。
        /// </summary>
        public void SetCompressed()
        {
            // byte型のビット演算がint扱いになるのでキャストし直して突っ込む
            bytes[DataInfomationByte] = (byte) (bytes[DataInfomationByte] | CompressedMask);
        }

        /// <summary>
        /// データ領域の開始バイト数を設定します。
        /// </summary>
        /// <param name="dataBeginIndex">データ領域の開始バイト</param>
        public void SetDataBeginIndex(int dataBeginIndex)
        {
            SetInt(dataBeginIndex, DataBeginIndexBeginByte);
        }

        /// <summary>
        /// リソース領域の開始バイト数を設定します。
        /// </summary>
        /// <param name="resourceBeginIndex">リソース領域の開始バイト</param>
        public void SetResourceBeginIndex(int resourceBeginIndex)
        {
            SetInt(resourceBeginIndex, ResourceBeginIndexBeginByte);
        }

        /// <summary>
        /// データ領域の展開サイズを設定します。
        /// </summary>
        /// <param name="dataLength">データ領域の展開サイズ</param>
        public void SetDataLength(int dataLength)
        {
            SetInt(dataLength, DataLengthBeginByte);
        }

        /// <summary>
        /// リソース領域の展開サイズを設定します。
        /// </summary>
        /// <param name="resourceLength">リソース領域の展開サイズ</param>
        public void SetResourceLength(int resourceLength)
        {
            if (resourceLength <= 0)
            {
                return;
            }

            SetInt(resourceLength, ResourceLengthBeginByte);

            // byte型のビット演算がint扱いになるのでキャストし直して突っ込む
            bytes[DataInfomationByte] = (byte) (bytes[DataInfomationByte] | HasResourceMask);
        }

        /// <summary>
        /// 改竄防止確認のための認証用データを設定します。
        /// </summary>
        /// <param name="data">認証用データ</param>
        public void SetAuthData(AdvAuthority.AuthData data)
        {
            bytes[CheckByteBeginByte] = data.CheckByte;
            SetInt(data.Nonce, NonceBeginByte);
            SetInt(data.KeyNonce, KeyNonceBeginByte);
        }

#endregion

#region protected methods

        protected void SetInt(int value, int byteBeginIndex)
        {
            byte[] length = BitConverter.GetBytes(value);

            int byteLength = length.Length;
            for (int i = 0; i < byteLength; ++i)
            {
                bytes[byteBeginIndex + i] = length[i];
            }
        }

#endregion
    }
}
#endif
