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
using ArabicUtils;
using CaretUnit = ArabicUtils.CaretUnit;

namespace Jigbox.TextView
{
    /// <summary>
    /// アラビア語専用TextCharacters
    /// </summary>
    public class ArabicTextCharacters : TextCharacters
    {
        
        public List<CaretUnit> CaretUnits { get; private set; } 
        
#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">アラビア語を含む文字列</param>
        public ArabicTextCharacters(string source) : base(ArabicTextConverter.Convert(source))
        {
            // 文字列が空の場合はConvert内部でも何もやっていない為、キャレットの追加も行わない
            if (string.IsNullOrEmpty(source))
            {
                return;
            }
            
            if (ArabicTextConverter.IsCreateCaretUnit)
            {
                CaretUnits = new List<CaretUnit>(ArabicTextConverter.CaretUnits);
            }
        }

#endregion
    }
}
