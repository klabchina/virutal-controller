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
    /// <summary>
    /// バーチャルパッドの表示を構成するクラス
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class VirtualPadView : MonoBehaviour
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

        /// <summary>有効範囲を表すRectTransformの参照</summary>
        public virtual RectTransform ValidRect { get { return RectTransform; } }

        /// <summary>操作用のハンドルオブジェクトのRectTransform</summary>
        [HideInInspector]
        [SerializeField]
        protected RectTransform handle;

        /// <summary>ハンドルの可動域(半径)</summary>
        public virtual float RangeOfMotion { get { return RectTransform.rect.size.x * 0.5f; } }

#endregion

#region public methods

        /// <summary>
        /// バーチャルパッドの表示を有効化します。
        /// </summary>
        /// <param name="position">入力位置</param>
        public virtual void Activate(Vector2 position)
        {
        }

        /// <summary>
        /// バーチャルパッドの表示を無効化します。
        /// </summary>
        public virtual void Diactivate()
        {
            handle.localPosition = Vector3.zero;
        }

        /// <summary>
        /// ハンドル位置を更新します。
        /// </summary>
        /// <param name="position"></param>
        public virtual void UpdateHandle(Vector2 position)
        {
            handle.localPosition = position;
        }

#endregion
    }
}
