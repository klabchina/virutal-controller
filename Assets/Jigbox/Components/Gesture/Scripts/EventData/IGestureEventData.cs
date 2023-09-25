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

namespace Jigbox.Gesture
{
    /// <summary>
    /// ジェスチャーで処理されるデータのインタフェース
    /// </summary>
    public interface IGestureEventData
    {
        /// <summary>Camera</summary>
        Camera EventCamera { get; set; }

        /// <summary>入力イベントが発生したGameObjectのRectTransform</summary>
        RectTransform EventTarget { get; set; }
    }
}
