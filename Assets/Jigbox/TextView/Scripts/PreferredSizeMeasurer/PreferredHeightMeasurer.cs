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

    public class PreferredHeightMeasurer : PreferredBaseMeasurer
    {
#region properties

        /// <summary>値がキャッシュされているかどうか</summary>
        public override bool HasCache { get { return !IsCacheInvalid && property.IsLineHeightFixed == lastIsLineHeightFixed; } }

        /// <summary>必要な幅を求める方法の種類</summary>
        public virtual PreferredHeightType Type { get { return PreferredHeightType.AllLine; } }

        /// <summary>最後に計算された際のIsLineHeightFixed</summary>
        protected bool lastIsLineHeightFixed;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property">TextViewの値を取得するためのプロパティ</param>
        public PreferredHeightMeasurer(PreferredSizeMeasurerProperty property)
        {
            this.property = property;
            IsCacheInvalid = true;
            lastIsLineHeightFixed = property.IsLineHeightFixed;
        }

        /// <summary>
        /// 高さを計算します。
        /// </summary>
        /// <param name="textLines">改行処理後の物理行としてのテキスト情報</param>
        public virtual void CalculateHeight(List<TextLine> textLines)
        {
            IsCacheInvalid = false;
            lastIsLineHeightFixed = property.IsLineHeightFixed;

            if (textLines == null || textLines.Count == 0)
            {
                Value = 0.0f;
                return;
            }
            
            int fontSize = property.FontSize;

            if (this.property.AlignMode == TextAlignMode.Placement)
            {
                TextLine startLine = textLines[0];
                TextLine endLine = textLines[textLines.Count - 1];
                
                float startLineUpperHeight = startLine.CalculateHeightUpperBaseLine(fontSize, lastIsLineHeightFixed);
                float endLineUpperHeight = textLines.Count == 1
                    ? startLineUpperHeight
                    : endLine.CalculateHeightUpperBaseLine(fontSize, lastIsLineHeightFixed);
                float endLineUnderHeight =
                    endLine.CalculateHeightUnderBaseLine(property.Font, endLineUpperHeight, lastIsLineHeightFixed);

                Value = startLineUpperHeight + (startLine.LineY - endLine.LineY) + endLineUnderHeight;
            }

            if (this.property.AlignMode == TextAlignMode.Font)
            {
                Value = 0.0f;
                
                float pointSize = this.property.PointSize;
                float fontScale = this.property.FontScale;
                float ascentLine = this.property.AscentLine;
                float descentLine = this.property.DescentLine;
                
                for(int i = 0; i < textLines.Count;i++)
                {
                    Value += textLines[i].CalculateHeightFromFontInfo(
                        fontSize, 
                        pointSize, 
                        fontScale,
                        ascentLine,
                        descentLine,
                        lastIsLineHeightFixed);
                    
                    if (i != 0)
                    {
                        Value += this.property.LineGap;
                    }
                }
            }
        }

#endregion
    }
}
