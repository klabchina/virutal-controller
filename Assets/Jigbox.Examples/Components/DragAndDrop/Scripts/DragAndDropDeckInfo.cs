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

namespace Jigbox.Examples
{
    public sealed class DragAndDropDeckInfo : MonoBehaviour
    {
#region properties

        [SerializeField]
        Text cost = null;

        [SerializeField]
        Text hp = null;

        [SerializeField]
        Text attack = null;

        int costValue = 0;

        int tempCostValue = 0;

        int hpValue = 0;

        int tempHpValue = 0;

        int attackValue = 0;

        int tempAttackValue = 0;

#endregion

#region public methods

        public void Init()
        {
            UpdateText(false);
        }

        public void SetTempolary(int cost, int hp, int attack)
        {
            tempCostValue = cost;
            tempHpValue = hp;
            tempAttackValue = attack;

            UpdateText(true);
        }

        public void Cancel()
        {
            UpdateText(false);
        }

        public void Apply()
        {
            costValue = tempCostValue;
            hpValue = tempHpValue;
            attackValue = tempAttackValue;

            UpdateText(false);
        }

#endregion

#region private methods

        void UpdateText(bool isTemporary)
        {
            cost.text = isTemporary ? tempCostValue.ToString() : costValue.ToString();
            if (isTemporary)
            {
                cost.color = tempCostValue == costValue ? Color.white : tempCostValue > costValue ? Color.green : Color.red;
            }
            else
            {
                cost.color = Color.white;
            }

            hp.text = isTemporary ? tempHpValue.ToString() : hpValue.ToString();
            if (isTemporary)
            {
                hp.color = tempHpValue == hpValue ? Color.white : tempHpValue > hpValue ? Color.green : Color.red;
            }
            else
            {
                hp.color = Color.white;
            }

            attack.text = isTemporary ? tempAttackValue.ToString() : attackValue.ToString();
            if (isTemporary)
            {
                attack.color = tempAttackValue == attackValue ? Color.white : tempAttackValue > attackValue ? Color.green : Color.red;
            }
            else
            {
                attack.color = Color.white;
            }
        }

#endregion
    }
}
