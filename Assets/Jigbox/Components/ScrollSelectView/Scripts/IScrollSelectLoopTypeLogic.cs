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
    public interface IScrollSelectLoopTypeLogic
    {
        /// <summary>
        /// スクロールの開始点の座標を返します
        /// </summary>
        Vector2 ScrollOriginPoint { get; }

        /// <summary>
        /// リスト全域の必要な大きさを返します
        /// </summary>
        /// <value>The size of the content preferred.</value>
        float ContentPreferredSize { get; }

        /// <summary>
        /// 指定されたindexからoffsetの分だけずらしたindexを返します
        /// </summary>
        /// <param name="index"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        int GetCellIndexByOffset(int index, int offset);

        /// <summary>
        /// 指定されたindexが選択セルのindexとして有効な範囲内におさまる値にします
        /// /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetValidCellIndex(int index);

        /// <summary>
        /// 指定されたindexのセルが選択状態のときのContentのポジションを返します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Vector3 CalculateJumpPositionBySelectedIndex(int index);
    }
}
