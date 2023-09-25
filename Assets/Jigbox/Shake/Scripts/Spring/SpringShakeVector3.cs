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
using Jigbox.Tween;
using UnityEngine;

namespace Jigbox.Shake
{
    [Serializable]
    public class SpringShakeVector3 : SpringShake<Vector3, SpringShakeVector3>
    {
#region properties

        /// <summary>x軸方向に振動させるかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool xAxis;

        /// <summary>x軸方向に振動させるかどうか</summary>
        public bool XAxis
        {
            get { return xAxis; }
            set { xAxis = value; }
        }

        /// <summary>y軸方向に振動させるかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool yAxis;

        /// <summary>y軸方向に振動させるかどうか</summary>
        public bool YAxis
        {
            get { return yAxis; }
            set { yAxis = value; }
        }

        /// <summary>z軸方向に振動させるかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool zAxis;

        /// <summary>z軸方向に振動させるかどうか</summary>
        public bool ZAxis
        {
            get { return zAxis; }
            set { zAxis = value; }
        }

        /// <summary>反射する向きのランダムさを指定する。</summary>
        [HideInInspector]
        [SerializeField]
        protected float angleRandomness;

        /// <summary>反射する向きのランダムさを指定する。</summary>
        /// <remarks>前回の振動の向きの真反対から±何度の範囲に振動するかを度で指定する。</remarks>
        public float AngleRandomness
        {
            get { return angleRandomness; }
            set { angleRandomness = value; }
        }

        /// <summary>振動の次元</summary>
        protected int dimension;

        /// <summary>前フレームでの <see cref="SpringShake{TValue,TDerived}.Value"/></summary>
        float previouseValue;

        /// <summary>振動の向きをxyz座標で表したもの</summary>
        protected Vector3 direction;

        /// <summary>振動の向きを、ある軸をx軸y軸z軸のそれぞれを回転軸に回転させた角度で表したもの。</summary>
        /// <remark>どの軸を回転させるかは振動の次元による。</remark>
        protected Vector3 directionAngle;

#endregion

#region constructors

        /// <summary>コンストラクター</summary>
        public SpringShakeVector3() {}

        /// <summary><c>OnUpdate</c>イベントハンドラーを登録するコンストラクター</summary>
        /// <param name="onUpdate">イベントハンドラー</param>
        public SpringShakeVector3(Action<SpringShakeVector3> onUpdate) : base(onUpdate) {}

        /// <summary>コピーコンストラクター</summary>
        /// <param name="other">コピー元のインスタンス</param>
        public SpringShakeVector3(SpringShakeVector3 other) : base(other)
        {
            XAxis = other.XAxis;
            YAxis = other.YAxis;
            ZAxis = other.ZAxis;
            AngleRandomness = other.AngleRandomness;
        }

        /// <summary><c>OnUpdate</c>イベントハンドラーを登録するコピーコンストラクター</summary>
        /// <param name="other">コピー元のインスタンス</param>
        /// <param name="onUpdate">イベントハンドラー</param>
        public SpringShakeVector3(SpringShakeVector3 other, Action<SpringShakeVector3> onUpdate) : this(other)
        {
            OnUpdate(onUpdate);
        }

#endregion

        /// <summary>初期状態から稼働させます</summary>
        public override void Start()
        {
            dimension = (XAxis ? 1 : 0) + (YAxis ? 1 : 0) + (ZAxis ? 1 : 0);
            var unitVec = Vector3.right;
            switch (dimension)
            {
                case 1:
                    break;
                case 2:
                    directionAngle = new Vector3(
                        !XAxis ? UnityEngine.Random.Range(0f, 360f) : 0f,
                        !YAxis ? UnityEngine.Random.Range(0f, 360f) : 0f,
                        !ZAxis ? UnityEngine.Random.Range(0f, 360f) : 0f
                    );
                    unitVec = !XAxis ? Vector3.up : !YAxis ? Vector3.forward : Vector3.right;
                    break;
                case 3:
                    directionAngle = new Vector3(
                        UnityEngine.Random.Range(0f, 360f),
                        UnityEngine.Random.Range(0f, 360f),
                        UnityEngine.Random.Range(0f, 360f)
                    );
                    break;
            }
            direction = Quaternion.Euler(directionAngle.x, directionAngle.y, directionAngle.z) * unitVec;
            base.Start();
        }

        /// <summary>引数に与えられた経過時間での値を計算します</summary>
        /// <returns>経過時間での値</returns>
        /// <param name="time">秒単位の時間</param>
        public override Vector3 ValueAt(float time)
        {
            float wave;
            switch (dimension)
            {
                case 1:
                    wave = CalculateWave(time);
                    return Origin + (XAxis ? new Vector3(wave, 0, 0) : YAxis ? new Vector3(0, wave, 0) : new Vector3(0, 0, wave));
                case 2:
                case 3:
                    wave = CalculateWave(time);

                    if (previouseValue * wave < 0)
                    {
                        // 原点を通り過ぎたので、振動の向きにランダムさを加える。

                        Vector3 unitVec = Vector3.right;
                        switch (dimension)
                        {
                            case 2:
                                var random = UnityEngine.Random.Range(-AngleRandomness, AngleRandomness);
                                directionAngle = new Vector3(
                                    !XAxis ? directionAngle.x + random : 0,
                                    !YAxis ? directionAngle.x + random : 0,
                                    !ZAxis ? directionAngle.x + random : 0
                                );
                                unitVec = !XAxis ? Vector3.up : !YAxis ? Vector3.forward : Vector3.right;
                                break;
                            case 3:
                                directionAngle = new Vector3(
                                    directionAngle.x + UnityEngine.Random.Range(-AngleRandomness, AngleRandomness),
                                    directionAngle.y + UnityEngine.Random.Range(-AngleRandomness, AngleRandomness),
                                    directionAngle.z + UnityEngine.Random.Range(-AngleRandomness, AngleRandomness)
                                );
                                break;
                        }
                        direction = Quaternion.Euler(directionAngle.x, directionAngle.y, directionAngle.z) * unitVec;
                    }

                    previouseValue = wave;

                    return Origin + direction * wave;
                default:
                    // 振動軸が指定されていないので振動させない
                    return Origin;
            }
        }

        /// <summary>Mono 2.x でビルドするときにコンパイラーに情報を与えるためのコードで、このメソッドは呼ばれません。</summary>
        static void Mono2xFullAotWorkaround()
        {
            new SpringShakeBehaviour<Vector3, SpringShakeVector3>(null).Init();
        }
    }
}
