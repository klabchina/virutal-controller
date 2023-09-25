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
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// キャレットの描画用情報
    /// </summary>
    public struct CaretDrawInfo : IEquatable<CaretDrawInfo>
    {
        /// <summary>キャレットの座標</summary>
        public readonly Vector2 position;

        /// <summary>キャレットの高さ</summary>
        public readonly float caretHeight;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position">キャレット表示座標</param>
        /// <param name="caretHeight">キャレットの高さ</param>
        public CaretDrawInfo(Vector2 position, float caretHeight)
        {
            this.position = position;
            this.caretHeight = caretHeight;
        }

        public override string ToString()
        {
            return string.Format("[CaretInfo] position=(x:{0},y:{1}), caretHeight={2}", position.x, position.y, caretHeight);
        }

#region IEquatable

        public bool Equals(CaretDrawInfo other)
        {
            return position.Equals(other.position) && caretHeight.Equals(other.caretHeight);
        }

        public override bool Equals(object obj)
        {
            var other = obj as CaretDrawInfo?;
            return other != null && Equals(other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (position.GetHashCode() * 397) ^ caretHeight.GetHashCode();
            }
        }

        public static bool operator ==(CaretDrawInfo left, CaretDrawInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CaretDrawInfo left, CaretDrawInfo right)
        {
            return !left.Equals(right);
        }

#endregion
    }
}

