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
using Jigbox.Components;

namespace Jigbox.Examples
{
    public sealed class DragAndDropCardListController : MonoBehaviour
    {
#region inner classes, enum, and structs

        class CellItemInfo
        {
            public int Id { get; private set; }

            public DragAndDropExampleCard Card { get; private set; }

            public CellItemInfo(int id, DragAndDropExampleCard card)
            {
                Id = id;
                Card = card;
            }
        }

#endregion

#region properties

        [SerializeField]
        TileViewHorizontal tileView = null;

        [SerializeField]
        RectTransform dragArea = null;

        List<CellItemInfo> cards = new List<CellItemInfo>();

#endregion

#region public methods

        public void Init()
        {
            for (int i = 0; i < DragAndDropExampleCard.CardData.Length; ++i)
            {
                cards.Add(new CellItemInfo(i, DragAndDropExampleCard.CardData[i]));
            }
            tileView.VirtualCellCount = cards.Count;
            tileView.AddUpdateEvent(UpdateCell);
            tileView.ReserveRefreshAllCells(null, CreateCell, false);
        }

        public void Deselected(int id, DragAndDropExampleCard card)
        {
            cards.Add(new CellItemInfo(id, card));

            tileView.VirtualCellCount = cards.Count;
            tileView.RefreshAllCells(CreateCell);
        }

#endregion

#region private methods

        void Selected(DraggableEventData eventData)
        {
            if (eventData.Droppable == null)
            {
                return;
            }

            DragAndDropCellItemController controller = eventData.Draggable.GetComponent<DragAndDropCellItemController>();
            for (int i = 0; i < cards.Count; ++i)
            {
                if (cards[i].Id == controller.Id)
                {
                    cards.RemoveAt(i);
                    break;
                }
            }

            tileView.VirtualCellCount = cards.Count;
            tileView.RefreshAllCells(CreateCell);
        }

        GameObject CreateCell()
        {
            GameObject gameObject = Instantiate(Resources.Load<GameObject>("DragAndDropCardPrefab"));
            DragAndDropCellItemController controller = gameObject.GetComponent<DragAndDropCellItemController>();
            controller.Draggable.DragArea = dragArea;
            controller.Draggable.AddEndDragEvent(Selected);

            return gameObject;
        }

        void UpdateCell(GameObject cell, int index)
        {
            DragAndDropCellItemController controller = cell.GetComponent<DragAndDropCellItemController>();
            controller.Id = cards[index].Id;
            controller.CardView.Card = cards[index].Card;
        }

#endregion
    }
}
