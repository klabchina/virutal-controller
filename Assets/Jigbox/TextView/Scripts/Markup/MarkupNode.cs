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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jigbox.TextView.Markup
{
    public interface ITextContentNode
    {
        string TextContent { get; }
    }

    public class MarkupNode
    {
        /// <summary>子ノード</summary>
        protected List<MarkupNode> children;

        /// <summary>
        /// 子ノード
        /// </summary>
        /// <value>The children.</value>
        public List<MarkupNode> Children
        {
            get
            {
                if (children == null)
                {
                    children = new List<MarkupNode>();
                }
                return children;
            }
        }

        /// <summary>子ノードの数</summary>
        public int ChildrenCount { get { return children != null ? children.Count : 0; } }

        /// <summary>設定されている属性情報</summary>
        protected Dictionary<string, string> attributes;

        /// <summary>
        /// 設定されている属性情報
        /// </summary>
        /// <value>The attributes.</value>
        public Dictionary<string, string> Attributes 
        {
            get
            {
                if (attributes == null)
                {
                    attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return attributes;
            }
        }

        /// <summary>属性の数</summary>
        public int AttributesCount { get { return attributes != null ? attributes.Count : 0; } }

        /// <summary>
        /// 親ノード.
        /// </summary>
        /// <value>The parent.</value>
        public MarkupNode Parent { get; private set; }

        /// <summary>
        /// 指定ノードを子ノードとして加える.
        /// </summary>
        /// <param name="node">MarkupNode.</param>
        public void AppendChild(MarkupNode node)
        {
            this.Children.Add(node);
            node.Parent = this;
        }

        /// <summary>
        /// 属性の数を設定します。
        /// </summary>
        /// <param name="capacity">属性の数</param>
        public void SetAttributesCapacity(int capacity)
        {
            this.attributes = new Dictionary<string, string>(capacity, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 子要素の数を設定します。
        /// </summary>
        /// <param name="capacity">子要素の数</param>
        public void SetChildrenCapacity(int capacity)
        {
            this.children = new List<MarkupNode>(capacity);
        }
    }

    public class TextNode : MarkupNode, ITextContentNode
    {
#region ITextContentNode

        public string TextContent { get; set; }

#endregion

        public TextNode(string text)
        {
            this.TextContent = text;
        }
    }

    public class Element : MarkupNode, ITextContentNode
    {
#region ITextContentNode

        public string TextContent
        {
            get
            {
                var children = this.Children.OfType<ITextContentNode>().Select(x => x.TextContent).ToArray();
                return string.Join("", children);
            }
        }

#endregion

        public string TagName { get; set; }

        public Element(string tagName)
        {
            this.TagName = tagName;
        }
    }

}
