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
    /// <summary>
    /// AccordionListCellInfoをリストで保持しつつユーティリティを提供するクラス
    /// </summary>
    public abstract class AccordionListCellInfoModel
    {
        /// <summary>
        /// AccordionListCellInfoへアクセスする為に必要な情報
        /// </summary>
        protected struct CellInfo : IEquatable<CellInfo>
        {
            /// <summary>ノードId</summary>
            readonly int nodeId;

            /// <summary>メインセルならtrue</summary>
            readonly AccordionListCellType cellType;

            public CellInfo(int nodeId, AccordionListCellType cellType)
            {
                this.nodeId = nodeId;
                this.cellType = cellType;
            }

            public bool Equals(CellInfo other)
            {
                return nodeId == other.nodeId && cellType == other.cellType;
            }

            public override int GetHashCode()
            {
                // Riderによる自動生成
                // FNVハッシュを利用
                unchecked
                {
                    return (nodeId * 397) ^ (int) cellType;
                }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is CellInfo && Equals((CellInfo) obj);
            }

            public static bool operator ==(CellInfo left, CellInfo right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(CellInfo left, CellInfo right)
            {
                return !left.Equals(right);
            }
        }

#region field & properties

        /// <summary>セルの情報リスト</summary>
        readonly List<AccordionListCellInfo> cellInfos = new List<AccordionListCellInfo>();

        /// <summary>セルの情報リストの参照</summary>
        public virtual List<AccordionListCellInfo> CellInfos
        {
            get { return cellInfos; }
        }

        /// <summary>チャイルドエリア用のセル情報リスト</summary>
        readonly List<AccordionListCellInfo> childAreaCellInfos = new List<AccordionListCellInfo>();

        /// <summary>チャイルドエリア用のセル情報リストの参照</summary>
        public virtual List<AccordionListCellInfo> ChildAreaCellInfos
        {
            get { return childAreaCellInfos; }
        }

        /// <summary>セルの数をHashCodeをKeyとして保持</summary>
        readonly Dictionary<int, int> cellCounts = new Dictionary<int, int>();

        /// <summary>セルの数をHashCodeをKeyとして保持の参照</summary>
        protected virtual Dictionary<int, int> CellCounts
        {
            get { return cellCounts; }
        }

        /// <summary>セルのPrefabの種類をHashSetで保持</summary>
        readonly HashSet<AccordionListCellBase> cellPrefabs = new HashSet<AccordionListCellBase>();

        /// <summary>セルのPrefabの種類をHashSetで保持の参照</summary>
        public virtual HashSet<AccordionListCellBase> CellPrefabs
        {
            get { return cellPrefabs; }
        }

        /// <summary>ルートノード</summary>
        readonly AccordionListRootNode rootNode = new AccordionListRootNode();

        /// <summary>ルートノードの参照</summary>
        protected virtual AccordionListRootNode RootNode
        {
            get { return rootNode; }
        }

        /// <summary>ノードキャッシュ</summary>
        readonly Dictionary<int, AccordionListNode> nodeCaches = new Dictionary<int, AccordionListNode>();

        /// <summary>ノードキャッシュの参照</summary>
        protected virtual Dictionary<int, AccordionListNode> NodeCaches
        {
            get { return nodeCaches; }
        }

        /// <summary>全てのセル情報</summary>
        readonly Dictionary<CellInfo, AccordionListCellInfo> allCellInfo = new Dictionary<CellInfo, AccordionListCellInfo>();

        /// <summary>全てのセル情報の参照</summary>
        protected virtual Dictionary<CellInfo, AccordionListCellInfo> AllCellInfo
        {
            get { return allCellInfo; }
        }

        /// <summary>
        /// IDが設定されていないノードの一時保持用
        /// ID設定後は一覧から削除されます。
        /// </summary>
        readonly List<AccordionListNode> nonIdNodeList = new List<AccordionListNode>();

        /// <summary>
        /// IDが設定されていないノードの一時保持用の参照
        /// ID設定後は一覧から削除されます。
        /// </summary>
        protected virtual List<AccordionListNode> NonIdNodeList
        {
            get { return nonIdNodeList; }
        }

        /// <summary>使用するノード一覧</summary>
        readonly List<AccordionListNode> useNodeList = new List<AccordionListNode>();

        /// <summary>使用するノード一覧の参照</summary>
        protected virtual List<AccordionListNode> UseNodeList
        {
            get { return useNodeList; }
        }

        /// <summary>挿入予定リスト</summary>
        readonly List<AccordionListCellInfo> insertCellInfos = new List<AccordionListCellInfo>();

        /// <summary>挿入予定リストの参照</summary>
        protected virtual List<AccordionListCellInfo> InsertCellInfos
        {
            get { return insertCellInfos; }
        }

        /// <summary>削除予定リスト</summary>
        readonly List<AccordionListCellInfo> removeCellInfos = new List<AccordionListCellInfo>();

        /// <summary>削除予定リストの参照</summary>
        public virtual List<AccordionListCellInfo> RemoveCellInfos
        {
            get { return removeCellInfos; }
        }

        /// <summary>Prefab毎の最小セルサイズのキャッシュ</summary>
        readonly Dictionary<int, float> minimumCellSizes = new Dictionary<int, float>();

        /// <summary>Prefab毎の最小セルサイズのキャッシュの参照</summary>
        protected virtual Dictionary<int, float> MinimumCellSizes
        {
            get { return minimumCellSizes; }
        }

#endregion

#region public methods

        /// <summary>
        /// ノード配列をキャッシュに入れる
        /// </summary>
        /// <param name="nodeList">ノード配列</param>
        public virtual void AddNodeList<T>(IEnumerable<T> nodeList) where T : AccordionListNode
        {
            // ノード一覧に登録する
            foreach (var node in nodeList)
            {
                UseNodeList.Add(node);
            }
        }

        /// <summary>
        /// ルートノードから再帰でノードをセルとして並べる
        /// </summary>
        /// <param name="isSingleMode">シングルモードフラグ</param>
        public virtual void LayoutCellFromRootNode(bool isSingleMode)
        {
            CreateNodeTree();

            SetDefaultExpand(RootNode);

            AddAllCellInfo();

            SetSingleMode(isSingleMode);

            Expand(RootNode);
        }

        /// <summary>
        /// ノードをノードIDから探す
        /// </summary>
        /// <param name="id">ノードID</param>
        /// <returns>該当しないIDの場合はnullを返します</returns>
        public virtual AccordionListNode FindNodeWithID(int id)
        {
            AccordionListNode result;
            if (NodeCaches.TryGetValue(id, out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// CellInfoを探します
        /// </summary>
        /// <param name="nodeId">ノードId</param>
        /// <param name="cellType">セルの種類</param>
        /// <returns>セル情報/見つからない場合はnull</returns>
        public virtual AccordionListCellInfo FindCellInfo(int nodeId, AccordionListCellType cellType = AccordionListCellType.Main)
        {
            AccordionListCellInfo cellInfo;
            if (AllCellInfo.TryGetValue(new CellInfo(nodeId, cellType), out cellInfo))
            {
                return cellInfo;
            }

            return null;
        }

        /// <summary>
        /// RefreshCells表示用のセル情報に変更
        /// </summary>
        public virtual void Refresh()
        {
            for (var i = 0; i < CellInfos.Count; i++)
            {
                var cellInfo = CellInfos[i];
                cellInfo.Index = i;
                cellInfo.CellReference = null;
                cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Insert;
            }

            foreach (var cellInfo in RemoveCellInfos)
            {
                cellInfo.Index = int.MinValue;
                cellInfo.CellReference = null;
                cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Normal;
            }

            foreach (var cellInfo in ChildAreaCellInfos)
            {
                cellInfo.Index = FindCellInfo(cellInfo.Node.Id, AccordionListCellType.Main).Index;
                cellInfo.CellReference = null;
            }

            RemoveCellInfos.Clear();
        }

        /// <summary>
        /// 指定したノードの親まで開ける
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="isSingleMode">シングルモード</param>
        public virtual void ExpandAncestors(AccordionListNode node, bool isSingleMode)
        {
            var cellInfo = FindCellInfo(node.ParentId);
            if (cellInfo == null)
            {
                return;
            }

            // 表示セルの場合はそれ以上先祖を辿らない
            if (cellInfo.Index >= 0)
            {
                if (isSingleMode)
                {
                    CollapseBrotherNode(cellInfo.Node);
                }

                Expand(cellInfo.Node);
                return;
            }

            cellInfo.Node.IsExpand = true;

            if (isSingleMode)
            {
                CollapseBrotherNode(cellInfo.Node);
            }

            ExpandAncestors(cellInfo.Node, isSingleMode);
        }

        /// <summary>
        /// 兄弟ノードを閉じる
        /// </summary>
        /// <param name="node"></param>
        public virtual void CollapseBrotherNode(AccordionListNode node)
        {
            var parentNode = FindNodeWithID(node.ParentId);
            for (var i = 0; i < parentNode.ChildrenNode.Count; i++)
            {
                var child = parentNode.ChildrenNode[i];
                if (child.Id != node.Id)
                {
                    Collapse(child);
                }
            }
        }

        /// <summary>
        /// 指定したノードを開く
        /// </summary>
        /// <param name="node">ノード</param>
        public virtual void Expand(AccordionListNode node)
        {
            ExpandCore(node);

            // ルートノードの場合は追加
            if (node.Id == int.MinValue)
            {
                CellInfos.AddRange(InsertCellInfos);
            }
            else
            {
                var startCellInfo = FindCellInfo(node.Id);
                CellInfos.InsertRange(startCellInfo.Index + 1, InsertCellInfos);
            }

            InsertCellInfos.Clear();

            for (var i = 0; i < CellInfos.Count; i++)
            {
                var cellInfo = CellInfos[i];
                if (cellInfo.Status == AccordionListCellInfo.CellInfoStatus.Insert)
                {
                    cellInfo.Index = i;
                }

                if (cellInfo.Index != i)
                {
                    cellInfo.Index = i;
                    cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Update;
                }
            }

            foreach (var cellInfo in ChildAreaCellInfos)
            {
                var main = FindCellInfo(cellInfo.Node.Id, AccordionListCellType.Main);
                cellInfo.Index = main.Index;
            }

            ChildAreaCellInfos.Sort((info, cellInfo) => info.Index - cellInfo.Index);
        }

        /// <summary>
        /// 指定したノードを閉じる
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="allDescendant">子孫全て閉じるか</param>
        public virtual void Collapse(AccordionListNode node, bool allDescendant = false)
        {
            CollapseCore(node, allDescendant);

            node.IsExpand = false;

            for (int i = 0; i < CellInfos.Count; i++)
            {
                var cellInfo = CellInfos[i];
                if (cellInfo.Index == i)
                {
                    continue;
                }

                cellInfo.Index = i;
                cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Update;
            }

            foreach (var cellInfo in ChildAreaCellInfos)
            {
                var main = FindCellInfo(cellInfo.Node.Id, AccordionListCellType.Main);
                cellInfo.Index = main.Index;
            }

            ChildAreaCellInfos.Sort((info, cellInfo) => info.Index - cellInfo.Index);
        }

        /// <summary>
        /// 全て開く
        /// </summary>
        public virtual void ExpandAll()
        {
            foreach (var cellInfo in CellInfos)
            {
                cellInfo.Index = int.MinValue;
                cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Normal;
                cellInfo.CellReference = null;
                RemoveCellInfo(cellInfo.CellPrefab);
            }

            CellInfos.Clear();

            ExpandCore(RootNode, true);

            CellInfos.AddRange(InsertCellInfos);
            InsertCellInfos.Clear();

            for (var i = 0; i < CellInfos.Count; i++)
            {
                var cellInfo = CellInfos[i];
                cellInfo.Index = i;
            }
        }

        /// <summary>
        /// 全て閉じる
        /// </summary>
        public virtual void CollapseAll()
        {
            foreach (var kvp in AllCellInfo)
            {
                var cellInfo = kvp.Value;
                cellInfo.Index = int.MinValue;
                cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Normal;
                cellInfo.CellReference = null;
                cellInfo.Node.IsExpand = false;
            }

            CellInfos.Clear();
            ChildAreaCellInfos.Clear();

            Expand(RootNode);
        }

        /// <summary>
        /// 指定された種類のPrefabで紐づいているセルの数を取得します。
        /// </summary>
        /// <param name="cellPrefab">Prefabの種類</param>
        /// <returns>セルの数</returns>
        public virtual int CellCountByPrefab(AccordionListCellBase cellPrefab)
        {
            var hashCode = cellPrefab.GetHashCode();

            if (!CellCounts.ContainsKey(hashCode))
            {
                Debug.LogError("invalid args");
                return 0;
            }

            return CellCounts[hashCode];
        }

        /// <summary>
        /// 現在のスクロール量から、一番初めに見えるセルのIndexを計算して取得します
        /// </summary>
        /// <param name="scrollPosition">現在のスクロール量</param>
        /// <returns>セルのIndex</returns>
        public virtual int FirstIndexAtScrollPosition(float scrollPosition)
        {
            var min = 0;
            var max = CellInfos.Count - 1;
            while (max >= min)
            {
                int middle = min + (max - min) / 2;

                var frontPosition = Mathf.Abs(SimplifyCellFrontPosition(CellInfos[middle], true));
                var backPosition = Mathf.Abs(SimplifyCellBackPosition(CellInfos[middle], true));
                if (frontPosition <= scrollPosition && scrollPosition <= backPosition)
                {
                    return middle;
                }
                else if (backPosition < scrollPosition)
                {
                    min = middle + 1;
                }
                else if (frontPosition > scrollPosition)
                {
                    max = middle - 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// 一番初めに見えるセルのIndexから、最後に見えるセルのIndexを計算して取得します
        /// </summary>
        /// <param name="scrollPosition">現在のスクロール位置</param>
        /// <param name="firstIndex">最初に見えるセル番号</param>
        /// <param name="simplifyViewportSize">viewportのサイズ</param>
        /// <returns>セルのIndex</returns>
        public virtual int LastIndexByFirstIndex(float scrollPosition, int firstIndex, float simplifyViewportSize)
        {
            var lastIndex = cellInfos.Count - 1;

            var index = 0;
            for (var i = firstIndex; i <= lastIndex; i++)
            {
                if (i == lastIndex)
                {
                    index = lastIndex;
                    break;
                }

                var cellInfo = CellInfos[i];
                if (scrollPosition + simplifyViewportSize < Mathf.Abs(SimplifyCellBackPosition(cellInfo, true)))
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// セルの情報を全て削除します
        /// </summary>
        public virtual void Clear()
        {
            CellInfos.Clear();
            CellCounts.Clear();
            CellPrefabs.Clear();
            InsertCellInfos.Clear();
            RemoveCellInfos.Clear();
            ChildAreaCellInfos.Clear();

            NodeCaches.Clear();
            AllCellInfo.Clear();
            NonIdNodeList.Clear();
            UseNodeList.Clear();
            RootNode.ClearChildren();
            MinimumCellSizes.Clear();
        }

        /// <summary>
        /// シングルモードの表示にする
        /// </summary>
        /// <param name="isSingleMode">シングルモードフラグ</param>
        public virtual void SetSingleMode(bool isSingleMode)
        {
            if (!isSingleMode)
            {
                return;
            }

            SetSingleModeCore(RootNode);
        }

        /// <summary>
        /// セルの最小サイズを返します
        /// </summary>
        /// <returns>セルの最小サイズ</returns>
        public virtual float MinimumCellSizeByPrefab(AccordionListCellBase cellPrefab)
        {
            var prefabHash = cellPrefab.GetHashCode();
            float minimumSize;
            if (MinimumCellSizes.TryGetValue(prefabHash, out minimumSize))
            {
                return minimumSize;
            }

            minimumSize = float.MaxValue;
            foreach (var cellInfo in CellInfos)
            {
                if (cellInfo.PrefabHash != prefabHash)
                {
                    continue;
                }

                if (minimumSize > cellInfo.Size)
                {
                    minimumSize = cellInfo.Size;
                }
            }

            MinimumCellSizes.Add(prefabHash, minimumSize);

            return minimumSize;
        }

        /// <summary>
        /// 先祖にあるチャイルドエリアセルを探す
        /// </summary>
        /// <param name="origin">基準</param>
        /// <param name="result">戻り値</param>
        public virtual void AncestorsChildAreaCell(AccordionListCellInfo origin, List<AccordionListCellInfo> result)
        {
            var node = origin.Node;
            if (node.HasChildAreaCell)
            {
                result.Add(FindCellInfo(node.Id, AccordionListCellType.ChildArea));
            }

            var cellInfo = FindCellInfo(node.ParentId, AccordionListCellType.Main);
            if (cellInfo == null)
            {
                return;
            }

            AncestorsChildAreaCell(cellInfo, result);
        }

        /// <summary>
        /// 先祖に含まれているか
        /// </summary>
        /// <param name="origin">基準</param>
        /// <param name="target">対象</param>
        /// <returns>含まれているならtrue</returns>
        public virtual bool IsContainsAncestors(AccordionListCellInfo origin, AccordionListCellInfo target)
        {
            return IsContainsAncestorsCore(origin.Node, target);
        }

        /// <summary>
        /// チャイルドエリアを返す
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <returns>サイズ</returns>
        public virtual float ChildAreaSize(AccordionListCellInfo cellInfo)
        {
            var startPos = SimplifyCellBackPosition(cellInfo, false);
            var optionalCell = FindCellInfo(cellInfo.Node.Id, AccordionListCellType.Optional);
            var result = SimplifyCellFrontPosition(optionalCell, false) - startPos;

            return Mathf.Abs(result);
        }

        /// <summary>
        /// スクリーン内に含まれているか
        /// </summary>
        /// <param name="screenFront">ScreenFront</param>
        /// <param name="screenBack">ScreenBack</param>
        /// <param name="cellInfo">セル情報</param>
        /// <returns>含まれていればtrue</returns>
        public virtual bool IsContainScreen(float screenFront, float screenBack, AccordionListCellInfo cellInfo)
        {
            var cellFront = Mathf.Abs(SimplifyCellFrontPosition(cellInfo, false));
            var cellBack = Mathf.Abs(SimplifyCellBackPosition(cellInfo, false));
            return cellBack >= screenFront && cellFront <= screenBack;
        }

#endregion

#region protected methods

        /// <summary>
        /// 指定したノードを展開する
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="allDescendant">全ての子孫を開くか</param>
        protected virtual void ExpandCore(AccordionListNode node, bool allDescendant = false)
        {
            if (node.ChildCount == 0)
            {
                return;
            }

            if (node.HasChildAreaCell)
            {
                var cellInfo = FindCellInfo(node.Id, AccordionListCellType.ChildArea);
                AddCellInfo(cellInfo);
            }

            for (var i = 0; i < node.ChildrenNode.Count; i++)
            {
                var childNode = node.ChildrenNode[i];
                var cellInfo = FindCellInfo(childNode.Id);
                var isInsert = cellInfo.Index < 0;
                if (isInsert)
                {
                    AddCellInfo(cellInfo);
                }

                if (childNode.IsExpand || allDescendant)
                {
                    ExpandCore(childNode, allDescendant);
                }

                if (isInsert)
                {
                    var optionalCellInfo = FindCellInfo(childNode.Id, AccordionListCellType.Optional);
                    AddCellInfo(optionalCellInfo);
                }
            }

            node.IsExpand = true;
        }

        /// <summary>
        /// 閉じる処理
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="allDescendant">子孫全て閉じるか</param>
        protected virtual void CollapseCore(AccordionListNode node, bool allDescendant)
        {
            if (!node.IsExpand && allDescendant == false)
            {
                return;
            }

            if (node.HasChildAreaCell)
            {
                var childAreaCell = FindCellInfo(node.Id, AccordionListCellType.ChildArea);
                if (childAreaCell.Index >= 0)
                {
                    RemoveCellInfo(childAreaCell);
                }
            }

            // 子孫ノードまで再帰で閉じる必要がある
            for (var i = 0; i < node.ChildrenNode.Count; i++)
            {
                var childNode = node.ChildrenNode[i];
                var cellInfo = FindCellInfo(childNode.Id);
                var findIndex = cellInfo.Index >= 0;
                if (findIndex)
                {
                    RemoveCellInfo(cellInfo);
                }

                CollapseCore(childNode, allDescendant);

                if (findIndex)
                {
                    var optionalCellInfo = FindCellInfo(childNode.Id, AccordionListCellType.Optional);
                    RemoveCellInfo(optionalCellInfo);
                }

                if (allDescendant)
                {
                    childNode.IsExpand = false;
                }
            }
        }

        /// <summary>
        /// シングルモードの表示にする
        /// </summary>
        /// <param name="node">ノード</param>
        protected virtual void SetSingleModeCore(AccordionListNode node)
        {
            if (node.IsExpand)
            {
                bool isExpand = false;
                for (var i = 0; i < node.ChildrenNode.Count; i++)
                {
                    var childNode = node.ChildrenNode[i];
                    if (childNode.IsExpand && !isExpand)
                    {
                        isExpand = true;
                        SetSingleModeCore(childNode);
                    }
                    else
                    {
                        Collapse(childNode, true);
                    }
                }
            }
        }

        /// <summary>
        /// セルの関連情報を更新します
        /// </summary>
        /// <param name="cellPrefab">セルのPrefab</param>
        /// <param name="prefabHash">Prefabのハッシュ</param>
        protected virtual void UpdateCellInfo(AccordionListCellBase cellPrefab, int prefabHash)
        {
            if (prefabHash == 0)
            {
                return;
            }

            if (!CellPrefabs.Contains(cellPrefab))
            {
                CellPrefabs.Add(cellPrefab);
                CellCounts.Add(prefabHash, 1);
            }
            else
            {
                CellCounts[prefabHash]++;
            }
        }

        /// <summary>
        /// セルの関連情報を更新し、必要な場合削除します
        /// </summary>
        /// <param name="cellPrefab">セルPrefab</param>
        protected virtual void RemoveCellInfo(AccordionListCellBase cellPrefab)
        {
            if (cellPrefab == null)
            {
                return;
            }

            var hashCode = cellPrefab.GetHashCode();

            CellCounts[hashCode]--;

            if (CellCounts[hashCode] == 0)
            {
                CellPrefabs.Remove(cellPrefab);
                CellCounts.Remove(hashCode);
            }
        }

        /// <summary>
        /// 登録されているノードでツリー構造を作る
        /// </summary>
        protected virtual void CreateNodeTree()
        {
            AddToNodeIdTable(RootNode);

            // IDが割り振られていないノードを探す
            foreach (var node in UseNodeList)
            {
                SearchForNonIdNodes(node);
            }

            // IDが振られていないノードにIDを設定する
            SetIdToNonIdNodes();

            // 親子関係を作る
            foreach (var node in UseNodeList)
            {
                CreateParentChildRelationship(node);
            }

            // 親IDを設定する
            RefreshParentIds(RootNode);

            UseNodeList.Clear();
        }

        /// <summary>
        /// 親IDを再設定する
        /// </summary>
        /// <param name="node">ノード</param>
        protected virtual void RefreshParentIds(AccordionListNode node)
        {
            if (node.ChildCount == 0)
            {
                return;
            }

            for (var i = 0; i < node.ChildrenNode.Count; i++)
            {
                var childNode = node.ChildrenNode[i];
                childNode.SetParentId(node.Id);
                RefreshParentIds(childNode);
            }
        }

        /// <summary>
        /// IDが設定されていないノードを探す
        /// </summary>
        /// <param name="node">ノード</param>
        protected virtual void SearchForNonIdNodes(AccordionListNode node)
        {
            if (node.Id == int.MinValue)
            {
                nonIdNodeList.Add(node);
            }
            else
            {
                AddToNodeIdTable(node);
            }

            if (node.ChildCount == 0)
            {
                return;
            }

            for (var i = 0; i < node.ChildrenNode.Count; i++)
            {
                var childNode = node.ChildrenNode[i];
                SearchForNonIdNodes(childNode);
            }
        }

        /// <summary>
        /// 初期表示設定のノードは親まで展開状態にする
        /// </summary>
        /// <param name="node">ノード</param>
        protected virtual void SetDefaultExpand(AccordionListNode node)
        {
            if (node.ChildCount == 0)
            {
                return;
            }

            for (var i = 0; i < node.ChildrenNode.Count; i++)
            {
                var childNode = node.ChildrenNode[i];
                if (childNode.IsExpand)
                {
                    // 設定されてるノードまでの先祖を展開済みにする
                    SetExpandAncestorsNode(childNode);
                }

                SetDefaultExpand(childNode);
            }
        }

        /// <summary>
        /// 指定したノードの先祖を展開済みにする
        /// </summary>
        /// <param name="node">ノード</param>
        protected virtual void SetExpandAncestorsNode(AccordionListNode node)
        {
            var parentNode = FindNodeWithID(node.ParentId);
            if (parentNode == null)
            {
                return;
            }

            if (parentNode.IsExpand)
            {
                return;
            }

            parentNode.IsExpand = true;
            SetExpandAncestorsNode(parentNode);
        }

        /// <summary>
        /// ノードIDとノードでマッピングする
        /// </summary>
        /// <param name="node">ノード</param>
        protected virtual void AddToNodeIdTable(AccordionListNode node)
        {
            if (NodeCaches.ContainsKey(node.Id))
            {
                throw new ArgumentException("同じノードIDが設定されています。違うノードIDを設定してください。ID=" + node.Id);
            }

            NodeCaches.Add(node.Id, node);
        }

        /// <summary>
        /// ノードの親子関係を構築
        /// </summary>
        /// <param name="node">ノード</param>
        protected virtual void CreateParentChildRelationship(AccordionListNode node)
        {
            AccordionListNode parent;
            if (NodeCaches.TryGetValue(node.ParentId, out parent))
            {
                parent.AddChild(node);
            }
            else
            {
                Debug.LogErrorFormat("can't find parent node id {0}", node.ParentId.ToString());
            }
        }

        /// <summary>
        /// 全てのセル情報を登録する
        /// </summary>
        protected virtual void AddAllCellInfo()
        {
            // RootNodeは特別なので別途セル情報を作成
            var rootCellInfo = RootNode.ToCellInfo();
            AllCellInfo.Add(new CellInfo(RootNode.Id, AccordionListCellType.Main), rootCellInfo);

            AddChildrenCellInfo(rootCellInfo);
        }

        /// <summary>
        /// 子ノードのセル情報を追加
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        protected virtual void AddChildrenCellInfo(AccordionListCellInfo cellInfo)
        {
            var node = cellInfo.Node;
            if (node.ChildCount == 0)
            {
                return;
            }

            var padding = CreateChildrenPadding(cellInfo);

            for (var i = 0; i < node.ChildCount; i++)
            {
                var childNode = node.ChildrenNode[i];
                var isFirstNode = i == 0;
                var isLastNode = i == node.ChildCount - 1;

                var mainCellInfo = CreateMainCellInfo(childNode, padding, isFirstNode, isLastNode);
                AllCellInfo.Add(new CellInfo(childNode.Id, AccordionListCellType.Main), mainCellInfo);

                if (childNode.HasChildAreaCell)
                {
                    var childAreaCellInfo = CreateChildAreaCellInfo(childNode, padding);
                    AllCellInfo.Add(new CellInfo(childNode.Id, AccordionListCellType.ChildArea), childAreaCellInfo);
                }

                var optionalCellInfo = CreateOptionalCellInfo(childNode, padding, isFirstNode, isLastNode);
                AllCellInfo.Add(new CellInfo(childNode.Id, AccordionListCellType.Optional), optionalCellInfo);

                AddChildrenCellInfo(mainCellInfo);
            }
        }

        /// <summary>
        /// IDが振られていないノードにIDを設定する
        /// -1開始で負の値で設定します
        /// </summary>
        protected virtual void SetIdToNonIdNodes()
        {
            int nextId = -1;
            foreach (var node in NonIdNodeList)
            {
                while (NodeCaches.ContainsKey(nextId))
                {
                    nextId--;
                }

                node.SetID(nextId);
                nextId--;

                AddToNodeIdTable(node);
            }

            NonIdNodeList.Clear();
        }

        /// <summary>
        /// セルを挿入します
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        protected virtual void AddCellInfo(AccordionListCellInfo cellInfo)
        {
            if (cellInfo.CellType == AccordionListCellType.ChildArea)
            {
                ChildAreaCellInfos.Add(cellInfo);
            }
            else
            {
                cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Insert;
                InsertCellInfos.Add(cellInfo);
            }

            UpdateCellInfo(cellInfo.CellPrefab, cellInfo.PrefabHash);
        }

        /// <summary>
        /// セルの情報を削除します
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        protected virtual void RemoveCellInfo(AccordionListCellInfo cellInfo)
        {
            if (cellInfo.CellType == AccordionListCellType.ChildArea)
            {
                RemoveCellInfos.Add(cellInfo);
                ChildAreaCellInfos.Remove(cellInfo);
                RemoveCellInfo(cellInfo.CellPrefab);
                cellInfo.Size = 0;
            }
            else
            {
                cellInfo.Status = AccordionListCellInfo.CellInfoStatus.Remove;
                RemoveCellInfos.Add(cellInfo);
                CellInfos.Remove(cellInfo);

                RemoveCellInfo(cellInfo.CellPrefab);
            }
        }

        /// <summary>
        /// 先祖に対象のセルが含まれているか
        /// </summary>
        /// <param name="origin">基準</param>
        /// <param name="target">対象</param>
        /// <returns>含まれているならtrue</returns>
        protected virtual bool IsContainsAncestorsCore(AccordionListNode origin, AccordionListCellInfo target)
        {
            if (origin.Id == target.Node.Id)
            {
                return true;
            }

            var cellInfo = FindCellInfo(origin.ParentId, AccordionListCellType.Main);
            if (cellInfo == null)
            {
                return false;
            }

            return IsContainsAncestorsCore(cellInfo.Node, target);
        }

#endregion

#region abstract methods

        /// <summary>
        /// メインセル情報を作成する
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="padding">セル情報のPadding</param>
        /// <param name="isFirstNode">兄弟ノード中の最初か</param>
        /// <param name="isLastNode">兄弟ノード中の最後か</param>
        /// <returns>セル情報</returns>
        protected abstract AccordionListCellInfo CreateMainCellInfo(AccordionListNode node, Padding padding, bool isFirstNode, bool isLastNode);

        /// <summary>
        /// オプショナルセル情報を作成する
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="padding">セル情報のPadding</param>
        /// <param name="isFirstNode">兄弟ノード中の最初か</param>
        /// <param name="isLastNode">兄弟ノード中の最後か</param>
        /// <returns>セル情報</returns>
        protected abstract AccordionListCellInfo CreateOptionalCellInfo(AccordionListNode node, Padding padding, bool isFirstNode, bool isLastNode);

        /// <summary>
        /// チャイルドエリアセル情報を作成する
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="padding">Padding</param>
        /// <returns>セル情報</returns>
        protected abstract AccordionListCellInfo CreateChildAreaCellInfo(AccordionListNode node, Padding padding);

        /// <summary>
        /// 全てのセルの座標を計算する
        /// </summary>
        /// <param name="padding">AccordionListの外周余白</param>
        /// <param name="cellPivot">セルPivot</param>
        public abstract void CalculateCellPosition(Padding padding, Vector2 cellPivot);

        /// <summary>
        /// 引数のセル情報の子ノードに適用するPaddingを生成する
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <returns>Padding</returns>
        protected abstract Padding CreateChildrenPadding(AccordionListCellInfo cellInfo);

        /// <summary>
        /// セルの上端を返す
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <param name="withSpacing">Spacingを含めるか</param>
        /// <returns>座標</returns>
        public abstract Vector2 CellFrontPosition(AccordionListCellInfo cellInfo, bool withSpacing);

        /// <summary>
        /// セルの末端を返す
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <param name="withSpacing">Spacingを含めるか</param>
        /// /// <returns>座標</returns>
        public abstract Vector2 CellBackPosition(AccordionListCellInfo cellInfo, bool withSpacing);

        /// <summary>
        /// セルの上端を返す
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <param name="withSpacing">Spacingを含めるか</param>
        /// <returns>値</returns>
        public abstract float SimplifyCellFrontPosition(AccordionListCellInfo cellInfo, bool withSpacing);

        /// <summary>
        /// セルの末端を返す
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <param name="withSpacing">Spacingを含めるか</param>
        /// <returns>値</returns>
        public abstract float SimplifyCellBackPosition(AccordionListCellInfo cellInfo, bool withSpacing);

#endregion
    }
}
