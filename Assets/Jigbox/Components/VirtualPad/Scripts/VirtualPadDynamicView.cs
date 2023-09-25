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
    /// 位置が動的に変化するバーチャルパッドの表示を構成するクラス
    /// </summary>
    public class VirtualPadDynamicView : VirtualPadView
    {
#region properties

        /// <summary>有効範囲を表すRectTransformの参照</summary>
        [HideInInspector]
        [SerializeField]
        protected RectTransform validRect;

        /// <summary>有効範囲を表すRectTransformの参照</summary>
        public override RectTransform ValidRect { get { return validRect != null ? validRect : RectTransform; } }

#endregion

#region public methods

        /// <summary>
        /// バーチャルパッドの表示を有効化します。
        /// </summary>
        /// <param name="position">入力位置</param>
        public override void Activate(Vector2 position)
        {
            gameObject.SetActive(true);
            transform.localPosition = position;
        }

        /// <summary>
        /// バーチャルパッドの表示を無効化します。
        /// </summary>
        public override void Diactivate()
        {
            base.Diactivate();
            gameObject.SetActive(false);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            gameObject.SetActive(false);
        }

#endregion
    }
}
