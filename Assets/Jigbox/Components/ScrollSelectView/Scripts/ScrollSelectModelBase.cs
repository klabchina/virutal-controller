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
using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.Components
{
    [Serializable]
    public abstract class ScrollSelectModelBase : IScrollSelectLayout
    {
#region Static Constants

        /// <summary>
        /// The default padding.
        /// </summary>
        public static readonly Padding DefaultPadding = Padding.zero;

        /// <summary>
        /// The default size of the cell.
        /// </summary>
        public static readonly float DefaultCellSize = 100f;

        /// <summary>
        /// The default cell pivot.
        /// </summary>
        public static readonly Vector2 DefaultCellPivot = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// The default spacing.
        /// </summary>
        public static readonly float DefaultSpacing;

#endregion

#region Abstract

        /// <summary>
        /// 可視領域のピボット(基準点)を参照/指定します
        /// </summary>
        /// <value>The view pivot.</value>
        public abstract Vector2 ViewPivot { get; set; }

        /// <summary>
        /// 座標に相当するセルの最小インデックスを返します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="position">Position.</param>
        public abstract int CellIndex(Vector2 position);

#endregion

#region Fields & Properties

        /// <summary>
        /// ループタイプを参照/指定します
        /// </summary>
        [SerializeField]
        protected ScrollSelectViewBase.ScrollSelectLoopType loopType;

        /// <summary>
        /// ループタイプを参照/指定します
        /// </summary>
        public virtual ScrollSelectViewBase.ScrollSelectLoopType LoopType
        {
            get { return loopType; }
            set
            {
                if (loopType != value)
                {
                    loopType = value;
                    loopTypeLogic = null;
                }
            }
        }

        /// <summary>
        /// LoopTypeの違いによるlogic処理を移譲するinterfaceへの参照
        /// </summary>
        protected IScrollSelectLoopTypeLogic loopTypeLogic;

        /// <summary>
        /// LoopTypeの違いによるlogic処理を移譲するinterfaceへの参照
        /// </summary>
        protected IScrollSelectLoopTypeLogic LoopTypeLogic
        {
            get
            {
                if (loopTypeLogic == null)
                {
                    loopTypeLogic = GetLoopTypeLogic();
                }

                return loopTypeLogic;
            }
        }

        /// <summary>
        /// リスト外周の余白を参照/指定します
        /// </summary>
        /// <value>The padding.</value>
        [SerializeField]
        protected Padding padding = DefaultPadding;

        /// <summary>
        /// リスト外周の余白を参照/指定します
        /// </summary>
        /// <value>The padding.</value>
        public virtual Padding Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        /// <summary>
        /// セル一つあたりの"幅"を参照/指定します
        /// </summary>
        /// <remarks>
        /// 横方向に並ぶ場合は横幅(width)、縦方向に並ぶ場合は縦幅(height)として用いられます
        /// </remarks>
        /// <value>The size of the cell.</value>
        [SerializeField]
        protected float cellSize = DefaultCellSize;

        /// <summary>
        /// セル一つあたりの"幅"を参照/指定します
        /// </summary>
        /// <remarks>
        /// 横方向に並ぶ場合は横幅(width)、縦方向に並ぶ場合は縦幅(height)として用いられます
        /// </remarks>
        /// <value>The size of the cell.</value>
        public virtual float CellSize
        {
            get { return cellSize; }
            set { cellSize = value; }
        }

        /// <summary>
        /// セルのピボット(基準点)を参照/指定します
        /// </summary>
        /// <value>The cell pivot.</value>
        public virtual Vector2 CellPivot
        {
            get { return DefaultCellPivot; }
            set
            {
#if UNITY_EDITOR || JIGBOX_DEBUG
                // スクロールセレクトビューではcellのsetterは使わせない
                UnityEngine.Assertions.Assert.IsTrue(false, "Can't set CellPivot.");
#endif
            }
        }

        /// <summary>Pivotに合わせてセルをずらす割合</summary>
        protected abstract float CellPositionOffsetRate { get; }

        /// <summary>
        /// セル同士の余白を参照/指定します
        /// </summary>
        /// <value>The spacing.</value>
        [SerializeField]
        protected float spacing = DefaultSpacing;

        /// <summary>
        /// セル同士の余白を参照/指定します
        /// </summary>
        /// <value>The spacing.</value>
        public virtual float Spacing
        {
            get { return spacing; }
            set { spacing = value; }
        }

        /// <summary>
        /// リストが表示する仮想のセル総数を参照/指定します
        /// </summary>
        /// <value>The virtual cell count.</value>
        protected int virtualCellCount = -1;

        /// <summary>
        /// リストが表示する仮想のセル総数のプロパティ
        /// </summary>
        /// <value>The virtual cell count.</value>
        public virtual int VirtualCellCount
        {
            get { return virtualCellCount; }
            set { virtualCellCount = value; }
        }

        /// <summary>
        /// リストが保持管理しているセルのリストを参照します
        /// </summary>
        /// <value>The managed cells.</value>
        readonly List<IndexedRectTransform> managedCells = new List<IndexedRectTransform>();

        /// <summary>
        /// リストが保持管理しているセルのリストを参照します
        /// </summary>
        /// <value>The managed cells.</value>
        public virtual List<IndexedRectTransform> ManagedCells
        {
            get { return managedCells; }
        }

        /// <summary>
        /// 可視領域のサイズを参照/指定します
        /// </summary>
        /// <value>The size of the viewport.</value>
        [SerializeField]
        protected Vector2 viewportSize;

        /// <summary>
        /// 可視領域のサイズを参照/指定します
        /// </summary>
        /// <value>The size of the viewport.</value>
        public Vector2 ViewportSize
        {
            get { return viewportSize; }
            set { viewportSize = value; }
        }

        /// <summary>可視領域の長さを返します</summary>
        public abstract float ViewportLength { get; }


        /// <summary>
        /// リスト全域の必要な大きさを返します
        /// </summary>
        public virtual float ContentPreferredSize
        {
            get { return LoopTypeLogic.ContentPreferredSize; }
        }

        /// <summary>
        /// 選択セルのHead側のスペース領域を参照/指定します
        /// </summary>
        [SerializeField]
        protected float selectedCellHeadSpacing = DefaultSpacing;

        /// <summary>
        /// 選択セルのHead側のスペース領域を参照/指定します
        /// </summary>
        public float SelectedCellHeadSpacing
        {
            get { return selectedCellHeadSpacing; }
            set { selectedCellHeadSpacing = value; }
        }

        /// <summary>
        /// 選択されたセルのTail側にに加えられる余白を参照/指定します
        /// </summary>
        [SerializeField]
        protected float selectedCellTailSpacing = DefaultSpacing;

        /// <summary>
        /// 選択されたセルのTail側にに加えられる余白を参照/指定します
        /// </summary>
        public float SelectedCellTailSpacing
        {
            get { return selectedCellTailSpacing; }
            set { selectedCellTailSpacing = value; }
        }

        /// <summary>
        /// 選択セルのポジションをRate指定する値
        /// Viewportに対して(0 ~ 1)の範囲で指定してください
        /// 数値が低いほどHead側に、高いほどTail側に寄せられます
        /// </summary>
        [SerializeField]
        protected float selectedCellPositionRate = 0.5f;

        /// <summary>
        /// 選択セルのポジションをRate指定する値
        /// Viewportに対して(0 ~ 1)の範囲で指定してください
        /// 数値が低いほどHead側に、高いほどTail側に寄せられます
        /// </summary>
        public float SelectedCellPositionRate
        {
            get { return selectedCellPositionRate; }
            set { selectedCellPositionRate = value; }
        }

        /// <summary>
        /// 選択セルのIndex
        /// </summary>
        public int SelectedCellIndex { get; protected set; }

        /// <summary>
        /// 選択セルのIndex
        /// </summary>
        protected ScrollSelectRectTransform cachedScrollSelectCell;

        /// <summary>
        /// 選択されているセルを返します
        /// </summary>
        public ScrollSelectRectTransform SelectedCell
        {
            get
            {
                if (cachedScrollSelectCell == null)
                {
                    foreach (var cell in ManagedCells)
                    {
                        var scrollSelectCell = cell as ScrollSelectRectTransform;
                        if (scrollSelectCell.IsSelected)
                        {
                            cachedScrollSelectCell = scrollSelectCell;
                            break;
                        }
                    }
                }

                return cachedScrollSelectCell;
            }
        }
        
        /// <summary>
        /// 移動予定のCellのIndexを返します
        /// </summary>
        public int MoveToCellIndex { get; protected set; }

        /// <summary>
        /// スクロールの開始点の座標を返します
        /// </summary>
        public Vector2 ScrollOriginPoint
        {
            get { return LoopTypeLogic.ScrollOriginPoint; }
        }

        /// <summary>スクロール位置</summary>
        public virtual Vector2 ScrollPosition { get; protected set; }

        /// <summary>スクロール量</summary>
        protected abstract float ScrollValue { get; }

        /// <summary>選択セルの現在位置と選択状態の基準位置の差</summary>
        protected Vector2 selectedCellPositionDelta = Vector2.zero;

        /// <summary>セル同士の間隔</summary>
        public float CellInterval { get { return CellSize + spacing; } }

        /// <summary>
        /// 論理データ上でのCellが置かれる範囲の真ん中のポジション
        /// </summary>
        protected virtual float CellSpaceCenterPosition
        {
            get { return CellInterval * NecessaryCellCount * 0.5f; }
        }

        /// <summary>
        /// 必要となるセルの数
        /// </summary>
        protected virtual int NecessaryCellCount
        {
            get
            {
#if UNITY_EDITOR || JIGBOX_DEBUG
                UnityEngine.Assertions.Assert.IsFalse(Mathf.Abs(CellInterval) <= 0);
#endif

                // スクロールされている途中を考慮してCellIntervaを1つ分追加した範囲
                var visibleRange = ViewportLength + CellInterval;

                // virportを埋めるのに必要なセルの数を求める
                var count = Mathf.CeilToInt(visibleRange / CellInterval);

                // 上下に1つ分ずつ追加し高速スクロールでセルがみきれるのを防ぐ
                return count + 2;
            }
        }

        /// <summary>選択セルの現在位置と選択状態の基準位置の差の大きさ</summary>
        public abstract float SelectedCellDelta { get; protected set; }

        /// <summary>
        /// 選択セルをAdjustするときに使用する選択セルとの距離を返します
        /// </summary>
        public abstract Vector2 SelectedCellPositionDeltaForAdjust { get; }

        /// <summary>選択セルの配置可能範囲の中央から選択セルの配置位置までのオフセット値</summary>
        protected virtual float SelectedCellOffsetFromCenter
        {
            get
            {
                if (ViewportLength < CellSize)
                {
                    return 0f;
                }
                return (ViewportLength - CellSize) * (SelectedCellPositionRate - 0.5f);
            }
        }

        /// <summary>
        /// セルが存在するかどうか
        /// </summary>
        public virtual bool HasManagedCells
        {
            get { return ManagedCells.Count != 0; }
        }

#endregion

#region Information API

        /// <summary>
        /// 渡されてきたIndexedRectTransformが管理下にあるかどうかを返します
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool Contains(IndexedRectTransform cell)
        {
            return cell != null && ManagedCells.Contains(cell);
        }

        /// <summary>
        /// 渡されてきたRectTransformが管理下にあるかどうかを返します
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public bool Contains(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                return false;
            }

            foreach (var cell in ManagedCells)
            {
                if (cell.RectTransform == rectTransform)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 渡されてきたIndexのセルが管理下にあるかどうか返します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Contains(int index)
        {
            foreach (var cell in ManagedCells)
            {
                if (cell.Index == index)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 渡されたインデックスに相当する座標を返します
        /// PositionIndexが渡されてくることを想定しています
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="index">Index.</param>
        public virtual float CellPosition(int index)
        {
            // Cellのpivotを踏まえての位置調整はOffsetのところで行うためここでは計算に加えない
            return CellInterval * index;
        }

        /// <summary>
        /// 表示するのに必要なセルの数を返します
        /// スクロールセレクトビューではextendable引数は機能しません
        /// </summary>
        /// <returns>The cell count.</returns>
        public virtual int VisibleCellCount(bool extendable)
        {
            return NecessaryCellCount;
        }

        /// <summary>
        /// 座標に相当するセルの最小インデックスを返します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="span">Span.</param>
        public virtual int CellIndex(float span)
        {
            if (span < CellInterval)
            {
                return 0;
            }

            return (int) Math.Floor(span / CellInterval);
        }
        

        /// <summary>
        /// ループタイプをみて使用すべきIScrollSelectLoopTypeLogicのインスタンスを返します
        /// </summary>
        protected abstract IScrollSelectLoopTypeLogic GetLoopTypeLogic();

        /// <summary>
        /// 選択セルの現在位置と基準位置の差からインデックスのオフセットを取得します。
        /// </summary>
        /// <returns></returns>
        protected int GetIndexOffsetByDelta()
        {
            // 最初の1回分は、選択セルの中央から移動が始まるため、予め0.5を加算した状態から計算する
            int indexOffset = Mathf.FloorToInt(Mathf.Abs(SelectedCellDelta) / CellInterval + 0.5f);
            // 移動方向が正の場合は、そのまま、負の場合は、オフセットの符号を反転する
            return SelectedCellDelta >= 0.0f ? indexOffset : -indexOffset;
        }

        /// <summary>
        /// 選択セルが取るべきPositionIndexを返します
        /// </summary>
        protected virtual int GetSelectedCellPositionIndex()
        {
            // 論理データ上のセル全体の中心の位置から、セルが見きれない範囲から計算された選択セルの位置分ずらして求める
            return CellIndex(CellSpaceCenterPosition + SelectedCellOffsetFromCenter);
        }

        /// <summary>
        /// セルをContent上に置く場合のScrollPositionに影響を受ける分のoffsetを求めます
        /// </summary>
        /// <returns></returns>
        public virtual float GetOffsetScrollPosition()
        {
            return ScrollValue + SelectedCellDelta;
        }

        /// <summary>
        /// 論理データと実際のデータの差を中心座標から計算したoffsetを返します
        /// </summary>
        /// <returns></returns>
        public virtual float GetOffsetForShiftToCenter()
        {
            // 論理データの位置を実際のContent上での座標に変換するため、
            // 先頭位置からのスクロール量を基準として、データと実体のズレを補正する

            float offset = 0f;
            
            // 論理データを配置する領域と実際のViewpotの領域のズレを調整
            offset -= CellSpaceCenterPosition - ViewportLength * 0.5f;

            // 論理データ上での選択セルの位置が実際に配置される位置と合うように調整
            var selectedCellPositionIndex = GetSelectedCellPositionIndex();
            var selectedCellPosition = CellPosition(selectedCellPositionIndex);
            offset += CellSpaceCenterPosition - selectedCellPosition;

            // 選択セルの配置位置が中央からズレている分を補正
            offset += SelectedCellOffsetFromCenter;

            // Pivotの影響による表示のズレを補正
            offset += CellSize * CellPositionOffsetRate;

            return offset;
        }

        /// <summary>
        /// 選択セル専用のスペースを適用した場合のOffsetを計算します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual float GetSelectedSpacingOffset(int index)
        {
            var selectedCellPositionIndex = GetSelectedCellPositionIndex();

            // 選択セルからのどれだけズレているかを割合化する
            // この割合は、絶対値が0.5以上になると選択セル自体が切り替わってしまうため、-0.5 ~ 0.5の範囲を取る
            float rate = SelectedCellDelta / CellInterval;
#if UNITY_EDITOR || JIGBOX_DEBUG
            UnityEngine.Assertions.Assert.IsTrue(rate <= 0.5f && rate >= -0.5f);
#endif

            switch (index - selectedCellPositionIndex)
            {
                // 選択セル
                case 0:
                    return -Mathf.Lerp(0.0f, SelectedCellHeadSpacing, -rate) + Mathf.Lerp(0.0f, SelectedCellTailSpacing, rate);
                // 選択セルの一つ前
                case -1:
                    return -Mathf.Lerp(0.0f, SelectedCellHeadSpacing, 1.0f - rate);
                // 選択セルの一つ後
                case 1:
                    return Mathf.Lerp(0.0f, SelectedCellTailSpacing, 1.0f + rate);
                // 上記パターン以外の全て
                default:
                    return index <= selectedCellPositionIndex ? -SelectedCellHeadSpacing : SelectedCellTailSpacing;
            }
        }

        /// <summary>
        /// 引数で渡されたindexを有効な範囲のindexになるようにして返します
        /// ループでない場合は数値が最小値と最大値で制限にかけられます
        /// ループの場合は最小値と最大値のindexが繋がるように数値を正規化して返します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual int GetValidCellIndex(int index)
        {
            return LoopTypeLogic.GetValidCellIndex(index);
        }

        /// <summary>
        /// 現在の表示中のindexから指定したindexに移動するにあたり、移動距離が近いindexの差分を返します(Loop指定されている場合はLoopも考慮されます)
        /// indexは0 ~ N(indexの最大値)が来ることを想定しています
        /// </summary>
        /// <param name="toIndex">移動したいindex</param>
        /// <returns>移動距離が近いindexの差分</returns>
        public virtual int GetNearIndexOffset(int toIndex)
        {
            int fromIndex = SelectedCellIndex;
            int cellCount = VirtualCellCount;

#if UNITY_EDITOR || JIGBOX_DEBUG
            // 範囲内のindexでない場合はAssert対象になる
            UnityEngine.Assertions.Assert.IsTrue(0 <= toIndex  && toIndex < cellCount);
#endif
            if (cellCount <= 0)
            {
                // Cellの個数が0以下の場合はoffset値を算出することができないため0で返す
                return 0;
            }

            var offset = toIndex - fromIndex;
            // ループだった場合に、指定されたindexに近い正負の向きを割り出す
            if (LoopType == ScrollSelectViewBase.ScrollSelectLoopType.Loop)
            {
                if (Mathf.Abs(offset) > cellCount * 0.5f)
                {
                    offset = offset < 0 ? offset + cellCount : offset - cellCount;
                }
            }

            return offset;
        }

        /// <summary>
        /// 指定されたindexを選択セルindexにするために移動する必要のあるセルの数を返します。
        /// 正の整数値を返します
        /// </summary>
        /// <param name="index"></param>
        public virtual int GetMoveIndexCount(int index)
        {
            return Mathf.Abs(GetNearIndexOffset(GetValidCellIndex(index)));
        }


        /// <summary>
        /// 現在の選択セルから指定されたoffsetIndex分移動をさせようとしたときに実際に移動する数を返します。
        /// ループのときは引数の数値がそのまま返る。
        /// ループでないときに移動する量に条件がかかる。
        /// </summary>
        /// <param name="offsetIndex"></param>
        /// <returns></returns>
        public virtual int GetValidCellOffset(int offsetIndex)
        {
            // ループのときは指定された分移動できるため数値をそのまま返す。
            if (LoopType == ScrollSelectViewBase.ScrollSelectLoopType.Loop)
            {
                return offsetIndex;
            }

            // ループでないときは移動できる限りがあるのでValidateにかける
            var targetIndex = GetValidCellIndex(SelectedCellIndex + offsetIndex);
            return targetIndex - SelectedCellIndex;
        }

        /// <summary>
        /// 現在の選択セルからoffsetIndex分の移動を指定された場合に実際に移動するセルの数を返します。
        /// 正の整数値を返します
        /// </summary>
        /// <param name="offsetIndex"></param>
        /// <returns></returns>
        public virtual int GetMoveOffsetCount(int offsetIndex)
        {
            return Mathf.Abs(GetValidCellOffset(offsetIndex));
        }

#endregion

#region Control API

        /// <summary>
        /// セルを追加します
        /// </summary>
        /// <param name="cellTransform"></param>
        /// <returns></returns>
        public IndexedRectTransform AddCell(RectTransform cellTransform)
        {
            var newCell = new ScrollSelectRectTransform(ManagedCells.Count, cellTransform);
            ManagedCells.Add(newCell);
            return newCell;
        }

        /// <summary>
        /// 選択セル用Indexを明示的に更新します
        /// 更新前と後でセルが同じでも選択セルから外れたものとして扱います
        /// </summary>
        /// <param name="index"></param>
        /// <returns>選択から外れたセルが返されます</returns>
        public IndexedRectTransform SetSelectedCellIndex(int index)
        {
            // 選択セルでなくなるのでフラグの更新をする
            if (SelectedCell != null)
            {
                SelectedCell.IsSelected = false;
            }

            // 選択がはずれるセルへの参照をとる。SelectedIndexが更新される前に
            var deselectCell = SelectedCell;
            // 選択セルIndexが範囲内におさまるようにする
            SelectedCellIndex = GetValidCellIndex(index);
            // 選択セルを更新したのでキャッシュしていた選択セルの情報をクリアする
            cachedScrollSelectCell = null;
            return deselectCell;
        }

        /// <summary>
        /// スクロール量から選択中セルのIndexを更新します
        /// </summary>
        /// <returns>選択から外れたセルが返されます</returns>
        public virtual IndexedRectTransform UpdateSelectedCellIndex()
        {
            int indexOffset = GetIndexOffsetByDelta();
            if (indexOffset == 0)
            {
                return null;
            }

            // スクロールさせる方向とインデックスの動きが逆なので減算する
            var deselectedCell = SetSelectedCellIndex(SelectedCellIndex - indexOffset);
            SelectedCellDelta -= CellInterval * indexOffset;
            return deselectedCell;
        }

        /// <summary>
        /// スクロール位置を同期させます。
        /// </summary>
        /// <param name="position">スクロール位置</param>
        /// <returns>スクロール位置が変化していれば<c>true</c>、変化していなければ<c>false</c>を返します。</returns>
        public virtual bool SyncScrollPosition(Vector2 position)
        {
            // セルが無いときは何もしない
            if (!HasManagedCells || VirtualCellCount <= 0)
            {
                return false;
            }
            
            // 動いてないなら処理自体が必要ないはず
            if (ScrollPosition.x == position.x && ScrollPosition.y == position.y)
            {
                return false;
            }

            // 選択セルが選択位置からどれだけ移動しているか
            selectedCellPositionDelta += ScrollPosition - position;
            ScrollPosition = position;

            // セルのスワップ確認と、PositionIndexの更新をおこなう
            SwapPositionIndex();

            return true;
        }

        /// <summary>
        /// スクロールセレクトビューでは独自のSyncScrollPositionを使用するためこちらは使いません
        /// </summary>
        /// <param name="position"></param>
        /// <param name="callback"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void SyncScrollPosition(Vector2 position, Action<IndexedRectTransform, int> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 現在選択中のセルに合わせて、位置を更新します。
        /// </summary>
        /// <returns>更新後の位置</returns>
        public abstract Vector3 MoveToSelectedCell();

        /// <summary>
        /// 無限スクロール状態で端に到達しないようにするためにスクロール位置を調整します。
        /// </summary>
        /// <param name="scrollPosition">スクロール位置</param>
        public abstract void SlideScrollPosition(Vector2 scrollPosition);

        /// <summary>
        /// セルがスワップ必要なときに各セルのPositoinIndexを更新する
        /// </summary>
        public virtual void SwapPositionIndex()
        {
            int swapIndexOffset = GetIndexOffsetByDelta();
            if (swapIndexOffset == 0)
            {
                return;
            }

            // スワップした後のPositionIndexの更新を行う
            foreach (var cell in ManagedCells)
            {
                var scrollSelectCell = cell as ScrollSelectRectTransform;
                int newIndex = (scrollSelectCell.PositionIndex + swapIndexOffset) % ManagedCells.Count;
                if (newIndex < 0)
                {
                    newIndex += ManagedCells.Count;
                }

                scrollSelectCell.PositionIndex = newIndex;
            }
        }

        /// <summary>
        /// 配置したセルを割り振られるべきindexで更新していきます
        /// 第二引数にtrueを指定した場合セルの更新が全て行われます。falseの場合はindexの更新があるセルだけ更新が行われます
        /// </summary>
        /// <param name="onUpdateCellIndex"></param>
        /// <param name="forceUpdate"></param>
        public virtual void UpdateCellIndex(Action<IndexedRectTransform, int> onUpdateCellIndex, bool forceUpdate)
        {
            var selectedCellPositionIndex = GetSelectedCellPositionIndex();
            foreach (var cell in ManagedCells)
            {
                var scrollSelectCell = cell as ScrollSelectRectTransform;
                // 選択セルからのindexのoffsetをとる
                var indexOffset = scrollSelectCell.PositionIndex - selectedCellPositionIndex;
                int nextIndex = LoopTypeLogic.GetCellIndexByOffset(SelectedCellIndex, indexOffset);
                if (cell.Index != nextIndex || forceUpdate)
                {
                    onUpdateCellIndex(cell, nextIndex);
                }

                // 選択セルの場合はフラグの更新をする
                if (scrollSelectCell.PositionIndex == selectedCellPositionIndex)
                {
                    scrollSelectCell.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// 配置したセルを割り振られるべきindexで更新していきます
        /// indexの更新があるセルだけ更新が行われます
        /// </summary>
        /// <param name="onUpdateCellIndex"></param>
        public virtual void UpdateCellIndex(Action<IndexedRectTransform, int> onUpdateCellIndex)
        {
            UpdateCellIndex(onUpdateCellIndex, false);
        }

        /// <summary>
        /// 移動先のセルのindexを有効な値にして返します。
        /// </summary>
        /// <param name="index"> 移動先のセルindex </param>
        /// <returns> 有効な値に加工されたindex </returns>
        public virtual int SetMoveToCellIndex(int index)
        {
            MoveToCellIndex = GetValidCellIndex(index);
            return MoveToCellIndex;
        }

#endregion
    }
}
