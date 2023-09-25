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

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public class ColorChangeFilter : GraphicComponentGroup
    {
#region properties
        
        /// <summary>子オブジェクトの色の変更を無効にするかどうか</summary>
        [SerializeField]
        protected bool invalidInChildren = false;

        /// <summary>色の変更が無効になっているかどうか</summary>
        public override bool IsInvalid { get { return true; } }

        /// <summary>子オブジェクトの色の変更が無効になっているかどうか</summary>
        public override bool IsInvalidChildren { get { return invalidInChildren; } }

        /// <summary>自身のコンポーネントのAlphaを操作不可にするかどうか</summary>
        public override bool IsInvalidControlAlpha { get { return true; } }

#endregion
    }
}
