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
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public class AdvSelectManager : MonoBehaviour
    {
#region properties
        
        /// <summary>UI管理コンポーネント</summary>
        protected AdvUIManager uiManager;

        /// <summary>選択肢</summary>
        protected List<AdvSelectItemController> items = new List<AdvSelectItemController>();

        /// <summary>選択肢が選択された際のコールバック</summary>
        protected Action onSelectCallback = null;

#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="uiManager">UI制御コンポーネント</param>
        public virtual void Init(AdvUIManager uiManager)
        {
            this.uiManager = uiManager;
        }

        /// <summary>
        /// 初期化前の状態に戻します。
        /// </summary>
        public virtual void Uninit()
        {
            uiManager = null;
            onSelectCallback = null;
            ClearSelectItems();
        }

        /// <summary>
        /// 選択肢が選択された際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">選択肢が選択された際に呼び出されるコールバック</param>
        public void AddSelectCallback(Action callback)
        {
            if (onSelectCallback == null)
            {
                onSelectCallback = callback;
            }
            else
            {
                onSelectCallback += callback;
            }
        }

        /// <summary>
        /// 選択肢が選択された際に呼び出されます。
        /// </summary>
        public void OnSelect()
        {
            ClearSelectItems();
            gameObject.SetActive(false);

            if (onSelectCallback != null)
            {
                onSelectCallback();
            }
        }

        /// <summary>
        /// 選択肢を追加します。
        /// </summary>
        /// <param name="item"></param>
        public void AddSelect(AdvSelectItemController item)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            item.SetManager(this);
            items.Add(item);
            item.gameObject.transform.SetParent(transform, false);
        }

#endregion

#region protected methods

        /// <summary>
        /// 選択肢をクリアします。
        /// </summary>
        protected void ClearSelectItems()
        {
            foreach (AdvSelectItemController item in items)
            {
                Destroy(item.gameObject);
            }
            items.Clear();
        }

#endregion
    }
}
