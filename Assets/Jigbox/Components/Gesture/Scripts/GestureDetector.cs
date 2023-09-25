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
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Jigbox.UIControl;

namespace Jigbox.Components
{
    /// <summary>
    /// 入力によるジェスチャーの検出を行うコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RaycastArea))]
    public class GestureDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
#region properties

        /// <summary>ジェスチャーの検出が有効かどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool isEnable = true;

        /// <summary>ジェスチャーの検出が有効かどうか</summary>
        public bool IsEnable
        {
            get
            {
                return isEnable;
            }
            set
            {
                if (isEnable != value)
                {
                    isEnable = value;

                    if (raycastArea != null)
                    {
                        raycastArea.raycastTarget = isEnable;
                    }
                }
            }
        }

        /// <summary>RaycastArea</summary>
        [HideInInspector]
        [SerializeField]
        protected RaycastArea raycastArea;

        /// <summary>RaycastArea</summary>
        public RaycastArea RaycastArea { get { return raycastArea; } }

        /// <summary>判定領域の大きさ</summary>
        public Vector2 Size { get { return raycastArea.Size; } set { raycastArea.Size = value; } }

        /// <summary>入力の排他制御用クラス</summary>
        [HideInInspector]
        [SerializeField]
        protected SubmitColleague colleague;

        /// <summary>ジェスチャーの判別用クラス</summary>
        [HideInInspector]
        [SerializeField]
        protected List<GestureUmpireBehaviour> umpires = new List<GestureUmpireBehaviour>();

        /// <summary>押下されているポインタ</summary>
        protected HashSet<PointerEventData> pressedPointers = new HashSet<PointerEventData>();

        /// <summary>押下されているポインタの数</summary>
        public int PressedCount { get { return pressedPointers.Count; } }

        /// <summary>ポインタが画面から離されたかどうか</summary
        protected bool isRelease = false;

#if UNITY_EDITOR

        /// <summary>エディタやPCで実行中に入力のロックが自動解除されるのをブロックします</summary>
        public bool IsBlockAutoReleaseInEditor { get; set; }

        /// <summary>入力のロック状態を維持するかどうか</summary>
        protected bool isKeepLock = false;

        /// <summary>入力のロックが有効かどうか</summary>
        protected bool IsEnableLock
        {
            get
            {
                if (isKeepLock)
                {
                    return true;
                }
                else
                {
                    return colleague.IsEnable;
                }
            }
        }

#endif

#endregion

#region public methods

        /// <summary>
        /// 判定領域が押下された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!IsEnable)
            {
                return;
            }

#if UNITY_EDITOR
            if (!IsEnableLock)
#else
            if (!colleague.IsEnable)
#endif
            {
                colleague.GetLock();

                if (!colleague.IsEnable)
                {
                    return;
                }
            }

            isRelease = false;

            pressedPointers.Add(eventData);

            foreach (GestureUmpireBehaviour umpire in umpires)
            {
                if (umpire != null && umpire.IsEnable)
                {
                    umpire.OnPress(eventData, pressedPointers);
                }
            }
        }

        /// <summary>
        /// 判定領域押下後に指が離された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!IsEnable)
            {
                return;
            }

#if UNITY_EDITOR
            if (!IsEnableLock)
#else
            if (!colleague.IsEnable)
#endif
            {
                return;
            }

            isRelease = true;

            pressedPointers.RemoveWhere(pointer => pointer.pointerId == eventData.pointerId);

            foreach (GestureUmpireBehaviour umpire in umpires)
            {
                if (umpire != null && umpire.IsEnable)
                {
                    umpire.OnRelease(eventData, pressedPointers);
                    isRelease &= umpire.MayUnlock;
                }
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// 入力のロックが自動解除された際に呼び出されます。
        /// </summary>
        protected void AutoUnlock()
        {
#if UNITY_EDITOR
            // デバッグ機能による入力中は、画面への入力がない状態で
            // 強制的にイベントだけ発火させるという本来ありえない状況で
            // イベントを実行しているので、入力の排他制御のデッドロック防止機能で
            // ロックの自動解除が行われるが、それをブロックして入力を維持する
            if (IsBlockAutoReleaseInEditor)
            {
                // ロック自体はすでに外れてしまっているので、
                // フラグで擬似的にロックを維持する
                isKeepLock = true;
                return;
            }
#endif
            foreach (GestureUmpireBehaviour umpire in umpires)
            {
                if (umpire != null && umpire.IsEnable)
                {
                    umpire.OnAutoUnlock();
                }
            }
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            // GameObject自体を複製したり、PrefabをInstantiateした場合、
            // シリアライズされている情報は同じになるので、
            // Colleagueに設定されるID情報を新しいインスタンスのものに更新する
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                colleague = new SubmitColleague(this, colleague.Group, colleague.CoolTime);
            }
#else
            colleague = new SubmitColleague(this, colleague.Group, colleague.CoolTime);
#endif
            colleague.SetAutoUnlockCallback(AutoUnlock);
        }

        protected virtual void Update()
        {
            foreach (GestureUmpireBehaviour umpire in umpires)
            {
                if (umpire != null && umpire.IsEnable && umpire.IsEnableUpdate)
                {
                    umpire.OnUpdate(pressedPointers);
                }
            }
        }

        protected virtual void LateUpdate()
        {
#if UNITY_EDITOR
            if (isRelease && IsEnableLock)
#else
            if (isRelease && colleague.IsEnable)
#endif
            {
                colleague.Unlock();
                pressedPointers.Clear();
                isRelease = false;
#if UNITY_EDITOR
                isKeepLock = false;
#endif
            }
        }

        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            if (IsEnableLock)
#else
            if (colleague.IsEnable)
#endif
            {
                colleague.Unlock();
                pressedPointers.Clear();
                isRelease = false;
#if UNITY_EDITOR
                isKeepLock = false;
#endif

                foreach (GestureUmpireBehaviour umpire in umpires)
                {
                    if (umpire != null && umpire.IsEnable)
                    {
                        umpire.OnAutoUnlock();
                    }
                }
            }
        }

#endregion
    }
}
