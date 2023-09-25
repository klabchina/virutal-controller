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
    public interface IBalloonLayoutProperty
    {
        /// <summary>
        /// バルーンのレイアウト位置をどこにするか決める列挙
        /// </summary>
        BalloonLayout BalloonLayout { get; }

        /// <summary>
        /// バルーンの位置計算の基準となるワールド座標
        /// </summary>
        Vector2 BasePosition { get; }

        /// <summary>
        /// バルーンの基準座標と隣り合うContentの辺のどこを中心として位置計算を行うか
        /// 0.5の場合中心、0.0と1.0の場合端を基準にして計算を行う
        /// </summary>
        float BalloonLayoutPositionRate { get; }

        /// <summary>
        /// 基準座標とバルーンの間にどれだけ空間を空けるか
        /// </summary>
        float Spacing { get; }

        /// <summary>
        /// バルーンで取り扱うオブジェクトのワールド座標
        /// </summary>
        Vector2 ContentPosition { get; }

        /// <summary>
        /// バルーンで取り扱うオブジェクトのサイズ
        /// </summary>
        Vector2 ContentSize { get; }

        /// <summary>
        /// バルーンで取り扱うオブジェクトのPivot
        /// </summary>
        Vector2 ContentPivot { get; }

        /// <summary>
        /// バルーンで取り扱うオブジェクトの実スケール
        /// </summary>
        Vector2 ContentLossyScale { get; }

        /// <summary>
        /// 自動レイアウトを行うかどうか
        /// </summary>
        bool IsAutoLayout { get; }

        /// <summary>
        /// 自動レイアウト矩形のワールド座標
        /// </summary>
        Vector2 AutoLayoutAreaPosition { get; }

        /// <summary>
        /// 自動レイアウト矩形のサイズ
        /// </summary>
        Vector2 AutoLayoutAreaSize { get; }

        /// <summary>
        /// 自動レイアウト矩形のPivot
        /// </summary>
        Vector2 AutoLayoutAreaPivot { get; }

        /// <summary>
        /// 自動レイアウト矩形の実スケール
        /// </summary>
        Vector2 AutoLayoutAreaLossyScale { get; }
    }
}
