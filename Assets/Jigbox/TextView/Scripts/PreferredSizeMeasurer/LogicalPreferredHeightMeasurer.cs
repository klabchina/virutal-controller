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
using System.Collections.Generic;

namespace Jigbox.TextView
{
    using PreferredSizeMeasurerProperty = PreferredSizeMeasurer.PreferredSizeMeasurerProperty;

    public class LogicalPreferredHeightMeasurer : PreferredHeightMeasurer
    {
#region properties

        /// <summary>必要な幅を求める方法の種類</summary>
        public override PreferredHeightType Type { get { return PreferredHeightType.AllLogicalLine; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property">TextViewの値を取得するためのプロパティ</param>
        public LogicalPreferredHeightMeasurer(PreferredSizeMeasurerProperty property) : base(property)
        {
        }

        /// <summary>
        /// 高さを計算します。
        /// </summary>
        /// <param name="textLines">改行処理後の物理行としてのテキスト情報</param>
        public override void CalculateHeight(List<TextLine> textLines)
        {
            IsCacheInvalid = false;
            lastIsLineHeightFixed = property.IsLineHeightFixed;

            if (textLines == null || textLines.Count == 0)
            {
                Value = 0.0f;
                return;
            }

            float lineGap = property.LineGap;
            float baseLinePosition = 0.0f;
            float? startLineUpperHeight = null;
            float endLineUnderHeight = 0.0f;
            float? startLineBase = null;
            float endLineBase = 0.0f;

            if (this.property.AlignMode == TextAlignMode.Placement)
            {
                // ロジック自体はCalculateLinesOffsetYと同等
                for (int i = 0; i < textLines.Count;)
                {
                    float upperHeight;
                    float underHeight;
                    i += CalculateLogicalLineHeight(textLines, i, out upperHeight, out underHeight);
                    baseLinePosition -= upperHeight;

                    if (startLineUpperHeight == null)
                    {
                        startLineUpperHeight = upperHeight;
                    }

                    if (startLineBase == null)
                    {
                        startLineBase = baseLinePosition;
                    }

                    endLineUnderHeight = underHeight;
                    endLineBase = baseLinePosition;

                    baseLinePosition -= underHeight + lineGap;
                }

                Value = startLineUpperHeight.Value + (startLineBase.Value - endLineBase) + endLineUnderHeight;
            }
            
            if (this.property.AlignMode == TextAlignMode.Font)
            {
                Value = 0.0f;
                int fontSize = this.property.FontSize;
                
                float pointSize = this.property.PointSize;
                float fontScale = this.property.FontScale;
                float ascentLine = this.property.AscentLine;
                float descentLine = this.property.DescentLine;
                float prevLineHeight = 0.0f;
                float currentLineHeight = 0.0f;
                
                for(int i = 0; i < textLines.Count;i++)
                {
                    currentLineHeight = textLines[i].CalculateHeightFromFontInfo(
                        fontSize, 
                        pointSize, 
                        fontScale,
                        ascentLine,
                        descentLine,
                        lastIsLineHeightFixed);

                    // 自動改行が入った場合、一行の高さがルビなどで異なる場合があるため改行前の一行の高さを算出する
                    if (textLines[i].IsAutoLineBreak)
                    {
                        if (currentLineHeight > prevLineHeight)
                        {
                            Value += currentLineHeight - prevLineHeight;
                            prevLineHeight = currentLineHeight;
                        }
                        continue;
                    }

                    Value += currentLineHeight;
                    prevLineHeight = currentLineHeight;

                    if (i != 0)
                    {
                        Value += this.property.LineGap;
                    }
                }
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// 物理行から論理行で1行分に相当する分の高さを計算します。
        /// </summary>
        /// <param name="textLines">改行処理後の物理行としてのテキスト情報</param>
        /// <param name="index">開始インデックス</param>
        /// <param name="upperHeight">ベースラインより上の幅</param>
        /// <param name="underHeight">ベースラインより下の幅</param>
        /// <returns>計算した行数を返します。</returns>
        protected int CalculateLogicalLineHeight(List<TextLine> textLines, int index, out float upperHeight, out float underHeight)
        {
            // 自動改行によって分割された行が何行連続しているか
            int lineCount = 1;
            for (int i = index + 1; i < textLines.Count; ++i)
            {
                if (!textLines[i].IsAutoLineBreak)
                {
                    break;
                }
                else
                {
                    ++lineCount;
                }
            }

            int fontSize = property.FontSize;
            Font font = property.Font;

            upperHeight = 0.0f;

            for (int i = 0; i < lineCount; ++i)
            {
                float lineUpperHeight = textLines[index + i].CalculateHeightUpperBaseLine(fontSize, lastIsLineHeightFixed);
                upperHeight = Mathf.Max(upperHeight, lineUpperHeight);
            }

            underHeight = 0.0f;

            for (int i = 0; i < lineCount; ++i)
            {

                float lineUnderHeight = textLines[index + i].CalculateHeightUnderBaseLine(font, upperHeight, lastIsLineHeightFixed);
                underHeight = Mathf.Max(underHeight, lineUnderHeight);
            }

            return lineCount;
        }

#endregion
    }
}
