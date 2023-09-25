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
using System.Security.Cryptography;

namespace Jigbox.NovelKit
{
    public static class AdvAuthority
    {
#region inner classes, enum, and structs

        public class AuthData
        {
            /// <summary>ハッシュの確認用バイト情報</summary>
            public byte CheckByte { get; protected set; }

            /// <summary>データ用のトークン</summary>
            public int Nonce { get; protected set; }

            /// <summary>キー用のトークン</summary>
            public int KeyNonce { get; protected set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="checkByte">ハッシュの確認用バイト情報</param>
            /// <param name="nonce">データ用のトークン</param>
            /// <param name="keyNonce">キー用のトークン</param>
            public AuthData(byte checkByte, int nonce, int keyNonce)
            {
                CheckByte = checkByte;
                Nonce = nonce;
                KeyNonce = keyNonce;
            }
        }

#endregion

#region constants

        /// <summary>1回辺りの探索回数</summary>
        static readonly int SearchTryCount = 100000;

        /// <summary>int型のバイト列の長さ</summary>
        static readonly int IntByteLength = 4;

        /// <summary>データからハッシュを取り出す際のキー</summary>
        static readonly byte[] DataHashKey = new byte[] { 0xf3, 0x21, 0x5a, 0x1c, 0x74, 0x39, 0xc0, 0xae };

        /// <summary>デフォルト状態での認証情報の生成難度</summary>
        public static readonly int DefaultDifficulty = 3;

        /// <summary>認証情報の生成難度の最小値</summary>
        public static readonly int DifficultyMin = 2;

        /// <summary>認証情報の生成難度の最大値</summary>
        public static readonly int DifficultyMax = 8;

#endregion

#region properties

        /// <summary>認証情報の生成難度</summary>
        static int difficulty = DefaultDifficulty;

        /// <summary>
        /// <para>認証情報の生成難度</para>
        /// <para>1以下の値は難易度として不適切です。難度は上げるほどに認証情報の生成に必要な時間が大幅に増加します。</para>
        /// </summary>
        public static int Difficulty
        {
            get
            {
                return difficulty;
            }
            set
            {
                difficulty = value <= DifficultyMin ? DifficultyMin : value <= DifficultyMax ? value : DifficultyMax;
            }
        }


#endregion

#region public methods

#if UNITY_EDITOR || NOVELKIT_EDITOR

        /// <summary>
        /// 渡されたバイト列からハッシュを生成し、認証情報を返します。
        /// </summary>
        /// <param name="data">データのバイト列</param>
        /// <param name="secretWord">ハッシュを生成する際のキーワード</param>
        /// <returns>認証情報</returns>
        public static AuthData Compute(byte[] data, string secretWord = "")
        {
            byte[] secret = Encoding.UTF8.GetBytes(secretWord);
            int secretLength = secret.Length;
            byte[] key = new byte[secretLength + IntByteLength];
            Array.Copy(secret, key, secretLength);

            HMACSHA256 dataSha = new HMACSHA256(DataHashKey);
            byte[] dataHash = dataSha.ComputeHash(data);
            int dataHashLength = dataHash.Length;
            byte[] buffer = new byte[dataHashLength + IntByteLength];
            Array.Copy(dataHash, buffer, dataHashLength);

            AuthData authData = null;
            byte[] hash = null;

            while (hash == null)
            {
                int nonce = UnityEngine.Random.Range(0, int.MaxValue - SearchTryCount);
                int keyNonce = UnityEngine.Random.Range(0, int.MaxValue);

                Array.Copy(BitConverter.GetBytes(keyNonce), 0, key, secretLength, IntByteLength);
                HMACSHA256 sha = new HMACSHA256(key);

                for (int i = 0; i < SearchTryCount; ++i)
                {
                    Array.Copy(BitConverter.GetBytes(nonce + i), 0, buffer, dataHashLength, IntByteLength);
                    hash = sha.ComputeHash(buffer);

                    for (int j = 1; j < Difficulty; ++j)
                    {
                        if (hash[j] != hash[0])
                        {
                            hash = null;
                            break;
                        }
                    }

                    if (hash != null)
                    {
                        authData = new AuthData(hash[0], nonce + i, keyNonce);
                        break;
                    }
                }
            }

            return authData;
        }

#endif

        /// <summary>
        /// 認証情報とデータからハッシュを生成し、データが正しいものかどうかを返します。
        /// </summary>
        /// <param name="data">データのバイト列</param>
        /// <param name="authData">認証情報</param>
        /// <param name="secretWord">ハッシュを生成する際のキーワード</param>
        /// <returns>データが正しければ<c>true</c>、誤っていれば<c>false</c>を返します。</returns>
        public static bool Authentication(byte[] data, AuthData authData, string secretWord = "")
        {
            byte[] secret = Encoding.UTF8.GetBytes(secretWord);
            int secretLength = secret.Length;
            byte[] key = new byte[secretLength + IntByteLength];
            Array.Copy(secret, key, secretLength);
            Array.Copy(BitConverter.GetBytes(authData.KeyNonce), 0, key, secretLength, IntByteLength);

            HMACSHA256 dataSha = new HMACSHA256(DataHashKey);
            byte[] dataHash = dataSha.ComputeHash(data);
            int dataHashLength = dataHash.Length;
            byte[] buffer = new byte[dataHashLength + IntByteLength];
            Array.Copy(dataHash, buffer, dataHashLength);
            Array.Copy(BitConverter.GetBytes(authData.Nonce), 0, buffer, dataHashLength, IntByteLength);

            HMACSHA256 sha = new HMACSHA256(key);
            byte[] hash = sha.ComputeHash(buffer);

            for (int i = 0; i < Difficulty; ++i)
            {
                if (hash[i] != authData.CheckByte)
                {
                    return false;
                }
            }

            return true;
        }

#endregion
    }
}
