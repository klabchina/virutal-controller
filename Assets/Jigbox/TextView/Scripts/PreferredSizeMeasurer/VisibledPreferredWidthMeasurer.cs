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

    public class VisibledPreferredWidthMeasurer : PreferredWidthMeasurer
    {
#region constanst

        /// <summary>デフォルトの位置情報</summary>
        protected static readonly MainRenderingElementPosition ElementPosition = MainRenderingElementPosition.LineHeadTail;

#endregion

#region properties

        /// <summary>値がキャッシュされているかどうか</summary>
        public override bool HasCache { get { return !IsCacheInvalid && property.VisibleLength == lastVisibleLength; } }

        /// <summary>必要な幅を求める方法</summary>
        public override PreferredWidthType Type { get { return PreferredWidthType.Visibled; } }

        /// <summary>最後に計算された際のVisibleLength</summary>
        protected int lastVisibleLength = 0;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="property">TextViewの値を取得するためのプロパティ</param>
        public VisibledPreferredWidthMeasurer(PreferredSizeMeasurerProperty property) : base(property)
        {
            lastVisibleLength = property.VisibleLength;
        }

        /// <summary>
        /// 横幅を計算します。
        /// </summary>
        /// <param name="renderingElements">配置情報まで含めた状態の論理行</param>
        public override void CalculateWidth(List<LineRenderingElements> renderingElements)
        {
            IsCacheInvalid = false;
            lastVisibleLength = property.VisibleLength;

            if (renderingElements == null || renderingElements.Count == 0)
            {
                Value = 0.0f;
                return;
            }

            float widthMax = 0.0f;

            for (int i = 0; i < renderingElements.Count; ++i)
            {
                float width = GetWidth(renderingElements[i]);
                if (width > widthMax)
                {
                    widthMax = width;
                }
            }

            Value = widthMax;
        }

#endregion

#region protected methods

        /// <summary>
        /// 表示する文字数分の幅を取得します。
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected float GetWidth(LineRenderingElements element)
        {
            if (property.VisibleLength == Components.TextView.UnlimitedVisibleLength)
            {
                return element.Width;
            }

            int maxIndex = property.VisibleLength - 1;

            // 最終文字のGlyphIndexを求める
            int lastElementIndex = -1;
            foreach (ISplitDenyZone zone in element.Zones)
            {
                foreach (MainRenderingElement main in zone.Elements)
                {
                    if (main.GlyphIndex <= maxIndex && main.GlyphIndex > lastElementIndex)
                    {
                        lastElementIndex = main.GlyphIndex;
                    }
                }
            }

            // 最終文字がない場合、表示する文字が1文字もない
            if (lastElementIndex == -1)
            {
                return 0.0f;
            }

            float width = 0.0f;
            for (int i = 0; i < element.ZonesCount; i++)
            {
                ISplitDenyZone zone = element.ElementAt(i);
                MainRenderingElementPosition zonePosition = ElementPosition.PositionAtIndex(i, element.ZonesCount);

                for (int j = 0; j < zone.ElementsCount; j++)
                {
                    MainRenderingElement main = zone.ElementAt(j);
                    MainRenderingElementPosition mainPosition = zonePosition.PositionAtIndex(j, zone.ElementsCount);

                    if (main.GlyphIndex > maxIndex)
                    {
                        continue;
                    }

                    // 表示上の最終文字を入力の最終文字と同じ扱いにする
                    if (main.GlyphIndex == lastElementIndex)
                    {
                        // もともと行頭文字だった場合は、行頭かつ行末の扱いにする
                        if (mainPosition.IsLineHead())
                        {
                            mainPosition = MainRenderingElementPosition.LineHeadTail;
                        }
                        else
                        {
                            mainPosition = MainRenderingElementPosition.LineTail;
                        }
                    }

                    width += main.Width(mainPosition);
                }
            }

            return width;
        }

#endregion
    }
}
