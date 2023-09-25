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
    public class TweenVector2 : TweenBase<Vector2>
    {
#region Constructor

        public TweenVector2()
        {
        }

        public TweenVector2(Action<ITween<Vector2>> onUpdate) : base(onUpdate)
        {
        }

        public TweenVector2(ITween<Vector2> other) : base(other)
        {
        }

        public TweenVector2(ITween<Vector2> other, Action<ITween<Vector2>> onUpdate) : base(other, onUpdate)
        {
        }

#endregion

#region Implement Abstract Method

        public override Vector2 Change
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

        public override Vector2 ValueAt(float time)
        {
            return Vector2.LerpUnclamped(Begin, Final, Progress(time));
        }

#endregion
    }
}
