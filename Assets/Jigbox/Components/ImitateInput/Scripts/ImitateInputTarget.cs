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
using Jigbox.UIControl;

namespace Jigbox.Components
{
    /// <summary>
    /// ImitateInputModuleからの入力イベント受け取る対象としての目印となるコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    public class ImitateInputTarget : UIBehaviour, ICanvasRaycastFilter
    {
#region properties

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

        /// <summary>実描画領域の左下、右上の座標</summary>
        protected Bounds cachedBounds;

        /// <summary>Transform.positionのキャッシュ</summary>
        protected Vector3 cachedPosition = Vector3.zero;

        /// <summary>Transform.lossyScaleのキャッシュ</summary>
        protected Vector3 cachedScale = Vector3.one;

        /// <summary>RectTransformに変化があったかどうか</summary>
        protected bool rectTransformChanged = false;

        /// <summary>
        /// <para>InputModuleからの入力判定のバリデーションをスルーするかどうか</para>
        /// <para>ImitateInputModuleからImitateInputTargetManager経由で利用されます</para>
        /// </summary>
        public bool ThroughValidateFromInputModule { get; set; }

#endregion

#region public methods

        /// <summary>
        /// レイキャストが有効かどうかを返します。
        /// </summary>
        /// <param name="screenPoint">入力の画面上における座標</param>
        /// <param name="eventCamera">カメラ</param>
        /// <returns></returns>
        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }

            if (ThroughValidateFromInputModule)
            {
                return true;
            }

            // 状態が変わった場合のみ再計算
            if (rectTransformChanged
                || RectTransform.position != cachedPosition
                || RectTransform.lossyScale != cachedScale)
            {
                cachedBounds = RectTransformUtils.GetBounds(RectTransform);
                cachedPosition = RectTransform.position;
                cachedScale = RectTransform.lossyScale;
                rectTransformChanged = false;
            }

            Vector3 world = screenPoint;
            if (eventCamera != null)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(RectTransform, screenPoint, eventCamera, out world);
            }

            // 矩形範囲内にポインタがある場合のみ、ブロック
            if (world.x >= cachedBounds.min.x
                && world.x <= cachedBounds.max.x
                && world.y >= cachedBounds.min.y
                && world.y <= cachedBounds.max.y)
            {
                return false;
            }

            return true;
        }

#endregion

#region override unity methods

        protected override void Start()
        {
            base.Start();

            // RectTransformのサイズは、Start以降でないと正しい数値が取れない
            cachedBounds = RectTransformUtils.GetBounds(RectTransform);
            cachedPosition = RectTransform.position;
            cachedScale = RectTransform.lossyScale;

            ThroughValidateFromInputModule = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ImitateInputTargetManager.Register(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ImitateInputTargetManager.Unregister(this);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            // サイズやpivotの変化はここでキャッチする
            base.OnRectTransformDimensionsChange();
            rectTransformChanged = true;
        }

#endregion
    }
}
