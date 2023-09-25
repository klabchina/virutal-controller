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

using System.Linq;
using Jigbox.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Examples
{
    public class ExampleAccordionListVerticalController : ExampleSceneBase
    {
        [SerializeField]
        ExampleVerticalAccordionList accordionList = null;

        [SerializeField]
        ExampleVerticalAccordionList accordionListOptional = null;

        [SerializeField]
        ExampleWorldCategoryMainCell worldCategoryCell = null;

        [SerializeField]
        ExampleSmallAreaMainCell smallAreaCell = null;

        [SerializeField]
        ExampleWorldCategoryMainCell worldCategoryCellOptional = null;

        [SerializeField]
        ExampleSmallAreaMainCell smallAreaCellOptional = null;

        [SerializeField]
        AccordionListOptionalCell optionalCell = null;

        [SerializeField]
        AccordionListChildAreaCell childAreaCell = null;

        [SerializeField]
        ExampleSmallAreaDetailCell detailCell = null;

        [SerializeField]
        ToggleSwitch singleModeToggle = null;

        [SerializeField]
        BasicButton expandAllButton = null;

        [SerializeField]
        BasicButton collapseAllButton = null;

        [SerializeField]
        BasicButton resetButton = null;

        [SerializeField]
        InputField rateInputField = null;

        [SerializeField]
        BasicButton rateButton = null;

        [SerializeField]
        InputField nodeInputField = null;

        [SerializeField]
        BasicButton nodeButton = null;


        protected override void Awake()
        {
            base.Awake();
            InitializeAccordionList();
            InitializeAccordionListWithOptional();

            singleModeToggle.AddEvent(isOn =>
            {
                accordionList.IsSingleMode = isOn;
                accordionList.RefreshCells();

                accordionListOptional.IsSingleMode = isOn;
                accordionListOptional.RefreshCells();
            });

            expandAllButton.AddEvent(InputEventType.OnClick, () =>
            {
                accordionList.ExpandAll();
                accordionListOptional.ExpandAll();
            });

            collapseAllButton.AddEvent(InputEventType.OnClick, () =>
            {
                accordionList.CollapseAll();
                accordionListOptional.CollapseAll();
            });

            resetButton.AddEvent(InputEventType.OnClick, () =>
            {
                accordionList.Clear();
                accordionListOptional.Clear();
                InitializeAccordionList();
                InitializeAccordionListWithOptional();
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
                accordionListOptional.JumpByRate(ratio);
            });

            nodeButton.AddEvent(InputEventType.OnClick, () =>
            {
                if (string.IsNullOrEmpty(nodeInputField.text))
                {
                    return;
                }

                var nodeId = int.Parse(nodeInputField.text);
                Debug.LogFormat("[Button Event] jump nodeId: {0}", nodeId);
                accordionList.JumpByNodeId(nodeId, -1);
                accordionListOptional.JumpByNodeId(nodeId, -1);
            });
        }

        void InitializeAccordionList()
        {
            var continentNodes = ExampleDataGenerator.Continents
                .Select(c => new ExampleWorldCategoryNode(c, worldCategoryCell))
                .ToArray();

            var smallAreaNodes = ExampleDataGenerator.SmallAreas
                .Select(sa => new ExampleSmallAreaNode(sa, smallAreaCell))
                .ToArray();

            var detailNodes = ExampleDataGenerator.SmallAreas
                .Select(sa => new ExampleSmallAreaDetailNode(sa, detailCell))
                .ToArray();

            accordionList.AddNodeList(continentNodes);
            accordionList.AddNodeList(smallAreaNodes);
            accordionList.AddNodeList(detailNodes);

            accordionList.FillCells();
        }

        void InitializeAccordionListWithOptional()
        {
            var continentNodes = ExampleDataGenerator.Continents
                .Select(c => new ExampleWorldCategoryNode(c, worldCategoryCellOptional, childAreaCell, optionalCell))
                .ToArray();

            var smallAreaNodes = ExampleDataGenerator.SmallAreas
                .Select(sa => new ExampleSmallAreaNode(sa, smallAreaCellOptional, childAreaCell, optionalCell))
                .ToArray();

            var detailNodes = ExampleDataGenerator.SmallAreas
                .Select(sa => new ExampleSmallAreaDetailNode(sa, detailCell))
                .ToArray();

            accordionListOptional.AddNodeList(continentNodes);
            accordionListOptional.AddNodeList(smallAreaNodes);
            accordionListOptional.AddNodeList(detailNodes);

            accordionListOptional.FillCells();
        }
    }
}
