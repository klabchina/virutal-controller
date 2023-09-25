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
using Jigbox.Delegatable;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    /// <summary>
    /// 可変長リストの基底クラス
    /// </summary>
    public abstract class VariableListBase : MonoBehaviour
    {
#region constants

        /// <summary>
        /// セルのPivot固定
        /// </summary>
        protected static readonly Vector2 cellPivot = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// セルの補完数、継ぎ目なく見えるようにセルの数を上増しします。
        /// </summary>
        protected static readonly int interpolation = 1;

#endregion

#region inner classes

        /// <summary>
        /// 仮想コレクションでの更新用デリゲート型
        /// </summary>
        public class VariableListUpdateDelegate : EventDelegate<VariableListCell>
        {
            public VariableListUpdateDelegate(Callback callback) : base(callback)
            {
            }
        }

#endregion

#region serialize fields

        /// <summary>
        /// uGUIのScrollRect
        /// </summary>
        [SerializeField]
        ScrollRect scrollRect = null;

        /// <summary>
        /// ScrollRectの参照
        /// </summary>
        protected ScrollRect ScrollRect
        {
            get
            {
                if (scrollRect == null)
                {
                    scrollRect = GetComponent<ScrollRect>();
                }

                return scrollRect;
            }
            set { scrollRect = value; }
        }

        /// <summary>
        /// Viewport
        /// </summary>
        RectTransform viewport = null;

        /// <summary>
        /// Viewportの参照
        /// </summary>
        protected RectTransform Viewport
        {
            get
            {
                if (viewport == null)
                {
                    viewport = ScrollRect.viewport;
                }

                return viewport;
            }
        }

        /// <summary>
        /// Content
        /// </summary>
        RectTransform content = null;

        /// <summary>
        /// Contentの参照
        /// </summary>
        protected RectTransform Content
        {
            get
            {
                if (content == null)
                {
                    content = ScrollRect.content;
                }

                return content;
            }
        }

        /// <summary>
        /// Viewportとセルの間にある空白のサイズ
        /// </summary>
        [SerializeField]
        [HideInInspector]
        Padding padding = new Padding();

        /// <summary>
        /// Viewportとセルの間にある空白のサイズの参照
        /// </summary>
        public Padding Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        /// <summary>
        /// セルのアップデートコールバック
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onUpdateCellDelegates = new DelegatableList();

        /// <summary>
        /// セルのアップデートコールバックの参照
        /// </summary>
        public DelegatableList OnUpdateCellDelegates
        {
            get { return onUpdateCellDelegates; }
        }

        /// <summary>
        /// セルのサイズ更新コールバック
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onUpdateCellSizeDelegates = new DelegatableList();

        /// <summary>
        /// セルのサイズ更新コールバックの参照
        /// </summary>
        public DelegatableList OnUpdateCellSizeDelegates
        {
            get { return onUpdateCellSizeDelegates; }
        }

#endregion

#region properties

        /// <summary>
        /// セルの情報リスト
        /// </summary>
        VariableListCellInfoModel model = new VariableListCellInfoModel();

        /// <summary>
        /// セルの情報リストへの参照
        /// </summary>
        protected virtual VariableListCellInfoModel Model
        {
            get { return model; }
        }

        /// <summary>
        /// セルのPrefabハッシュコードとプールを紐づけた辞書
        /// </summary>
        Dictionary<int, VariableListCellPool> cellPools = new Dictionary<int, VariableListCellPool>();

        /// <summary>
        /// セルのPrefabハッシュコードとプールを紐づけた辞書への参照
        /// </summary>
        protected virtual Dictionary<int, VariableListCellPool> CellPools
        {
            get { return cellPools; }
        }

        /// <summary>
        /// 現在Viewport内に存在するインスタンスのハッシュセット
        /// </summary>
        HashSet<VariableListCell> visibleCellHashSet = new HashSet<VariableListCell>();

        /// <summary>
        /// 現在Viewport内に存在するインスタンスのハッシュセットへの参照
        /// </summary>
        public virtual HashSet<VariableListCell> VisibleCellHashSet
        {
            get { return visibleCellHashSet; }
        }

        /// <summary>
        /// セルの入れ替え対象のリスト
        /// </summary>
        List<VariableListCell> removeTargets = new List<VariableListCell>();

        /// <summary>
        /// セルの入れ替え対象のリストへの参照
        /// </summary>
        protected List<VariableListCell> RemoveTargets
        {
            get { return removeTargets; }
        }

        /// <summary>
        /// スクロール領域の、コンテナの配置基点からの移動相対量をベクトルで返す
        /// </summary>
        public virtual Vector2 ScrollPosition
        {
            get
            {
                var parent = Content.parent as RectTransform;
                var position = Content.localPosition;
                var offsetX = Content.rect.width * Content.pivot.x - parent.rect.width * parent.pivot.x;
                var offsetY = Content.rect.height * (1.0f - Content.pivot.y) -
                              parent.rect.height * (1.0f - parent.pivot.y);

                return new Vector2(-position.x + offsetX, position.y + offsetY);
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// スクロールが利用可能かどうかを返します
        /// </summary>
        protected virtual bool IsValidScroll
        {
            get
            {
                return Viewport.rect.width < Content.rect.width ||
                       Viewport.rect.height < Content.rect.height;
            }
        }

        /// <summary>
        /// セルがViewport内で見える最大数を取得します
        /// </summary>
        /// <param name="target">セルのPrefab</param>
        /// <returns>セルが見える最大数</returns>
        protected virtual int MaxVisibleInstanceCount(VariableListCell target)
        {
            var cellCount = Model.CellCountByPrefab(target);
            var cellSize = Model.MinimumCellSizeByPrefab(target);
            var spacing = target.SpacingFront;

            // セルの理論上の最大数を出すため、Spacingの値は小さい方を取得する
            if (target.SpacingFront > target.SpacingBack)
            {
                spacing = target.SpacingBack;
            }

            var maxVisibleInstanceCount = 1;

            for (var i = 1; i <= cellCount; i++)
            {
                // 途切れなく見えるようにinterpolation分だけセルの数を上増しする
                maxVisibleInstanceCount = Mathf.Clamp(i + interpolation, 0, cellCount);

                if ((cellSize + spacing) * i > SimplifyViewportSize)
                {
                    break;
                }
            }

            return maxVisibleInstanceCount;
        }

        /// <summary>
        /// first ~ Lastで指定されたIndexのセル情報を使用してインスタンスをアップデートします
        /// </summary>
        /// <param name="firstIndex"></param>
        /// <param name="lastIndex"></param>
        protected virtual void UpdateCellsBySpan(int firstIndex, int lastIndex)
        {
            // セルの情報がない場合はセルのインスタンスも存在しないためreturn
            if (Model.Count() == 0)
            {
                return;
            }

            for (var i = firstIndex; i <= lastIndex; i++)
            {
                UpdateCellItem(Model.Get(i));
            }

            RelegateToOutOfContent();
        }

        /// <summary>
        /// セルをPoolから取得しインスタンスをアップデートします
        /// </summary>
        /// <param name="cell"></param>
        protected virtual void UpdateCellItem(VariableListCellInfo cell)
        {
            var pool = CellPools[cell.PrefabHash];
            var take = pool.Take();
            take.Index = cell.Index;
            take.CellSize = cell.Size;

            VisibleCellHashSet.Add(take);

            UpdateCellTransform(take);

            ExecuteUpdateCellEvent(take);
        }

        /// <summary>
        /// セルのTransformを初期化します
        /// </summary>
        /// <param name="rectTrans">RectTransform</param>
        /// <param name="sizeDelta">更新後のサイズ</param>
        protected virtual void InitializeCellTransform(RectTransform rectTrans, Vector2 sizeDelta)
        {
            rectTrans.pivot = cellPivot;
            rectTrans.sizeDelta = sizeDelta;
        }

        /// <summary>
        /// セルのTransformを更新します
        /// </summary>
        /// <param name="rectTrans">RectTransform</param>
        /// <param name="pos">更新後のPosition</param>
        /// <param name="sizeDelta">更新後のサイズ</param>
        protected virtual void SetCellTransform(RectTransform rectTrans, Vector2 pos, Vector2 sizeDelta)
        {
            rectTrans.anchoredPosition = pos;
            rectTrans.sizeDelta = sizeDelta;
        }

        /// <summary>
        /// セルを生成し、Content以下に配置します
        /// </summary>
        /// <param name="generator">セルの生成ファンクション</param>
        /// <returns>生成されたセルインスタンス</returns>
        protected virtual VariableListCell AddCellItem(Func<Transform,VariableListCell> generator)
        {
            var cell = generator(Content);

            if (cell == null)
            {
                Debug.LogError("please check for your prefab have a generator()");
            }

            var rt = cell.GetComponent<RectTransform>();

            if (rt == null)
            {
                Debug.LogError("please check for your prefab have a RectTransform");
            }

            InitializeCellTransform(cell);
            return cell;
        }

        /// <summary>
        /// ScrollPositionを元に、セルが画面外に移動したかどうかを取得して更新します
        /// </summary>
        /// <param name="scrollPosition">ViewportとContentの相対的な移動量</param>
        protected virtual void SyncScrollPosition(float scrollPosition)
        {
            var firstIndex = FirstIndexAtScrollPosition(scrollPosition);
            var lastIndex = LastIndexByFirstIndex(firstIndex);
            RemoveTargets.Clear();

            foreach (var instance in VisibleCellHashSet)
            {
                if (instance.Index < firstIndex || lastIndex < instance.Index)
                {
                    RemoveTargets.Add(instance);
                }
            }

            foreach (var target in RemoveTargets)
            {
                var hash = Model.Get(target.Index).PrefabHash;
                CellPools[hash].Release(target);

                VisibleCellHashSet.Remove(target);
            }

            for (var i = firstIndex; i <= lastIndex; i++)
            {
                // GC回避のためforeachで回す
                bool isContains = false;

                foreach (var cell in VisibleCellHashSet)
                {
                    if (cell.Index == i)
                    {
                        isContains = true;
                    }
                }

                if (isContains)
                {
                    continue;
                }

                UpdateCellItem(Model.Get(i));
            }

            RelegateToOutOfContent();
        }

        /// <summary>
        /// サイズが可変するセルのインスタンスを一度実体化し、セルのサイズを更新します
        /// </summary>
        protected virtual void CalculateActuallyCellSize()
        {
            foreach (var cellInfo in Model.GetCellInfos())
            {
                // サイズが可変しないセルの場合、処理をスキップ
                if (!cellInfo.IsVariable)
                {
                    continue;
                }

                var hashCode = cellInfo.PrefabHash;
                
                // プール対象ではないならプールを用意
                if (!CellPools.ContainsKey(hashCode))
                {
                    var addPool = new VariableListCellPool();
                    CellPools.Add(hashCode, addPool);
                }

                // プールから吐き出せない場合は生成してプールに追加
                if (!CellPools[hashCode].IsAvailableTake)
                {
                    var generatedCell = AddCellItem(cellInfo.CellPrefab.Generator);
                    CellPools[hashCode].Bring(generatedCell);
                }

                // プールから Take してサイズ計算して Release 
                var instance = CellPools[hashCode].Take();
                instance.Index = cellInfo.Index;

                ExecuteUpdateCellSizeEvent(instance);

                cellInfo.Size = instance.CellSize;

                CellPools[hashCode].Release(instance);
            }
        }

        /// <summary>
        /// Contentサイズをセルの情報を元に更新します
        /// </summary>
        protected virtual void CalculateContentSize()
        {
            Content.sizeDelta = ContentFitSizeDelta;
        }

        /// <summary>
        /// セルをインスタンス化して必要な数を計算しプールします
        /// </summary>
        protected virtual void PoolCells()
        {
            var cellPrefabs = Model.GetCellPrefabs();

            foreach (var prefab in cellPrefabs)
            {
                var maxVisibleCellInstanceCount = MaxVisibleInstanceCount(prefab);
                var hashCode = prefab.GetHashCode();
                var createCount = 0;

                if (!CellPools.ContainsKey(hashCode))
                {
                    var addPool = new VariableListCellPool();
                    CellPools.Add(hashCode, addPool);
                }

                var pool = CellPools[hashCode];

                createCount = maxVisibleCellInstanceCount - pool.PoolCount;

                for (var i = 0; i < createCount; i++)
                {
                    var cellInstance = AddCellItem(prefab.Generator);

                    CellPools[hashCode].Bring(cellInstance);
                }
            }
        }

        /// <summary>
        /// 現在のスクロール量から、一番初めに見えるセルのIndexを計算して取得します
        /// </summary>
        /// <param name="scrollPosition">現在のスクロール量</param>
        /// <returns>セルのIndex</returns>
        protected virtual int FirstIndexAtScrollPosition(float scrollPosition)
        {
            var firstCellInfo = Model.Get(0);
            var totalCellSize = PaddingFront + firstCellInfo.Size;
            var index = 0;
            var spacing = firstCellInfo.SpacingBack;

            if (scrollPosition < totalCellSize)
            {
                return index;
            }

            for (var i = 1; i < Model.Count(); i++)
            {
                var cellInfo = Model.Get(i);
                if (cellInfo.SpacingFront > spacing)
                {
                    spacing = cellInfo.SpacingFront;
                }

                totalCellSize += cellInfo.Size + spacing;

                if (scrollPosition < totalCellSize)
                {
                    index = i;
                    break;
                }

                spacing = cellInfo.SpacingBack;
            }

            return index;
        }

        /// <summary>
        /// 一番初めに見えるセルのIndexから、最後に見えるセルのIndexを計算して取得します
        /// </summary>
        /// <param name="firstIndex">最初に見えるセル番号</param>
        /// <returns>セルのIndex</returns>
        protected virtual int LastIndexByFirstIndex(int firstIndex)
        {
            var lastIndex = Model.Count() - 1;

            // 表示されている先頭のセルがリスト末尾の場合
            if (lastIndex == firstIndex)
            {
                return lastIndex;
            }

            // 最初のSpacingを含めると計算が正しくなくなるので初期値をマイナスにしておく
            var totalCellSize = -Model.Get(firstIndex + 1).SpacingFront;
            var index = 0;
            var spacing = 0.0f;

            // 表示されている先頭のセルは見切れている場合があるため計算に含めない
            for (var i = firstIndex + 1; i <= lastIndex; i++)
            {
                if (lastIndex == i)
                {
                    index = lastIndex;
                    break;
                }

                var cellInfo = Model.Get(i);
                if (spacing < cellInfo.SpacingFront)
                {
                    spacing = cellInfo.SpacingFront;
                }

                totalCellSize += cellInfo.Size + spacing;

                if (totalCellSize > SimplifyViewportSize)
                {
                    index = i;
                    break;
                }

                spacing = cellInfo.SpacingBack;
            }

            return index;
        }

        /// <summary>
        /// セルがViewport内に収まる数の場合、Contentの表示位置を初期位置に補正します
        /// </summary>
        protected virtual void RepositionIfNeeded()
        {
            if (IsValidScroll)
            {
                return;
            }

            JumpByIndex(0);
        }

        /// <summary>
        /// セルのアップデートコールバックを実行します
        /// </summary>
        /// <param name="cell">アップデート対象のセル</param>
        protected virtual void ExecuteUpdateCellEvent(VariableListCell cell)
        {
            if (onUpdateCellDelegates.Count > 0)
            {
                onUpdateCellDelegates.Execute(cell);
            }
        }

        /// <summary>
        /// セルのサイズ更新コールバックを実行します
        /// </summary>
        /// <param name="cell">アップデート対象のセル</param>
        protected virtual void ExecuteUpdateCellSizeEvent(VariableListCell cell)
        {
            if (onUpdateCellSizeDelegates.Count > 0)
            {
                onUpdateCellSizeDelegates.Execute(cell);
            }
        }

#endregion

#region public methods

        /// <summary>
        /// セルの情報を追加します
        /// </summary>
        /// <param name="cellPrefab">セルのPrefab</param>
        public virtual void AddCell(VariableListCell cellPrefab)
        {
            Model.Add(cellPrefab);
        }

        /// <summary>
        /// セルの情報を複数追加します
        /// </summary>
        /// <param name="cellPrefabs">セルのPrefab</param>
        public virtual void AddCells(IEnumerable<VariableListCell> cellPrefabs)
        {
            foreach (var cellPrefab in cellPrefabs)
            {
                AddCell(cellPrefab);
            }
        }

        /// <summary>
        /// 指定されたIndexにセルを挿入します
        /// </summary>
        /// <param name="index">番号</param>
        /// <param name="cellPrefab">セルのPrefab</param>
        public virtual void InsertCell(int index, VariableListCell cellPrefab)
        {
            Model.Insert(index, cellPrefab);
        }

        /// <summary>
        /// 指定されたIndexにセルを複数挿入します
        /// </summary>
        /// <param name="index">番号</param>
        /// <param name="cellPrefabs">セルのPrefab</param>
        public virtual void InsertCells(int index, IEnumerable<VariableListCell> cellPrefabs)
        {
            foreach (var cellPrefab in cellPrefabs)
            {
                InsertCell(index, cellPrefab);
                index++;
            }
        }

        /// <summary>
        /// セルを削除します
        /// </summary>
        /// <param name="index">番号</param>
        public virtual void RemoveCell(int index)
        {
            Model.Remove(index);
        }

        /// <summary>
        /// セルを複数削除します
        /// </summary>
        /// <param name="index">番号</param>
        /// <param name="count">削除数</param>
        public virtual void RemoveCells(int index, int count)
        {
            for (var i = 0; i < count; i++)
            {
                RemoveCell(index);
            }
        }

        /// <summary>
        /// セルの再構築を行います
        /// </summary>
        /// <param name="destroyCaches">再構築時にキャッシュしているセルのインスタンス破棄も行うかどうか</param>
        public virtual void RefreshCells(bool destroyCaches = false)
        {
            foreach (var pool in CellPools.Values)
            {
                if (destroyCaches)
                {
                    pool.RemoveAll();
                }
                else
                {
                    pool.ReleaseAll();
                }
            }

            VisibleCellHashSet.Clear();

            FillCells();
        }

        /// <summary>
        /// セルのインスタンスを必要数生成し、初期化します
        /// </summary>
        public virtual void FillCells()
        {
            // セルが存在しない場合、処理が行えないためスキップする
            if (Model.Count() == 0)
            {
                return;
            }

            ScrollRect.StopMovement();

            CalculateActuallyCellSize();
            PoolCells();
            CalculateContentSize();
            RepositionIfNeeded();
            UpdateScrollingEnabled();

            var firstIndex = FirstIndexAtScrollPosition(SimplifyScrollPosition);
            var lastIndex = LastIndexByFirstIndex(firstIndex);

            UpdateCellsBySpan(firstIndex, lastIndex);
        }

        /// <summary>
        /// セルの情報とインスタンスを全て削除します
        /// </summary>
        public virtual void ClearAllCells()
        {
            if (Model.Count() == 0)
            {
                return;
            }

            ScrollRect.StopMovement();

            foreach (var pool in CellPools.Values)
            {
                pool.RemoveAll();
            }

            VisibleCellHashSet.Clear();
            Model.Clear();
            CalculateContentSize();
        }

        /// <summary>
        /// 指定されたインデックスにスクロール位置をスライドします
        /// </summary>
        /// <param name="index">セル番号</param>
        /// <param name="offset">Viewport上のどこにセルを表示するか</param>
        public virtual void JumpByIndex(int index, float offset = 0.0f)
        {
            if (Model.Count() == 0)
            {
                return;
            }

            index = Mathf.Clamp(index, 0, Model.Count() - 1);

            var rate = RateByIndex(index);
            var offsetSize = ((SimplifyViewportSize / 2) - (Model.Get(index).Size / 2)) * offset;

            ScrollRect.StopMovement();

            Content.localPosition = CalculateJumpPositionByRate(rate, offsetSize);
        }

        /// <summary>
        /// 指定されたインデックスに応じた正規化された割合(0.0f 〜 1.0f)を返します
        /// </summary>
        /// <returns>正規化されたスクロール量</returns>
        /// <param name="index">セル番号</param>
        public virtual float RateByIndex(int index)
        {
            if (Model.Count() == 0)
            {
                return 0.0f;
            }

            index = Mathf.Clamp(index, 0, Model.Count() - 1);

            var y = SimplifyCellPosition(index);

            return y / SimplifyContentSize;
        }

        /// <summary>
        /// 指定された正規化された割合(0.0f 〜 1.0f)に応じたスクロール位置をスライドします
        /// </summary>
        /// <param name="rate">正規化されたスクロール量</param>
        public virtual void JumpByRate(float rate)
        {
            if (Model.Count() == 0)
            {
                return;
            }

            ScrollRect.StopMovement();

            Content.localPosition = CalculateJumpPositionByRate(rate);
        }

        /// <summary>
        /// セルのアップデートコールバックを追加します
        /// </summary>
        /// <param name="callback">アップデートコールバック</param>
        public virtual void AddUpdateCellEvent(VariableListUpdateDelegate.Callback callback)
        {
            onUpdateCellDelegates.Add(new VariableListUpdateDelegate(callback));
        }

        /// <summary>
        /// セルのサイズ更新コールバックを追加します
        /// </summary>
        /// <param name="callback">アップデートコールバック</param>
        public virtual void AddUpdateCellSizeEvent(VariableListUpdateDelegate.Callback callback)
        {
            onUpdateCellSizeDelegates.Add(new VariableListUpdateDelegate(callback));
        }

#endregion

#region abstract properties & methods

        /// <summary>
        /// Viewport中央を基準とした、Contentの現在のスクロール位置を割合で返します
        /// </summary>
        public abstract float ContentPositionRate { get; }

        /// <summary>
        /// Contentが取るべき必要サイズを返します
        /// </summary>
        public abstract float ContentPreferredSize { get; }

        /// <summary>
        /// Contentの取るべき差分サイズを返します
        /// </summary>
        protected abstract Vector2 ContentFitSizeDelta { get; }

        /// <summary>
        /// Viewportのサイズを返します
        /// </summary>
        protected abstract float SimplifyViewportSize { get; }

        /// <summary>
        /// Contentのサイズを返します
        /// </summary>
        protected abstract float SimplifyContentSize { get; }

        /// <summary>
        /// Contentの現在のスクロール量を返します
        /// </summary>
        protected abstract float SimplifyScrollPosition { get; }

        /// <summary>
        /// 前方に空ける間隔を返します
        /// </summary>
        protected abstract float PaddingFront { get; }

        /// <summary>
        ///スクロールの基準位置を返します
        /// </summary>
        protected abstract Vector2 ScrollOriginPoint { get; }

        /// <summary>
        /// 指定したIndexのセルの位置を返します
        /// </summary>
        /// <param name="index">セルの番号</param>
        /// <returns>セルの位置</returns>
        public abstract Vector2 CellPosition(int index);

        /// <summary>
        /// 指定したIndexのセルの位置を返します
        /// </summary>
        /// <param name="index">セルの番号</param>
        /// <returns>セルの位置</returns>
        protected abstract float SimplifyCellPosition(int index);

        /// <summary>
        /// セルのTransformを初期化します
        /// サイズに影響を与えるためAnchor, Pivot, SizeDelta(Padding適用分)の更新を行います
        /// Anchorの更新は継承先で行います
        /// </summary>
        /// <param name="instance">セルのインスタンス</param>
        protected abstract void InitializeCellTransform(VariableListCell instance);

        /// <summary>
        /// セルの座標を計算し、更新メソッドに渡します
        /// </summary>
        /// <param name="instance">セルのインスタンス</param>
        protected abstract void UpdateCellTransform(VariableListCell instance);

        /// <summary>
        /// Viewport領域外に使用されていないセルのインスタンスを配置します
        /// </summary>
        protected abstract void RelegateToOutOfContent();

        /// <summary>
        /// 指定された正規化された割合(0.0f ~ 1.0f)に応じたスクロール位置を返します
        /// </summary>
        /// <param name="rate">正規化されたスクロール量</param>
        /// <param name="offset">セルを表示する際の補正値</param>
        /// <returns>ジャンプ後の位置</returns>
        protected abstract Vector3 CalculateJumpPositionByRate(float rate, float offsetSize = 0.0f);

        /// <summary>
        /// スクロール方向を有効化します
        /// </summary>
        protected abstract void UpdateScrollingEnabled();

#endregion

#region override unity methods

        void LateUpdate()
        {
            if (Model.Count() == 0)
            {
                return;
            }

            SyncScrollPosition(SimplifyScrollPosition);
        }

#endregion
    }
}
