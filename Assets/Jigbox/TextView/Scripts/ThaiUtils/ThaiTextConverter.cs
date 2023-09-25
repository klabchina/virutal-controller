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
using ThaiUtils.Tokenizer;
using ThaiUtils.UnitParser;
using ThaiUtils.Layout;

namespace ThaiUtils
{
    /// <summary>
    /// タイ語のテキストをTextViewで表示できるようにするための変換モジュール
    /// </summary>
    public static class ThaiTextConverter
    {
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

        /// <summary>テキストの構成</summary>
        static List<TextUnit> texts = new List<TextUnit>();

        /// <summary>テキストの構成</summary>
        public static List<TextUnit> Texts { get { return texts; } }

        /// <summary>キャレット参照単位の情報</summary>
        static List<CaretUnit> caretUnits = new List<CaretUnit>();

        /// <summary>キャレット参照単位の情報</summary>
        public static List<CaretUnit> CaretUnits { get { return caretUnits; } }

        /// <summary>
        /// テキストを変換します。
        /// </summary>
        /// <param name="source">テキスト</param>
        public static void Convert(string source)
        {
            isConverting = true;

            texts.Clear();
            caretUnits.Clear();

            ThaiTokenizer.Tokenize(source);
            List<Token> tokens = ThaiTokenizer.Tokens;

            int convertedDstLength = 0;

            for (int i = 0; i < tokens.Count; ++i)
            {
                Token token = tokens[i];

                if (token.type == TokenType.NeedLayout)
                {
                    // 最小分割可能単位に分ける
                    ThaiUnitParser.Parse(source, token.startIndex, token.length);
                    // レイアウト情報を作成する
                    ThaiLayoutGenerator.Generate(source,
                        token.startIndex,
                        ThaiUnitParser.CharacterTypes,
                        ThaiUnitParser.Units,
                        convertedDstLength);

                    texts.AddRange(ThaiLayoutGenerator.LayoutedUnits);
                    if (IsCreateCaretUnit)
                    {
                        List<CaretUnit> layoutedCaretUnit = ThaiLayoutGenerator.CaretUnits;
                        for (int j = 0; j < layoutedCaretUnit.Count; ++j)
                        {
                            convertedDstLength += layoutedCaretUnit[j].destination.length;
                        }
                        caretUnits.AddRange(layoutedCaretUnit);
                    }
                }
                else
                {
                    texts.Add(new TextUnit(source.ToCharArray(token.startIndex, token.length)));
                    if (isCreateCaretUnit)
                    {
                        // タイ語以外の文字は1文字ずつ処理
                        for (int j = 0; j < token.length; ++j)
                        {
                            caretUnits.Add(new CaretUnit(token.startIndex + j, 1, convertedDstLength + j, 1));
                        }
                        convertedDstLength += token.length;
                    }
                }
            }

            isConverting = false;
        }
    }
}
