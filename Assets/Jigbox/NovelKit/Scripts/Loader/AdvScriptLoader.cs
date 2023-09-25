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
    public class AdvScriptLoader : IAdvScriptLoader
    {
#region properties

        /// <summary>Loader</summary>
        public IAdvResourceLoader Loader { get; set; }

        /// <summary>Decoder</summary>
        protected AdvScriptDataDecoder decoder;

        /// <summary>Decoder</summary>
        public virtual AdvScriptDataDecoder Decoder
        {
            get
            {
                if (decoder == null)
                {
                    decoder = new AdvScriptDataDecoder();
                }
                return decoder;
            }
        }

#endregion

#region public methods

        /// <summary>
        /// <para>シナリオスクリプトから行単位で分割したデータを読み込みます。</para>
        /// <para>バイナリデータの場合、スクリプトのみ、テキストデータの場合、リソース情報を含む全データを取得します。</para>
        /// </summary>
        /// <param name="path">読み込むシナリオスクリプトのパス</param>
        /// <returns>読み込みに成功すれば、行単位に分割された文字列を返します。</returns>
        public virtual string[] Load(string path)
        {
            if (Loader == null)
            {
                throw new System.NullReferenceException("Loader is not exist!");
            }
            TextAsset scriptData = Loader.Load<TextAsset>(path);
            if (scriptData == null)
            {
                return new string[0];
            }

            Decoder.DecodeResources(scriptData.bytes);

            Decoder.Decode(scriptData.bytes);
            if (Decoder.IsEnable)
            {
                return decoder.GetData();
            }

            return scriptData.text.Split(AdvScriptDataDecoder.LineFeedCode, System.StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// <para>シナリオスクリプトからリソース情報を読み込みます。</para>
        /// <para>バイナリデータの場合、リソース情報のみ、テキストデータの場合、スクリプトを含む全データを取得します。</para>
        /// </summary>
        /// <param name="path">読み込むシナリオスクリプトのパス</param>
        /// <returns>読み込みに成功すれば、行単位に分割された文字列を返します。</returns>
        public virtual string[] LoadResourceInfo(string path)
        {
            if (Loader == null)
            {
                throw new System.NullReferenceException("Loader is not exist!");
            }
            TextAsset scriptData = Loader.Load<TextAsset>(path);
            if (scriptData == null)
            {
                return new string[0];
            }

            Decoder.DecodeResources(scriptData.bytes);
            if (Decoder.IsEnable)
            {
                return decoder.GetData();
            }

            return scriptData.text.Split(AdvScriptDataDecoder.LineFeedCode, System.StringSplitOptions.RemoveEmptyEntries);
        }

#endregion
    }
}
