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
using Jigbox.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class VariableListVerticalExampleController : ExampleSceneBase
    {
        public enum CellType
        {
            Text,
            Image
        }

        [System.Serializable]
        public class CellData
        {
            public CellType CellType;
            public string CellText = string.Empty;
            public bool isOwn;
        }

        [SerializeField]
        List<CellData> initCellData = null;

        [SerializeField]
        VariableListVertical variableListVertical = null;

        [SerializeField]
        ExampleVerticalImageCell imageCell = null;

        [SerializeField]
        ExampleVerticalTextCell textCell = null;

        [SerializeField]
        InputField textInputField = null;

        [SerializeField]
        Components.TextView textView = null;

        [SerializeField]
        ToggleSwitch toggleSwitch = null;

        List<CellData> cellDataList = new List<CellData>();

        void OnUpdateCell(VariableListCell cell)
        {
            var chat = cellDataList[cell.Index];

            switch (chat.CellType)
            {
                case CellType.Text:
                    if (cell is ExampleVerticalTextCell)
                    {
                        var c = cell as ExampleVerticalTextCell;
                        c.SetText(chat.CellText, chat.isOwn);
                    }
                    break;
                case CellType.Image:
                    if (cell is ExampleVerticalImageCell)
                    {
                        var c = cell as ExampleVerticalImageCell;
                        c.SwitchView(chat.isOwn);
                    }
                    break;
            }
        }

        void OnSizeUpdateCell(VariableListCell cell)
        {
            var chat = cellDataList[cell.Index];

            switch (chat.CellType)
            {
                case CellType.Text:
                    if (cell is ExampleVerticalTextCell)
                    {
                        var c = cell as ExampleVerticalTextCell;
                        c.SetText(chat.CellText, chat.isOwn);
                        c.OnSizeUpdate(chat.isOwn);
                    }
                    break;
                case CellType.Image:
                    break;
            }
        }

        [AuthorizedAccess]
        void OnAddTextRequest()
        {
            if (textInputField.text == string.Empty)
            {
                return;
            }

            var chat = new CellData();
            chat.CellType = CellType.Text;
            chat.CellText = textInputField.text;
            chat.isOwn = toggleSwitch.IsOn;
            textInputField.text = string.Empty;

            cellDataList.Add(chat);
            variableListVertical.AddCell(textCell);
            variableListVertical.RefreshCells();
            variableListVertical.JumpByRate(1.0f);
        }

        [AuthorizedAccess]
        void OnAddImageRequest()
        {
            var chat = new CellData();
            chat.CellType = CellType.Image;
            chat.isOwn = toggleSwitch.IsOn;

            cellDataList.Add(chat);
            variableListVertical.AddCell(imageCell);
            variableListVertical.RefreshCells();
            variableListVertical.JumpByRate(1.0f);
        }

        void Start()
        {
            foreach (var cell in initCellData)
            {
                cellDataList.Add(cell);
                if (cell.CellType == CellType.Text)
                {
                    variableListVertical.AddCell(textCell);
                }

                if (cell.CellType == CellType.Image)
                {
                    variableListVertical.AddCell(imageCell);
                }
            }

            variableListVertical.AddUpdateCellEvent(OnUpdateCell);
            variableListVertical.AddUpdateCellSizeEvent(OnSizeUpdateCell);
            variableListVertical.FillCells();
        }

        void LateUpdate()
        {
            if (variableListVertical.gameObject.activeInHierarchy)
            {
                textView.Text = variableListVertical.ContentPositionRate.ToString();
            }
        }
    }
}
