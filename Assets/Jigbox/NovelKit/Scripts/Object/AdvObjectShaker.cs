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
using System.Collections.Generic;
using Jigbox.Tween;

namespace Jigbox.NovelKit
{
    public class AdvObjectShaker : MonoBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 振動情報
        /// </summary>
        protected class ShakeInfo
        {
            /// <summary>Tween</summary>
            protected ITween tween;

            /// <summary>振幅</summary>
            protected float amplitude;

            /// <summary>基準値</summary>
            protected float baseValue;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="tween">Tween</param>
            /// <param name="amplitude">振幅</param>
            /// <param name="baseValue">基準値</param>
            public ShakeInfo(ITween tween, float amplitude, float baseValue)
            {
                this.tween = tween;
                this.amplitude = amplitude;
                this.baseValue = baseValue;
            }

            /// <summary>
            /// 更新した値を返します。
            /// </summary>
            /// <param name="t">Tween</param>
            /// <returns></returns>
            public float Update(ITween<float> t)
            {
                float v = amplitude * Mathf.Sin(t.Value);
                return baseValue + v;
            }

            /// <summary>
            /// Tweenを停止します。
            /// </summary>
            public void Stop()
            {
                tween.Complete();
                tween.Kill();
            }
        }

#endregion

#region properties
        
        /// <summary>シナリオ用オブジェクトの参照</summary>
        protected AdvObjectBase advObject;

        /// <summary>振動情報</summary>
        protected Dictionary<string, ShakeInfo> shakeInfo = new Dictionary<string, ShakeInfo>();

#endregion

#region public methods

        /// <summary>
        /// 振動を開始します。
        /// </summary>
        /// <param name="axes">軸</param>
        /// <param name="duration">1振動あたりの時間</param>
        /// <param name="amplitude">振幅</param>
        /// <param name="type">モーションの種類</param>
        /// <returns></returns>
        public bool Shake(string axes, float duration, float amplitude, MotionType type)
        {
            if (shakeInfo.ContainsKey(axes))
            {
                return false;
            }

            if (advObject == null)
            {
                advObject = GetComponent<AdvObjectBase>();
            }

            TweenSingle tween = new TweenSingle();

            tween.Begin = -Mathf.PI;
            tween.Final = Mathf.PI;
            tween.EasingWith(t => t);
            tween.MotionType = type;
            tween.Duration = duration;
            tween.LoopMode = LoopMode.Restart;
            tween.Interval = 0.0f;

            ShakeInfo info = null;

            switch (axes)
            {
                case AdvCommandObjectShake.AxesX:
                    info = new ShakeInfo(tween, amplitude, advObject.Position.x);
                    tween.OnUpdate(t =>
                    {
                        float v = info.Update(t);
                        Vector3 position = advObject.Position;
                        position.x = v;
                        advObject.Position = position;
                    });
                    break;
                case AdvCommandObjectShake.AxesY:
                    info = new ShakeInfo(tween, amplitude, advObject.Position.y);
                    tween.OnUpdate(t =>
                    {
                        float v = info.Update(t);
                        Vector3 position = advObject.Position;
                        position.y = v;
                        advObject.Position = position;
                    });
                    break;
                case AdvCommandObjectShake.AxesZ:
                    info = new ShakeInfo(tween, amplitude, advObject.Position.z);
                    tween.OnUpdate(t =>
                    {
                        float v = info.Update(t);
                        Vector3 position = advObject.Position;
                        position.z = v;
                        advObject.Position = position;
                    });
                    break;
            }

            shakeInfo.Add(axes, info);
            tween.Start();

            return true;
        }

        /// <summary>
        /// 振動を停止させます。
        /// </summary>
        public void Stop()
        {
            foreach (ShakeInfo info in shakeInfo.Values)
            {
                info.Stop();
            }

            shakeInfo.Clear();
        }

#endregion

#region override unity methods

        /// <summary>
        /// OnDestroy
        /// </summary>
        void OnDestroy()
        {
            Stop();
        }

#endregion
    }
}
