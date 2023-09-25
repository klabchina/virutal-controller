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
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

namespace Jigbox.Examples
{
    public sealed class SceneListItemController : MonoBehaviour
    {
#region properties

        [SerializeField]
        Text text = null;

        [SerializeField]
        Image bg = null;

        [SerializeField]
        Image selected = null;

        [SerializeField]
        GameObject lastSelect = null;

        int index;

        public Action<int> OnClickHandler { get; set; }

        static List<int> selectedIndices = new List<int>();

        static Sprite toggleRect;

        static Sprite toggleChecked;

#endregion

#region public methods

        public void SetText(string text, int index, bool isLastSelect)
        {
            this.text.text = text;
            this.index = index;
            bg.color = index % 2 == 0 ? Color.white : new Color(0.8f, 1.0f, 1.0f);
            selected.sprite = selectedIndices.Contains(index) ? toggleChecked : toggleRect;
            lastSelect.SetActive(isLastSelect);
        }

#endregion

#region private methods

        [AuthorizedAccess]
        void OnClick()
        {
            if (OnClickHandler != null)
            {
                OnClickHandler(index);
            }
            if (!selectedIndices.Contains(index))
            {
                selectedIndices.Add(index);
            }
            SceneTransition.StackableSceneManager.Instance.SetPassingData("IndexInputSystem", InputWrapper.IsInputSystem);
            SceneManager.LoadScene(text.text);
        }

#endregion

#region override unity methods

        void Awake()
        {
            if (toggleRect == null)
            {
                toggleRect = Resources.Load<Sprite>("Sprites/Toggle-Rect");
            }
            if (toggleChecked == null)
            {
                toggleChecked = Resources.Load<Sprite>("Sprites/Toggle-Checked");
            }
        }

#endregion
    }
}
