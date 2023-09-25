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

namespace Jigbox.TextView
{
    /// <summary>
    /// LineRenderingElements 内のゾーンに内包されるエレメントの「間」を指すクラスです。
    /// このクラスはイミュータブル（不変）クラスです。
    /// 下図の “[…]” で囲んだものがゾーンで、“d” などの各要素がエレメントです。
    /// <code>
    /// [ a b c ] [ d e f ] [ g ]
    /// `-------'   ^
    ///   Zone      Element
    /// </code>
    /// このポインタークラスはエレメントの「間」を指します。
    /// 下図の “^” の箇所をこのクラスが指します。
    /// 末尾を指す (3, 0) のポインターがあることに注意してください。
    /// <code>
    ///               [ a b c ] [ d e f ] [ g ]
    ///                ^ ^ ^     ^ ^ ^     ^   ^
    /// zone index   : 0 0 0     1 1 1     2   3
    /// element index: 0 1 2     0 1 2     0   0
    /// </code>
    /// </summary>
    public class LineRenderingElementsPointer : IComparable<LineRenderingElementsPointer>
    {
#region fields & properties

        /// <summary>
        /// このポインターの対象。このクラスはこの対象内の位置を指す。
        /// </summary>
        public readonly LineRenderingElements Target;

        /// <summary>
        /// 指すゾーンの位置。
        /// </summary>
        protected int zoneIndex;

        public int ZoneIndex
        {
            get { return zoneIndex; }
        }

        /// <summary>
        /// 指すエレメント間の位置。
        /// </summary>
        protected int elementIndex;

        public int ElementIndex
        {
            get { return elementIndex; }
        }

#endregion

#region constructors

        public LineRenderingElementsPointer(LineRenderingElements target, int zoneIndex = 0, int elementIndex = 0)
        {
#if JIGBOX_DEBUG
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            var targetZonesCount = target.ZonesCount;
            if (zoneIndex < 0 || targetZonesCount < zoneIndex) // target.Zones.Count == zoneIndex は正常
            {
                throw new ArgumentOutOfRangeException("zoneIndex");
            }
            if (zoneIndex == targetZonesCount && elementIndex != 0)
            {
                throw new ArgumentException(
                    "if \"zoneIndex\" points the end of \"target\", \"elementIndex\" must be 0",
                    "elementIndex"
                );
            }
            if (zoneIndex != targetZonesCount && (elementIndex < 0 || target.ElementAt(zoneIndex).ElementsCount <= elementIndex))
            {
                throw new ArgumentOutOfRangeException("elementIndex");
            }
#endif
            this.Target = target;
            this.zoneIndex = zoneIndex;
            this.elementIndex = elementIndex;
        }

#endregion

#region override operators

        public static bool operator <(LineRenderingElementsPointer p0, LineRenderingElementsPointer p1)
        {
            return p0.CompareTo(p1) < 0;
        }

        public static bool operator >(LineRenderingElementsPointer p0, LineRenderingElementsPointer p1)
        {
            return p0.CompareTo(p1) > 0;
        }

#endregion

#region public methods

        public int CompareTo(LineRenderingElementsPointer other)
        {
            if (other == null)
            {
                // CompareTo の定義により、全てのインスタンスは null より大きい
                // https://msdn.microsoft.com/ja-jp/library/43hc6wht(v=vs.110).aspx
                return 1;
            }
            if (!this.Target.Equals(other.Target))
            {
                throw new ArgumentException("other must have the same \"Target\" as \"this\"", "other");
            }
            if (this.zoneIndex == other.zoneIndex)
            {
                return this.elementIndex - other.elementIndex;
            }
            return this.zoneIndex - other.zoneIndex;
        }

        public override bool Equals(object other)
        {
            var otherPointer = other as LineRenderingElementsPointer;
            if (otherPointer == null)
            {
                return false;
            }
            return this.Target.Equals(otherPointer.Target)
            && this.zoneIndex == otherPointer.zoneIndex
            && this.elementIndex == otherPointer.elementIndex;
        }

        public override int GetHashCode()
        {
            unchecked // オーバーフローのチェックをしない
            {
                const int prime0 = 67;
                const int prime1 = 101;
                int hash = prime0;
                hash = (hash * prime1) ^ this.Target.GetHashCode();
                hash = (hash * prime1) ^ zoneIndex;
                hash = (hash * prime1) ^ elementIndex;
                return hash;
            }
        }

#region Point Next or Previous

        /// <summary>
        /// 次のMainRenderingElementへ移動します
        /// </summary>
        /// <returns></returns>
        public LineRenderingElementsPointer NextMainRenderingElement()
        {
            if (IsEndOfElements())
            {
                // 現在全体の末尾を指している場合、次はないので null
                return null;
            }
            if (Target.ElementAt(zoneIndex).ElementsCount == elementIndex + 1)
            {
                // 現在のゾーン内の最後を指している場合、次のゾーンの先頭に移動
                return new LineRenderingElementsPointer(Target, zoneIndex + 1);
            }
            // ゾーン内での移動
            return new LineRenderingElementsPointer(Target, zoneIndex, elementIndex + 1);
        }

        /// <summary>
        /// 次のMainRenderingElementへ移動します
        /// </summary>
        /// <returns></returns>
        public LineRenderingElementsPointer PreviousMainRenderingElement()
        {
            if (zoneIndex == 0 && elementIndex == 0)
            {
                // 現在全体の先頭を指している場合、前はないので null
                return null;
            }
            if (elementIndex == 0)
            {
                // 現在のゾーン内の先頭を差している場合、前のゾーンの最後に移動
                return new LineRenderingElementsPointer(
                    Target,
                    zoneIndex - 1,
                    Target.ElementAt(zoneIndex - 1).ElementsCount - 1
                );
            }
            // ゾーン内での移動
            return new LineRenderingElementsPointer(Target, zoneIndex, elementIndex - 1);
        }

        /// <summary>
        /// 次のSplitDenyZoneへ移動します
        /// </summary>
        /// <returns></returns>
        public LineRenderingElementsPointer NextSplitDenyZone()
        {
            // 次に出力するべき本文を指す仕様であるため、「次に出力する本文がない」状態も示す必要があるため「=」が含まれているのは正しい
            if (zoneIndex >= Target.ZonesCount)
            {
                return null;
            }

            return new LineRenderingElementsPointer(Target, zoneIndex + 1);
        }

        /// <summary>
        /// 前のSplitDenyZoneへ移動します
        /// </summary>
        /// <returns></returns>
        public LineRenderingElementsPointer PreviousSplitDenyZone()
        {
            if (zoneIndex <= 0)
            {
                return null;
            }

            return new LineRenderingElementsPointer(Target, zoneIndex - 1);
        }

#endregion

#region Can Point Next or Previous

        /// <summary>
        /// 次のMainRenderingElementが存在しているかどうかを返します
        /// </summary>
        /// <returns></returns>
        public bool HasNextMainRenderingElement()
        {
            return !IsEndOfElements();
        }

        /// <summary>
        /// 前のMainRenderingElementが存在しているかどうかを返します
        /// </summary>
        /// <returns></returns>
        public bool HasPreviousMainRenderingElement()
        {
            return zoneIndex != 0 || elementIndex != 0;
        }

        /// <summary>
        /// 次のSplitDenyZoneが存在しているかどうかを返します
        /// </summary>
        /// <returns></returns>
        public bool HasNextSplitDenyZone()
        {
            return zoneIndex < Target.ZonesCount;
        }

        /// <summary>
        /// 前のSplitDenyZoneが存在しているかどうかを返します
        /// </summary>
        /// <returns></returns>
        public bool HasPreviousSplitDenyZone()
        {
            return zoneIndex > 0;
        }

#endregion

#region EOE

        /// <summary>
        /// 全体の末尾を示しているかどうかを返します
        /// </summary>
        /// <returns></returns>
        public bool IsEndOfElements()
        {
            return Target.ZonesCount == zoneIndex;
        }

#endregion

#region Indexer

        /// <summary>
        /// 指す位置のゾーン。
        /// </summary>
        public ISplitDenyZone Zone()
        {
            if (IsEndOfElements())
            {
                return null;
            }
            return Target.ElementAt(zoneIndex);
        }

        /// <summary>
        /// 指す位置の直後のエレメント。
        /// </summary>
        public MainRenderingElement Element()
        {
            if (IsEndOfElements())
            {
                return null;
            }
            return Target.ElementAt(zoneIndex).ElementAt(elementIndex);
        }

#endregion

#endregion
    }
}
