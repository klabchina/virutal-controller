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
    public sealed class PinchGestureInfomationController : GestureExampleInfomationController
    {
#region properties

        [SerializeField]
        GameObject target = null;

#endregion

#region private methods

        void UpdateInfomation(PinchGestureEventData data)
        {
            infomation.text = string.Format(
                "primary position : {0}\n"
                + "secondary position : {1}\n"
                + "distance : {2}\n"
                + "distance delta : {3}\n",
                data.PrimaryPosition,
                data.SecondaryPosition,
                data.Distance,
                data.DistanceDelta);
        }

        [AuthorizedAccess]
        void OnPinch(PinchGestureEventData data)
        {
            Vector3 scale = target.transform.localScale;
            // スカラー値で100距離が開くと+1になる想定の計算
            float scaleDelta = data.DistanceSqrMagnitudeDelta * 0.0001f;
            scale.x = Mathf.Clamp(scale.x + scaleDelta, 0.5f, 2.0f);
            scale.y = scale.x;
            target.transform.localScale = scale;

            UpdateInfomation(data);
        }

#endregion
    }
}
