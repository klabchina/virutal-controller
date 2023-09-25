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

using System.Collections.Generic;
using Jigbox.Delegatable;
using Jigbox.UIControl;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    /// <summary>
    /// アコーディオンリスト基底クラス
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AccordionListBase : MonoBehaviour, IAccordionList
    {
#region constants

        /// <summary>セルのPivot固定</summary>
        protected static readonly Vector2 cellPivot = new Vector2(0.5f, 0.5f);

        /// <summary>セルの補完数、継ぎ目なく見えるようにセルの数を上増しします。</summary>
        protected static readonly int interpolation = 1;

#endregion

#region inner classes

        /// <summary>
        /// 仮想コレクションでの更新用デリゲート型
        /// </summary>
        public class AccordionListUpdateDelegate : EventDelegate<AccordionListCellBase, AccordionListNode>
        {
            public AccordionListUpdateDelegate(Callback callback) : base(callback)
            {
            }
        }

#endregion

#region serialize fields

        /// <summary>uGUIのScrollRect</summary>
        ScrollRect scrollRect;

        /// <summary>ScrollRectの参照</summary>
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
        }

        /// <summary>Viewport</summary>
        RectTransform viewport;

        /// <summary>Viewportの参照</summary>
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

        /// <summary>Content</summary>
        RectTransform content;

        /// <summary>Contentの参照</summary>
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

        /// <summary>チャイルドエリアセルの設置場所</summary>
        [SerializeField]
        [HideInInspector]
        RectTransform childAreaContent;

        /// <summary>チャイルドエリアセルの設置場所の参照</summary>
        public virtual RectTransform ChildAreaContent
        {
            get
            {
                if (childAreaContent == null)
                {
                    Debug.LogError("please set childAreaContent in inspector");
                }

                return childAreaContent;
            }

            set { childAreaContent = value; }
        }

        /// <summary>チャイルドエリアセルのトランジション</summary>
        AccordionListChildAreaTransitionBase childAreaTransitionBase;

        /// <summary>チャイルドエリアセルのトランジション参照</summary>
        protected virtual AccordionListChildAreaTransitionBase ChildAreaTransitionBase
        {
            get
            {
                if (childAreaTransitionBase == null)
                {
                    childAreaTransitionBase = GetComponent<AccordionListChildAreaTransitionBase>();
                }

                return childAreaTransitionBase;
            }
        }

        /// <summary>開閉トランジション</summary>
        AccordionListTransitionBase transitionBase;

        /// <summary>開閉トランジション</summary>
        protected virtual AccordionListTransitionBase TransitionBase
        {
            get
            {
                if (transitionBase == null)
                {
                    transitionBase = GetComponent<BasicAccordionListTransition>();
                    if (transitionBase != null)
                    {
                        transitionBase.SetHandler(TransitionHandler);
                    }
                }

                return transitionBase;
            }
        }

        /// <summary>トランジションハンドラ</summary>
        AccordionListTransitionHandlerBase transitionHandler;

        /// <summary>トランジションハンドラの参照</summary>
        protected virtual AccordionListTransitionHandlerBase TransitionHandler
        {
            get
            {
                if (transitionHandler == null)
                {
                    transitionHandler = GetComponent<AccordionListTransitionHandlerBase>();
                    if (transitionHandler != null)
                    {
                        transitionHandler.Init(ClippingArea, NotClippingArea, Content, OnCompleteExpand, OnCompleteCollapse);
                    }
                }

                return transitionHandler;
            }
        }

        /// <summary>トランジション時のクリッピング領域</summary>
        [SerializeField]
        [HideInInspector]
        RectTransform clippingArea;

        /// <summary>トランジション時のクリッピング領域</summary>
        public virtual RectTransform ClippingArea
        {
            get
            {
                if (clippingArea == null)
                {
                    Debug.LogError("please set clippingArea in inspector");
                }

                return clippingArea;
            }
            set { clippingArea = value; }
        }

        /// <summary>トランジション時のクリッピング無し領域</summary>
        [SerializeField]
        [HideInInspector]
        RectTransform notClippingArea;

        /// <summary>トランジション時のクリッピング無し領域の参照</summary>
        public virtual RectTransform NotClippingArea
        {
            get
            {
                if (notClippingArea == null)
                {
                    Debug.LogError("please set notClippingArea in inspector");
                }

                return notClippingArea;
            }
            set { notClippingArea = value; }
        }

        /// <summary>入力イベントを受ける・受けないを制御するコンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        RaycastValidator raycastValidator;

        /// <summary>入力イベントを受ける・受けないを制御するコンポーネントの参照</summary>
        public virtual RaycastValidator RaycastValidator
        {
            get
            {
                if (raycastValidator == null)
                {
                    Debug.LogError("please AddComponent RaycastValidator in inspector");
                }

                return raycastValidator;
            }
            set { raycastValidator = value; }
        }

        /// <summary>Viewportとセルの間にある空白のサイズ</summary>
        [SerializeField]
        [HideInInspector]
        Padding padding;

        /// <summary>Viewportとセルの間にある空白のサイズの参照</summary>
        public Padding Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        /// <summary>シングルモード</summary>
        [SerializeField]
        [HideInInspector]
        bool isSingleMode;

        /// <summary>シングルモードの参照</summary>
        public bool IsSingleMode
        {
            get { return isSingleMode; }
            set
            {
                isSingleMode = value;
                Model.SetSingleMode(isSingleMode);
            }
        }

        /// <summary>セルのアップデートコールバック</summary>
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onUpdateCellDelegates = new DelegatableList();

        /// <summary>セルのアップデートコールバックの参照</summary>
        public DelegatableList OnUpdateCellDelegates
        {
            get { return onUpdateCellDelegates; }
        }

        /// <summary>セルのサイズ更新コールバック</summary>
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onUpdateCellSizeDelegates = new DelegatableList();

        /// <summary>セルのサイズ更新コールバックの参照</summary>
        public DelegatableList OnUpdateCellSizeDelegates
        {
            get { return onUpdateCellSizeDelegates; }
        }

        /// <summary>ノードを開く時の開始時コールバック</summary>
        [HideInInspector]
        [SerializeField]
        DelegatableList onStartExpandDelegates = new DelegatableList();

        /// <summary>ノードを開く時の開始時コールバックの参照</summary>
        public DelegatableList OnStartExpandDelegates
        {
            get { return onStartExpandDelegates; }
        }

        /// <summary>ノードを開く時の終了時コールバック</summary>
        [HideInInspector]
        [SerializeField]
        DelegatableList onCompleteExpandDelegates = new DelegatableList();

        /// <summary>ノードを開く時の終了時コールバックの参照</summary>
        public DelegatableList OnCompleteExpandDelegates
        {
            get { return onCompleteExpandDelegates; }
        }

        /// <summary>ノードを閉じる時の開始時コールバック</summary>
        [HideInInspector]
        [SerializeField]
        DelegatableList onStartCollapseDelegates = new DelegatableList();

        /// <summary>ノードを閉じる時の開始時コールバックの参照</summary>
        public DelegatableList OnStartCollapseDelegates
        {
            get { return onStartCollapseDelegates; }
        }

        /// <summary>ノードを閉じる時の終了時コールバック</summary>
        [HideInInspector]
        [SerializeField]
        DelegatableList onCompleteCollapseDelegates = new DelegatableList();

        /// <summary>ノードを閉じる時の終了時コールバックの参照</summary>
        public DelegatableList OnCompleteCollapseDelegates
        {
            get { return onCompleteCollapseDelegates; }
        }

#endregion

#region field & properties

        /// <summary>トランジション中か</summary>
        public virtual bool IsTransition { get; protected set; }

        /// <summary>セルのPrefabハッシュコードとプールを紐づけた辞書</summary>
        readonly Dictionary<int, AccordionListCellPool> cellPools = new Dictionary<int, AccordionListCellPool>();

        /// <summary>セルのPrefabハッシュコードとプールを紐づけた辞書への参照</summary>
        protected virtual Dictionary<int, AccordionListCellPool> CellPools
        {
            get { return cellPools; }
        }

        /// <summary>現在Viewport内に存在するインスタンスのハッシュセット</summary>
        readonly HashSet<AccordionListCellBase> visibleCellHashSet = new HashSet<AccordionListCellBase>();

        /// <summary>現在Viewport内に存在するインスタンスのハッシュセットへの参照</summary>
        public virtual HashSet<AccordionListCellBase> VisibleCellHashSet
        {
            get { return visibleCellHashSet; }
        }

        /// <summary>セルの入れ替え対象のリスト</summary>
        readonly List<AccordionListCellBase> removeTargets = new List<AccordionListCellBase>();

        /// <summary>セルの入れ替え対象のリストへの参照</summary>
        protected List<AccordionListCellBase> RemoveTargets
        {
            get { return removeTargets; }
        }

        /// <summary>スクロール領域の、コンテナの配置基点からの移動相対量をベクトルで返す</summary>
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

        /// <summary>スクロールが利用可能かどうかを返します</summary>
        protected virtual bool IsValidScroll
        {
            get
            {
                return Viewport.rect.width < Content.rect.width ||
                       Viewport.rect.height < Content.rect.height;
            }
        }

        /// <summary>チャイルドエリアトランジション用祖先チャイルドエリア</summary>
        readonly List<AccordionListCellInfo> ancestorsChildAreaCells = new List<AccordionListCellInfo>();

        /// <summary>チャイルドエリアトランジション用祖先チャイルドエリアの参照</summary>
        protected virtual List<AccordionListCellInfo> AncestorsChildAreaCells
        {
            get { return ancestorsChildAreaCells; }
        }

        /// <summary>展開時表示領域</summary>
        protected virtual float ExpandVisibleSize
        {
            get { return SimplifyViewportSize; }
        }

#endregion

#region public methods

        /// <summary>
        /// ノード登録する
        /// </summary>
        /// <param name="nodes">ノードの配列</param>
        public virtual void AddNodeList<T>(IEnumerable<T> nodes) where T : AccordionListNode
        {
            Model.AddNodeList(nodes);
        }

        /// <summary>
        /// 指定したノードIDを開く
        /// </summary>
        /// <param name="nodeId">ノードID</param>
        /// <param name="useTransition">トランジションを行うか</param>
        public virtual void Expand(int nodeId, bool useTransition = true)
        {
            var node = Model.FindNodeWithID(nodeId);
            if (node == null)
            {
                Debug.LogWarningFormat("can't find node id {0}", nodeId.ToString());
                return;
            }

            Expand(node, useTransition);
        }

        /// <summary>
        /// 指定したノードを開く
        /// </summary>
        /// <param name="node">開きたいノード</param>
        /// <param name="useTransition">トランジションを行うか</param>
        public virtual void Expand(AccordionListNode node, bool useTransition = true)
        {
            if (IsTransition)
            {
                return;
            }

            // シングルモードの場合先に兄弟ノードで開いているノードがあれば折り畳み処理を行う
            if (IsSingleMode)
            {
                Model.CollapseBrotherNode(node);
                if (Model.RemoveCellInfos.Count > 0)
                {
                    CalculateCellPosition();
                    CalculateContentSize();
                    UpdateCollapseCells(false, null);
                    OnCompleteCollapse(null);
                }
            }

            var cellInfo = Model.FindCellInfo(node.Id);
            // 指定ノードまで開いていない場合または画面外のノードを操作する場合は画面内に入れる
            if (cellInfo.Index < 0 || !cellInfo.HasCellReference)
            {
                JumpByNodeId(node.Id, -1);
                SyncScrollPosition(SimplifyScrollPosition);
            }

            if (node.IsExpand)
            {
                return;
            }

            OnNotifyStartExpand(cellInfo);
            var optionalCell = Model.FindCellInfo(node.Id, AccordionListCellType.Optional);
            OnNotifyStartExpand(optionalCell);

            var validTransition = useTransition && TransitionBase != null;
            Model.Expand(node);
            CalculateActuallyCellSize();
            PoolCells();
            CalculateCellPosition();
            CalculateContentSize();
            RepositionIfNeeded();
            UpdateExpandCells(validTransition, cellInfo);
            RaycastValidator.IsValid = false;

            ClippingArea.SetAsLastSibling();
            if (validTransition)
            {
                StartExpandTransition(cellInfo);
            }
            else
            {
                OnCompleteExpand(cellInfo);
            }
        }


        /// <summary>
        /// ノードID指定で閉じる
        /// </summary>
        /// <param name="nodeId">ノードID</param>
        /// <param name="useTransition">トランジションの有無</param>
        public virtual void Collapse(int nodeId, bool useTransition = true)
        {
            var node = Model.FindNodeWithID(nodeId);
            Collapse(node, useTransition);
        }

        /// <summary>
        /// 指定したノードを閉じる
        /// </summary>
        /// <param name="node">閉じたいノード</param>
        /// <param name="useTransition">トランジションを行うか</param>
        public virtual void Collapse(AccordionListNode node, bool useTransition = true)
        {
            if (IsTransition || !node.IsExpand)
            {
                return;
            }

            var cellInfo = Model.FindCellInfo(node.Id);
            if (!cellInfo.HasCellReference)
            {
                JumpByNodeId(node.Id, -1.0f);
                SyncScrollPosition(SimplifyScrollPosition);
            }

            OnNotifyStartCollapse(cellInfo);
            var optionalCell = Model.FindCellInfo(cellInfo.Node.Id, AccordionListCellType.Optional);
            OnNotifyStartCollapse(optionalCell);


            var validTransition = useTransition && TransitionBase != null;

            Model.Collapse(node);
            CalculateCellPosition();
            CalculateContentSize();
            UpdateCollapseCells(validTransition, cellInfo);

            RaycastValidator.IsValid = false;
            ClippingArea.SetAsLastSibling();

            if (validTransition)
            {
                StartCollapseTransition(cellInfo);
            }
            else
            {
                OnCompleteCollapse(cellInfo);
            }
        }

        /// <summary>
        /// 全てのノードを開く
        /// </summary>
        public virtual void ExpandAll()
        {
            if (IsSingleMode)
            {
                Debug.LogWarning("can't use in single open mode");
                return;
            }

            Model.ExpandAll();

            RefreshCells();
            JumpByIndex(0);
        }

        /// <summary>
        /// 全てのノードを閉じる
        /// </summary>
        public virtual void CollapseAll()
        {
            Model.CollapseAll();

            RefreshCells();
            JumpByIndex(0);
        }

        /// <summary>
        /// ノードからセルの追加とセルインスタンスの初期化
        /// </summary>
        public virtual void FillCells()
        {
            if (Model.CellInfos.Count > 0)
            {
                RefreshCells();
            }
            else
            {
                Model.LayoutCellFromRootNode(IsSingleMode);
                LayoutCells();
            }
        }

        /// <summary>
        /// セルの再構築を行います
        /// </summary>
        /// <param name="destroyCaches">再構築時にキャッシュしているセルのインスタンス破棄も行うかどうか</param>
        public virtual void RefreshCells(bool destroyCaches = false)
        {
            Model.Refresh();
            foreach (var pool in CellPools)
            {
                if (destroyCaches)
                {
                    pool.Value.RemoveAll();
                }
                else
                {
                    pool.Value.ReleaseAll();
                }
            }

            VisibleCellHashSet.Clear();

            LayoutCells();
        }

        /// <summary>
        /// セルのアップデートコールバックを追加します
        /// </summary>
        /// <param name="callback">アップデートコールバック</param>
        public virtual void AddUpdateCellEvent(EventDelegate<AccordionListCellBase, AccordionListNode>.Callback callback)
        {
            OnUpdateCellDelegates.Add(new AccordionListUpdateDelegate(callback));
        }

        /// <summary>
        /// セルのサイズ更新コールバックを追加します
        /// </summary>
        /// <param name="callback">アップデートコールバック</param>
        public virtual void AddUpdateCellSizeEvent(EventDelegate<AccordionListCellBase, AccordionListNode>.Callback callback)
        {
            OnUpdateCellSizeDelegates.Add(new AccordionListUpdateDelegate(callback));
        }

        /// <summary>
        /// 展開開始時コールバックを追加します
        /// </summary>
        /// <param name="callback">コールバック</param>
        public virtual void AddOnStartExpandEvent(EventDelegate<AccordionListCellBase, AccordionListNode>.Callback callback)
        {
            OnStartExpandDelegates.Add(new AccordionListUpdateDelegate(callback));
        }

        /// <summary>
        /// 展開完了時コールバックを追加します
        /// </summary>
        /// <param name="callback">コールバック</param>
        public virtual void AddOnCompleteExpandEvent(EventDelegate<AccordionListCellBase, AccordionListNode>.Callback callback)
        {
            OnCompleteExpandDelegates.Add(new AccordionListUpdateDelegate(callback));
        }

        /// <summary>
        /// 折り畳み開始時コールバックを追加します
        /// </summary>
        /// <param name="callback">コールバック</param>
        public virtual void AddOnStartCollapseEvent(EventDelegate<AccordionListCellBase, AccordionListNode>.Callback callback)
        {
            OnStartCollapseDelegates.Add(new AccordionListUpdateDelegate(callback));
        }

        /// <summary>
        /// 折り畳み完了時コールバックを追加します
        /// </summary>
        /// <param name="callback">コールバック</param>
        public virtual void AddOnCompleteCollapseEvent(EventDelegate<AccordionListCellBase, AccordionListNode>.Callback callback)
        {
            OnCompleteCollapseDelegates.Add(new AccordionListUpdateDelegate(callback));
        }

        /// <summary>
        /// セルの情報とインスタンスを全て削除します
        /// </summary>
        public virtual void Clear()
        {
            if (Model.CellInfos.Count == 0)
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
        /// 指定された正規化された割合(0.0f 〜 1.0f)に応じたスクロール位置をスライドします
        /// </summary>
        /// <param name="rate">正規化されたスクロール量</param>
        public virtual void JumpByRate(float rate)
        {
            if (Model.CellInfos.Count == 0)
            {
                return;
            }

            ScrollRect.StopMovement();

            Content.localPosition = CalculateJumpPositionByRate(rate);

            var scrollPosition = SimplifyScrollPosition;
            var firstIndex = FirstIndexAtScrollPosition(scrollPosition);
            var lastIndex = LastIndexByFirstIndex(scrollPosition, firstIndex, SimplifyViewportSize);

            UpdateCellsBySpan(firstIndex, lastIndex);
        }

        /// <summary>
        /// ノードによるスクロール位置の移動
        /// </summary>
        /// <param name="nodeId">ノードID</param>
        /// <param name="viewportOffset">Viewport上の表示位置(デフォルトは中央)/-1はFront側、+1はBack側</param>
        public virtual void JumpByNodeId(int nodeId, float viewportOffset = 0f)
        {
            if (Model.CellInfos.Count == 0)
            {
                return;
            }

            var cellInfo = Model.FindCellInfo(nodeId);
            if (cellInfo == null)
            {
                Debug.LogWarningFormat("can't find node id {0}", nodeId.ToString());
                return;
            }

            ScrollRect.StopMovement();

            if (cellInfo.Index < 0)
            {
                ExpandAncestors(cellInfo);
            }

            JumpByIndex(cellInfo.Index, viewportOffset);
        }

#endregion

#region protected methods

        /// <summary>
        /// 指定したcellInfoまで展開
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        protected virtual void ExpandAncestors(AccordionListCellInfo cellInfo)
        {
            //表示されていないのでこのノードまで開く
            Model.ExpandAncestors(cellInfo.Node, IsSingleMode);
            CalculateActuallyCellSize();
            PoolCells();
            CalculateCellPosition();
            CalculateContentSize();
            if (Model.RemoveCellInfos.Count > 0)
            {
                UpdateCollapseCells(false, null);
                OnCompleteCollapse(null);
            }

            UpdateExpandCells(false, cellInfo);
            OnCompleteExpand(null);
            UpdateScrollingEnabled();
            RepositionIfNeeded();
        }

        /// <summary>
        /// 展開終了時の処理
        /// </summary>
        protected virtual void OnCompleteExpand(AccordionListCellInfo targetCellInfo)
        {
            if (childAreaTransitionBase != null)
            {
                ChildAreaTransitionBase.ForceComplete();
            }

            foreach (var cellInfo in Model.CellInfos)
            {
                if (cellInfo.HasCellReference)
                {
                    SetParentToContent(cellInfo.CellReference.RectTransform);
                }
            }

            foreach (var cellInfo in Model.ChildAreaCellInfos)
            {
                if (cellInfo.HasCellReference)
                {
                    SetParentToChildAreaContent(cellInfo.CellReference.RectTransform);
                }
            }

            RelegateToOutOfContent();
            UpdateScrollingEnabled();

            if (targetCellInfo != null)
            {
                OnNotifyCompleteExpand(targetCellInfo);
                var optionalCell = Model.FindCellInfo(targetCellInfo.Node.Id, AccordionListCellType.Optional);
                OnNotifyCompleteExpand(optionalCell);
            }

            IsTransition = false;
            RaycastValidator.IsValid = true;
        }


        /// <summary>
        /// 折り畳み完了時
        /// </summary>
        protected virtual void OnCompleteCollapse(AccordionListCellInfo targetCellInfo)
        {
            if (ChildAreaTransitionBase != null)
            {
                ChildAreaTransitionBase.ForceComplete();
            }

            foreach (var cellInfo in Model.RemoveCellInfos)
            {
                if (cellInfo.CellType == AccordionListCellType.ChildArea)
                {
                    if (cellInfo.HasCellReference)
                    {
                        SetParentToChildAreaContent(cellInfo.CellReference.RectTransform);
                    }
                }
                else
                {
                    if (cellInfo.HasCellReference)
                    {
                        SetParentToContent(cellInfo.CellReference.RectTransform);
                    }
                }

                ReleaseCell(cellInfo);
                cellInfo.Index = int.MinValue;
                cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Normal;
            }

            Model.RemoveCellInfos.Clear();

            foreach (var cellInfo in Model.CellInfos)
            {
                if (cellInfo.HasCellReference)
                {
                    SetParentToContent(cellInfo.CellReference.RectTransform);
                }
            }

            foreach (var cellInfo in Model.ChildAreaCellInfos)
            {
                if (cellInfo.HasCellReference)
                {
                    SetParentToChildAreaContent(cellInfo.CellReference.RectTransform);
                }
            }

            RelegateToOutOfContent();
            UpdateScrollingEnabled();
            RepositionIfNeeded();

            if (targetCellInfo != null)
            {
                OnNotifyCompleteCollapse(targetCellInfo);
                var optionalCell = Model.FindCellInfo(targetCellInfo.Node.Id, AccordionListCellType.Optional);
                OnNotifyCompleteCollapse(optionalCell);
            }

            IsTransition = false;
            RaycastValidator.IsValid = true;
        }


        /// <summary>
        /// クリッピング領域に親を移動する
        /// </summary>
        /// <param name="target">移動する対象</param>
        protected virtual void SetParentToClippingArea(Transform target)
        {
            target.SetParent(ClippingArea, true);
        }

        /// <summary>
        /// クリッピングなし領域に親を移動する
        /// </summary>
        /// <param name="target">移動する対象</param>
        protected virtual void SetParentToNotClippingArea(Transform target)
        {
            target.SetParent(NotClippingArea, true);
        }

        /// <summary>
        /// Content領域に親を移動する
        /// </summary>
        /// <param name="target">移動する対象</param>
        protected virtual void SetParentToContent(Transform target)
        {
            target.SetParent(Content, true);
        }

        /// <summary>
        /// ChildAreaContent領域に親を移動する
        /// </summary>
        /// <param name="target">移動する対象</param>
        protected virtual void SetParentToChildAreaContent(Transform target)
        {
            target.SetParent(ChildAreaContent, true);
        }

        /// <summary>
        /// InsertされたcellInfoとinsertされたことで影響を受けたcellInfoを更新する
        /// </summary>
        /// <param name="validTransition">トランジションの有効/無効</param>
        /// <param name="targetCellInfo">対象のセル</param>
        protected virtual void UpdateExpandCells(bool validTransition, AccordionListCellInfo targetCellInfo)
        {
            var visibleSize = ExpandVisibleSize;
            var scrollPosition = ExpandSimplifyScrollPosition(targetCellInfo);
            var firstIndex = FirstIndexAtScrollPosition(scrollPosition);
            var lastIndex = LastIndexByFirstIndex(scrollPosition, firstIndex, visibleSize);

            var screenFront = Mathf.Abs(scrollPosition);
            var screenBack = screenFront + visibleSize;
            foreach (var cellInfo in Model.ChildAreaCellInfos)
            {
                if (Model.IsContainScreen(screenFront, screenBack, cellInfo))
                {
                    UpdateCellItem(cellInfo);
                }

                if (targetCellInfo.Index <= cellInfo.Index
                    && validTransition
                    && cellInfo.HasCellReference
                    && !Model.IsContainsAncestors(targetCellInfo, cellInfo))
                {
                    UpdateCellItem(cellInfo);
                    SetParentToNotClippingArea(cellInfo.CellReference.RectTransform);
                }
            }

            bool isSetPosition = false;
            Vector2 startPos = Vector2.zero;

            foreach (var cellInfo in Model.CellInfos)
            {
                if (cellInfo.Status == AccordionListCellInfo.CellInfoStatus.Normal)
                {
                    continue;
                }

                var isClip = cellInfo.Status == AccordionListCellInfo.CellInfoStatus.Insert && validTransition;

                if (!isSetPosition && isClip)
                {
                    startPos = Model.CellFrontPosition(cellInfo, true);
                    ClippingArea.anchoredPosition = startPos;
                    isSetPosition = true;
                }

                if (isClip)
                {
                    var size = startPos - Model.CellBackPosition(cellInfo, true);
                    size.x = Mathf.Abs(size.x);
                    size.y = Mathf.Abs(size.y);
                    ClippingArea.sizeDelta = size;
                }

                var notClip = cellInfo.Status == AccordionListCellInfo.CellInfoStatus.Update && validTransition;

                if (firstIndex <= cellInfo.Index && cellInfo.Index <= lastIndex || cellInfo.HasCellReference)
                {
                    UpdateCellItem(cellInfo);

                    if (isClip && cellInfo.HasCellReference)
                    {
                        SetParentToClippingArea(cellInfo.CellReference.RectTransform);
                    }

                    if (notClip && cellInfo.HasCellReference)
                    {
                        SetParentToNotClippingArea(cellInfo.CellReference.RectTransform);
                    }
                }

                if (isClip && cellInfo.Node.HasChildAreaCell && cellInfo.Node.IsExpand)
                {
                    var childAreaCell = Model.FindCellInfo(cellInfo.Node.Id, AccordionListCellType.ChildArea);
                    if (childAreaCell.HasCellReference)
                    {
                        SetParentToClippingArea(childAreaCell.CellReference.RectTransform);
                    }
                }

                cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Normal;
            }
            
            if (!validTransition)
            {
                UpdateContentAnchoredPosition(scrollPosition);
            }
        }

        /// <summary>
        /// RemoveされたcellInfoと影響を受けたcellInfoを更新する
        /// </summary>
        /// <param name="validTransition">トランジションの有効/無効</param>
        /// <param name="targetCellInfo">対象のセル</param>
        protected virtual void UpdateCollapseCells(bool validTransition, AccordionListCellInfo targetCellInfo)
        {
            bool isSetPosition = false;
            Vector2 startPos = Vector2.zero;

            // 削除セルの更新
            if (validTransition)
            {
                foreach (var cellInfo in Model.RemoveCellInfos)
                {
                    if (cellInfo.CellType == AccordionListCellType.ChildArea)
                    {
                        if (targetCellInfo.Node.Id == cellInfo.Node.Id)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!isSetPosition)
                        {
                            startPos = Model.CellFrontPosition(cellInfo, true);
                            ClippingArea.anchoredPosition = startPos;
                            isSetPosition = true;
                        }

                        var size = startPos - Model.CellBackPosition(cellInfo, true);
                        size.x = Mathf.Abs(size.x);
                        size.y = Mathf.Abs(size.y);
                        ClippingArea.sizeDelta = size;
                    }

                    // 画面外のセル情報が削除対象の可能性がある
                    if (cellInfo.HasCellReference)
                    {
                        SetParentToClippingArea(cellInfo.CellReference.RectTransform);
                    }
                }
            }

            var scrollPosition = CollapsedSimplifyScrollPosition(targetCellInfo);

            // チャイルドエリアセルの更新
            var screenFront = Mathf.Abs(scrollPosition);
            var screenBack = screenFront + SimplifyViewportSize;
            foreach (var cellInfo in Model.ChildAreaCellInfos)
            {
                if (Model.IsContainScreen(screenFront, screenBack, cellInfo))
                {
                    UpdateCellItem(cellInfo);
                }

                if (validTransition
                    && targetCellInfo.Index <= cellInfo.Index
                    && cellInfo.HasCellReference
                    && !Model.IsContainsAncestors(targetCellInfo, cellInfo))
                {
                    UpdateCellItem(cellInfo);
                    SetParentToNotClippingArea(cellInfo.CellReference.RectTransform);
                }
            }

            // メインセルとオプショナルセルの更新
            var firstIndex = FirstIndexAtScrollPosition(scrollPosition);
            var lastIndex = LastIndexByFirstIndex(scrollPosition, firstIndex, SimplifyViewportSize);

            foreach (var cellInfo in Model.CellInfos)
            {
                if (cellInfo.Status == AccordionListCellInfo.CellInfoStatus.Normal && (cellInfo.Index < firstIndex || lastIndex < cellInfo.Index))
                {
                    continue;
                }

                var notClip = cellInfo.Status == AccordionListCellInfo.CellInfoStatus.Update && validTransition;

                if (firstIndex <= cellInfo.Index && cellInfo.Index <= lastIndex || cellInfo.HasCellReference)
                {
                    UpdateCellItem(cellInfo);

                    if (notClip && cellInfo.HasCellReference)
                    {
                        SetParentToNotClippingArea(cellInfo.CellReference.RectTransform);
                    }
                }

                cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Normal;
            }

            if (!validTransition)
            {
                UpdateContentAnchoredPosition(scrollPosition);
            }
        }

        /// <summary>
        /// 展開トランジションを開始する
        /// </summary>
        /// <param name="cellInfo">対象のセル</param>
        protected virtual void StartExpandTransition(AccordionListCellInfo cellInfo)
        {
            IsTransition = true;
            TransitionHandler.SetCellInfo(cellInfo);
            var expandTransition = ExpandClippingAreaSize;
            TransitionBase.ExpandTransition(0, expandTransition);
            AncestorsChildAreaCells.Clear();
            Model.AncestorsChildAreaCell(cellInfo, AncestorsChildAreaCells);
            ChildAreaTransitionBase.StartExpand(AncestorsChildAreaCells, Model.ChildAreaSize(cellInfo), expandTransition);
        }

        /// <summary>
        /// 折り畳みトランジションを開始する
        /// </summary>
        /// <param name="cellInfo">対象のセル</param>
        protected virtual void StartCollapseTransition(AccordionListCellInfo cellInfo)
        {
            IsTransition = true;
            TransitionHandler.SetCellInfo(cellInfo);
            TransitionBase.CollapseTransition(CollapseClippingAreaSize, 0);
            AncestorsChildAreaCells.Clear();
            Model.AncestorsChildAreaCell(cellInfo, AncestorsChildAreaCells);
            ChildAreaTransitionBase.StartCollapse(AncestorsChildAreaCells, CollapseSize);
        }

        /// <summary>
        /// セルのインスタンスを必要数生成し、初期化します
        /// </summary>
        protected virtual void LayoutCells()
        {
            // セルが存在しない場合、処理が行えないためスキップする
            if (Model.CellInfos.Count == 0)
            {
                return;
            }

            ScrollRect.StopMovement();

            CalculateActuallyCellSize();
            PoolCells();
            CalculateCellPosition();
            CalculateContentSize();
            RepositionIfNeeded();
            UpdateScrollingEnabled();

            var scrollPosition = SimplifyScrollPosition;
            var firstIndex = FirstIndexAtScrollPosition(scrollPosition);
            var lastIndex = LastIndexByFirstIndex(scrollPosition, firstIndex, SimplifyViewportSize);

            UpdateCellsBySpan(firstIndex, lastIndex);

            foreach (var cellInfo in Model.CellInfos)
            {
                cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Normal;
            }

            // トランジションキャッシュを作っておく
            var transition = TransitionBase;
        }

        /// <summary>
        /// 指定されたインデックスにスクロール位置をスライドします
        /// </summary>
        /// <param name="index">セル番号</param>
        /// <param name="viewportOffset">Viewport上の表示位置(デフォルトは中央)/-1はFront側、+1はBack側</param>
        protected virtual void JumpByIndex(int index, float viewportOffset = 0.0f)
        {
            if (Model.CellInfos.Count == 0)
            {
                return;
            }

            Content.localPosition = ContentPositionByIndex(index, viewportOffset);
        }

        /// <summary>
        /// 指定されたインデックスに応じた正規化された割合(0.0f 〜 1.0f)を返します
        /// </summary>
        /// <returns>正規化されたスクロール量</returns>
        /// <param name="index">セル番号</param>
        protected virtual float RateByIndex(int index)
        {
            if (Model.CellInfos.Count == 0)
            {
                return 0.0f;
            }

            index = Mathf.Clamp(index, 0, Model.CellInfos.Count - 1);

            var y = SimplifyCellPosition(index);
            return y / SimplifyContentSize;
        }

        /// <summary>
        /// Content座標をindexから調べる
        /// </summary>
        /// <param name="index">セル番号</param>
        /// <param name="viewportOffset">Viewport上の表示位置/-1はFront側、+1はBack側</param>
        /// <returns>Content座標</returns>
        protected virtual Vector3 ContentPositionByIndex(int index, float viewportOffset)
        {
            index = Mathf.Clamp(index, 0, Model.CellInfos.Count - 1);

            var rate = RateByIndex(index);
            var offsetSize = (SimplifyViewportSize / 2 - Model.CellInfos[index].Size / 2) * viewportOffset;

            return CalculateJumpPositionByRate(rate, offsetSize);
        }

        /// <summary>
        /// ScrollPositionを元に、セルが画面外に移動したかどうかを取得して更新します
        /// </summary>
        /// <param name="scrollPosition">ViewportとContentの相対的な移動量</param>
        protected virtual void SyncScrollPosition(float scrollPosition)
        {
            var visibleSize = SimplifyViewportSize;
            var firstIndex = FirstIndexAtScrollPosition(scrollPosition);
            var lastIndex = LastIndexByFirstIndex(scrollPosition, firstIndex, visibleSize);
            RemoveTargets.Clear();
            var screenFront = Mathf.Abs(scrollPosition);
            var screenBack = screenFront + visibleSize;

            foreach (var instance in VisibleCellHashSet)
            {
                var cellInfo = Model.FindCellInfo(instance.NodeId, instance.CellType);
                if (cellInfo.CellType == AccordionListCellType.ChildArea)
                {
                    if (!Model.IsContainScreen(screenFront, screenBack, cellInfo))
                    {
                        RemoveTargets.Add(instance);
                    }
                }
                else if (cellInfo.Index < firstIndex || lastIndex < cellInfo.Index)
                {
                    RemoveTargets.Add(instance);
                }
            }

            foreach (var target in RemoveTargets)
            {
                var cellInfo = Model.FindCellInfo(target.NodeId, target.CellType);
                ReleaseCell(cellInfo);
            }

            for (var i = firstIndex; i <= lastIndex; i++)
            {
                var cellInfo = Model.CellInfos[i];

                if (cellInfo.HasCellReference)
                {
                    continue;
                }

                UpdateCellItem(cellInfo);
            }

            // 画面内に表示されているセルのチャイルドエリアセルを表示する
            for (var i = firstIndex; i <= lastIndex; i++)
            {
                var cellInfo = Model.CellInfos[i];

                UpdateAncestorsChildAreaCells(cellInfo, screenFront, screenBack);
            }

            RelegateToOutOfContent();
        }

        /// <summary>
        /// 対象のセルから辿れる祖先のチャイルドエリアセルを表示する
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <param name="screenFront">画面上端</param>
        /// <param name="screenBack">画面末端</param>
        protected virtual void UpdateAncestorsChildAreaCells(AccordionListCellInfo cellInfo, float screenFront, float screenBack)
        {
            // ルートに辿りついたら終了
            if (cellInfo.Node.Id == int.MinValue)
            {
                return;
            }

            if (cellInfo.Node.HasChildAreaCell && cellInfo.Node.IsExpand)
            {
                var childAreaCellInfo = Model.FindCellInfo(cellInfo.Node.Id, AccordionListCellType.ChildArea);
                if (!childAreaCellInfo.HasCellReference)
                {
                    if (Model.IsContainScreen(screenFront, screenBack, childAreaCellInfo))
                    {
                        UpdateCellItem(childAreaCellInfo);
                    }
                }
            }

            UpdateAncestorsChildAreaCells(Model.FindCellInfo(cellInfo.Node.ParentId, AccordionListCellType.Main), screenFront, screenBack);
        }

        /// <summary>
        /// サイズが可変するセルのインスタンスを一度実体化し、セルのサイズを更新します
        /// </summary>
        protected virtual void CalculateActuallyCellSize()
        {
            foreach (var cellInfo in Model.CellInfos)
            {
                // サイズが可変しないセルの場合、処理をスキップ
                if (!cellInfo.IsVariable)
                {
                    continue;
                }

                if (cellInfo.Status != AccordionListCellInfo.CellInfoStatus.Insert)
                {
                    continue;
                }

                var hashCode = cellInfo.PrefabHash;

                // プール対象ではないならプールを用意
                if (!CellPools.ContainsKey(hashCode))
                {
                    var addPool = new AccordionListCellPool();
                    CellPools.Add(hashCode, addPool);
                }

                // プールから Take してサイズ計算して Release 
                var instance = TakeCell(cellInfo);
                instance.NodeId = cellInfo.Node.Id;
                UpdateCellTransformInCalculateSize(instance);

                OnNotifyUpdateCellSize(instance, cellInfo.Node);

                cellInfo.Size = instance.CellSize;

                CellPools[hashCode].Release(instance);
            }

            RelegateToOutOfContent();
        }

        /// <summary>
        /// first ~ Lastで指定されたIndexのセル情報を使用してインスタンスをアップデートします
        /// </summary>
        /// <param name="firstIndex"></param>
        /// <param name="lastIndex"></param>
        protected virtual void UpdateCellsBySpan(int firstIndex, int lastIndex)
        {
            // セルの情報がない場合はセルのインスタンスも存在しないためreturn
            if (Model.CellInfos.Count == 0)
            {
                return;
            }

            for (var i = firstIndex; i <= lastIndex; i++)
            {
                UpdateCellItem(Model.CellInfos[i]);
            }
        }


        /// <summary>
        /// セルをPoolから取得しインスタンスをアップデートします
        /// </summary>
        /// <param name="cellInfo"></param>
        protected virtual void UpdateCellItem(AccordionListCellInfo cellInfo)
        {
            if (cellInfo.CellPrefab == null)
            {
                return;
            }

            AccordionListCellBase cell;

            if (cellInfo.Status == AccordionListCellInfo.CellInfoStatus.Insert || cellInfo.CellReference == null)
            {
                cell = TakeCell(cellInfo);

                VisibleCellHashSet.Add(cell);

                if (cellInfo.CellType == AccordionListCellType.ChildArea)
                {
                    SetParentToChildAreaContent(cell.RectTransform);
                    cell.RectTransform.SetAsLastSibling();
                }
                else
                {
                    cell.RectTransform.SetSiblingIndex(cellInfo.Index + 2);
                }
            }
            else
            {
                cell = cellInfo.CellReference;
            }

            cell.SetAccordionList(this);
            cell.NodeId = cellInfo.Node.Id;
            cell.CellSize = cellInfo.Size;
            cellInfo.CellReference = cell;

#if UNITY_EDITOR
            if (cellInfo.CellType == AccordionListCellType.Main)
            {
                var mainCell = (AccordionListMainCell) cell;
                mainCell.Margin = cellInfo.Node.Margin;
                mainCell.SpacingFront = cellInfo.CurrentSpacingFront;
                mainCell.SpacingBack = cellInfo.CurrentSpacingBack;
                mainCell.ChildAreaPadding = cellInfo.Node.ChildAreaPadding;
            }
            else if (cellInfo.CellType == AccordionListCellType.Optional)
            {
                var optionalCell = (AccordionListOptionalCell) cell;
                optionalCell.Margin = cellInfo.Node.OptionalCellMargin;
            }
#endif

            UpdateCellTransform(cell, cellInfo);
            OnNotifyUpdateCell(cell, cellInfo.Node);
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
        /// 展開時のスクロール位置を返す
        /// </summary>
        /// <param name="targetCellInfo">展開するセル情報</param>
        /// <returns>スクロール位置</returns>
        protected virtual float ExpandSimplifyScrollPosition(AccordionListCellInfo targetCellInfo)
        {
            return SimplifyScrollPosition;
        }

        /// <summary>
        /// 折り畳み後のスクロール位置を返す
        /// </summary>
        /// <param name="targetCellInfo">折り畳むセル情報</param>
        /// <returns>スクロール位置</returns>
        protected virtual float CollapsedSimplifyScrollPosition(AccordionListCellInfo targetCellInfo)
        {
            var contentPreferredSize = ContentPreferredSize;
            var simplifyScrollPosition = SimplifyScrollPosition;
            var simplifyViewportSize = SimplifyViewportSize;

            // 折り畳み領域を取得
            var removeCells = Model.RemoveCellInfos;
            var firstPosition = Model.CellFrontPosition(removeCells[0], true);
            var lastPosition = Model.CellBackPosition(removeCells[removeCells.Count - 1], true);

            // スクロール位置を折り畳み領域によって更新する
            simplifyScrollPosition = UpdateScrollPositionByCollapse(firstPosition, lastPosition, simplifyScrollPosition);

            // Contentより小さい場合はそのまま
            if (contentPreferredSize > simplifyScrollPosition + simplifyViewportSize)
            {
                return simplifyScrollPosition;
            }

            // Contentより大きい場合はContentから画面サイズを引いた値
            return contentPreferredSize - simplifyViewportSize;
        }

        /// <summary>
        /// セルの表示を更新します
        /// </summary>
        /// <param name="cell">アップデート対象のセル</param>
        /// <param name="node">アップデート対象のノード</param>
        protected virtual void OnNotifyUpdateCell(AccordionListCellBase cell, AccordionListNode node)
        {
            cell.OnUpdateCell(this, node);
            if (onUpdateCellDelegates.Count > 0)
            {
                onUpdateCellDelegates.Execute(cell, node);
            }
        }

        /// <summary>
        /// セルサイズを更新します
        /// </summary>
        /// <param name="cell">アップデート対象のセル</param>
        /// <param name="node">アップデート対象のノード</param>
        protected virtual void OnNotifyUpdateCellSize(AccordionListCellBase cell, AccordionListNode node)
        {
            cell.CellSize = cell.OnUpdateCellSize(this, node);
            if (OnUpdateCellSizeDelegates.Count > 0)
            {
                OnUpdateCellSizeDelegates.Execute(cell, node);
            }
        }

        /// <summary>
        /// 展開の開始を通知する
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        protected virtual void OnNotifyStartExpand(AccordionListCellInfo cellInfo)
        {
            if (cellInfo.CellReference == null)
            {
                return;
            }

            cellInfo.CellReference.OnStartExpand(this, cellInfo.Node);
            if (OnStartExpandDelegates.Count > 0)
            {
                OnStartExpandDelegates.Execute(cellInfo.CellReference, cellInfo.Node);
            }
        }

        /// <summary>
        /// 展開の完了を通知する
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        protected virtual void OnNotifyCompleteExpand(AccordionListCellInfo cellInfo)
        {
            if (cellInfo.CellReference == null)
            {
                return;
            }

            cellInfo.CellReference.OnCompleteExpand(this, cellInfo.Node);
            if (OnCompleteExpandDelegates.Count > 0)
            {
                OnCompleteExpandDelegates.Execute(cellInfo.CellReference, cellInfo.Node);
            }
        }

        /// <summary>
        /// 折り畳みの開始を通知する
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        protected virtual void OnNotifyStartCollapse(AccordionListCellInfo cellInfo)
        {
            if (cellInfo.CellReference == null)
            {
                return;
            }

            cellInfo.CellReference.OnStartCollapse(this, cellInfo.Node);
            if (OnStartCollapseDelegates.Count > 0)
            {
                OnStartCollapseDelegates.Execute(cellInfo.CellReference, cellInfo.Node);
            }
        }

        /// <summary>
        /// 折り畳みの完了を通知する
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        protected virtual void OnNotifyCompleteCollapse(AccordionListCellInfo cellInfo)
        {
            if (cellInfo.CellReference == null)
            {
                return;
            }

            cellInfo.CellReference.OnCompleteCollapse(this, cellInfo.Node);
            if (OnCompleteCollapseDelegates.Count > 0)
            {
                OnCompleteCollapseDelegates.Execute(cellInfo.CellReference, cellInfo.Node);
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
        /// 全てのセルの座標を計算しcellInfoに設定する
        /// </summary>
        protected virtual void CalculateCellPosition()
        {
            Model.CalculateCellPosition(Padding, cellPivot);
        }

        /// <summary>
        /// セルをインスタンス化して必要な数を計算しプールします
        /// </summary>
        protected virtual void PoolCells()
        {
            foreach (var prefab in Model.CellPrefabs)
            {
                var hashCode = prefab.GetHashCode();
                if (prefab.CellType == AccordionListCellType.ChildArea)
                {
                    if (!CellPools.ContainsKey(hashCode))
                    {
                        var addPool = new AccordionListCellPool();
                        CellPools.Add(hashCode, addPool);
                    }

                    continue;
                }


                if (!CellPools.ContainsKey(hashCode))
                {
                    var addPool = new AccordionListCellPool();
                    CellPools.Add(hashCode, addPool);
                }

                var pool = CellPools[hashCode];
                var maxVisibleCellInstanceCount = MaxVisibleInstanceCount(prefab);
                var createCount = maxVisibleCellInstanceCount - pool.PoolCount;

                for (var i = 0; i < createCount; i++)
                {
                    var cellInstance = AddCellItem(prefab);
                    CellPools[hashCode].Bring(cellInstance);
                }
            }
        }

        /// <summary>
        /// セルがViewport内で見える最大数を取得します
        /// </summary>
        /// <param name="target">セルのPrefab</param>
        /// <returns>セルが見える最大数</returns>
        protected virtual int MaxVisibleInstanceCount(AccordionListCellBase target)
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
        /// セルをPoolから取得する
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <returns>セルインスタンス</returns>
        protected virtual AccordionListCellBase TakeCell(AccordionListCellInfo cellInfo)
        {
            var pool = CellPools[cellInfo.PrefabHash];
            if (!pool.IsAvailableTake)
            {
                var cellInstance = AddCellItem(cellInfo.CellPrefab);
                CellPools[cellInfo.PrefabHash].Bring(cellInstance);
            }

            return pool.Take();
        }

        /// <summary>
        /// セルをPoolに戻す
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        protected virtual void ReleaseCell(AccordionListCellInfo cellInfo)
        {
            var cell = cellInfo.CellReference;
            if (cell != null)
            {
                var hash = cellInfo.PrefabHash;
                CellPools[hash].Release(cell);
                VisibleCellHashSet.Remove(cell);
            }

            cellInfo.CellReference = null;
        }

        /// <summary>
        /// セルを生成し、Content以下に配置します
        /// </summary>
        /// <param name="cellPrefab">セルPrefab</param>
        /// <returns>生成されたセルインスタンス</returns>
        protected virtual AccordionListCellBase AddCellItem(AccordionListCellBase cellPrefab)
        {
            var cell = cellPrefab.Generate(Content);
            if (cell == null)
            {
                Debug.LogError("please check for your prefab have a generator()");
            }

            var rt = cell.RectTransform;
            if (rt == null)
            {
                Debug.LogError("please check for your prefab have a RectTransform");
            }

            RelegateToOutOfContent(cell, SimplifyViewportSize);

            return cell;
        }

        /// <summary>
        /// セルのTransformを更新します
        /// </summary>
        /// <param name="rectTrans">RectTransform</param>
        /// <param name="sizeDelta">更新後のサイズ</param>
        protected virtual void SetCellTransform(RectTransform rectTrans, Vector2 sizeDelta)
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
            rectTrans.pivot = cellPivot;
            rectTrans.sizeDelta = sizeDelta;
        }

        /// <summary>
        /// スクロール量から、一番初めに見えるセルのIndexを計算して取得します
        /// </summary>
        /// <param name="scrollPosition">スクロール量</param>
        /// <returns>セルのIndex</returns>
        protected virtual int FirstIndexAtScrollPosition(float scrollPosition)
        {
            return Model.FirstIndexAtScrollPosition(scrollPosition);
        }

        /// <summary>
        /// 一番初めに見えるセルのIndexから、最後に見えるセルのIndexを計算して取得します
        /// </summary>
        /// <param name="scrollPosition">スクロール量</param>
        /// <param name="firstIndex">最初に見えるセル番号</param>
        /// <param name="visibleSize">見える範囲</param>
        /// <returns>セルのIndex</returns>
        protected virtual int LastIndexByFirstIndex(float scrollPosition, int firstIndex, float visibleSize)
        {
            return Model.LastIndexByFirstIndex(scrollPosition, firstIndex, visibleSize);
        }

        /// <summary>
        /// Viewport領域外に使用されていないセルのインスタンスを配置します
        /// </summary>
        protected virtual void RelegateToOutOfContent()
        {
            var viewportSize = SimplifyViewportSize;
            // GC回避のためにPairで抜き出す
            foreach (var poolPair in CellPools)
            {
                var pool = poolPair.Value;
                var instances = pool.Free;

                foreach (var instance in instances)
                {
                    RelegateToOutOfContent(instance, viewportSize);
                }
            }
        }

#endregion

#region abstract properties & methods

        /// <summary>AccordionListCellInfoModelへのアクセスを返します</summary>
        protected abstract AccordionListCellInfoModel Model { get; }

        /// <summary>Viewport中央を基準とした、Contentの現在のスクロール位置を割合で返します</summary>
        public abstract float ContentPositionRate { get; }

        /// <summary>Contentが取るべき必要サイズを返します</summary>
        public abstract float ContentPreferredSize { get; }

        /// <summary>Contentの取るべき差分サイズを返します</summary>
        protected abstract Vector2 ContentFitSizeDelta { get; }

        /// <summary>Viewportのサイズを返します</summary>
        protected abstract float SimplifyViewportSize { get; }

        /// <summary>Contentのサイズを返します</summary>
        protected abstract float SimplifyContentSize { get; }

        /// <summary>Contentの現在のスクロール量を返します</summary>
        protected abstract float SimplifyScrollPosition { get; }

        /// <summary>展開時クリッピング領域</summary>
        protected abstract float ExpandClippingAreaSize { get; }

        /// <summary>折り畳み時クリッピング領域のサイズ</summary>
        protected abstract float CollapseClippingAreaSize { get; }

        /// <summary>折り畳み時に小さくなるサイズ</summary>
        protected abstract float CollapseSize { get; }

        /// <summary>前方に空ける間隔を返します</summary>
        protected abstract float PaddingFront { get; }

        /// <summary>スクロールの基準位置を返します</summary>
        protected abstract Vector2 ScrollOriginPoint { get; }

        /// <summary>
        /// 指定したIndexのセルの位置を返します
        /// </summary>
        /// <param name="index">セルの番号</param>
        /// <returns>セルの位置</returns>
        protected abstract float SimplifyCellPosition(int index);

        /// <summary>
        /// サイズ計算時のセル更新を行います
        /// サイズに影響を与えるためAnchor, Pivot, SizeDelta(Padding適用分)の更新を行います
        /// Anchorの更新は継承先で行います
        /// </summary>
        /// <param name="instance">セルのインスタンス</param>
        protected abstract void UpdateCellTransformInCalculateSize(AccordionListCellBase instance);

        /// <summary>
        /// スクロール位置を元にContentのanchoredPositionを更新します
        /// </summary>
        /// <param name="scrollPosition"></param>
        protected abstract void UpdateContentAnchoredPosition(float scrollPosition);

        /// <summary>
        /// セルの座標を計算し、更新メソッドに渡します
        /// </summary>
        /// <param name="instance">セルのインスタンス</param>
        /// <param name="cellInfo">セル情報</param>
        protected abstract void UpdateCellTransform(AccordionListCellBase instance, AccordionListCellInfo cellInfo);

        /// <summary>
        /// Viewport領域外にインスタンスを配置します
        /// </summary>
        /// <param name="instance">セルのインスタンス</param>
        /// <param name="viewportSize">ビューポートサイズ</param>
        protected abstract void RelegateToOutOfContent(AccordionListCellBase instance, float viewportSize);

        /// <summary>
        /// 指定された正規化された割合(0.0f ~ 1.0f)に応じたスクロール位置を返します
        /// </summary>
        /// <param name="rate">正規化されたスクロール量</param>
        /// <param name="offsetSize">セルを表示する際の補正値</param>
        /// <returns>ジャンプ後の位置</returns>
        protected abstract Vector3 CalculateJumpPositionByRate(float rate, float offsetSize = 0.0f);

        /// <summary>
        /// スクロール方向を有効化します
        /// </summary>
        protected abstract void UpdateScrollingEnabled();

        /// <summary>
        /// 折り畳みの領域でスクロール位置の補正をかけます
        /// </summary>
        /// <param name="firstPosition">折り畳み領域の開始地点</param>
        /// <param name="lastPosition">折り畳み領域の終了地点</param>
        /// <param name="simplifyScrollPosition">スクロール位置</param>
        /// <returns></returns>
        protected abstract float UpdateScrollPositionByCollapse(Vector2 firstPosition, Vector2 lastPosition, float simplifyScrollPosition);

#endregion

#region unity methods

        void LateUpdate()
        {
            if (Model.CellInfos.Count == 0)
            {
                return;
            }

            if (IsTransition)
            {
                return;
            }

            SyncScrollPosition(SimplifyScrollPosition);
        }

#endregion

#region IAccordionList

        /// <summary>
        /// セルクリックの通知を受け取る
        /// </summary>
        /// <param name="nodeId">ノードID</param>
        /// <param name="cellType">セルの種類</param>
        public virtual void OnClickCell(int nodeId, AccordionListCellType cellType)
        {
            if (IsTransition)
            {
                return;
            }

            var node = Model.FindNodeWithID(nodeId);
            if (node.ChildCount == 0)
            {
                return;
            }

            if (node.IsExpand)
            {
                Collapse(node);
            }
            else
            {
                Expand(node);
            }
        }

#endregion
    }
}
