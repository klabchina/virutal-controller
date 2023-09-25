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
using System;
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public static class AdvResourceUtil
    {
#region inner classes, enum, and structs

        public enum ResourceType
        {
            /// <summary>グラフィック関連</summary>
            Graphic,
            /// <summary>サウンド</summary>
            Sound,
            /// <summary>シナリオスクリプト</summary>
            Script,
            /// <summary>マクロ、定数</summary>
            Definition,
        }

#endregion

#region constants
        
        /// <summary>リソース数の区切り文字</summary>
        static readonly char ResourceCountDelimiter = '=';
        
        /// <summary>グラフィック関連のリソースのラベル</summary>
        static readonly string GraphicResourceLabel = "resource=";

        /// <summary>サウンドのリソースのラベル</summary>
        static readonly string SoundResourceLabel = "sound=";

        /// <summary>シナリオスクリプトのリソースのラベル</summary>
        static readonly string ScriptResourceLabel = "script=";

        /// <summary>マクロ、定数定義ファイルのリソースのラベル</summary>
        static readonly string DefinitionResourceLabel = "definition=";

#endregion

#region public methods

        /// <summary>
        /// スクリプトで使用するリソース情報を取得します。
        /// </summary>
        /// <param name="stream">スクリプトのストリーム情報</param>
        /// <returns></returns>
        public static Dictionary<ResourceType, List<string>> GetResources(string stream)
        {
            if (string.IsNullOrEmpty(stream))
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("AdvResourceUtil.GetResources : That string is empty!");
#endif
                return null;
            }

            return GetResources(stream.Split(AdvScriptDataDecoder.LineFeedCode, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// スクリプトで使用するリソース情報を取得します。
        /// </summary>
        /// <param name="textAsset">テキストアセット</param>
        /// <returns></returns>
        public static Dictionary<ResourceType, List<string>> GetResources(TextAsset textAsset)
        {
            if (textAsset == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("AdvResourceUtil.GetResources : That textasset is null!");
#endif
                return null;
            }
            if (string.IsNullOrEmpty(textAsset.text))
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("AdvResourceUtil.GetResources : That textasset is empty!");
#endif
                return null;
            }

            return GetResources(textAsset.text.Split(AdvScriptDataDecoder.LineFeedCode, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// スクリプト内に含まれるリソースの総数を取得します。
        /// </summary>
        /// <param name="header">リソース用ヘッダー文字列</param>
        /// <returns></returns>
        public static int GetResourcesTotalCount(string header)
        {
            string[] resourceInfo = header.Split(AdvScriptParams.Delimiter, System.StringSplitOptions.RemoveEmptyEntries);

            int count = 0;
            foreach (string info in resourceInfo)
            {
                string[] data = info.Split(ResourceCountDelimiter);
                int resourceCount;
                if (int.TryParse(data[1], out resourceCount))
                {
                    count += resourceCount;
                }
            }

            return count;
        }

        /// <summary>
        /// 文字列がリソース用ヘッダーかどうかを返します。
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>渡された文字列がヘッダーであれば<c>true</c>、そうでなければ<c>false</c>を返します。</returns>
        public static bool IsHeader(string str)
        {
            // 先頭にグラフィック関連リソースのラベルがあれば、リソース用ヘッダーとみなす
            return str.IndexOf(GraphicResourceLabel) == 0;
        }

        /// <summary>
        /// リソース用ヘッダー情報を取得します。
        /// </summary>
        /// <param name="graphic">グラフィック関連のリソースの数</param>
        /// <param name="sound">サウンドのリソースの数</param>
        /// <param name="script">シナリオスクリプトのリソースの数</param>
        /// <param name="definition">マクロ、定数定義ファイルのリソースの数</param>
        /// <returns></returns>
        public static string GetResourceHeader(int graphic, int sound, int script, int definition)
        {
            string[] resources = new string[]
                {
                    GraphicResourceLabel + graphic,
                    SoundResourceLabel + sound,
                    ScriptResourceLabel + script,
                    DefinitionResourceLabel + definition
                };
            return string.Join(AdvScriptParams.DelimiterString, resources);
        }

#endregion

#region private methods

        /// <summary>
        /// スクリプトで使用するリソース情報を取得します。
        /// </summary>
        /// <param name="scriptStream">スクリプトのストリーム情報</param>
        /// <returns></returns>
        static Dictionary<ResourceType, List<string>> GetResources(string[] scriptStream)
        {
            Dictionary<ResourceType, List<string>> resources = CreateEmptyDictionary();

            string header = scriptStream[0];
            string[] resourceInfo = header.Split(AdvScriptParams.Delimiter, StringSplitOptions.RemoveEmptyEntries);

            int index = 1;
            int count = 0;

            foreach (List<string> list in resources.Values)
            {
                // 退行処理
                // リソースの種類の定義数が実行時点より少ない状態でビルドされたものに対応するための対策
                if (resourceInfo.Length <= count)
                {
                    break;
                }

                string[] data = resourceInfo[count].Split(ResourceCountDelimiter);

                int resourceCount;
                if (int.TryParse(data[1], out resourceCount))
                {
                    if (resourceCount == 0)
                    {
                        ++count;
                        continue;
                    }

                    for (int i = 0; i < resourceCount; ++i)
                    {
                        list.Add(scriptStream[index]);
                        ++index;
                    }
                }

                ++count;
            }

            return resources;
        }

        /// <summary>
        /// リソースの一覧を格納する空の辞書を作成します。
        /// </summary>
        /// <returns></returns>
        static Dictionary<ResourceType, List<string>> CreateEmptyDictionary()
        {
            Dictionary<ResourceType, List<string>> dictionary = new Dictionary<ResourceType, List<string>>();
            dictionary.Add(ResourceType.Graphic, new List<string>());
            dictionary.Add(ResourceType.Sound, new List<string>());
            dictionary.Add(ResourceType.Script, new List<string>());
            dictionary.Add(ResourceType.Definition, new List<string>());
            return dictionary;
        }

#endregion
    }
}
