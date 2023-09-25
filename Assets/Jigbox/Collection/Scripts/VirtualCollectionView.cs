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
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Jigbox.Collection;
using Jigbox.Delegatable;

namespace Jigbox.Components
{
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class VirtualCollectionView<T> : MonoBehaviour, IVirtualCollectionView where T : IVirtualCollection
    {
#region Inner Class

        /// <summary>
        /// 仮想コレクションでの更新用デリゲート型
        /// </summary>
        public class VirtualCollectionUpdateDelegate : EventDelegate<GameObject, int>
        {
            public VirtualCollectionUpdateDelegate(Callback callback) : base(callback)
            {
            }
        }

#endregion

#region Abstract

        /// <summary>
        /// スクロールの開始点の座標を返します
        /// </summary>
        /// <value>The scroll origin point.</value>
        public abstract Vector2 ScrollOriginPoint { get; }

        /// <summary>
        /// コンテナがとるべきアンカーを基準にしたサイズを計算して返します
        /// </summary>
        /// <value>The content fit size delta.</value>
        public abstract Vector2 ContentFitSizeDelta { get; }

        /// <summary>
        /// 親コンテナのアンカーからの相対位置座標を元に、セルの全体からのインデックスを計算します
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="position">Position.</param>
        public abstract int CellIndex(Vector2 position);

        /// <summary>
        /// セルのインデックスから、親コンテナのアンカーからの、セルの相対位置座標を計算します
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="index">Index.</param>
        public abstract Vector2 CellPosition(int index);

        protected abstract T Model { get; }

        protected abstract void UpdateCellTransform(IndexedRectTransform cell);

        /// <summary>
        /// ヘッダとフッタの合計サイズを返します
        /// </summary>
        protected abstract Vector2 HeaderFooterSize { get; }

        /// <summary>
        /// ヘッダのサイズをVector2側にして返します
        /// ストレッチされる方向に関しては0となります
        /// </summary>
        protected abstract Vector2 HeaderSize { get; }

        /// <summary>
        /// スクロールできるかどうかの設定を更新します
        /// </summary>
        protected abstract void UpdateScrollingEnabled();

        #endregion

#region SerializeFields

        [HideInInspector]
        [SerializeField]
        protected Padding padding = Padding.zero;

        [SerializeField]
        [HideInInspector]
        protected DelegatableList onUpdateCellDelegates = new DelegatableList();

        [SerializeField]
        [HideInInspector]
        protected VirtualCollectionHeaderFooter headerFooter;

#endregion

#region Fields

        protected ScrollRect scrollRect;

        protected RectTransform content;

        protected RectTransform viewport;

        protected IInstanceProvider<GameObject> cellProvider;

        protected IInstanceDisposer<GameObject> cellDisposer = new DefaultInstanceDisposer();

        protected bool isInitialized = false;

        /// <summary>RefreshAllCellsを要求するイベントハンドラ</summary>
        protected Action requestHandler = null;

        /// <summary>
        /// LateUpdateでModelに渡されるUpdateceCellを保存しておくためのAction
        /// (GCAlloc対策用)
        /// </summary>
        protected Action<IndexedRectTransform, int> cachedUpdateCell;

        /// <summary>
        /// 現在表示されているセルを取得する際に使用する、GC回避のための辞書のキャッシュ
        /// </summary>
        protected Dictionary<int, GameObject> visibleCellCache = new Dictionary<int, GameObject>();

#endregion

#region Public Properties

        /// <summary>
        /// 外周の余白を参照/指定します
        /// </summary>
        /// <value>The padding.</value>
        public Padding Padding
        {
            get { return Model.Padding; }
            set { Model.Padding = padding = value; }
        }

        /// <summary>
        /// セルの内容を更新するタイミングで呼び出されるイベントハンドラ
        /// </summary>
        /// <value>The on update cell delegates.</value>
        public DelegatableList OnUpdateCellDelegates
        {
            get { return onUpdateCellDelegates; }
        }

        /// <summary>
        /// スクロール領域の、コンテナの配置基点からの移動相対量をベクトルで表現します
        /// </summary>
        /// <value>The scroll position.</value>
        public virtual Vector2 ScrollPosition
        {
            get
            {
                var parent = Content.parent as RectTransform;
                var position = Content.localPosition;
                var offsetX = Content.rect.width * Content.pivot.x - parent.rect.width * parent.pivot.x;
                var offsetY = Content.rect.height * (1.0f - Content.pivot.y) - parent.rect.height * (1.0f - parent.pivot.y);

                return new Vector2(-position.x + offsetX, position.y + offsetY);
            }
        }

        /// <summary>
        /// スクロールが利用可能かどうかを返します
        /// </summary>
        public virtual bool IsValidScroll
        {
            get
            {
                return Viewport.rect.width < Content.rect.width ||
                       Viewport.rect.height < Content.rect.height;
            }
        }

        /// <summary>
        /// 表示させたい、仮想のセルの総数を参照/指定します
        /// </summary>
        /// <value>The virtual cell count.</value>
        public virtual int VirtualCellCount
        {
            get { return Model.VirtualCellCount; }
            set
            {
                if (Model.VirtualCellCount == value)
                {
                    return;
                }
                Model.VirtualCellCount = value;
                Content.sizeDelta = ContentFitSizeDelta;
            }
        }

        /// <summary>
        /// 任意の方法でセルとなるGameObjectをTileViewに渡すプロバイダーを参照/指定します
        /// </summary>
        /// <value>The cell provider.</value>
        public virtual IInstanceProvider<GameObject> CellProvider
        {
            get
            {
                if (cellProvider == null)
                {
                    cellProvider = new InstanceProvider<GameObject>();
                }
                return cellProvider;
            }
            set { cellProvider = value; }
        }

        /// <summary>
        /// 任意の方法でセルを処分するディスポーザを参照/指定します
        /// </summary>
        public virtual IInstanceDisposer<GameObject> CellDisposer
        {
            get { return cellDisposer; }
            set { cellDisposer = value; }
        }

        /// <summary>
        /// ヘッダ・フッタ用コンポーネント
        /// </summary>
        public VirtualCollectionHeaderFooter HeaderFooter { get { return headerFooter; } }

#endregion

#region Protected Properties

        protected virtual ScrollRect ScrollRect
        {
            get
            {
                if (scrollRect == null)
                {
                    scrollRect = GetScrollRect();
                }
                return scrollRect;
            }
        }

        protected virtual RectTransform Content
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

        protected virtual RectTransform Viewport
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

#endregion

#region Public Method

        /// <summary>
        /// レイアウトを行うための値の同期や配置済みのセルの状態を整理します。
        /// </summary>
        public virtual void Refresh()
        {
            BatchSerializeField();
            UpdateScrollingEnabled();
            CatchUpExistCells();
            RelayoutAllCells();
            // ヘッダ・フッタも再配置する
            if (headerFooter != null)
            {
                headerFooter.Relayout();
            }
        }

        /// <summary>
        /// Viewportから視認できるセルの個数を返します
        /// </summary>
        /// <returns>The cell count.</returns>
        /// <param name="extendable">If set to <c>true</c> extendable.</param>
        public virtual int VisibleCellCount(bool extendable)
        {
            return Model.VisibleCellCount(extendable);
        }

        /// <summary>
        /// 全てのセルの配置を更新します
        /// </summary>
        public virtual void RelayoutAllCells()
        {
            if (Model.ViewportSize.x < 0 || Model.ViewportSize.y < 0)
            {
                // Unity のシリアライズ対象のオブジェクトの初期化の過程でViewportのサイズと同期した時にこういう状況がある
                return;
            }
            foreach (var cell in Model.ManagedCells)
            {
                UpdateCellTransform(cell);
            }
            UpdateScrollingEnabled();
        }

        /// <summary>
        /// ヘッダ・フッタとセルの配置を更新します
        /// ユーザがヘッダ・フッタの設定変更を行ったあとに呼び出してもらうAPIになります
        /// </summary>
        public virtual void RelayoutHeaderFooter()
        {
            Content.sizeDelta = ContentFitSizeDelta;
            MoveToOriginPoint();
            RelayoutAllCells();
            if (headerFooter != null)
            {
                headerFooter.Relayout();
            }
        }

        /// <summary>
        /// ビューをセルで埋めます
        /// </summary>
        /// <param name="generator">Generator.</param>
        public virtual void FillCells(Func<GameObject> generator)
        {
            if (generator == null)
            {
                throw new ArgumentNullException("generator");
            }

            BatchSerializeField();
            UpdateScrollingEnabled();
            int managedCellCount = Model.ManagedCells.Count;
            int visibleCellCount = Model.VisibleCellCount(true);

            for (int i = managedCellCount; i < visibleCellCount; i++)
            {
                AddCellItem(generator());
            }
        }

        /// <summary>
        /// ビューをセルで埋めます
        /// </summary>
        /// <param name="cellPrefab">Cell prefab.</param>
        public virtual void FillCells(GameObject cellPrefab)
        {
            if (cellPrefab == null)
            {
                throw new ArgumentNullException("cellPrefab");
            }
            FillCells(() => Instantiate(cellPrefab, Content, false));
        }

        /// <summary>
        /// ビューを、セルで埋めます
        /// </summary>
        public virtual void FillCells()
        {
            if (cellProvider == null)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("[{0}] CellProvider can't provide cell instance.", GetType());
#endif
                return;
            }
            FillCells(cellProvider.Generate);
        }

        /// <summary>
        /// リストビューから全てのセルを消去します
        /// </summary>
        public virtual void ClearAllCells()
        {
            if (Model.ManagedCells.Count == 0)
            {
                return;
            }
            foreach (var cell in Model.ManagedCells.ToArray())
            {
                DestroyCellObject(cell.RectTransform.gameObject);
            }
            Model.ManagedCells.Clear();
        }

        /// <summary>
        /// 現在表示されているセルのIndexとGameObjectの参照を取得します
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<int, GameObject> GetVisibleCells()
        {
            visibleCellCache.Clear();

            foreach (var cell in Model.ManagedCells)
            {
                visibleCellCache.Add(cell.Index, cell.RectTransform.gameObject);
            }

            return visibleCellCache;
        }

        /// <summary>
        /// スクロール開始点までスクロール位置を移動します
        /// </summary>
        public virtual void MoveToOriginPoint()
        {
            Content.localPosition = ScrollOriginPoint;
        }

        /// <summary>
        /// 全てのセルを強制的に再構築、更新します
        /// </summary>
        /// <param name="cellProvider">セルとなるGameObjectを生成する処理</param>
        /// <param name="hasPrefabChanged">セルのPrefabを差し替えるか否か</param>
        public virtual void RefreshAllCells(Func<GameObject> cellProvider, bool hasPrefabChanged = false)
        {
            if (hasPrefabChanged)
            {
                ClearAllCells();
            }
            MoveToOriginPoint();
            FillCells(cellProvider);
            RemoveRedundantCells();
            RelayoutAllCells();
            foreach (var cell in Model.ManagedCells)
            {
                UpdateCell(cell, cell.Index);
            }
            // ヘッダ・フッタも再配置する
            if (headerFooter != null)
            {
                headerFooter.Relayout();
            }
        }

        /// <summary>
        /// 全てのセルを強制的に再構築、更新します
        /// </summary>
        /// <param name="cellPrefab">セルのPrefab</param>
        /// <param name="hasPrefabChanged">セルのPrefabを差し替えるか否か</param>
        public virtual void RefreshAllCells(GameObject cellPrefab, bool hasPrefabChanged = false)
        {
            if (cellPrefab == null)
            {
                throw new ArgumentNullException("cellPrefab");
            }
            RefreshAllCells(() => Instantiate(cellPrefab, Content, false), hasPrefabChanged);
        }

        /// <summary>
        /// 全てのセルを強制的に再構築、更新します
        /// </summary>
        /// <param name="hasPrefabChanged">セルのPrefabを差し替えるか否か</param>
        public virtual void RefreshAllCells(bool hasPrefabChanged = false)
        {
            if (cellProvider == null)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("[{0}] CellProvider can't provide cell instance.", GetType());
#endif
                return;
            }
            RefreshAllCells(cellProvider.Generate, hasPrefabChanged);
        }

        /// <summary>
        /// <para>全てのセルの再構築、更新を要求します。</para>
        /// <para>実際に再構築、更新が行われるのは、このコンポーネントの次の LateUpdate() のタイミングとなります。</para>
        /// <para>直前呼び出した処理が終わる前に、再度呼び出した場合、直前に設定されたコールバックは上書きされます。</para>
        /// </summary>
        /// <param name="onComplete">再構築が完了した際のコールバック</param>
        /// <param name="cellProvider">セルとなるGameObjectを生成する処理</param>
        /// <param name="hasPrefabChanged">セルのPrefabを差し替えるか否か</param>
        public virtual void ReserveRefreshAllCells(
            Action onComplete,
            Func<GameObject> cellProvider,
            bool hasPrefabChanged = false)
        {
            requestHandler = () =>
            {
                RefreshAllCells(cellProvider, hasPrefabChanged);
                if (onComplete != null)
                {
                    onComplete();
                }
            };
        }

        /// <summary>
        /// <para>全てのセルの再構築、更新を要求します。</para>
        /// <para>実際に再構築、更新が行われるのは、このコンポーネントの次の LateUpdate() のタイミングとなります。</para>
        /// <para>直前呼び出した処理が終わる前に、再度呼び出した場合、直前に設定されたコールバックは上書きされます。</para>
        /// </summary>
        /// <param name="onComplete">再構築が完了した際のコールバック</param>
        /// <param name="cellPrefab">セルのPrefab</param>
        /// <param name="hasPrefabChanged">セルのPrefabを差し替えるか否か</param>
        public virtual void ReserveRefreshAllCells(
            Action onComplete,
            GameObject cellPrefab,
            bool hasPrefabChanged = false)
        {
            requestHandler = () =>
            {
                RefreshAllCells(cellPrefab, hasPrefabChanged);
                if (onComplete != null)
                {
                    onComplete();
                }
            };
        }

        /// <summary>
        /// <para>全てのセルの再構築、更新を要求します。</para>
        /// <para>実際に再構築、更新が行われるのは、このコンポーネントの次の LateUpdate() のタイミングとなります。</para>
        /// <para>直前呼び出した処理が終わる前に、再度呼び出した場合、直前に設定されたコールバックは上書きされます。</para>
        /// </summary>
        /// <param name="onComplete">再構築が完了した際のコールバック</param>
        /// <param name="hasPrefabChanged">セルのPrefabを差し替えるか否か</param>
        public virtual void ReserveRefreshAllCells(Action onComplete, bool hasPrefabChanged = false)
        {
            requestHandler = () =>
            {
                RefreshAllCells(hasPrefabChanged);
                if (onComplete != null)
                {
                    onComplete();
                }
            };
        }

        /// <summary>
        /// Content RectTransform 以下の子要素を探索し、セルに該当するモノを管理対象にします
        /// </summary>
        /// <remarks>孫要素（子要素の子要素）は探索しません</remarks>
        /// <returns>管理下に無かったセルを発見した場合は<c>true</c>、無ければ<c>false</c></returns>
        /// <param name="errorCallback">セルに適さないGameObjectを発見した場合のコールバック</param>
        public virtual bool CatchUpExistCells(Action<GameObject> errorCallback = null)
        {
            if (Content.childCount == 0 || OnUpdateCellDelegates.Count == 0)
            {
                return false;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return false;
            }
#endif
            var exist = false;
            for (int i = 0; i < Content.childCount; i++)
            {
                var child = Content.GetChild(i) as RectTransform;
                if (child == null)
                {
                    if (errorCallback != null)
                    {
                        errorCallback(Content.GetChild(i).gameObject);
                    }
                    continue;
                }
                // Header、Footer用のComponentの場合はCellとして扱わない
                if (headerFooter != null && child.GetComponent<VirtualCollectionHeaderFooterObject>() != null)
                {
                    continue;
                }
                if (Model.Contains(child))
                {
                    continue;
                }
                try
                {
                    // 試しに OnUpdateCell に合うか叩いてみて、例外が無いならセルと見做す
                    if (OnUpdateCellDelegates.Count > 0)
                    {
                        OnUpdateCellDelegates.Execute(child.gameObject, i);
                    }
                    Model.AddCell(child);
                    exist = true;
                }
                catch (Exception)
                {
                    if (errorCallback != null)
                    {
                        errorCallback(child.gameObject);
                    }
                }
            }
            return exist;
        }

        /// <summary>
        /// シリアライズフィールドとモデル層の状態を同期させます
        /// </summary>
        public virtual void BatchSerializeField()
        {
            Model.Padding = padding;
            Model.ViewportSize = Viewport.rect.size;
            Content.sizeDelta = ContentFitSizeDelta;

            MoveToOriginPoint();
        }

        /// <summary>
        /// セルの内容を更新する際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(GameObject, int)の関数</param>
        public void AddUpdateEvent(VirtualCollectionUpdateDelegate.Callback callback)
        {
            onUpdateCellDelegates.Add(new VirtualCollectionUpdateDelegate(callback));
        }

#endregion

#region Protected Method

        protected virtual ScrollRect GetScrollRect()
        {
            var sr = GetComponent<ScrollRect>();
            sr.vertical = false;
            sr.horizontal = false;
            sr.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            sr.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            return sr;
        }

        protected virtual void UpdateCell(IndexedRectTransform cell, int index)
        {
            var active = index < VirtualCellCount;
            cell.Index = index;
            UpdateCellTransform(cell);

            var go = cell.RectTransform.gameObject;
            go.SetActive(active);

            if (active && OnUpdateCellDelegates.Count > 0)
            {
                OnUpdateCellDelegates.Execute(go, index);
            }
        }

        protected virtual void AddCellItem(GameObject go)
        {
            if (go == null)
            {
                throw new ArgumentNullException("GameObject");
            }
            var rt = go.transform as RectTransform;
            if (rt == null)
            {
                var msg = string.Format("[{0}] Not acceptable GameObject. {1} has not RectTransform component", GetType(), go.name);
                throw new InvalidOperationException(msg);
            }
            rt.SetParent(Content, false);

            var cell = Model.AddCell(rt);
            UpdateCell(cell, cell.Index);
        }

        protected virtual void DestroyCellObject(GameObject cellObject)
        {
            cellDisposer.Dispose(cellObject);
        }

        protected virtual void RemoveRedundantCells()
        {
            var managedCellCount = Model.ManagedCells.Count;
            var visibleCellCount = VisibleCellCount(true);
            if (managedCellCount == 0 || managedCellCount <= visibleCellCount)
            {
                return;
            }
            var redundants = new Queue<GameObject>();
            for (int i = visibleCellCount; i < managedCellCount; i++)
            {
                redundants.Enqueue(Model.ManagedCells[i].RectTransform.gameObject);
            }
            // 管理下から参照を先に外す
            Model.ManagedCells.RemoveRange(visibleCellCount, redundants.Count);

            // 管理下から外れたGameObjectを破棄
            while (redundants.Count > 0)
            {
                DestroyCellObject(redundants.Dequeue());
            }
        }

        /// <summary>
        /// Content RectTransform 以下の子要素を探索し、ヘッダ・フッタに該当するモノを管理対象にします
        /// </summary>
        /// <returns></returns>
        protected virtual bool CatchUpExistHeaderFooter()
        {
            // ヘッダ・フッタの設定がない場合は何もしない
            if (headerFooter == null)
            {
                return false;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return false;
            }
#endif
            var exist = false;
            GameObject headerGo = null;
            GameObject footerGo = null;
            for (int i = 0; i < Content.childCount; i++)
            {
                var baseView = Content.GetChild(i).GetComponent<VirtualCollectionHeaderFooterObject>();
                // Header、Footer用のComponentの場合はCellとして扱わない
                if (baseView == null)
                {
                    continue;
                }
                if (baseView is VirtualCollectionHeader)
                {
                    // 先にみつけたヘッダを設定する
                    if (headerGo == null)
                    {
                        headerGo = baseView.gameObject;
                        headerFooter.SetHeader(headerGo);
                    }
                }
                if (baseView is VirtualCollectionFooter)
                {
                    // 先にみつけたフッタを設定する
                    if (footerGo == null)
                    {
                        footerGo = baseView.gameObject;
                        headerFooter.SetFooter(footerGo);
                    }
                }
                exist = true;
            }
            return exist;
        }

#endregion

#region Unity Method

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            // OnValidateでsizeDeltaを変更するとワーニングがでるのでdelayCallで呼び出す
            UnityEditor.EditorApplication.delayCall += OnDelayValidate;
        }

        protected virtual void OnDelayValidate()
        {
            if (this)
            {
                Refresh();
            }
        }
#endif

        protected virtual void OnEnable()
        {
            if (!isInitialized)
            {
                // Scene上に存在するヘッダ・フッタ用オブジェクトのチェックはOnEnable時に一度だけ走らせる
                CatchUpExistHeaderFooter();
                Refresh();
                isInitialized = true;
                // LateUpdateでGCAllocが発生しないようにActionにCacheさせる
                cachedUpdateCell = UpdateCell;
            }
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void LateUpdate()
        {
            // RectTransformの基本計算は、OnEnable終了時点で完了しているが、
            // その後、CanvasScalerによる補正がUpdate中に行われるため、
            // その両方が確実に終わっているLateUpdateのタイミングで処理する
            if (requestHandler != null)
            {
                requestHandler();
                requestHandler = null;
            }

            // GCAlloc削減対応
            // 直接メソッドを渡すとGCAllocが発生するのでActionにしたものを渡すようにする
            Model.SyncScrollPosition(ScrollPosition - HeaderSize, cachedUpdateCell);
        }

#endregion
    }
}
