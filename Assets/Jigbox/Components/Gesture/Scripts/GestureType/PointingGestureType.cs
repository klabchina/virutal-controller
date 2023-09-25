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

namespace Jigbox.Gesture
{
    /// <summary>
    /// 特定位置への入力によるジェスチャーの種類
    /// </summary>
    public enum PointingGestureType
    {
        /// <summary>ポインタが押下された</summary>
        Press = 0,
        /// <summary>押下されたポインタが離された</summary>
        Release,
        /// <summary>ポインタ押下後、動かさずにそのまま一定時間押し続けた</summary>
        LongPress,
        /// <summary>ポインタ押下後、動かさずに離された</summary>
        Click,
        /// <summary>短時間に2回連続してクリックを行った</summary>
        DoubleClick,
    }
}
