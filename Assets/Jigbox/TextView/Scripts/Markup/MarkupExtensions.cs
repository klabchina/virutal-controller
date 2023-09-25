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
using System.Collections;

namespace Jigbox.TextView.Markup
{
    public static class MarkupExtensions
    {
        /// <summary>
        /// MarkupNodeでパースされているAttributesから指定のAttribute名の値を取得して返す.
        /// 格納されていない場合はディフォルトの値を返す.
        /// </summary>
        /// <returns>指定された属性名に格納されている値.</returns>
        /// <param name="node">検索するMarkupNode.</param>
        /// <param name="attr">属性名.</param>
        /// <param name="defaultValue">格納されていなかった場合のディフォルトの値.</param>
        public static string GetAttribute(this MarkupNode node, string attr, string defaultValue = "")
        {
            if (node.AttributesCount == 0)
            {
                return defaultValue;
            }
            return node.Attributes.ContainsKey(attr) ? node.Attributes[attr] : defaultValue;
        }
    }
}