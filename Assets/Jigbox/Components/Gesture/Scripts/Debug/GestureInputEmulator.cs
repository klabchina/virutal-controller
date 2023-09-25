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

namespace Jigbox.Components
{
    /// <summary>
    /// マルチタッチジェスチャーのデバッグ用擬似入力モジュール
    /// </summary>
    public class GestureInputEmulator : MonoBehaviour
    {
#region serializefields

        /// <summary>ジェスチャーの検出用コンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected GestureDetector detector;

        /// <summary>擬似入力の入力ID</summary>
        [HideInInspector]
        [SerializeField]
        protected int dummyId = 1;

        /// <summary>擬似入力を発生させるキー</summary>
        [HideInInspector]
        [SerializeField]
        protected KeyCode key = KeyCode.LeftControl;

#endregion

#if UNITY_EDITOR

#region properties

        /// <summary>EventSystem</summary>
        protected EventSystem eventSystem;

        /// <summary>EventSystem</summary>
        protected EventSystem EventSystem
        {
            get
            {
                if (eventSystem == null)
                {
                    eventSystem = FindObjectOfType<EventSystem>();
                }
                return eventSystem;
            }
        }

        /// <summary>入力情報</summary>
        protected PointerEventData pointerEventData;

        /// <summary>入力情報</summary>
        protected PointerEventData PointerEventData
        {
            get
            {
                if (pointerEventData == null)
                {
                    pointerEventData = new PointerEventData(EventSystem);
                }
                return pointerEventData;
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// 画面上での座標を取得します。
        /// </summary>
        /// <returns></returns>
        protected Vector3 GetScreenPoint()
        {
            Canvas canvas = detector.RaycastArea.canvas;
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                Camera camera = canvas.worldCamera;
                if (camera != null)
                {
                    return RectTransformUtility.WorldToScreenPoint(camera, transform.position);
                }
                // Overlayじゃない状態でカメラを設定していない事自体が本来はあり得ないが
                // もし、その状態できた場合は、正しい座標を計算できないので0ベクトルを返す
                else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return transform.position;
            }
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            if (detector == null)
            {
                Debug.LogWarning("GestureDetector not found!", gameObject);
                enabled = false;
            }
        }

        protected virtual void Update()
        {
            if (InputWrapper.GetKeyDown(key))
            {
                PointerEventData eventData = PointerEventData;
                eventData.Reset();
                eventData.position = GetScreenPoint();
                eventData.pointerPress = detector.gameObject;
                eventData.pointerId = dummyId;
                detector.IsBlockAutoReleaseInEditor = true;
                ExecuteEvents.Execute(detector.gameObject, eventData, ExecuteEvents.pointerDownHandler);
            }
            
            if (InputWrapper.GetKeyUp(key))
            {
                PointerEventData eventData = PointerEventData;
                eventData.Reset();
                eventData.position = GetScreenPoint();
                eventData.pointerId = dummyId;
                detector.IsBlockAutoReleaseInEditor = false;
                ExecuteEvents.Execute(detector.gameObject, eventData, ExecuteEvents.pointerUpHandler);
            }
        }

#endregion

#endif
    }
}
