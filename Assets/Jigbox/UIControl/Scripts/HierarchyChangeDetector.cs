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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.UIControl
{
    [DisallowMultipleComponent]
    public class HierarchyChangeDetector : MonoBehaviour, IHierarchyChangeReceiver
    {
#region properties

        /// <summary>
        /// 配下にオブジェクトが生成された子のリスト
        /// 毎フレームクリアされる
        /// </summary>
        protected List<Transform> changedChildren = new List<Transform>();

        /// <summary>
        /// 配下で削除された子のリスト
        /// 毎フレームクリアされる
        /// </summary>
        protected List<Transform> removedChildren = new List<Transform>();

        /// <summary>
        /// 監視対象オブジェクトのリスト
        /// </summary>
        protected List<Transform> detectors = new List<Transform>();

        /// <summary>
        /// 監視対象が追加された際のコールバック
        /// </summary>
        protected Action onHierarchyChanged = null;

        /// <summary>
        /// ヒエラルキーが現在のフレームで更新されているかどうか
        /// </summary>
        public virtual bool IsHierarchyDirty
        {
            get { return changedChildren.Count != 0 || removedChildren.Count != 0; }
        }

        /// <summary>
        /// フレーム終了時のコルーチンのキャッシュ
        /// </summary>
        protected Coroutine cachedFrameEndObserver = null;

        /// <summary>
        /// GC回避のためのキャッシュ
        /// </summary>
        protected YieldInstruction cachedWaitForEndFrame = new WaitForEndOfFrame();

        /// <summary>
        /// GC回避のためのキャッシュ
        /// </summary>
        protected List<Transform> cachedTransforms = new List<Transform>();

#endregion

#region public methods

        /// <summary>
        /// 監視対象のオブジェクトを取得します
        /// </summary>
        /// <returns></returns>
        public virtual List<Transform> GetHierarchyObjects()
        {
            UpdateForwarder();
            return detectors;
        }

        /// <summary>
        /// 監視対象が増えた際のコールバックを追加します
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddHierarchyChangedCallback(Action callback)
        {
            onHierarchyChanged += callback;
        }

#endregion

#region private & protected methods

        /// <summary>
        /// 監視対象を追加します
        /// </summary>
        protected virtual void AddForwarder()
        {
            if (changedChildren.Count == 0)
            {
                return;
            }

            foreach (var child in changedChildren)
            {
                cachedTransforms.Clear();
                child.GetComponentsInChildren(true, cachedTransforms);

                foreach (var newTransform in cachedTransforms)
                {
                    // 既にコンポーネントがアタッチされているオブジェクトを弾く
                    if (detectors.Contains(newTransform))
                    {
                        continue;
                    }

                    var forwarder = newTransform.gameObject.GetComponent<UnityEventForwarder>();

                    if (forwarder == null)
                    {
                        forwarder = newTransform.gameObject.AddComponent<UnityEventForwarder>();
                    }

                    forwarder.AddReceiver(this);
                    detectors.Add(newTransform);
                }
            }
        }

        /// <summary>
        /// 監視対象を削除します
        /// </summary>
        protected void RemoveForwarder()
        {
            if (removedChildren.Count == 0)
            {
                return;
            }

            foreach (var child in removedChildren)
            {
                if (detectors.Contains(child))
                {
                    detectors.Remove(child);
                }

                if (changedChildren.Contains(child))
                {
                    changedChildren.Remove(child);
                }
            }
        }

        /// <summary>
        /// このコンポーネントの状態を更新します
        /// </summary>
        protected virtual void UpdateForwarder()
        {
            AddForwarder();
            RemoveForwarder();

            if (IsHierarchyDirty)
            {
                NoticeHierarchyChanged();
            }

            ClearCurrentFrameData();
        }

        /// <summary>
        /// フレーム内で変更されたデータのリストを削除します
        /// </summary>
        protected virtual void ClearCurrentFrameData()
        {
            if (!IsHierarchyDirty)
            {
                return;
            }

            changedChildren.Clear();
            removedChildren.Clear();
            cachedFrameEndObserver = null;
        }

        /// <summary>
        /// フレーム終わりを見張るコルーチンを発火します
        /// </summary>
        protected virtual void StartFrameEndCoroutine()
        {
            if (Application.isPlaying && isActiveAndEnabled && cachedFrameEndObserver == null)
            {
                cachedFrameEndObserver = StartCoroutine(FrameEndObserver());
            }
        }

        /// <summary>
        /// フレーム終わりを見張るためのコルーチンです
        /// </summary>
        /// <returns></returns>
        IEnumerator FrameEndObserver()
        {
            yield return cachedWaitForEndFrame;
            UpdateForwarder();
        }

#endregion

#region callback

        /// <summary>
        /// 監視対象が更新された際のコールバックを発火します
        /// </summary>
        protected virtual void NoticeHierarchyChanged()
        {
            if (onHierarchyChanged != null)
            {
                onHierarchyChanged();
            }
        }

        /// <summary>
        /// 子オブジェクトが追加、削除された際に呼ばれます
        /// </summary>
        /// <param name="child"></param>
        public virtual void OnNoticeChildrenChanged(Transform child)
        {
            if (changedChildren.Contains(child))
            {
                return;
            }

            changedChildren.Add(child);
            StartFrameEndCoroutine();
        }

        /// <summary>
        /// 子オブジェクトが削除された際に呼ばれます
        /// </summary>
        /// <param name="child"></param>
        public virtual void OnNoticeDestroy(Transform child)
        {
            if (removedChildren.Contains(child))
            {
                return;
            }

            removedChildren.Add(child);
            StartFrameEndCoroutine();
        }

#endregion

#region override unity methods

        void Awake()
        {
            changedChildren.Add(transform);
            UpdateForwarder();
        }

        void OnEnable()
        {
            if (IsHierarchyDirty)
            {
                UpdateForwarder();
            }
        }

        void OnDestroy()
        {
            if (cachedFrameEndObserver != null)
            {
                StopCoroutine(cachedFrameEndObserver);
                cachedFrameEndObserver = null;
            }
        }

#endregion
    }
}
