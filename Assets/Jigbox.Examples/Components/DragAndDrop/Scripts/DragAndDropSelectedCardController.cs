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
using System;
using Jigbox.Components;

namespace Jigbox.Examples
{
    public sealed class DragAndDropSelectedCardController : MonoBehaviour
    {
#region properties

        [SerializeField]
        DragAndDropCardListController listController = null;

        [SerializeField]
        DragAndDropCardView cardView = null;

        int id;

        DragAndDropExampleCard tempCard;

        public DragAndDropExampleCard Card { get { return tempCard == null ? cardView.Card : tempCard; } }

#endregion

#region private methods

        [AuthorizedAccess]
        void OnEnter(DroppableEventData eventData)
        {
            DragAndDropCellItemController controller = eventData.Draggable.GetComponent<DragAndDropCellItemController>();
            tempCard = controller.CardView.Card;
        }

        [AuthorizedAccess]
        void OnExit(DroppableEventData eventData)
        {
            tempCard = null;
        }

        [AuthorizedAccess]
        void OnDrop(DroppableEventData eventData)
        {
            if (!cardView.gameObject.activeSelf)
            {
                cardView.gameObject.SetActive(true);
            }
            else
            {
                listController.Deselected(id, cardView.Card);
            }

            DragAndDropCellItemController controller = eventData.Draggable.GetComponent<DragAndDropCellItemController>();
            id = controller.Id;
            cardView.Card = controller.CardView.Card;
            tempCard = null;
        }

#endregion
    }
}
