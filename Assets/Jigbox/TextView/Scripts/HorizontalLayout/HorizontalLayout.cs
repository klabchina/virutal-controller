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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.TextView.HorizontalLayout
{
    /// <summary>
    /// 各種レイアウト用のデータを取り扱うクラス(TextViewで表示されるグリフは全てIHorizontalLayoutedElementとしてこのクラスに登録される)
    /// </summary>
    public class HorizontalLayout : IEnumerable
    {
        /// <summary>
        /// GetEnumerator()をした際にGC Allocが発生しないようにするための列挙子
        /// </summary>
        public class Enumerator : IEnumerator<IHorizontalLayoutedElement>
        {
            /// <summary>表示要素のリスト</summary>
            List<IHorizontalLayoutedElement> elements;

            /// <summary>参照の開始位置のインデックス</summary>
            int startIndex;

            /// <summary>参照の終了位置のインデックス</summary>
            int endIndex;

            /// <summary>現在参照している要素のインデックス</summary>
            int currentIndex;

            /// <summary>現在参照している要素</summary>
            IHorizontalLayoutedElement current;

            /// <summary>現在参照している要素</summary>
            public IHorizontalLayoutedElement Current { get { return current; } }

            /// <summary>現在参照している要素</summary>
            object IEnumerator.Current { get { return current; } }

            /// <summary>現在参照している要素</summary>
            IHorizontalLayoutedElement IEnumerator<IHorizontalLayoutedElement>.Current { get { return current; } }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="elements">表示要素のリスト</param>
            public Enumerator(List<IHorizontalLayoutedElement> elements)
            {
                this.elements = elements;
                startIndex = -1;
                endIndex = 0;
                currentIndex = -1;
                current = null;
            }

            /// <summary>
            /// 参照先を次の要素に進めます。
            /// </summary>
            /// <returns>次の要素が存在しない場合、<c>false</c>を返します。</returns>
            public bool MoveNext()
            {
                ++currentIndex;
                if (currentIndex >= endIndex)
                {
                    return false;
                }
                current = elements[currentIndex];
                return true;
            }

            /// <summary>
            /// 参照する範囲を設定します。
            /// </summary>
            /// <param name="startIndex">初期位置のインデックス</param>
            /// <param name="endIndex">終了位置のインデックス</param>
            public void SetRange(int startIndex, int endIndex)
            {
                this.startIndex = startIndex;
                this.endIndex = endIndex;
                currentIndex = startIndex - 1;
            }

            /// <summary>
            /// 参照を初期位置に戻します。
            /// </summary>
            public void Reset()
            {
                currentIndex = startIndex - 1;
            }

            /// <summary>
            /// アンマネージドリソースを解放します。
            /// </summary>
            void System.IDisposable.Dispose()
            {
            }
        }

        private List<IHorizontalLayoutedElement> allElements;

        Enumerator enumerator;

        int startIndex;
        int endIndex;

        /// <summary>
        /// 表示領域を超えてしまった時のライン数
        /// </summary>
        /// <value>The overflow line.</value>
        public int? OverflowLine { get; private set; }

        /// <summary>
        /// 表示領域を超えてしまった時のはみ出したテキスト情報
        /// </summary>
        /// <value>The overflow remain text.</value>
        public TextLine[] OverflowRemainText { get; private set; }

        /// <summary>
        /// 実際に描画させるElementの開始Index
        /// </summary>
        public int DisplayableElementStartIndex
        {
            get
            {
                return this.startIndex;
            }
            set
            {
                this.startIndex = Mathf.Max(0, value);
            }
        }

        /// <summary>
        /// 実際に描画させるElementの終了Index
        /// </summary>
        public int DisplayableElementEndIndex
        {
            get
            {
                return this.endIndex;
            }
            set
            {
                this.endIndex = Mathf.Max(0, value);
            }
        }

        public HorizontalLayout()
        {
            this.allElements = new List<IHorizontalLayoutedElement>();
            this.enumerator = new Enumerator(allElements);
            this.OverflowLine = null;
            this.OverflowRemainText = null;
            this.startIndex = 0;
            this.endIndex = 0;
        }

        /// <summary>
        /// レイアウトの高さを取得する
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get
            {
                if (this.allElements.Count == 0)
                {
                    return 0f;
                }

                var max = this.allElements[this.startIndex].yMaxOffsetSpecialAdjusted;
                var min = this.allElements[this.startIndex].yMinOffsetSpecialAdjusted;

                for (int i = this.startIndex + 1; i < this.endIndex; i++)
                {
                    max = Mathf.Min(max, this.allElements[i].yMaxOffsetSpecialAdjusted);
                    min = Mathf.Max(min, this.allElements[i].yMinOffsetSpecialAdjusted);
                }

                return Math.Abs(max - min);
            }
        }

        /// <summary>
        /// レイアウトの幅を取得する
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get
            {
                if (this.allElements.Count == 0)
                {
                    return 0f;
                }

                return Math.Abs(xMax - xMin);
            }
        }

        /// <summary>
        /// 登録されているグリフの配置するx軸の最大値.
        /// </summary>
        /// <value>The x max.</value>
        public float xMax
        {
            get
            {
                if (this.allElements.Count == 0)
                {
                    return 0f;
                }

                var max = this.allElements[this.startIndex].xMax;
                for (int i = this.startIndex + 1; i < this.endIndex; i++)
                {
                    max = Mathf.Max(max, this.allElements[i].xMax);
                }
                return max;
            }
        }

        /// <summary>
        /// 登録されているグリフの配置するx軸の最小値.
        /// </summary>
        /// <value>The x minimum.</value>
        public float xMin
        {
            get
            {
                if (this.allElements.Count == 0)
                {
                    return 0f;
                }

                var min = this.allElements[this.startIndex].xMin;
                for (int i = this.startIndex + 1; i < this.endIndex; i++)
                {
                    min = Mathf.Min(min, this.allElements[i].xMin);
                }
                return min;
            }
        }

        /// <summary>
        /// 登録されているグリフの配置するy軸の最大値.
        /// </summary>
        /// <value>The y max.</value>
        public float yMax
        {
            get
            {
                if (this.allElements.Count == 0)
                {
                    return 0f;
                }

                var max = this.allElements[this.startIndex].yMax;
                for (int i = this.startIndex + 1; i < this.endIndex; i++)
                {
                    max = Mathf.Min(max, this.allElements[i].yMax);
                }
                return max;
            }
        }

        /// <summary>
        /// 登録されているグリフの配置するy軸の最小値.
        /// </summary>
        /// <value>The y minimum.</value>
        public float yMin
        {
            get
            {
                if (this.allElements.Count == 0)
                {
                    return 0f;
                }

                var min = this.allElements[this.startIndex].yMin;
                for (int i = this.startIndex + 1; i < this.endIndex; i++)
                {
                    min = Mathf.Max(min, this.allElements[i].yMin);
                }
                return min;
            }
        }

        /// <summary>
        /// レイアウトに表示するグリフを追加する.
        /// </summary>
        /// <param name="element">IHorizontalLayoutedElement.</param>
        public void Add(IHorizontalLayoutedElement element)
        {
            this.allElements.Add(element);
        }

        /// <summary>
        /// 指定したIndexのElementを取得します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IHorizontalLayoutedElement GetElementAtIndex(int index)
        {
            if (index < 0 || index >= this.allElements.Count)
            {
                return null;
            }

            return this.allElements[index];
        }

        /// <summary>
        /// 登録されているグリフをクリアする.
        /// </summary>
        public void Clear()
        {
            this.allElements.Clear();
            this.OverflowLine = null;
            this.OverflowRemainText = null;
            this.startIndex = 0;
            this.endIndex = 0;
        }

        /// <summary>
        /// 表示領域から漏れたテキストを登録する.
        /// </summary>
        /// <param name="line">はみ出た時のライン.</param>
        /// <param name="remain">はみ出たテキスト.</param>
        public void SetOverflowText(int line, TextLine[] remain)
        {
            this.OverflowLine = line;
            this.OverflowRemainText = remain;
        }

        /// <summary>
        /// 表示領域から漏れたテキストの情報をクリアする
        /// </summary>
        public void ClearOverflowText()
        {
            this.OverflowLine = null;
            this.OverflowRemainText = null;
        }

        /// <summary>
        /// Glyph情報を更新します。
        /// </summary>
        /// <param name="font">Glyphで使用しているFont</param>
        public void UpdateGlyph(Font font)
        {
            var glyphCatalog = GlyphCatalog.GetCatalog(font);
            foreach (var element in allElements)
            {
                if (element is HorizontalLayoutedGlyph)
                {
                    HorizontalLayoutedGlyph layoutedGlyph = element as HorizontalLayoutedGlyph;
                    Glyph glyph = layoutedGlyph.GlyphPlacement.Glyph as Glyph;
                    layoutedGlyph.GlyphPlacement.Glyph = glyphCatalog.Get(glyph.GetSpec(font));
                }
            }
        }

#region IEnumerable

        public HorizontalLayout.Enumerator GetEnumerator()
        {
            enumerator.SetRange(startIndex, endIndex);
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            enumerator.SetRange(startIndex, endIndex);
            return enumerator;
        }

#endregion
    }
}
