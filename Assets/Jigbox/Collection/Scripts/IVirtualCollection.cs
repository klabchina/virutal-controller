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
using System;
using System.Collections.Generic;

namespace Jigbox.Collection
{
    public interface IVirtualCollection
    {
#region Properties

        /// <summary>
        /// 外周の余白を参照/指定します
        /// </summary>
        /// <value>The padding.</value>
        Padding Padding { get; set; }

        /// <summary>
        /// Viewportの面積を参照/指定します
        /// </summary>
        /// <value>The size of the viewport.</value>
        Vector2 ViewportSize { get; set; }

        /// <summary>
        /// 管理されているセルのリストを示します
        /// </summary>
        /// <value>The managed cells.</value>
        List<IndexedRectTransform> ManagedCells { get; }

        /// <summary>
        /// 仮想的に計算する為のセルの総数を参照/指定します
        /// </summary>
        /// <value>The virtual cell count.</value>
        int VirtualCellCount { get; set; }

#endregion

#region Information Methods

        /// <summary>
        /// Viewportから視認できるセルの総数を返します
        /// </summary>
        /// <returns>The cell count.</returns>
        /// <param name="extendable">If set to <c>true</c> extendable.</param>
        int VisibleCellCount(bool extendable);

        /// <summary>
        /// 指定したセルを示す<see cref="IndexedRectTransform"/>インスタンスが管理下に含まれているかを返します
        /// </summary>
        /// <param name="cell">Cell.</param>
        bool Contains(IndexedRectTransform cell);

        /// <summary>
        /// 指定したセルを示す<see cref="RectTransform"/>インスタンスが管理下に含まれているかを返します
        /// </summary>
        /// <param name="rectTransform">Rect transform.</param>
        bool Contains(RectTransform rectTransform);

        /// <summary>
        /// 指定したインデックスに対応するセルが管理下に含まれているかを返します
        /// </summary>
        /// <param name="index">Index.</param>
        bool Contains(int index);

#endregion

#region Control Methods

        /// <summary>
        /// RectTransform を持つオブジェクトをセルとして追加します
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="cell">Cell.</param>
        IndexedRectTransform AddCell(RectTransform cell);

        /// <summary>
        /// スクロールした座標に応じてタイルの状態を同期させます
        /// </summary>
        /// <param name="position">基準点からのスクロール量を示す座標</param>
        /// <param name="callback">セルを更新する為のコールバック</param>
        void SyncScrollPosition(Vector2 position, Action<IndexedRectTransform, int> callback);

#endregion
    }

    public interface IVirtualCollection<T> : IVirtualCollection
    {
        /// <summary>
        /// セルの大きさを参照/指定します
        /// </summary>
        /// <value>The size of the cell.</value>
        T CellSize { get; set; }

        /// <summary>
        /// セルのピボットを示します
        /// </summary>
        /// <value>The cell pivot.</value>
        Vector2 CellPivot { get; set; }

        /// <summary>
        /// セル同士の間隔を参照/指定します
        /// </summary>
        /// <value>The spacing.</value>
        T Spacing { get; set; }

        /// <summary>
        /// コンテナのピボットを示します
        /// </summary>
        /// <value>The viewport pivot.</value>
        Vector2 ViewPivot { get; }

        /// <summary>
        /// コンテナ全域を表示する為に必要な面積を示します
        /// </summary>
        /// <value>The size of the content preferred.</value>
        T ContentPreferredSize { get; }

        /// <summary>
        /// 親コンテナのアンカーからの相対位置座標を元に、セルの全体からのインデックスを計算します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="position">Span.</param>
        int CellIndex(T position);

        /// <summary>
        /// セルのインデックスから、親コンテナのアンカーからの、セルの相対位置座標を計算します
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="index">Index.</param>
        T CellPosition(int index);
    }
}
