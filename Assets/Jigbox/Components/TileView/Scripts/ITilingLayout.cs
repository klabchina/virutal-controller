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
using Jigbox.Collection;

namespace Jigbox.Components
{
    public interface ITilingLayout : IVirtualCollection<Vector2>
    {
        /// <summary>
        /// Viewport から視認できる行の数を示します
        /// </summary>
        /// <returns>The row count.</returns>
        /// <param name="extendable">If set to <c>true</c> extendable.</param>
        int VisibleRowCount(bool extendable);

        /// <summary>
        /// Viewport から視認できる列の数を示します
        /// </summary>
        /// <returns>The column count.</returns>
        /// <param name="extendable">If set to <c>true</c> extendable.</param>
        int VisibleColumnCount(bool extendable);

        /// <summary>
        /// セルインデックスに対応する行のインデックスを返します
        /// </summary>
        /// <returns>The index by cell index.</returns>
        /// <param name="cellIndex">Cell index.</param>
        int RowIndex(int cellIndex);

        /// <summary>
        /// セルインデックスに対応する列のインデックスを返します
        /// </summary>
        /// <returns>The index by cell index.</returns>
        /// <param name="cellIndex">Cell index.</param>
        int ColumnIndex(int cellIndex);

        /// <summary>
        /// 行列のそれぞれのインデックスから、セルの全体からのインデックスを計算します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="columnIndex">Column index.</param>
        int CellIndex(int rowIndex, int columnIndex);

        /// <summary>
        /// 行列のそれぞれのインデックスから、親コンテナのアンカーからの、セルの相対位置座標を計算します
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="rowIndex">Row.</param>
        /// <param name="columnIndex">Column.</param>
        Vector2 CellPosition(int rowIndex, int columnIndex);

        /// <summary>
        /// 行列それぞれから ViewportSize の論理サイズを更新します
        /// </summary>
        /// <returns>The matrix.</returns>
        /// <param name="rowSize">Row count.</param>
        /// <param name="columnSize">Column count.</param>
        Vector2 SetMatrix(int rowSize, int columnSize);
    }
}
