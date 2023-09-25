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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// VariableListCellInfoをリストで保持しつつユーティリティを提供するクラス
    /// </summary>
    public class VariableListCellInfoModel
    {
#region fields & properties

        /// <summary>
        /// セルの情報リスト
        /// </summary>
        protected List<VariableListCellInfo> cellInfos = new List<VariableListCellInfo>();

        /// <summary>
        /// セルの数をHashCodeをKeyとして保持
        /// </summary>
        protected Dictionary<int, int> cellCounts = new Dictionary<int, int>();

        /// <summary>
        /// セルのPrefabの種類をHashSetで保持
        /// </summary>
        protected HashSet<VariableListCell> cellPrefabs = new HashSet<VariableListCell>();

#endregion

#region protected methods

        /// <summary>
        /// セルの関連情報を更新します
        /// </summary>
        /// <param name="cellPrefab">セルのPrefab</param>
        protected virtual void UpdateCellInfo(VariableListCell cellPrefab)
        {
            if (!cellPrefabs.Contains(cellPrefab))
            {
                cellPrefabs.Add(cellPrefab);
            }

            var hashCode = cellPrefab.GetHashCode();

            if (cellCounts.ContainsKey(hashCode))
            {
                cellCounts[hashCode]++;
                return;
            }

            cellCounts.Add(hashCode, 1);
        }

        /// <summary>
        /// セルの関連情報を更新し、必要な場合削除します
        /// </summary>
        /// <param name="cellPrefab"></param>
        protected virtual void RemoveCellInfo(VariableListCell cellPrefab)
        {
            var hashCode = cellPrefab.GetHashCode();

            cellCounts[hashCode]--;

            if (cellCounts[hashCode] == 0)
            {
                cellPrefabs.Remove(cellPrefab);
                cellCounts.Remove(hashCode);
            }
        }

#endregion

#region public methods

        /// <summary>
        /// Indexに紐づいたPrefabの情報を追加します
        /// </summary>
        /// <param name="cellPrefab">Prefabの種類</param>
        public virtual void Add(VariableListCell cellPrefab)
        {
            Insert(Count(), cellPrefab);
        }

        /// <summary>
        /// セルを挿入します
        /// </summary>
        /// <param name="index">挿入先番号</param>
        /// <param name="cellPrefab">セルのPrefab</param>
        public virtual void Insert(int index, VariableListCell cellPrefab)
        {
            cellInfos.Insert(index, new VariableListCellInfo(index, cellPrefab));

            for (var i = index + 1; i < cellInfos.Count; i++)
            {
                cellInfos[i].Index++;
            }

            UpdateCellInfo(cellPrefab);
        }

        /// <summary>
        /// セルの情報を削除します
        /// </summary>
        /// <param name="index">セルの番号</param>
        public virtual void Remove(int index)
        {
            var cellPrefab = cellInfos[index].CellPrefab;

            cellInfos.RemoveAt(index);

            for (var i = index; i < cellInfos.Count; i++)
            {
                cellInfos[i].Index--;
            }

            RemoveCellInfo(cellPrefab);
        }

        /// <summary>
        /// セルの情報を全て削除します
        /// </summary>
        public virtual void Clear()
        {
            cellInfos.Clear();
            cellCounts.Clear();
            cellPrefabs.Clear();
        }

        /// <summary>
        /// セルの情報の数を返します
        /// </summary>
        /// <returns>セルの数</returns>
        public virtual int Count()
        {
            return cellInfos.Count;
        }

        /// <summary>
        /// セルの情報を取得します
        /// </summary>
        /// <param name="index">セル番号</param>
        /// <returns>セルの情報</returns>
        public virtual VariableListCellInfo Get(int index)
        {
            return cellInfos[index];
        }

        /// <summary>
        /// セルの情報リストを取得します
        /// </summary>
        /// <returns>セルの情報リスト</returns>
        public virtual IEnumerable<VariableListCellInfo> GetCellInfos()
        {
            return cellInfos;
        }

        /// <summary>
        /// 保持しているセルのPrefabの種類への参照
        /// </summary>
        public virtual IEnumerable<VariableListCell> GetCellPrefabs()
        {
            return cellPrefabs;
        }

        /// <summary>
        /// 指定された種類のPrefabで紐づいているセルの数を取得します。
        /// </summary>
        /// <param name="cellPrefab">Prefabの種類</param>
        /// <returns>セルの数</returns>
        public virtual int CellCountByPrefab(VariableListCell cellPrefab)
        {
            var hashCode = cellPrefab.GetHashCode();

            if (!cellCounts.ContainsKey(hashCode))
            {
                Debug.LogError("invalid args");
                return 0;
            }

            return cellCounts[hashCode];
        }

        /// <summary>
        /// 対象Prefabのセルの情報を返します
        /// </summary>
        /// <param name="cellPrefab">セルのPrefab</param>
        /// <returns>セルの情報</returns>
        protected virtual IEnumerable<VariableListCellInfo> CellInfosByPrefab(VariableListCell cellPrefab)
        {
            List<VariableListCellInfo> targets = new List<VariableListCellInfo>();
            var prefabHash = cellPrefab.GetHashCode();

            foreach (var cellInfo in cellInfos)
            {
                if (cellInfo.PrefabHash == prefabHash)
                {
                    targets.Add(cellInfo);
                }
            }

            return targets;
        }

        /// <summary>
        /// セルの最小サイズを返します
        /// </summary>
        /// <returns>セルの最小サイズ</returns>
        public virtual float MinimumCellSizeByPrefab(VariableListCell cellPrefab)
        {
            var minimumSize = float.MaxValue;
            var cellInfos = CellInfosByPrefab(cellPrefab);

            foreach (var info in cellInfos)
            {
                if (minimumSize > info.Size)
                {
                    minimumSize = info.Size;
                }
            }

            return minimumSize;
        }

        /// <summary>
        /// セルのSpacingを含めた合計サイズを返します
        /// </summary>
        /// <returns>セルの合計サイズ</returns>
        public virtual float TotalSize()
        {
            if (Count() == 0)
            {
                return 0.0f;
            }

            var spacing = cellInfos[0].SpacingBack;
            var totalSize = cellInfos[0].Size;

            // 前方間隔と後方間隔ではどちらか大きい方のみを使用するため、Spacingの値を書き換えている
            for (var i = 1; i < cellInfos.Count; i++)
            {
                if (spacing < cellInfos[i].SpacingFront)
                {
                    spacing = cellInfos[i].SpacingFront;
                }

                totalSize += cellInfos[i].Size + spacing;

                spacing = cellInfos[i].SpacingBack;
            }

            return totalSize;
        }

#endregion
    }
}
