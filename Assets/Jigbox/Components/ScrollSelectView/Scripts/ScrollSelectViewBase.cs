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
using Jigbox.Delegatable;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(ScrollSelectViewInputProxy))]
    [RequireComponent(typeof(ScrollSelectViewTransition))]
    [DisallowMultipleComponent]
    public abstract class ScrollSelectViewBase : VirtualCollectionView<IScrollSelectLayout>
    {
#region inner classes, enum, and structs

        /// <summary>スクロールセレクトのループタイプ</summary>
        public enum ScrollSelectLoopType
        {
            None = 0, // ループさせない
            Loop = 1, // ループさせる
        }

#endregion

#region constants

        /// <summary>位置の補完を行うかの判定の際に速度にかける係数のデフォルト値</summary>
        protected static float DefaultVelocityToAdjustCoefficient = 1.0f / 4.5f;

#endregion

#region Serialize Fields

        /// <summary>トランジション用コンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected ScrollSelectViewTransition transition;

        /// <summary>入力関連の代行コンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected ScrollSelectViewInputProxy inputProxy;

        /// <summary>入力イベントを受ける・受けないを制御するコンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected UIControl.RaycastValidator raycastValidator;

        /// <summary>
        /// 選択されるセルが新しくなるときに呼ばれるデリゲート
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onSelect = new DelegatableList();

        /// <summary>
        /// 選択されていたセルがされなくなったときに呼ばれるデリゲート
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onDeselect = new DelegatableList();

        /// <summary>
        /// セルの移動が停止した時に呼ばれるデリゲート
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onAdjustComplete = new DelegatableList();

#endregion

#region Properties

        /// <summary>スクロールセレクトのループタイプ</summary>
        public ScrollSelectLoopType LoopType
        {
            get { return Model.LoopType; }
            set { Model.LoopType = value; }
        }

        /// <summary>
        /// 敷き詰めるセルUIの幅を参照/指定します
        /// </summary>
        /// <remarks>水平方向に並ぶ場合ならwidthとして、垂直方向に並ぶ場合ならheightとして参照されます</remarks>
        /// <value>The size of the cell.</value>
        public float CellSize
        {
            get { return Model.CellSize; }
            set { Model.CellSize = value; }
        }

        /// <summary>敷き詰めるセルUIのピボットを参照/指定します</summary>
        public Vector2 CellPivot
        {
            get { return Model.CellPivot; }
        }

        /// <summary>敷き詰めるセルUI同士の間隔を参照/指定します</summary>
        public float Spacing
        {
            get { return Model.Spacing; }
            set { Model.Spacing = value; }
        }

        /// <summary>Viewport上での選択位置を割合で参照/指定します</summary>
        public float SelectedCellPositionRate
        {
            get { return Model.SelectedCellPositionRate; }
            set { Model.SelectedCellPositionRate = value; }
        }

        /// <summary>選択されているセルのインデックス</summary>
        public int SelectedCellIndex
        {
            get { return Model.SelectedCellIndex; }
        }

        /// <summary>スクロールの速度</summary>
        protected abstract float Velocity { get; }

        /// <summary>位置の補完を行うかの判定の際に速度にかける係数</summary>
        protected virtual float VelocityToAdjustCoefficient
        {
            get { return DefaultVelocityToAdjustCoefficient; }
        }

        /// <summary>非選択状態だったセルが選択された際のコールバック</summary>
        public DelegatableList OnSelect
        {
            get { return onSelect; }
        }

        /// <summary>選択状態だったセルの選択が解除された際のコールバック</summary>
        public DelegatableList OnDeselect
        {
            get { return onDeselect; }
        }

        /// <summary>スクロールによる移動が停止した際のコールバック</summary>
        public DelegatableList OnAdjustComplete
        {
            get { return onAdjustComplete; }
        }

        /// <summary>
        /// スクロールの開始点の座標を返します
        /// </summary>
        /// <value>The scroll origin point.</value>
        public override Vector2 ScrollOriginPoint
        {
            get { return Model.ScrollOriginPoint; }
        }

        /// <summary>スクロールビューにヘッダ・フッタは無いため0とする</summary>
        protected sealed override Vector2 HeaderFooterSize
        {
            get { return Vector2.zero; }
        }

        /// <summary>スクロールビューにヘッダ・フッタは無いため0とする</summary>
        protected sealed override Vector2 HeaderSize
        {
            get { return Vector2.zero; }
        }

        /// <summary>選択されたセルのHead側にに加えられる余白を参照/指定します</summary>
        public float SelectedCellHeadSpacing
        {
            get { return Model.SelectedCellHeadSpacing; }
            set { Model.SelectedCellHeadSpacing = value; }
        }

        /// <summary>選択されたセルのTail側にに加えられる余白を参照/指定します</summary>
        public float SelectedCellTailSpacing
        {
            get { return Model.SelectedCellTailSpacing; }
            set { Model.SelectedCellTailSpacing = value; }
        }

        /// <summary>Scrollbarへの参照のキャッシュ</summary>
        protected Scrollbar cachedScrollBar;

        /// <summary>
        /// Scrollbarへ参照/指定します
        /// VerticalかHorizontalかで参照さきのScrollbarが変わります
        /// </summary>
        protected abstract Scrollbar ScrollBar { get; set; }
        
        /// <summary>ScrollBarの入力を監視するコンポーネント</summary>
        protected ScrollSelectViewScrollBarInputProxy scrollBarInputProxy;

        /// <summary>
        /// InputProxyの状態がDrag中かどうかを返します
        /// </summary>
        protected virtual bool IsDraggingInputProxy
        {
            get
            {
                if (scrollBarInputProxy == null)
                {
                    return inputProxy.IsDragging;
                }
                
                return inputProxy.IsDragging || scrollBarInputProxy.IsDragging;
            }
        }

#endregion

#region Public method

        /// <summary>
        /// シリアライズフィールドとモデル層の状態を同期させます
        /// スクロールセレクトビューではContentのSizeDeltaを求めるロジックがbaseと変わる
        /// </summary>
        public override void BatchSerializeField()
        {
            Model.Padding = padding;
            Model.ViewportSize = Viewport.rect.size;
            // base側での処理と違ってContentFitSizeDeltaをいれるだけでよい
            Content.sizeDelta = ContentFitSizeDelta;
        }

        /// <summary>
        /// 初期の表示状態に戻します
        /// スクロールセレクトビューにおいては、indexが0のセルを選択セルとした状態にします
        /// </summary>
        public override void MoveToOriginPoint()
        {
            RelayoutByIndex(0);
        }

        /// <summary>
        /// ビューをセルで埋めます
        /// </summary>
        /// <param name="generator">Generator.</param>
        public override void FillCells(Func<GameObject> generator)
        {
            base.FillCells(generator);
            Model.UpdateCellIndex(cachedUpdateCell, true);
            // 選択セルのデリゲートを呼ぶ
            OnSelectCell(Model.SelectedCell);
        }

        /// <summary>
        /// セルを再配置させます
        /// </summary>
        public override void RelayoutAllCells()
        {
            // ループ設定のときはスクロールバーは表示させない
            if (ScrollBar != null && cachedScrollBar != null && LoopType == ScrollSelectLoopType.Loop)
            {
                ScrollBar = null;
                cachedScrollBar.gameObject.SetActive(false);
            }

            // ループ設定でないときは、スクロールバーが設定されていたら表示させる
            if (ScrollBar == null && cachedScrollBar != null && LoopType == ScrollSelectLoopType.None)
            {
                ScrollBar = cachedScrollBar;
                ScrollBar.gameObject.SetActive(true);
            }

            // Loop版とnonLoop版を切り替えられたときのためにContent.sizeDeltaの更新が必要
            Content.sizeDelta = ContentFitSizeDelta;
            // 初期位置に戻す
            RelayoutByIndex(0);
        }

        /// <summary>
        /// セルを再配置させると同時に引数でループ設定を指定できます。
        /// </summary>
        /// <param name="isLoop"></param>
        public virtual void RelayoutAllCells(bool isLoop)
        {
            Model.LoopType = isLoop ? ScrollSelectLoopType.Loop : ScrollSelectLoopType.None;

            RelayoutAllCells();
        }

        /// <summary>
        /// 指定されたindexの要素が選択された状態にします
        /// 必ず選択セル関連のコールバックが呼ばれます
        /// </summary>
        /// <param name="index"></param>
        public virtual void JumpByIndex(int index)
        {
            // 非アクティブのときは動作させないようにする
            // JumpByIndex自体は非アクティブ時でも正しく動作させられるが、MoveByIndexと挙動をできるだけあわせるため
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            RelayoutByIndex(index);
        }

        /// <summary>
        /// 指定されたindexの要素が指定された時間をかけて選択された状態にします
        /// </summary>
        /// <param name="index"></param>
        /// <param name="duration"></param>
        public virtual void MoveByIndex(int index, float duration)
        {
            // 非アクティブのときは動作させないようにする
            // 非アクティブ時に動作させてしまうとLateUpdateの処理が走らず選択セルが更新されないままスクロールだけ行われる
            // その場合に、アクティブにしたときに表示のずれと、OnSelectも走らずOnAdjsutCompleteで呼ばれるindexもずれているという問題が起きてしまうのを避けるため
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (!Model.HasManagedCells || VirtualCellCount <= 0)
            {
                return;
            }

            // 指定された時間が0秒以下の場合はアニメーションがいらないためJumpByIndexの処理をさせる
            if (duration <= 0f)
            {
                JumpByIndex(index);
                return;
            }

            StopAdjustTransition();
            ScrollRect.StopMovement();

            // 一番近い移動量ですむためのindexの差を求める
            int indexDiff = Model.GetNearIndexOffset(Model.SetMoveToCellIndex(index));
            Vector3 distance = CalculateChangeSelectCellDistance(indexDiff);
            AdjustSelectedCell(distance, duration);
            // MoveByIndexによるトランジション中はユーザからの入力を受け付けないようにする
            if (raycastValidator != null)
            {
                raycastValidator.IsValid = false;
            }
        }

        /// <summary>
        /// 現在の選択セルからoffsetIndex分セルを移動させます。
        /// ループじゃない場合は0 ~ セルの最大値までに限られます。
        /// </summary>
        /// <param name="offsetIndex"></param>
        /// <param name="duration"></param>
        public virtual void MoveByOffset(int offsetIndex, float duration)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (!Model.HasManagedCells || VirtualCellCount <= 0)
            {
                return;
            }

            var validOffset = Model.GetValidCellOffset(offsetIndex);

            // 移動するセルの数が0の場合は何もしない
            if (validOffset == 0)
            {
                return;
            }

            // 指定された時間が0秒以下の場合はアニメーションがいらないためJumpByIndexの処理をさせる
            if (duration <= 0f)
            {
                JumpByIndex(Model.SelectedCellIndex + validOffset);
                return;
            }

            StopAdjustTransition();
            ScrollRect.StopMovement();

            Vector3 distance = CalculateChangeSelectCellDistance(validOffset);
            AdjustSelectedCell(distance, duration);

            // MoveByIndexによるトランジション中はユーザからの入力を受け付けないようにする
            if (raycastValidator != null)
            {
                raycastValidator.IsValid = false;
            }
        }

        /// <summary>
        /// 選択セルを更新する際のコールバックを追加します
        /// </summary>
        /// <param name="callback"></param>
        public void AddSelectEvent(VirtualCollectionUpdateDelegate.Callback callback)
        {
            onSelect.Add(new VirtualCollectionUpdateDelegate(callback));
        }

        /// <summary>
        /// 選択セルが選択セルではなくなる際のコールバックを追加します
        /// </summary>
        /// <param name="callback"></param>
        public void AddDeselectEvent(VirtualCollectionUpdateDelegate.Callback callback)
        {
            onDeselect.Add(new VirtualCollectionUpdateDelegate(callback));
        }

        /// <summary>
        /// 選択セルが自動補完による移動が終わった際のコールバックを追加します
        /// </summary>
        /// <param name="callback"></param>
        public void AddAdjustCompleteEvent(VirtualCollectionUpdateDelegate.Callback callback)
        {
            onAdjustComplete.Add(new VirtualCollectionUpdateDelegate(callback));
        }

        /// <summary>
        /// 無限スクロール時に端に行かないようにContentとセルの位置を移動させる
        /// </summary>
        public virtual void SlideContent()
        {
            // ループしない版のときは何もしない
            if (LoopType == ScrollSelectLoopType.None)
            {
                return;
            }

            // Contentが移動する距離を求めておく
            Vector3 slideDistance = new Vector3(ScrollOriginPoint.x, ScrollOriginPoint.y) - Content.localPosition;
            // モデル側のScrollPositionの更新
            Model.SlideScrollPosition(ScrollPosition);
            Content.localPosition = ScrollOriginPoint;

            // Contentを移動した分セルも移動させる
            foreach (var cell in Model.ManagedCells)
            {
                cell.RectTransform.localPosition -= slideDistance;
            }

            if (transition != null)
            {
                transition.UpdateBasePosition();
            }
        }

        /// <summary>
        /// 位置の補完が必要であれば、補完を行います。
        /// </summary>
        public virtual void AdjustPositionIfNeeded()
        {
            if (!Model.HasManagedCells || VirtualCellCount <= 0)
            {
                return;
            }

            float velocity = Velocity;
            if (velocity == 0.0f)
            {
                return;
            }

            // スクロール速度が一定以上であればAdjustしない
            if (Mathf.Abs(velocity) * VelocityToAdjustCoefficient > CellSize)
            {
                // 以下のような問題が発生していたため、例外条件を入れる。
                // 問題：LoopTypeがNoneの場合に、OnAdjustCompleteが発火しないケースがある。
                // 発生方法：速度を持ったまま慣性によるスクロールで端のセルが選択セルに切り替える。
                // 原因：端の選択セルが選ばれた時にはスクロール速度が早いためAdjustが実行されない。しかし次に切り替わるセルもうないため選択セルの更新が以降起きない。
                // そのため、Adjustが呼ばれずOnAdjsutCompleteが発火しない。
                // 対応：LoopTypeがNoneのときに慣性によるスクロールで端のセルに到達した場合は、スクロールをとめAdjustを実行する。
                if (LoopType == ScrollSelectLoopType.Loop)
                {
                    return;
                }
                if (Model.SelectedCellIndex > 0 && Model.SelectedCellIndex < Model.VirtualCellCount - 1)
                {
                    return;
                }
            }

            // スクロール速度が一定以下
            // もしくはループタイプがNoneで先頭、末尾のセルが選択セルに選ばれた場合はAdjustする
            AdjustPosition();
        }

        /// <summary>
        /// 選択セルを基準に位置を補完します。
        /// </summary>
        public virtual void AdjustPosition()
        {
            if (!Model.HasManagedCells || VirtualCellCount <= 0)
            {
                return;
            }

            Model.SetMoveToCellIndex(Model.SelectedCellIndex);
            ScrollRect.StopMovement();
            AdjustSelectedCell(Model.SelectedCellPositionDeltaForAdjust);
        }

        /// <summary>
        /// 位置の補完を停止します。
        /// </summary>
        public virtual void StopAdjustTransition()
        {
            if (transition != null)
            {
                transition.Stop();
            }

            if (raycastValidator != null)
            {
                raycastValidator.IsValid = true;
            }
        }

        /// <summary>
        /// <para>ワールド座標上で重なる位置にあるセルが選択セルとなるように移動させます。</para>
        /// <para>重なるセルがない場合は、選択セルの位置を補完します。</para>
        /// </summary>
        /// <param name="position">ワールド座標</param>
        public virtual void ChangeSelectCellOrAdjust(Vector3 position)
        {
            if (!Model.HasManagedCells || VirtualCellCount <= 0)
            {
                return;
            }

            var selectedCell = Model.SelectedCell;
            foreach (IndexedRectTransform cell in Model.ManagedCells)
            {
                Bounds bounds = UIControl.RectTransformUtils.GetBounds(cell.RectTransform);
                if (bounds.Contains(position))
                {
                    // LoopTypeがNoneのときはcellが範囲外のindexを指定されActiveがfalseになり表示がきられているときがある
                    // 表示されていないセルを押したときに反応しないようにActive出ないときは何もしない
                    if (!cell.RectTransform.gameObject.activeInHierarchy)
                    {
                        break;
                    }

                    var scrollSelectCell = cell as ScrollSelectRectTransform;
                    int indexDiff = scrollSelectCell.PositionIndex - selectedCell.PositionIndex;
                    Vector3 distance = CalculateChangeSelectCellDistance(indexDiff);
                    if (distance.x != 0.0f || distance.y != 0.0f)
                    {
                        Model.SetMoveToCellIndex(scrollSelectCell.Index);
                        AdjustSelectedCell(distance);
                    }

                    return;
                }
            }

            if (Model.SelectedCellDelta != 0.0f)
            {
                Model.SetMoveToCellIndex(selectedCell.Index);
                AdjustSelectedCell(Model.SelectedCellPositionDeltaForAdjust);
            }
        }

        /// <summary>
        /// 指定されたindexを選択セルindexにするために移動する必要のあるセルの数を返します。
        /// 正の整数値を返します
        /// </summary>
        /// <param name="index"></param>
        public virtual int GetMoveIndexCount(int index)
        {
            return Model.GetMoveIndexCount(index);
        }

        /// <summary>
        /// 現在の選択セルからoffsetIndex分の移動を指定された場合に実際に移動するセルの数を返します。
        /// 正の整数値を返します
        /// </summary>
        /// <param name="offsetIndex"></param>
        /// <returns></returns>
        public virtual int GetMoveOffsetCount(int offsetIndex)
        {
            return Model.GetMoveOffsetCount(offsetIndex);
        }

        /// <summary>
        /// スクロールセレクトビューにおいて使えない
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public sealed override int CellIndex(Vector2 position)
        {
            throw new NotImplementedException();
        }

        public override void ClearAllCells()
        {
            StopAdjustTransition();
            ScrollRect.StopMovement();

            base.ClearAllCells();
        }

#endregion

#region protected methods

        /// <summary>
        /// 指定されたindexの要素が選択された状態になるよう再配置します
        /// 必ず選択セル関連のコールバックが呼ばれます
        /// </summary>
        /// <param name="index"></param>
        protected virtual void RelayoutByIndex(int index)
        {
            StopAdjustTransition();
            ScrollRect.StopMovement();

            if (!Model.HasManagedCells || VirtualCellCount <= 0)
            {
                return;
            }

            // 選択セルindexの更新
            IndexedRectTransform deselectCell = Model.SetSelectedCellIndex(index);
            OnDeselectCell(deselectCell);

            Content.localPosition = Model.MoveToSelectedCell();

            Model.UpdateCellIndex(cachedUpdateCell);
            foreach (var cell in Model.ManagedCells)
            {
                UpdateCellTransform(cell);
            }

            OnSelectCell(Model.SelectedCell);

            OnCompleteAdjust();
        }

        /// <summary>
        /// セルの位置更新を行う
        /// スクロールセレクトビューではユーザに渡すindexとセルとして配置するindexを別々のものとするためここで渡されるindexは使用しない
        /// 主にVirtualCollectionView側で定義されているメソッドから呼び出されている箇所で利用される
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected override void UpdateCell(IndexedRectTransform cell, int index)
        {
            UpdateCellTransform(cell);
        }

        /// <summary>
        /// セルをインデックスに合わせて更新します。
        /// </summary>
        /// <param name="cell">対象のセル</param>
        /// <param name="index">インデックス</param>
        protected virtual void UpdateCellIndex(IndexedRectTransform cell, int index)
        {
            var active = 0 <= index && index < VirtualCellCount;
            cell.Index = index;

            var go = cell.RectTransform.gameObject;
            go.SetActive(active);

            var scrollSelecteCell = cell as ScrollSelectRectTransform;
            scrollSelecteCell.UpdateScrollSelectCellIndex();
            if (active && OnUpdateCellDelegates.Count > 0)
            {
                OnUpdateCellDelegates.Execute(go, index);
            }
        }

        /// <summary>
        /// セルが選択された際に呼び出されます。
        /// </summary>
        /// <param name="cell">選択されたセル</param>
        protected virtual void OnSelectCell(IndexedRectTransform cell)
        {
            if (OnSelect.Count > 0 && cell.RectTransform.gameObject.activeSelf)
            {
                OnSelect.Execute(cell.RectTransform.gameObject, cell.Index);
            }

            if (!IsDraggingInputProxy)
            {
                AdjustPositionIfNeeded();
            }
        }

        /// <summary>
        /// セルの選択が解除された際に呼び出されます。
        /// </summary>
        /// <param name="cell">選択が解除されたセル</param>
        protected virtual void OnDeselectCell(IndexedRectTransform cell)
        {
            if (OnDeselect.Count > 0 && cell.RectTransform.gameObject.activeSelf)
            {
                OnDeselect.Execute(cell.RectTransform.gameObject, cell.Index);
            }
        }

        /// <summary>
        /// セルの位置の補完が完了した際に呼び出されます。
        /// </summary>
        protected virtual void OnCompleteAdjust()
        {
            if (onAdjustComplete.Count > 0)
            {
                UpdateSelectIndex();
                onAdjustComplete.Execute(Model.SelectedCell.RectTransform.gameObject, Model.SelectedCellIndex);
            }
            // 機能によってはユーザの入力の受け付けないようにしているため解除する
            if (raycastValidator != null)
            {
                raycastValidator.IsValid = true;
            }
        }

        /// <summary>
        /// 選択セルが正しい位置に来るように位置を補正します。
        /// </summary>
        protected virtual void AdjustSelectedCell(Vector3 distance)
        {
            if (transition != null)
            {
                transition.Adjust(distance);
                scrollRect.velocity = Vector2.zero;
            }
        }

        /// <summary>
        /// 選択セルが正しい位置に来るように位置を補正します。
        /// 時間を指定することができます。
        /// </summary>
        protected virtual void AdjustSelectedCell(Vector3 distance, float duration)
        {
            if (transition != null)
            {
                transition.Adjust(distance, duration);
                scrollRect.velocity = Vector2.zero;
            }
        }

        /// <summary>
        /// 移動予定のセルがある場合は移動先のセルに対してRelayoutを行い
        /// Scroll中の場合は現在の選択セルに対してRelayoutを行います
        /// </summary>
        protected virtual void RelayoutMoveToCell()
        {
            if (transition != null)
            {
                if (transition.IsWorkingAdjust)
                {
                    RelayoutByIndex(Model.MoveToCellIndex);
                    
                    return;
                }
            }

            if (Model.SelectedCellDelta != 0.0f)
            {
                RelayoutByIndex(Model.SelectedCellIndex);
            }
        }

        protected virtual void UpdateSelectIndex()
        {
            // ScrollSelect用のSyncScrollPositionを使う
            if (Model.SyncScrollPosition(ScrollPosition))
            {
                // 選択セルindexの更新
                IndexedRectTransform deselectedCell = Model.UpdateSelectedCellIndex();
                if (deselectedCell != null)
                {
                    OnDeselectCell(deselectedCell);
                }

                Model.UpdateCellIndex(cachedUpdateCell);

                foreach (IndexedRectTransform cell in Model.ManagedCells)
                {
                    UpdateCellTransform(cell);
                }

                // SelectedCellに更新があった場合はイベントを呼ぶ
                if (deselectedCell != null)
                {
                    OnSelectCell(Model.SelectedCell);
                }
            }
        }

        /// <summary>
        /// スクロール位置を補正します。
        /// </summary>
        protected abstract void CorrectScrollPosition();

        /// <summary>
        /// 選択セルからindexOffset分離れたセルの距離を求めます。
        /// </summary>
        /// <param name="indexOffset"></param>
        /// <returns></returns>
        protected abstract Vector3 CalculateChangeSelectCellDistance(int indexOffset);

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            if (transition != null)
            {
                transition.Init(Content, OnCompleteAdjust);
            }

            inputProxy.Init(this);

            // スクロールセレクトビューのMovementTypeはClampedにする
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            // ScrollBarへの参照をcacheする
            // 非表示にするためにはScrollRectのScrollbarへの参照をnullにする必要がり、null設定を元に戻せるようにするため
            cachedScrollBar = ScrollBar;

            // ScrollBarInputProxyの参照を取得する
            // ScrollBarInputProxyは後から追加されたため、参照を動的に追加する方法で既存実装に影響を与えないように実装しています。
            if (cachedScrollBar != null)
            {
                scrollBarInputProxy = cachedScrollBar.gameObject.GetComponent<ScrollSelectViewScrollBarInputProxy>();
            }
            
            // ScrollBarにInputProxyが設定されていない場合、AddComponentをする
            if (scrollBarInputProxy == null && cachedScrollBar != null)
            {
                scrollBarInputProxy = cachedScrollBar.gameObject.AddComponent<ScrollSelectViewScrollBarInputProxy>();
            }
            
            if (scrollBarInputProxy != null)
            {
                scrollBarInputProxy.Init(this);
            }
        }

        /// <summary>
        /// Base側の処理でやっているもののうちいくつか無駄なものを省いた処理を行うようoverrideする
        /// </summary>
        protected override void OnEnable()
        {
            if (!isInitialized)
            {
                BatchSerializeField();
                UpdateScrollingEnabled();
                CatchUpExistCells();
                // Scene上に存在するヘッダ・フッタ用オブジェクトのチェックはOnEnable時に一度だけ走らせる
                isInitialized = true;
                // LateUpdateでGCAllocが発生しないようにActionにCacheさせる
                cachedUpdateCell = UpdateCellIndex;
            }
        }

        protected override void OnDisable()
        {
            RelayoutMoveToCell();
            inputProxy.Refresh();

            if (scrollBarInputProxy != null)
            {
                scrollBarInputProxy.Refresh();
            }
        }

        protected override void LateUpdate()
        {
            // RectTransformの基本計算は、OnEnable終了時点で完了しているが、
            // その後、CanvasScalerによる補正がUpdate中に行われるため、
            // その両方が確実に終わっているLateUpdateのタイミングで処理する
            if (requestHandler != null)
            {
                requestHandler();
                requestHandler = null;
            }

            UpdateSelectIndex();

            if (!IsDraggingInputProxy && Model.HasManagedCells && VirtualCellCount > 0)
            {
                CorrectScrollPosition();
            }
        }

#endregion
    }
}
