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

using Jigbox.Shake;
using UnityEngine;

namespace Jigbox.Components
{
    public class WaveShakePosition : MonoBehaviour
    {
#region properties

        [HideInInspector]
        [SerializeField]
        bool playOnStart = false;

        [HideInInspector]
        [SerializeField]
        bool xAxisEnabled = false;

        /// <summary>x軸方向に振動させるかどうか</summary>
        public bool XAxisEnabled { get { return xAxisEnabled; } set { xAxisEnabled = value; } }

        [HideInInspector]
        [SerializeField]
        WaveShake xShake = new WaveShake();

        /// <summary>x軸方向のShake</summary>
        public WaveShake XShake { get { return xShake; } }

        [HideInInspector]
        [SerializeField]
        bool yAxisEnabled = false;

        /// <summary>y軸方向に振動させるかどうか</summary>
        public bool YAxisEnabled { get { return yAxisEnabled; } set { yAxisEnabled = value; } }

        [HideInInspector]
        [SerializeField]
        WaveShake yShake = new WaveShake();

        /// <summary>y軸方向のShake</summary>
        public WaveShake YShake { get { return yShake; } }

        [HideInInspector]
        [SerializeField]
        bool zAxisEnabled = false;

        /// <summary>z軸方向に振動させるかどうか</summary>
        public bool ZAxisEnabled { get { return zAxisEnabled; } set { zAxisEnabled = value; } }

        [HideInInspector]
        [SerializeField]
        WaveShake zShake = new WaveShake();

        /// <summary>z軸方向のShake</summary>
        public WaveShake ZShake { get { return zShake; } }

        [HideInInspector]
        [SerializeField]
        Vector3 origin = Vector3.zero;

        /// <summary>振動の原点</summary>
        public Vector3 Origin { get { return origin; } set { origin = value; } }

#if UNITY_EDITOR

#pragma warning disable 414
        /// <summary>Inpsector上での初期化が行われているかどうか</summary>
        [HideInInspector]
        [SerializeField]
        bool hasBeenInitialized = false;
#pragma warning restore 414

#endif

#endregion

#region Unity methods

        void Start()
        {
            xShake.OnUpdate(_ =>
            {
                transform.localPosition = new Vector3(origin.x + xShake.Value, transform.localPosition.y, transform.localPosition.z);
            });
            yShake.OnUpdate(_ =>
            {
                transform.localPosition = new Vector3(transform.localPosition.x, origin.y + yShake.Value, transform.localPosition.z);
            });
            zShake.OnUpdate(_ =>
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, origin.z + zShake.Value);
            });
            if (playOnStart)
            {
                if (xAxisEnabled)
                {
                    XShake.Start();
                }
                if (yAxisEnabled)
                {
                    YShake.Start();
                }
                if (zAxisEnabled)
                {
                    ZShake.Start();
                }
            }
        }

        void OnDestroy()
        {
            XShake.Complete();
            YShake.Complete();
            ZShake.Complete();
        }

#endregion
    }
}
