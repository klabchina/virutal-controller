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

using ArabicUtils;
using Jigbox.TextView;
using Jigbox.TextView.Markup;
using ThaiUtils;

namespace Jigbox.Components
{
    /// <summary>
    /// TextView_InputField用のクラス
    /// </summary>
    public class InputMarkupParser : MarkupParser
    {
        protected override TextSource ConvertNodeToTextSource(MarkupNode rootNode, string errorMessage, TextLanguageType languageType)
        {
            ArabicTextConverter.IsCreateCaretUnit = languageType == TextLanguageType.Arabic;
            ThaiTextConverter.IsCreateCaretUnit = languageType == TextLanguageType.Thai;

            TextViewCaretUnitProxy.Initialize();

            TextSource textSourceResult = base.ConvertNodeToTextSource(rootNode, errorMessage, languageType);

            foreach (var textRun in textSourceResult.TextRuns)
            {
                if (textRun is LineBreak)
                {
                    TextViewCaretUnitProxy.AddFirstUnit();
                }
                else if (textRun is ArabicTextCharacters)
                {
                    var arabic = (ArabicTextCharacters) textRun;
                    // アラビア語は空文字でもArabicTextCharactersが作られてしまう
                    // CaretUnitは空文字は入れるデータがない為null判定を行っている
                    if (arabic.CaretUnits != null)
                    {
                        TextViewCaretUnitProxy.AddArabicCaretUnits(arabic.CaretUnits);    
                    }
                }
                else if (textRun is ThaiTextCharacters)
                {
                    var thai = (ThaiTextCharacters) textRun;
                    TextViewCaretUnitProxy.AddThaiCaretUnits(thai.CaretUnits);
                }
                else if (textRun is TextCharacters)
                {
                    var textCharacters = (TextCharacters) textRun;
                    TextViewCaretUnitProxy.GenerateDefaultCaretUnits(textCharacters.RawCharacters);
                }
            }

            TextViewCaretUnitProxy.AddAllDefaultCaretUnits();

            ArabicTextConverter.IsCreateCaretUnit = false;
            ThaiTextConverter.IsCreateCaretUnit = false;
            return textSourceResult;
        }
    }
}
