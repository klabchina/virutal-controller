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
    public abstract class TrimTokenizer : ThreeProcessTokenizer
    {
#region proptected methods

        /// <summary>
        /// トークン化する文字列を読み出し始める位置まで読み進めます。
        /// </summary>
        protected override void SeekTokenizePoint()
        {
            char character = tokenizeInfo.Character;

            // スペースは無視
            if (char.IsWhiteSpace(character))
            {
                return;
            }

            // 文字(先頭文字の場合は、数字、-、_は非許容)が来たらそこから抽出を開始
            if (char.IsLetter(character))
            {
                state = State.TokenizeProcess;
                fetchStartIndex = tokenizeInfo.Index;
                fetchLength = 1;
            }
            else
            {
                tokenizeInfo.SetError("数字、記号からは開始出来ません。");
                return;
            }
        }

        /// <summary>
        /// 文字列を読み出して、トークン化します。
        /// </summary>
        protected override void ReadAndCreateToken()
        {
            // 文字、または数字が続いている間はタグ名として扱う
            if (IsLetterOrDigitOrTrail(tokenizeInfo.Character))
            {
                ++fetchLength;
                return;
            }

            CreateToken(tokenizeInfo.Text.Substring(fetchStartIndex, fetchLength));
            // 読み込まなかった文字の分のインデックスを戻す
            --tokenizeInfo.Index;
            state = State.PostProcess;
        }

        /// <summary>
        /// トークンを生成します。
        /// </summary>
        /// <param name="str">トークンとして扱う文字列</param>
        protected abstract void CreateToken(string str);

#endregion
    }
}
