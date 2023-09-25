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
using System.Globalization;
using System.Collections.Generic;

namespace Jigbox.TextView.ParserV2
{
    public sealed class CharacterReferenceTokenizer : TextTokenizer
    {
        // '&'以降、';'が来るまでの文字を抽出し、
        // 該当する文字(文字列)に置き換え、その後はテキストへと移る

#region constants

        /// <summary>文字参照で扱う文字列と対応する文字(文字列)のリスト</summary>
        static readonly Dictionary<string, string> CharacterEntityReferences =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "amp", "&" },
                { "apos", "'" },
                { "quot", "\"" },
                { "lt", "<" },
                { "gt", ">" },
                { "shy", "" }
            };

#endregion

#region protected methods

        /// <summary>
        /// 文字列からトークン化する文字列までを読み出します。
        /// </summary>
        /// <returns>読み出しが完了して、トークン化したら<c>true</c>を返します。</returns>
        protected override bool Read()
        {
            if (tokenizeInfo.Character == CharSemicolon)
            {
                if (fetchLength == 0)
                {
                    tokenizeInfo.SetError("文字参照を行うための識別用の文字列が入力されていません。");
                }
                else
                {
                    tokens.Add(new TextToken(Resolve(tokenizeInfo.Text.Substring(fetchStartIndex, fetchLength))));
                    Mode = TokenizeMode.Text;
                }
            }
            else
            {
                ++fetchLength;
            }

            ++tokenizeInfo.Index;

            return Mode != TokenizeMode.CharacterReference;
        }

#endregion

#region private methods

        /// <summary>
        /// 文字列から文字参照の条件に該当する文字を返します。
        /// </summary>
        /// <param name="value">文字列</param>
        /// <returns>文字参照として扱える文字列であれば、該当する文字列を返し、扱えない文字列であれば空文字列を返します。</returns>
        static string Resolve(string value)
        {
            if (value.StartsWith("#x"))
            {
                // &#xNNNN;
                var result = 0;
                if (Int32.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out result))
                {
                    return new string((char) result, 1);
                }
            }
            else if (value.StartsWith("#"))
            {
                // &#NN;
                var result = 0;
                if (Int32.TryParse(value.Substring(1), NumberStyles.Integer, null, out result))
                {
                    return new string((char) result, 1);
                }
            }
            else if (CharacterEntityReferences.ContainsKey(value))
            {
                // &XXX;
                return CharacterEntityReferences[value];
            }
            return string.Empty;
        }

#endregion
    }
}
