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

using System;
using UnityEngine;

namespace Jigbox.Shake
{
    [Serializable]
    public class Wave
    {
#region properties

        /// <summary>振動の振幅</summary>
        [HideInInspector]
        [SerializeField]
        protected float amplitude;

        /// <summary>振動の振幅</summary>
        public float Amplitude
        {
            get { return amplitude; }
            set { amplitude = value; }
        }

        /// <summary>振動の周波数 [Hz]</summary>
        [HideInInspector]
        [SerializeField]
        protected float frequency;

        /// <summary>振動の周波数 [Hz]</summary>
        public float Frequency
        {
            get { return frequency; }
            set { frequency = value; }
        }

#endregion

        /// <summary>正弦波関数</summary>
        /// <param name="time">経過時間</param>
        /// <returns>関数の値</returns>
        public virtual float Calculate(float time)
        {
            // 周期 = 1 / 周波数
            // 現在の周期に対する割合 = 経過時間 / 周期
            // なので 現在の周期に対する割合 = 周波数 × 経過時間
            // 1周期は角度では2πなので sin の引数は 2π × 周波数 × 経過時間
            return Amplitude * Mathf.Sin(2 * Mathf.PI * Frequency * time);
        }
    }
}
