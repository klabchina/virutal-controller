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
using System.Linq;
using Jigbox.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class VariableListHorizontalExampleController : MonoBehaviour
    {
        [SerializeField]
        VariableListHorizontal variableListHorizontal = null;

        [SerializeField]
        Components.TextView textView = null;

        [SerializeField]
        ExampleHorizontalTextCell textCell = null;

        [SerializeField]
        InputField textInputField = null;

        [SerializeField]
        InputField indexInputField = null;

        [SerializeField]
        InputField countInputField = null;

        [SerializeField]
        InputField rateInputField = null;

        [SerializeField]
        int sampleCount = 3;

        List<string> cellData = new List<string>();

        void OnUpdateCell(VariableListCell variableListCell)
        {
            if (variableListCell is ExampleHorizontalTextCell)
            {
                var cell = variableListCell as ExampleHorizontalTextCell;
                cell.OnUpdate(cellData[cell.Index] + Environment.NewLine +  "CurrentIndex : " + cell.Index);
            }
        }

        [AuthorizedAccess]
        void OnInsertText()
        {
            int index;
            int.TryParse(indexInputField.text, out index);

            int count;
            if (!int.TryParse(countInputField.text, out count))
            {
                count = 1;
            }

            cellData.InsertRange(index, Enumerable.Repeat(textInputField.text, count));

            variableListHorizontal.InsertCells(index, Enumerable.Repeat(textCell, count).Cast<VariableListCell>());
            variableListHorizontal.RefreshCells();
        }

        [AuthorizedAccess]
        void OnRemoveText()
        {
            int index;
            int.TryParse(indexInputField.text, out index);

            int count;
            if (!int.TryParse(countInputField.text, out count))
            {
                count = 1;
            }

            // セルの数以上消そうとしていたら例外吐かないように丸める
            if (index + count > cellData.Count)
            {
                count = cellData.Count - index;
            }

            cellData.RemoveRange(index, count);

            variableListHorizontal.RemoveCells(index, count);
            variableListHorizontal.RefreshCells(true);
        }

        [AuthorizedAccess]
        void OnClearText()
        {
            cellData.Clear();
            variableListHorizontal.ClearAllCells();
        }

        [AuthorizedAccess]
        void OnJumpByIndex()
        {
            int index;
            int.TryParse(indexInputField.text, out index);

            variableListHorizontal.JumpByIndex(index);
        }

        [AuthorizedAccess]
        void OnJumpByRate()
        {
            float rate;
            float.TryParse(rateInputField.text, out rate);

            variableListHorizontal.JumpByRate(rate);
        }

        void Start()
        {
            string cellText = string.Empty;

            for (int i = 0; i < sampleCount; i++)
            {
                cellText = "Cell Sample" + Environment.NewLine +  "InitIndex : " + i;
                cellData.Add(cellText);

                variableListHorizontal.AddCell(textCell);
            }

            variableListHorizontal.AddUpdateCellEvent(OnUpdateCell);
            variableListHorizontal.AddUpdateCellSizeEvent(OnUpdateCell);
            variableListHorizontal.FillCells();
        }

        void LateUpdate()
        {
            if (variableListHorizontal.gameObject.activeInHierarchy)
            {
                textView.Text = variableListHorizontal.ContentPositionRate.ToString();
            }
        }
    }
}
