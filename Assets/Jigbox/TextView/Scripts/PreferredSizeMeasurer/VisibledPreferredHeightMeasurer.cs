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
    using PreferredSizeMeasurerProperty = PreferredSizeMeasurer.PreferredSizeMeasurerProperty;

    public class VisibledPreferredHeightMeasurer : PreferredHeightMeasurer
    {
#region properties

        /// <summary>値がキャッシュされているかどうか</summary>
        public override bool HasCache
        {
            get
            {
                return base.HasCache
                    && property.VisibleLength == lastVisibleLength
                    && property.VisibleLineStart == lastVisibleLineStartIndex;
            }
        }

        /// <summary>必要な幅を求める方法の種類</summary>
        public override PreferredHeightType Type { get { return PreferredHeightType.Visibled; } }

        /// <summary>最後に計算された際のVisibleLength</summary>
        protected int lastVisibleLength = 0;

        /// <summary>最後に計算された際のVisibleLineStart</summary>
        protected int lastVisibleLineStartIndex = 0;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property">TextViewの値を取得するためのプロパティ</param>
        public VisibledPreferredHeightMeasurer(PreferredSizeMeasurerProperty property) : base(property)
        {
            lastVisibleLength = property.VisibleLength;
            lastVisibleLineStartIndex = property.VisibleLineStart;
        }

        /// <summary>
        /// 高さを計算します。
        /// </summary>
        /// <param name="textLines">改行処理後の物理行としてのテキスト情報</param>
        public override void CalculateHeight(List<TextLine> textLines)
        {
            IsCacheInvalid = false;
            lastIsLineHeightFixed = property.IsLineHeightFixed;
            lastVisibleLength = property.VisibleLength;
            lastVisibleLineStartIndex = property.VisibleLineStart;

            if (textLines == null || textLines.Count == 0 || lastVisibleLineStartIndex >= textLines.Count)
            {
                Value = 0.0f;
                return;
            }

            int visibleLength = property.VisibleLength;

            int lineStartIndex = lastVisibleLineStartIndex;
            int lineEndIndex = textLines.Count - 1;

            if (visibleLength != Components.TextView.UnlimitedVisibleLength)
            {
                for (int i = lineStartIndex; i < textLines.Count; ++i)
                {
                    if (textLines[i].MaxIndex() + 1 >= property.VisibleLength)
                    {
                        lineEndIndex = i;
                        break;
                    }
                }
            }

            TextLine startLine = textLines[lineStartIndex];
            TextLine endLine = textLines[lineEndIndex];
            
            int fontSize = property.FontSize;

            if (this.property.AlignMode == TextAlignMode.Placement)
            {
                float startLineUpperHeight =
                    startLine.CalculateHeightUpperBaseLine(fontSize, lastIsLineHeightFixed, visibleLength);
                float endLineUpperHeight = lineStartIndex == lineEndIndex
                    ? startLineUpperHeight
                    : endLine.CalculateHeightUpperBaseLine(fontSize, lastIsLineHeightFixed, visibleLength);
                float endLineUnderHeight = endLine.CalculateHeightUnderBaseLine(property.Font, endLineUpperHeight,
                    lastIsLineHeightFixed, visibleLength);

                Value = startLineUpperHeight + (startLine.LineY - endLine.LineY) + endLineUnderHeight;
            }
            if (this.property.AlignMode == TextAlignMode.Font)
            {
                Value = 0.0f;
                
                float pointSize = this.property.PointSize;
                float fontScale = this.property.FontScale;
                float ascentLine = this.property.AscentLine;
                float descentLine = this.property.DescentLine;

                for(int i = lineStartIndex; i <= lineEndIndex; i++)
                {
                    Value += textLines[i].CalculateHeightFromFontInfo(
                        fontSize, 
                        pointSize, 
                        fontScale,
                        ascentLine,
                        descentLine,
                        lastIsLineHeightFixed);

                    if (i > lineStartIndex)
                    {
                        Value += this.property.LineGap;
                    }
                }
            }
        }

#endregion
    }
}
