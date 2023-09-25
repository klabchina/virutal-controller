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
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jigbox.Components
{
    [ExecuteAlways]
    public class RectangleSizePreserver : MonoBehaviour
    {
#region inner classes, enum, and structs        

        public enum FixedType
        {
            /// <summary>固定なし</summary>
            None,
            /// <summary>最小サイズを固定</summary>
            FixMin,
            /// <summary>最大サイズを固定</summary>
            FixMax,
        }

#endregion
        
#region properties

        /// <summary>Updateで更新するかどうか</summary>
        [HideInInspector]
        [SerializeField]
        bool isUpdate = true;
        
        /// <summary>横幅の固定設定</summary>
        [HideInInspector]
        [SerializeField]
        protected FixedType fixWidth = FixedType.None;

        /// <summary>縦幅の固定設定</summary>
        [HideInInspector]
        [SerializeField]
        protected FixedType fixHeight = FixedType.None;

        /// <summary>固定する幅</summary>
        [HideInInspector]
        [SerializeField]
        protected Vector2 fixedSize;

        /// <summary>固定していない状態への復元用</summary>
        [HideInInspector]
        [SerializeField]
        protected Vector2 baseSizeDelta;

        /// <summary>RectTransform</summary>
        protected RectTransform rectTransform;

        /// <summary>RectTransform</summary>
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = transform as RectTransform;
                }
                return rectTransform;
            }
        }

        /// <summary>矩形領域の大きさ</summary>
        public Vector2 Size
        {
            get
            {
                return RectTransform.rect.size;
            }
            set
            {
                if (RectTransform.rect.size != value)
                {
                    Vector2 lastSizeDelta = RectTransform.sizeDelta;

                    Vector2 anchorMargin = Vector2.zero;
                    anchorMargin.x = RectTransform.rect.width - RectTransform.sizeDelta.x;
                    anchorMargin.y = RectTransform.rect.height - RectTransform.sizeDelta.y;
                    RectTransform.sizeDelta = value - anchorMargin;

                    Vector2 sizeDeltaOffset = lastSizeDelta - RectTransform.sizeDelta;
                    Vector3 offset = RectTransform.localPosition;

                    offset.x -= (RectTransform.pivot.x - 0.5f) * sizeDeltaOffset.x;
                    offset.y -= (RectTransform.pivot.y - 0.5f) * sizeDeltaOffset.y;

                    RectTransform.localPosition = offset;
                }
            }
        }

        /// <summary>横幅を固定しているかどうか</summary>
        protected bool IsFixWidth { get { return fixWidth != FixedType.None; } }

        /// <summary>縦幅を固定しているかどうか</summary>
        protected bool IsFixHeight { get { return fixHeight != FixedType.None; } }

#if UNITY_EDITOR
        /// <summary>1フレーム前の画面サイズ</summary>
        Vector2 lastScreenSize = Vector2.zero;
#endif

#endregion

#region public methods

        /// <summary>
        /// サイズを更新します。
        /// </summary>
        public void UpdateSize()
        {
            Vector2 size = Size;

#if UNITY_EDITOR
            // エディタ実行時は画面比を変えた際の再更新のために画面サイズを監視する
            bool isScreenResized = lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height;
            if (isScreenResized)
            {
                lastScreenSize.x = Screen.width;
                lastScreenSize.y = Screen.height;
            }

            if (!IsFixWidth)
            {
                baseSizeDelta.x = RectTransform.sizeDelta.x;
                fixedSize.x = size.x;
            }
            if (!IsFixHeight)
            {
                baseSizeDelta.y = RectTransform.sizeDelta.y;
                fixedSize.y = size.y;
            }
#endif

#if UNITY_EDITOR
            if (IsFixWidth && !isScreenResized)
#else
            if (IsFixWidth)
#endif
            {
                if (fixWidth == FixedType.FixMin ? Size.x < fixedSize.x : Size.x > fixedSize.x)
                {
                    size.x = fixedSize.x;
                }
            }
            else
            {
                size.x = RectTransform.rect.width - RectTransform.sizeDelta.x + baseSizeDelta.x;
            }
            
#if UNITY_EDITOR
            if (IsFixHeight && !isScreenResized)
#else
            if (IsFixHeight)
#endif
            {
                if (fixHeight == FixedType.FixMin ? Size.y < fixedSize.y : Size.y > fixedSize.y)
                {
                    size.y = fixedSize.y;
                }
            }
            else
            {
                size.y = RectTransform.rect.height - RectTransform.sizeDelta.y + baseSizeDelta.y;
            }

            Size = size;
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            if (fixedSize.x == 0.0f && fixedSize.y == 0.0f)
            {
                fixedSize = Size;
            }

            if (!IsFixWidth && baseSizeDelta.x != RectTransform.sizeDelta.x)
            {
                baseSizeDelta.x = RectTransform.sizeDelta.x;
            }
            if (!IsFixHeight && baseSizeDelta.y != RectTransform.sizeDelta.y)
            {
                baseSizeDelta.y = RectTransform.sizeDelta.y;
            }
            
            UpdateSize();
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (isUpdate)
                {
                    UpdateSize();
                }
            }
            else
            {
                UpdateSize();
            }
#else
            if (isUpdate)
            {
                UpdateSize();
            }
#endif
        }

#endregion
    }
}
