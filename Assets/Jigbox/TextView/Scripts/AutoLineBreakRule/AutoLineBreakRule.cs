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
using Jigbox.Components;

namespace Jigbox.TextView
{
    /// <summary>
    /// 自動改行ルールを取り扱うクラス
    /// </summary>
    public class AutoLineBreakRule
    {
        /// <summary>約物の中で半角として扱うIndexのセット</summary>
        static readonly HashSet<int> punctuationIndexes = new HashSet<int>();

        /// <summary>
        /// 自動改行ルールを適用します
        /// </summary>
        /// <param name="input">表示データ</param>
        /// <param name="width">画面幅</param>
        /// <param name="lineBreakRule">禁則処理ルール</param>
        /// <param name="useBurasage">ぶら下げを行うかどうか</param>
        /// <param name="ellipsisParameter">Overflow拡張コンポーネント</param>
        /// <param name="keepTailSpace">末尾のホワイトスペースを残しておくか</param>
        /// <returns>自動改行ルールに従って改行された表示データ</returns>
        public static List<TextLine> Apply(
            LineRenderingElements input,
            float width,
            LineBreakRule lineBreakRule,
            bool useBurasage,
            TextViewOverflowExtension.AutoLineEllipsisParameter? ellipsisParameter,
            bool keepTailSpace
        )
        {
            var result = new List<TextLine>();
            var pointer = new LineRenderingElementsPointer(input);
            var isTrailingLine = false;
            var totalSkippedCount = 0;
            var lineIndex = 0;
            punctuationIndexes.Clear();

            while (true)
            {
                // keepTailSpaceがtrueの場合は削除するWhitespaceがないのでこの処理は行わない
                if (isTrailingLine && !keepTailSpace)
                {
                    LineRenderingElementsPointer newPointer;
                    int skippedCount;
                    SkipWhitespace(pointer, out newPointer, out skippedCount);
                    pointer = newPointer;
                    totalSkippedCount += skippedCount;
                }

                var offsetWidth = 0.0f;
                if (ellipsisParameter != null && ellipsisParameter.Value.EllipsisLine == lineIndex)
                {
                    offsetWidth = ellipsisParameter.Value.OffsetWidth;
                }

                var punctuationCount = punctuationIndexes.Count;
                var builder = ApplyOneLine(pointer, width - offsetWidth, lineBreakRule, useBurasage, totalSkippedCount, keepTailSpace);
                // 新たな半角化する約物の対象が見つかったので初期化してゾーンの最初からTextLineを作る
                if (punctuationCount != punctuationIndexes.Count)
                {
                    var zone = pointer.Zone();
                    var indexes = new int[punctuationIndexes.Count];
                    punctuationIndexes.CopyTo(indexes);
                    LineRenderingElementsAssembly.ReAssemble(zone, indexes);
                    result.Clear();
                    pointer = new LineRenderingElementsPointer(input);
                    isTrailingLine = false;
                    totalSkippedCount = 0;
                    lineIndex = 0;
                   
                    continue;
                }

                result.Add(builder.Result);

                pointer = builder.To;

                if (pointer.IsEndOfElements())
                {
                    break;
                }

                isTrailingLine = true;
                lineIndex++;
            }

            return result;
        }

        /// <summary>
        /// 自動改行ルールを適用し、表示データ1行分を作成します
        /// </summary>
        /// <param name="pointer">開始位置</param>
        /// <param name="width">画面幅</param>
        /// <param name="lineBreakRule">禁則処理ルール</param>
        /// <param name="useBurasage">ぶら下げを行うかどうか</param>
        /// <param name="totalSkippedCount">スキップ済み文字数</param>
        /// <param name="keepTailSpace">末尾のスペースを維持するか</param>
        /// <returns>自動改行ルールに従って改行された表示データ1行分</returns>
        protected static TextLineBuilder ApplyOneLine(
            LineRenderingElementsPointer pointer,
            float width,
            LineBreakRule lineBreakRule,
            bool useBurasage,
            int totalSkippedCount,
            bool keepTailSpace)
        {
            var builder = new TextLineBuilder(pointer, totalSkippedCount, punctuationIndexes);
            var punctuationCount = punctuationIndexes.Count;

            // 分割禁止区域単位で文字を置いていく
            while (builder.Width <= width)
            {
                if (punctuationCount != punctuationIndexes.Count)
                {
                    return null;
                }
                if (builder.MoveNextSplitDenyZone() == false)
                {
                    return builder;
                }
            }
            
            if (punctuationCount != punctuationIndexes.Count)
            {
                return null;
            }

            // ぶら下げを行う設定の場合、可能か調べる
            if (useBurasage)
            {
                var lastZone = builder.To.PreviousSplitDenyZone().Zone();

                // 「最後の分割禁止区域単位に含まれる文字が1文字」かつ「最後の文字がぶら下げ可能文字」の場合、ぶら下げを行う
                //
                // NOTE:
                // 前者の条件は必須ではない。本当は「画面幅より1文字だけ溢れている場合」を確認した方がより多くの入力でぶら下げを行える。
                // ただし、その場合「画面幅から溢れた文字は何文字か」を確認する必要がある。
                // それを行うには、追加の処理が必要となり、パフォーマンスが悪化するため、
                //「最後の分割禁止区域単位に含まれる文字が1文字であれば画面幅より溢れているのはその文字だけ」という判定で代用している。
                if (lastZone.ElementsCount == 1 && lineBreakRule.CanBurasage(lastZone.ElementAt(0).IGlyph))
                {
                    return builder;
                }
            }

            // 分割禁止区域単位で戻っていき、自動改行ルールをすべて満たす改行位置を探す
            while (builder.MovePreviousSplitDenyZone())
            {
                // すべての自動改行ルールを満たす改行位置は存在しない
                if (builder.MainRenderingElementsCount == 0)
                {
                    break;
                }

                // 長さチェック
                if (builder.Width > width)
                {
                    continue;
                }

                // 行末禁則
                var lastElement = builder.LastElement;
                var lastGlyph = (lastElement == null) ? null : lastElement.IGlyph;
                if (lineBreakRule.IsNotAllowAtEndOfLine(lastGlyph))
                {
                    continue;
                }

                // 行頭禁則
                var nextLineBeginOfGlyph = builder.To.Element().IGlyph;
                if (lineBreakRule.IsNotAllowAtBeginOfLine(nextLineBeginOfGlyph))
                {
                    continue;
                }

                // 分離禁止文字
                if (lineBreakRule.CanWrap(lastGlyph, nextLineBeginOfGlyph) == false)
                {
                    continue;
                }
                
                // 収まっていない時に次の文字がスペースの間はさらに進める
                // InputFieldで使うTextViewでのみ行う
                if (keepTailSpace)
                {
                    LineRenderingElementsPointer keepPointer = builder.To;
                    int keepCount = 0;

                    while (keepPointer != null)
                    {
                        var fontElement = keepPointer.Element() as MainRenderingFontElement;

                        if (fontElement == null)
                        {
                            break;
                        }

                        if (fontElement.Glyph.IsWhiteSpaceOrControl == false)
                        {
                            break;
                        }

                        keepCount++;
                        builder.MoveNextMainRenderingElement();
                        keepPointer = builder.To;
                    }

                    builder.TailSpacingElementCount = keepCount;
                }

                // すべての自動改行ルールを満たす改行位置を発見
                return builder;
            }

            // すべての自動改行ルールを満たす改行位置は存在しなかったので、ルールを無視して画面幅に収まるだけの文字を置く
            builder = new TextLineBuilder(pointer, totalSkippedCount, punctuationIndexes);
            while (builder.Width <= width)
            {
                // Note:
                // 本来はbuilder.MoveNextMainRenderingElement() == falseになった際にもwhileループを抜ける処理をいれるべき
                // しかし、この関数の最初のwhileループによって条件が成立しないことが保証されているため、チェックはしない

                builder.MoveNextMainRenderingElement();
            }

            // 画面幅に1文字も収まらない場合でも、最低1文字は置く
            if (builder.MainRenderingElementsCount > 1)
            {
                builder.MovePreviousMainRenderingElement();
            }

            return builder;
        }

        /// <summary>
        /// 空白文字を読み飛ばし、読み飛ばし後の位置と読み飛ばした空白文字数を返します
        /// </summary>
        /// <param name="startPointer">開始位置</param>
        /// <param name="skippedPointer">読み飛ばし後の位置</param>
        /// <param name="skippedCount">読み飛ばした空白文字数</param>
        protected static void SkipWhitespace(
            LineRenderingElementsPointer startPointer,
            out LineRenderingElementsPointer skippedPointer,
            out int skippedCount
        )
        {
            skippedPointer = startPointer;
            skippedCount = 0;

            while (skippedPointer != null)
            {
                var fontElement = skippedPointer.Element() as MainRenderingFontElement;

                if (fontElement == null)
                {
                    break;
                }

                if (fontElement.Glyph.IsWhiteSpaceOrControl == false)
                {
                    break;
                }

                skippedCount++;
                skippedPointer = skippedPointer.NextMainRenderingElement();

                if (skippedPointer == null)
                {
                    break;
                }
            }
        }
    }
}
