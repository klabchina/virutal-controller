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
    public sealed class AttributeValueTokenizer : ThreeProcessTokenizer
    {
        // 引用符(' or ")が来るまでの空白文字をトリミングし、
        // そこから、再度引用符が来るまでを属性の値として抽出する
        // 値の抽出後は、後方の空白文字のトリミングは行わずに属性へと移る

#region properties

        /// <summary>シングルクォーテーションマークで開始したかどうか</summary>
        bool isSingleQuote = false;

        /// <summary>値の内部にクォーテーションマークが含まれているかどうか</summary>
        bool isExistQuoteInValue = false;

        /// <summary>自身のトークナイズ状態</summary>
        protected override TokenizeMode OwnMode { get { return TokenizeMode.AttributeValue; } }

#endregion

#region protected methods

        /// <summary>
        /// 初期化します。
        /// </summary>
        protected override void Init()
        {
            base.Init();
            isSingleQuote = false;
            isExistQuoteInValue = false;
        }

        /// <summary>
        /// トークン化する文字列を読み出し始める位置まで読み進めます。
        /// </summary>
        protected override void SeekTokenizePoint()
        {
            char character = tokenizeInfo.Character;

            // スペースは無視
            if (char.IsWhiteSpace(character))
            {
                if (tokenizeInfo.IsFinalCharacter)
                {
                    tokenizeInfo.SetError("属性の値の先頭には、 ' か \" を入力しなければいけません。");
                }
                return;
            }

            bool isSingleQuote = character == CharSingleQuote;
            bool isDoubleQuote = character == CharDoubleQuote;

            if (isSingleQuote || isDoubleQuote)
            {
                state = State.TokenizeProcess;
                this.isSingleQuote = isSingleQuote;
                fetchStartIndex = tokenizeInfo.Index + 1;
            }
            else
            {
                tokenizeInfo.SetError("属性の値の先頭は、 ' 、 \" 以外入力できません。");
            }
        }

        /// <summary>
        /// 文字列を読み出して、トークン化します。
        /// </summary>
        protected override void ReadAndCreateToken()
        {
            char character = tokenizeInfo.Character;

            if (character != (isSingleQuote ? CharSingleQuote : CharDoubleQuote))
            {
                ++fetchLength;

                // "\\\'" or "\\\""の場合は、属性の終了と判断せずに属性に含めるように処理する
                if (character == CharBackSlash && !tokenizeInfo.IsFinalCharacter)
                {
                    if (tokenizeInfo.NextCharacter == (isSingleQuote ? CharSingleQuote : CharDoubleQuote))
                    {
                        isExistQuoteInValue = true;
                        ++tokenizeInfo.Index;
                        ++fetchLength;
                    }
                    // '\\'の後にクォーテーションが続かない場合はエラー扱いとする
                    else
                    {
                        tokenizeInfo.SetError("属性の値は、クォーテーションのエスケープの目的以外で \\ は使えません。");
                    }
                }

                return;
            }

            string str = tokenizeInfo.Text.Substring(fetchStartIndex, fetchLength);
            if (isExistQuoteInValue)
            {
                str = UnescapeQuote(str);
            }

            tokens.Add(new AttributeValueToken(str));
            state = State.PostProcess;
        }
        
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

            // 文字、または数字が来たらそこから抽出を開始
            if (IsLetterOrDigitOrTrail(character))
            {
                // ここでは読み出した文字列を実際には使わないのでインデックスを元に戻す
                --tokenizeInfo.Index;
                Mode = TokenizeMode.Attribute;
                return;
            }

            switch (character)
            {
                case CharCloseTag:
                    Mode = TokenizeMode.Text;
                    break;
                case CharSlash:
                    // "/>"が来たらタグを閉じてテキスト状態に戻す
                    if (tokenizeInfo.IsFinalCharacter)
                    {
                        tokenizeInfo.SetError("> がないため、タグが正しく閉じられませんでした。");
                        return;
                    }

                    if (tokenizeInfo.NextCharacter == CharCloseTag)
                    {
                        // トークン化された文字列の中から、直近でタグとして
                        // トークン化されたものを取り出す
                        Token lastTag = null;
                        for (int i = tokens.Count - 1; i >= 0; --i)
                        {
                            if (tokens[i] is BeginTagToken)
                            {
                                lastTag = tokens[i];
                                break;
                            }
                        }

                        tokens.Add(new EndTagToken(lastTag.String, true));
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

#endregion

#region private methods

        /// <summary>
        /// 文字列に含まれている引用符表現(\" or \')からエスケープを外します。
        /// </summary>
        /// <param name="value">文字列</param>
        /// <returns></returns>
        static string UnescapeQuote(string value)
        {
            stringBuilder.Length = 0;

            for (int i = 0; i < value.Length; ++i)
            {
                char character = value[i];
                if (character == CharBackSlash && i + 1 < value.Length)
                {
                    char nextCharacter = value[i + 1];
                    if (nextCharacter == CharSingleQuote)
                    {
                        stringBuilder.Append(CharSingleQuote);
                        ++i;
                    }
                    else if (nextCharacter == CharDoubleQuote)
                    {
                        stringBuilder.Append(CharDoubleQuote);
                        ++i;
                    }
                }
                else
                {
                    stringBuilder.Append(character);
                }
            }

            return stringBuilder.ToString();
        }

#endregion
    }
}
