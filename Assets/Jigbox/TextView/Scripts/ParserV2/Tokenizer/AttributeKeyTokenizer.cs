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
    public sealed class AttributeKeyTokenizer : TrimTokenizer
    {
        // 読み込み位置から空白文字をトリミングし、
        // 文字、数字が来たら、それ以外の文字が来るまでを属性名として抽出する
        // 抽出が始まる前に、タグの終了文字が来たらテキストに、
        // "/>"が来たら閉じタグを挿入してテキストへと移る
        // 属性抽出後、'='が来たら属性の値へと移る

#region properties

        /// <summary>自身のトークナイズ状態</summary>
        protected override TokenizeMode OwnMode { get { return TokenizeMode.Attribute; } }

#endregion

#region protected methods

        /// <summary>
        /// 次にトークンを読み出し始める位置まで読み進めます。
        /// </summary>
        protected override void SeekNextTokenizePoint()
        {
            char character = tokenizeInfo.Character;

            // スペースは無視
            if (char.IsWhiteSpace(character))
            {
                return;
            }

            // '='以外は全て不正
            if (character == CharEqual)
            {
                Mode = TokenizeMode.AttributeValue;
            }
            else
            {
                tokenizeInfo.SetError("属性名の後に = が入力されなかったため、属性の値を設定できませんでした。");
                return;
            }
        }

        /// <summary>
        /// トークンを生成します。
        /// </summary>
        /// <param name="str">トークンとして扱う文字列</param>
        protected override void CreateToken(string str)
        {
            tokens.Add(new AttributeKeyToken(str));
        }

#endregion
    }
}
