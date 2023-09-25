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
using UnityEngine.UI;
using Jigbox.Components;

namespace Jigbox.Examples
{
    public sealed class SliderHandleTransitionController : ButtonTransitionBase
    {
#region properties

        [SerializeField]
        Image handle = null;

#endregion

#region public methods

        public override void NoticeAutoUnlock()
        {
            SetColor(false);
        }
        
#endregion

#region protected methods
        
        protected override void OnPress()
        {
            SetColor(true);
        }

        protected override void OnRelease()
        {
            SetColor(false);
        }

        protected override void StopTransition()
        {
            SetColor(false);
        }

#endregion

#region private methods

        void SetColor(bool isSelected)
        {
            handle.color = isSelected ? Color.cyan : Color.white;
        }

#endregion
    }
}
