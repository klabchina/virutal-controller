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
using UnityEngine.Assertions;
using System.Collections.Generic;

namespace Jigbox.TextView
{
    public class PreferredSizeMeasurer
    {
#region inner classes, enum, and structs

        public class PreferredSizeMeasurerProperty
        {
            /// <summary>TextView</summary>
            protected Components.TextView textView;

            /// <summary>フォント</summary>
            public virtual Font Font { get { return textView.Font; } }

            /// <summary>フォントサイズ</summary>
            public virtual int FontSize { get { return textView.FontSize; } }

            /// <summary>1行の高さを固定するかどうか</summary>
            public virtual bool IsLineHeightFixed { get { return textView.IsLineHeightFixed; } }

            /// <summary>表示開始行数</summary>
            public virtual int VisibleLineStart { get { return textView.VisibleLineStart; } }

            /// <summary>表示文字数</summary>
            public virtual int VisibleLength { get { return textView.VisibleLength; } }

            /// <summary>行間</summary>
            public virtual float LineGap { get { return textView.LineGap; } }
            
            /// <summary>表示モード</summary>
            public virtual TextAlignMode AlignMode { get { return textView.AlignMode; } }
            
            /// <summary>フォント情報から取得するPointSize</summary>
            public virtual float PointSize { get { return textView.FontFaceInfo.pointSize; } }
            
            /// <summary>フォント情報から取得するScale</summary>
            public virtual float FontScale { get { return textView.FontFaceInfo.scale; } }

            /// <summary>フォント情報から取得するAscentLine</summary>
            public virtual float AscentLine { get { return textView.FontFaceInfo.ascentLine; } }

            /// <summary>フォント情報から取得するDescentLine</summary>
            public virtual  float DescentLine { get { return textView.FontFaceInfo.descentLine; } }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="textView">TextView</param>
            public PreferredSizeMeasurerProperty(Components.TextView textView)
            {
                this.textView = textView;
            }
        }

#endregion

#region properties

        /// <summary>値がキャッシュされているかどうか</summary>
        public bool HasCache { get { return widthMeasurer.HasCache && heightMeasurer.HasCache; } }

        /// <summary>TextViewの値を取得するためのプロパティ</summary>
        protected PreferredSizeMeasurerProperty property;

        /// <summary>必要な幅の計測用モジュール</summary>
        protected PreferredWidthMeasurer widthMeasurer;

        /// <summary>必要な幅</summary>
        public float PreferredWidth { get { return widthMeasurer.Value; } }

        /// <summary>必要な高さの計測用モジュール</summary>
        protected PreferredHeightMeasurer heightMeasurer;

        /// <summary>必要な高さ</summary>
        public float PreferredHeight { get { return heightMeasurer.Value; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="textView">TextView</param>
        public PreferredSizeMeasurer(Components.TextView textView)
        {
            if (textView != null)
            {
                property = new PreferredSizeMeasurerProperty(textView);
                widthMeasurer = new PreferredWidthMeasurer(property);
                heightMeasurer = new PreferredHeightMeasurer(property);
            }
        }

        /// <summary>
        /// テキストが改行されずに表示できるようにするために必要な幅を計算します。
        /// </summary>
        /// <param name="renderingElements">配置情報まで含めた状態の論理行</param>
        /// <param name="type">必要な幅を求める方法</param>
        public void CalculatePreferredWidth(List<LineRenderingElements> renderingElements, PreferredWidthType type = PreferredWidthType.FirstLine)
        {
            if (widthMeasurer.Type != type)
            {
                widthMeasurer = CreateWidthMeasurer(type);
            }

            if (!widthMeasurer.HasCache)
            {
                widthMeasurer.CalculateWidth(renderingElements);
            }
        }

        /// <summary>
        /// テキストが縦方向にはみ出さないようにするために必要な高さを計算します。
        /// </summary>
        /// <param name="textLines">改行処理後の物理行としてのテキスト情報</param>
        /// <param name="type">必要な高さを求める方法</param>
        public void CalculatePreferredHeight(List<TextLine> textLines, PreferredHeightType type = PreferredHeightType.AllLine)
        {
            if (heightMeasurer.Type != type)
            {
                heightMeasurer = CreateHeightMeasurer(type);
            }

            if (!heightMeasurer.HasCache)
            {
                heightMeasurer.CalculateHeight(textLines);
            }
        }

        /// <summary>
        /// キャッシュを無効化します。
        /// </summary>
        public void InvalidateCache()
        {
            widthMeasurer.IsCacheInvalid = true;
            heightMeasurer.IsCacheInvalid = true;
        }

#endregion

#region protected methods

        /// <summary>
        /// 必要な幅の計測用モジュールを生成します。
        /// </summary>
        /// <param name="type">必要な幅を求める方法</param>
        /// <returns></returns>
        protected PreferredWidthMeasurer CreateWidthMeasurer(PreferredWidthType type)
        {
            switch (type)
            {
                case PreferredWidthType.FirstLine:
                    return new PreferredWidthMeasurer(property);
                case PreferredWidthType.AllLogicalLine:
                    return new LogicalPreferredWidthMeasurer(property);
                case PreferredWidthType.Visibled:
                    return new VisibledPreferredWidthMeasurer(property);
                default:
                    Assert.IsTrue(false, "If you support all PreferredWidthTypes, you will not reach here");
                    return widthMeasurer;
            }
        }

        /// <summary>
        /// 必要な高さの計測用モジュールを生成します。
        /// </summary>
        /// <param name="type">必要な高さを求める方法</param>
        /// <returns></returns>
        protected PreferredHeightMeasurer CreateHeightMeasurer(PreferredHeightType type)
        {
            switch (type)
            {
                case PreferredHeightType.AllLine:
                    return new PreferredHeightMeasurer(property);
                case PreferredHeightType.AllLogicalLine:
                    return new LogicalPreferredHeightMeasurer(property);
                case PreferredHeightType.Visibled:
                    return new VisibledPreferredHeightMeasurer(property);
                default:
                    Assert.IsTrue(false, "If you support all PreferredWidthTypes, you will not reach here");
                    return heightMeasurer;
            }
        }

#endregion
    }
}
