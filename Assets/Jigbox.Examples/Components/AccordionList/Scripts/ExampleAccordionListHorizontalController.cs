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
using Jigbox.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class ExampleAccordionListHorizontalController : MonoBehaviour
    {
        /// <summary>
        /// 黄金角
        /// </summary>
        readonly float alpha = (360f / (1 + (1 + Mathf.Sqrt(5) / 2.0f)));

        [SerializeField]
        AccordionListHorizontal accordionList = null;

        [SerializeField]
        ExampleAccordionListHorizontalCell horizontalCell = null;

        [SerializeField]
        Components.TextView contentPositionTextView = null;

        [SerializeField]
        ToggleSwitch singleModeToggle = null;

        [SerializeField]
        BasicButton expandAllButton = null;

        [SerializeField]
        BasicButton collapseAllButton = null;
        
        [SerializeField]
        InputField rateInputField = null;

        [SerializeField]
        BasicButton rateButton = null;

        void Awake()
        {
            InitializeCell();

            singleModeToggle.AddEvent(isOn =>
            {
                accordionList.IsSingleMode = isOn;
                accordionList.RefreshCells();
            });
            
            rateButton.AddEvent(InputEventType.OnClick, () =>
            {
                if (string.IsNullOrEmpty(rateInputField.text))
                {
                    return;
                }

                var ratio = float.Parse(rateInputField.text);
                Debug.LogFormat("[Button Event] jump rate: {0}", ratio);

                accordionList.JumpByRate(ratio);
            });


            expandAllButton.AddEvent(InputEventType.OnClick, () => { accordionList.ExpandAll(); });

            collapseAllButton.AddEvent(InputEventType.OnClick, () => { accordionList.CollapseAll(); });
        }

        void InitializeCell()
        {
            var step = 0.2f;
            for (var j = 1; j <= 10; j++)
            {
                var parent = new ExampleAccordionListColorSampleNode(GenerateItem(j, j * alpha * step), horizontalCell);
                for (int i = 1; i <= 10; i++)
                {
                    var child = new ExampleAccordionListColorSampleNode(GenerateItem(j * 10 + i, i * alpha * step), horizontalCell);

                    parent.AddChild(child);
                }

                accordionList.AddNodeList(new[] {parent});
            }

            accordionList.FillCells();
        }


        /// <summary>
        /// セルの中身のデータモデルインスタンスを生成します
        /// </summary>
        /// <param name="index"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        ExampleColorItem GenerateItem(int index, float theta)
        {
            var loopCount = theta / 360f;
            var hue = (theta % 360) / 360f;
            var sat = Mathf.Clamp01(0.5f + loopCount * 0.1f);

            var color = Color.HSVToRGB(hue, sat, 0.5f);
            return new ExampleColorItem()
            {
                Index = index,
                Color = color,
            };
        }


        void Update()
        {
            contentPositionTextView.Text = accordionList.ContentPositionRate.ToString();
        }
    }
}
