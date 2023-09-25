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
    public sealed class FlickGestureInfomationController : GestureExampleInfomationController
    {
        
#region private methods

        void UpdateInfomation(FlickGestureEventData data)
        {
            infomation.text = string.Format(
                "begin : {0}\n"
                + "position : {1}\n"
                + "smoothPosition : {2}\n"
                + "acceleration : {3}\n"
                + "angle : {4}\n"
                + "euler : {5}",
                data.BeginPosition,
                data.Position,
                data.SmoothPosition,
                data.Acceleration,
                data.Angle,
                data.EulerAngle);

        }

        [AuthorizedAccess]
        void OnFlickLeft(FlickGestureEventData data)
        {
            CreateLabel("Flick Left", Color.white, labelParent, data.Position);
            UpdateInfomation(data);
        }

        [AuthorizedAccess]
        void OnFlickRight(FlickGestureEventData data)
        {
            CreateLabel("Flick Right", Color.white, labelParent, data.Position);
            UpdateInfomation(data);
        }

        [AuthorizedAccess]
        void OnFlickUp(FlickGestureEventData data)
        {
            CreateLabel("Flick Up", Color.white, labelParent, data.Position);
            UpdateInfomation(data);
        }

        [AuthorizedAccess]
        void OnFlickDown(FlickGestureEventData data)
        {
            CreateLabel("Flick Down", Color.white, labelParent, data.Position);
            UpdateInfomation(data);
        }

#endregion
    }
}
