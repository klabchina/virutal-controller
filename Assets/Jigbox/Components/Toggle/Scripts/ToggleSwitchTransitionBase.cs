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

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public abstract class ToggleSwitchTransitionBase : MonoBehaviour
    {
#region properties

        /// <summary>トランジションの対象となるGameObject</summary>
        [HideInInspector]
        [SerializeField]
        protected GameObject knob;

        /// <summary>トランジションの対象となるGameObject</summary>
        public GameObject Knob { get { return knob; } }

        /// <summary>ON状態での座標</summary>
        [HideInInspector]
        [SerializeField]
        protected Vector3 positionOn;

        /// <summary>ON状態での座標</summary>
        public Vector3 PositionOn { get { return positionOn; } set { positionOn = value; } }

        /// <summary>OFF状態での位置</summary>
        [HideInInspector]
        [SerializeField]
        protected Vector3 positionOff;

        /// <summary>OFF状態での位置</summary>
        public Vector3 PositionOff { get { return positionOff; } set { positionOff = value; } }

        /// <summary>トランジションの時間</summary>
        [HideInInspector]
        [SerializeField]
        protected float duration = 0.1f;

        /// <summary>ON状態でのワールド座標</summary>
        protected Vector3 positionOnWorld;

        /// <summary>ON状態でのワールド座標</summary>
        public Vector3 PositionOnWorld { get { return positionOnWorld; } }

        /// <summary>OFF状態でのワールド座標</summary>
        protected Vector3 positionOffWorld;

        /// <summary>OFF状態でのワールド座標</summary>
        public Vector3 PositionOffWorld { get { return positionOffWorld; } }

        /// <summary>状態</summary>
        protected bool status = true;

#endregion

#region public methods

        /// <summary>
        /// 状態を初期化します。
        /// </summary>
        /// <param name="toggleStatus">トグルの状態</param>
        public virtual void Init(bool toggleStatus)
        {
            if (knob == null)
            {
                return;
            }

            status = toggleStatus;
            knob.transform.localPosition = toggleStatus ? positionOn : positionOff;
        }

        /// <summary>
        /// 状態を切り替えます。
        /// </summary>
        /// <param name="toggleStatus">状態</param>
        public virtual void Switch()
        {
            if (knob == null)
            {
                return;
            }

            status = !status;
            StartTransition();
        }

#endregion

#region protected methods

        /// <summary>
        /// トランジションを開始します。
        /// </summary>
        protected abstract void StartTransition();

        /// <summary>
        /// トランジションを停止します。
        /// </summary>
        protected virtual void StopTransition()
        {
        }

#endregion

#region override unity methods

        protected virtual void Start()
        {
            // Canvasの設定次第でワールド座標で処理する必要がある場合があるので、
            // ワールド座標における状態をキャッシュしておく

            if (knob == null)
            {
#if UNITY_EDITOR
                Debug.LogError("ToggleSwitchTransitionBase.Start : Not serialized knob object!", this);
#endif
                return;
            }

            // 他のコンポーネントの相互変換による影響で
            // Awake時点では正しい値が取れないので、Startで計算する
            Transform knobTransform = knob.transform;
            Vector3 cachedPosition = knobTransform.localPosition;

            knobTransform.localPosition = Vector3.zero;
            positionOnWorld = knobTransform.TransformPoint(positionOn);
            positionOnWorld.z = 0;

            positionOffWorld = knobTransform.TransformPoint(positionOff);
            positionOffWorld.z = 0;

            knobTransform.localPosition = cachedPosition;
        }

        protected virtual void OnDisable()
        {
            StopTransition();
        }

#endregion
    }
}
