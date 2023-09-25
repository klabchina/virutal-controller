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
    public class AdvScriptDataEncoder
    {
#region properties

        /// <summary>Compressor</summary>
        public virtual IAdvScriptDataCompressor Compressor { get; set; }
        
        /// <summary>書き込み用ヘッダー</summary>
        protected AdvScriptDateWriteHeader header;

        /// <summary>エンコードされたバイト列</summary>
        protected byte[] bytes = null;

        /// <summary>圧縮するかどうか</summary>
        public bool IsCompression { get; set; }

        /// <summary>改竄防止を行うかどうか</summary>
        public bool IsBlockTamper { get; set; }

        /// <summary>改竄防止用に利用するキーワード(空でも可)</summary>
        public string SecretWord { get; set; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AdvScriptDataEncoder()
        {
            Compressor = new AdvUTF8Compressor();
            SecretWord = string.Empty;
        }

        /// <summary>
        /// ストリーム情報をエンコードします。
        /// </summary>
        /// <param name="stream">エンコードするストリーム</param>
        public virtual void Encode(List<string> stream)
        {
            header = new AdvScriptDateWriteHeader();
            
            int resourceCount = 0;
            if (AdvResourceUtil.IsHeader(stream[0]))
            {
                // リソースのヘッダー分で+1する
                resourceCount = AdvResourceUtil.GetResourcesTotalCount(stream[0]) + 1;
            }

            int dataLength = 0;
            int resourceLength = 0;

            if (IsCompression)
            {
                if (Compressor == null)
                {
#if UNITY_EDITOR || NOVELKIT_EDITOR || NOVELKIT_DEBUG
                    AdvLog.Error("Compressor not exist!");
#endif
                    bytes = new byte[0];
                    return;
                }

                if (resourceCount > 0)
                {
                    byte[] resource = Compressor.Compression(stream.GetRange(0, resourceCount), ref resourceLength);
                    byte[] data = Compressor.Compression(stream.GetRange(resourceCount, stream.Count - resourceCount), ref dataLength);

                    bytes = new byte[resource.Length + data.Length];
                    resource.CopyTo(bytes, 0);
                    data.CopyTo(bytes, resource.Length);

                    header.SetDataBeginIndex(AdvScriptDataHeader.HeaderLength + resource.Length);
                    header.SetResourceBeginIndex(AdvScriptDataHeader.HeaderLength);
                }
                else
                {
                    bytes = Compressor.Compression(stream, ref dataLength);
                }

                // ヘッダーに圧縮されていることを記録
                header.SetCompressed();
            }
            else
            {
                List<byte> data = new List<byte>();

                // リソース分のデータをバイト列に変換
                for (int i = 0; i < resourceCount; ++i)
                {
                    data.AddRange(Encoding.UTF8.GetBytes(stream[i]));
                    data.Add(AdvUTF8Compressor.LineFeedCode);
                }

                resourceLength = data.Count;

                // スクリプトデータをバイト列に変換
                int streamLength = stream.Count;
                for (int i = resourceCount; i < streamLength; ++i)
                {
                    data.AddRange(Encoding.UTF8.GetBytes(stream[i]));
                    data.Add(AdvUTF8Compressor.LineFeedCode);
                }

                dataLength = data.Count - resourceLength;

                bytes = data.ToArray();

                header.SetDataBeginIndex(AdvScriptDataHeader.HeaderLength + resourceLength);
                header.SetResourceBeginIndex(AdvScriptDataHeader.HeaderLength);
            }

            if (IsBlockTamper)
            {
                AdvAuthority.AuthData authData = AdvAuthority.Compute(bytes, SecretWord);
                header.SetAuthData(authData);
            }

            // ヘッダーにデータを記録
            header.SetDataLength(dataLength);
            header.SetResourceLength(resourceLength);
        }

        /// <summary>
        /// ヘッダー情報を取得します。
        /// </summary>
        /// <returns>エンコードしたデータに合わせたヘッダー情報を返します。</returns>
        public virtual byte[] GetHeader()
        {
            return header.Bytes;
        }

        /// <summary>
        /// エンコードされたデータを取得します。
        /// </summary>
        /// <returns>エンコードしたバイト列を返します。</returns>
        public virtual byte[] GetData()
        {
            return bytes;
        }

#endregion
    }
}
#endif
