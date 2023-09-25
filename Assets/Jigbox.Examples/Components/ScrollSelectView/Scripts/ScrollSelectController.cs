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
using Jigbox.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class ScrollSelectController : ExampleSceneBase
    {
#region Constants

        /// <summary>
        /// 黄金角
        /// </summary>
        readonly float alpha = (360f / (1 + (1 + Mathf.Sqrt(5) / 2.0f)));

#endregion

        [SerializeField]
        ScrollSelectViewVertical scrollSelectViewVertical = null;

        [SerializeField]
        GameObject cellPrefabForVertical = null;

        [SerializeField]
        ScrollSelectJacketViewSample jacketView = null;

        [SerializeField]
        ScrollSelectViewHorizontal scrollSelectViewHorizontal = null;

        [SerializeField]
        GameObject cellPrefabForHorizontal = null;
        
        [SerializeField]
        ScrollSelectJacketViewSample horizontalJacketView = null;
        
        [SerializeField]
        int sampleCount = 512;

        [SerializeField]
        InputField jumpIndexInput = null;

        [SerializeField]
        InputField moveIndexInput = null;

        [SerializeField]
        bool outputLog = true;

        readonly List<ScrollSelectItemModelSample> collection = new List<ScrollSelectItemModelSample>();

        ScrollSelectItemModelSample[] pool;

        [AuthorizedAccess]
        public void UpdateCell(GameObject cell, int index)
        {
            var view = cell.GetComponent<ScrollSelectItemViewSample>();
            if (view)
            {
                view.BatchModel(pool[index]);
            }
        }

        /// <summary>
        /// ビューが見せるサンプルのセルの配列を、コントローラーの状態に応じて構成して返します
        /// </summary>
        /// <returns>The collection.</returns>
        ScrollSelectItemModelSample[] VisibleCollection()
        {
            return collection.ToArray();
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
        ScrollSelectItemModelSample GenerateItem(int index, float theta)
        {
            var loopCount = theta / 360f;
            var hue = (theta % 360) / 360f;
            var sat = Mathf.Clamp01(0.5f + loopCount * 0.1f);

            var color = Color.HSVToRGB(hue, sat, 0.5f);
            return new ScrollSelectItemModelSample(index, color);
        }

        protected override void Awake()
        {
            base.Awake();
            RefreshCellSamples();
        }

        void Start()
        {
            scrollSelectViewVertical.VirtualCellCount = pool.Length;
            scrollSelectViewVertical.FillCells(cellPrefabForVertical);
            scrollSelectViewVertical.RelayoutAllCells();
 
            scrollSelectViewHorizontal.VirtualCellCount = pool.Length;
            scrollSelectViewHorizontal.FillCells(cellPrefabForHorizontal);
            scrollSelectViewHorizontal.RelayoutAllCells();
        }

        [AuthorizedAccess]
        void JumpByIndex()
        {
            if (jumpIndexInput == null || string.IsNullOrEmpty(jumpIndexInput.text))
            {
                return;
            }

            var index = int.Parse(jumpIndexInput.text);
            if (outputLog)
            {
                Debug.LogFormat("[Botton Event] jump index at: {0}", index);
            }
            scrollSelectViewVertical.JumpByIndex(index);
            scrollSelectViewHorizontal.JumpByIndex(index);
        }

        [AuthorizedAccess]
        void MoveByIndex()
        {
            if (moveIndexInput == null || string.IsNullOrEmpty(moveIndexInput.text))
            {
                return;
            }

            var index = int.Parse(moveIndexInput.text);
            if (outputLog)
            {
                Debug.LogFormat("[Botton Event] move index at: {0}", index);
            }

            // 移動する時間を移動量に対して比例させるため
            // 移動するセルの数を求める
            var vCount = scrollSelectViewVertical.GetMoveIndexCount(index);
            var hCount = scrollSelectViewHorizontal.GetMoveIndexCount(index);
            // セル1つ分の移動に0.2秒の時間をかける
            scrollSelectViewVertical.MoveByIndex(index, vCount * 0.2f);
            scrollSelectViewHorizontal.MoveByIndex(index, hCount * 0.2f);
        }

        [AuthorizedAccess]
        void OnAdjustCompleteVertical(GameObject cell, int index)
        {
            if (outputLog)
            {
                Debug.Log("Vertical AdjustComplete index " + index);
            }
        }

        [AuthorizedAccess]
        void OnAdjustCompleteHorizontal(GameObject cell, int index)
        {
            if (outputLog)
            {
                Debug.Log("Horizontal AdjustComplete index " + index);
            }
        }

        [AuthorizedAccess]
        void OnSelectVertical(GameObject cell, int index)
        {
            if (jacketView)
            {
                jacketView.ChangeJacket(pool[index]);
            }
        }

        [AuthorizedAccess]
        void OnSelectHorizontal(GameObject cell, int index)
        {
            if (horizontalJacketView)
            {
                horizontalJacketView.ChangeJacket(pool[index]);
            }
        }

        [AuthorizedAccess]
        void OnDeselect(GameObject cell, int index)
        {
            if (outputLog)
            {
                Debug.Log("OnDeselect index " + index);
            }
        }

        [AuthorizedAccess]
        void OnRelayoutLoop()
        {
            scrollSelectViewVertical.RelayoutAllCells(true);
            scrollSelectViewHorizontal.RelayoutAllCells(true);
        }

        [AuthorizedAccess]
        void OnRelayoutNonLoop()
        {
            scrollSelectViewVertical.RelayoutAllCells(false);
            scrollSelectViewHorizontal.RelayoutAllCells(false);
        }

        [AuthorizedAccess]
        void OnMoveNextIndex()
        {
            scrollSelectViewVertical.MoveByOffset(1, 0.2f);
            scrollSelectViewHorizontal.MoveByOffset(1, 0.2f);
        }

        [AuthorizedAccess]
        void OnMovePrevIndex()
        {
            scrollSelectViewVertical.MoveByOffset(-1, 0.2f);
            scrollSelectViewHorizontal.MoveByOffset(-1, 0.2f);
        }
    }
}
