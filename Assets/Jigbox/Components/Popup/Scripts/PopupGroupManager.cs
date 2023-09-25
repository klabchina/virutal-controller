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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jigbox.Components
{
    public class PopupGroupManager : PopupManagerBase
    {
#region properties

        /// <summary>
        /// デフォルトで使用されるグループ名
        /// </summary>
        public static readonly string DefaultGroupName = "PopupGroup";

        protected static PopupGroupManager instance;

        public static PopupGroupManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PopupGroupManager();
                }

                return instance;
            }
        }

        /// <summary>
        /// 配置順を厳密に持ったViewのリスト
        /// 最前面のViewを取得する場合などに使用します
        /// </summary>
        LinkedList<PopupGroupView> alignViews = new LinkedList<PopupGroupView>();

        /// <summary>
        /// シーン名 + グループ名の文字列をkeyとして持つViewの辞書
        /// </summary>
        Dictionary<string, PopupGroupView> views = new Dictionary<string, PopupGroupView>();

#endregion

#region constructor & public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected PopupGroupManager()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        /// <summary>
        /// ポップアップをグループ名を指定して開きます
        /// グループが存在しない場合新たにPopupViewを生成します
        /// グループが存在し、既に開いているポップアップがある場合は、ポップアップが閉じてから開かれます
        /// </summary>
        /// <param name="order">オーダー。PopupGroupOrder 型ではない場合規定のグループとしてポップアップを開きます</param>
        public override void OpenQueue(PopupOrder order)
        {
            string groupName;

            var groupOrder = order as IPopupGroupOrder;
            if (groupOrder != null)
            {
                groupName = groupOrder.GroupName;
            }
            else
            {
                groupName = DefaultGroupName;
#if UNITY_EDITOR
                Debug.LogWarning("Need to use PopupGroupOrder. Could not get group name. Use default group name.");
#endif
            }

            PopupGroupView view = GetView(groupName);
            if (view != null)
            {
                view.OpenQueue(order);
            }
        }

        /// <summary>
        /// ポップアップをグループ名を指定して開きます
        /// グループが存在しない場合新たにPopupViewを生成します
        /// グループが存在し、既に開いているポップアップがある場合は最前面に表示されます
        /// </summary>
        /// <param name="order">オーダー。PopupGroupOrder 型ではない場合規定のグループとしてポップアップを開きます</param>
        public override void Open(PopupOrder order)
        {
            string groupName;

            var groupOrder = order as IPopupGroupOrder;
            if (groupOrder != null)
            {
                groupName = groupOrder.GroupName;
            }
            else
            {
                groupName = DefaultGroupName;
#if UNITY_EDITOR
                Debug.LogWarning("Need to use PopupGroupOrder. Could not get group name. Use default group name.");
#endif
            }
            
            PopupGroupView view = GetView(groupName);
            if (view != null)
            {
                view.Open(order);
            }
        }

        /// <summary>
        /// グループ名を指定し、前面に出ているポップアップを閉じます。
        /// </summary>
        /// <param name="groupName">グループ名</param>
        public virtual void Close(string groupName)
        {
            PopupGroupView view = GetView(groupName);
            if (view != null)
            {
                view.Close();
            }
        }

        /// <summary>
        /// グループ名を指定し、ポップアップを全て閉じます
        /// </summary>
        /// <param name="groupName">グループ名</param>
        public virtual void CloseAll(string groupName)
        {
            PopupGroupView view = GetView(groupName);
            if (view != null)
            {
                view.CloseAll();
            }
        }

        /// <summary>
        /// 現在生成されているPopupViewにキャッシュされているポップアップの表示オーダーを全て破棄します
        /// </summary>
        public virtual void ClearAllViewOrder()
        {
            foreach (var view in views.Values)
            {
                view.ClearOrder();
            }
        }

#endregion

#region override public methods

        public override void Close()
        {
            // GetViewで新たにViewが作成されるため、Viewが存在しない場合は処理を弾く
            if (views.Count == 0)
            {
                return;
            }

            base.Close();
        }

        public override void CloseAll()
        {
            if (views.Count == 0)
            {
                return;
            }

            base.CloseAll();
        }

        public override void ClearOrder()
        {
            if (views.Count == 0)
            {
                return;
            }

            base.ClearOrder();
        }

        public override bool IsOpenPopup()
        {
            if (views.Count == 0)
            {
                return false;
            }

            return base.IsOpenPopup();
        }

        public override bool IsStandbyPopup()
        {
            if (views.Count == 0)
            {
                return false;
            }

            return base.IsStandbyPopup();
        }

#endregion

#region protected methods

        /// <summary>
        /// 現在最前面に表示されているViewを取得する
        /// Viewが存在しない場合、デフォルト名のグループとして新たにViewを作成する
        /// </summary>
        /// <returns>View</returns>
        protected override PopupView GetView()
        {
            if (views.Count == 0)
            {
                return GetView(DefaultGroupName);
            }

            return alignViews.Last.Value;
        }

        /// <summary>
        /// 指定したグループ名のViewを取得する
        /// Viewが存在しない場合は新たに作成する
        /// </summary>
        /// <param name="groupName">グループ名</param>
        /// <returns>View</returns>
        protected virtual PopupGroupView GetView(string groupName)
        {
            PopupGroupView view;

            if (!views.TryGetValue(groupName, out view))
            {
                if (viewProvider == null)
                {
#if UNITY_EDITOR
                    Debug.LogError("PopupGroupManager.GetView : Can't create PopupView because not exist provider!");
#endif
                    return null;
                }

                view = viewProvider.Generate() as PopupGroupView;

                // 取得したViewがPopupGroupViewでない場合エラーをだす
                if (view == null)
                {
#if UNITY_EDITOR
                    Debug.LogError(
                        "PopupGroupManager.GetView : Can't cast to PopupGroupView from ViewProvider.Generate() provided instance");
#endif
                    return null;
                }

                view.gameObject.SetActive(false);
                view.SetDisposer(this);
                view.OnDestoryCallback = OnDestroyPopupView;
                view.GroupName = groupName;
                view.MoveToFront();

                // Viewが既に存在する場合、最前面のViewをBackに移動させる処理を行う
                if (views.Count > 0)
                {
                    var frontView = alignViews.Last.Value;
                    frontView.MoveToBack();

                    // BackKeyの優先度を設定するためにPopupの数をViewに入れ込む
                    var totalPopupCount = frontView.BackViewPopupCount + frontView.PopupCount;
                    view.BackViewPopupCount = totalPopupCount;
                }

                views.Add(groupName, view);
                alignViews.AddLast(view);
            }
#if UNITY_EDITOR
            else if (!view.IsFront)
            {
                // 指定したグループが存在し、かつ最前面でない場合はエラーをだす
                Debug.LogError("This group is not place on front, Do not control.");
                return null;
            }
#endif

            return view;
        }

        /// <summary>
        /// PopupViewが破棄された際に呼ばれます
        /// PopupViewの参照の破棄と表示順変更処理を行います
        /// </summary>
        /// <param name="key">Viewのキー</param>
        protected virtual void OnDestroyPopupView(string key)
        {
            if (views.ContainsKey(key))
            {
                var view = views[key];

                views.Remove(key);
                alignViews.Remove(view);

                // 最前面のViewが破棄された場合はViewの入れ替え処理を行う
                if (view.IsFront && alignViews.Count != 0)
                {
                    var backView = alignViews.Last.Value;
                    backView.MoveToFront();
                }
            }
        }

        /// <summary>
        /// シーン遷移時に現在生成されているViewのアクティブを切り替えます
        /// </summary>
        /// <param name="beforeScene">遷移前のシーン</param>
        /// <param name="nextScene">遷移後のシーン</param>
        protected virtual void OnActiveSceneChanged(Scene beforeScene, Scene nextScene)
        {
            foreach (var view in views.Values)
            {
                if (view.BelongSceneName == nextScene.name)
                {
                    view.gameObject.SetActive(true);
                }
                else if (view.isActiveAndEnabled)
                {
                    view.gameObject.SetActive(false);
                }
            }
        }

#endregion
    }
}
