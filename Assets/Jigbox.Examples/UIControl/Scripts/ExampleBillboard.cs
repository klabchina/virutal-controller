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
using System.Collections;

namespace Jigbox.Examples
{
    public class ExampleBillboard : MonoBehaviour
    {
#region properties

        /// <summary>向きを設定する対象</summary>
        [SerializeField]
        GameObject target;

        /// <summary>x軸の回転を固定するか</summary>
        [SerializeField]
        bool freezeRotateX = false;

        /// <summary>y軸の回転を固定するか</summary>
        [SerializeField]
        bool freezeRotateY = false;

        /// <summary>向きを反転させるかどうか</summary>
        [SerializeField]
        bool isReverse = false;

        /// <summary>位置のオフセット</summary>
        [SerializeField]
        Vector3 offset = Vector3.zero;

#endregion

#region public methods

        public void SetOffset(Vector3 offset)
        {
            this.offset = offset;
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

#endregion

#region override unity methods

        void Update()
        {
            if (target != null)
            {
                Vector3 targetPosition = target.transform.position;
                if (freezeRotateX)
                {
                    targetPosition.y = transform.position.y;
                }
                if (freezeRotateY)
                {
                    targetPosition.x = transform.position.x;
                }
                Vector3 temp = transform.localPosition;
                transform.localPosition = temp + offset;
                if (isReverse)
                {
                    targetPosition = transform.position + (transform.position - targetPosition);
                }
                transform.LookAt(targetPosition);
                transform.localPosition = temp;
            }
        }

#endregion
    }
}
