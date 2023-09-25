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

namespace Jigbox.UIControl
{
    public static class PointerEventDataUtils
    {
#region public methods

        /// <summary>
        /// <para>PointerEventData.positionをワールド座標に変換して取得します。</para>
        /// <para>※画面からポインタ(指)が離れている状態では使用できません。</para>
        /// </summary>
        /// <param name="eventData">入力イベント情報</param>
        /// <returns>ワールド座標</returns>
        public static Vector3 GetWorldPoint(PointerEventData eventData)
        {
            Vector3 position;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                eventData.pointerPress.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out position);

            return position;
        }

        /// <summary>
        /// <para>PointerEventData.positionを入力を受けたオブジェクト内のローカル空間の座標に変換して取得します。</para>
        /// <para>※画面からポインタ(指)が離れている状態では使用できません。</para>
        /// </summary>
        /// <param name="eventData">入力イベント情報</param>
        /// <returns>ローカル座標(入力を受けたオブジェクトとの相対座標)</returns>
        public static Vector3 GetLocalPoint(PointerEventData eventData)
        {
            Vector2 position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    eventData.pointerPress.transform as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out position);

            return position;
        }

        /// <summary>
        /// PointerEventData.positionを対象オブジェクト内のローカル空間の座標に変換して取得します。
        /// </summary>
        /// <param name="eventData">入力イベント情報</param>
        /// <param name="target">対象オブジェクトのRectTransform</param>
        /// <returns>ローカル座標(対象オブジェクトとの相対座標)</returns>
        public static Vector3 GetLocalPoint(PointerEventData eventData, RectTransform target)
        {
            Vector2 position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target,
                eventData.position,
                eventData.pressEventCamera,
                out position);
            
            return position;
        }

#endregion
    }
}
