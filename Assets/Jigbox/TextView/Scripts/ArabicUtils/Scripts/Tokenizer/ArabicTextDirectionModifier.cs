/**
 * Additional Language Utility Library
 * Copyright(c) 2018 KLab, Inc. All Rights Reserved.
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

using System.Collections.Generic;

namespace ArabicUtils.Tokenizer
{
    public static class ArabicTextDirectionModifier
    {
#region properties

        /// <summary>読み込まれた括弧</summary>
        static List<char> brackets = new List<char>();

        /// <summary>読み込まれた括弧のトークンのインデックス</summary>
        static List<int> bracketIndices = new List<int>();

#endregion

#region public methods

        /// <summary>
        /// 文字の表示方向を修正します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="tokens">トークン</param>
        public static void Modify(string text, List<Token> tokens)
        {
            int lastLTRTokenIndex = 0;
            bool enableLeftToRight = false;

            for (int i = 0; i < tokens.Count; ++i)
            {
                Token token = tokens[i];
                switch (token.type)
                {
                    case TokenType.Arabic:
                        token.isRightToLeft = true;
                        enableLeftToRight = false;
                        break;
                    case TokenType.ArabicNumber:
                        token.isRightToLeft = false;
                        enableLeftToRight = false;
                        break;
                    case TokenType.Letter:
                        token.isRightToLeft = false;
                        if (!enableLeftToRight)
                        {
                            enableLeftToRight = true;
                        }
                        else
                        {
                            ChangeDirection(tokens, lastLTRTokenIndex, i);
                        }
                        lastLTRTokenIndex = i;
                        break;
                    case TokenType.Number:
                        token.isRightToLeft = false;
                        if (enableLeftToRight)
                        {
                            ChangeDirection(tokens, lastLTRTokenIndex, i);
                            lastLTRTokenIndex = i;
                        }
                        break;
                    // ↓ 基本的にrtlとなるが、アラビア語以外の文字に挟まれている状態ではltrに変化する
                    case TokenType.WhiteSpace:
                    case TokenType.Control:
                    case TokenType.LeftBracket:
                    case TokenType.RightBracket:
                    case TokenType.Other:
                        token.isRightToLeft = true;
                        break;
                    default:
                        UnityEngine.Assertions.Assert.IsTrue(false);
                        break;
                }
            }

            ModifyPairedBracket(text, tokens);
        }

#endregion

#region private methods

        /// <summary>
        /// 文字の表示方向を左から右に変更します。
        /// </summary>
        /// <param name="tokens">トークン</param>
        /// <param name="startIndex">開始位置のインデックス</param>
        /// <param name="targetIndex">対象のインデックス</param>
        static void ChangeDirection(List<Token> tokens, int startIndex, int targetIndex)
        {
            for (int i = startIndex; i <= targetIndex; ++i)
            {
                tokens[i].isRightToLeft = false;
            }
        }

        /// <summary>
        /// 対となる括弧の向きを修正します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="tokens">トークン</param>
        static void ModifyPairedBracket(string text, List<Token> tokens)
        {
            brackets.Clear();
            bracketIndices.Clear();

            for (int i = 0; i < tokens.Count; ++i)
            {
                Token token = tokens[i];

                switch (token.type)
                {
                    case TokenType.LeftBracket:
                        // 左括弧(開き括弧)が来たら、スタックする
                        brackets.Add(text[token.startIndex]);
                        bracketIndices.Add(i);
                        break;
                    case TokenType.RightBracket:
                        // 右括弧(閉じ括弧)が来たら、スタックされている括弧に対応しているものがあるか確認する
                        char pairedLeftBracket = BracketTable.GetLeftBracket(text[token.startIndex]);
                        int pairedIndex = -1;
                        for (int j = brackets.Count - 1; j >= 0; --j)
                        {
                            if (brackets[j] == pairedLeftBracket)
                            {
                                pairedIndex = j;
                                break;
                            }
                        }

                        if (pairedIndex == -1)
                        {
                            break;
                        }

                        // 括弧のどちらかが右から左に処理する設定になっている場合は、括弧の間にある文字を確認する
                        if (tokens[bracketIndices[pairedIndex]].isRightToLeft || tokens[i].isRightToLeft)
                        {
                            bool hasArabic = false;
                            bool hasLTRLetter = false;
                            for (int j = bracketIndices[pairedIndex] + 1; j < i; ++j)
                            {
                                switch (tokens[j].type)
                                {
                                    case TokenType.Arabic:
                                    case TokenType.ArabicNumber:
                                        hasArabic = true;
                                        break;
                                    case TokenType.Letter:
                                        hasLTRLetter = true;
                                        break;
                                }
                            }

                            // 括弧の間にアラビア文字以外の文字が含まれていて、アラビア文字が含まれていない場合、
                            // 括弧間に存在する文字は、全て左から右に処理するものとする
                            if (!hasArabic && hasLTRLetter)
                            {
                                ChangeDirection(tokens, bracketIndices[pairedIndex], i);
                            }
                        }

                        // 処理が済んだ括弧を除外する
                        int removeLength = brackets.Count - pairedIndex;
                        brackets.RemoveRange(pairedIndex, removeLength);
                        bracketIndices.RemoveRange(pairedIndex, removeLength);
                        break;
                    default:
                        break;
                }
            }
        }

#endregion
    }
}
