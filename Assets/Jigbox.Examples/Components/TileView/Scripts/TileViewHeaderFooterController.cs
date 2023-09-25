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
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class TileViewHeaderFooterController : ExampleSceneBase
    {
        #region Properties

        [SerializeField]
        TileViewVertical tileViewVertical = null;

        [SerializeField]
        TileViewHorizontal tileViewHorizontal = null;

        [SerializeField]
        GameObject cellPrefab = null;

        [SerializeField]
        GameObject headerVerticalPrefab = null;

        [SerializeField]
        GameObject footerVerticalPrefab = null;

        [SerializeField]
        GameObject headerHorizontalPrefab = null;

        [SerializeField]
        GameObject footerHorizontalPrefab = null;

        [SerializeField]
        int sampleCount = 256;

        [SerializeField]
        InputField indexInputField = null;

        [SerializeField]
        InputField cellOnlyRateInputField = null;

        [SerializeField]
        InputField allRateInputField = null;

        [SerializeField]
        Text scrollRateText = null;

        [SerializeField]
        Text contentPositionRateText = null;

        #endregion

        #region Fields

        readonly List<CellSampleModel> collection = new List<CellSampleModel>();

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
                if (headerVerticalPrefab)
                {
                    tileViewVertical.HeaderFooter.SetHeader(Instantiate(headerVerticalPrefab));
                }
                if (footerVerticalPrefab)
                {
                    tileViewVertical.HeaderFooter.SetFooter(Instantiate(footerVerticalPrefab));
                }
                tileViewVertical.Refresh();
            }
            if (tileViewHorizontal != null)
            {
                tileViewHorizontal.VirtualCellCount = pool.Length;
                tileViewHorizontal.FillCells(cellPrefab);
                if (headerHorizontalPrefab)
                {
                    tileViewHorizontal.HeaderFooter.SetHeader(Instantiate(headerHorizontalPrefab));
                }
                if (footerHorizontalPrefab)
                {
                    tileViewHorizontal.HeaderFooter.SetFooter(Instantiate(footerHorizontalPrefab));
                }
                tileViewHorizontal.Refresh();
            }
        }

        void LateUpdate()
        {
            if (tileViewHorizontal.gameObject.activeInHierarchy)
            {
                scrollRateText.text = tileViewHorizontal.ScrollRate.ToString();
                contentPositionRateText.text = tileViewHorizontal.ContentPositionRate.ToString();
            }

            if (tileViewVertical.gameObject.activeInHierarchy)
            {
                scrollRateText.text = tileViewVertical.ScrollRate.ToString();
                contentPositionRateText.text = tileViewVertical.ContentPositionRate.ToString();
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
            return collection.ToArray();
        }

        [AuthorizedAccess]
        void AddHeader()
        {
            if (tileViewVertical != null && headerHorizontalPrefab != null)
            {
                tileViewVertical.HeaderFooter.SetHeader(Instantiate(headerVerticalPrefab));
                tileViewVertical.RelayoutHeaderFooter();
            }
            if (tileViewHorizontal != null && headerHorizontalPrefab != null)
            {
                tileViewHorizontal.HeaderFooter.SetHeader(Instantiate(headerHorizontalPrefab));
                tileViewHorizontal.RelayoutHeaderFooter();
            }
        }

        [AuthorizedAccess]
        void AddFooter()
        {
            if (tileViewVertical != null && footerVerticalPrefab != null)
            {
                tileViewVertical.HeaderFooter.SetFooter(Instantiate(footerVerticalPrefab));
                tileViewVertical.RelayoutHeaderFooter();
            }
            if (tileViewHorizontal != null && footerHorizontalPrefab != null)
            {
                tileViewHorizontal.HeaderFooter.SetFooter(Instantiate(footerHorizontalPrefab));
                tileViewHorizontal.RelayoutHeaderFooter();
            }
        }

        [AuthorizedAccess]
        void ClearHeaderFooter()
        {
            if (tileViewVertical != null)
            {
                tileViewVertical.HeaderFooter.RemoveHeader();
                tileViewVertical.HeaderFooter.RemoveFooter();
                tileViewVertical.RefreshAllCells(cellPrefab, false);
            }
            if (tileViewHorizontal != null)
            {
                tileViewHorizontal.HeaderFooter.RemoveHeader();
                tileViewHorizontal.HeaderFooter.RemoveFooter();
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
        void JumpByRateCellOnly()
        {
            if (string.IsNullOrEmpty(cellOnlyRateInputField.text))
            {
                return;
            }

            float rate = float.Parse(cellOnlyRateInputField.text);
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

        [AuthorizedAccess]
        void JumpByRateWithHeaderFooter()
        {
            if (string.IsNullOrEmpty(allRateInputField.text))
            {
                return;
            }

            float rate = float.Parse(allRateInputField.text);
            Debug.LogFormat("[Button Event] jump rate with headerfooter: {0}", rate);

            if (tileViewHorizontal != null)
            {
                tileViewHorizontal.JumpByRateWithHeaderFooter(rate);
            }
            if (tileViewVertical != null)
            {
                tileViewVertical.JumpByRateWithHeaderFooter(rate);
            }
        }
        #endregion
    }
}
