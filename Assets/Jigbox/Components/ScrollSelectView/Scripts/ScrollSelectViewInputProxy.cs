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
    public class ScrollSelectViewInputProxy : ScrollSelectViewInputProxyBase
    {
#region constants

        /// <summary>ポインタが動いていないと判断する際の誤差範囲のデフォルト値</summary>
        protected static readonly float DefaultTolerance = 3.0f;

#endregion

#region properties

        /// <summary>入力がクリックとして扱われる誤差範囲</summary>
        [HideInInspector]
        [SerializeField]
        protected float tolerance = DefaultTolerance;

        /// <summary>最初にポインタが入力された位置の座標</summary>
        protected Vector3 pressPosition = Vector3.zero;

        /// <summary>誤差範囲外にポインタが移動したかどうか</summary>
        protected bool movedOutOfTolerance = false;

#endregion

#region public methods

        /// <summary>
        /// ドラッグ対象が見つかった際に呼び出されます。(実質押下と同タイミング)
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (IsValidPointerId)
            {
                return;
            }
            
            base.OnInitializePotentialDrag(eventData);
            
            pressPosition = GetWorldPoint(eventData);
            movedOutOfTolerance = false;
        }

        /// <summary>
        /// ドラッグ中にポインタが移動した際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDrag(PointerEventData eventData)
        {
            //Dragの復帰処理があるので先に基底処理を呼ぶ
            base.OnDrag(eventData);
            
            if (PressedPointerId != InputWrapper.GetEventDataTouchId(eventData))
            {
                return;
            }
            
            // 一度でも誤差範囲外に出たら、それ以降クリックとしては扱われなくなる
            if (!movedOutOfTolerance && !IsValidPosition(eventData.position, eventData.pressPosition))
            {
                movedOutOfTolerance = true;
            }
        }

        /// <summary>
        /// ドラッグが終了した際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (PressedPointerId != InputWrapper.GetEventDataTouchId(eventData))
            {
                return;
            }
            
            base.OnEndDrag(eventData);

            if (!movedOutOfTolerance)
            {
                pressPosition = GetWorldPoint(eventData);
                ScrollSelectView.ChangeSelectCellOrAdjust(pressPosition);
            }
            else
            {
                ScrollSelectView.AdjustPositionIfNeeded();
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// <para>PointerEventData.positionをワールド座標に変換して取得します。</para>
        /// <para>※画面からポインタ(指)が離れている状態では使用できません。</para>
        /// </summary>
        /// <param name="eventData">入力イベント情報</param>
        /// <returns>ワールド座標</returns>
        protected Vector3 GetWorldPoint(PointerEventData eventData)
        {
            Vector3 position;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                eventData.pointerDrag.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out position);

            return position;
        }

        /// <summary>
        /// 座標のズレが誤差範囲に収まっているかどうかを返します。
        /// </summary>
        /// <param name="p1">座標1</param>
        /// <param name="p2">座標2</param>
        /// <returns>ズレが許容範囲内なら<c>true</c>、範囲外なら<c>false</c>を返します。</returns>
        protected bool IsValidPosition(Vector3 p1, Vector3 p2)
        {
            Vector3 vector = p1 - p2;
            return vector.sqrMagnitude <= tolerance * tolerance;
        }

        /// <summary>
        /// LateUpdateで監視しているPress判定が終了した際に呼ばれます。
        /// </summary>
        protected override void OnPressReleased()
        {
            base.OnPressReleased();
            ScrollSelectView.ChangeSelectCellOrAdjust(pressPosition);
        }

#endregion
    }
}
