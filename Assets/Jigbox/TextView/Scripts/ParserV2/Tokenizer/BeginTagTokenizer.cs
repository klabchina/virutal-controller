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
    public sealed class BeginTagTokenizer : TrimTokenizer
    {
        // '<'以降、文字、数字が来るまでの空白文字をトリミングし、
        // そこから文字、数字以外が来るまでをタグとして抽出する
        // タグ抽出後は、空白文字をトリミングして、タグの終了文字ならテキスト、
        // '='なら属性、"/>"なら閉じタグを挿入してテキストへと移る

#region properties

        /// <summary>自身のトークナイズ状態</summary>
        protected override TokenizeMode OwnMode { get { return TokenizeMode.Tag; } }

#endregion

#region proptected methods

        /// <summary>
        /// 次にトークンを読み出し始める位置まで読み進めます。
        /// </summary>
        protected override void SeekNextTokenizePoint()
        {
            char character = tokenizeInfo.Character;

            // スペースは無視
            if (char.IsWhiteSpace(character))
            {
                // 次の文字が記号などではない場合は、次から属性名の抽出を行う
                if (!tokenizeInfo.IsFinalCharacter && IsLetterOrDigitOrTrail(tokenizeInfo.NextCharacter))
                {
                    Mode = TokenizeMode.Attribute;
                }
                return;
            }

            switch (character)
            {
                case CharCloseTag:
                    Mode = TokenizeMode.Text;
                    break;
                case CharEqual:
                    // 属性名なしで'='が来たら、デフォルトの属性名を設定
                    tokens.Add(new AttributeKeyToken("Value"));
                    Mode = TokenizeMode.AttributeValue;
                    break;
                case CharSlash:
                    // "/>"が来たらタグを閉じてテキスト状態に戻す
                    if (tokenizeInfo.IsFinalCharacter)
                    {
                        // 最後の状態チェック時にエラーとして処理されるのでここではエラーメッセージを入れない
                        return;
                    }

                    if (tokenizeInfo.NextCharacter == CharCloseTag)
                    {
                        // 直近でトークン化された文字列がタグであるはずなので、
                        // 同じ文字列で閉じタグを生成する
                        Token last = tokens[tokens.Count - 1];
                        tokens.Add(new EndTagToken(last.String, true));
                        ++tokenizeInfo.Index;
                        Mode = TokenizeMode.Text;
                    }
                    else
                    {
                        tokenizeInfo.SetError("/ の後に > が入力されなかったため、タグを正しく閉じられませんでした。");
                        return;
                    }
                    break;
                default:
                    tokenizeInfo.SetError("属性名には、文字、数字、 - 、 _ 以外の文字は入力できません。");
                    break;
            }
        }

        /// <summary>
        /// トークンを生成します。
        /// </summary>
        /// <param name="str">トークンとして扱う文字列</param>
        protected override void CreateToken(string str)
        {
            tokens.Add(new BeginTagToken(str));
        }

#endregion
    }
}
