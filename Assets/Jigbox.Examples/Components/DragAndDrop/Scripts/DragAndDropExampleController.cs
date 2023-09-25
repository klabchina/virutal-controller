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
using System.Collections.Generic;

namespace Jigbox.Examples
{
    public sealed class DragAndDropExampleController : ExampleSceneBase
    {
#region properties

        [SerializeField]
        DragAndDropCardListController listController = null;

        [SerializeField]
        DragAndDropDeckInfo deckInfo = null;

        [SerializeField]
        List<DragAndDropSelectedCardController> selectedCards = new List<DragAndDropSelectedCardController>();

#endregion

#region private methods

        [AuthorizedAccess]
        void UpdateDeckInfo()
        {
            deckInfo.Apply();
        }

        [AuthorizedAccess]
        void TemporaryUpdateDeckInfo()
        {
            int cost = 0;
            int hp = 0;
            int attack = 0;

            foreach (DragAndDropSelectedCardController selected in selectedCards)
            {
                DragAndDropExampleCard card = selected.Card;
                if (card != null)
                {
                    cost += card.Cost;
                    hp += card.Hp;
                    attack += card.Attack;
                }
            }

            deckInfo.SetTempolary(cost, hp, attack);
        }

        [AuthorizedAccess]
        void CancelTemporary()
        {
            deckInfo.Cancel();
        }

#endregion

#region override unity methods

        void Start()
        {
            listController.Init();
            deckInfo.Init();
        }

#endregion
    }
}
