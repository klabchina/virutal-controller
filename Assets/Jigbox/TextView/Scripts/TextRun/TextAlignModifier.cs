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

namespace Jigbox.TextView
{
    /// <summary>
    /// アライメントを変更する要素
    /// </summary>
    public class TextAlignModifier : TextRun
    {
#region properties

        /// <summary>長さ</summary>
        public override int Length { get { return 0; } }

        /// <summary>アライメント</summary>
        public TextAlign Align { get; private set; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="align">アライメント</param>
        public TextAlignModifier(string align)
        {
            switch (align)
            {
                case "l":
                case "left":
                    Align = TextAlign.Left;
                    break;
                case "r":
                case "right":
                    Align = TextAlign.Right;
                    break;
                case "c":
                case "center":
                    Align = TextAlign.Center;
                    break;
                default:
                    Align = TextAlign.Left;
                    break;
            }
        }

#endregion
    }
}
