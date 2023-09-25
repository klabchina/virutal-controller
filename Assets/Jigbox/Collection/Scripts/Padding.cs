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
using System;

namespace Jigbox
{
    
    /// <summary>
    /// Paddingとなっているが余白について表現する構造体
    /// </summary>
    [Serializable]
    public struct Padding : IEquatable<Padding>
    {
        public static Padding zero = new Padding(0);

        [SerializeField]
        int left;

        public int Left { get { return left; } set { left = value; } }

        [SerializeField]
        int right;

        public int Right { get { return right; } set { right = value; } }

        [SerializeField]
        int top;

        public int Top { get { return top; } set { top = value; } }

        [SerializeField]
        int bottom;

        public int Bottom { get { return bottom; } set { bottom = value; } }

        public Padding(int padding)
        {
            left = padding;
            right = padding;
            top = padding;
            bottom = padding;
        }

        public Padding(int x, int y)
        {
            left = x;
            right = x;
            top = y;
            bottom = y;
        }

        public Padding(int left, int right, int top, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is Padding)
            {
                return Equals((Padding) obj);
            }
            return false;
        }

        public bool Equals(Padding other)
        {
            return (left == other.left) && (right == other.right) && (top == other.top) && (bottom == other.bottom);
        }

        public override int GetHashCode()
        {
            return Left ^ Right ^ Top ^ Bottom;
        }

        public static bool operator ==(Padding a, Padding b)
        {
            return a.left == b.left && a.right == b.right && a.top == b.top && a.bottom == b.bottom;
        }

        public static bool operator !=(Padding a, Padding b)
        {
            return !(a == b);
        }
    }
}
