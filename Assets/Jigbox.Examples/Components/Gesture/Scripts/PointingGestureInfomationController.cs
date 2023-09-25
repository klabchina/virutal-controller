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
    public sealed class PointingGestureInfomationController : GestureExampleInfomationController
    {
#region private methods

        void UpdateInfomation(PointingGestureEventData data)
        {
            infomation.text = string.Format("position : {0}", data.Position);
        }

        [AuthorizedAccess]
        void OnPress(PointingGestureEventData data)
        {
            UpdateInfomation(data);
        }

        [AuthorizedAccess]
        void OnClick(PointingGestureEventData data)
        {
            CreateLabel("Click", Color.white, labelParent, data.Position);
            UpdateInfomation(data);
        }

        [AuthorizedAccess]
        void OnDoubleClick(PointingGestureEventData data)
        {
            CreateLabel("Double Click", Color.red, labelParent, data.Position);
            UpdateInfomation(data);
        }

        [AuthorizedAccess]
        void OnLongPress(PointingGestureEventData data)
        {
            CreateLabel("LongPress", Color.yellow, labelParent, data.Position);
            UpdateInfomation(data);
        }

#endregion
    }
}
