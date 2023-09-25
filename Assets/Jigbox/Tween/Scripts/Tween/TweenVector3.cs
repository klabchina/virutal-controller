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

namespace Jigbox.Tween
{
    [Serializable]
    public class TweenVector3 : TweenBase<Vector3>
    {
#region Constractor

        public TweenVector3()
        {
        }

        public TweenVector3(Action<ITween<Vector3>> onUpdate) : base(onUpdate)
        {
        }

        public TweenVector3(ITween<Vector3> other) : base(other)
        {
        }

        public TweenVector3(ITween<Vector3> other, Action<ITween<Vector3>> onUpdate) : base(other, onUpdate)
        {
        }

#endregion

#region Impliment Abstract Method

        public override Vector3 Change
        {
            get
            {
                return Final - Begin;
            }
            set
            {
                Final = Begin + value;
            }
        }

        public override Vector3 ValueAt(float time)
        {
            return Vector3.LerpUnclamped(Begin, Final, Progress(time));
        }

#endregion
    }
}
