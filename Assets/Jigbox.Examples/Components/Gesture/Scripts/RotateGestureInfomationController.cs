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
using Jigbox.Gesture;

namespace Jigbox.Examples
{
    public sealed class RotateGestureInfomationController : GestureExampleInfomationController
    {
#region properties

        [SerializeField]
        GameObject target = null;

#endregion

#region private methods

        void UpdateInfomation(RotateGestureEventData data)
        {
            infomation.text = string.Format(
                "primary position : {0}\n"
                + "secondary position : {1}\n"
                + "direction : {2}\n"
                + "angle : {3}\n"
                + "angle delta : {4}\n"
                + "euler : {5}\n"
                + "euler delta : {6}",
                data.PrimaryPosition,
                data.SecondaryPosition,
                data.Direction,
                data.Angle,
                data.AngleDelta,
                data.EulerAngle,
                data.EulerAngleDelta);
        }

        [AuthorizedAccess]
        void OnRotate(RotateGestureEventData data)
        {
            Vector3 eulerAngle = target.transform.localEulerAngles;
            eulerAngle.z += data.EulerAngleDelta;
            target.transform.localEulerAngles = eulerAngle;

            UpdateInfomation(data);
        }

#endregion
    }
}
