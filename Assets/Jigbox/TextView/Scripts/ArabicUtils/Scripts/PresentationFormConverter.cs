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
using System.Text;
using ArabicUtils.Tokenizer;

namespace ArabicUtils
{
    public static class PresentationFormConverter
    {
#region properties

        /// <summary>アラビア語部分用のStringBuilder</summary>
        static StringBuilder arabicBuilder = new StringBuilder();

        /// <summary>プレゼンテーションフォームに変換した際の元の文字列のインデックスの参照</summary>
        static List<CaretUnit.TextInfo> convertedIndices = new List<CaretUnit.TextInfo>();

        /// <summary>プレゼンテーションフォームに変換した際の元の文字列のインデックスの参照</summary>
        public static List<CaretUnit.TextInfo> ConvertedIndices { get { return convertedIndices; } }

#endregion

#region public methods

        /// <summary>
        /// アラビア語をプレゼンテーションフォームに変換して取得します。
        /// </summary>
        /// <param name="source">元の文字列</param>
        /// <param name="token">トークン</param>
        /// <param name="isCreateCaretUnits">キャレット参照単位の情報を生成するかどうか</param>
        /// <returns></returns>
        public static string GetPresentationFormText(string source, Token token, bool isCreateCaretUnits = false)
        {
            arabicBuilder.Length = 0;
            convertedIndices.Clear();

            int from = token.startIndex;
            int to = token.startIndex + token.length;
            char lastCharacter = ArabicLetter.InvalidUnicodeCharacter;

            for (int i = from; i < to; ++i)
            {
                char character = source[i];

                // シャクルは仕組み的に表示できないので、除去する
                if (ShaklTable.IsShakl(character))
                {
                    continue;
                }

                // 変形する文字は全てプレゼンテーションに変換した状態で扱うようにする
                character = ArabicTable.GetPresentationForm(character);

                bool isLamAlef = false;
                // Lam、Alefが続けて入力された場合は、結合して1文字として扱う
                if (lastCharacter == (char) ArabicLetter.PresentationForms.Lam)
                {
                    char ligatured = LigatureLamAlef(character);
                    if (character != ligatured)
                    {
                        isLamAlef = true;
                        character = ligatured;
                        // ラーム・アリフとして入れ直すために既に入っているLamを外す
                        arabicBuilder.Remove(arabicBuilder.Length - 1, 1);
                        if (isCreateCaretUnits)
                        {
                            convertedIndices.RemoveAt(convertedIndices.Count - 1);
                        }
                    }
                }

                arabicBuilder.Append(character);
                if (isCreateCaretUnits)
                {
                    convertedIndices.Add(new CaretUnit.TextInfo(isLamAlef ? i - 1 : i, isLamAlef ? 2 : 1));
                }

                lastCharacter = character;
            }

            return arabicBuilder.ToString();
        }

        /// <summary>
        /// 文字の形を位置に合わせて変形させます。
        /// </summary>
        /// <param name="prev">手前の文字セット</param>
        /// <param name="current">現在の文字セット</param>
        /// <param name="next">次の文字セット</param>
        /// <returns></returns>
        public static char ModifyForm(ArabicFormSet prev, ArabicFormSet current, ArabicFormSet next)
        {
            if (IsFinal(prev, current, next))
            {
                return (char) current.final;
            }
            else if (IsInitial(prev, current, next))
            {
                return (char) current.initial;
            }
            else if (IsMedial(prev, current, next))
            {
                return (char) current.medial;
            }
            else
            {
                return (char) current.isolated;
            }
        }

        /// <summary>
        /// 現在の文字が尾字で表示されるかを返します。
        /// </summary>
        /// <param name="prev">手前の文字セット</param>
        /// <param name="current">現在の文字セット</param>
        /// <param name="next">次の文字セット</param>
        /// <returns></returns>
        public static bool IsFinal(ArabicFormSet prev, ArabicFormSet current, ArabicFormSet next)
        {
            // そもそも変形できない
            if (!current.hasFinal)
            {
                return false;
            }
            // 頭字、中字から続かない
            if (prev == null || !prev.hasInitialAndMedial)
            {
                return false;
            }
            // 自身が中字を持っていて、尾字に続く
            if (current.hasInitialAndMedial && next != null && next.hasFinal)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 現在の文字が頭字で表示されるかを返します。
        /// </summary>
        /// <param name="prev">手前の文字セット</param>
        /// <param name="current">現在の文字セット</param>
        /// <param name="next">次の文字セット</param>
        /// <returns></returns>
        public static bool IsInitial(ArabicFormSet prev, ArabicFormSet current, ArabicFormSet next)
        {
            // そもそも変形できない
            if (!current.hasInitialAndMedial)
            {
                return false;
            }
            // 頭字、中字から続く
            if (prev != null && prev.hasInitialAndMedial)
            {
                return false;
            }
            // 中字、尾字に続かない
            if (next == null || (!next.hasInitialAndMedial && !next.hasFinal))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 現在の文字が中字で表示されるかを返します。
        /// </summary>
        /// <param name="prev">手前の文字セット</param>
        /// <param name="current">現在の文字セット</param>
        /// <param name="next">次の文字セット</param>
        /// <returns></returns>
        public static bool IsMedial(ArabicFormSet prev, ArabicFormSet current, ArabicFormSet next)
        {
            // そもそも変形できない
            if (!current.hasInitialAndMedial)
            {
                return false;
            }
            // 頭字、中字から続かない
            if (prev == null || !prev.hasInitialAndMedial)
            {
                return false;
            }
            // 尾字に続かない
            if (next == null || !next.hasFinal)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Lamに続く文字がAlefであれば、結合された文字を返します。
        /// </summary>
        /// <param name="character">Lamに続く文字</param>
        /// <returns></returns>
        public static char LigatureLamAlef(char character)
        {
            switch (character)
            {
                case (char) ArabicLetter.PresentationForms.AlefMaddaAbove:
                    return (char) ArabicLetter.PresentationForms.LamAlefMaddaAbove;
                case (char) ArabicLetter.PresentationForms.AlefHamzaAbove:
                    return (char) ArabicLetter.PresentationForms.LamAlefHamzaAbove;
                case (char) ArabicLetter.PresentationForms.AlefHamzaBelow:
                    return (char) ArabicLetter.PresentationForms.LamAlefHamzaBelow;
                case (char) ArabicLetter.PresentationForms.Alef:
                    return (char) ArabicLetter.PresentationForms.LamAlef;
                default:
                    return character;
            }
        }

#endregion
    }
}
