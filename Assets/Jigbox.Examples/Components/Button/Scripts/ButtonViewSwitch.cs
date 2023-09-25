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
using Jigbox.Components;

namespace Jigbox.Examples
{
    public sealed class ButtonViewSwitch : MonoBehaviour
    {
#region properties

        [SerializeField]
        ButtonBase button1 = null;

        [SerializeField]
        ButtonBase button2 = null;

#endregion

#region private methods

        [AuthorizedAccess]
        void OnSwitch()
        {
            button1.gameObject.SetActive(!button1.gameObject.activeSelf);
            button2.gameObject.SetActive(!button2.gameObject.activeSelf);
        }

#endregion

#region override unity methods

        void Start()
        {
            button1.gameObject.SetActive(true);
            button2.gameObject.SetActive(false);
        }

#endregion
    }
}
