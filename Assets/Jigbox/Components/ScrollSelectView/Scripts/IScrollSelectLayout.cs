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
using Jigbox.Collection;
using UnityEngine;

namespace Jigbox.Components
{
    public interface IScrollSelectLayout : IVirtualCollection<float>
    {
        /// <summary>
        /// スクロールセレクトのループ指定
        /// </summary>
        ScrollSelectViewBase.ScrollSelectLoopType LoopType { get; set; }

        /// <summary>
        /// 可視領域の長さを返します
        /// </summary>
        float ViewportLength { get; }

        /// <summary>
        /// 選択されたセルのHead側にに加えられる余白を参照/指定します
        /// </summary>
        float SelectedCellHeadSpacing { get; set; }

        /// <summary>
        /// 選択されたセルのTail側にに加えられる余白を参照/指定します
        /// </summary>
        float SelectedCellTailSpacing { get; set; }

        /// <summary>
        /// Viewport上での選択位置を割合で参照/指定します
        /// </summary>
        float SelectedCellPositionRate { get; set; }

        /// <summary>
        /// 選択中セルのindexを参照/指定します
        /// </summary>
        int SelectedCellIndex { get; }

        /// <summary>
        /// 選択中セルを参照します
        /// </summary>
        ScrollSelectRectTransform SelectedCell { get; }

        /// <summary>
        /// 移動予定のCellのIndexを返します
        /// </summary>
        int MoveToCellIndex { get; }
        
        /// <summary>
        /// スクロールの開始点の座標を返します
        /// </summary>
        Vector2 ScrollOriginPoint { get; }

        /// <summary>
        /// 選択セルと選択位置との距離を求めるのに使用するdelta値を表現します
        /// </summary>
        float SelectedCellDelta { get; }

        /// <summary>セル同士の間隔</summary>
        float CellInterval { get; }

        /// <summary>
        /// Adjustするときに使用する選択セルとの距離を返します
        /// </summary>
        Vector2 SelectedCellPositionDeltaForAdjust { get; }

        /// <summary>
        /// セルが存在するかどうかを返します
        /// </summary>
        bool HasManagedCells { get; }

        /// <summary>
        /// 選択セル用Indexを明示的に更新します
        /// 更新前と後でセルが同じでも選択セルから外れたものとして扱います
        /// </summary>
        /// <param name="index"></param>
        /// <returns>選択から外れたセルが返されます</returns>
        IndexedRectTransform SetSelectedCellIndex(int index);

        /// <summary>
        /// スクロール量から選択中セルのIndexを更新します
        /// </summary>
        /// <returns>選択から外れたセルが返されます</returns>
        IndexedRectTransform UpdateSelectedCellIndex();

        /// <summary>
        /// スクロール位置を同期させます
        /// </summary>
        /// <param name="position">スクロール位置</param>
        /// <returns>スクロール位置が変化していれば<c>true</c>、変化していなければ<c>false</c>を返します。</returns>
        bool SyncScrollPosition(Vector2 position);

        /// <summary>
        /// 現在選択中のセルに合わせて、位置を更新します。
        /// </summary>
        /// <returns>更新後の位置</returns>
        Vector3 MoveToSelectedCell();
        
        /// <summary>
        /// 無限スクロール状態で端に到達しないようにするためにスクロール位置を調整します。
        /// </summary>
        /// <param name="scrollPosition">スクロール位置</param>
        void SlideScrollPosition(Vector2 scrollPosition);

        /// <summary>
        /// セルをContent上に置く場合のScrollPositionに影響を受ける分のoffsetを求めます
        /// </summary>
        /// <returns></returns>
        float GetOffsetScrollPosition();

        /// <summary>
        /// 論理データと実際のデータの差を中心座標から計算したoffsetを返します
        /// </summary>
        /// <returns></returns>
        float GetOffsetForShiftToCenter();

        /// <summary>
        /// 選択セル専用のスペースを適用した場合のOffsetを計算します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        float GetSelectedSpacingOffset(int index);

        /// <summary>
        /// 配置したセルを割り振られるべきindexで更新していきます
        /// 第二引数にtrueを指定した場合セルの更新が全て行われます。falseの場合はindexの更新があるセルだけ更新が行われます
        /// </summary>
        void UpdateCellIndex(Action<IndexedRectTransform, int> onUpdateCellIndex, bool forceUpdate);

        /// <summary>
        /// 配置したセルを割り振られるべきindexで更新していきます
        /// indexの更新があるセルだけ更新が行われます
        /// </summary>
        void UpdateCellIndex(Action<IndexedRectTransform, int> onUpdateCellIndex);

        /// <summary>
        /// 引数で渡されたindexを有効な範囲のindexになるようにして返します
        /// ループでない場合は数値が最小値と最大値で制限にかけられます
        /// ループの場合は最小値と最大値のindexが繋がるように数値を正規化して返します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetValidCellIndex(int index);

        /// <summary>
        /// 現在の表示中のindexから指定したindexに移動するにあたり、移動距離が近いindexの差分を返します(Loop指定されている場合はLoopも考慮されます)
        /// </summary>
        /// <param name="toIndex"></param>
        /// <returns></returns>
        int GetNearIndexOffset(int toIndex);

        /// <summary>
        /// 指定されたindexを選択セルindexにするために移動する必要のあるセルの数を返します。
        /// 正の整数値を返します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetMoveIndexCount(int index);

        /// <summary>
        /// 現在の選択セルから指定されたoffsetIndex分移動をさせようとしたときに実際に移動する数を返します。
        /// ループのときは引数の数値がそのまま返る。
        /// ループでないときに移動する量に条件がかかる。
        /// </summary>
        /// <param name="offsetIndex"></param>
        /// <returns></returns>
        int GetValidCellOffset(int offsetIndex);

        /// <summary>
        /// 現在の選択セルからoffsetIndex分の移動を指定された場合に実際に移動するセルの数を返します。
        /// 正の整数値を返します
        /// </summary>
        /// <param name="offsetIndex"></param>
        /// <returns></returns>
        int GetMoveOffsetCount(int offsetIndex);

        /// <summary>
        /// 移動先のセルのindexを有効な値にして返します。
        /// </summary>
        /// <param name="index"> 移動先のセルindex </param>
        /// <returns> 有効な値に加工されたindex </returns>
        int SetMoveToCellIndex(int index);
    }
}
