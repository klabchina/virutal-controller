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
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    using TextInfo = AdvWindowTextController.TextInfo;

    public class AdvBacklogManager : MonoBehaviour
    {
#region properties

        /// <summary>スクロールバー</summary>
        [SerializeField]
        protected Scrollbar scrollbar;

        /// <summary>スクロールビューのコンテンツ領域</summary>
        [SerializeField]
        protected GameObject contentContainer;

        /// <summary>非表示状態のリストアイテムを格納しておく領域</summary>
        [SerializeField]
        protected GameObject hideItemContainer;

        /// <summary>ログをクリアしないようにするかどうか</summary>
        [SerializeField]
        private bool dontClearLog = false;

        /// <summary>UI管理コンポーネント</summary>
        protected AdvUIManager uiManager;

        /// <summary>表示しているバックログのリストアイテム</summary>
        protected List<AdvBacklogItemController> items = new List<AdvBacklogItemController>();

        /// <summary>非表示状態のバックログのリストアイテム</summary>
        protected List<AdvBacklogItemController> hideItems = new List<AdvBacklogItemController>();

        /// <summary>アイテム数</summary>
        protected int ItemCount { get { return items.Count + hideItems.Count; } }

        /// <summary>バックログが表示されているかどうか</summary>
        public virtual bool IsShow { get { return gameObject.activeSelf; } }

        /// <summary>スクロール量を再設定するかどうか</summary>
        protected bool isRefreshScroll = false;

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

            if (dontClearLog)
            {
                return;
            }

            for (int i = 0; i < items.Count; ++i)
            {
                Destroy(items[i].gameObject);
            }
            items.Clear();
            for (int i = 0; i < hideItems.Count; ++i)
            {
                Destroy(hideItems[i].gameObject);
            }
            hideItems.Clear();
        }

        /// <summary>
        /// バックログに表示するリストアイテムを生成します。
        /// </summary>
        /// <param name="loader">Loader</param>
        /// <param name="resourcePath">リソースのパス</param>
        /// <param name="length">バックログの長さ</param>
        public virtual void CreateItems(IAdvResourceLoader loader, string resourcePath, int length)
        {
            if (items.Count > 0)
            {
                return;
            }

            GameObject item = loader.Load<GameObject>(resourcePath);
            AdvBacklogItemController itemController = item.GetComponent<AdvBacklogItemController>();
            if (itemController == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("対象リソースにバックログのアイテム用制御コンポーネントが存在しません。");
#endif
                return;
            }

            for (int i = 0; i < length; ++i)
            {
                GameObject obj = Instantiate(item) as GameObject;
                obj.transform.SetParent(hideItemContainer.transform, false);
                AdvBacklogItemController controller = obj.GetComponent<AdvBacklogItemController>();
                controller.Init(this);
                hideItems.Add(controller);
            }
        }

        /// <summary>
        /// バックログを表示します。
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
            isRefreshScroll = true;
        }

        /// <summary>
        /// バックログを非表示にします。
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// テキストを設定します。
        /// </summary>
        /// <param name="info">テキスト情報</param>
        public virtual void SetText(TextInfo info)
        {
            if (hideItems.Count > 0)
            {
                AddAndSetInfo(info);
            }
            else
            {
                SetInfo(info);
            }
        }

        /// <summary>
        /// リストアイテムのリピートボタンが押された際に、アイテム側から呼び出されます。
        /// </summary>
        /// <param name="sound"></param>
        public virtual void RepeatSound(string sound)
        {
            uiManager.RepeatSound(sound);
        }

#endregion

#region protected methods

        /// <summary>
        /// 非表示状態のアイテムをコンテンツ領域に移動して、テキスト情報を設定します。
        /// </summary>
        /// <param name="info"></param>
        protected void AddAndSetInfo(TextInfo info)
        {
            AdvBacklogItemController item = hideItems[0];
            hideItems.RemoveAt(0);

            item.SetText(info);
            items.Add(item);
            item.transform.SetParent(contentContainer.transform, false);
            item.transform.SetAsLastSibling();
        }

        /// <summary>
        /// コンテンツ領域に表示されているアイテムの内、最も古いものにテキスト情報を設定します。
        /// </summary>
        /// <param name="info"></param>
        protected void SetInfo(TextInfo info)
        {
            foreach (AdvBacklogItemController item in items)
            {
                if (item.transform.GetSiblingIndex() == 0)
                {
                    item.SetText(info);
                    item.transform.SetAsLastSibling();
                    break;
                }
            }
        }

#endregion

#region override unity methods

        void Update()
        {
            // 表示切替直後にスクロール量を設定した場合、
            // コンテンツ領域のサイズの再計算に上書きされて値が設定できないため
            // 1フレーム分遅延させるためにフラグで処理
            if (isRefreshScroll)
            {
                if (scrollbar != null)
                {
                    scrollbar.value = 0.0f;
                }
                isRefreshScroll = false;
            }
        }

#endregion
    }
}
