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
using Jigbox.TextView.Markup;

namespace Jigbox.TextView.ParserV2
{
    public abstract class Token
    {
        /// <summary>トークンの種類</summary>
        public abstract TokenType Type { get; }

        /// <summary>トークン化された文字列</summary>
        public string String { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="str">トークン化する文字列</param>
        public Token(string str)
        {
            String = str;
        }

        /// <summary>
        /// MarkupNodeを生成します。
        /// </summary>
        /// <returns></returns>
        public virtual MarkupNode CreateMarkupNode()
        {
            UnityEngine.Assertions.Assert.IsTrue(false, "It is not necessary to use other than TextToken and TagToken!");
            return null;
        }
    }

    public sealed class TextToken : Token
    {
        /// <summary>トークンの種類</summary>
        public override TokenType Type { get { return TokenType.Text; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="text">トークン化する文字列</param>
        public TextToken(string text) : base(text)
        {
        }

        /// <summary>
        /// MarkupNode(TextNode)を生成します。
        /// </summary>
        /// <returns></returns>
        public override MarkupNode CreateMarkupNode()
        {
            return new TextNode(String);
        }
    }

    public sealed class BeginTagToken : Token
    {
        /// <summary>トークンの種類</summary>
        public override TokenType Type { get { return TokenType.Tag; } }

        /// <summary>子となる要素の数</summary>
        public int ChildrenCount { get; set; }

        /// <summary>属性の数</summary>
        public int AttributeCount { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tag">トークン化する文字列</param>
        public BeginTagToken(string tag) : base(tag)
        {
            ChildrenCount = 0;
            AttributeCount = 0;
        }

        /// <summary>
        /// MarkupNode(Element)を生成します。
        /// </summary>
        /// <returns></returns>
        public override MarkupNode CreateMarkupNode()
        {
            Element element = new Element(String);
            if (ChildrenCount > 0)
            {
                element.SetChildrenCapacity(ChildrenCount);
            }
            if (AttributeCount > 0)
            {
                element.SetAttributesCapacity(AttributeCount);
            }
            return element;
        }
    }

    public sealed class EndTagToken : Token
    {
        /// <summary>トークンの種類</summary>
        public override TokenType Type { get { return TokenType.EndTag; } }

        /// <summary>空要素として許容するか</summary>
        public bool AllowEmpty { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tag">トークン化する文字列</param>
        /// <param name="allowEmpty">空要素として許容するか</param>
        public EndTagToken(string tag, bool allowEmpty = false) : base(tag)
        {
            AllowEmpty = allowEmpty;
        }
    }

    public sealed class AttributeKeyToken : Token
    {
        /// <summary>トークンの種類</summary>
        public override TokenType Type { get { return TokenType.Attribute; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="attribute">トークン化する文字列</param>
        public AttributeKeyToken(string attribute) : base(attribute)
        {
        }
    }

    public sealed class AttributeValueToken : Token
    {
        /// <summary>トークンの種類</summary>
        public override TokenType Type { get { return TokenType.AttributeValue; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">トークン化する文字列</param>
        public AttributeValueToken(string value) : base(value)
        {
        }
    }
}
