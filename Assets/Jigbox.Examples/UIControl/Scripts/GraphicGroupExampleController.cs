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
using UnityEngine.UI;
using Jigbox.Components;

namespace Jigbox.Examples
{
    public sealed class GraphicGroupExampleController : ExampleSceneBase
    {
#region properties

        [SerializeField]
        GraphicGroup group1 = null;

        [SerializeField]
        GraphicGroup group2 = null;

        [SerializeField]
        GraphicGroup group3 = null;

        [SerializeField]
        Image image1 = null;

        [SerializeField]
        Image image2 = null;

        [SerializeField]
        Image image3 = null;

        [SerializeField]
        Text text1 = null;

        [SerializeField]
        Text text2 = null;

        [SerializeField]
        Text text3 = null;

        [SerializeField]
        Color additiveColor = new Color(0.25f, 0.25f, 0.25f);

        [SerializeField]
        Color multiplyColor = Color.gray;

        [Range(0.0f, 1.0f)]
        [SerializeField]
        float alpha = 0.5f;
        
#endregion

#region private methods

        void UpdateText()
        {
            text1.text = string.Format("Group1\nR:{0} G:{1} B:{2} A:{3}",
                image1.color.r, image1.color.g, image1.color.b, image1.color.a);
            text2.text = string.Format("Group2\nR:{0} G:{1} B:{2} A:{3}",
                image2.color.r, image2.color.g, image2.color.b, image2.color.a);
            text3.text = string.Format("Group3\nR:{0} G:{1} B:{2} A:{3}",
                image3.color.r, image3.color.g, image3.color.b, image3.color.a);
        }

        [AuthorizedAccess]
        void SetAdditiveGroup1()
        {
            group1.SetColorAdditive(additiveColor);
            UpdateText();
        }

        [AuthorizedAccess]
        void SetAdditiveGroup2()
        {
            group2.SetColorAdditive(additiveColor);
            UpdateText();
        }

        [AuthorizedAccess]
        void SetAdditiveGroup3()
        {
            group3.SetColorAdditive(additiveColor);
            UpdateText();
        }

        [AuthorizedAccess]
        void SetMultiplyGroup1()
        {
            group1.SetColorMultiply(multiplyColor);
            UpdateText();
        }

        [AuthorizedAccess]
        void SetMultiplyGroup2()
        {
            group2.SetColorMultiply(multiplyColor);
            UpdateText();
        }

        [AuthorizedAccess]
        void SetMultiplyGroup3()
        {
            group3.SetColorMultiply(multiplyColor);
            UpdateText();
        }

        [AuthorizedAccess]
        void SetAdditiveAndAlphaGroup1()
        {
            group1.SetColorAdditive(additiveColor, alpha);
            UpdateText();
        }

        [AuthorizedAccess]
        void SetAdditiveAndAlphaGroup2()
        {
            group2.SetColorAdditive(additiveColor, alpha);
            UpdateText();
        }

        [AuthorizedAccess]
        void SetAdditiveAndAlphaGroup3()
        {
            group3.SetColorAdditive(additiveColor, alpha);
            UpdateText();
        }

        [AuthorizedAccess]
        void SetMultiplyAndAlphaGroup1()
        {
            group1.SetColorMultiply(multiplyColor, alpha);
            UpdateText();
        }

        [AuthorizedAccess]
        void SetMultiplyAndAlphaGroup2()
        {
            group2.SetColorMultiply(multiplyColor, alpha);
            UpdateText();
        }

        [AuthorizedAccess]
        void SetMultiplyAndAlphaGroup3()
        {
            group3.SetColorMultiply(multiplyColor, alpha);
            UpdateText();
        }

        [AuthorizedAccess]
        void ThroughAllGroup()
        {
            group1.SetThroughInChildren();
            UpdateText();
        }

        [AuthorizedAccess]
        void ThroughGroup2()
        {
            group2.SetThrough();
            UpdateText();
        }

        [AuthorizedAccess]
        void ThroughGroup3()
        {
            group3.SetThrough();
            UpdateText();
        }

        [AuthorizedAccess]
        void ResetGroup1()
        {
            group1.ResetColor();
            UpdateText();
        }

        [AuthorizedAccess]
        void ResetGroup2()
        {
            group2.ResetColor();
            UpdateText();
        }

        [AuthorizedAccess]
        void ResetGroup3()
        {
            group3.ResetColor();
            UpdateText();
        }

#endregion

#region override unity methods

        protected override void Awake()
        {
            base.Awake();
            UpdateText();
        }

#endregion
    }
}
