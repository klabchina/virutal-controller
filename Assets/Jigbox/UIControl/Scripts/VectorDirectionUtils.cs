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

namespace Jigbox.UIControl
{
    /// <summary>
    /// ベクトルの方向計算ユーティリティクラス
    /// </summary>
    public static class VectorDirectionUtils
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 向き
        /// </summary>
        public enum Direction
        {
            /// <summary>左</summary>
            Left,
            /// <summary>右</summary>
            Right,
            /// <summary>上</summary>
            Up,
            /// <summary>下</summary>
            Down,
        }

#endregion

#region constants

        /// <summary>左向きのベクトル</summary>
        static readonly Vector2 LeftDirection = Vector2.left;

        /// <summary>上向きのベクトル</summary>
        static readonly Vector2 UpDirection = Vector2.up;

#endregion

#region public methods

        /// <summary>
        /// ベクトルの表す方向を取得して返します。
        /// </summary>
        /// <param name="vector">ベクトル</param>
        /// <returns></returns>
        public static Direction GetDirection(Vector2 vector)
        {
            vector.Normalize();
            
            if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
            {
                float dot = Vector2.Dot(LeftDirection, vector);
                return dot >= 0.0f ? Direction.Left : Direction.Right;
            }
            else
            {
                float dot = Vector2.Dot(UpDirection, vector);
                return dot >= 0.0f ? Direction.Up : Direction.Down;
            }
        }

#endregion
    }
}
