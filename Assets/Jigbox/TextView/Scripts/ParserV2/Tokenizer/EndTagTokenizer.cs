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
    public sealed class EndTagTokenizer : TrimTokenizer
    {
        // "</"以降、文字、数字が来るまでの空白文字をトリミングし、
        // そこから文字、数字以外が来るまでをタグとして抽出する
        // タグ抽出後は、空白文字をトリミングして、タグの終了文字が来たらテキストに移る

#region properties

        /// <summary>自身のトークナイズ状態</summary>
        protected override TokenizeMode OwnMode { get { return TokenizeMode.EndTag; } }

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

            // タグの終了文字以外は全て不正
            if (character == CharCloseTag)
            {
                Mode = TokenizeMode.Text;
            }
            else
            {
                tokenizeInfo.SetError("> 以外の文字が入力されたため、閉じタグを正しく閉じられませんでした。");
                return;
            }
        }

        /// <summary>
        /// トークンを生成します。
        /// </summary>
        /// <param name="str">トークンとして扱う文字列</param>
        protected override void CreateToken(string str)
        {
            tokens.Add(new EndTagToken(str));
        }

#endregion
    }
}
