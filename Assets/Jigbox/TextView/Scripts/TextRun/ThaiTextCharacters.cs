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
using ThaiUtils;
using CaretUnit = ThaiUtils.CaretUnit;

namespace Jigbox.TextView
{
    /// <summary>
    /// タイ語専用TextCharacters
    /// </summary>
    public class ThaiTextCharacters : TextRun
    {
#region properties

        /// <summary>文字列長</summary>
        public override int Length { get { return length; } }

        /// <summary>テキスト</summary>
        public List<TextUnit> Texts { get; private set; }

        int length = 0;

        /// <summary>キャレット情報</summary>
        public List<CaretUnit> CaretUnits { get; private set; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">タイ語を含む文字列</param>
        public ThaiTextCharacters(string source)
        {
            ThaiTextConverter.Convert(source);
            Texts = new List<TextUnit>(ThaiTextConverter.Texts);
            for (int i = 0; i < Texts.Count; ++i)
            {
                length += Texts[i].data.Length;
            }

            if (ThaiTextConverter.IsCreateCaretUnit)
            {
                CaretUnits = new List<CaretUnit>(ThaiTextConverter.CaretUnits);
            }
        }

#endregion
    }
}
