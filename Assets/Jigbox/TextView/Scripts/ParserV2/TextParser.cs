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

using UnityEngine;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using Jigbox.TextView.Markup;

namespace Jigbox.TextView.ParserV2
{
    public static class TextParser
    {
#region properties

        /// <summary>パース情報</summary>
        static ParseInfo parseInfo = new ParseInfo();

        /// <summary>本文のトークナイザ</summary>
        static readonly PlainTextTokenizer plainTextTokenizer = new PlainTextTokenizer();

        /// <summary>文字参照のトークナイザ</summary>
        static readonly CharacterReferenceTokenizer characterReferenceTokenizer = new CharacterReferenceTokenizer();

        /// <summary>タグのトークナイザ</summary>
        static readonly BeginTagTokenizer beginTagTokenizer = new BeginTagTokenizer();

        /// <summary>閉じタグのトークナイザ</summary>
        static readonly EndTagTokenizer endTagTokenizer = new EndTagTokenizer();

        /// <summary>属性名のトークナイザ</summary>
        static readonly AttributeKeyTokenizer attributeKeyTokenizer = new AttributeKeyTokenizer();

        /// <summary>属性の値のトークナイザ</summary>
        static readonly AttributeValueTokenizer attributeValueTokenizer = new AttributeValueTokenizer();

#endregion

#region public methods

        /// <summary>
        /// 渡されたテキストをパースします。
        /// </summary>
        /// <param name="text">パースするテキスト</param>
        /// <returns>パース結果の情報を返します。</returns>
        public static ParseResult TryParse(string text)
        {
            Tokenize(text);

            if (TextTokenizer.IsFailed)
            {
                return new ParseResult(TextTokenizer.Message);
            }

            MarkupNode node = Parse();

            if (parseInfo.IsFailed)
            {
                return new ParseResult(parseInfo.Message);
            }

            return new ParseResult(node);
        }

#endregion

#region private methods

        /// <summary>
        /// テキストをパースします。
        /// </summary>
        /// <returns></returns>
        static MarkupNode Parse()
        {
            parseInfo.Init();

            MarkupNode root = new MarkupNode();
            int topNodeCount = TextTokenizer.GetTopNodesCount();
            if (topNodeCount > 0)
            {
                root.SetChildrenCapacity(topNodeCount);
            }

            Stack<Element> tags = new Stack<Element>();
            string attribute = null;

            foreach (Token token in TextTokenizer.GetTokens())
            {
                switch (token.Type)
                {
                    case TokenType.Text:
                        if (tags.Count == 0)
                        {
                            root.AppendChild(token.CreateMarkupNode());
                        }
                        else
                        {
                            tags.Peek().AppendChild(token.CreateMarkupNode());
                        }
                        break;
                    case TokenType.Tag:
                        MarkupNode node = token.CreateMarkupNode();
                        if (tags.Count > 0)
                        {
                            tags.Peek().AppendChild(node);
                        }
                        tags.Push(node as Element);
                        break;
                    case TokenType.EndTag:
                        if (tags.Count == 0)
                        {
                            parseInfo.SetError("閉じタグ(" + token.String + ")に対応するタグが存在しません。");
                            return null;
                        }

                        Element lastOpenedTag = tags.Peek();
                        if (lastOpenedTag.TagName == token.String)
                        {
                            EndTagToken endTagToken = token as EndTagToken;
                            // 空要素として許容されていないタグの子要素が0の場合はエラーとして扱う
                            if (!endTagToken.AllowEmpty && lastOpenedTag.ChildrenCount == 0)
                            {
                                parseInfo.SetError("タグ(" + lastOpenedTag.TagName + ")内に要素が存在しません。");
                                return null;
                            }

                            // スタックに積まれた最後のタグを閉じる際には、ノード自体をリスト側に追加する
                            if (tags.Count == 1)
                            {
                                root.AppendChild(tags.Pop());
                            }
                            else
                            {
                                tags.Pop();
                            }
                        }
                        else
                        {
                            parseInfo.SetError("タグ(" + lastOpenedTag.TagName + ")に対応する閉じタグが入力される前に"
                                + "別な閉じタグ(" + token.String +")が入力されました。");
                            return null;
                        }
                        break;
                    case TokenType.Attribute:
                        if (string.IsNullOrEmpty(attribute))
                        {
                            attribute = token.String;
                        }
                        else
                        {
                            UnityEngine.Assertions.Assert.IsTrue(false, "Failed tokenize! Attribute token is not contiguous!");
                            return null;
                        }
                        break;
                    case TokenType.AttributeValue:
                        MarkupNode tag = tags.Peek();
                        if (!tag.Attributes.ContainsKey(attribute))
                        {
                            tag.Attributes.Add(attribute, token.String);
                        }
                        // 同じ属性が複数回記述されている場合は、後に記述されたもので上書きする
                        else
                        {
                            tag.Attributes[attribute] = token.String;
                        }
                        attribute = null;
                        break;
                    default:
                        break;
                }
            }

            if (tags.Count > 0)
            {
                parseInfo.SetError("タグ(" + tags.Peek().TagName + ")に対応する閉じタグが入力されていません。");
                return null;
            }

            return root;
        }

        /// <summary>
        /// テキストを要素単位にトークン化します。
        /// </summary>
        /// <param name="text">トークナイズする文字列</param>
        static void Tokenize(string text)
        {
            TextTokenizer.Refresh(text);

            while (!TextTokenizer.IsEnd)
            {
                switch (TextTokenizer.Mode)
                {
                    case TokenizeMode.Text:
                        plainTextTokenizer.Tokenize();
                        break;
                    case TokenizeMode.CharacterReference:
                        characterReferenceTokenizer.Tokenize();
                        break;
                    case TokenizeMode.Tag:
                        beginTagTokenizer.Tokenize();
                        break;
                    case TokenizeMode.EndTag:
                        endTagTokenizer.Tokenize();
                        break;
                    case TokenizeMode.Attribute:
                        attributeKeyTokenizer.Tokenize();
                        break;
                    case TokenizeMode.AttributeValue:
                        attributeValueTokenizer.Tokenize();
                        break;
                    default:
                        break;
                }
            }
        }

#endregion
    }
}
