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

using UnityEngine;
using Jigbox.TextView;
using Jigbox.TextView.Markup;

namespace Jigbox.Examples
{
    public class ExampleMarkupParser : MarkupParser
    {
#region protected methods

        protected override TextRun[] VisitMarkupElement(Element element, string tagNameUpper)
        {
            if (tagNameUpper == "GOLD")
            {
                return GoldElement();
            }
            else if (tagNameUpper == "CRYSTAL")
            {
                return CrystalElement();
            }
            else
            {
                return base.VisitMarkupElement(element, tagNameUpper);
            }
        }

#endregion

#region private methods

        TextRun[] GoldElement()
        {
            return new TextRun[] {
                new InlineImage() {
                    Source = "Sprites/Gold Icon",
                    Name = "gold",
                    Width = InlineImageSize.CreateByDefaultMagnificationValue(),
                    Height = InlineImageSize.CreateByDefaultMagnificationValue()
                }
            };
        }

        TextRun[] CrystalElement()
        {
            return new TextRun[] {
                new InlineImage() {
                    Source = "Sprites/Crystal Icon",
                    Name = "crystal",
                    Width = InlineImageSize.CreateByDefaultMagnificationValue(),
                    Height = InlineImageSize.CreateByDefaultMagnificationValue()
                }
            };
        }

#endregion
    }
}
