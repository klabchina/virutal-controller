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
using System.Text;

namespace Jigbox.NovelKit
{
    public class AdvScriptDataDecoder
    {
#region constants

        /// <summary>改行コード</summary>
        public static readonly char[] LineFeedCode = new char[] { '\n' };

#endregion

#region properties

        /// <summary>Decompressor</summary>
        public virtual IAdvScriptDataDecompressor Decompressor { get; set; }

        /// <summary>読み込み用ヘッダー</summary>
        protected AdvScriptDataReadHeader header;

        /// <summary>データが有効かどうか</summary>
        public virtual bool IsEnable { get { return header.IsEnable; } }

        /// <summary>改竄防止を行うかどうか</summary>
        public bool IsBlockTamper { get; set; }

        /// <summary>改竄防止用に利用するキーワード(空でも可)</summary>
        public string SecretWord { get; set; }

        /// <summary>改竄が検知された際のコールバック</summary>
        public Action OnTampered { get; set; }

        /// <summary>デコードされたバイト列</summary>
        protected string[] data = null;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AdvScriptDataDecoder()
        {
            Decompressor = new AdvUTF8Decompressor();
            SecretWord = string.Empty;
        }

        /// <summary>
        /// ストリーム情報をデコードします。
        /// </summary>
        /// <param name="stream">ストリーム情報</param>
        public virtual void Decode(byte[] stream)
        {
            header = new AdvScriptDataReadHeader(stream);
            if (!header.IsEnable)
            {
                return;
            }

            if (IsBlockTamper)
            {
                if (!Authentication(stream))
                {
                    data = new string[0];
                    return;
                }
            }

            if (header.IsCompressed)
            {
                if (Decompressor == null)
                {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                    AdvLog.Error("Decompressor not exist!");
#endif
                    data = new string[0];
                    return;
                }

                data = Decompressor.Decompression(stream, header.DataBeginIndex, header.DataLength).Split(LineFeedCode, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                // 実データに関する部分を抽出
                int dataLength = header.DataLength;
                byte[] bytes = new byte[dataLength];
                Array.Copy(stream, header.DataBeginIndex, bytes, 0, dataLength);

                data = Encoding.UTF8.GetString(bytes).Split(LineFeedCode, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// ストリーム情報からリソースに関する情報をデコードします。
        /// </summary>
        /// <param name="stream">ストリーム情報</param>
        public virtual void DecodeResources(byte[] stream)
        {
            header = new AdvScriptDataReadHeader(stream);
            if (!header.IsEnable || !header.HasResource)
            {
                return;
            }

            if (IsBlockTamper)
            {
                if (!Authentication(stream))
                {
                    data = new string[0];
                    return;
                }
            }

            if (header.IsCompressed)
            {
                if (Decompressor == null)
                {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                    AdvLog.Error("Decompressor not exist!");
#endif
                    data = new string[0];
                    return;
                }

                // リソースに関する部分のバイト列を抽出
                int resourceBufferLength = header.DataBeginIndex - header.ResourceBeginIndex;
                byte[] resourceStream = new byte[resourceBufferLength];
                Array.Copy(stream, header.ResourceBeginIndex, resourceStream, 0, resourceBufferLength);

                data = Decompressor.Decompression(resourceStream, 0, header.ResourceLength).Split(LineFeedCode, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                // リソースに関する部分のバイト列を抽出
                int resourceLength = header.ResourceLength;
                byte[] bytes = new byte[resourceLength];
                Array.Copy(stream, header.ResourceBeginIndex, bytes, 0, resourceLength);

                data = Encoding.UTF8.GetString(bytes).Split(LineFeedCode, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// デコードされたデータを取得します。
        /// </summary>
        /// <returns>デコードした文字列情報を返します。</returns>
        public virtual string[] GetData()
        {
            return data;
        }

#endregion

#region protected methods

        /// <summary>
        /// データの認証を行い、改竄が行われていないかどうかを返します。
        /// </summary>
        /// <param name="stream">ストリーム情報</param>
        /// <returns>データが正しければ<c>true</c>、誤っていれば<c>false</c>を返します。</returns>
        protected virtual bool Authentication(byte[] stream)
        {
            int dataBeginIndex = header.HasResource ? header.ResourceBeginIndex : header.DataBeginIndex;
            byte[] data = new byte[stream.Length - dataBeginIndex];
            Array.Copy(stream, dataBeginIndex, data, 0, data.Length);

            AdvAuthority.AuthData authData = new AdvAuthority.AuthData(header.CheckByte, header.Nonce, header.KeyNonce);

            bool result = AdvAuthority.Authentication(data, authData, SecretWord);
            if (!result && OnTampered != null)
            {
                OnTampered();
            }

            return result;
        }

#endregion
    }
}
