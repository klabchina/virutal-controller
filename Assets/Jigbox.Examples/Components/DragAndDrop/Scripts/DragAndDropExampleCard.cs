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

namespace Jigbox.Examples
{
    public sealed class DragAndDropExampleCard
    {
#region constants

        public static DragAndDropExampleCard[] CardData = new DragAndDropExampleCard[]
        {
            new DragAndDropExampleCard(1, 1, 1),
            new DragAndDropExampleCard(1, 2, 1),
            new DragAndDropExampleCard(1, 2, 1),
            new DragAndDropExampleCard(1, 1, 1),
            new DragAndDropExampleCard(1, 1, 1),
            new DragAndDropExampleCard(1, 1, 1),
            new DragAndDropExampleCard(1, 1, 1),
            new DragAndDropExampleCard(1, 1, 2),
            new DragAndDropExampleCard(1, 1, 2),
            new DragAndDropExampleCard(1, 1, 2),
            new DragAndDropExampleCard(2, 3, 2),
            new DragAndDropExampleCard(2, 2, 2),
            new DragAndDropExampleCard(2, 1, 4),
            new DragAndDropExampleCard(2, 1, 3),
            new DragAndDropExampleCard(2, 2, 2),
            new DragAndDropExampleCard(2, 4, 1),
            new DragAndDropExampleCard(2, 3, 1),
            new DragAndDropExampleCard(2, 1, 2),
            new DragAndDropExampleCard(2, 1, 2),
            new DragAndDropExampleCard(2, 2, 3),
            new DragAndDropExampleCard(2, 2, 3),
            new DragAndDropExampleCard(2, 2, 3),
            new DragAndDropExampleCard(3, 4, 2),
            new DragAndDropExampleCard(3, 3, 2),
            new DragAndDropExampleCard(3, 3, 2),
            new DragAndDropExampleCard(3, 3, 4),
            new DragAndDropExampleCard(3, 5, 1),
            new DragAndDropExampleCard(3, 1, 5),
            new DragAndDropExampleCard(3, 2, 4),
            new DragAndDropExampleCard(3, 3, 3),
            new DragAndDropExampleCard(4, 5, 4),
            new DragAndDropExampleCard(4, 6, 3),
            new DragAndDropExampleCard(4, 8, 1),
            new DragAndDropExampleCard(4, 2, 5),
            new DragAndDropExampleCard(4, 3, 4),
            new DragAndDropExampleCard(4, 4, 4),
            new DragAndDropExampleCard(5, 7, 6),
            new DragAndDropExampleCard(5, 5, 9),
            new DragAndDropExampleCard(5, 9, 5),
            new DragAndDropExampleCard(5, 6, 8),
            new DragAndDropExampleCard(5, 6, 6),
            new DragAndDropExampleCard(5, 7, 8),
        };

#endregion

#region properties

        public int Cost { get; private set; }

        public int Hp { get; private set; }

        public int Attack { get; private set; }

#endregion

#region public methods

        public DragAndDropExampleCard(int cost, int hp, int attack)
        {
            Cost = cost;
            Hp = hp;
            Attack = attack;
        }

#endregion
    }
}
