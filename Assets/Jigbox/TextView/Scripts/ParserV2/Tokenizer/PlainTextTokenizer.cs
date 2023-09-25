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

namespace Jigbox.TextView.ParserV2
{
    public sealed class PlainTextTokenizer : TextTokenizer
    {
#region public methods

        /// <summary>
        /// 現在読み込まれている文字列から本文用のトークンを生成します。
        /// </summary>
        public void CreateToken()
        {
            if (fetchLength == 0)
            {
                return;
            }

            tokens.Add(new TextToken(tokenizeInfo.Text.Substring(fetchStartIndex, fetchLength)));
        }

#endregion

#region protected methods

        /// <summary>
        /// 文字列からトークン化する文字列までを読み出します。
        /// </summary>
        /// <returns>読み出しが完了して、トークン化したら<c>true</c>を返します。</returns>
        protected override bool Read()
        {
            char character = tokenizeInfo.Character;

            switch (character)
            {
                // タグ
                case CharOpenTag:
                    CreateToken();
                    // '<'直後が/の場合は閉じタグ
                    Mode = (!tokenizeInfo.IsFinalCharacter && tokenizeInfo.NextCharacter == CharSlash) 
                        ? TokenizeMode.EndTag : TokenizeMode.Tag;
                    break;
                // 文字参照
                case CharAmpersand:
                    CreateToken();
                    Mode = TokenizeMode.CharacterReference;
                    break;
                default:
                    ++fetchLength;
                    break;
            }

            // 閉じタグの場合、"</"で2文字分のインデックスを進める
            if (Mode == TokenizeMode.EndTag)
            {
                tokenizeInfo.Index += 2;
            }
            else
            {
                ++tokenizeInfo.Index;
            }

            return Mode != TokenizeMode.Text;
        }

#endregion
    }
}
