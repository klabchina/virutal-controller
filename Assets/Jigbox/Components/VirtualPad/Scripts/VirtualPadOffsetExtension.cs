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
using Jigbox.VirtualPad;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jigbox.Components
{
    /// <summary>
    /// 時間と位置の補正を行う拡張コンポーネント
    /// </summary>
    public class VirtualPadOffsetExtension : VirtualPadActivateExtensionBase
    {
#region serialize fields

        /// <summary>
        /// バーチャルパッドのタッチ場所を補正するピクセル数
        /// </summary>
        [SerializeField]
        protected float activateThreshold;

        /// <summary>
        /// バーチャルパッドの有効化までの時間
        /// </summary>
        [SerializeField]
        protected float activateSecondsOffset;

#endregion

#region properties

        /// <summary>
        /// バーチャルパッドのタッチ位置から、有効化するまでの閾値
        /// </summary>
        public float ActivateThreshold
        {
            get { return activateThreshold; }
            set { activateThreshold = value; }
        }

        /// <summary>
        /// バーチャルパッドの有効化までの時間
        /// </summary>
        public float ActivateSecondsOffset
        {
            get { return activateSecondsOffset; }
            set { activateSecondsOffset = value; }
        }

#endregion

#region fields

        /// <summary>
        /// Pressされてからの経過時間
        /// </summary>
        protected float elapsedSeconds = 0.0f;

        /// <summary>
        /// バーチャルパッドの時間経過後の閾値計算に使用される位置
        /// </summary>
        protected Vector2 fromPosition = Vector2.zero;

        /// <summary>
        /// 現在閾値計算を行なっている状態かどうか
        /// </summary>
        protected bool isCalculatingThreshold = false;

#endregion

#region public methods

        public override void Activate(PointerEventData pointerEventData)
        {
            if (this.pointerEventData != null)
            {
                return;
            }

            base.Activate(pointerEventData);

            elapsedSeconds = 0.0f;
            isCalculatingThreshold = false;
            fromPosition = Vector2.zero;
        }

        public override void OnUpdate()
        {
            if (this.pointerEventData == null)
            {
                return;
            }

            // 既に時間の補正が終了している場合は、位置の計算のみを行う
            if (isCalculatingThreshold)
            {
                if (IsOverThreshold(fromPosition, pointerEventData.position))
                {
                    EnableVirtualPad();
                }

                return;
            }

            elapsedSeconds += Time.deltaTime;

            if (elapsedSeconds >= ActivateSecondsOffset)
            {
                isCalculatingThreshold = true;
                fromPosition = pointerEventData.position;
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// 閾値を超えているかどうかの計算を行います。
        /// </summary>
        /// <param name="from">元になる位置</param>
        /// <param name="current">現在の位置</param>
        /// <returns>閾値を超えているかどうか</returns>
        protected virtual bool IsOverThreshold(Vector2 from, Vector2 current)
        {
            return Vector2.Distance(from, current) >= activateThreshold;
        }

#endregion
    }
}
