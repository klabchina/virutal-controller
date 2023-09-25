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

using Jigbox.Gesture;

namespace Jigbox.Examples
{
    public sealed class DragGestureInfomationController : GestureExampleInfomationController
    {
#region private methods

        void UpdateInfomation(DragGestureEventData data)
        {
            infomation.text = string.Format(
                "begin : {0}\n"
                + "position : {1}\n"
                + "movement : {2}\n"
                + "movement form begin : {3}\n"
                + "angle : {4}\n"
                + "euler : {5}",
                data.BeginPosition,
                data.Position,
                data.Movement,
                data.MovementFromBegin,
                data.Angle,
                data.EulerAngle);
        }

        [AuthorizedAccess]
        void OnDrag(DragGestureEventData data)
        {
            UpdateInfomation(data);
        }

#endregion
    }
}
