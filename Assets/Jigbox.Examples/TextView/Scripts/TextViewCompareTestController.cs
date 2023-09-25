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

using Jigbox.TextView;
using System;
using UnityEngine;
using UnityEngine.UI;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Jigbox.Examples
{
    public sealed class TextViewCompareTestController : MonoBehaviour
    {
#region constants

        readonly static string ViewSizeTextFormat = "Text Size = {0} x {1}";

#endregion

#region properties

        [SerializeField]
        Components.TextView viewSizeText = null;

        [SerializeField]
        Text[] textSamples = null;

        [SerializeField]
        Components.TextView[] textViewSamples = null;

        [SerializeField]
        InputField inputField = null;

        [SerializeField]
        Text lineBreakRuleName = null;

        bool isValueTextUpdate;

        Vector2 textViewSize;


        int ruleMaxNum;
        int ruleIndex;

#endregion

#region unity methods

        void Start()
        {
            ruleMaxNum = Enum.GetValues(typeof(TextLineBreakRule)).Length;
            ChangeLineBreakRule((int) TextLineBreakRule.Japanese);
            ChangeText(string.Empty);
        }

        void Update()
        {
            var size = textViewSamples[0].rectTransform.rect.size;
            if (textViewSize != size)
            {
                textViewSize = size;
                viewSizeText.Text = string.Format(ViewSizeTextFormat, (int) textViewSize.x, (int) textViewSize.y);
            }

            if (!isValueTextUpdate)
            {
                return;
            }

            ChangeText(inputField.text);
            isValueTextUpdate = false;
        }

#endregion

#region public methods

        /// <summary>
        /// InputFieldの値が変更された時に呼ばれます
        /// </summary>
        public void OnChangeValueText()
        {
            // 一度に大量の文字列更新が走ると(コピペしたときなど)動作が極端に重くなるためフレーム単位で更新をかけるようにする
            isValueTextUpdate = true;
        }

#endregion

#region private methods

        /// <summary>
        /// Textの中身を書き換えます
        /// </summary>
        void ChangeText(string value)
        {
            foreach (var text in textSamples)
            {
                text.text = value;
            }
            foreach (var text in textViewSamples)
            {
                text.Text = value;
            }
        }

        /// <summary>
        /// LineBreakRuleのボタンが押された際に呼ばれます
        /// </summary>
        [AuthorizedAccess]
        void OnNextLineBreakRule()
        {
            ChangeLineBreakRule(ruleIndex + 1);
        }

        /// <summary>
        /// LineBreakRuleを切り替えます
        /// </summary>
        void ChangeLineBreakRule(int index)
        {
            ruleIndex = index >= ruleMaxNum ? 0 : index;
            var rule = (TextLineBreakRule) Enum.ToObject(typeof(TextLineBreakRule), ruleIndex);
            foreach (var text in textViewSamples)
            {
                text.LineBreakRule = rule;
            }
            lineBreakRuleName.text = rule.ToString();
        }

        /// <summary>
        /// Backボタンが押された時に呼ばれます
        /// </summary>
        [AuthorizedAccess]
        void BackScene()
        {
            SceneManager.LoadScene(0);
        }

#endregion
    }
}
