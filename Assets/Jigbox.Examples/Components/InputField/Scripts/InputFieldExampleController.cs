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
using System.Globalization;
using Jigbox.Components;
using Jigbox.TextView;
using UnityEngine;
using UnityEngine.UI;
using ToggleGroup = Jigbox.Components.ToggleGroup;

namespace Jigbox.Examples
{
    public class InputFieldExampleController : ExampleSceneBase
    {
        [SerializeField]
        AccordionListVertical verticalList = null;

        [SerializeField]
        TextView_InputField inputField = null;

        [SerializeField]
        InputFieldNodeView nodeView = null;

        [SerializeField]
        BasicButton sendButton = null;

        [SerializeField]
        GameObjectToggle readonlyToggle = null;

        [SerializeField]
        GameObjectToggle multilineToggle = null;

        [SerializeField]
        ToggleGroup languageToggleGroup = null;

        [SerializeField]
        Font defaultFont = null;

        [SerializeField]
        Font thaiFont = null;

        [SerializeField]
        Font arabicFont = null;

        readonly List<InputFieldNode> nodeList = new List<InputFieldNode>();

        // Start is called before the first frame update
        void Start()
        {
            inputField.AddOnEndEditEvent(str => { Debug.Log("[OnEndEdit] = " + str); });

            inputField.AddOnValueChangedEvent(str => { Debug.Log("[OnValueChanged] str = " + str); });

            inputField.AddOnValidateInputEvent((text, index, addedChar) =>
            {
                var unicodeCategory = char.GetUnicodeCategory(addedChar);
                Debug.Log("[Validate] str = " + text + ", index = " + index + ", addedChar = " + addedChar + ", unicode = " + ((int) addedChar).ToString("\\u{0:x4}") + ", unicodeCategory = " + unicodeCategory);

                return addedChar;
            });

            sendButton.AddEvent(InputEventType.OnClick, () =>
            {
                verticalList.Clear();
                var node = new InputFieldNode(nodeView, nodeList.Count)
                {
                    LanguageFont = GetLanguageFont(languageToggleGroup.ActiveToggleIndex),
                    LanguageType = GetLanguageType(languageToggleGroup.ActiveToggleIndex),
                    Text = inputField.Text,
                };
                nodeList.Add(node);
                verticalList.AddNodeList(nodeList);
                verticalList.FillCells();
            });

            readonlyToggle.AddEvent((isOn) => { inputField.ReadOnly = isOn; });

            multilineToggle.AddEvent((isOn) =>
            {
                if (isOn)
                {
                    inputField.LineType = InputField.LineType.MultiLineNewline;
                    inputField.TextViewComponent.HorizontalOverflow = HorizontalWrapMode.Wrap;
                }
                else
                {
                    inputField.LineType = InputField.LineType.SingleLine;
                    inputField.TextViewComponent.HorizontalOverflow = HorizontalWrapMode.Overflow;
                }
            });

            languageToggleGroup.AddEvent(index =>
            {
                inputField.LanguageType = GetLanguageType(index);
                inputField.TextViewComponent.Font = GetLanguageFont(index);
                inputField.TextViewComponent.Alignment = inputField.LanguageType == TextLanguageType.Arabic ? TextAlign.Right : TextAlign.Left;
                inputField.Text = string.Empty;
            });

            inputField.ReadOnly = readonlyToggle.IsOn;
            inputField.LineType = multilineToggle.IsOn ? InputField.LineType.MultiLineNewline : InputField.LineType.SingleLine;
        }

        TextLanguageType GetLanguageType(int index)
        {
            switch (index)
            {
                case 0:
                    return TextLanguageType.Default;
                case 1:
                    return TextLanguageType.Thai;
                case 2:
                    return TextLanguageType.Arabic;
            }

            Debug.LogWarning("LanguageTypeが見つかりませんでした");
            return TextLanguageType.Default;
        }

        Font GetLanguageFont(int index)
        {
            switch (index)
            {
                case 0:
                    return defaultFont;
                case 1:
                    return thaiFont;
                case 2:
                    return arabicFont;
            }

            Debug.LogWarning("Fontが見つかりませんでした");
            return defaultFont;
        }
    }
}
