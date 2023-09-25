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
    /// 特定方向への移動によるジェスチャーの種類
    /// </summary>
    public enum SwipeGestureType
    {
        /// <summary>ポインタが押下後、画面左方に向かってへ移動して離された</summary>
        SwipeLeft,
        /// <summary>ポインタが押下後、画面右方に向かってへ移動して離された</summary>
        SwipeRight,
        /// <summary>ポインタが押下後、画面上方に向かってへ移動して離された</summary>
        SwipeUp,
        /// <summary>ポインタが押下後、画面下方に向かってへ移動して離された</summary>
        SwipeDown,
    }
}
