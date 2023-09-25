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
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public static class AdvConstantValueParser
    {
#region public methods

        /// <summary>
        /// ストリーム情報から定数を生成します。
        /// </summary>
        /// <param name="stream">定数ファイルのストリーム情報</param>
        /// <param name="resource">対象ファイルのパス</param>
        /// <returns></returns>
        public static Dictionary<string, string[]> CreateConstantValues(string[] stream, string resource)
        {
            Dictionary<string, string[]> values = new Dictionary<string, string[]>();

            for (int i = 0; i < stream.Length; ++i)
            {
                if (string.IsNullOrEmpty(stream[i]))
                {
                    continue;
                }

                AdvScriptConstantValue value = new AdvScriptConstantValue(stream[i]);

                if (!string.IsNullOrEmpty(value.ErrorMessage))
                {
#if UNITY_EDITOR
                    LogErrorConstantValue(i + 1, value, resource);
#endif
                    continue;
                }

                if (values.ContainsKey(value.Name))
                {
                    values[value.Name] = value.Param;
                }
                else
                {
                    values.Add(value.Name, value.Param);
                }
            }

            return values;
        }

        /// <summary>
        /// エラー内容を出力します。
        /// </summary>
        /// <param name="lineIndex">対象の行番号</param>
        /// <param name="value">エラーとなっている定数</param>
        /// <param name="resource">対象スクリプトのパス</param>
        public static void LogErrorConstantValue(int lineIndex, AdvScriptConstantValue value, string resource)
        {
#if UNITY_EDITOR || NOVELKIT_DEBUG
            string text = string.Empty;
            foreach (string param in value.BaseParam)
            {
                text += param + " ";
            }

            AdvLog.Error("定数の定義に失敗しました！"
                + "\n対象ファイル : " + resource
                + "\n" + lineIndex + " : " + text
                + "\nメッセージ : " + value.ErrorMessage);
#endif
        }

#endregion
    }
}
