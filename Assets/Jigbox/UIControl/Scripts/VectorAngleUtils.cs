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

namespace Jigbox.UIControl
{
    /// <summary>
    /// ベクトルの角度ユーティリティクラス
    /// </summary>
    public static class VectorAngleUtils
    {
#region properties
        
        /// <summary>acos(cosθ)=0と仮定するベクトル</summary>
        static readonly Vector3 OriginDirection = Vector3.up;

#endregion

#region public methods

        /// <summary>
        /// 方向を表す正規化されたベクトルから-π～πまでのラジアン角を求めます。
        /// </summary>
        /// <param name="vector">方向を表す正規化されたベクトル</param>
        /// <returns></returns>
        public static float GetAngle(Vector3 vector)
        {
            // 本来はcosθ=(A、Bの内積) / ((Aの長さ * Bの長さ))で求められるが
            // 単位ベクトルで計算すれば長さは必ず1であるため、長さの計算は省略できる
            float dot = Vector3.Dot(OriginDirection, vector);
            // 単位ベクトルの内積が取る値は、必ず-1～1の範囲であり、
            // これをcosθで表すとπ～0となる
            // 1回転するために必要な角度の範囲は2πとなり、そのままでは
            // 半回転分の値しか取得できないため、基準となるベクトルから
            // 反時計回り、時計回りにどのくらい角度を取っているかで表す
            // 数学的には、反時計回りを正の角度として扱う
            // ベクトルの位置関係は正確には外積を用いるべきだが、
            // この場合は、基準となるベクトルが固定されているため、
            // より単純な条件で反時計回りなのか時計回りなのかの判定を行う
            return vector.x <= 0 ? Mathf.Acos(dot) : -Mathf.Acos(dot);
        }

        /// <summary>
        /// 方向を表す正規化されたベクトルv1、v2からv1に対してv2がなす角を-π～πまでのラジアン角を求めます。
        /// </summary>
        /// <param name="v1">角度の基準となる方向を表す正規化されたベクトル</param>
        /// <param name="v2">方向を表す正規化されたベクトル</param>
        /// <returns></returns>
        public static float GetAngle(Vector3 v1, Vector3 v2)
        {
            float dot = Vector3.Dot(v1, v2);

            // v1に対して、v2が左右どちらに位置しているかによって角度の符号が変わる
            // 外積によって2つのベクトルを含む面に対して垂直なベクトルが求められ、
            // かつ、ベクトルの向きによって面の表裏も反転する
            // これを利用して、xy平面上の2つのベクトルが基準となるベクトルからみて
            // 左右のどちらに存在するのかが判定できる
            bool isLeft = Vector3.Cross(v1, v2).z > 0;
            if (isLeft)
            {
                // v2がv1の左側に存在する場合、角度は正                
                return Mathf.Acos(dot);
            }
            else
            {
                // v2がv1の右側に存在する場合、角度は負
                return -Mathf.Acos(dot);
            }
        }

        /// <summary>
        /// ラジアン角からオイラー角を求めます。
        /// </summary>
        /// <param name="angle">ラジアン角</param>
        /// <returns></returns>
        public static float ToEuler(float angle)
        {
            return 180.0f * (angle / Mathf.PI);
        }

#endregion
    }
}
