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
    /// ポインタの移動によるジェスチャーの種類
    /// </summary>
    public enum DragGestureType
    {
        /// <summary>ポインタが押下後、移動した</summary>
        Drag = 0,
        /// <summary>ポインタが押下後、画面左方に向かってへ移動した</summary>
        DragLeft,
        /// <summary>ポインタが押下後、画面右方に向かってへ移動した</summary>
        DragRight,
        /// <summary>ポインタが押下後、画面上方に向かってへ移動した</summary>
        DragUp,
        /// <summary>ポインタが押下後、画面下方に向かってへ移動した</summary>
        DragDown,
        /// <summary>ポインタ押下後、移動が始まった</summary>
        BeginDrag,
        /// <summary>ポインタが離され、移動が終わった</summary>
        EndDrag,
    }
}
