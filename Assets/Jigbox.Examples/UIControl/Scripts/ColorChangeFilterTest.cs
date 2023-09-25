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
using System.Reflection;

namespace Jigbox.Examples
{
    /// <summary>
    /// ColorChangeFilterによる
    /// 親コンポーネントのカラー変更への制御のサンプルです.
    /// </summary>
    public class ColorChangeFilterTest : ExampleSceneBase
    {
        [AuthorizedAccess]
        bool invalidInChildren = true;

        [AuthorizedAccess]
        bool clickable = true;

        [SerializeField]
        ColorChangeFilter animRoot = null;

        [SerializeField]
        BasicButton button = null;

        [SerializeField]
        Text validateInChildText = null;

        [SerializeField]
        Text clickableText = null;

        void Start()
        {
            OnChangeInvalidInChildren(!invalidInChildren);
            OnChangeClickable(!clickable);
        }

        [AuthorizedAccess]
        void OnChangeInvalidInChildren(bool invalidInChildren)
        {
            clickable = true;
            button.Clickable = true;
            button.ImageInfo.ResetColor();
            clickableText.text = 
                string.Format("change\nclickable:<color=\"red\">{0}</color>", this.clickable);

            this.invalidInChildren = !invalidInChildren;
            var refrectInfo = animRoot.GetType().GetField(
                                  "invalidInChildren", 
                                  BindingFlags.NonPublic | BindingFlags.Instance);
            refrectInfo.SetValue(animRoot, this.invalidInChildren);

            refrectInfo = button.GetType().GetField(
                "imageInfo", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            refrectInfo.SetValue(button, null);

            validateInChildText.text = 
                string.Format("change\ninvalid in children:<color=\"red\">{0}</color>", this.invalidInChildren);
        }

        [AuthorizedAccess]
        void OnChangeClickable(bool clickable)
        {
            this.clickable = !clickable;
            button.Clickable = this.clickable;

            clickableText.text = 
                string.Format("change\nclickable:<color=\"red\">{0}</color>", this.clickable);
        }
    }
}
