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
using System.IO;
using System.Text;

namespace Jigbox.NovelKit
{
    public class AdvScriptStreamingAssetsLoader : AdvScriptLoader
    {
#region public methods

        /// <summary>
        /// <para>シナリオスクリプトから行単位で分割したデータを読み込みます。</para>
        /// <para>バイナリデータの場合、スクリプトのみ、テキストデータの場合、リソース情報を含む全データを取得します。</para>
        /// </summary>
        /// <param name="path">読み込むシナリオスクリプトのパス</param>
        /// <returns>読み込みに成功すれば、行単位に分割された文字列を返します。</returns>
        public override string[] Load(string path)
        {
            byte[] scriptData = LoadFromStreamingAssets(path);
            if (scriptData == null)
            {
                return new string[0];
            }

            Decoder.DecodeResources(scriptData);

            Decoder.Decode(scriptData);
            if (Decoder.IsEnable)
            {
                return decoder.GetData();
            }

            return ConvertString(scriptData);
        }

        /// <summary>
        /// <para>シナリオスクリプトからリソース情報を読み込みます。</para>
        /// <para>バイナリデータの場合、リソース情報のみ、テキストデータの場合、スクリプトを含む全データを取得します。</para>
        /// </summary>
        /// <param name="path">読み込むシナリオスクリプトのパス</param>
        /// <returns>読み込みに成功すれば、行単位に分割された文字列を返します。</returns>
        public override string[] LoadResourceInfo(string path)
        {
            byte[] scriptData = LoadFromStreamingAssets(path);
            if (scriptData == null)
            {
                return new string[0];
            }

            Decoder.DecodeResources(scriptData);
            if (Decoder.IsEnable)
            {
                return decoder.GetData();
            }

            return ConvertString(scriptData);
        }

#endregion

#region protected methods

        /// <summary>
        /// StreamingAssets以下のシナリオスクリプトを読み込んでバイト列を返します。
        /// </summary>
        /// <param name="path">読み込むシナリオスクリプトのパス</param>
        /// <returns></returns>
        protected virtual byte[] LoadFromStreamingAssets(string path)
        {
            string fullPath = Application.streamingAssetsPath + "/" + path + ".bytes";
            if (File.Exists(fullPath))
            {
                return File.ReadAllBytes(fullPath);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("Not exist scenario script: " + fullPath);
#endif
                return null;
            }
        }

        /// <summary>
        /// バイト列から行単位の文字列データに変換します。
        /// </summary>
        /// <param name="bytes">文字列情報のバイト列</param>
        /// <returns></returns>
        protected virtual string[] ConvertString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes).Split(AdvScriptDataDecoder.LineFeedCode, System.StringSplitOptions.RemoveEmptyEntries);
        }

#endregion
    }
}
