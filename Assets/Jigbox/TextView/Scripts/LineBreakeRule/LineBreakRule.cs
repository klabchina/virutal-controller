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

namespace Jigbox.TextView
{
    public abstract class LineBreakRule
    {
#region properties

        /// <summary>禁則処理ルール</summary>
        static Dictionary<TextLineBreakRule, LineBreakRule> rules = new Dictionary<TextLineBreakRule, LineBreakRule>();

#endregion

#region public methods

        /// <summary>
        /// 禁則処理ルールを取得します。
        /// </summary>
        /// <param name="lineBreakRule">禁則処理ルールの種類</param>
        /// <returns></returns>
        public static LineBreakRule GetLineBreakRule(TextLineBreakRule lineBreakRule)
        {
            LineBreakRule rule = null;

            if (rules.TryGetValue(lineBreakRule, out rule))
            {
                return rule;
            }

            switch (lineBreakRule)
            {
                case TextLineBreakRule.SimplifiedChinese:
                    rule = new LineBreakRuleSimplifiedChinese();
                    break;
                case TextLineBreakRule.TraditionalChinese:
                    rule = new LineBreakRuleTraditionalChinese();
                    break;
                case TextLineBreakRule.Korean:
                    rule = new LineBreakRuleKorean();
                    break;
                case TextLineBreakRule.Japanese:
                // falldown
                default:
                    rule = new LineBreakRuleJapanese();
                    break;
            }

            rules.Add(lineBreakRule, rule);

            return rule;
        }

        /// <summary>
        /// 指定グリフが行頭禁止文字かどうか.
        /// </summary>
        /// <returns><c>true</c> if this instance is not allow at begin of line the specified g; otherwise, <c>false</c>.</returns>
        /// <param name="g">Glyph.</param>
        public abstract bool IsNotAllowAtBeginOfLine(IGlyph g);

        /// <summary>
        /// 指定グリフが行末禁止文字かどうか.
        /// </summary>
        /// <returns><c>true</c> if this instance is not allow at end of line the specified g; otherwise, <c>false</c>.</returns>
        /// <param name="g">Glyph.</param>
        public abstract bool IsNotAllowAtEndOfLine(IGlyph g);

        /// <summary>
        /// グリフ間で改行が可能かどうか
        /// </summary>
        /// <param name="firstLineTailGlyph">1行目行末のグリフ</param>
        /// <param name="secondLineBeginGlyph">2行目行頭のグリフ</param>
        /// <returns></returns>
        public abstract bool CanWrap(IGlyph firstLineTailGlyph, IGlyph secondLineBeginGlyph);

        /// <summary>
        /// 指定グリフがぶら下げ可能文字かどうか
        /// </summary>
        /// <param name="g">グリフ</param>
        /// <returns>ぶら下げ可能な場合はtrue</returns>
        public virtual bool CanBurasage(IGlyph g)
        {
            return false;
        }

#endregion
    }
}
