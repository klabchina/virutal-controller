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

using Jigbox.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class TileViewController : ExampleSceneBase
    {
#region Properties

        [SerializeField]
        TileViewVertical tileViewVertical = null;

        [SerializeField]
        TileViewHorizontal tileViewHorizontal = null;

        [SerializeField]
        GameObject cellPrefab = null;

        [SerializeField]
        int sampleCount = 256;

        [SerializeField]
        InputField indexInputField = null;

        [SerializeField]
        InputField rateInputField = null;

#endregion

#region Fields

        readonly List<CellSampleModel> collection = new List<CellSampleModel>();

        int filterMode;

        bool isOrderAscend = true;

        CellSampleModel[] pool;

#endregion

#region Unity Methods

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            sampleCount = Math.Max(0, sampleCount);
            RefreshCellSamples();
        }

#endif

        protected override void Awake()
        {
            base.Awake();
            RefreshCellSamples();
        }

        void Start()
        {
            if (tileViewVertical != null)
            {
                tileViewVertical.VirtualCellCount = pool.Length;
                tileViewVertical.FillCells(cellPrefab);
            }
            if (tileViewHorizontal != null)
            {
                tileViewHorizontal.VirtualCellCount = pool.Length;
                tileViewHorizontal.FillCells(cellPrefab);
            }
        }

#endregion

#region Public Methods

        [AuthorizedAccess]
        public void ChangeSortOrder(bool isAscend)
        {
            isOrderAscend = isAscend;
            pool = VisibleCollection();
            if (tileViewVertical != null)
            {
                tileViewVertical.RefreshAllCells(cellPrefab, false);
            }
            if (tileViewHorizontal != null)
            {
                tileViewHorizontal.RefreshAllCells(cellPrefab, false);
            }
        }

#endregion

#region Private Methods

        [AuthorizedAccess]
        void UpdateCell(GameObject go, int index)
        {
            if (index < 0 || index >= pool.Length)
            {
                return;
            }
            var view = go.GetComponent<CellSampleView>();
            if (view != null)
            {
                view.BatchModel(pool[index]);
            }
        }

        void RefreshCellSamples()
        {
            collection.Clear();
            for (int i = 0; i < sampleCount; i++)
            {
                if (collection.Count <= i)
                {
                    collection.Add(GenerateCellSample(i, 2));
                }
                else
                {
                    collection[i] = GenerateCellSample(i, 2);
                }
            }
            pool = VisibleCollection();
        }

        CellSampleModel GenerateCellSample(int index, int step)
        {
            var loopCount = (index * step) / 360;
            var hue = ((index * step) % 360) / 360f;
            var sat = Mathf.Clamp01(0.3f + loopCount * 0.1f);

            return new CellSampleModel
            {
                text = index.ToString(),
                background = Color.HSVToRGB(hue, sat, 1),
                foreground = Color.HSVToRGB(hue, 0.8f, 0.3f)
            };
        }

        /// <summary>
        /// サンプルのセルの前景色からHue(色相）を度数法(360度)で表現したものを計算します
        /// </summary>
        /// <returns>The hue.</returns>
        /// <param name="cell">Cell.</param>
        float GetHue(CellSampleModel cell)
        {
            float hue, sat, val;
            Color.RGBToHSV(cell.foreground, out hue, out sat, out val);
            return (hue * 360f) % 360f;
        }

        /// <summary>
        /// ビューが見せるサンプルのセルの配列を、コントローラーの状態に応じて構成して返します
        /// </summary>
        /// <returns>The collection.</returns>
        CellSampleModel[] VisibleCollection()
        {
            var source = isOrderAscend ? collection : collection.Reverse<CellSampleModel>();
            switch (filterMode)
            {
                case 1:  // 奇数のセル
                    return source.Where(cell => int.Parse(cell.text) % 2 != 0).ToArray();
                case 2:  // 偶数のセル
                    return source.Where(cell => int.Parse(cell.text) % 2 == 0).ToArray();
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
            if (tileViewVertical != null)
            {
                tileViewVertical.VirtualCellCount = pool.Length;
                tileViewVertical.RefreshAllCells(cellPrefab, false);
            }
            if (tileViewHorizontal != null)
            {
                tileViewHorizontal.VirtualCellCount = pool.Length;
                tileViewHorizontal.RefreshAllCells(cellPrefab, false);
            }
        }

        [AuthorizedAccess]
        void JumpByIndex()
        {
            if (string.IsNullOrEmpty(indexInputField.text))
            {
                Debug.Log("null");
                return;
            }

            int index = int.Parse(indexInputField.text);
            Debug.LogFormat("[Button Event] jump index at: {0}", index);

            if (tileViewHorizontal != null)
            {
                tileViewHorizontal.JumpByIndex(index);
            }
            if (tileViewVertical != null)
            {
                tileViewVertical.JumpByIndex(index);
            }
        }

        [AuthorizedAccess]
        void JumpByRate()
        {
            if (string.IsNullOrEmpty(rateInputField.text))
            {
                return;
            }

            float rate = float.Parse(rateInputField.text);
            Debug.LogFormat("[Button Event] jump rate: {0}", rate);

            if (tileViewHorizontal != null)
            {
                tileViewHorizontal.JumpByRate(rate);
            }
            if (tileViewVertical != null)
            {
                tileViewVertical.JumpByRate(rate);
            }
        }

#endregion
    }
}
