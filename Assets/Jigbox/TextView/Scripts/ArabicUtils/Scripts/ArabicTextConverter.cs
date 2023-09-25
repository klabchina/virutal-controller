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

using System.Text;
using System.Collections.Generic;
using ArabicUtils.Tokenizer;

namespace ArabicUtils
{
    public static class ArabicTextConverter
    {
#region properties

        /// <summary>変換中かどうか</summary>
        static bool isConverting = false;

        /// <summary>キャレット参照単位の情報を生成するかどうか</summary>
        static bool isCreateCaretUnit = false;

        /// <summary>キャレット参照単位の情報を生成するかどうか</summary>
        public static bool IsCreateCaretUnit
        {
            get
            {
                return isCreateCaretUnit;
            }

            set
            {
                if (isConverting)
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.LogError("Can't change flag, when converting.");
#endif
                }
                else
                {
                    isCreateCaretUnit = value;
                }
            }
        }

        /// <summary>キャレット参照単位の情報</summary>
        static List<CaretUnit> caretUnits = new List<CaretUnit>();

        /// <summary>キャレット参照単位の情報</summary>
        public static List<CaretUnit> CaretUnits { get { return caretUnits; } }

        /// <summary>StringBuilder</summary>
        static StringBuilder builder = new StringBuilder();

#endregion

#region public methods

        /// <summary>
        /// 文字列をアラビア語表示用に変換します。
        /// </summary>
        /// <param name="source">元となる文字列</param>
        /// <returns></returns>
        public static string Convert(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            isConverting = true;

            ArabicTextTokenizer.Tokenize(source);
            List<Token> tokens = ArabicTextTokenizer.Tokens;
            builder.Length = 0;
            caretUnits.Clear();
            int insertIndex = 0;

            for (int i = 0; i < tokens.Count; ++i)
            {
                Token token = tokens[i];

                // アラビア語部分は変換が必要
                if (token.type == TokenType.Arabic)
                {
                    AppendArabic(source, token);
                    insertIndex = builder.Length;
                }
                // 右から左の状態で括弧は入力と表示が逆になる
                else if ((token.type == TokenType.LeftBracket || token.type == TokenType.RightBracket)
                    && token.isRightToLeft)
                {
                    AppendBracket(source, token);
                    insertIndex = builder.Length;
                }
                else
                {
                    if (token.isRightToLeft)
                    {
                        Append(source, token);
                        insertIndex = builder.Length;
                    }
                    else
                    {
                        Insert(source, token, insertIndex);
                    }
                }
            }

            isConverting = false;

            return builder.ToString();
        }

        /// <summary>
        /// 文字列を鏡文字表示状態で左から右に表示できる状態に変換します。
        /// </summary>
        /// <param name="source">元となる文字列</param>
        /// <returns></returns>
        public static string ConvertMirrorLTR(string source)
        {
            builder.Length = 0;

            for (int i = 0; i < source.Length; ++i)
            {
                builder.Insert(0, source[i]);
            }

            return builder.ToString();
        }

#endregion

#region private methods

        /// <summary>
        /// アラビア語を末尾に追加します。
        /// </summary>
        /// <param name="source">元の文字列</param>
        /// <param name="token">トークン</param>
        static void AppendArabic(string source, Token token)
        {
            string arabicText = PresentationFormConverter.GetPresentationFormText(source, token, isCreateCaretUnit);
            int length = arabicText.Length;

            if (length == 0)
            {
                // 普通の入力なら、基本的にreturnするような事にはならないはず
                return;
            }

            List<CaretUnit.TextInfo> convertedIndices = null;
            if (isCreateCaretUnit)
            {
                convertedIndices = PresentationFormConverter.ConvertedIndices;
            }

            // 参照やメソッドの実行回数を減らすために、参照をズラしていくので
            // 予め、次の参照先を先頭にしてから処理する
            ArabicFormSet prev = null;
            ArabicFormSet current = null;
            ArabicFormSet next = ArabicTable.GetFormSet(arabicText[0]);

            for (int i = 0; i < length; ++i)
            {
                if (isCreateCaretUnit)
                {
                    caretUnits.Add(new CaretUnit(convertedIndices[i].index, convertedIndices[i].length, builder.Length, 1, true));
                }

                current = next;
                next = i + 1 < length ? ArabicTable.GetFormSet(arabicText[i + 1]) : null;

                // 変形しない文字の場合はそのまま処理する
                if (current == null)
                {
                    builder.Append(arabicText[i]);
                    prev = null;
                    continue;
                }

                builder.Append(PresentationFormConverter.ModifyForm(prev, current, next));
                prev = current;
            }
        }

        /// <summary>
        /// 指定された範囲の括弧を表示を反転させて、末尾に追加します。
        /// </summary>
        /// <param name="source">元の文字列</param>
        /// <param name="token">トークン</param>
        static void AppendBracket(string source, Token token)
        {
            if (isCreateCaretUnit)
            {
                caretUnits.Add(new CaretUnit(token.startIndex, 1, builder.Length, 1, true));
            }

            if (token.type == TokenType.LeftBracket)
            {
                builder.Append(BracketTable.GetRightBracket(source[token.startIndex]));
            }
            else
            {
                builder.Append(BracketTable.GetLeftBracket(source[token.startIndex]));
            }
        }

        /// <summary>
        /// 指定された範囲の文字列を末尾に追加します。
        /// </summary>
        /// <param name="source">元の文字列</param>
        /// <param name="token">トークン</param>
        static void Append(string source, Token token)
        {
            int from = token.startIndex;
            int to = token.startIndex + token.length;

            for (int i = from; i < to; ++i)
            {
                if (isCreateCaretUnit)
                {
                    caretUnits.Add(new CaretUnit(i, 1, builder.Length, 1, true));
                }

                builder.Append(source[i]);
            }
        }

        /// <summary>
        /// 指定された範囲の文字列を指定された位置に挿入します。
        /// </summary>
        /// <param name="source">元の文字列</param>
        /// <param name="token">トークン</param>
        /// <param name="insertIndex">挿入位置のインデックス</param>
        static void Insert(string source, Token token, int insertIndex)
        {
            int from = token.startIndex;
            int to = token.startIndex + token.length;

            for (int i = from; i < to; ++i)
            {
                if (isCreateCaretUnit)
                {
                    for (int j = insertIndex; j < caretUnits.Count; ++j)
                    {
                        caretUnits[j] = caretUnits[j].Shift();
                    }
                    caretUnits.Add(new CaretUnit(i, 1, insertIndex, 1, false));
                }

                builder.Insert(insertIndex, source[i]);
            }
        }

#endregion
    }
}
