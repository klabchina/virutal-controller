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

using System.Collections.Generic;
using ArabicUtils;
using UnityEngine;
using Jigbox.TextView.ParserV2;
using ThaiUtils;

namespace Jigbox.TextView.Markup
{
    public class MarkupParserBase
    {
        public bool TreatNewLineAsLineBreak { get; set; }

        public MarkupParserBase()
        {
            this.TreatNewLineAsLineBreak = false;
        }

        protected virtual TextRun[] VisitMarkupNode(MarkupNode node)
        {
            if (node is TextNode)
            {
                string content = (node as TextNode).TextContent;

                if (this.TreatNewLineAsLineBreak)
                {
                    string[] segments = content.Split('\n');
                    // 改行で分割した文字列 + その間に挟むLineBreak分の要素を生成
                    TextRun[] textChars = new TextRun[Mathf.Max(segments.Length * 2 - 1, 0)];

                    // 配列の中身が以下の様になるように要素を詰める
                    // [TextCharacters] [LineBreak] [TextCharacters] [LineBreak] ... [TextCharacter]
                    for (int i = 0; i < segments.Length; ++i)
                    {
                        textChars[i * 2] = new TextCharacters(segments[i]);
                        if (i < segments.Length - 1)
                        {
                            textChars[i * 2 + 1] = new LineBreak();
                        }
                    }

                    return textChars;
                }

                return new TextRun[]
                {
                    new TextCharacters(content.Replace("\n", "").Replace("\r", ""))
                };
            }

            if (node is Element)
            {
                Element element = node as Element;
                return this.VisitMarkupElement(element, element.TagName.ToUpper());
            }

            return GetChildrenTextRuns(node);
        }

        protected virtual TextRun[] VisitMarkupElement(Element element, string tagNameUpper)
        {
            if (tagNameUpper == "RUBY")
            {
                return new TextRun[]
                {
                    new TextCharactersRubyGroup(element.TextContent, element.GetAttribute("Value"))
                };
            }

            if (tagNameUpper == "BR")
            {
                return new TextRun[] { new LineBreak() };
            }

            return GetChildrenTextRuns(element);
        }

        protected TextRun[] GetChildrenTextRuns(MarkupNode node)
        {
            int childrenCount = node.ChildrenCount;
            if (childrenCount == 0)
            {
                return new TextRun[0];
            }

            // タグをネストすると再起的に呼び出されるため、毎回作る
            List<TextRun[]> list = new List<TextRun[]>(childrenCount);
            int textRunCount = 0;

            for (int i = 0; i < childrenCount; ++i)
            {
                TextRun[] runs = VisitMarkupNode(node.Children[i]);
                textRunCount += runs.Length;
                list.Add(runs);
            }

            TextRun[] result = new TextRun[textRunCount];
            int index = 0;

            foreach (TextRun[] runs in list)
            {
                foreach (TextRun run in runs)
                {
                    result[index] = run;
                    ++index;
                }
            }

            return result;
        }

        protected virtual TextSource ConvertNodeToTextSource(MarkupNode rootNode, string errorMessage, TextLanguageType languageType)
        {
            TextRun[] runs = VisitMarkupNode(rootNode);

            if (languageType == TextLanguageType.Arabic)
            {
                bool isExistArabic = false;

                foreach (var run in runs)
                {
                    TextCharacters textCharacters = run as TextCharacters;

                    if (textCharacters == null)
                    {
                        continue;
                    }

                    if (ArabicLetter.IsExistArabic(textCharacters.RawCharacters))
                    {
                        isExistArabic = true;
                        break;
                    }
                }

                // アラビア語が含まれている場合は、全ての文字をrtl用に変換する
                if (isExistArabic)
                {
                    bool forceLTR = false;

                    for (int i = 0; i < runs.Length; ++i)
                    {
                        if (runs[i] is TextBiDirectionOverride)
                        {
                            forceLTR = (runs[i] as TextBiDirectionOverride).ForceLTR;
                            continue;
                        }

                        if (runs[i] is TextCharacters)
                        {
                            TextCharacters textCharacters = runs[i] as TextCharacters;

                            // TextCharactersRubyGroupはアラビア語に使われない想定
                            if (forceLTR)
                            {
                                // 左から右に表示している間は、アラビア文字の変換などは行われなくなる
                                runs[i] = new TextCharacters(
                                    ArabicTextConverter.ConvertMirrorLTR(textCharacters.RawCharacters));
                            }
                            else
                            {
                                runs[i] = new ArabicTextCharacters(textCharacters.RawCharacters);
                            }
                        }
                    }
                }

                return new TextSource(runs, errorMessage);
            }

            if (languageType == TextLanguageType.Thai)
            {
                for (int i = 0; i < runs.Length; ++i)
                {
                    if (runs[i] is TextCharacters)
                    {
                        TextCharacters textCharacters = runs[i] as TextCharacters;
                        if (ThaiTable.IsLayoutNeeded(textCharacters.RawCharacters))
                        {
                            runs[i] = new ThaiTextCharacters(textCharacters.RawCharacters);
                        }
                    }
                }
            }

            return new TextSource(runs, errorMessage);
        }

        public TextSource ParseTextSource(string value, TextLanguageType languageType = TextLanguageType.Default)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new TextSource(new TextRun[] { }, string.Empty);
            }

            MarkupNode root;
            string errorMessage = string.Empty;

            ParseResult result = TextParser.TryParse(value);
            if (result.WasSuccessful)
            {
                root = result.Node;
            }
            else
            {
                root = new MarkupNode();
                root.AppendChild(new TextNode(value));
                errorMessage = result.Message;
            }

            return this.ConvertNodeToTextSource(root, errorMessage, languageType);
        }
    }
}
