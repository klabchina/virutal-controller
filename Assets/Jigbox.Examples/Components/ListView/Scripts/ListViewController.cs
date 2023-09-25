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
using System.Linq;
using System.Collections.Generic;
using Jigbox.Components;

namespace Jigbox.Examples
{
    public class ListViewController : ExampleSceneBase
    {
#region Constants

        /// <summary>
        /// 黄金角
        /// </summary>
        readonly float alpha = (360f / (1 + (1 + Mathf.Sqrt(5) / 2.0f)));

#endregion

#region Serialize Fields

        [SerializeField]
        ListViewVertical listViewVertical = null;

        [SerializeField]
        ListViewHorizontal listViewHorizontal = null;

        [SerializeField]
        GameObject cellPrefabForVertical = null;

        [SerializeField]
        GameObject cellPrefabForHorizontal = null;

        [SerializeField]
        InputField jumpIndexInput = null;

        [SerializeField]
        InputField jumpRatioInput = null;

        [SerializeField]
        int sampleCount = 512;

#endregion

#region Fields

        readonly List<ListItemModelSample> collection = new List<ListItemModelSample>();

        int filterMode;

        bool isOrderAscend = true;

        ListItemModelSample[] pool;

#endregion

#region Event Handler

        [AuthorizedAccess]
        public void UpdateCell(GameObject cell, int index)
        {
            if (index < 0 || index >= pool.Length)
            {
                return;
            }
            var view = cell.GetComponent<ListItemViewSample>();
            if (view)
            {
                view.BatchModel(pool[index]);
            }
        }

        [AuthorizedAccess]
        public void ChangeSortOrder(bool isAscend)
        {
            isOrderAscend = isAscend;
            pool = VisibleCollection();
            if (listViewVertical != null)
            {
                listViewVertical.RefreshAllCells(cellPrefabForVertical, false);
            }
            if (listViewHorizontal != null)
            {
                listViewHorizontal.RefreshAllCells(cellPrefabForHorizontal, false);
            }
        }

        [AuthorizedAccess]
        public void JumpByIndex()
        {
            if (jumpIndexInput == null || string.IsNullOrEmpty(jumpIndexInput.text))
            {
                return;
            }

            var index = int.Parse(jumpIndexInput.text);
            Debug.LogFormat("[Botton Event] jump index at: {0}", index);

            if (listViewVertical != null)
            {
                listViewVertical.JumpByIndex(index);
            }
            if (listViewHorizontal != null)
            {
                listViewHorizontal.JumpByIndex(index);
            }
        }

        [AuthorizedAccess]
        public void JumpByRatio()
        {
            if (jumpRatioInput == null || string.IsNullOrEmpty(jumpRatioInput.text))
            {
                return;
            }

            var ratio = float.Parse(jumpRatioInput.text);
            Debug.LogFormat("[Botton Event] jump rate: {0}", ratio);

            if (listViewVertical != null)
            {
                listViewVertical.JumpByRate(ratio);
            }
            if (listViewHorizontal != null)
            {
                listViewHorizontal.JumpByRate(ratio);
            }
        }

#endregion

#region Private Methods

        /// <summary>
        /// サンプルのセルの前景色からHue(色相）を度数法(360度)で表現したものを計算します
        /// </summary>
        /// <returns>The hue.</returns>
        /// <param name="cell">Cell.</param>
        float GetHue(ListItemModelSample cell)
        {
            float hue, sat, val;
            Color.RGBToHSV(cell.Color, out hue, out sat, out val);
            return (hue * 360f) % 360f;
        }

        /// <summary>
        /// ビューが見せるサンプルのセルの配列を、コントローラーの状態に応じて構成して返します
        /// </summary>
        /// <returns>The collection.</returns>
        ListItemModelSample[] VisibleCollection()
        {
            var source = isOrderAscend ? collection : collection.Reverse<ListItemModelSample>().ToList();
            switch (filterMode)
            {
                case 1:  // 奇数のセル
                    return source.Where(cell => cell.Index % 2 != 0).ToArray();
                case 2:  // 偶数のセル
                    return source.Where(cell => cell.Index % 2 == 0).ToArray();
                case 3:  // 赤いセル
                    return source.Where(cell =>
                    {
                        var hue = GetHue(cell);
                        return (hue >= 0 && hue < 20) || (hue > 340 && hue <= 360);
                    }).ToArray();
                case 4:  // 黄色いセル
                    return source.Where(cell =>
                    {
                        var hue = GetHue(cell);
                        return (hue >= 35 && hue < 60);
                    }).ToArray();
                case 5:  // 緑色のセル
                    return source.Where(cell =>
                    {
                        var hue = GetHue(cell);
                        return (hue >= 75 && hue < 150);
                    }).ToArray();
                default: // 0, フィルターなし
                    return source.ToArray();
            }
        }

        void RefreshCellSamples()
        {
            collection.Clear();
            var step = 0.2f;
            for (int i = 0; i < sampleCount; i++)
            {
                if (collection.Count <= i)
                {
                    collection.Add(GenerateItem(i, i * alpha * step));
                }
                else
                {
                    collection[i] = GenerateItem(i, i * alpha * step);
                }
            }
            pool = VisibleCollection();
        }

        /// <summary>
        /// セルの中身のデータモデルインスタンスを生成します
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="theta"></param>
        ListItemModelSample GenerateItem(int index, float theta)
        {
            var loopCount = theta / 360f;
            var hue = (theta % 360) / 360f;
            var sat = Mathf.Clamp01(0.5f + loopCount * 0.1f);

            var color = Color.HSVToRGB(hue, sat, 0.5f);
            return new ListItemModelSample(index, color);
        }

        [AuthorizedAccess]
        void ChangeNotFilter()
        {
            ChangeFilter(0);
        }

        [AuthorizedAccess]
        void ChangeOddNumberFilter()
        {
            ChangeFilter(1);
        }

        [AuthorizedAccess]
        void ChangeEvenNumberFilter()
        {
            ChangeFilter(2);
        }

        void ChangeFilter(int filterMode)
        {
            this.filterMode = filterMode;
            pool = VisibleCollection();
            if (listViewVertical != null)
            {
                listViewVertical.VirtualCellCount = pool.Length;
                listViewVertical.RefreshAllCells(cellPrefabForVertical, false);
            }
            if (listViewHorizontal != null)
            {
                listViewHorizontal.VirtualCellCount = pool.Length;
                listViewHorizontal.RefreshAllCells(cellPrefabForHorizontal, false);
            }
        }

#endregion

#region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            RefreshCellSamples();
        }

        void Start()
        {
            if (listViewVertical != null) 
            {
                listViewVertical.VirtualCellCount = pool.Length;
                listViewVertical.FillCells(cellPrefabForVertical);
            }
            if (listViewHorizontal != null)
            {
                listViewHorizontal.VirtualCellCount = pool.Length;
                listViewHorizontal.FillCells(cellPrefabForHorizontal);
            }
        }

#endregion
    }
}
