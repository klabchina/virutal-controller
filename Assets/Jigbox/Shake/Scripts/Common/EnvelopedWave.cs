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
    public class EnvelopedWave : Wave
    {
#region properties

        /// <summary>包絡線</summary>
        [HideInInspector]
        [SerializeField]
        protected AnimationCurve envelopeCurve;

        /// <summary>包絡線</summary>
        /// <remark>横軸は、<see cref="Progress"/>が非<c>null</c>の場合は、Shakeの動作開始時点を0、終了時点を1として指定してください、<c>null</c>の場合は、経過時間を指定してください。</remark>
        public AnimationCurve EnvelopeCurve
        {
            get { return envelopeCurve; }
            set { envelopeCurve = value; }
        }

        /// <summary>包絡線</summary>
        protected Func<float, float> envelopeFunc;

        /// <summary>包絡線</summary>
        /// <remark>引数は、<see cref="Progress"/>が非<c>null</c>の場合は、Shakeの動作開始時点を0、終了時点を1として与えられます、<c>null</c>の場合は、経過時間が与えられます。</remark>
        public Func<float, float> EnvelopeFunc
        {
            get { return envelopeFunc; }
            set { envelopeFunc = value; }
        }

        /// <summary>経過時間から0〜1の進捗度を求める関数</summary>
        public Func<float, float> Progress { get; set; }

#endregion

#region methods

        /// <summary>経過時間<code>time</code>における包絡線の値</summary>
        /// <param name="time">経過時間</param>
        /// <returns>包絡線の値</returns>
        protected float EnvelopeAt(float time)
        {
            var param = Progress != null ? Progress(time) : time;
            if (EnvelopeCurve != null)
            {
                return EnvelopeCurve.Evaluate(param);
            }
            if (EnvelopeFunc != null)
            {
                return EnvelopeFunc(param);
            }
            return 1;
        }

        /// <summary>正弦波関数</summary>
        /// <param name="time">経過時間</param>
        /// <returns>関数の値</returns>
        public override float Calculate(float time)
        {
            return base.Calculate(time) * EnvelopeAt(time);
        }

#endregion
    }
}
